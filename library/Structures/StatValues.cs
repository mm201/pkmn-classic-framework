using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public struct StatValues
    {
        public StatValues(int hp, int attack, int defense, int speed, int special_attack, int special_defense) : this()
        {
            Hp = hp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
            SpecialAttack = special_attack;
            SpecialDefense = special_defense;
        }

        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }

        public int this[Stats stat]
        {
            get
            {
                switch (stat)
                {
                    case Stats.Hp:
                        return Hp;
                    case Stats.Attack:
                        return Attack;
                    case Stats.Defense:
                        return Defense;
                    case Stats.Speed:
                        return Speed;
                    case Stats.SpecialAttack:
                        return SpecialAttack;
                    case Stats.SpecialDefense:
                        return SpecialDefense;
                    default:
                        throw new ArgumentException();
                }
            }
            set
            {
                switch (stat)
                {
                    case Stats.Hp:
                        Hp = value;
                        break;
                    case Stats.Attack:
                        Attack = value;
                        break;
                    case Stats.Defense:
                        Defense = value;
                        break;
                    case Stats.Speed:
                        Speed = value;
                        break;
                    case Stats.SpecialAttack:
                        SpecialAttack = value;
                        break;
                    case Stats.SpecialDefense:
                        SpecialDefense = value;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public int[] ToArray()
        {
            return new int[] { Hp, Attack, Defense, Speed, SpecialAttack, SpecialDefense };
        }

        public byte[] ToByteArray()
        {
            return new byte[] { (byte)Hp, (byte)Attack, (byte)Defense, (byte)Speed, (byte)SpecialAttack, (byte)SpecialDefense };
        }
    }
}
