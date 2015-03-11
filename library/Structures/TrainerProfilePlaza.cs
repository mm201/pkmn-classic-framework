using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class TrainerProfilePlaza
    {
        public TrainerProfilePlaza()
        {

        }

        public TrainerProfilePlaza(int pid, byte[] data_prefix, byte[] data)
        {
            if (data.Length != 152) throw new ArgumentException("Profile data must be 152 bytes.");

            PID = pid;
            DataPrefix = data_prefix;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public byte[] DataPrefix; // 12 bytes
        public byte[] Data; // 152 bytes

        // todo: These 4 values are basically big guesses. Fact check.
        // todo: Add more fields
        public Versions Version
        {
            get
            {
                return (Versions)DataPrefix[0x03];
            }
        }

        public Languages Language
        {
            get
            {
                return (Languages)DataPrefix[0x04];
            }
        }

        public byte Country
        {
            get
            {
                return Data[0x50];
            }
        }

        public byte Region
        {
            get
            {
                return Data[0x52];
            }
        }

        public uint OT
        {
            get
            {
                return BitConverter.ToUInt32(Data, 8);
            }
        }

        public EncodedString4 Name
        {
            get
            {
                return new EncodedString4(Data, 12, 16);
            }
        }

        public TrainerProfilePlaza Clone()
        {
            return new TrainerProfilePlaza(PID, DataPrefix.ToArray(), Data.ToArray());
        }
    }
}
