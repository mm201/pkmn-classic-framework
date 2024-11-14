using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Support;
using PkmnFoundations.Wfc;

namespace PkmnFoundations.Structures
{
    public class Pokemon5 : PokemonPartyBase
    {
        public Pokemon5(Pokedex.Pokedex pokedex) : base(pokedex)
        {
            Initialize();
        }

        public Pokemon5(Pokedex.Pokedex pokedex, BinaryReader data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public Pokemon5(Pokedex.Pokedex pokedex, byte[] data) : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public Pokemon5(Pokedex.Pokedex pokedex, byte[] data, int offset)
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

                m_nature = block[25]; // todo: which nature is real? Do they both need to match?
                m_bw_flags = BitConverter.ToUInt16(block, 26);
                HiddenAbility = (m_bw_flags & 0x01) != 0;
                N = (m_bw_flags & 0x02) != 0;
                Unknown1 = BitConverter.ToUInt32(block, 28);

                EggLocationID_Plat = BitConverter.ToUInt16(block, 28);
                LocationID_Plat = BitConverter.ToUInt16(block, 30);
            }

            {
                byte[] block = blocks[2];

                NicknameEncoded = new EncodedString5(block, 0, 22);
                Unknown2 = block[22];
                Version = (Versions)block[23];
                ribbons3 = BitConverter.ToInt32(block, 24);
                Unknown3 = BitConverter.ToUInt32(block, 28);
            }

            {
                byte[] block = blocks[3];

                TrainerNameEncoded = new EncodedString5(block, 0, 16);

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
                Unknown4 = BitConverter.ToUInt16(block, 30);
            }

            byte[] ribbons = new byte[12];
            Array.Copy(BitConverter.GetBytes(ribbons1), 0, ribbons, 0, 4);
            Array.Copy(BitConverter.GetBytes(ribbons2), 0, ribbons, 4, 4);
            Array.Copy(BitConverter.GetBytes(ribbons3), 0, ribbons, 8, 4);

            Ribbons.Clear();
            UnknownRibbons.Clear();

            IDictionary<int, Ribbon> allRibbons = m_pokedex.RibbonsByGeneration(Generations.Generation5);

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
            get { return Generations.Generation5; }
        }

        private byte m_nature;
        public override Natures Nature
        {
            get
            {
                return (Natures)(m_nature % 25u);
            }
        }

        private ushort m_bw_flags;
        public bool HiddenAbility { get; set; }
        public bool N { get; set; }
        public uint Unknown1 { get; set; } // appears just after a flags region storing DW ability and N ownership.

        public EncodedString5 NicknameEncoded { get; set; } // public so trash bytes can be inspected/manipulated
        public override string Nickname
        {
            get
            {
                return (NicknameEncoded == null) ? null : NicknameEncoded.Text;
            }
            set
            {
                if (Nickname == value) return;
                if (NicknameEncoded == null) NicknameEncoded = new EncodedString5(value, 22);
                else NicknameEncoded.Text = value;
            }
        }

        public byte Unknown2 { get; set; } // appears just before Version
        public uint Unknown3 { get; set; } // appears just after the last ribbon block
        public EncodedString5 TrainerNameEncoded { get; set; }
        public override string TrainerName
        {
            get
            {
                return (TrainerNameEncoded == null) ? null : TrainerNameEncoded.Text;
            }
            set
            {
                if (TrainerName == value) return;
                if (TrainerNameEncoded == null) TrainerNameEncoded = new EncodedString5(value, 16);
                else TrainerNameEncoded.Text = value;
            }
        }

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
                // todo: fact check how BW2 stores trainer memo locations not present in BW1
                ushort eggLocationId, locationId;
                bool plat = false;// IsBw2();
                eggLocationId = (plat && EggLocationID_Plat != 0) ? EggLocationID_Plat : EggLocationID;
                locationId = (plat && LocationID_Plat != 0) ? LocationID_Plat : LocationID;

                return new TrainerMemo(m_pokedex, LocationNumbering.Generation5,
                    TrainerMemoDateTime(EggDate), TrainerMemoDateTime(Date),
                    eggLocationId, locationId, EncounterLevel == 0, EncounterLevel);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ushort Unknown4 { get; set; } // appears just after encounter type. (includes what was HGSS's pokeball field)

        // todo: obtain complete list of Pokeball IDs in Gen5. (same issue as hgss)
        public byte PokeBallID { get; set; }
        private Item m_pokeball;
        public override Pokedex.Item Pokeball
        {
            get
            {
                if (m_pokeball != null) return m_pokeball;
                if (!m_pokedex.Pokeballs.ContainsKey(PokeBallID)) return null;
                m_pokeball = m_pokedex.Pokeballs[PokeBallID];
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
                PokeBallID = (byte)(pokeballId);
            }
        }

