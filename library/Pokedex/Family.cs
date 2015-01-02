using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Family : PokedexRecordBase
    {
        public Family(Pokedex pokedex, int id, int basic_male_id, int basic_female_id,
            int baby_male_id, int baby_female_id, int incense_id, byte gender_ratio)
            : base(pokedex)
        {
            m_basic_male_pair = new LazyKeyValuePair<int, Species>(k => k == 0 ? null : m_pokedex.Species(k), v => v.NationalDex);
            m_basic_female_pair = new LazyKeyValuePair<int, Species>(k => k == 0 ? null : m_pokedex.Species(k), v => v.NationalDex);
            m_baby_male_pair = new LazyKeyValuePair<int, Species>(k => k == 0 ? null : m_pokedex.Species(k), v => v.NationalDex);
            m_baby_female_pair = new LazyKeyValuePair<int, Species>(k => k == 0 ? null : m_pokedex.Species(k), v => v.NationalDex);
            m_incense_pair = new LazyKeyValuePair<int, Item>(k => k == 0 ? null : m_pokedex.Items(k), v => v.ID);
            m_lazy_pairs.Add(m_basic_male_pair);
            m_lazy_pairs.Add(m_basic_female_pair);
            m_lazy_pairs.Add(m_baby_male_pair);
            m_lazy_pairs.Add(m_baby_female_pair);
            m_lazy_pairs.Add(m_incense_pair);

            ID = id;
            m_basic_male_pair.Key = basic_male_id;
            m_basic_female_pair.Key = basic_female_id;
            m_baby_male_pair.Key = baby_male_id;
            m_baby_female_pair.Key = baby_female_id;
            m_incense_pair.Key = incense_id;
            GenderRatio = gender_ratio;
        }

        public int ID { get; private set; }
        public byte GenderRatio { get; private set; }

        private LazyKeyValuePair<int, Species> m_basic_male_pair;
        private LazyKeyValuePair<int, Species> m_basic_female_pair;
        private LazyKeyValuePair<int, Species> m_baby_male_pair;
        private LazyKeyValuePair<int, Species> m_baby_female_pair;
        private LazyKeyValuePair<int, Item> m_incense_pair;

        public int BasicMaleID 
        {
            get { return m_basic_male_pair.Key; }
        }
        public Species BasicMale 
        {
            get { return m_basic_male_pair.Value; }
        }

        public int BasicFemaleID 
        {
            get { return m_basic_female_pair.Key; }
        }
        public Species BasicFemale
        {
            get { return m_basic_female_pair.Value; }
        }

        public int BabyMaleID 
        {
            get { return m_baby_male_pair.Key; }
        }
        public Species BabyMale
        {
            get { return m_baby_male_pair.Value; }
        }

        public int BabyFemaleID 
        {
            get { return m_baby_female_pair.Key; }
        }
        public Species BabyFemale
        {
            get { return m_baby_female_pair.Value; }
        }

        public int IncenseID 
        {
            get { return m_incense_pair.Key; }
        }
        public Item Incense
        {
            get { return m_incense_pair.Value; }
        }
    }
}
