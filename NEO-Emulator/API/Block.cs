using LunarParser;
using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Emulator.API
{
    public class Block : Header
    {
        public uint height;
        public List<Transaction> transactions = new List<Transaction>();

        public Block(uint height, uint timestamp) : base(timestamp)
        {
            this.height = height;
        }

        internal bool Load(DataNode root)
        {
            this.timestamp = root.GetUInt32("timestamp");

            this.transactions.Clear();

            foreach (var child in root.Children)
            {
                if (child.Name == "transaction")
                {
                    var tx = new Transaction(this);
                    tx.Load(child);
                    transactions.Add(tx);
                }
            }

            return true;
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("block");
            foreach (var tx in transactions)
            {
                result.AddNode(tx.Save());
            }

            result.AddField("timestamp", timestamp);

            return result;
        }

        [Syscall("Neo.Block.GetTransactionCount")]
        public bool GetTransactionCount(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();
            var block = ((VM.Types.InteropInterface)obj).GetInterface<Block>();

            if (block == null)
                return false;

            engine.EvaluationStack.Push(block.transactions.Count);
            return true;
        }

        [Syscall("Neo.Block.GetTransactions")]
        public bool GetTransactions(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();
            var block = ((VM.Types.InteropInterface)obj).GetInterface<Block>();

            if (block == null)
                return false;

            // returns Transaction[]

            var txs = new StackItem[block.transactions.Count];
            for (int i=0; i<block.transactions.Count; i++)
            {
                var tx = block.transactions[i];
                txs[i] = new VM.Types.InteropInterface(tx);
            }

            var array = new VM.Types.Array(txs);

            throw new NotImplementedException();
        }

        
        [Syscall("Neo.Block.GetTransaction")]
        public bool GetTransaction(ExecutionEngine engine)
        {
            var index = (int)engine.EvaluationStack.Pop().GetBigInteger();
            var obj = engine.EvaluationStack.Pop();
            var block = ((VM.Types.InteropInterface)obj).GetInterface<Block>();

            if (block == null)
                return false;

            if (index<0 || index>=block.transactions.Count)
            {
                return false;
            }

            var tx = block.transactions[index];
            engine.EvaluationStack.Push(new VM.Types.InteropInterface(tx));
            return true;
        }
    }
}
