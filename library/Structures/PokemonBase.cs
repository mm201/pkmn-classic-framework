using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public abstract class PokemonBase : BinarySerializableBase
    {
        public PokemonBase(Pokedex.Pokedex pokedex)
            : base()
        {
            m_pokedex = pokedex;
            Initialize();
        }

        public PokemonBase(Pokedex.Pokedex pokedex, BinaryReader data)
            : base()
        {
            m_pokedex = pokedex;
            Initialize();
            Load(data);
        }

        public PokemonBase(Pokedex.Pokedex pokedex, byte[] data)
            : base()
        {
            m_pokedex = pokedex;
            Initialize();
            Load(data);
        }

        public PokemonBase(Pokedex.Pokedex pokedex, byte[] data, int offset)
            : base()
        {
            m_pokedex = pokedex;
            Initialize();
            Load(data, offset);
        }

        private void Initialize()
        {
            m_species_pair = Species.CreatePair(m_pokedex);
            m_form_pair = Form.CreatePairForSpecies(m_pokedex, () => Species);
            m_item_pair = Item.CreatePairForGeneration(m_pokedex, () => Generation);
            m_ability_pair = Ability.CreatePair(m_pokedex);
        }

        protected Pokedex.Pokedex m_pokedex;
        private MoveSlot[] m_moves = new MoveSlot[4];

        private LazyKeyValuePair<int, Species> m_species_pair;
        public int SpeciesID
        { 
            get { return m_species_pair.Key; }
            set 
            {
                // changing species will cause the looked up Form to be
                // incorrect so null it out.
                // xxx: should really observer pattern this.
                m_species_pair.Key = value;
                m_form_pair.Invalidate();
            }
        }
        public Species Species
        {
            get { return m_species_pair.Value; }
            set 
            {
                m_species_pair.Value = value;
                m_form_pair.Invalidate();
            }
        }

        private LazyKeyValuePair<byte, Form> m_form_pair;
        public byte FormID
        {
            get { return m_form_pair.Key; }
            set { m_form_pair.Key = value; }
        }
        public Form Form
        {
            get { return m_form_pair.Value; }
            set { m_form_pair.Value = value; }
        }

        public abstract Generations Generation { get; }

        private LazyKeyValuePair<int, Item> m_item_pair;
        public int HeldItemID
        {
            get { return m_item_pair.Key; }
            set { m_item_pair.Key = value; }
        }
        public Item HeldItem
        {
            get { return m_item_pair.Value; }
            set { m_item_pair.Value = value; }
        }

        private LazyKeyValuePair<int, Ability> m_ability_pair;
        public int AbilityID
        {
            get { return m_ability_pair.Key; }
            set { m_ability_pair.Key = value; }
        }
        public Ability Ability
        {
            get { return m_ability_pair.Value; }
            set { m_ability_pair.Value = value; }
        }

        public MoveSlot[] Moves { get { return m_moves; } }
        public uint TrainerID { get; set; }
        public uint Personality { get; set; }
        public Natures Nature { get { return (Natures)(Personality % 25u); } }
        public byte Level { get; set; }
        public byte Happiness { get; set; }
        public Languages Language { get; set; }
        public IvStatValues IVs { get; set; }
        public ByteStatValues EVs { get; set; }
        public abstract String Nickname { get; set; }

        protected static List<int> BlockScramble(uint personality)
        {
            int x = 4; // todo: this can be an argument but YAGNI
            int xFactorial = 24;

            //if (x < 0) throw new ArgumentOutOfRangeException();
            //if (x == 0) return new int[0];

            int index;

            List<int> remaining = Enumerable.Range(0, x).ToList();
            List<int> result = new List<int>(x);

            while (x > 0)
            {
                int xMinusOneFactorial = xFactorial / x;

                index = (int)((personality % xFactorial) / xMinusOneFactorial);
                result.Add(remaining[index]);
                remaining.RemoveAt(index);

                x--;
                xFactorial = xMinusOneFactorial;
            }

            return result;
        }

        protected static List<int> Invert(List<int> arg)
        {
            // todo: this is of general utility and should go in a utility class.
            int max = arg.Max();
            List<int> result = new List<int>(max);

            for (int x = 0; x <= max; x++)
                result.Add(arg.IndexOf(x));

            return result;
        }
    }
}
