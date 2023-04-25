using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Wfc
{
    public class BoxRecord4 : BinarySerializableBase
    {
        public BoxRecord4()
        {
        }

        public BoxRecord4(int pid, BoxLabels4 label, ulong serial_number, BinaryReader data)
            : base()
        {
            PID = pid;
            Label = label;
            SerialNumber = serial_number;
            Load(data);
        }

        public BoxRecord4(int pid, BoxLabels4 label, ulong serial_number, byte[] data)
            : base()
        {
            PID = pid;
            Label = label;
            SerialNumber = serial_number;
            Load(data);
        }

        public BoxRecord4(int pid, BoxLabels4 label, ulong serial_number, byte[] data, int offset)
            : base()
        {
            PID = pid;
            Label = label;
            SerialNumber = serial_number;
            Load(data, offset);
        }

        public int PID { get; set; }
        public BoxLabels4 Label { get; set; }
        public ulong SerialNumber { get; set; }

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

                if (value.Length != 540) throw new ArgumentException("Box data must be 540 bytes.");
                m_data = value.ToArray();
            }
        }

        public override int Size
        {
            get 
            {
                return 540;
            }
        }

        protected override void Load(System.IO.BinaryReader reader)
        {
            m_data = reader.ReadBytes(540);
        }

        protected override void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(m_data);
        }

        public BoxRecord4 Clone()
        {
            return new BoxRecord4(PID, Label, SerialNumber, Data);
        }
    }

    public enum BoxLabels4 : int
    {
        Favorite = 0x00,
        Cool = 0x01,
        Cute = 0x02,
        Suggested = 0x03,
        Fun = 0x04,
        Select = 0x05
    }
}
