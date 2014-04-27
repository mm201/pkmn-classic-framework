using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokeFoundations.GTS
{
    public static class Common
    {
        public static string ToHexStringUpper(this byte[] bytes)
        {
            // http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa/14333437#14333437
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        public static string ToHexStringLower(this byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(87 + b + (((b - 10) >> 31) & -39));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(87 + b + (((b - 10) >> 31) & -39));
            }
            return new string(c);
        }

        public static byte[] FromHexString(String hex)
        {
            // very suboptimal but error tolerant
            byte output = 0;
            List<byte> result = new List<byte>(hex.Length / 2);
            bool havePrev = false;
            foreach (char c in hex.ToCharArray())
            {
                if (c >= '0' && c <= '9')
                {
                    output |= (byte)(c - '0');
                }
                if (c >= 'A' && c <= 'F')
                {
                    output |= (byte)(c - '7');
                }
                if (c >= 'a' && c <= 'f')
                {
                    output |= (byte)(c - 'W');
                }
                if (havePrev)
                {
                    havePrev = false;
                    result.Add(output);
                    output = 0;
                }
                else
                {
                    havePrev = true;
                    output <<= 4;
                }
            }

            return result.ToArray();
        }


    }
}