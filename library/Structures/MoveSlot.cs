using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class MoveSlot
    {
        public MoveSlot(Pokedex.Pokedex pokedex, int moveId, byte ppUps, byte remainingPp)
        {
            m_pokedex = pokedex;
            Initialize();

            MoveID = moveId;
            PPUps = ppUps;
            RemainingPP = remainingPp;
        }

        public void Initialize()
        {
            m_move_pair = Move.CreatePair(m_pokedex);
        }

        private Pokedex.Pokedex m_pokedex;

        private LazyKeyValuePair<int, Move> m_move_pair;
        public int MoveID 
        {
            get { return m_move_pair.Key; }
            set { m_move_pair.Key = value; }
        }
        public Move Move
        {
            get { return m_move_pair.Value; }
            set { m_move_pair.Value = value; }
        }

        public byte PPUps { get; set; } // todo: validate range
        public byte RemainingPP { get; set; } // todo: validate range (against pokedex data and pp ups)
        public int PP { get { return Move.PP * (5 + PPUps) / 5; } }
    }
}
