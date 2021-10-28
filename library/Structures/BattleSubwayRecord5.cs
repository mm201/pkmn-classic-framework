using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class BattleSubwayRecord5
    {
        public BattleSubwayRecord5(Pokedex.Pokedex pokedex)
        {
            Pokedex = pokedex;
        }

        public BattleSubwayRecord5(Pokedex.Pokedex pokedex, byte[] data)
        {
            Pokedex = pokedex;
            Load(data, 0);
        }

        public BattleSubwayRecord5(Pokedex.Pokedex pokedex, byte[] data, int start)
        {
            Pokedex = pokedex;
            Load(data, start);
        }

        public Pokedex.Pokedex Pokedex { get; set; }

        public BattleSubwayPokemon5[] Party;
        public BattleSubwayProfile5 Profile;

        public TrendyPhrase5 PhraseChallenged;
        public TrendyPhrase5 PhraseWon;
        public TrendyPhrase5 PhraseLost;
        public ushort Unknown3;

        public byte Rank;
        public byte RoomNum;
        public byte BattlesWon;
        public byte[] Unknown4; // 5 bytes
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
            writer.Write(PhraseChallenged.Data);
            writer.Write(PhraseWon.Data);
            writer.Write(PhraseLost.Data);
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
                Party[x] = new BattleSubwayPokemon5(Pokedex, data, start + x * 0x3c);
            }
            Profile = new BattleSubwayProfile5(data, 0xb4 + start);

            byte[] trendyPhrase = new byte[8];
            Array.Copy(data, 0xd6 + start, trendyPhrase, 0, 8);
            PhraseChallenged = new TrendyPhrase5(trendyPhrase);
            trendyPhrase = new byte[8];
            Array.Copy(data, 0xde + start, trendyPhrase, 0, 8);
            PhraseWon = new TrendyPhrase5(trendyPhrase);
            trendyPhrase = new byte[8];
            Array.Copy(data, 0xe6 + start, trendyPhrase, 0, 8);
            PhraseLost = new TrendyPhrase5(trendyPhrase);

            Unknown3 = BitConverter.ToUInt16(data, 0xee + start);
        }
    }
}
