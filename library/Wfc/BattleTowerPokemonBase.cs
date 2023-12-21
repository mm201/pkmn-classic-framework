using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Wfc
{
    public abstract class BattleTowerPokemonBase : PokemonBase
    {
        public BattleTowerPokemonBase(Pokedex.Pokedex pokedex) : base(pokedex)
        {

        }

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

        protected static ushort CombineSpeciesForm(int species, byte form)
        {
            if (species > 0x7ff || species < 0) throw new ArgumentOutOfRangeException("species");
            if (form > 0x1f) throw new ArgumentOutOfRangeException("form");
            return (ushort)(species & 0x7ff | form << 11);
        }

        protected static int GetSpeciesFromCombined(ushort combined)
        {
            return combined & 0x7ff;
        }

        protected static byte GetFormFromCombined(ushort combined)
        {
            return (byte)(combined >> 11);
        }

        internal static void GetMovesFromArray(IList<MoveSlot> result, Pokedex.Pokedex pokedex, ushort[] moves, byte ppUps)
        {
            if (moves.Length != 4) throw new ArgumentException("moves");
            if (result.Count != 4) throw new ArgumentException("movesOut");

            for (int i = 0; i < 4; i++)
            {
                result[i] = MoveFromValues(pokedex, moves[i], (byte)(ppUps & 0x03));

                ppUps >>= 2;
            }
        }

        internal static MoveSlot MoveFromValues(Pokedex.Pokedex pokedex, ushort move, byte ppUps)
        {
            MoveSlot result = new MoveSlot(pokedex, move, ppUps, 0);
            result.RemainingPP = (byte)result.PP;
            return result;
        }

        internal static ushort[] GetArrayFromMoves(IList<MoveSlot> moves)
        {
            if (moves.Count != 4) throw new ArgumentException("moves");
            ushort[] result = new ushort[4];

            for (int i = 0; i < 4; i++)
            {
                result[i] = (ushort)moves[i].MoveID;
            }
            return result;
        }

        internal static byte GetPpUpsFromMoves(IList<MoveSlot> moves)
        {
            // The first move uses the least significant bits, moving up from there.
            // [1, 3, 0, 0] -> 0x0d
            byte ppUps = 0;
            for (int i = 0; i < 4; i++)
            {
                ppUps |= (byte)(moves[i].PPUps << (i * 2));
            }
            return ppUps;
        }

        public ushort[] GetMoveIds()
        {
            return GetArrayFromMoves(Moves);
        }

        public byte GetPpUps()
        {
            return GetPpUpsFromMoves(Moves);
        }
    }
}
