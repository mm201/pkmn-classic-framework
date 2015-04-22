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
    /// Structure used to represent Pokémon on the GTS in Generation IV.
    /// Includes a Pokémon box structure and metadata related to the trainer
    /// and request.
    /// </summary>
    public class GtsRecord4 : GtsRecordBase, IEquatable<GtsRecord4>
    {
        public GtsRecord4(Pokedex.Pokedex pokedex)
            : base(pokedex)
        {
            Initialize();
        }

        public GtsRecord4(Pokedex.Pokedex pokedex, BinaryReader data)
            : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public GtsRecord4(Pokedex.Pokedex pokedex, byte[] data)
            : base(pokedex)
        {
            Initialize();
            Load(data);
        }

        public GtsRecord4(Pokedex.Pokedex pokedex, byte[] data, int offset)
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
                if (value.Length != 0xEC) throw new ArgumentException("PKM length is incorrect");
                m_data = value;
                m_data_readonly = new ReadOnlyCollection<byte>(m_data);
            }
        }
        private byte[] m_data;
        private ReadOnlyCollection<byte> m_data_readonly;

        public byte Unknown1;
        public byte Unknown2;

        /// <summary>
        /// 16 bytes
        /// </summary>
        public EncodedString4 TrainerName;

        public ushort TrainerOT;

        protected override void Save(BinaryWriter writer)
        {
            // todo: enclose in properties and validate these when assigning.
            if (TrainerName.RawData.Length != 0x10) throw new FormatException("Trainer name length is incorrect");

            writer.Write(DataActual, 0, 0xEC);                         // 0x0000
            writer.Write(Species);                                     // 0x00EC
            writer.Write((byte)Gender);                                // 0x00EE
            writer.Write(Level);                                       // 0x00EF
            writer.Write(RequestedSpecies);                            // 0x00F0
            writer.Write((byte)RequestedGender);                       // 0x00F2
            writer.Write(RequestedMinLevel);                           // 0x00F3
            writer.Write(RequestedMaxLevel);                           // 0x00F4
            writer.Write(Unknown1);                                    // 0x00F5
            writer.Write((byte)TrainerGender);                         // 0x00F6
            writer.Write(Unknown2);                                    // 0x00F7
            writer.Write(DateToTimestamp(TimeDeposited));              // 0x00F8
            writer.Write(DateToTimestamp(TimeExchanged));              // 0x0100
            writer.Write(PID);                                         // 0x0108
            writer.Write(TrainerName.RawData, 0, 0x10);                // 0x010C
            writer.Write(TrainerOT);                                   // 0x011C
            writer.Write(TrainerCountry);                              // 0x011E
            writer.Write(TrainerRegion);                               // 0x011F
            writer.Write(TrainerClass);                                // 0x0120
            writer.Write(IsExchanged);                                 // 0x0121
            writer.Write(TrainerVersion);                              // 0x0122
            writer.Write(TrainerLanguage);                             // 0x0123
        }

        protected override void Load(BinaryReader reader)
        {
            DataActual = reader.ReadBytes(0xEC);                       // 0x0000
            Species = reader.ReadUInt16();                             // 0x00EC
            Gender = (Genders)reader.ReadByte();                       // 0x00EE
            Level = reader.ReadByte();                                 // 0x00EF
            RequestedSpecies = reader.ReadUInt16();                    // 0x00F0
            RequestedGender = (Genders)reader.ReadByte();              // 0x00F2
            RequestedMinLevel = reader.ReadByte();                     // 0x00F3
            RequestedMaxLevel = reader.ReadByte();                     // 0x00F4
            Unknown1 = reader.ReadByte();                              // 0x00F5
            TrainerGender = (TrainerGenders)reader.ReadByte();         // 0x00F6
            Unknown2 = reader.ReadByte();                              // 0x00F7
            TimeDeposited = TimestampToDate(reader.ReadUInt64());      // 0x00F8
            TimeExchanged = TimestampToDate(reader.ReadUInt64());      // 0x0100
            PID = reader.ReadInt32();                                  // 0x0108
            TrainerName = new EncodedString4(reader.ReadBytes(0x10));  // 0x010C
            TrainerOT = reader.ReadUInt16();                           // 0x011C
            TrainerCountry = reader.ReadByte();                        // 0x011E
            TrainerRegion = reader.ReadByte();                         // 0x011F
            TrainerClass = reader.ReadByte();                          // 0x0120
            IsExchanged = reader.ReadByte();                           // 0x0121
            TrainerVersion = reader.ReadByte();                        // 0x0122
            TrainerLanguage = reader.ReadByte();                       // 0x0123
        }

        public override int Size
        {
            get { return 292; }
        }

        public GtsRecord4 Clone()
        {
            // todo: I am not very efficient
            return new GtsRecord4(m_pokedex, Save());
        }

        public bool Validate()
        {
            // todo: a. legitimacy check, and b. check that pkm data matches metadata
            return true;
        }

        public bool CanTrade(GtsRecord4 other)
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

        public void FlagTraded(GtsRecord4 other)
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

        public static bool operator ==(GtsRecord4 a, GtsRecord4 b)
        {
            if ((object)a == null && (object)b == null) return true;
            if ((object)a == null || (object)b == null) return false;
            // todo: optimize me
            return a.Save().SequenceEqual(b.Save());
        }

        public static bool operator !=(GtsRecord4 a, GtsRecord4 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as GtsRecord4);
        }

        public bool Equals(GtsRecord4 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return ((int)DateToBinary(TimeDeposited) + (int)DateToBinary(TimeExchanged)) ^ PID;
        }

        internal static long DateToBinary(DateTime ? dt)
        {
            if (dt == null) return 0L;
            return ((DateTime)dt).ToBinary();
        }
    }
}
