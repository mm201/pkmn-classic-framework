using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class BattleVideoRecord4
    {
        public BattleVideoRecord4()
        {
        }

        public BattleVideoRecord4(int pid, long serial_number, byte[] data)
        {
            if (data.Length != 7500) throw new ArgumentException("Battle video data must be 7500 bytes.");

            byte[] data_head = new byte[228];
            byte[] data_main = new byte[7272];

            Array.Copy(data, 0, data_head, 0, 228);
            Array.Copy(data, 228, data_main, 0, 7272);

            PID = pid;
            SerialNumber = serial_number;
            Header = new BattleVideoHeader4(pid, serial_number, data_head);
            Data = data_main;
        }

        public BattleVideoRecord4(int pid, long serial_number, BattleVideoHeader4 header, byte[] data_main)
        {
            if (data_main.Length != 7272) throw new ArgumentException("Battle video main data must be 7500 bytes.");

            PID = pid;
            SerialNumber = serial_number;
            Header = header;
            Data = data_main;
        }

        public int PID;
        public long SerialNumber;
        public BattleVideoHeader4 Header;
        public byte[] Data;

        public BattleVideoRecord4 Clone()
        {
            return new BattleVideoRecord4(PID, SerialNumber, Header.Clone(), Data.ToArray());
        }

        public byte[] Save()
        {
            byte[] result = new byte[7500];
            Array.Copy(Header.Data, 0, result, 0, 228);
            Array.Copy(Data, 228, result, 0, 7272);
            return result;
        }
    }
}
