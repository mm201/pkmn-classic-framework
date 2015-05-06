using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public abstract class PokemonPartyBase : PokemonBase
    {
        public PokemonPartyBase(Pokedex.Pokedex pokedex)
            : base(pokedex)
        {
            Initialize();
        }

        private void Initialize()
        {
            Ribbons = new HashSet<Ribbon>();
            UnknownRibbons = new HashSet<int>();
        }

        private int m_experience;
        public int Experience 
        {
            get
            {
                return m_experience;
            }
            set
            {
                if (m_experience == value) return;
                m_experience = value;
                m_level = null;
                // xxx: if exp changes but level doesn't, this gets invalidated for no reason.
                // (really needs observable)
                m_stats = null;
            }
        }

        private byte? m_level;
        public override byte Level
        {
            get
            {
                if (m_level == null)
                    m_level = LevelAt(m_experience, Species.GrowthRate);
                return (byte)m_level;
            }
            set
            {
                if (m_level == value) return;
                m_experience = ExperienceAt(value, Species.GrowthRate);
                m_level = value;
                m_stats = null;
            }
        }

        protected bool m_female;
        protected bool m_genderless;
        public override Genders Gender
        {
            get
            {
                if (m_genderless) return Genders.None;
                if (m_female) return Genders.Female;
                return Genders.Male;
            }
            set
            {
                m_female = value == Genders.Female;
                m_genderless = value == Genders.None;
            }
        }

        public Markings Markings { get; set; }
        public ConditionValues ContestStats { get; set; }
        public bool IsEgg { get; set; }
        public bool IsBadEgg { get; set; }

        /// <summary>
        /// this field decides whether or not its name gets reverted when it evolves.
        /// </summary>
        public bool HasNickname { get; set; }

        /// <summary>
        /// aka. obedience flag. A few pokemon, eg. Mew, are disobedient unless this is set.
        /// </summary>
        public bool FatefulEncounter { get; set; }

        public Versions Version { get; set; }

        public virtual TrainerMemo TrainerMemo { get; set; }
        
        // this is the notorious genIV encounter type flag, not used for much besides validation
        public byte EncounterType { get; set; }

        public abstract String TrainerName { get; set; }
        public TrainerGenders TrainerGender { get; set; }

        public abstract Item Pokeball { get; set; }

        public HashSet<Ribbon> Ribbons { get; private set; }

        /// <summary>
        /// This allows preservation of unknown ribbon flags when saving.
        /// </summary>
        public HashSet<int> UnknownRibbons { get; private set; }

        private IntStatValues m_stats = null;
        public virtual IntStatValues Stats
        {
            get
            {
                if (m_stats == null)
                {
                    // todo: stat formula
                    throw new NotImplementedException();
                }
                return m_stats;
            }
        }

        private byte m_pokerus_days_left = 0;
        public virtual byte PokerusDaysLeft
        {
            get
            {
                return m_pokerus_days_left;
            }
            set
            {
                if (value > 15) throw new ArgumentOutOfRangeException();
                m_pokerus_days_left = value;
            }
        }

        private byte m_pokerus_strain = 0;
        public virtual byte PokerusStrain
        {
            get
            {
                return m_pokerus_strain;
            }
            set
            {
                if (value > 15) throw new ArgumentOutOfRangeException();
                m_pokerus_strain = value;
            }
        }

        public Pokerus Pokerus
        {
            get
            {
                // note: "strain 0" is invalid and will cause the pokemon to
                // lose pokerus entirely once its days left hits 0.
                if (PokerusDaysLeft > 0) return Pokerus.Infected;
                if (PokerusStrain > 0) return Pokerus.Cured;
                return Pokerus.None;
            }
        }

        public static ushort ComputeChecksum(byte[] data)
        {
            ushort result = 0;
            for (int x = 0; x < data.Length; x += 2)
                result += BitConverter.ToUInt16(data, x);

            return result;
        }

        public static ushort ComputeChecksum(byte[][] data)
        {
            return (ushort)data.Sum(inner => ComputeChecksum(inner));
        }

        protected static int DecryptRNG(int prev)
        {
            return prev * 0x41c64e6d + 0x6073;
        }

        protected static void DecryptBlocks(byte[][] blocks, ushort checksum)
        {
            int rand = (int)checksum;

            for (int x = 0; x < 4; x++)
            {
                byte[] block = blocks[x];
                for (int pos = 0; pos < 32; pos += 2)
                {
                    rand = DecryptRNG(rand);
                    block[pos] ^= (byte)(rand >> 16);
                    block[pos + 1] ^= (byte)(rand >> 24);
                }
            }
        }

        protected static void ShuffleBlocks(byte[][] blocks, uint personality, bool unshuffle)
        {
            // shuffle blocks to their correct order
            List<int> blockSequence = BlockScramble((personality & 0x0003e000) >> 0x0d);
            if (unshuffle) blockSequence = Invert(blockSequence);

            AssertHelper.Equals(blockSequence.Count, 4);
            {
                byte[][] blocks2 = new byte[4][];
                for (int x = 0; x < 4; x++)
                    blocks2[x] = blocks[blockSequence[x]];
                for (int x = 0; x < 4; x++)
                    blocks[x] = blocks2[x];
            }
        }

        public static bool HasRibbon(byte[] ribbons, int value)
        {
            if (value >= 96 || value < 0) throw new ArgumentOutOfRangeException();
            int offset = value >> 3;
            byte mask = (byte)(1 << (value & 0x07));
            return (ribbons[offset] & mask) != 0;
        }

        protected static DateTime? TrainerMemoDateTime(byte[] data)
        {
            // todo: merge with GtsRecordBase datetime helper.
            if (data.Length != 3) throw new ArgumentException();
            if (data[1] == 0 && data[2] == 0 && data[0] == 0) return null;

            try
            {
                return new DateTime(2000 + data[0], data[1], data[2]);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }
}
