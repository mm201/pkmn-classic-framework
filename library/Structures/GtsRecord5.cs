using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using PkmnFoundations.Support;
using System.Collections.ObjectModel;

namespace PkmnFoundations.Structures
{
    /// <summary>
    /// Structure used to represent Pokémon on the GTS in Generation V.
    /// Includes a Pokémon box structure and metadata related to the trainer
    /// and request.
    /// </summary>
    public class GtsRecord5 : GtsRecordBase, IEquatable<GtsRecord5>
    {
        public GtsRecord5(Pokedex.Pokedex pokedex)
            : base(pokedex)
        {
            Initialize();
        }

        public GtsRecord5(Pokedex.Pokedex pokedex, BinaryReader data)
            : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public GtsRecord5(Pokedex.Pokedex pokedex, byte[] data)
            : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public GtsRecord5(Pokedex.Pokedex pokedex, byte[] data, int offset)
            : base(pokedex)
        {
            Initialize();
            Load(data, offset);
        }

        private void Initialize()
        {

        }

        /// <summary>
        /// Obfuscated Pokémon (pkm) data. 236 bytes
        /// </summary>
        public IList<byte> Data
        {
            get
            {
                return m_data_readonly;
            }
            set
            {
                DataActual = value.ToArray();
            }
        }

        private byte[] DataActual
        {
            get
            {
                return m_data;
            }
            set
            {
                if (value == null)
                {
                    m_data = null;
                    m_data_readonly = null;
                    m_pokemon = null;
                    return;
                }
                if (value.Length != 0xEC) throw new ArgumentException("PKM length is incorrect");
                m_data = value;
                m_data_readonly = new ReadOnlyCollection<byte>(m_data);
                m_pokemon = null;
            }
        }
        private byte[] m_data;
        private ReadOnlyCollection<byte> m_data_readonly;
        private PokemonParty5 m_pokemon;

        public override PokemonPartyBase Pokemon
        {
            get
            {
                if (DataActual == null || m_pokedex == null)
                    return null;
                if (m_pokemon == null)
                    m_pokemon = new PokemonParty5(m_pokedex, DataActual);

                return m_pokemon;
            }
        }

        public byte Unknown1;
        public byte Unknown2;

        public uint TrainerOT;

        /// <summary>
        /// 16 bytes
        /// </summary>
        public EncodedString5 TrainerNameEncoded;

        public override string TrainerName
        {
            get
            {
                if (TrainerNameEncoded == null) return null;
                return TrainerNameEncoded.Text;
            }
            set
            {
                if (TrainerNameEncoded == null) TrainerNameEncoded = new EncodedString5(value, 16);
                else TrainerNameEncoded.Text = value;
            }
        }

        public byte TrainerBadges; // speculative. Usually 8.
        public byte TrainerUnityTower;

        protected override void Save(BinaryWriter writer)
        {
            // todo: enclose in properties and validate these when assigning.
            if (TrainerNameEncoded.RawData.Length != 0x10) throw new FormatException("Trainer name length is incorrect");

            writer.Write(DataActual, 0, 0xEC);                           // 0000
            writer.Write(Species);                                       // 00EC
            writer.Write((byte)Gender);                                  // 00EE
            writer.Write(Level);                                         // 00EF
            writer.Write(RequestedSpecies);                              // 00F0
            writer.Write((byte)RequestedGender);                         // 00F2
            writer.Write(RequestedMinLevel);                             // 00F3
            writer.Write(RequestedMaxLevel);                             // 00F4
            writer.Write(Unknown1);                                      // 00F5
            writer.Write((byte)TrainerGender);                           // 00F6
            writer.Write(Unknown2);                                      // 00F7
            writer.Write(DateToTimestamp(TimeDeposited));                // 00F8
            writer.Write(DateToTimestamp(TimeExchanged));                // 0100
            writer.Write(PID);                                           // 0108
            writer.Write(TrainerOT);                                     // 010C
            writer.Write(TrainerNameEncoded.RawData, 0, 0x10);           // 0110
            writer.Write(TrainerCountry);                                // 0120
            writer.Write(TrainerRegion);                                 // 0121
            writer.Write(TrainerClass);                                  // 0122
            writer.Write(IsExchanged);                                   // 0123
            writer.Write(TrainerVersion);                                // 0124
            writer.Write(TrainerLanguage);                               // 0125
            writer.Write(TrainerBadges);                                 // 0126
            writer.Write(TrainerUnityTower);                             // 0127
        }

