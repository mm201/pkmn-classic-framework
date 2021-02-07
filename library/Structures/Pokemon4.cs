using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    public class Pokemon4 : PokemonPartyBase
    {
        public Pokemon4(Pokedex.Pokedex pokedex) : base(pokedex)
        {
            Initialize();
        }

        public Pokemon4(Pokedex.Pokedex pokedex, BinaryReader data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public Pokemon4(Pokedex.Pokedex pokedex, byte[] data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public Pokemon4(Pokedex.Pokedex pokedex, byte[] data, int offset) : base(pokedex)
        {
            Initialize();
            Load(data, offset);
        }

        private void Initialize()
        {
        }

        protected override void Load(BinaryReader reader)
        {
            // header (unencrypted)
            Personality = reader.ReadUInt32();                           // 0000
            ushort zero = reader.ReadUInt16();                           // 0004
            ushort checksum = reader.ReadUInt16();                       // 0006

            // read out the main payload, apply xor decryption
            byte[][] blocks = new byte[4][];
            for (int x = 0; x < 4; x++)                                  // 0008
                blocks[x] = reader.ReadBytes(32);

            DecryptBlocks(blocks, checksum);
            ShuffleBlocks(blocks, Personality, true);

            IsBadEgg = ComputeChecksum(blocks) != checksum;

            int ribbons1, ribbons2, ribbons3;

            {
                byte[] block = blocks[0];

                SpeciesID = BitConverter.ToUInt16(block, 0);
                HeldItemID = BitConverter.ToUInt16(block, 2);
                TrainerID = BitConverter.ToUInt32(block, 4);
                Experience = BitConverter.ToInt32(block, 8);
                Happiness = block[12];
                AbilityID = block[13];
                Markings = (Markings)block[14];
                Language = (Languages)block[15];
                EVs = new ByteStatValues(block[16],
                                         block[17],
                                         block[18],
                                         block[19],
                                         block[20],
                                         block[21]);
                ContestStats = new ConditionValues(block[22],
                                                   block[23], 
                                                   block[24], 
                                                   block[25], 
                                                   block[26], 
                                                   block[27]);

                ribbons2 = BitConverter.ToInt32(block, 28);
            }

            {
                byte[] block = blocks[1];

                Moves[0] = new MoveSlot(m_pokedex, BitConverter.ToUInt16(block, 0), block[12], block[8]);
                Moves[1] = new MoveSlot(m_pokedex, BitConverter.ToUInt16(block, 2), block[13], block[9]);
                Moves[2] = new MoveSlot(m_pokedex, BitConverter.ToUInt16(block, 4), block[14], block[10]);
                Moves[3] = new MoveSlot(m_pokedex, BitConverter.ToUInt16(block, 6), block[15], block[11]);

                int ivs = BitConverter.ToInt32(block, 16);
                IVs = new IvStatValues(ivs & 0x3fffffff);
                IsEgg = (ivs & 0x40000000) != 0;
                HasNickname = (ivs & 0x80000000) != 0;

                ribbons1 = BitConverter.ToInt32(block, 20);

                byte forme = block[24];
                FatefulEncounter = (forme & 0x01) != 0;
                m_female = (forme & 0x02) != 0;
                m_genderless = (forme & 0x04) != 0;
                FormID = (byte)(forme >> 3);

                ShinyLeaves = (ShinyLeaves)block[25];
                Unknown1 = BitConverter.ToUInt16(block, 26);

                EggLocationID_Plat = BitConverter.ToUInt16(block, 28);
                LocationID_Plat = BitConverter.ToUInt16(block, 30);
            }

            {
                byte[] block = blocks[2];

                NicknameEncoded = new EncodedString4(block, 0, 22);
                Unknown2 = block[22];
                Version = (Versions)block[23];
                ribbons3 = BitConverter.ToInt32(block, 24);
                Unknown3 = BitConverter.ToUInt32(block, 28);
            }

            {
                byte[] block = blocks[3];

                TrainerNameEncoded = new EncodedString4(block, 0, 16);

                // todo: store as DateTime
                EggDate = new byte[3];
                Array.Copy(block, 16, EggDate, 0, 3);
                Date = new byte[3];
                Array.Copy(block, 19, Date, 0, 3);

                EggLocationID = BitConverter.ToUInt16(block, 22);
                LocationID = BitConverter.ToUInt16(block, 24);
                byte pokerusStatus = block[26];
                PokerusDaysLeft = (byte)(pokerusStatus & 0x0f);
                PokerusStrain = (byte)(pokerusStatus >> 4);
                PokeBallID = block[27];

                byte encounter_level = block[28];
                EncounterLevel = (byte)(encounter_level & 0x7f);
                bool trainerFemale = (encounter_level & 0x80) != 0;
                TrainerGender = trainerFemale ? TrainerGenders.Female : TrainerGenders.Male;

                EncounterType = block[29];
                PokeBallID_Hgss = block[30];
                Unknown4 = block[31];
            }

            byte[] ribbons = new byte[12];
            Array.Copy(BitConverter.GetBytes(ribbons1), 0, ribbons, 0, 4);
            Array.Copy(BitConverter.GetBytes(ribbons2), 0, ribbons, 4, 4);
            Array.Copy(BitConverter.GetBytes(ribbons3), 0, ribbons, 8, 4);

            Ribbons.Clear();
            UnknownRibbons.Clear();

            IDictionary<int, Ribbon> allRibbons = m_pokedex.Ribbons(Generations.Generation4);

            for (int x = 0; x < 96; x++)
            {
                if (PokemonPartyBase.HasRibbon(ribbons, x))
                {
                    if (allRibbons.ContainsKey(x))
                        Ribbons.Add(allRibbons[x]);
                    else
                        UnknownRibbons.Add(x);
                }
            }
        }

        protected override void Save(BinaryWriter writer)
        {
            // todo: implement save
            throw new NotImplementedException();
        }

        public override Generations Generation
        {
            get { return Generations.Generation4; }
        }

        public ShinyLeaves ShinyLeaves { get; set; }
        public ushort Unknown1 { get; set; } // appears just after a flags region storing gender, forme, shiny leaves, etc.

        public EncodedString4 NicknameEncoded { get; set; } // public so trash bytes can be inspected/manipulated
        public override string Nickname
        {
            get
            {
                return (NicknameEncoded == null) ? null : NicknameEncoded.Text;
            }
            set
            {
                if (Nickname == value) return;
                if (NicknameEncoded == null) NicknameEncoded = new EncodedString4(value, 22);
                else NicknameEncoded.Text = value;
            }
        }

        public byte Unknown2 { get; set; } // appears just before Version
        public uint Unknown3 { get; set; } // appears just after the last ribbon block
        public EncodedString4 TrainerNameEncoded { get; set; }
        public override string TrainerName
        {
            get
            {
                return (TrainerNameEncoded == null) ? null : TrainerNameEncoded.Text;
            }
            set
            {
                if (TrainerName == value) return;
                if (TrainerNameEncoded == null) TrainerNameEncoded = new EncodedString4(value, 16);
                else TrainerNameEncoded.Text = value;
            }
        }

        // Dates encoding seems to be 1 byte for year, starting in 2000,
        // 1 byte for month (1-12), and 1 byte for day of month (1-31, depending)
        // EggDate is all 0s if not hatched.
        // Platinum locations are 0 on DP, egg locations are 0 if not hatched.
        public byte[] EggDate { get; set; } // 3 bytes
        public byte[] Date { get; set; } // 3 bytes
        public ushort EggLocationID { get; set; }
        public ushort LocationID { get; set; }
        public ushort EggLocationID_Plat { get; set; }
        public ushort LocationID_Plat { get; set; }
        public byte EncounterLevel { get; set; }

        public override TrainerMemo TrainerMemo
        {
            get
            {
                // fixme: Pal Park memos are just saying Pal Park. We should be able to get the region too.
                ushort eggLocationId, locationId;
                bool plat = IsPlatHgss();
                eggLocationId = (plat && EggLocationID_Plat != 0) ? EggLocationID_Plat : EggLocationID;
                locationId = (plat && LocationID_Plat != 0) ? LocationID_Plat : LocationID;

                return new TrainerMemo(m_pokedex, LocationNumbering.Generation4, 
                    TrainerMemoDateTime(EggDate), TrainerMemoDateTime(Date), 
                    eggLocationId, locationId, EncounterLevel == 0, EncounterLevel);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public byte Unknown4 { get; set; } // appears just after HGSS pokeball

        // todo: obtain complete list of Pokeball IDs in HGSS
        public byte PokeBallID { get; set; }
        public byte PokeBallID_Hgss { get; set; }

        private Item m_pokeball;
        public override Pokedex.Item Pokeball
        {
            get 
            {
                if (m_pokeball != null) return m_pokeball;
                int pokeballId = IsHgss() ? PokeBallID_Hgss : PokeBallID;
                m_pokeball = m_pokedex.Pokeballs(pokeballId);
                return m_pokeball;
            }
            set 
            {
                if (m_pokeball == value) return;
                if (value == null) throw new ArgumentNullException();

                if (value.PokeballValue == null) throw new ArgumentException("Item is not a valid Pokeball.");
                if ((int)value.PokeballValue > 255 || (int)value.PokeballValue < 0)
                    throw new ArgumentOutOfRangeException("Pokeball ID must be within the valid range for a byte.");

                int pokeballId = (int)value.PokeballValue;
                bool is_hgss = IsHgss();
                bool is_hgss_pokeball = IsHgssPokeball(pokeballId);
                if (!is_hgss && is_hgss_pokeball) throw new NotSupportedException("Can't place an HGSS Pokeball on a DPPt Pokémon.");
                // xxx: we can probably allow an hgss pokeball on a dppt pokemon
                // although the structure won't be valid
                // todo: fact check how hgss responds in this situation.

                // todo: fact check these 3 values:
                // 1. a pokemon in an HGSS ball has a DPPt value of 4
                // 2. any pokemon from DPPt has an HGSS value of 0
                // 3. a pokemon from HGSS in a DPPt ball has equal HGSS and DPPt values? Or is its HGSS value 0?
                // (investigating pokemon in the system should be enough)
                PokeBallID = (byte)(is_hgss_pokeball ? 4 : pokeballId);
                PokeBallID_Hgss = (byte)(is_hgss ? pokeballId : 0);
            }
        }

        private bool IsPlatHgss()
        {
            return (Version == Versions.Platinum || Version == Versions.HeartGold || Version == Versions.SoulSilver);
        }

        private bool IsHgss()
        {
            return (Version == Versions.HeartGold || Version == Versions.SoulSilver);
        }

        private static bool IsHgssPokeball(int pokeballId)
        {
            return pokeballId > 16;
        }

        public override int Size
        {
            get { return 136; }
        }
    }
}
