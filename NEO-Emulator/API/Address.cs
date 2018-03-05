using LunarParser;
using Neo.Cryptography;
using Neo.Emulator.Utils;
using System.Collections.Generic;

namespace Neo.Emulator.API
{
    public class Address
    {
        public string name;
        public KeyPair keys;

        public byte[] byteCode;

        public Storage storage = new Storage();

        public Dictionary<string, decimal> balances = new Dictionary<string, decimal>();

        internal bool Load(DataNode root)
        {
            this.name = root.GetString("name");
            this.byteCode = root.GetString("code").HexToByte();

            var privKey = root.GetString("key").HexToByte();

            if (privKey.Length != 32)
            {
                return false;
            }

            this.keys = new KeyPair(privKey);

            var storageNode = root.GetNode("storage");

            this.storage.Load(storageNode);

            this.balances.Clear();
            var balanceNode = root.GetNode("balance");
            if (balanceNode != null)
            {
                foreach (var child in balanceNode.Children)
                {
                    if (child.Name == "entry")
                    {
                        var symbol = child.GetString("symbol");
                        var amount = child.GetDecimal("amount");

                        balances[symbol] = amount;
                    }
                }
            }

            return true;
        }

        public DataNode Save()
        {
            var result = DataNode.CreateObject("address");

            result.AddField("name", this.name);
            result.AddField("hash", this.keys.PrivateKey.ByteToHex());
            result.AddField("key", this.keys.PrivateKey.ByteToHex());
            result.AddField("code", this.byteCode.ByteToHex());

            result.AddNode(this.storage.Save());

            return result;
        }
    }

}