        public override ushort HP
        {
            get
            {
                return (ushort)Stats[Structures.Stats.Hp];
            }
        }

        private bool IsBw2()
        {
            return (Version == Versions.Black2 || Version == Versions.White2);
        }

        public override int Size
        {
            get { return 136; }
        }

        public static ValidationSummary ValidateActual(PokemonBase thePokemon)
        {
            if (thePokemon.SpeciesID < 1 || thePokemon.SpeciesID > 649) return new ValidationSummary() { IsValid = false };

            try
            {
                if (thePokemon.Form == null) return new ValidationSummary() { IsValid = false };
            }
            catch (KeyNotFoundException)
            {
                return new ValidationSummary() { IsValid = false }; // form not in pokedex
            }
            if (thePokemon.SpeciesID == 641 && thePokemon.FormID != 0) return new ValidationSummary() { IsValid = false }; // therian tornadus
            if (thePokemon.SpeciesID == 642 && thePokemon.FormID != 0) return new ValidationSummary() { IsValid = false }; // therian thundrus
            if (thePokemon.SpeciesID == 645 && thePokemon.FormID != 0) return new ValidationSummary() { IsValid = false }; // therian landorus
            if (thePokemon.SpeciesID == 646 && thePokemon.FormID != 0) return new ValidationSummary() { IsValid = false }; // black/white kyurem
            if (thePokemon.SpeciesID == 647 && thePokemon.FormID != 0) return new ValidationSummary() { IsValid = false }; // resolute keldeo
            if (thePokemon.Form.Suffix == "spiky-eared") return new ValidationSummary() { IsValid = false };
            if (thePokemon.Form.Suffix == "fairy") return new ValidationSummary() { IsValid = false };
            if (thePokemon.Form.Suffix == "mega") return new ValidationSummary() { IsValid = false };
            if (thePokemon.Form.Suffix == "mega-x") return new ValidationSummary() { IsValid = false };
            if (thePokemon.Form.Suffix == "mega-y") return new ValidationSummary() { IsValid = false };
            if (thePokemon.Form.Suffix == "primal") return new ValidationSummary() { IsValid = false };
            if (thePokemon.SpeciesID == 25 && thePokemon.FormID > 0) return new ValidationSummary() { IsValid = false }; // cosplay pikachu

            if (thePokemon.HeldItemID < 0) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID >= 113 && thePokemon.HeldItemID <= 115) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID >= 120 && thePokemon.HeldItemID <= 133) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID >= 328 && thePokemon.HeldItemID <= 503) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID >= 505 && thePokemon.HeldItemID <= 536) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID == 574) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID == 576) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID >= 578 && thePokemon.HeldItemID <= 579) return new ValidationSummary() { IsValid = false };
            if (thePokemon.HeldItemID >= 592) return new ValidationSummary() { IsValid = false };

            if (thePokemon.AbilityID <= 0) return new ValidationSummary() { IsValid = false };
            if (thePokemon.AbilityID > 164) return new ValidationSummary() { IsValid = false };
            if (!thePokemon.Form.Abilities(Generations.Generation5).Abilities.Contains(thePokemon.Ability) &&
                !thePokemon.Form.Abilities(Generations.Generation5).HiddenAbilities.Contains(thePokemon.Ability) &&
                // Allow Blue Basculin to have Reckless. https://bulbapedia.bulbagarden.net/wiki/Basculin_(Pok%C3%A9mon)#Trivia
                !(thePokemon.Form.Suffix == "blue-striped" && thePokemon.AbilityID == 120))
                return new ValidationSummary() { IsValid = false };

            foreach (MoveSlot move in thePokemon.Moves)
            {
                if (move.MoveID < 0) return new ValidationSummary() { IsValid = false };
                if (move.MoveID == 165) return new ValidationSummary() { IsValid = false }; // struggle
                if (move.MoveID > 559) return new ValidationSummary() { IsValid = false };
            }

            if (thePokemon is Pokemon5)
            {
                var thePokemon5 = (Pokemon5)thePokemon;
                if (!thePokemon5.NicknameEncoded.IsValid) return new ValidationSummary() { IsValid = false };
                if (!thePokemon5.TrainerNameEncoded.IsValid) return new ValidationSummary() { IsValid = false };
            }
            else if (thePokemon is BattleSubwayPokemon5)
            {
                var theBattleSubwayPokemon5 = (BattleSubwayPokemon5)thePokemon;
                if (!theBattleSubwayPokemon5.NicknameEncoded.IsValid) return new ValidationSummary() { IsValid = false };
            }

            return new ValidationSummary() { IsValid = true };
        }
    }
}
