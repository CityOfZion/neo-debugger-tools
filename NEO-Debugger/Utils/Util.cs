using LunarParser;
using LunarParser.JSON;
using Neo.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Debugger.Utils
{
    public static class Util
    {
        public static DataNode GetArgsListAsNode(string argList)
        {
            var node = JSONReader.ReadFromString("{\"params\": [" + argList + "]}");
            return node.GetNode("params");
        }

        public static bool IsValidWallet(string address)
        {
            if (string.IsNullOrEmpty(address) || address[0] != 'A')
            {
                return false;
            }

            try
            {
                var buffer = address.Base58CheckDecode();
                return buffer != null && buffer.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsHex(string chars)
        {
            if (string.IsNullOrEmpty(chars)) return false;
            if (chars.Length % 2 != 0) return false;

            bool isHex;
            foreach (var c in chars)
            {
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }
            return true;
        }

        public static KeyPair GetKeyFromString(string key)
        {
            if (key.Length == 52)
            {
                return KeyPair.FromWIF(key);
            }
            else
            if (key.Length == 64)
            {
                var keyBytes = key.HexToBytes();
                return new KeyPair(keyBytes);
            }
            else
            {
                return null;
            }
        }

        public static string ParseNode(DataNode node, int index)
        {
            string val;

            if (node.ChildCount > 0)
            {
                val = "";

                foreach (var child in node.Children)
                {
                    if (val.Length > 0) val += ", ";

                    val += ParseNode(child, -1);
                }
                val = $"[{val}]";
            }
            else
            if (node.Kind == NodeKind.Null)
            {
                val = "[]";
            }
            else
            if (node.Kind == NodeKind.Numeric || node.Kind == NodeKind.Boolean)
            {
                val = node.Value;
            }
            else
            if (node.Kind == NodeKind.String)
            {
                val = $"\"{node.Value}\"";

            }
            else
            {
                val = node.Value;
            }

            return val;
        }

        public static  string BytesToString(byte[] bytes)
        {
            var s = "";
            foreach (var b in bytes)
            {
                if (s.Length > 0) s += ",";
                s += b.ToString();
            }
            s = $"[{s}]";
            return s;
        }
    }
}
