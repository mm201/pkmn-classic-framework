using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Location : PokedexRecordBase
    {
        public Location(Pokedex pokedex, int id, int region_id, int ? value3,
            int ? value_colo, int ? value_xd, int ? value4, int ? value5,
            int ? value6, LocalizedString name)
            : base(pokedex)
        {
            m_region_pair = Region.CreatePair(m_pokedex);
            m_lazy_pairs.Add(m_region_pair);

            ID = id;
            m_region_pair.Key = region_id;
            Value3 = value3;
            ValueColo = value_colo;
            ValueXd = value_xd;
            Value4 = value4;
            Value5 = value5;
            Value6 = value6;
            Name = name;
        }

        public Location(Pokedex pokedex, IDataReader reader)
            : this(pokedex, 
            Convert.ToInt32(reader["id"]), 
            Convert.ToInt32(reader["region_id"]), 
            (int ?)DatabaseExtender.Cast<uint ?>(reader["Value3"]),
            (int?)DatabaseExtender.Cast<uint?>(reader["Value_Colo"]),
            (int?)DatabaseExtender.Cast<uint?>(reader["Value_XD"]),
            (int?)DatabaseExtender.Cast<uint?>(reader["Value4"]),
            (int?)DatabaseExtender.Cast<uint?>(reader["Value5"]),
            (int?)DatabaseExtender.Cast<uint?>(reader["Value6"]),
            LocalizedStringFromReader(reader, "Name_"))
        {
        }

        public int ID { get; private set; }
        public LocalizedString Name { get; private set; }

        private LazyKeyValuePair<int, Region> m_region_pair;
        public int RegionID
        {
            get { return m_region_pair.Key; }
        }
        public Region Region
        {
            get { return m_region_pair.Value; }
        }

        // xxx: All these 3/4/5/6 fields are repetitive.
        // We need a GenerationField<T> helper which bottles them all up into
        // one field. Basically Dictionary<Generations, T> but with a helper to
        // pull out a T ?
        public int? Value3 { get; private set; }
        public int? ValueColo { get; private set; }
        public int? ValueXd { get; private set; }
        public int? Value4 { get; private set; }
        public int? Value5 { get; private set; }
        public int? Value6 { get; private set; }

        public int? Value(Generations generation)
        {
            LocationNumbering numbering = GenerationToLocationNumbering(generation);
            return Value(numbering);
        }

        public int ? Value(LocationNumbering numbering)
        {
            switch (numbering)
            {
                case LocationNumbering.Generation1:
                case LocationNumbering.Generation2:
                    throw new NotSupportedException();
                case LocationNumbering.Generation3:
                    return Value3;
                case LocationNumbering.Colosseum:
                    return ValueColo;
                case LocationNumbering.XD:
                    return ValueXd;
                case LocationNumbering.Generation4:
                    return Value4;
                case LocationNumbering.Generation5:
                    return Value5;
                case LocationNumbering.Generation6:
                    return Value6;
                default:
                    throw new ArgumentException();
            }
        }

        public static LocationNumbering GenerationToLocationNumbering(Generations generation)
        {
            switch (generation)
            {
                case Generations.Generation1:
                    return LocationNumbering.Generation1;
                case Generations.Generation2:
                    return LocationNumbering.Generation2;
                case Generations.Generation3:
                    return LocationNumbering.Generation3;
                case Generations.Generation4:
                    return LocationNumbering.Generation4;
                case Generations.Generation5:
                    return LocationNumbering.Generation5;
                case Generations.Generation6:
                    return LocationNumbering.Generation6;
                default:
                    throw new ArgumentException();
            }
        }

        public static LazyKeyValuePair<int, Location> CreatePair(Pokedex pokedex)
        {
            return new LazyKeyValuePair<int, Location>(
                k => k == 0 ? null : (pokedex == null ? null : pokedex.Locations(k)),
                v => v == null ? 0 : v.ID);
        }

        public static LazyKeyValuePair<int, Location> CreatePairForGeneration(Pokedex pokedex, Func<Generations> generationGetter)
        {
            return new LazyKeyValuePair<int, Location>(
                k =>
                {
                    if (k == 0) return null;
                    if (pokedex == null) return null;
                    var locations = pokedex.Locations(GenerationToLocationNumbering(generationGetter()));
                    if (locations == null) return null;
                    if (!locations.ContainsKey(k)) return null;
                    return locations[k];
                },
                v => v == null ? 0 : (v.Value(generationGetter()) ?? -1));
        }

        public static LazyKeyValuePair<int, Location> CreatePairForLocationNumbering(Pokedex pokedex, Func<LocationNumbering> generationGetter)
        {
            return new LazyKeyValuePair<int, Location>(
                k =>
                {
                    if (k == 0) return null;
                    if (pokedex == null) return null;
                    var locations = pokedex.Locations(generationGetter());
                    if (locations == null) return null;
                    if (!locations.ContainsKey(k)) return null;
                    return locations[k];
                },
                v => v == null ? 0 : (v.Value(generationGetter()) ?? -1));
        }
    }
}
