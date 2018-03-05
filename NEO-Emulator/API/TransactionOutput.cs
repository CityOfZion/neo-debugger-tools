using LunarParser;
using Neo.Emulator.Utils;
using Neo.VM;
using Neo.Cryptography;
using System.Numerics;

namespace Neo.Emulator.API
{
    public class TransactionOutput : IApiInterface, IInteropInterface
    {
        public readonly byte[] assetID;
        public readonly BigInteger amount;
        public UInt160 hash;

        public TransactionOutput(byte[] assetID, BigInteger amount, UInt160 hash)
        {
            if (hash == null || assetID == null)
            {
                throw new System.Exception("Unexpected null");
            }

            this.assetID = assetID;
            this.amount = amount;
            this.hash = hash;
        }

        internal static TransactionOutput FromNode(DataNode root)
        {
            var hex = root.GetString("id");
            var assetID = hex.HexToByte();

            hex = root.GetString("hash");
            UInt160 hash;
            if (!UInt160.TryParse(hex, out hash))
            {
                hash = new UInt160(new byte[20]);
            }

            var amm = root.GetString("amount", "1");
            var amount = BigInteger.Parse(amm);

            return new TransactionOutput(assetID, amount, hash);
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("output");
            result.AddField("id", this.assetID.ByteToHex());
            result.AddField("hash", this.hash.ToString());
            result.AddField("amount", this.amount.ToString());
            return result;
        }

        [Syscall("Neo.Output.GetAssetId")]
        public static bool GetAssetId(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if  (obj == null)
            {
                return false;
            }

            var tx = obj.GetInterface<TransactionOutput>();

            engine.EvaluationStack.Push(tx.assetID);
            return true;
        }

        [Syscall("Neo.Output.GetValue")]
        public static bool GetValue(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if (obj == null)
            {
                return false;
            }

            var tx = obj.GetInterface<TransactionOutput>();

            engine.EvaluationStack.Push(tx.amount);
            return true;
        }

        [Syscall("Neo.Output.GetScriptHash")]
        public static bool GetScriptHash(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop() as VM.Types.InteropInterface;

            if (obj == null)
            {
                return false;
            }

            var tx = obj.GetInterface<TransactionOutput>();
            engine.EvaluationStack.Push(tx.hash.ToArray());

            /*var debugger = engine.ScriptContainer as NeoDebugger;

            if (debugger == null)
            {
                return false;
            }

            engine.EvaluationStack.Push(engine.CurrentContext.ScriptHash);
            */

            return true;
        }
    }
}
