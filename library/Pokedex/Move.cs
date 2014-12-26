using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Move
    {
        public Move(Pokedex pokedex, int id, int type_id, LocalizedString name,
            DamageClass damage_class, int damage, int pp, int accuracy, int priority,
            BattleTargets target)
        {
            m_pokedex = pokedex;
            ID = id;
            TypeID = type_id;
            Name = name;
            DamageClass = damage_class;
            Damage = damage;
            PP = pp;
            Accuracy = accuracy;
            Priority = priority;
            Target = target;
            // todo: Nice description text
        }

        private Pokedex m_pokedex;

        public int ID { get; private set; }
        // todo: this field should be private and only dealt with during construction.
        // Instead, an actual Type object should be exposed and its ID number can be retrieved if necessary.
        public int TypeID { get; private set; }
        public LocalizedString Name { get; private set; }
        public DamageClass DamageClass { get; private set; }
        public int Damage { get; private set; }
        public int PP { get; private set; }
        public int Accuracy { get; private set; }
        public int Priority { get; private set; }
        public BattleTargets Target { get; private set; }
    }
}
