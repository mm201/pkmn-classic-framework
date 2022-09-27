using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class BattleTowerRecord4 : BattleTowerRecordBase
    {
        public BattleTowerRecord4(Pokedex.Pokedex pokedex)
        {
            Pokedex = pokedex;
        }

        public BattleTowerRecord4(Pokedex.Pokedex pokedex, byte[] data)
        {
            Pokedex = pokedex;
            Load(data, 0);
        }

        public BattleTowerRecord4(Pokedex.Pokedex pokedex, byte[] data, int start)
        {
            Pokedex = pokedex;
            Load(data, start);
        }

        public Pokedex.Pokedex Pokedex { get; set; }

        private BattleTowerPokemon4[] m_party;
        private BattleTowerProfile4 m_profile;
        private TrendyPhrase4 m_phrase_challenged;
        private TrendyPhrase4 m_phrase_won;
        private TrendyPhrase4 m_phrase_lost;

        public override IList<BattleTowerPokemonBase> Party
        {
            get
            {
                return m_party;
            }
            set
            {
                if (!(value is BattleTowerPokemon4[])) throw new ArgumentException("value must be BattleTowerPokemon4[]");
                BattleTowerPokemon4[] party = (BattleTowerPokemon4[])value;
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
                m_profile = (BattleTowerProfile4)value;
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
                m_phrase_challenged = (TrendyPhrase4)value;
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
                m_phrase_won = (TrendyPhrase4)value;
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
                m_phrase_lost = (TrendyPhrase4)value;
            }
        }


        public ushort Unknown3; // Seems to be some sort of Elo rating. Goes up to about 8000.

        public byte Rank;
        public byte RoomNum;
        public byte BattlesWon;
        public ulong Unknown5; // Appears to be zero on AdmiralCurtiss's scraped data, but is very big ints on data being uploaded.
        public int PID;

        public byte[] Save()
        {
            byte[] data = new byte[0xe4];
            MemoryStream ms = new MemoryStream(data);
            BinaryWriter writer = new BinaryWriter(ms);

            for (int x = 0; x < 3; x++)
            {
                writer.Write(Party[x].Save());
            }
            writer.Write(((BattleTowerProfile4)Profile).Save());
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
            if (start + 0xe4 > data.Length) throw new ArgumentOutOfRangeException("start");

            m_party = new BattleTowerPokemon4[3];
            for (int x = 0; x < 3; x++)
            {
                Party[x] = new BattleTowerPokemon4(Pokedex, data, start + x * 0x38);
            }
            Profile = new BattleTowerProfile4(data, 0xa8 + start);

            byte[] trendyPhrase = new byte[8];
            Array.Copy(data, 0xca + start, trendyPhrase, 0, 8);
            PhraseChallenged = new TrendyPhrase4(trendyPhrase);
            trendyPhrase = new byte[8];
            Array.Copy(data, 0xd2 + start, trendyPhrase, 0, 8);
            PhraseWon = new TrendyPhrase4(trendyPhrase);
            trendyPhrase = new byte[8];
            Array.Copy(data, 0xda + start, trendyPhrase, 0, 8);
            PhraseLost = new TrendyPhrase4(trendyPhrase);

            Unknown3 = BitConverter.ToUInt16(data, 0xe2 + start);
        }
    }
}
