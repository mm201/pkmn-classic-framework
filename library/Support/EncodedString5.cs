using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PkmnFoundations.Support
{
    public class EncodedString5 : EncodedStringBase
    {
        /// <summary>
        /// Instances an EncodedString5 from its binary representation.
        /// </summary>
        /// <param name="data">This buffer is copied.</param>
        public EncodedString5(byte[] data) : base(data)
        {
        }

        /// <summary>
        /// Instances an EncodedString5 from its binary representation.
        /// </summary>
        /// <param name="data">Buffer to copy from</param>
        /// <param name="start">Offset in buffer</param>
        /// <param name="length">Number of bytes (not chars) to copy</param>
        public EncodedString5(byte[] data, int start, int length) : base(data, start, length)
        {
        }

        /// <summary>
        /// Instances an EncodedString5 from a Unicode string.
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="length">Length of encoded buffer in bytes (not chars)</param>
        public EncodedString5(string text, int length) : base(text, length)
        {
        }

        // todo: Use pointers for both of these
		public static string DecodeString_impl(byte[] data, int start, int count)
		{
            if (data.Length < start + count) throw new ArgumentOutOfRangeException("count");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

			StringBuilder sb = new StringBuilder();

            for (int i = start; i < start + count * 2; i += 2)
			{
                ushort gamecode = BitConverter.ToUInt16(data, i);
                if (gamecode == 0xffff) { break; }
                char ch = Generation5TextLookupTable.ContainsKey(gamecode) ?
                    Generation5TextLookupTable[gamecode] :
                    (char)gamecode;

                sb.Append(ch);
            }

			return sb.ToString();
		}

        public static string DecodeString_impl(byte[] data)
        {
            return DecodeString_impl(data, 0, data.Length >> 1);
        }

        public static byte[] EncodeString_impl(string str, int size)
        {
            int actualLength = (size >> 1) - 1;
            if (str.Length > actualLength) throw new ArgumentOutOfRangeException("size");

            byte[] result = new byte[size];
            MemoryStream m = new MemoryStream(result);

            foreach (char c in str.ToCharArray())
            {
                m.Write(BitConverter.GetBytes(LookupReverse.ContainsKey(c) ? LookupReverse[c] : c), 0, 2);
            }

            m.WriteByte(0xff);
            m.WriteByte(0xff);
            return result;
        }

        protected override string DecodeString(byte[] data, int start, int count)
        {
            return DecodeString_impl(data, start, count);
        }

        protected override byte[] EncodeString(string str, int size)
        {
            return EncodeString_impl(str, size);
        }


        public override string ToString()
		{
			return Text;
		}

        public EncodedString5 Clone()
        {
            return new EncodedString5(RawData);
        }

        private static Dictionary<char, ushort> m_lookup_reverse = null;
        private static Dictionary<char, ushort> LookupReverse
        {
            get
            {
                if (m_lookup_reverse == null)
                {
                    Dictionary<char, ushort> reverse = new Dictionary<char, ushort>(Generation5TextLookupTable.Count);

                    foreach (KeyValuePair<ushort, char> pair in Generation5TextLookupTable)
                    {
                        //if (!reverse.ContainsKey(pair.Value))
                            reverse.Add(pair.Value, pair.Key);
                    }

                    m_lookup_reverse = reverse;
                }
                return m_lookup_reverse;
            }
        }

        private static Dictionary<ushort, char> Generation5TextLookupTable = new Dictionary<ushort, char>
        {
            {0x2467, '\u00d7'}, {0x2468, '\u00f7'}, {0x246c, '\u2026'}, {0x246d, '\u2642'}, 
            {0x246e, '\u2640'}, {0x246f, '\u2660'}, {0x2470, '\u2663'}, {0x2471, '\u2665'}, 
            {0x2472, '\u2666'}, {0x2473, '\u2605'}, {0x2474, '\u25ce'}, {0x2475, '\u25cb'}, 
            {0x2476, '\u25a1'}, {0x2477, '\u25b3'}, {0x2478, '\u25c7'}, {0x2479, '\u266a'}, 
            {0x247a, '\u2600'}, {0x247b, '\u2601'}, {0x247d, '\u2602'}, 
        };
    }
}
