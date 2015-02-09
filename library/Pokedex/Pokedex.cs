using System;
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

            List<FormStats> form_stats = db.PokedexGetAllFormStats(this);
            form_stats.Sort(delegate(FormStats f, FormStats other) 
            { 
                if (f.FormID != other.FormID) return f.FormID.CompareTo(other.FormID); 
                return f.MinGeneration.CompareTo(other.MinGeneration); 
            });

            Dictionary<int, SortedDictionary<Generations, FormStats>> resultFormStats = new Dictionary<int,SortedDictionary<Generations,FormStats>>();
            SortedDictionary<Generations, FormStats> currFormStats = null;
            int currFormId = 0;

            foreach (FormStats f in form_stats)
            {
                if (currFormStats == null || currFormId != f.FormID)
                {
                    if (currFormStats != null) resultFormStats.Add(currFormId, currFormStats);
                    currFormStats = new SortedDictionary<Generations, FormStats>();
                }
                currFormStats.Add(f.MinGeneration, f);
                currFormId = f.FormID;
            }
            if (currFormStats != null) resultFormStats.Add(currFormId, currFormStats);
            m_form_stats = resultFormStats;
        }

        private void BuildAdditionalIndexes()
        {
            m_forms_by_value = new Dictionary<int, Dictionary<byte, Form>>();
            foreach (var pair in m_species)
                m_forms_by_value.Add(pair.Key, new Dictionary<byte, Form>());

            foreach (var pair in m_forms)
                m_forms_by_value[pair.Value.SpeciesID].Add(pair.Value.Value, pair.Value);

            Dictionary<int, Item> items3 = new Dictionary<int,Item>();
            Dictionary<int, Item> items4 = new Dictionary<int,Item>();
            Dictionary<int, Item> items5 = new Dictionary<int,Item>();
            Dictionary<int, Item> items6 = new Dictionary<int,Item>();
            m_items_generations = new Dictionary<Generations, Dictionary<int, Item>>();
            m_items_generations.Add(Generations.Generation3, items3);
            m_items_generations.Add(Generations.Generation4, items4);
            m_items_generations.Add(Generations.Generation5, items5);
            m_items_generations.Add(Generations.Generation6, items6);

            foreach (var pair in m_items)
            {
                Item i = pair.Value;
                if (i.Value3 != null) items3.Add((int)i.Value3, i);
                if (i.Value4 != null) items4.Add((int)i.Value4, i);
                if (i.Value5 != null) items5.Add((int)i.Value5, i);
                if (i.Value6 != null) items6.Add((int)i.Value6, i);
            }
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

            foreach (var k in m_form_stats)
            {
                foreach (var j in k.Value)
                    j.Value.PrefetchRelations();
            }
        }

        private Dictionary<int, Species> m_species;
        private Dictionary<int, Family> m_families;
        private Dictionary<int, Form> m_forms;
        private Dictionary<int, Dictionary<byte, Form>> m_forms_by_value;
        private Dictionary<int, SortedDictionary<Generations, FormStats>> m_form_stats;
        //private Dictionary<int, Evolution> m_evolutions;

        private Dictionary<int, Item> m_items;
        private Dictionary<int, Move> m_moves;
        private Dictionary<int, PkmnFoundations.Pokedex.Type> m_types;
        private Dictionary<int, Ability> m_abilities;

        private Dictionary<Generations, Dictionary<int, Item>> m_items_generations;

        // todo: add ReadOnlyIndexer1d class, replace these methods with them
        public Species Species(int national_dex)
        {
            return m_species[national_dex];
        }

        public Family Families(int id)
        {
            return m_families[id];
        }

        public Form Forms(int id)
        {
            return m_forms[id];
        }

        internal Dictionary<byte, Form> FormsByValue(int national_dex)
        {
            return m_forms_by_value[national_dex];
        }

        public SortedDictionary<Generations, FormStats> FormStats(int form_id)
        {
            return m_form_stats[form_id];
        }

        public Item Items(int id)
        {
            return m_items[id];
        }

        public Item Items(Generations generation, int value)
        {
            return m_items_generations[generation][value];
        }

        public Move Moves(int value)
        {
            return m_moves[value];
        }

        public PkmnFoundations.Pokedex.Type Types(int id)
        {
            return m_types[id];
        }

        public Ability Abilities(int value)
        {
            return m_abilities[value];
        }
    }
}
