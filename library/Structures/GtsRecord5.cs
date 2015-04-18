using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using PkmnFoundations.Support;

namespace PkmnFoundations.Structures
{
    /// <summary>
    /// Structure used to represent Pokémon on the GTS in Generation V.
    /// Includes a Pokémon box structure and metadata related to the trainer
    /// and request.
    /// </summary>
    public class GtsRecord5 : GtsRecordBase, IEquatable<GtsRecord5>
    {
        public GtsRecord5()
        {
            Initialize();
        }

        public GtsRecord5(byte[] data)
        {
            Initialize();
            Load(data);
        }

        private void Initialize()
        {

        }

        // xxx: Data and Unknown0 should be one field.

        /// <summary>
        /// Obfuscated Pokémon (pkm) data. 220 bytes
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// National Dex species number
        /// </summary>
        public ushort Species;

        /// <summary>
        /// Pokémon gender
        /// </summary>
        public Genders Gender;

        /// <summary>
        /// Pokémon level
        /// </summary>
        public byte Level;

        /// <summary>
        /// Requested National Dex species number
        /// </summary>
        public ushort RequestedSpecies;

        public Genders RequestedGender;

        public byte RequestedMinLevel;
        public byte RequestedMaxLevel;
        public byte Unknown1;
        public TrainerGenders TrainerGender;
        public byte Unknown2;

        public DateTime ? TimeDeposited;
        public DateTime ? TimeExchanged;

        /// <summary>
        /// User ID of the player (not Personality Value)
        /// </summary>
        public int PID;

        public uint TrainerOT;

        /// <summary>
        /// 16 bytes
        /// </summary>
        public EncodedString5 TrainerName;

        public byte TrainerCountry;
        public byte TrainerRegion;
        public byte TrainerClass;

        public byte IsExchanged;

        public byte TrainerVersion;
        public byte TrainerLanguage;

        public byte TrainerBadges; // speculative. Usually 8.
        public byte TrainerUnityTower;

        protected override void Save(BinaryWriter writer)
        {
            // todo: enclose in properties and validate these when assigning.
            if (Data.Length != 0xEC) throw new FormatException("PKM length is incorrect");
            if (TrainerName.RawData.Length != 0x10) throw new FormatException("Trainer name length is incorrect");

            writer.Write(Data, 0, 0xEC);                               // 0x0000
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
            writer.Write(TrainerOT);                                   // 0x010C
            writer.Write(TrainerName.RawData, 0, 0x10);                // 0x0110
            writer.Write(TrainerCountry);                              // 0x0120
            writer.Write(TrainerRegion);                               // 0x0121
            writer.Write(TrainerClass);                                // 0x0122
            writer.Write(IsExchanged);                                 // 0x0123
            writer.Write(TrainerVersion);                              // 0x0124
            writer.Write(TrainerLanguage);                             // 0x0125
            writer.Write(TrainerBadges);                               // 0x0126
            writer.Write(TrainerUnityTower);                           // 0x0127
        }

        protected override void Load(BinaryReader reader)
        {
            Data = reader.ReadBytes(0xEC);                             // 0x0000
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
            TrainerOT = reader.ReadUInt32();                           // 0x010C
            TrainerName = new EncodedString5(reader.ReadBytes(0x10));  // 0x0110
            TrainerCountry = reader.ReadByte();                        // 0x0120
            TrainerRegion = reader.ReadByte();                         // 0x0121
            TrainerClass = reader.ReadByte();                          // 0x0122
            IsExchanged = reader.ReadByte();                           // 0x0123
            TrainerVersion = reader.ReadByte();                        // 0x0124
            TrainerLanguage = reader.ReadByte();                       // 0x0125
            TrainerBadges = reader.ReadByte();                         // 0x0126
            TrainerUnityTower = reader.ReadByte();                     // 0x0127
        }

        public override int Size
        {
            get { return 296; }
        }

        public GtsRecord5 Clone()
        {
            // todo: I am not very efficient
            return new GtsRecord5(Save());
        }

        public bool Validate()
        {
            // todo: a. legitimacy check, and b. check that pkm data matches metadata
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
