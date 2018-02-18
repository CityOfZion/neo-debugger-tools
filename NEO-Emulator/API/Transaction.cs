using Neo.VM;
using System;
using System.Linq;
using System.Collections.Generic;
using LunarParser;

namespace Neo.Emulator.API
{
    public class Transaction : IInteropInterface, IScriptContainer
    {
        public NeoEmulator emulator; // temporary HACK 

        public byte[] hash;

        public List<TransactionInput> inputs = new List<TransactionInput>();
        public List<TransactionOutput> outputs = new List<TransactionOutput>();

        public readonly Block block;

        public Transaction(Block block)
        {
            this.block = block;

            var rnd = new Random();
            this.hash = new byte[20];
            rnd.NextBytes(this.hash);
        }

        byte[] IScriptContainer.GetMessage()
        {
            throw new NotImplementedException();
        }

        private static Transaction GetTransactionFromStack(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;
            if (obj == null)
            {
                return null;

            }

            var tx = obj.GetInterface<Transaction>();
            return tx;
        }

        // returns byte[]
        [Syscall("Neo.Transaction.GetHash")]
        public static bool GetHash(ExecutionEngine engine)
        {
            var tx = GetTransactionFromStack(engine);

            if (tx== null) {
                return false;
            }

            engine.EvaluationStack.Push(tx.hash);

            return true;
        }

        // returns byte 
        [Syscall("Neo.Transaction.GetType")]
        public static bool GetType(ExecutionEngine engine)
        {
            var tx = GetTransactionFromStack(engine);

            if (tx == null) {
                return false;
            }

            // The type is fixed, at least for now?
            // Also, is passing a byte array here the proper format? Or should be a BigInteger?
            byte[] result = new byte[] { (byte)TransactionType.ContractTransaction };
            engine.EvaluationStack.Push(result);

            throw new NotImplementedException();
        }

        [Syscall("Neo.Transaction.GetAttributes")]
        public static bool GetAttributes(ExecutionEngine engine)
        {
            //Transaction
            // returns TransactionAttribute[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Transaction.GetInputs")]
        public static bool GetInputs(ExecutionEngine engine)
        {
            var tx = GetTransactionFromStack(engine);

            if (tx == null)
            {
                return false;
            }

            var transactions = new List<StackItem>();

            foreach (var entry in tx.inputs)
            {
                transactions.Add(new VM.Types.InteropInterface(entry));
            }

            var outputs = new VM.Types.Array(transactions.ToArray<StackItem>());

            engine.EvaluationStack.Push(outputs);

            return true;
        }

        [Syscall("Neo.Transaction.GetOutputs")]
        public static bool GetOutputs(ExecutionEngine engine)
        {
            //Transaction
            // returns TransactionOutput[]

            return GetReferences(engine);
        }

        [Syscall("Neo.Transaction.GetReferences", 0.2)]
        public static bool GetReferences(ExecutionEngine engine)
        {
            var tx = GetTransactionFromStack(engine);

            if (tx == null)
            {
                return false;
            }

            var transactions = new List<StackItem>();

            foreach (var entry in tx.outputs)
            {
                transactions.Add(new VM.Types.InteropInterface(entry));
            }

            var outputs = new VM.Types.Array(transactions.ToArray<StackItem>());

            engine.EvaluationStack.Push(outputs);

            return true;
        }

        [Syscall("Neo.Transaction.GetUnspentCoins")]
        //returns TransactionOutput[]
        public static bool GetUnspentCoins()
        {
            throw new NotImplementedException();
        }

        #region internal methods
        internal bool Load(DataNode root)
        {
            inputs.Clear();
            outputs.Clear();

            foreach (var child in root.Children)
            {
                if (child.Name == "input")
                {
                    var input = new TransactionInput();
                    input.Load(child);
                    inputs.Add(input);
                }

                if (child.Name == "output")
                {
                    var output = new TransactionOutput();
                    output.Load(child);
                    outputs.Add(output);
                }
            }

            return true;
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("transaction");
            foreach (var input in inputs)
            {
                result.AddNode(input.Save());
            }
            foreach (var output in outputs)
            {
                result.AddNode(output.Save());
            }
            return result;
        }
        #endregion
    }
}
