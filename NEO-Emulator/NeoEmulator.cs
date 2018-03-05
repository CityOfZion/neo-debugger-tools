using Neo.VM;
using Neo.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Numerics;
using Neo.Emulator.API;
using LunarParser;
using Neo.Emulator.Utils;
using System.Diagnostics;
using Neo.Emulator.Profiler;

namespace Neo.Emulator
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
        public static NeoEmulator GetEmulator(this ExecutionEngine engine)
        {
            var tx  = (Transaction)engine.ScriptContainer;
            return tx.emulator;
        }

        public static Address GetAddress(this ExecutionEngine engine)
        {
            var emulator = engine.GetEmulator();
            return emulator.currentAddress;
        }

        public static Blockchain GetBlockchain(this ExecutionEngine engine)
        {
            var emulator = engine.GetEmulator();
            return emulator.blockchain;
        }

        public static Storage GetStorage(this ExecutionEngine engine)
        {
            var emulator = engine.GetEmulator();
            return emulator.currentAddress.storage;
        }
    }

    public class NeoEmulator 
    {
        private ExecutionEngine engine;
        private byte[] contractBytes;

        private InteropService interop;

        private HashSet<int> _breakpoints = new HashSet<int>();
        public IEnumerable<int> Breakpoints { get { return _breakpoints; } }

        public Blockchain blockchain { get; private set; }

        private DebuggerState lastState = new DebuggerState(DebuggerState.State.Invalid, -1);

        public Address currentAddress { get; private set; }
        public Transaction currentTransaction { get; private set; }

        private UInt160 currentHash;

        public CheckWitnessMode checkWitnessMode = CheckWitnessMode.Default;
        public TriggerType currentTrigger = TriggerType.Application;
        public uint timestamp = DateTime.Now.ToTimestamp();

        private double _usedGas;

        //Profiler context
        public static ProfilerContext _pctx;

        public NeoEmulator(Blockchain blockchain)
        {
            this.blockchain = blockchain;
            this.interop = new InteropService();

            _pctx = new ProfilerContext();
        }

        public int GetInstructionPtr()
        {
            return engine.CurrentContext.InstructionPointer;
        }

        public void SetExecutingAddress(Address address)
        {
            this.currentAddress = address;
            this.contractBytes = address.byteCode;

            var assembly = typeof(Neo.Emulator.Helper).Assembly;
            var methods = assembly.GetTypes()
                                  .SelectMany(t => t.GetMethods())
                                  .Where(m => m.GetCustomAttributes(typeof(SyscallAttribute), false).Length > 0)
                                  .ToArray();

            foreach (var method in methods)
            {
                var attr = (SyscallAttribute)method.GetCustomAttributes(typeof(SyscallAttribute), false).FirstOrDefault();

                interop.Register(attr.Method, (engine) => { return (bool)method.Invoke(null, new object[] { engine }); }, attr.gasCost);
                Debug.WriteLine("interopRegister:\t" + attr.Method);
            }
        }

        private int lastOffset = -1;

        private static void EmitObject(ScriptBuilder sb, object item)
        {
            if (item is byte[])
            {
                var arr = (byte[])item;

                for (int index = arr.Length - 1; index >= 0; index--)
                {
                    sb.EmitPush(arr[index]);
                }

                sb.EmitPush(arr.Length);
                sb.Emit(OpCode.PACK);
            }
            else
            if (item is List<object>)
            {
                var list = (List<object>)item;

                for (int index = 0; index < list.Count; index++)
                {
                    EmitObject(sb, list[index]);
                }              

                sb.EmitPush(list.Count);
                sb.Emit(OpCode.PACK);

                /*sb.Emit((OpCode)((int)OpCode.PUSHT + list.Count - 1));
                sb.Emit(OpCode.NEWARRAY);

                for (int index = 0; index < list.Count; index++)
                {
                    sb.Emit(OpCode.DUP); // duplicates array reference into top of stack
                    sb.EmitPush(new BigInteger(index));
                    EmitObject(sb, list[index]);
                    sb.Emit(OpCode.SETITEM);
                }*/
            }
            else
            if (item == null)
            {
                sb.EmitPush("");
            }
            else
            if (item is string)
            {
                sb.EmitPush((string)item);
            }
            else
            if (item is bool)
            {
                sb.EmitPush((bool)item);
            }
            else
            if (item is BigInteger)
            {
                sb.EmitPush((BigInteger)item);
            }
            else
            {
                throw new Exception("Unsupport contract param: " + item.ToString());
            }
        }

        public void Reset(DataNode inputs)
        {
            
            if (lastState.state == DebuggerState.State.Reset)
            {
                return;
            }

            if (currentTransaction == null)
            {
                //throw new Exception("Transaction not set");
                currentTransaction = new Transaction(this.blockchain.currentBlock);
            }

            _usedGas = 0;

            currentTransaction.emulator = this;
            engine = new ExecutionEngine(currentTransaction, Crypto.Default, null, interop);
            engine.LoadScript(contractBytes);

            foreach (var output in currentTransaction.outputs)
            {
                if (output.hash == this.currentHash)
                {
                    output.hash = new UInt160(engine.CurrentContext.ScriptHash);
                }
            }

            foreach (var pos in _breakpoints)
            {
                engine.AddBreakPoint((uint)pos);
            }

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                var items = new Stack<object>();

                if (inputs != null)
                {
                    foreach (var item in inputs.Children)
                    {
                        var obj = NeoEmulator.ConvertArgument(item);
                        items.Push(obj);
                    }
                }

                while (items.Count > 0)
                {
                    var item = items.Pop();
                    EmitObject(sb, item);
                }

                var loaderScript = sb.ToArray();
                //System.IO.File.WriteAllBytes("loader.avm", loaderScript);
                engine.LoadScript(loaderScript);
            }

            //engine.Reset();

            lastState = new DebuggerState(DebuggerState.State.Reset, 0);
            currentTransaction = null;
        }

        public void SetProfilerFilenameSource(string filename, string source)
        {
            _pctx.SetFilenameSource(filename, source);
        }

        public void SetProfilerLineno(int lineno)
        {
            _pctx.SetLineno(lineno);
        }

        public void ProfilerDumpCSV()
        {
            _pctx.DumpCSV();
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
            return !engine.State.HasFlag(VMState.HALT) && !engine.State.HasFlag(VMState.FAULT) && !engine.State.HasFlag(VMState.BREAK);
        }

        private bool ExecuteSingleStep()
        {
            if (this.lastState.state == DebuggerState.State.Reset)
            {
                engine.State = VMState.NONE;
            }

            var shouldContinue = GetRunningState();
            if (shouldContinue)
            {
                engine.StepInto();
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

            ExecuteSingleStep();

            try
            {
                lastOffset = engine.CurrentContext.InstructionPointer;

                var opcode = engine.lastOpcode;
                double opCost;

                if (opcode <= OpCode.PUSH16)
                {
                    opCost = 0;
                }
                else
                    switch (opcode)
                    {
                        case OpCode.SYSCALL:
                            {
                                var callInfo = interop.FindCall(engine.lastSysCall);
                                opCost = (callInfo != null) ? callInfo.gasCost : 0;

                                if (engine.lastSysCall.EndsWith("Storage.Put"))
                                {
                                    opCost *= (Storage.lastStorageLength / 1024.0);
                                }
                                break;
                            }

                        case OpCode.CHECKMULTISIG:
                        case OpCode.CHECKSIG: opCost = 0.1; break;

                        case OpCode.APPCALL:
                        case OpCode.TAILCALL:
                        case OpCode.SHA256:
                        case OpCode.SHA1: opCost = 0.01; break;

                        case OpCode.HASH256:
                        case OpCode.HASH160: opCost = 0.02; break;

                        case OpCode.NOP: opCost = 0; break;
                        default: opCost = 0.001; break;
                    }

                _usedGas += opCost;
                _pctx.TallyOpcode(opcode, opCost);
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
                lastState = new DebuggerState(DebuggerState.State.Break, lastOffset);
                engine.State = VMState.NONE;
                return lastState;
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

        public StackItem GetOutput()
        {
            var result = engine.EvaluationStack.Peek();
            return result;
        }

        public IEnumerable<StackItem> GetStack()
        {
            return engine.EvaluationStack;
        }

        public double GetUsedGas()
        {
            return _usedGas;
        }

        #region TRANSACTIONS
        public void SetTransaction(byte[] assetID, BigInteger amount)
        {
            var key = Runtime.invokerKeys;

            var bytes = key != null ? Helper.AddressToScriptHash(key.address) : new byte[20];

            var src_hash = new UInt160(bytes);
            var dst_hash = new UInt160(Helper.AddressToScriptHash(this.currentAddress.keys.address));
            this.currentHash = dst_hash;

            var total_amount = 10000;
           
            var tx = new Transaction(blockchain.currentBlock);
            //tx.inputs.Add(new TransactionInput(-1, src_hash));
            tx.outputs.Add(new TransactionOutput(assetID, amount, dst_hash));
            tx.outputs.Add(new TransactionOutput(assetID, total_amount - amount, src_hash));

            uint index = blockchain.currentHeight + 1;
            var block = new Block(index, DateTime.Now.ToTimestamp());
            block.transactions.Add(tx);
           
            blockchain.blocks[index] = block;

            this.currentTransaction = tx;
        }
        #endregion

        public static object ConvertArgument(DataNode item)
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

                if (isByteArray)
                {
                    var arr = new byte[item.ChildCount];
                    int index = 0;
                    foreach (var child in item.Children)
                    {
                        arr[index] = byte.Parse(child.Value);
                        index++;
                   }
                    return arr;
                }
                else
                {
                    var list = new List<object>();
                    foreach (var child in item.Children)
                    {
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


    }
}
