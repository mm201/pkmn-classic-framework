using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class BattleSubwayRecord5
    {
        public BattleSubwayRecord5()
        {

        }

        public BattleSubwayRecord5(byte[] data)
        {
            Load(data, 0);
        }

        public BattleSubwayRecord5(byte[] data, int start)
        {
            Load(data, start);
        }

        public BattleSubwayPokemon5[] Party;
        public BattleSubwayProfile5 Profile;
        public byte[] Unknown3;

        public byte Rank;
        public byte RoomNum;
        public byte BattlesWon;
        public byte[] Unknown4;
        public ulong Unknown5;
        public int PID;

        public byte[] Save()
        {
            byte[] data = new byte[0xf0];
            MemoryStream ms = new MemoryStream(data);
            BinaryWriter writer = new BinaryWriter(ms);

            for (int x = 0; x < 3; x++)
            {
                writer.Write(Party[x].Save());
            }
            writer.Write(Profile.Save());
            writer.Write(Unknown3);

            writer.Flush();
            ms.Flush();
            return data;
        }

        public void Load(byte[] data, int start)
        {
            if (start + 0xf0 > data.Length) throw new ArgumentOutOfRangeException("start");

            Party = new BattleSubwayPokemon5[3];
            for (int x = 0; x < 3; x++)
            {
                Party[x] = new BattleSubwayPokemon5(data, start + x * 0x3c);
            }
            Profile = new BattleSubwayProfile5(data, 0xb4 + start);
            Unknown3 = new byte[0x1a];
            Array.Copy(data, start + 0xd6, Unknown3, 0, 0x1a);
        }
    }
}
