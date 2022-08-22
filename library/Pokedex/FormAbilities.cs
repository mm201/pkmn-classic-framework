using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Pokedex
{
    public class FormAbilities : PokedexRecordBase
    {
        public FormAbilities(Pokedex pokedex, int form_id, Generations min_generation,
            int ability1, int ability2, int hiddenAbility1)
            : base(pokedex)
        {
            m_form_pair = Form.CreatePair(m_pokedex);
            m_ability1_pair = Ability.CreatePair(m_pokedex);
            m_ability2_pair = Ability.CreatePair(m_pokedex);
            m_hidden_ability1_pair = Ability.CreatePair(m_pokedex);
            m_lazy_pairs.Add(m_form_pair);
            m_lazy_pairs.Add(m_ability1_pair);
            m_lazy_pairs.Add(m_hidden_ability1_pair);

            m_form_pair.Key = form_id;
            MinGeneration = min_generation;
            m_ability1_pair.Key = ability1;
            m_ability2_pair.Key = ability2;
            m_hidden_ability1_pair.Key = hiddenAbility1;
        }

        public FormAbilities(Pokedex pokedex, IDataReader reader)
            : this(
                pokedex,
            Convert.ToInt32(reader["form_id"]),
            (Generations)Convert.ToInt32(reader["MinGeneration"]),
            Convert.ToInt32(reader["Ability1"]),
            Convert.ToInt32(reader["Ability2"]),
            Convert.ToInt32(reader["HiddenAbility1"])
            )
        {
        }

        public Generations MinGeneration { get; private set; }
        private LazyKeyValuePair<int, Form> m_form_pair;
        private LazyKeyValuePair<int, Ability> m_ability1_pair;
        private LazyKeyValuePair<int, Ability> m_ability2_pair;
        private LazyKeyValuePair<int, Ability> m_hidden_ability1_pair;

        public int FormID
        {
            get { return m_form_pair.Key; }
        }
        public Form Form
        {
            get { return m_form_pair.Value; }
        }

        public int Ability1ID
        {
            get { return m_ability1_pair.Key; }
        }
        public Ability Ability1
        {
            get { return m_ability1_pair.Value; }
        }

        public int Ability2ID
        {
            get { return m_ability2_pair.Key; }
        }
        public Ability Ability2
        {
            get { return m_ability2_pair.Value; }
        }

        public int HiddenAbility1ID
        {
            get { return m_hidden_ability1_pair.Key; }
        }
        public Ability HiddenAbility1
        {
            get { return m_hidden_ability1_pair.Value; }
        }

        public Ability[] Abilities
        { 
            get 
            {
                // xxx: We probably want the actual data to be stored in a collection to avoid this silly if-else.
                if (Ability1 != null && Ability2 != null)
                    return new Ability[] { Ability1, Ability2 };
                else if (Ability1 != null)
                    return new Ability[] { Ability1 };
                else if (Ability2 != null)
                    return new Ability[] { Ability2 };
                else
                    return new Ability[] { };
            } 
        }

    }
}
