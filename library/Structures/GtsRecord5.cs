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
    [Serializable()]
    public class GtsRecord5
    {
        public GtsRecord5()
        {
        }

        public GtsRecord5(byte[] data)
        {
            Load(data);
        }

        /// <summary>
        /// Obfuscated Pokémon (pkm) data. 220 bytes
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Unknown padding between pkm and rest of data. 16 bytes.
        /// </summary>
        public byte[] Unknown0;

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
        public GtsTrainerGenders TrainerGender;
        public byte Unknown2;

        public DateTime ? TimeDeposited;
        public DateTime ? TimeWithdrawn;

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

        public byte[] Save()
        {
            // todo: enclose in properties and validate these when assigning.
            if (Data.Length != 0xDC) throw new FormatException("PKM length is incorrect");
            if (TrainerName.RawData.Length != 0x10) throw new FormatException("Trainer name length is incorrect");
            byte[] data = new byte[296];
            MemoryStream s = new MemoryStream(data);
            s.Write(Data, 0, 0xDC);
            s.Write(Unknown0, 0, 0x10);
            s.Write(BitConverter.GetBytes(Species), 0, 2);
            s.WriteByte((byte)Gender);
            s.WriteByte(Level);
            s.Write(BitConverter.GetBytes(RequestedSpecies), 0, 2);
            s.WriteByte((byte)RequestedGender);
            s.WriteByte(RequestedMinLevel);
            s.WriteByte(RequestedMaxLevel);
            s.WriteByte(Unknown1);
            s.WriteByte((byte)TrainerGender);
            s.WriteByte(Unknown2);
            s.Write(BitConverter.GetBytes(DateToTimestamp(TimeDeposited)), 0, 8);
            s.Write(BitConverter.GetBytes(DateToTimestamp(TimeWithdrawn)), 0, 8);
            s.Write(BitConverter.GetBytes(PID), 0, 4);
            s.Write(BitConverter.GetBytes(TrainerOT), 0, 4);
            s.Write(TrainerName.RawData, 0, 0x10);
            s.WriteByte(TrainerCountry);
            s.WriteByte(TrainerRegion);
            s.WriteByte(TrainerClass);
            s.WriteByte(IsExchanged);
            s.WriteByte(TrainerVersion);
            s.WriteByte(TrainerLanguage);
            s.WriteByte(TrainerBadges);
            s.WriteByte(TrainerUnityTower);
            s.Close();
            return data;
        }

        public void Load(byte[] data)
        {
            if (data.Length != 296) throw new FormatException("GTS record length is incorrect.");

            Data = new byte[0xDC];
            Array.Copy(data, 0, Data, 0, 0xDC);
            Unknown0 = new byte[0x10];
            Array.Copy(data, 0xDC, Unknown0, 0, 0x10);
            Species = BitConverter.ToUInt16(data, 0xEC);
            Gender = (Genders)data[0xEE];
            Level = data[0xEF];
            RequestedSpecies = BitConverter.ToUInt16(data, 0xF0);
            RequestedGender = (Genders)data[0xF2];
            RequestedMinLevel = data[0xF3];
            RequestedMaxLevel = data[0xF4];
            Unknown1 = data[0xF5];
            TrainerGender = (GtsTrainerGenders)data[0xF6];
            Unknown2 = data[0xF7];
            TimeDeposited = TimestampToDate(BitConverter.ToUInt64(data, 0xF8));
            TimeWithdrawn = TimestampToDate(BitConverter.ToUInt64(data, 0x100));
            PID = BitConverter.ToInt32(data, 0x108);
            TrainerOT = BitConverter.ToUInt32(data, 0x10C);
            TrainerName = new EncodedString5(data, 0x110, 0x10);
            TrainerCountry = data[0x120];
            TrainerRegion = data[0x121];
            TrainerClass = data[0x122];
            IsExchanged = data[0x123];
            TrainerVersion = data[0x124];
            TrainerLanguage = data[0x125];
            TrainerBadges = data[0x126];
            TrainerUnityTower = data[0x127];
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
            TimeWithdrawn = DateTime.UtcNow;
            PID = other.PID;
            IsExchanged = 0x01;
        }

        public static bool CheckLevels(byte min, byte max, byte other)
        {
            if (max == 0) max = 255;
            return other >= min && other <= max;
        }

        public static DateTime ? TimestampToDate(ulong timestamp)
        {
            if (timestamp == 0) return null;

            ushort year = (ushort)((timestamp >> 0x30) & 0xffff);
            byte month = (byte)((timestamp >> 0x28) & 0xff);
            byte day = (byte)((timestamp >> 0x20) & 0xff);
            byte hour = (byte)((timestamp >> 0x18) & 0xff);
            byte minute = (byte)((timestamp >> 0x10) & 0xff);
            byte second = (byte)((timestamp >> 0x08) & 0xff);
            //byte fractional = (byte)(timestamp & 0xff); // always 0

            // allow ArgumentOutOfRangeExceptions to escape
            return new DateTime(year, month, day, hour, minute, second);
        }

        public static ulong DateToTimestamp(DateTime ? date)
        {
            if (date == null) return 0;
            DateTime date2 = (DateTime)date;

            return (ulong)(date2.Year & 0xffff) << 0x30
                | (ulong)(date2.Month & 0xff) << 0x28
                | (ulong)(date2.Day & 0xff) << 0x20
                | (ulong)(date2.Hour & 0xff) << 0x18
                | (ulong)(date2.Minute & 0xff) << 0x10
                | (ulong)(date2.Second & 0xff) << 0x08;
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
    }
}
