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

        public Markings Markings { get; set; }
        public ConditionValues ContestStats { get; set; }
        public bool IsEgg { get; set; }

        /// <summary>
        /// this field decides whether or not its name gets reverted when it evolves.
        /// </summary>
        public bool HasNickname { get; set; }

        /// <summary>
        /// aka. obedience flag. A few pokemon, eg. Mew, are disobedient unless this is set.
        /// </summary>
        public bool FatefulEncounter { get; set; }

        public Versions Version { get; set; }

        // todo: create database-driven TrainerMemo class
        // Should expose route, region, encounter type, level, date
        //public abstract TrainerMemo TrainerMemo { get; }

        public abstract String TrainerName { get; set; }
        public TrainerGenders TrainerGender { get; set; }

        public abstract Item Pokeball { get; set; }

        public HashSet<Ribbon> Ribbons { get; private set; }

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

        public static bool HasRibbon(byte[] ribbons, int value)
        {
            if (value >= 96 || value < 0) throw new ArgumentOutOfRangeException();
            int offset = value >> 3;
            byte mask = (byte)(1 << (value & 0x07));
            return (ribbons[offset] & mask) != 0;
        }
    }
}
