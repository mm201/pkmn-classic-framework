using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class DressupRecord4 : BinarySerializableBase
    {
        public DressupRecord4()
        {
        }

        public DressupRecord4(int pid, ulong serial_number, BinaryReader data)
            : base()
        {
            PID = pid;
            SerialNumber = serial_number;
            Load(data);
        }

        public DressupRecord4(int pid, ulong serial_number, byte[] data)
            : base()
        {
            PID = pid;
            SerialNumber = serial_number;
            Load(data);
        }

        public DressupRecord4(int pid, ulong serial_number, byte[] data, int offset)
            : base()
        {
            PID = pid;
            SerialNumber = serial_number;
            Load(data, offset);
        }

        public int PID { get; set; }
        public ulong SerialNumber { get; set; }

        // todo: Document this data structure.
        // List of decorations: https://projectpokemon.org/rawdb/diamond/msg/338.php
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

                if (value.Length != 224) throw new ArgumentException("Dressup data must be 224 bytes.");
                m_data = value.ToArray();
            }
        }

        public ushort Species
        {
            get
            {
                return BitConverter.ToUInt16(Data, 0x8c);
            }
        }

        public override int Size
        {
            get
            {
                return 224;
            }
        }

        protected override void Load(System.IO.BinaryReader reader)
        {
            m_data = reader.ReadBytes(224);
        }

        protected override void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(m_data);
        }

        public DressupRecord4 Clone()
        {
            return new DressupRecord4(PID, SerialNumber, Data);
        }
    }
}
