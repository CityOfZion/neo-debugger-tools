using Neo.VM;
using Neo.Lux.Cryptography;
using System.Collections.Generic;
using System;
using System.Numerics;
using Neo.Lux.Utils;
using Neo.Emulation.API;
using LunarLabs.Parser;
using Neo.Lux.Core;
using Account = Neo.Emulation.API.Account;
using ScriptBuilder = Neo.Lux.Utils.ScriptBuilder;
using Neo.Emulation.Utils;

namespace Neo.Emulation
{
    public enum CheckWitnessMode
    {
        Default,
        AlwaysTrue,
        AlwaysFalse
    }

    public struct DebuggerState
    {
        public enum State
        {
            Invalid,
            Reset,
            Running,
            Finished,
            Exception,
            Break
        }

        public readonly State state;
        public readonly int offset;

        public DebuggerState(State state, int offset)
        {
            this.state = state;
            this.offset = offset;
        }
    }

    public static class NeoEmulatorExtensions
    {
        public static Emulator GetEmulator(this ExecutionEngine engine)
        {
            var tx  = (API.Transaction)engine.ScriptContainer;
            return tx.emulator;
        }

        public static Account GetAccount(this ExecutionEngine engine)
        {
            var emulator = engine.GetEmulator();
            return emulator.currentAccount;
        }

        public static Blockchain GetBlockchain(this ExecutionEngine engine)
        {
            var emulator = engine.GetEmulator();
            return emulator.blockchain;
        }

        public static API.Storage GetStorage(this ExecutionEngine engine)
        {
            var emulator = engine.GetEmulator();
            return emulator.currentAccount.storage;
        }
    }

    public class Emulator 
    {
        public enum Type
        {
            Unknown,
            String,
            Boolean,
            Integer,
            Array,
            ByteArray
        }

        public class Variable
        {
            public StackItem value;
            public string name;
            public Type type;
        }

        public struct Assignment
        {
            public string name;
            public Type type;
        }

        public struct EmulatorStepInfo
        {
            public byte[] byteCode;
            public int offset;
            public OpCode opcode;
            public decimal gasCost;
            public string sysCall;
        }

        private ExecutionEngine engine;
        public byte[] ContractByteCode { get; private set; }

        private InteropService interop;

        private HashSet<int> _breakpoints = new HashSet<int>();
        public IEnumerable<int> Breakpoints { get { return _breakpoints; } }

        public Blockchain blockchain { get; private set; }

        private DebuggerState lastState = new DebuggerState(DebuggerState.State.Invalid, -1);

        public Account currentAccount { get; private set; }
        public API.Transaction currentTransaction { get; private set; }
        public Crypto crypto { get; private set; }

        public string currentMethod { get; private set; }

        private UInt160 currentHash;

        public CheckWitnessMode checkWitnessMode = CheckWitnessMode.Default;
        public TriggerType currentTrigger = TriggerType.Application;
        public uint timestamp = DateTime.Now.ToTimestamp();

        public decimal usedGas { get; private set; }
        public int usedOpcodeCount { get; private set; }

        public Action<EmulatorStepInfo> OnStep;

        public Emulator(Blockchain blockchain)
        {
            this.blockchain = blockchain;
            this.interop = new InteropService();            
        }

        public int GetInstructionPtr()
        {
            return engine.CurrentContext.InstructionPointer;
        }

        public void SetExecutingAccount(Account address)
        {
            this.currentAccount = address;
            this.ContractByteCode = address.byteCode;
        }

        private int lastOffset = -1;

        private ABI _ABI;

