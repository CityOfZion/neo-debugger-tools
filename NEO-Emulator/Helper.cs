using Neo.Cryptography;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;


namespace Neo.Emulator
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }
            return left.SequenceEqual(right);
        }
        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            return key.Sum(b => b);
        }
    }
    public static class Helper
    {
        [Nonemit]
        public static BigInteger AsBigInteger(this byte[] source)
        {
            return new BigInteger(source);
        }

        [Nonemit]
        public static byte[] AsByteArray(this BigInteger source)
        {
            return source.ToByteArray();
        }

        [Nonemit]
        public static byte[] AsByteArray(this string source)
        {
            return System.Text.Encoding.ASCII.GetBytes(source);
        }

        [Nonemit]
        public static string AsString(this byte[] source)
        {
            return System.Text.Encoding.ASCII.GetString(source);
        }

        [OpCode(OpCode.CAT)]
        public static byte[] Concat(this byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        [OpCode(OpCode.SUBSTR)]
        public static byte[] Range(this byte[] source, int index, int count)
        {
            byte[] ret = new byte[count];
            Buffer.BlockCopy(source, index, ret, 0, count);
            return ret;
        }

        [OpCode(OpCode.LEFT)]
        public static byte[] Take(this byte[] source, int count)
        {
            byte[] ret = new byte[count];
            Buffer.BlockCopy(source, 0, ret, 0, count);
            return ret;
        }

        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static ScriptBuilder EmitPush(this ScriptBuilder sb, object obj)
        {
            switch (obj)
            {
                case bool data:
                    sb.EmitPush(data);
                    break;
                case byte[] data:
                    sb.EmitPush(data);
                    break;
                case string data:
                    sb.EmitPush(data);
                    break;
                case BigInteger data:
                    sb.EmitPush(data);
                    break;
                case sbyte data:
                    sb.EmitPush(data);
                    break;
                case byte data:
                    sb.EmitPush(data);
                    break;
                case short data:
                    sb.EmitPush(data);
                    break;
                case ushort data:
                    sb.EmitPush(data);
                    break;
                case int data:
                    sb.EmitPush(data);
                    break;
                case uint data:
                    sb.EmitPush(data);
                    break;
                case long data:
                    sb.EmitPush(data);
                    break;
                case ulong data:
                    sb.EmitPush(data);
                    break;
                case Enum data:
                    sb.EmitPush(BigInteger.Parse(data.ToString("d")));
                    break;
                default:
                    throw new ArgumentException();
            }
            return sb;
        }


        public static string ByteToHex(this byte[] data)
        {
            string hex = BitConverter.ToString(data).Replace("-", "");
            return hex;
        }

        private static int BitLen(int w)
        {
            return (w < 1 << 15 ? (w < 1 << 7
                ? (w < 1 << 3 ? (w < 1 << 1
                ? (w < 1 << 0 ? (w < 0 ? 32 : 0) : 1)
                : (w < 1 << 2 ? 2 : 3)) : (w < 1 << 5
                ? (w < 1 << 4 ? 4 : 5)
                : (w < 1 << 6 ? 6 : 7)))
                : (w < 1 << 11
                ? (w < 1 << 9 ? (w < 1 << 8 ? 8 : 9) : (w < 1 << 10 ? 10 : 11))
                : (w < 1 << 13 ? (w < 1 << 12 ? 12 : 13) : (w < 1 << 14 ? 14 : 15)))) : (w < 1 << 23 ? (w < 1 << 19
                ? (w < 1 << 17 ? (w < 1 << 16 ? 16 : 17) : (w < 1 << 18 ? 18 : 19))
                : (w < 1 << 21 ? (w < 1 << 20 ? 20 : 21) : (w < 1 << 22 ? 22 : 23))) : (w < 1 << 27
                ? (w < 1 << 25 ? (w < 1 << 24 ? 24 : 25) : (w < 1 << 26 ? 26 : 27))
                : (w < 1 << 29 ? (w < 1 << 28 ? 28 : 29) : (w < 1 << 30 ? 30 : 31)))));
        }

        public static byte[] AddressToScriptHash(string s)
        {
            var bytes = s.Base58CheckDecode();
            var data = bytes.Skip(1).Take(20).ToArray();
            return data;
        }

        internal static int GetBitLength(this BigInteger i)
        {
            byte[] b = i.ToByteArray();
            return (b.Length - 1) * 8 + BitLen(i.Sign > 0 ? b[b.Length - 1] : 255 - b[b.Length - 1]);
        }

        internal static int GetLowestSetBit(this BigInteger i)
        {
            if (i.Sign == 0)
                return -1;
            byte[] b = i.ToByteArray();
            int w = 0;
            while (b[w] == 0)
                w++;
            for (int x = 0; x < 8; x++)
                if ((b[w] & 1 << x) > 0)
                    return x + w * 8;
            throw new Exception();
        }

        internal static BigInteger Mod(this BigInteger x, BigInteger y)
        {
            x %= y;
            if (x.Sign < 0)
                x += y;
            return x;
        }

        internal static BigInteger ModInverse(this BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        internal static BigInteger NextBigInteger(this Random rand, int sizeInBits)
        {
            if (sizeInBits < 0)
                throw new ArgumentException("sizeInBits must be non-negative");
            if (sizeInBits == 0)
                return 0;
            byte[] b = new byte[sizeInBits / 8 + 1];
            rand.NextBytes(b);
            if (sizeInBits % 8 == 0)
                b[b.Length - 1] = 0;
            else
                b[b.Length - 1] &= (byte)((1 << sizeInBits % 8) - 1);
            return new BigInteger(b);
        }

        internal static BigInteger NextBigInteger(this RandomNumberGenerator rng, int sizeInBits)
        {
            if (sizeInBits < 0)
                throw new ArgumentException("sizeInBits must be non-negative");
            if (sizeInBits == 0)
                return 0;
            byte[] b = new byte[sizeInBits / 8 + 1];
            rng.GetBytes(b);
            if (sizeInBits % 8 == 0)
                b[b.Length - 1] = 0;
            else
                b[b.Length - 1] &= (byte)((1 << sizeInBits % 8) - 1);
            return new BigInteger(b);
        }

        internal static bool TestBit(this BigInteger i, int index)
        {
            return (i & (BigInteger.One << index)) > BigInteger.Zero;
        }

        public static DateTime ToDateTime(this uint timestamp)
        {
            return unixEpoch.AddSeconds(timestamp).ToLocalTime();
        }

        public static DateTime ToDateTime(this ulong timestamp)
        {
            return unixEpoch.AddSeconds(timestamp).ToLocalTime();
        }

        public static string ToHexString(this IEnumerable<byte> value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in value)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe internal static int ToInt32(this byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                return *((int*)pbyte);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe internal static long ToInt64(this byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                return *((long*)pbyte);
            }
        }

        public static uint ToTimestamp(this DateTime time)
        {
            return (uint)(time.ToUniversalTime() - unixEpoch).TotalSeconds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe internal static ushort ToUInt16(this byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                return *((ushort*)pbyte);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe internal static uint ToUInt32(this byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                return *((uint*)pbyte);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe internal static ulong ToUInt64(this byte[] value, int startIndex)
        {
            fixed (byte* pbyte = &value[startIndex])
            {
                return *((ulong*)pbyte);
            }
        }

        internal static long WeightedAverage<T>(this IEnumerable<T> source, Func<T, long> valueSelector, Func<T, long> weightSelector)
        {
            long sum_weight = 0;
            long sum_value = 0;
            foreach (T item in source)
            {
                long weight = weightSelector(item);
                sum_weight += weight;
                sum_value += valueSelector(item) * weight;
            }
            if (sum_value == 0) return 0;
            return sum_value / sum_weight;
        }

        internal static IEnumerable<TResult> WeightedFilter<T, TResult>(this IList<T> source, double start, double end, Func<T, long> weightSelector, Func<T, long, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (start < 0 || start > 1) throw new ArgumentOutOfRangeException(nameof(start));
            if (end < start || start + end > 1) throw new ArgumentOutOfRangeException(nameof(end));
            if (weightSelector == null) throw new ArgumentNullException(nameof(weightSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            if (source.Count == 0 || start == end) yield break;
            double amount = source.Sum(weightSelector);
            long sum = 0;
            double current = 0;
            foreach (T item in source)
            {
                if (current >= end) break;
                long weight = weightSelector(item);
                sum += weight;
                double old = current;
                current = sum / amount;
                if (current <= start) continue;
                if (old < start)
                {
                    if (current > end)
                    {
                        weight = (long)((end - start) * amount);
                    }
                    else
                    {
                        weight = (long)((current - start) * amount);
                    }
                }
                else if (current > end)
                {
                    weight = (long)((end - old) * amount);
                }
                yield return resultSelector(item, weight);
            }
        }

        public static string MD5(this byte[] inputBytes)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

    }
}
