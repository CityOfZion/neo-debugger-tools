using Neo.VM;
using System;
using System.Collections.Generic;
using Neo.Emulator.Utils;

namespace Neo.Emulator.API
{
    public static class Asset
    {
        public struct Entry
        {
            public byte[] id;
            public string name;

            public Entry(string name, string id)
            {
                this.name = name;
                var bytes = id.HexToByte();
                Array.Reverse(bytes);
                this.id = bytes;
            }
        }

        private static List<Entry> _entries = null;
        public static IEnumerable<Entry> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new List<Entry>();

                    _entries.Add(new Entry("NEO", "c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b"));
                    _entries.Add(new Entry("GAS", "602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"));
                    //id = new byte[] { 155, 124, 255, 218, 166, 116, 190, 174, 15, 147, 14, 190, 96, 133, 175, 144, 147, 229, 254, 86, 179, 74, 92, 34, 12, 205, 207, 110, 252, 51, 111, 197 }
                }

                return _entries;
            }
        }

        [Syscall("Neo.Asset.GetAssetId")]
        public static bool GetAssetId(ExecutionEngine engine)
        {
            // Asset
            // return byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAssetType")]
        public static bool GetAssetType(ExecutionEngine engine)
        {
            // Asset
            //return byte
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAmount")]
        public static bool GetAmount(ExecutionEngine engine)
        {
            // Asset
            //returns long
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAvailable")]
        public static bool GetAvailable(ExecutionEngine engine)
        {
            // Asset
            //returns long 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetPrecision")]
        public static bool GetPrecision(ExecutionEngine engine)
        {
            // Asset
            //return byte 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetOwner")]
        public static bool GetOwner(ExecutionEngine engine)
        {
            // Asset
            //returns byte[]
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetAdmin")]
        public static bool GetAdmin(ExecutionEngine engine)
        {
            // Asset
            // void byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.GetIssuer")]
        public static bool GetIssuer(ExecutionEngine engine)
        {
            // Asset
            //return byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Asset.Create", 5000)]
        public static bool Create(ExecutionEngine engine)
        {
            //byte asset_type, string name, long amount, byte precision, byte[] owner, byte[] admin, byte[] issuer
            // retunrs Asset 
            throw new NotImplementedException();

        }

        [Syscall("Neo.Asset.Renew", 5000)]
        public static bool Renew(ExecutionEngine engine)
        {
            //byte years
            //returns uint 
            throw new NotImplementedException();

        }
    }
}
