using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;

namespace PkmnFoundations.Pokedex
{
    public class Move : PokedexRecordBase
    {
        public Move(Pokedex pokedex, int id, int type_id, LocalizedString name,
            DamageClass damage_class, int damage, int pp, int accuracy, int priority,
            BattleTargets target)
            : base(pokedex)
        {
            m_type_pair = Type.CreatePair(m_pokedex);
            m_lazy_pairs.Add(m_type_pair);

            ID = id;
            m_type_pair.Key = type_id;
            Name = name;
            DamageClass = damage_class;
            Damage = damage;
            PP = pp;
            Accuracy = accuracy;
            Priority = priority;
            Target = target;
            // todo: Nice description text
        }

        public Move(Pokedex pokedex, IDataReader reader)
            : this(
            pokedex,
            Convert.ToInt32(reader["Value"]),
            Convert.ToInt32(reader["type_id"]),
            LocalizedStringFromReader(reader, "Name_"),
            (DamageClass)Convert.ToInt32(reader["DamageClass"]),
            Convert.ToInt32(reader["Damage"]),
            Convert.ToInt32(reader["PP"]),
            Convert.ToInt32(reader["Accuracy"]),
            Convert.ToInt32(reader["Priority"]),
            (BattleTargets)Convert.ToInt32(reader["Target"])
            )
        {
        }

        public int ID { get; private set; }
        public LocalizedString Name { get; private set; }
        public DamageClass DamageClass { get; private set; }
        public int Damage { get; private set; }
        public int PP { get; private set; }
        public int Accuracy { get; private set; }
        public int Priority { get; private set; }
        public BattleTargets Target { get; private set; }

        private LazyKeyValuePair<int, PkmnFoundations.Pokedex.Type> m_type_pair;

        public int TypeID 
        {
            get { return m_type_pair.Key; }
        }
        public PkmnFoundations.Pokedex.Type Type
        {
            get { return m_type_pair.Value; }
        }
    }
}
