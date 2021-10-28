using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public abstract class BattleTowerPokemonBase : PokemonBase
    {
        public BattleTowerPokemonBase(Pokedex.Pokedex pokedex) : base(pokedex)
        {

        }

        public ushort[] Moveset;
        public byte Unknown1; // probably a bitmask of applied PP ups
        // In the main PKM structure, the remaining 2 bits on the IVs field
        // represents whether it's nicknamed and if it's an egg. Clearly, eggs
        // shouldn't be brought into Battle Tower.
        public uint IvFlags;

        public override byte Level
        {
            get
            {
                // todo: fact check that everything's lv50?
                return 50;
            }

            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
