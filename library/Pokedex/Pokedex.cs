﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Pokedex
    {
        public Pokedex(Database db, bool lazy)
        {
            if (lazy) throw new NotImplementedException();

            GetAllData(db);
            BuildAdditionalIndexes();
            PrefetchRelations();
        }

        private void GetAllData(Database db)
        {
            m_species = db.PokedexGetAllSpecies(this).ToDictionary(s => s.NationalDex, s => s);
            m_families = db.PokedexGetAllFamilies(this).ToDictionary(f => f.ID, f => f);
            m_forms = db.PokedexGetAllForms(this).ToDictionary(f => f.ID, f => f);
            m_items = db.PokedexGetAllItems(this).ToDictionary(i => i.ID, i => i);
            m_moves = db.PokedexGetAllMoves(this).ToDictionary(m => m.ID, m => m);
            m_types = db.PokedexGetAllTypes(this).ToDictionary(t => t.ID, t => t);
            m_abilities = db.PokedexGetAllAbilities(this).ToDictionary(a => a.Value, a => a);
            m_ribbons = db.PokedexGetAllRibbons(this).ToDictionary(r => r.ID, r => r);
            m_regions = db.PokedexGetAllRegions(this).ToDictionary(r => r.ID, r => r);
            m_locations = db.PokedexGetAllLocations(this).ToDictionary(l => l.ID, l => l);

            List<FormStats> form_stats = db.PokedexGetAllFormStats(this);
            m_form_stats = ProcessGenerationalChangeset(form_stats, fs => fs.FormID, fs => fs.MinGeneration);

            List<FormAbilities> form_abilities = db.PokedexGetAllFormAbilities(this);
            m_form_abilities = ProcessGenerationalChangeset(form_abilities, fa => fa.FormID, fa => fa.MinGeneration);
        }

        private void BuildAdditionalIndexes()
        {
            m_forms_by_value = new Dictionary<int, Dictionary<byte, Form>>();
            foreach (var pair in m_species)
                m_forms_by_value.Add(pair.Key, new Dictionary<byte, Form>());

            foreach (var pair in m_forms)
            {
#if !DEBUG
                if (!m_forms_by_value[pair.Value.SpeciesID].ContainsKey(pair.Value.Value))
#endif
                    m_forms_by_value[pair.Value.SpeciesID].Add(pair.Value.Value, pair.Value);
            }

            Dictionary<int, Item> items3 = new Dictionary<int,Item>();
            Dictionary<int, Item> items4 = new Dictionary<int,Item>();
            Dictionary<int, Item> items5 = new Dictionary<int,Item>();
            Dictionary<int, Item> items6 = new Dictionary<int,Item>();
            m_items_generations = new Dictionary<Generations, Dictionary<int, Item>>();
            m_items_generations.Add(Generations.Generation3, items3);
            m_items_generations.Add(Generations.Generation4, items4);
            m_items_generations.Add(Generations.Generation5, items5);
            m_items_generations.Add(Generations.Generation6, items6);
            m_pokeballs = new Dictionary<int, Item>();

            foreach (var pair in m_items)
            {
                Item i = pair.Value;
                if (i.Value3 != null) items3.Add((int)i.Value3, i);
                if (i.Value4 != null) items4.Add((int)i.Value4, i);
                if (i.Value5 != null) items5.Add((int)i.Value5, i);
                if (i.Value6 != null) items6.Add((int)i.Value6, i);
                if (i.PokeballValue != null) m_pokeballs.Add((int)i.PokeballValue, i);
            }

            m_ribbon_positions_generations = new Dictionary<Generations, Dictionary<int, Ribbon>>();
            AddGeneration(m_ribbon_positions_generations, m_ribbons, Generations.Generation3, r => r.Position3);
            AddGeneration(m_ribbon_positions_generations, m_ribbons, Generations.Generation4, r => r.Position4);
            AddGeneration(m_ribbon_positions_generations, m_ribbons, Generations.Generation5, r => r.Position5);
            AddGeneration(m_ribbon_positions_generations, m_ribbons, Generations.Generation6, r => r.Position6);

            m_ribbon_values_generations = new Dictionary<Generations, Dictionary<int, Ribbon>>();
            AddGeneration(m_ribbon_values_generations, m_ribbons, Generations.Generation3, r => r.Value3);
            AddGeneration(m_ribbon_values_generations, m_ribbons, Generations.Generation4, r => r.Value4);
            AddGeneration(m_ribbon_values_generations, m_ribbons, Generations.Generation5, r => r.Value5);
            AddGeneration(m_ribbon_values_generations, m_ribbons, Generations.Generation6, r => r.Value6);

            m_location_values_generations = new Dictionary<LocationNumbering, Dictionary<int, Location>>();
            AddGeneration(m_location_values_generations, m_locations, LocationNumbering.Generation3, l => l.Value3);
            AddGeneration(m_location_values_generations, m_locations, LocationNumbering.Generation4, l => l.Value4);
            AddGeneration(m_location_values_generations, m_locations, LocationNumbering.Generation5, l => l.Value5);
            AddGeneration(m_location_values_generations, m_locations, LocationNumbering.Generation6, l => l.Value6);
        }

        private Dictionary<int, SortedList<Generations, T>> ProcessGenerationalChangeset<T>(List<T> data, Func<T, int> idGetter, Func<T, Generations> minGenerationGetter)
        {
            // xxx: Instead of passing in these two lamdbas, we want an IGenerationalChangesetItem interface with corresponding properties.

            var sorted = data.ToList();
            sorted.Sort(delegate (T f, T other)
            {
                int idF = idGetter(f);
                int idOther = idGetter(other);
                if (idF != idOther) return idF.CompareTo(idOther);

                Generations genF = minGenerationGetter(f);
                Generations genOther = minGenerationGetter(other);
                return genF.CompareTo(genOther);
            });

            Dictionary<int, SortedList<Generations, T>> resultFormStats = new Dictionary<int, SortedList<Generations, T>>();
            SortedList<Generations, T> currFormStats = null;
            int currFormId = 0;

            foreach (T f in sorted)
            {
                int idF = idGetter(f);
                if (currFormStats == null || currFormId != idF)
                {
                    if (currFormStats != null) resultFormStats.Add(currFormId, currFormStats);
                    currFormStats = new SortedList<Generations, T>();
                }
                currFormStats.Add(minGenerationGetter(f), f);
                currFormId = idF;
            }
            if (currFormStats != null) resultFormStats.Add(currFormId, currFormStats);

            return resultFormStats;
        }

        private void AddGeneration<TGen, TKey, TValue>(Dictionary<TGen, Dictionary<TKey, TValue>> dest, Dictionary<TKey, TValue> src, TGen generation, Func<TValue, TKey?> keyGetter)
            where TKey : struct
        {
            dest.Add(generation,
                src.Where(pair => keyGetter(pair.Value) != null)
                .ToDictionary(pair => (TKey)keyGetter(pair.Value), pair => pair.Value));
        }

        private void PrefetchRelations()
        {
            // xxx: clean this up
            // todo: reflect these classes to decide whether or not prefetching
            // is even needed
            foreach (var k in m_species)
                k.Value.PrefetchRelations();
            foreach (var k in m_families)
                k.Value.PrefetchRelations();
            foreach (var k in m_forms)
                k.Value.PrefetchRelations();
            foreach (var k in m_items)
                k.Value.PrefetchRelations();
            foreach (var k in m_moves)
                k.Value.PrefetchRelations();
            foreach (var k in m_types)
                k.Value.PrefetchRelations();
            foreach (var k in m_abilities)
                k.Value.PrefetchRelations();
            foreach (var k in m_ribbons)
                k.Value.PrefetchRelations();
            foreach (var k in m_regions)
                k.Value.PrefetchRelations();
            foreach (var k in m_locations)
                k.Value.PrefetchRelations();

            foreach (var k in m_form_stats)
            {
                foreach (var j in k.Value)
                    j.Value.PrefetchRelations();
            }
            foreach (var k in m_form_abilities)
            {
                foreach (var j in k.Value)
                    j.Value.PrefetchRelations();
            }
        }

        private Dictionary<int, Species> m_species;
        private Dictionary<int, Family> m_families;
        private Dictionary<int, Form> m_forms;
        private Dictionary<int, Dictionary<byte, Form>> m_forms_by_value;
        private Dictionary<int, SortedList<Generations, FormStats>> m_form_stats;
        private Dictionary<int, SortedList<Generations, FormAbilities>> m_form_abilities;
        //private Dictionary<int, Evolution> m_evolutions;

        private Dictionary<int, Item> m_items;
        private Dictionary<int, Move> m_moves;
        private Dictionary<int, PkmnFoundations.Pokedex.Type> m_types;
        private Dictionary<int, Ability> m_abilities;
        private Dictionary<int, Ribbon> m_ribbons;

        private Dictionary<Generations, Dictionary<int, Item>> m_items_generations;
        private Dictionary<int, Item> m_pokeballs;
        private Dictionary<Generations, Dictionary<int, Ribbon>> m_ribbon_positions_generations;
        private Dictionary<Generations, Dictionary<int, Ribbon>> m_ribbon_values_generations;

        private Dictionary<int, Region> m_regions;
        private Dictionary<int, Location> m_locations;

        private Dictionary<LocationNumbering, Dictionary<int, Location>> m_location_values_generations;

        // todo: use readonly wrappers
        public IDictionary<int, Species> Species
        {
            get
            {
                return m_species;
            }
        }

        public IDictionary<int, Family> Families
        {
            get
            {
                return m_families;
            }
        }

        public IDictionary<int, Form> Forms
        {
            get
            {
                return m_forms;
            }
        }

        internal Dictionary<byte, Form> FormsByValue(int national_dex)
        {
            return m_forms_by_value[national_dex];
        }

        internal SortedList<Generations, FormStats> FormStats(int form_id)
        {
            return m_form_stats[form_id];
        }

        internal SortedList<Generations, FormAbilities> FormAbilities(int form_id)
        {
            return m_form_abilities[form_id];
        }

        public IDictionary<int, Item> Items
        {
            get
            {
                return m_items;
            }
        }

        public IDictionary<int, Item> ItemsByGeneration(Generations generation)
        {
            return m_items_generations[generation];
        }

        public IDictionary<int, Item> Pokeballs
        {
            get
            {
                return m_pokeballs;
            }
        }

        public IDictionary<int, Move> Moves
        {
            get
            {
                return m_moves;
            }
        }

        public IDictionary<int, PkmnFoundations.Pokedex.Type> Types
        {
            get
            {
                return m_types;
            }
        }

        public IDictionary<int, Ability> Abilities
        {
            get
            {
                return m_abilities;
            }
        }

        public IDictionary<int, Ribbon> Ribbons
        {
            get
            {
                return m_ribbons;
            }
        }

        public IDictionary<int, Ribbon> RibbonsByGeneration(Generations generation)
        {
            return m_ribbon_positions_generations[generation];
        }

        public IDictionary<int, Region> Regions
        {
            get
            {
                return m_regions;
            }
        }

        public IDictionary<int, Location> Locations
        {
            get
            {
                return m_locations;
            }
        }

        public IDictionary<int, Location> LocationsByGeneration(LocationNumbering generation)
        {
            return m_location_values_generations[generation];
        }

        public static int SpeciesAtGeneration(Generations generation)
        {
            // xxx: Pull from database
            switch (generation)
            {
                case Generations.Generation1:
                    return 151;
                case Generations.Generation2:
                    return 251;
                case Generations.Generation3:
                    return 386;
                case Generations.Generation4:
                    return 493;
                case Generations.Generation5:
                    return 649;
                case Generations.Generation6:
                    return 721;
                case Generations.Generation7:
                    return 807; // Sorry, LGPE is Gen8
                case Generations.Generation8:
                    return 905;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
