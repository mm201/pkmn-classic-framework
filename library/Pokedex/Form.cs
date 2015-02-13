using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Form : PokedexRecordBase
    {
        public Form(Pokedex pokedex, int id, int species_id, byte value,
            LocalizedString name, String suffix, int height, int weight, int experience)
            : base(pokedex)
        {
            m_species_pair = Species.CreatePair(m_pokedex);
            m_lazy_pairs.Add(m_species_pair);

            ID = id;
            m_species_pair.Key = species_id;
            Value = value;
            Name = name;
            Suffix = suffix;
            Height = height;
            Weight = weight;
            Experience = experience;
        }

        public Form(Pokedex pokedex, IDataReader reader)
            : this(
                pokedex,
                Convert.ToInt32(reader["id"]),
                Convert.ToInt32(reader["NationalDex"]),
                Convert.ToByte(reader["FormValue"]),
                LocalizedStringFromReader(reader, "Name_"),
                reader["FormSuffix"].ToString(),
                Convert.ToInt32(reader["Height"]),
                Convert.ToInt32(reader["Weight"]),
                Convert.ToInt32(reader["Experience"])
                )
        {
        }

        internal override void PrefetchRelations()
        {
            base.PrefetchRelations();
            m_form_stats = m_pokedex.FormStats(ID);
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

        private SortedList<Generations, FormStats> m_form_stats;
        public FormStats BaseStats(Generations generation)
        {
            if (m_form_stats == null) m_form_stats = m_pokedex.FormStats(ID);
            // xxx: this is O(n) and we can do O(log n) but it requires rolling
            // our own binary search and YAGNI for a list of at most 6 values.
            // http://stackoverflow.com/questions/20474896/finding-nearest-value-in-a-sorteddictionary
            return m_form_stats.Last(pair => (int)(pair.Key) <= (int)generation).Value;
        }

        public static LazyKeyValuePair<int, Form> CreatePair(Pokedex pokedex)
        {
            return new LazyKeyValuePair<int, Form>(
                k => k == 0 ? null : (pokedex == null ? null : pokedex.Forms(k)),
                v => v == null ? 0 : v.ID);
        }

        public static LazyKeyValuePair<byte, Form> CreatePairForSpecies(Pokedex pokedex, Func<Species> speciesGetter)
        {
            // 0 is a valid value here--don't map it to null!
            return new LazyKeyValuePair<byte, Form>(
                k => speciesGetter().Forms(k), 
                v => v.Value);
        }
    }
}
