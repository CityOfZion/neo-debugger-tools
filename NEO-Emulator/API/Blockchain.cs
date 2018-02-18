using LunarParser;
using LunarParser.JSON;
using Neo.VM;
using NeoLux;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Emulator.API
{
    public class Blockchain
    {
        public uint currentHeight { get { return (uint)blocks.Count; } }
        public Dictionary<uint, Block> blocks = new Dictionary<uint, Block>();
        public List<Address> addresses = new List<Address>();

        public Block currentBlock
        {
            get
            {
                if (blocks.Count == 0)
                {
                    return null;
                }
                return blocks[currentHeight];
            }
        }

        public Blockchain()
        {

        }

        public bool Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            var json = File.ReadAllText(fileName);
            var root = JSONReader.ReadFromString(json);

            root = root["blockchain"];

            blocks.Clear();
            foreach (var child in root.Children)
            {
                if (child.Name.Equals("block"))
                {
                    uint index = (uint)(blocks.Count + 1);
                    var block = new Block(index);
                    if (block.Load(child))
                    {
                        blocks[index] = block;
                    }
                }
                if (child.Name.Equals("address"))
                {
                    var address = new Address();
                    if (address.Load(child))
                    {
                        addresses.Add(address);
                    }
                }
            }

            return true;
        }

        public void Save(string fileName)
        {
            var result = DataNode.CreateObject("blockchain");
            for (uint i = 1; i <= blocks.Count; i++)
            {
                var block = blocks[i];
                result.AddNode(block.Save());
            }

            foreach (var address in addresses)
            {
                result.AddNode(address.Save());
            }

            var json = JSONWriter.WriteToString(result);
            File.WriteAllText(fileName, json);
        }

        private Address GenerateAddress(string name)
        {
            byte[] array = new byte[32];
            var random = new Random();
            random.NextBytes(array);

            var keys = new KeyPair(array);

            var address = new Address();
            address.name = name;
            address.keys = keys;

            this.addresses.Add(address);

            return address;
        }

        public Address DeployContract(string name, byte[] byteCode)
        {
            var address = GenerateAddress(name);
            address.byteCode = byteCode;
            return address;
        }

        [Syscall("Neo.Blockchain.GetHeight")]
        public static bool GetHeight(ExecutionEngine engine)
        {
            var blockchain = engine.GetBlockchain();
            engine.EvaluationStack.Push(blockchain.currentHeight);

            return true;
        }

        [Syscall("Neo.Blockchain.GetHeader", 0.1)]
        public static bool GetHeader(ExecutionEngine engine)
        {
            var obj = engine.EvaluationStack.Pop();

            Block block = null;

            var hash = obj.GetByteArray();

            if (hash.Length > 1)
            {
                throw new NotImplementedException();
            }

            var blockchain = engine.GetBlockchain();

            if (hash.Length == 1)
            {
                var temp = obj.GetBigInteger();

                var height = (uint)temp;

                if (blockchain.blocks.ContainsKey(height))
                {
                    block = blockchain.blocks[height];
                }
                else
                if (height <= blockchain.currentHeight)
                {
                    uint index = height + 1;
                    block = new Block(index);
                    block.timestamp = 1506787300;
                    blockchain.blocks[index] = block;
                }
            }

            if (block == null)
            {
            }

            engine.EvaluationStack.Push(new VM.Types.InteropInterface(block));
            return true;
            // returns Header
        }

        public Address FindAddressByName(string name)
        {
            foreach (var addr in addresses)
            {
                if (addr.name.Equals(name))
                {
                    return addr;
                }
            }

            return null;
        }

        [Syscall("Neo.Blockchain.GetBlock", 0.2)]
        public static bool GetBlock(ExecutionEngine engine)
        {
            //uint height
            //OR
            //byte[] hash
            //returns Block 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetTransaction", 0.1)]
        public static bool GetTransaction(ExecutionEngine engine)
        {
            //byte[] hash
            //returns Transaction 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetAccount", 0.1)]
        public static bool GetAccount(ExecutionEngine engine)
        {
            //byte[] script_hash
            // returns Account 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetValidators", 0.2)]
        public static bool GetValidators(ExecutionEngine engine)
        {
            //returns byte[][]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetAsset", 0.1)]
        public static bool GetAsset(ExecutionEngine engine)
        {
            //byte[] asset_id
            // returns Asset
            throw new NotImplementedException();
        }

        [Syscall("Neo.Blockchain.GetContract", 0.1)]
        public static bool GetContract(ExecutionEngine engine)
        {
            //byte[] script_hash
            //returns Contract
            throw new NotImplementedException();
        }
    }
}
