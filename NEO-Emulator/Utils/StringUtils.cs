using System;
using System.Globalization;

namespace Neo.Emulator.Utils
{
    public static class StringUtils
    {
        public static string ByteToHex(this byte[] data)
        {
            string hex = BitConverter.ToString(data).Replace("-", "");
            return hex;
        }

        public static byte[] HexToByte(this string HexString)
        {
            if (HexString.Length % 2 != 0)
                throw new Exception("Invalid HEX");
            byte[] retArray = new byte[HexString.Length / 2];
            for (int i = 0; i < retArray.Length; ++i)
            {
                retArray[i] = byte.Parse(HexString.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return retArray;
        }

        public static bool ByteMatch(this byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }
    }
}
