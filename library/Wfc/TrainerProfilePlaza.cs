using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Wfc
{
    public class TrainerProfilePlaza
    {
        public TrainerProfilePlaza()
        {

        }

        public TrainerProfilePlaza(int pid, byte[] data)
        {
            if (data.Length != 164) throw new ArgumentException("Profile data must be 164 bytes.");

            PID = pid;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public byte[] Data; // 164 bytes

        // todo: These 4 values are basically big guesses. Fact check.
        // todo: Add more fields
        public Versions Version
        {
            get
            {
                return (Versions)Data[0x02];
            }
        }

        public Languages Language
        {
            get
            {
                return (Languages)Data[0x03];
            }
        }

        public byte Country
        {
            get
            {
                return Data[0x4c];
            }
        }

        public byte Region
        {
            get
            {
                return Data[0x4e];
            }
        }

        public uint OT
        {
            get
            {
                return BitConverter.ToUInt32(Data, 0x14);
            }
        }

        public EncodedString4 Name
        {
            get
            {
                return new EncodedString4(Data, 0x18, 0x10);
            }
        }

        public TrainerProfilePlaza Clone()
        {
            return new TrainerProfilePlaza(PID, Data.ToArray());
        }
    }
}
