using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public abstract class TrendyPhraseBase
    {
        public TrendyPhraseBase(byte[] data)
        {
            Data = data;
        }

        private byte[] m_data;
        public byte[] Data
        {
            get
            {
                return m_data;
            }
            set
            {
                if (m_data == value) return;
                if (value == null)
                {
                    m_data = null;
                    return;
                }

                if (value.Length != 8) throw new ArgumentException("Trendy phrase data must be 8 bytes.");
                m_data = value.ToArray();
            }
        }

        public override string ToString()
        {
            return Render("{0}");
        }

        public abstract string Render(string wordFormat);

        internal static byte[] Pack(ushort mood, ushort index, ushort word1, ushort word2)
        {
            byte[] result = new byte[8];
            Array.Copy(BitConverter.GetBytes(mood), 0, result, 0, 2);
            Array.Copy(BitConverter.GetBytes(index), 0, result, 2, 2);
            Array.Copy(BitConverter.GetBytes(word1), 0, result, 4, 2);
            Array.Copy(BitConverter.GetBytes(word2), 0, result, 6, 2);
            return result;
        }

        public ushort Mood
        {
            get
            {
                return BitConverter.ToUInt16(Data, 0);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, Data, 0, 2);
            }
        }

        public ushort Index
        {
            get
            {
                return BitConverter.ToUInt16(Data, 2);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, Data, 2, 2);
            }
        }

        public ushort Word1
        {
            get
            {
                return BitConverter.ToUInt16(Data, 4);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, Data, 4, 2);
            }
        }

        public ushort Word2
        {
            get
            {
                return BitConverter.ToUInt16(Data, 6);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, Data, 6, 2);
            }
        }

    }
}
