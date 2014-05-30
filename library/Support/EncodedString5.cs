using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PkmnFoundations.Support
{
    public class EncodedString5
    {
        public EncodedString5(byte[] data)
        {
            RawData = data;
        }

        public EncodedString5(byte[] data, int start, int count)
        {
            if (data.Length < start + count) throw new ArgumentOutOfRangeException("count");

            byte[] trim = new byte[count];
            Array.Copy(data, start, trim, 0, count);
            AssignData(trim);
        }

        public EncodedString5(String text)
        {
            Text = text;
        }

        // todo: Use pointers for both of these
		public static string DecodeString(byte[] data, int start, int count)
		{
            if (data.Length < start + count) throw new ArgumentOutOfRangeException("count");

			StringBuilder sb = new StringBuilder();

            for (int i = start; i < start + count * 2; i += 2)
			{
				ushort gamecode = BitConverter.ToUInt16(data, i);
                // todo: convert some characters via lookup table
                // http://projectpokemon.org/wiki/Pokemon_Black/White_NDS_Structure#Characters_that_can_be_replaced_in_Unicode
				if (gamecode == 0xFFFF) { break; }
				sb.Append((char)gamecode);
			}

			return sb.ToString();
		}

        public static String DecodeString(byte[] data)
        {
            return DecodeString(data, 0, data.Length);
        }

        public static byte[] EncodeString(string str)
        {
            MemoryStream m = new MemoryStream(str.Length * 2 + 2);
            foreach (char c in str.ToCharArray())
            {
                m.Write(BitConverter.GetBytes(c), 0, 2);
            }
            m.WriteByte(0xff);
            m.WriteByte(0xff);
            return m.ToArray();
        }

		private byte[] m_raw_data;
		private string m_text;

		public string Text
        {
            get
            {
                if (m_text == null && m_raw_data == null) return null;
                if (m_text == null) m_text = DecodeString(m_raw_data);
                return m_text;
            }
            set
            {
                AssignText(value);
            }
        }

		public byte[] RawData
        {
            get
            {
                if (m_raw_data == null && m_text == null) return null;
                if (m_raw_data == null) m_raw_data = EncodeString(m_text);
                return m_raw_data.ToArray();
            }
            set
            {
                AssignData(value.ToArray());
            }
        }

        // lazy evaluate these conversions since they're slow
        private void AssignData(byte[] data)
        {
            m_raw_data = data;
            m_text = null;
        }

        private void AssignText(String text)
        {
            m_text = text;
            m_raw_data = null;
        }
		
		public override string ToString()
		{
			return Text;
		}
    }
}