        protected override void Load(BinaryReader reader)
        {
            DataActual = reader.ReadBytes(0xEC);                         // 0000
            Species = reader.ReadUInt16();                               // 00EC
            Gender = (Genders)reader.ReadByte();                         // 00EE
            Level = reader.ReadByte();                                   // 00EF
            RequestedSpecies = reader.ReadUInt16();                      // 00F0
            RequestedGender = (Genders)reader.ReadByte();                // 00F2
            RequestedMinLevel = reader.ReadByte();                       // 00F3
            RequestedMaxLevel = reader.ReadByte();                       // 00F4
            Unknown1 = reader.ReadByte();                                // 00F5
            TrainerGender = (TrainerGenders)reader.ReadByte();           // 00F6
            Unknown2 = reader.ReadByte();                                // 00F7
            TimeDeposited = TimestampToDate(reader.ReadUInt64());        // 00F8
            TimeExchanged = TimestampToDate(reader.ReadUInt64());        // 0100
            PID = reader.ReadInt32();                                    // 0108
            TrainerOT = reader.ReadUInt32();                             // 010C
            TrainerNameEncoded = new EncodedString5(reader.ReadBytes(0x10)); // 0110
            TrainerCountry = reader.ReadByte();                          // 0120
            TrainerRegion = reader.ReadByte();                           // 0121
            TrainerClass = reader.ReadByte();                            // 0122
            IsExchanged = reader.ReadByte();                             // 0123
            TrainerVersion = reader.ReadByte();                          // 0124
            TrainerLanguage = reader.ReadByte();                         // 0125
            TrainerBadges = reader.ReadByte();                           // 0126
            TrainerUnityTower = reader.ReadByte();                       // 0127
        }

        public override int Size
        {
            get { return 296; }
        }

        public GtsRecord5 Clone()
        {
            // todo: I am not very efficient
            return new GtsRecord5(m_pokedex, Save());
        }

        public override bool Validate()
        {
            if (!base.Validate()) return false;

            // todo: legitimacy check
            return true;
        }

        public bool CanTrade(GtsRecord5 other)
        {
            if (IsExchanged != 0 || other.IsExchanged != 0) return false;

            if (Species != other.RequestedSpecies) return false;
            if (other.RequestedGender != Genders.Either && Gender != other.RequestedGender) return false;
            if (!CheckLevels(other.RequestedMinLevel, other.RequestedMaxLevel, Level)) return false;
            
            if (RequestedSpecies != other.Species) return false;
            if (RequestedGender != Genders.Either && other.Gender != RequestedGender) return false;
            if (!CheckLevels(RequestedMinLevel, RequestedMaxLevel, other.Level)) return false;

            return true;
        }

        public void FlagTraded(GtsRecord5 other)
        {
            Species = other.Species;
            Gender = other.Gender;
            Level = other.Level;
            RequestedSpecies = other.RequestedSpecies;
            RequestedGender = other.RequestedGender;
            RequestedMinLevel = other.RequestedMinLevel;
            RequestedMaxLevel = other.RequestedMaxLevel;
            TimeDeposited = other.TimeDeposited;
            TimeExchanged = DateTime.UtcNow;
            PID = other.PID;
            IsExchanged = 0x01;
        }

        public static bool operator ==(GtsRecord5 a, GtsRecord5 b)
        {
            if ((object)a == null && (object)b == null) return true;
            if ((object)a == null || (object)b == null) return false;
            // todo: optimize me
            return a.Save().SequenceEqual(b.Save());
        }

        public static bool operator !=(GtsRecord5 a, GtsRecord5 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as GtsRecord5);
        }

        public bool Equals(GtsRecord5 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return ((int)GtsRecord4.DateToBinary(TimeDeposited) + (int)GtsRecord4.DateToBinary(TimeExchanged)) ^ PID;
        }
    }
}
