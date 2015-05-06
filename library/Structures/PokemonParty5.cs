using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class PokemonParty5 : Pokemon5
    {
        public PokemonParty5(Pokedex.Pokedex pokedex) : base(pokedex)
        {
            Initialize();
        }

        public PokemonParty5(Pokedex.Pokedex pokedex, BinaryReader data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public PokemonParty5(Pokedex.Pokedex pokedex, byte[] data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public PokemonParty5(Pokedex.Pokedex pokedex, byte[] data, int offset)
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
                HP = BitConverter.ToUInt16(block, 6);
                // todo: validate this against computed stats
                m_stats = new IntStatValues(BitConverter.ToUInt16(block, 8),
                    BitConverter.ToUInt16(block, 10),
                    BitConverter.ToUInt16(block, 12),
                    BitConverter.ToUInt16(block, 14),
                    BitConverter.ToUInt16(block, 16),
                    BitConverter.ToUInt16(block, 18));

                Mail = new byte[56];
                Array.Copy(block, 20, Mail, 0, 56);
                Unknown7 = new byte[8];
                Array.Copy(block, 76, Unknown7, 0, 8);
                Padding = new byte[16];
                Array.Copy(block, 84, Padding, 0, 16);
            }
        }

        // todo: parse status afflictions (enum + sleep turns)
        public byte StatusAffliction { get; set; }
        public byte Unknown5 { get; set; }
        public ushort Unknown6 { get; set; }
        public byte CapsuleIndex { get; set; }
        public ushort HP { get; set; } // remaining hp
        public byte[] Mail { get; set; } // 56 bytes
        public byte[] Unknown7 { get; set; } // 8 bytes
        public byte[] Padding { get; set; } // 16 bytes. Pads the length to match the gen4 structure.

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
