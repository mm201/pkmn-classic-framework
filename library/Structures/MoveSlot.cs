using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;

namespace PkmnFoundations.Structures
{
    public struct MoveSlot
    {
        public MoveSlot(Pokedex.Pokedex pokedex, int moveId, byte ppUps, byte remainingPp) : this()
        {
            m_pokedex = pokedex;
            MoveID = moveId;
            PPUps = ppUps;
            RemainingPP = remainingPp;
        }

        private Pokedex.Pokedex m_pokedex;

        public int MoveID { get; set; }
        public byte PPUps { get; set; } // todo: validate range
        public byte RemainingPP { get; set; } // todo: validate range (against pokedex data and pp ups)

        // todo: should have a MoveID/Move LazyKeyValuePair.
        public Move Move { get { return m_pokedex.Moves(MoveID); } }
        public int PP { get { return Move.PP * (5 + PPUps) / 5; } }
    }
}