        public void Reset(byte[] inputScript, ABI ABI, string methodName, IScriptTable table = null)
        {
            if (ContractByteCode == null || ContractByteCode.Length == 0)
            {
                throw new Exception("Contract bytecode is not set yet!");
            }

            if (lastState.state == DebuggerState.State.Reset)
            {
                return;
            }

            if (currentTransaction == null)
            {
                //throw new Exception("Transaction not set");
                currentTransaction = new API.Transaction(this.blockchain.currentBlock);
            }

            if(crypto == null)
            {
                crypto = new Crypto();
            }

            usedGas = 0;
            usedOpcodeCount = 0;

            currentTransaction.emulator = this;
            engine = new ExecutionEngine(currentTransaction, null, table, interop);
            engine.LoadScript(ContractByteCode);
            engine.LoadScript(inputScript);

            this.currentMethod = methodName;

            /*foreach (var output in currentTransaction.outputs)
            {
                if (output.hash == this.currentHash)
                {
                    output.hash = engine.CurrentContext.ScriptHash;
                }
            }*/

            foreach (var pos in _breakpoints)
            {
                engine.AddBreakPoint(ContractByteCode,(uint)pos);
            }

            //engine.Reset();

            lastState = new DebuggerState(DebuggerState.State.Reset, 0);
            currentTransaction = null;

            _variables.Clear();
            this._ABI = ABI;
        }

        public byte[] GenerateLoaderScriptFromInputs(DataNode inputs, ABI abi)
        {
            var methodName = abi != null && abi.entryPoint != null ? abi.entryPoint.name : null;

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                var items = new Stack<object>();

                if (inputs != null)
                {
                    AVMFunction method = methodName != null && abi.functions.ContainsKey(methodName) ? abi.functions[methodName] : null;

                    int index = 0;
                    foreach (var item in inputs.Children)
                    {
                        Emulator.Type hint = method != null ? method.inputs[index].type : Emulator.Type.Unknown;

                        var obj = Emulator.ConvertArgument(item, hint);
                        
                        items.Push(obj);

                        index++;
                    }
                }

                while (items.Count > 0)
                {
                    var item = items.Pop();
                    NeoAPI.EmitObject(sb, item);
                }

                var loaderScript = sb.ToArray();
                //System.IO.File.WriteAllBytes("loader.avm", loaderScript);

                return loaderScript;
            }
        }

        public void SetBreakpointState(int ofs, bool enabled)
        {
            if (enabled)
            {
                _breakpoints.Add(ofs);
            }
            else
            {
                _breakpoints.Remove(ofs);
            }
        }

        public bool GetRunningState()
        {
            return !engine.State.HasFlag(VMState.HALT) && !engine.State.HasFlag(VMState.FAULT) && engine.State.HasFlag(VMState.BREAK);
        }

