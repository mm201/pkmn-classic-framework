using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Support
{
    public abstract class EncodedStringBase
    {
        public EncodedStringBase()
        {

        }

        public EncodedStringBase(byte[] data) : this()
        {
            RawData = data;
        }

        /// <summary>
        /// Instances an EncodedString from its binary representation.
        /// </summary>
        /// <param name="data">Buffer to copy from</param>
        /// <param name="start">Offset in buffer</param>
        /// <param name="length">Number of bytes (not chars) to copy</param>
        public EncodedStringBase(byte[] data, int start, int length)
        {
            if (length < 2) throw new ArgumentOutOfRangeException("length");
            if (data.Length < start + length) throw new ArgumentOutOfRangeException("length");
            if (length % 2 != 0) throw new ArgumentException("length");

            Size = length;
            byte[] trim = new byte[length];
            Array.Copy(data, start, trim, 0, length);
            AssignData(trim);
        }

        /// <summary>
        /// Instances an EncodedString from a Unicode string.
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="length">Length of encoded buffer in bytes (not chars)</param>
        public EncodedStringBase(string text, int length)
        {
            if (length < 2) throw new ArgumentOutOfRangeException("length");
            if (length % 2 != 0) throw new ArgumentException("length");

            Size = length;
            Text = text;
        }

        // todo: move more of the encoded string implementation over here for DRY reasons

        private byte[] m_raw_data;
        private string m_text;

        public virtual string Text
        {
            get
            {
                if (m_text == null && m_raw_data == null) return null;
                if (m_text == null) m_text = DecodeString(m_raw_data, 0, m_raw_data.Length >> 1);
                return m_text;
            }
            set
            {
                int actualLength = (Size >> 1) - 1;
                if (value.Length > actualLength) throw new ArgumentException();
                AssignText(value);
            }
        }

        public virtual byte[] RawData
        {
            get
            {
                if (m_raw_data == null && m_text == null) return null;
                if (m_raw_data == null) m_raw_data = EncodeString(m_text, Size);
                return m_raw_data.ToArray();
            }
            set
            {
                int size = value.Length;
                if (size < 2) throw new ArgumentException();
                if (size % 2 != 0) throw new ArgumentException();

                Size = size;
                AssignData(value.ToArray());
            }
        }

        // lazy evaluate these conversions since they're slow
        protected virtual void AssignData(byte[] data)
        {
            m_raw_data = data;
            m_text = null;
        }

        protected virtual void AssignText(string text)
        {
            m_text = text;
            m_raw_data = null;
        }

        protected abstract string DecodeString(byte[] data, int start, int count);

        protected abstract byte[] EncodeString(string str, int size);

        public override string ToString()
        {
            return Text;
        }

        public virtual int Size { get; protected set; }

        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }
    }
}
