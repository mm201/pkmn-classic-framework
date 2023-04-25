using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Wfc
{
    public class BattleVideoRecord5
    {
        public BattleVideoRecord5()
        {
        }

        public BattleVideoRecord5(int pid, ulong serial_number, byte[] data)
        {
            if (data.Length != 6308) throw new ArgumentException("Battle video data must be 6308 bytes.");

            byte[] data_head = new byte[196];
            byte[] data_main = new byte[6112];

            Array.Copy(data, 0, data_head, 0, 196);
            Array.Copy(data, 196, data_main, 0, 6112);

            PID = pid;
            SerialNumber = serial_number;
            Header = new BattleVideoHeader5(pid, serial_number, data_head);
            Data = data_main;
        }

        public BattleVideoRecord5(int pid, ulong serial_number, BattleVideoHeader5 header, byte[] data_main)
        {
            if (data_main.Length != 6112) throw new ArgumentException("Battle video main data must be 6112 bytes.");

            PID = pid;
            SerialNumber = serial_number;
            Header = header;
            Data = data_main;
        }

        public int PID;
        public ulong SerialNumber;
        public BattleVideoHeader5 Header;
        public byte[] Data;

        public BattleVideoRecord5 Clone()
        {
            return new BattleVideoRecord5(PID, SerialNumber, Header.Clone(), Data.ToArray());
        }

        public byte[] Save()
        {
            byte[] result = new byte[6308];
            Array.Copy(Header.Data, 0, result, 0, 196);
            Array.Copy(Data, 196, result, 0, 6112);
            return result;
        }
    }
}
