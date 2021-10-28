using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class IvStatValues : ByteStatValues
    {
        public IvStatValues(byte hp, byte attack, byte defense, byte speed, byte special_attack, byte special_defense)
            : base(hp, attack, defense, speed, special_attack, special_defense)
        {
        }

        public IvStatValues(IEnumerable<byte> s) : base(s)
        {
            foreach (byte b in s)
            {
                if (b > 31) throw new ArgumentOutOfRangeException();
            }
        }

        public IvStatValues(int ivs) : base(new byte[6])
        {
            for (int x = 0; x < 6; x++)
            {
                Stats[x] = (byte)(ivs & 31);
                ivs >>= 5;
            }
        }

        public override byte this[Stats stat]
        {
            get
            {
                return base[stat];
            }
            set
            {
                if (value > 31) throw new ArgumentOutOfRangeException();
                base[stat] = value;
            }
        }

        public int ToInt32()
        {
            int shift = 0;
            int result = 0;
            foreach (byte iv in Stats)
            {
                result |= iv << shift;
                shift += 5;
            }
            return result;
        }

        public static byte UnpackIV(uint ivs, Stats stat)
        {
            int shift = (int)stat * 5 - 5;
            return (byte)(ivs << shift & 0x1f);
        }

        public static uint PackIVs(byte HP, byte Attack, byte Defense, byte Speed, byte SpAttack, byte SpDefense)
        {
            return (uint)((HP & 31) |
                ((Attack & 31) << 5) |
                ((Defense & 31) << 10) |
                ((Speed & 31) << 15) |
                ((SpAttack & 31) << 20) |
                ((SpDefense & 31) << 25));
        }

        public JudgeSummary JudgeSummary
        {
            get
            {
                Potential overall = Potential.Decent;
                int overallInt = Stats.Sum(s => (int)s);
                if (overallInt > 90) overall = Potential.AboveAverage;
                if (overallInt > 120) overall = Potential.RelativelySuperior;
                if (overallInt > 150) overall = Potential.Outstanding;

                byte bestIvValue = 0;
                StatFlags bestIvs = StatFlags.None;
                StatFlags zeroIvs = StatFlags.None;
                int current = 1;
                for (int x = 0; x < Stats.Length; x++)
                {
                    if (Stats[x] > bestIvValue)
                    {
                        bestIvValue = Stats[x];
                        bestIvs = (StatFlags)current;
                    }
                    else if (Stats[x] == bestIvValue)
                    {
                        bestIvs |= (StatFlags)current;
                    }
                    if (Stats[x] == 0)
                    {
                        zeroIvs |= (StatFlags)current;
                    }
                    current <<= 1;
                }

                Potential bestPotential = Potential.Decent;
                if (bestIvValue > 15) bestPotential = Potential.AboveAverage;
                if (bestIvValue > 25) bestPotential = Potential.RelativelySuperior;
                if (bestIvValue > 30) bestPotential = Potential.Outstanding;

                return new JudgeSummary(overall, bestIvs, bestPotential, zeroIvs);
            }
        }
    }

    public struct JudgeSummary
    {
        public Potential OverallPotential;
        public StatFlags BestIvs;
        public Potential BestPotential;
        public StatFlags ZeroIvs;

        public JudgeSummary(Potential overall_potential, StatFlags best_ivs, Potential best_potential, StatFlags zero_ivs)
        {
            OverallPotential = overall_potential;
            BestIvs = best_ivs;
            BestPotential = best_potential;
            ZeroIvs = zero_ivs;
        }
    }
}
