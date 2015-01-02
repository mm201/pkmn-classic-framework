using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Form : PokedexRecordBase
    {
        public Form(Pokedex pokedex, int id, int species_id, byte value,
            LocalizedString name, String suffix, int height, int weight, int experience)
            : base(pokedex)
        {
            ID = id;
            Value = value;
            Name = name;
            Suffix = suffix;
            Height = height;
            Weight = weight;
            Experience = experience;

            m_species_pair = new LazyKeyValuePair<int, Species>(k => k == 0 ? null : m_pokedex.Species(k), v => v.NationalDex);
            m_lazy_pairs.Add(m_species_pair);
            m_species_pair.Key = species_id;
        }

        public int ID { get; private set; }
        public byte Value { get; private set; }
        public LocalizedString Name { get; private set; }
        public String Suffix { get; private set; }
        public int Height { get; private set; }
        public int Weight { get; private set; }
        public int Experience { get; private set; }

        private LazyKeyValuePair<int, Species> m_species_pair;
        public int SpeciesID
        {
            get { return m_species_pair.Key; }
        }
        public Species Species
        {
            get { return m_species_pair.Value; }
        }
    }
}
