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
    }
}
