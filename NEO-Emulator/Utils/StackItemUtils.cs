using Neo.Cryptography;
using Neo.VM;
using System;
using System.Numerics;
using System.Text;

namespace Neo.Emulator.Utils
{
    public static class FormattingUtils
    {
        public static string StackItemAsString(StackItem item, bool addQuotes = false)
        {
            if (item.IsArray)
            {
                var s = new StringBuilder();
                var items = item.GetArray();

                s.Append('[');
                for (int i = 0; i < items.Length; i++)
                {
                    var element = items[i];
                    if (i > 0)
                    {
                        s.Append(',');
                    }
                    s.Append(StackItemAsString(element));
                }
                s.Append(']');
                return s.ToString();
            }

            if (item is Neo.VM.Types.Boolean)
            {
                return item.GetBoolean().ToString();
            }

            if (item is Neo.VM.Types.Integer)
            {
                return item.GetBigInteger().ToString();
            }

            if (item is Neo.VM.Types.InteropInterface)
            {
                return "{InteropInterface}";
            }

            var data = item.GetByteArray();

            if (data == null)
            {
                return "[Null]";
            }

            if (data == null || data.Length == 0)
            {
                return "False";
            }


            return FormattingUtils.OutputData(data, addQuotes);
        }

        public static string OutputLine(string col1, string col2, string col3)
        {
            int colSize = 14;
            return col1.PadRight(colSize) + col2.PadRight(colSize) + col3;
        }

        public static string OutputData(byte[] data, bool addQuotes, bool preferInts = false)
        {
            if (data == null)
            {
                return "[Null]";
            }

            for (int i = 0; i < data.Length; i++)
            {
                var c = (char)data[i];
                var isValidText = char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c)|| char.IsSymbol(c);
                if (!isValidText)
                {
                    if (preferInts)
                    {
                        var val = new BigInteger(data);
                        return val.ToString();
                    }

                    if (data.Length == 20)
                    {
                        var signatureHash = Crypto.Default.ToScriptHash(data);
                        return Crypto.Default.ToAddress(signatureHash);
                    }

                    return OutputHex(data);
                }
            }

            var result = System.Text.Encoding.ASCII.GetString(data);

            if (addQuotes)
            {
                result = '"' + result + '"';
            }

            return result;
        }

        public static string OutputHex(byte[] data)
        {
            string hex = BitConverter.ToString(data);
            return hex;
        }
    }
}
