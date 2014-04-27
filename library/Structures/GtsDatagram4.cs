using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace PokeFoundations.Structures
{
    /// <summary>
    /// Structure used to represent Pokémon on the GTS in Generation IV.
    /// Includes a Pokémon box structure and metadata related to the trainer
    /// and request.
    /// </summary>
    [Serializable()]
    public class GtsDatagram4 : ISerializable
    {
        /// <summary>
        /// Obfuscated Pokémon (pkm) data. 236 bytes
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
        public GtsTrainerGenders TrainerGender;
        public byte Unknown2;

        public DateTime TimeDeposited;
        public DateTime TimeWithdrawn;

        /// <summary>
        /// User ID of the player (not Personality Value)
        /// </summary>
        public int PID;

        /// <summary>
        /// 16 bytes
        /// </summary>
        public byte[] TrainerName; // todo: decode/encode to unicode and provide a String wrapper.

        public ushort TrainerOT;

        public byte TrainerCountry;
        public byte TrainerRegion;
        public byte TrainerClass;

        public byte IsExchanged;

        public byte TrainerVersion;
        public byte TrainerLanguage;

        public byte[] Save()
        {
            // todo: enclose in properties and validate these when assigning.
            if (Data.Length != 0xEC) throw new FormatException("PKM length is incorrect");
            if (TrainerName.Length != 0x10) throw new FormatException("Trainer name length is incorrect");
            byte[] data = new byte[292];
            MemoryStream s = new MemoryStream(data);
            s.Write(Data, 0, 0xEC);
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
            s.Write(BitConverter.GetBytes(TimeDeposited.ToBinary()), 0, 8);
            s.Write(BitConverter.GetBytes(TimeWithdrawn.ToBinary()), 0, 8);
            s.Write(BitConverter.GetBytes(PID), 0, 4);
            s.Write(TrainerName, 0, 0x10);
            s.Write(BitConverter.GetBytes(TrainerOT), 0, 2);
            s.WriteByte(TrainerCountry);
            s.WriteByte(TrainerRegion);
            s.WriteByte(TrainerClass);
            s.WriteByte(IsExchanged);
            s.WriteByte(TrainerVersion);
            s.WriteByte(TrainerLanguage);
            s.Close();
            return data;
        }

        public void Load(byte[] data)
        {
            if (data.Length != 292) throw new FormatException("GTS datagram length is incorrect.");

            Data = new byte[0xEC];
            Array.Copy(data, 0, Data, 0, 0xEC);
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
            // todo: Figure out what the correct binary storage format is for
            // official GTS timestamps
            TimeDeposited = DateTime.FromBinary(BitConverter.ToInt64(data, 0xF8));
            TimeWithdrawn = DateTime.FromBinary(BitConverter.ToInt64(data, 0x100));
            PID = BitConverter.ToInt32(data, 0x108);
            TrainerName = new byte[0x10];
            Array.Copy(data, 0x10C, TrainerName, 0, 0x10);
            TrainerOT = BitConverter.ToUInt16(data, 0x11C);
            TrainerCountry = data[0x11E];
            TrainerRegion = data[0x11F];
            TrainerClass = data[0x120];
            IsExchanged = data[0x121];
            TrainerVersion = data[0x122];
            TrainerLanguage = data[0x123];
        }
    }
}
