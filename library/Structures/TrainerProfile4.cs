using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class TrainerProfile4
    {
        public TrainerProfile4()
        {

        }

        public TrainerProfile4(int pid, byte[] data)
        {
            if (data.Length != 100) throw new ArgumentException("Profile data must be 100 bytes.");

            PID = pid;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public byte[] Data;

        public Versions Version
        {
            get
            {
                return (Versions)Data[0];
            }
        }

        public Languages Language
        {
            get
            {
                return (Languages)Data[1];
            }
        }

        public byte Country
        {
            get
            {
                return Data[2];
            }
        }

        public byte Region
        {
            get
            {
                return Data[3];
            }
        }

        public uint OT
        {
            get
            {
                return BitConverter.ToUInt32(Data, 4);
            }
        }

        public EncodedString4 Name
        {
            get
            {
                return new EncodedString4(Data, 8, 16);
            }
        }

        public TrainerProfile4 Clone()
        {
            return new TrainerProfile4(PID, Data.ToArray());
        }
    }
}
