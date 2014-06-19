using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class BoxRecord4
    {
        public BoxRecord4()
        {
        }

        public BoxRecord4(int pid, BoxLabels4 label, long serial_number, byte[] data)
        {
            if (data.Length != 540) throw new ArgumentException("Box data must be 540 bytes.");

            PID = pid;
            Label = label;
            SerialNumber = serial_number;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public BoxLabels4 Label;
        public long SerialNumber;
        public byte[] Data;

        public BoxRecord4 Clone()
        {
            return new BoxRecord4(PID, Label, SerialNumber, Data.ToArray());
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
