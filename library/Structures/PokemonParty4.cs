using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class PokemonParty4 : Pokemon4
    {
        public PokemonParty4(Pokedex.Pokedex pokedex) : base(pokedex)
        {
            Initialize();
        }

        public PokemonParty4(Pokedex.Pokedex pokedex, BinaryReader data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public PokemonParty4(Pokedex.Pokedex pokedex, byte[] data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public PokemonParty4(Pokedex.Pokedex pokedex, byte[] data, int offset)
            : base(pokedex)
        {
            Initialize();
            Load(data, offset);
        }

        private void Initialize()
        {
        }

        protected override void Load(BinaryReader reader)
        {
            base.Load(reader);

            {
                int rand = (int)Personality;
                byte[] block = reader.ReadBytes(100);
                for (int pos = 0; pos < 100; pos += 2)
                {
                    rand = DecryptRNG(rand);
                    block[pos] ^= (byte)(rand >> 16);
                    block[pos + 1] ^= (byte)(rand >> 24);
                }

                StatusAffliction = block[0];
                Unknown5 = block[1];
                Unknown6 = BitConverter.ToUInt16(block, 2);
                // todo: validate this against the computed level
                //Level = block[4];
                CapsuleIndex = block[5];
                _HP = BitConverter.ToUInt16(block, 6);
                m_stats = new IntStatValues(BitConverter.ToUInt16(block, 8),
                    BitConverter.ToUInt16(block, 10),
                    BitConverter.ToUInt16(block, 12),
                    BitConverter.ToUInt16(block, 14),
                    BitConverter.ToUInt16(block, 16),
                    BitConverter.ToUInt16(block, 18));

                HeldItemTrash = new byte[56];
                Array.Copy(block, 20, HeldItemTrash, 0, 56);
                Seals = new byte[24];
                Array.Copy(block, 76, Seals, 0, 24);
            }
        }

        // todo: parse status afflictions (enum + sleep turns)
        public byte StatusAffliction { get; set; }
        // todo: Parse ball seals
        public byte Unknown5 { get; set; }
        public ushort Unknown6 { get; set; }
        public byte CapsuleIndex { get; set; }
        public override ushort HP { get { return _HP; } } // remaining hp
        public ushort _HP { get; set; }
        public byte[] HeldItemTrash { get; set; } // 56 bytes
        public byte[] Seals { get; set; } // 24 bytes

        // cached stats on the party structure. This gives rise to the
        // "box trick" on Gens 1-4 whereby putting the pokemon into the box
        // recalculates its stats.
        private IntStatValues m_stats;
        public override IntStatValues Stats
        {
            get { return m_stats; }
        }

        public override int Size
        {
            get { return 236; }
        }
    }
}
