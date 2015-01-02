using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Species : PokedexRecordBase
    {
        public Species(Pokedex pokedex, int national_dex, int family_id, LocalizedString name, 
            GrowthRates growth_rate, byte gender_ratio, EggGroups egg_group_1, 
            EggGroups egg_group_2, int egg_steps, bool gender_variations)
            : base(pokedex)
        {
            NationalDex = national_dex;
            Name = name;
            GrowthRate = growth_rate;
            GenderRatio = gender_ratio;
            EggGroup1 = egg_group_1;
            EggGroup2 = egg_group_2;
            EggSteps = egg_steps;
            GenderVariations = gender_variations;

            m_family_pair = new LazyKeyValuePair<int, Family>(k => k == 0 ? null : m_pokedex.Families(k), v => v.ID);
            m_lazy_pairs.Add(m_family_pair);

            m_family_pair.Key = family_id;
        }

        // todo: Implement IEquitable and compare against NationalDex
        // Same goes for all these pokedex classes.

        public int NationalDex { get; private set; }
        public LocalizedString Name { get; private set; }
        public GrowthRates GrowthRate { get; private set; }
        public byte GenderRatio { get; private set; }
        public EggGroups EggGroup1 { get; private set; }
        public EggGroups EggGroup2 { get; private set; }
        public int EggSteps { get; private set; }
        public bool GenderVariations { get; private set; }

        private LazyKeyValuePair<int, Family> m_family_pair;
        public int FamilyID 
        { 
            get { return m_family_pair.Key; }
        }
        public Family Family
        {
            get { return m_family_pair.Value; }
        }

    }
}