        private bool ExecuteSingleStep()
        {
            if (this.lastState.state == DebuggerState.State.Reset)
            {
                var initialContext = engine.CurrentContext;
                while (engine.CurrentContext == initialContext)
                {
                    engine.StepInto();
                }

                if (this._ABI != null && _ABI.entryPoint != null)
                {
                    int index = 0;
                    foreach (var entry in _ABI.entryPoint.inputs)
                    {
                        try
                        {
                            var context = engine.CurrentContext;
                            var val = context.EvaluationStack.Peek(index);

                            var varType = entry.type;

                            // if the type is unknown we can always check if the type was known in a previous assigment
                            var prevVal = GetVariable(entry.name);
                            if (varType == Type.Unknown && prevVal != null)
                            {
                                varType = prevVal.type;
                            }

                            _variables[entry.name] = new Variable() { value = val, type = varType, name = entry.name };

                            index++;
                        }
                        catch
                        {
                            break;
                        }
                    }
                }

                return true;
            }

            var shouldContinue = GetRunningState();
            if (shouldContinue)
            {
                engine.StepInto();

                if (engine.State == VMState.NONE)
                {
                    int currentOffset = engine.CurrentContext.InstructionPointer;

                    if (_assigments.ContainsKey(currentOffset))
                    {
                        var ass = _assigments[currentOffset];
                        try
                        {
                            var context = engine.CurrentContext;
                            var val = context.EvaluationStack.Peek();
                            _variables[ass.name] =  new Variable() { value = val, type = ass.type, name = ass.name };
                        }
                        catch
                        {
                            // ignore for now
                        }
                    }
                }

                return GetRunningState();
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// executes a single instruction in the current script, and returns the last script offset
        /// </summary>
        public DebuggerState Step()
        {
            if (lastState.state == DebuggerState.State.Finished || lastState.state == DebuggerState.State.Invalid)
            {
                return lastState;
            }

            var opcode = engine.CurrentContext.NextInstruction; // lastOpcode
            ExecuteSingleStep();
            // is not getting the real last opcode

            try
            {
                lastOffset = engine.CurrentContext.InstructionPointer;
                if(lastOffset == 0) opcode = OpCode.RET; // hack to fix the last opcode

                //var opcode = engine.lastOpcode;
                decimal opCost;

                if (opcode <= OpCode.PUSH16)
                {
                    opCost = 0;
                }
                else
                    switch (opcode)
                    {
                        case OpCode.SYSCALL:
                            // TODO
                            //{
                            //    var callInfo = interop.FindCall(engine.lastSysCall);
                            //    opCost = (callInfo != null) ? callInfo.gasCost : 0;

                            //    if (engine.lastSysCall.EndsWith("Storage.Put"))
                            //    {
                            //        opCost *= (API.Storage.lastStorageLength / 1024.0m);
                            //        if (opCost < 1) opCost = 1;
                            //    }
                            //    break;
                            //}

                        case OpCode.CHECKMULTISIG:
                        case OpCode.CHECKSIG: opCost = 0.1m; break;

                        case OpCode.APPCALL:
                        case OpCode.TAILCALL:
                        case OpCode.SHA256:
                        case OpCode.SHA1: opCost = 0.01m; break;

                        case OpCode.HASH256:
                        case OpCode.HASH160: opCost = 0.02m; break;

                        case OpCode.NOP: opCost = 0; break;
                        default: opCost = 0.001m; break;
                    }

                usedGas += opCost;
                usedOpcodeCount++;
                
                OnStep?.Invoke(new EmulatorStepInfo() 
                { 
                    byteCode = engine.CurrentContext.Script, 
                    offset = engine.CurrentContext.InstructionPointer, 
                    opcode = opcode, 
                    gasCost = opCost, 
                    sysCall = null 
                 }); //opcode == OpCode.SYSCALL? engine.lastSysCall : null
            }
            catch
            {
                // failed to get instruction pointer
            }

            if (engine.State.HasFlag(VMState.FAULT))
            {
                lastState = new DebuggerState(DebuggerState.State.Exception, lastOffset);
                return lastState;
            }

            if (engine.State.HasFlag(VMState.BREAK))
            {
                if (_breakpoints.Contains(lastOffset))
                {
                    lastState = new DebuggerState(DebuggerState.State.Break, lastOffset);
                    return lastState;
                }
            }

            if (engine.State.HasFlag(VMState.HALT))
            {
                lastState = new DebuggerState(DebuggerState.State.Finished, lastOffset);
                return lastState;
            }

            lastState = new DebuggerState(DebuggerState.State.Running, lastOffset);
            return lastState;
        }

        /// <summary>
        /// executes the script until it finishes, fails or hits a breakpoint
        /// </summary>
        public DebuggerState Run()
        {
            do
            {
                lastState = Step();
            } while (lastState.state == DebuggerState.State.Running);

            return lastState;
        }

        public void Stop()
        {
            var context = engine.InvocationStack;
            while(context.Count > 0)
            {
                engine.InvocationStack.Pop();
            }
            lastState = new DebuggerState(DebuggerState.State.Finished, 0);
        }

        public StackItem GetOutput()
        {
            var result = engine.ResultStack.Peek();
            return result;
        }

        public IEnumerable<StackItem> GetEvaluationStack()
        {
            var context = engine.CurrentContext;
            return context.EvaluationStack;
        }

        public IEnumerable<StackItem> GetAltStack()
        {
            var context = engine.CurrentContext;
            return context.AltStack;
        }


        #region TRANSACTIONS
        public void SetTransaction(byte[] assetID, BigInteger amount)
        {
            var key = Runtime.invokerKeys;

            var bytes = key != null ? CryptoUtils.AddressToScriptHash(key.address) : new byte[20];

            var src_hash = new UInt160(bytes);
            var dst_hash = CryptoUtils.ToScriptHash(ContractByteCode);
            //var dst_hash = new UInt160(LuxUtils.ReverseHex(LuxUtils.ByteToHex(CryptoUtils.ToScriptHash(ContractByteCode).ToArray())).HexToBytes()); 
            //new UInt160(CryptoUtils.AddressToScriptHash(this.currentAccount.keys.address));
            this.currentHash = dst_hash;

            BigInteger asset_decimals = 100000000;
            BigInteger total_amount = (amount * 10) * asset_decimals; // FIXME instead of (amount * 10) we should take balance from virtual blockchain

            var block = blockchain.GenerateBlock();

            var tx = new API.Transaction(block);

            tx.outputs.Add(new API.TransactionOutput(assetID, amount, dst_hash));
            tx.outputs.Add(new API.TransactionOutput(assetID, total_amount - amount, src_hash));

            blockchain.ConfirmBlock(block);
          
            this.currentTransaction = tx;
        }
        #endregion

        public static object ConvertArgument(DataNode item, Emulator.Type hintType = Emulator.Type.Unknown)
        {
            if (item.HasChildren)
            {
                bool isByteArray = true;

                foreach (var child in item.Children)
                {
                    byte n;
                    if (string.IsNullOrEmpty(child.Value) || !byte.TryParse(child.Value, out n))
                    {
                        isByteArray = false;
                        break;
                    }
                }

                if (hintType == Type.Array)
                {
                    isByteArray = false;
                }

                if (isByteArray)
                {
                    var arr = new byte[item.ChildCount];
                    int index = arr.Length;
                    foreach (var child in item.Children)
                    {
                        index--;
                        arr[index] = byte.Parse(child.Value);
                   }
                    return arr;
                }
                else
                {
                    var list = new List<object>();
                    for (int i = 0; i< item.ChildCount; i++)
                    {
                        var child = item.GetNodeByIndex(i);
                        list.Add(ConvertArgument(child));
                    }
                    return list;
                }
            }

            BigInteger intVal;

            if (item.Kind == NodeKind.Numeric)
            {
                if (BigInteger.TryParse(item.Value, out intVal))
                {
                    return intVal;
                }
                else
                {
                    return 0;
                }
            }
            else
            if (item.Kind == NodeKind.Boolean)
            {
                return "true".Equals(item.Value.ToLowerInvariant()) ? true : false;
            }
            else
            if (item.Kind == NodeKind.Null)
            {
                return null;
            }
            else
            if (item.Value == null)
            {
                return null;
            }
            else
            if (item.Value.StartsWith("0x"))
            {
                return item.Value.Substring(2).HexToByte();
            }
            else
            {
                return item.Value;
            }
        }

        public byte[] GetExecutingByteCode()
        {
            try
            {
                return engine.CurrentContext.Script;
            }
            catch
            {
                return null;
            }
        }

        private Dictionary<int, Assignment> _assigments = new Dictionary<int, Assignment>();
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        public IEnumerable<Variable> Variables => _variables.Values;

        public void ClearAssignments()
        {
            _assigments.Clear();
            _variables.Clear();
        }

        public void AddAssigment(int offset, string name, Type type)
        {
            _assigments[offset] = new Assignment() { name = name, type = type};
        }

        public Variable GetVariable(string name)
        {
            if (_variables.ContainsKey(name))
            {
                return _variables[name];
            }

            return null;
        }
    }
}
