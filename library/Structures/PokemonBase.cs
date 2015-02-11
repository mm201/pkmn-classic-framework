using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public abstract class PokemonBase : BinarySerializableBase
    {
        public PokemonBase(Pokedex.Pokedex pokedex)
            : base()
        {
            m_pokedex = pokedex;
        }

        public PokemonBase(Pokedex.Pokedex pokedex, BinaryReader data)
            : base()
        {
            m_pokedex = pokedex;
            Load(data);
        }

        public PokemonBase(Pokedex.Pokedex pokedex, byte[] data)
            : base()
        {
            m_pokedex = pokedex;
            Load(data);
        }

        public PokemonBase(Pokedex.Pokedex pokedex, byte[] data, int offset)
            : base()
        {
            m_pokedex = pokedex;
            Load(data, offset);
        }

        protected Pokedex.Pokedex m_pokedex;
        private MoveSlot[] m_moves = new MoveSlot[4];

        // todo: use LazyKeyValuePairs for the ID fields.
        public int SpeciesID { get; set; }
        public int FormID { get; set; }
        public int HeldItemID { get; set; }
        public MoveSlot[] Moves { get { return m_moves; } }
        public uint TrainerID { get; set; }
        public uint Personality { get; set; }
        public Natures Nature { get { return (Natures)(Personality % 25u); } }
        public byte Level { get; set; }
        public byte Happiness { get; set; }
        public int AbilityID { get; set; }
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
