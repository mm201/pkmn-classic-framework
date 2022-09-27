using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class BattleSubwayRecord5 : BattleTowerRecordBase
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

        private BattleSubwayPokemon5[] m_party;
        private BattleSubwayProfile5 m_profile;
        private TrendyPhrase5 m_phrase_challenged;
        private TrendyPhrase5 m_phrase_won;
        private TrendyPhrase5 m_phrase_lost;

        public override IList<BattleTowerPokemonBase> Party
        {
            get
            {
                return m_party;
            }
            set
            {
                if (!(value is BattleSubwayPokemon5[])) throw new ArgumentException("value must be BattleSubwayPokemon5[]");
                BattleSubwayPokemon5[] party = (BattleSubwayPokemon5[])value;
                if (party.Length != 3) throw new ArgumentException("value must have length 3");
                m_party = party;
            }
        }


        public override BattleTowerProfileBase Profile
        {
            get
            {
                return m_profile;
            }

            set
            {
                m_profile = (BattleSubwayProfile5)value;
            }
        }

        public override TrendyPhraseBase PhraseChallenged
        {
            get
            {
                return m_phrase_challenged;
            }

            set
            {
                m_phrase_challenged = (TrendyPhrase5)value;
            }
        }

        public override TrendyPhraseBase PhraseWon
        {
            get
            {
                return m_phrase_won;
            }

            set
            {
                m_phrase_won = (TrendyPhrase5)value;
            }
        }

        public override TrendyPhraseBase PhraseLost
        {
            get
            {
                return m_phrase_lost;
            }

            set
            {
                m_phrase_lost = (TrendyPhrase5)value;
            }
        }

        public ushort Unknown3;

        public byte Rank;
        public byte RoomNum;
        public byte BattlesWon;
        public byte[] Unknown4; // 5 bytes, appears to always be 00 00 00 00 00?
        public ulong Unknown5; // Appears to be zero on AdmiralCurtiss's scraped data, but is very big ints on data being uploaded.
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
            writer.Write(((BattleSubwayProfile5)Profile).Save());
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
