using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class DressupRecord4
    {
        public DressupRecord4()
        {
        }

        public DressupRecord4(int pid, long serial_number, byte[] data)
        {
            if (data.Length != 224) throw new ArgumentException("Dressup data must be 224 bytes.");

            PID = pid;
            SerialNumber = serial_number;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public long SerialNumber;
        public byte[] Data;

        public ushort Species
        {
            get
            {
                return BitConverter.ToUInt16(Data, 0x8c);
            }
        }

        public DressupRecord4 Clone()
        {
            return new DressupRecord4(PID, SerialNumber, Data.ToArray());
        }
    }
}
