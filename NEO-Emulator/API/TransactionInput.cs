using LunarParser;
using Neo.VM;
using Neo.Emulator.Utils;

namespace Neo.Emulator.API
{
    public class TransactionInput : IInteropInterface
    {
        public readonly int prevIndex;
        public readonly byte[] prevHash;

        public TransactionInput(int prevIndex, byte[] prevHash)
        {
            this.prevIndex = prevIndex;
            this.prevHash = prevHash;
        }

        internal static TransactionInput FromNode(DataNode root)
        {
            var hex = root.GetString("hash");
            var prevHash = hex.HexToByte();
            var prevIndex = root.GetInt32("index");
            return new TransactionInput(prevIndex, prevHash);
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("input");
            result.AddField("hash", this.prevHash.ByteToHex());
            result.AddField("index", this.prevIndex.ToString());
            return result;
        }

        [Syscall("Neo.Input.GetHash")]
        public bool GetPrevHash(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();
            var input = ((VM.Types.InteropInterface)obj).GetInterface<TransactionInput>();

            if (input == null)
                return false;

            engine.EvaluationStack.Push(input.prevHash);
            return true;
        }

        [Syscall("Neo.Input.GetIndex")]
        public static bool GetPrevIndex(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();
            var input = ((VM.Types.InteropInterface)obj).GetInterface<TransactionInput>();

            if (input == null)
                return false;

            engine.EvaluationStack.Push(input.prevIndex);
            return true;
        }
    }
}
