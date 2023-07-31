using PkmnFoundations.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Wfc
{
    public class PlazaSchedule : BinarySerializableBase
    {
        public PlazaSchedule()
        {
        }

        public PlazaSchedule(uint duration, int unknown1, PlazaFootprintOptions footprintOptions,
            PlazaRoomTypes roomType, PlazaSeasons season, PlazaScheduleEntry[] schedule)
        {
            Duration = duration;
            Unknown1 = unknown1;
            FootprintOptions = footprintOptions;
            RoomType = roomType;
            Season = season;
            Schedule = schedule;
        }

        public PlazaSchedule(byte[] data)
        {
            Load(data);
        }

        public uint Duration; // Duration the room remains open for (seconds)
        public int Unknown1; // Unknown, Mythra thinks it may be a random seed
        public PlazaFootprintOptions FootprintOptions;
        public PlazaRoomTypes RoomType;
        public PlazaSeasons Season;
        public PlazaScheduleEntry[] Schedule;

        protected override void Load(BinaryReader reader)
        {
            Duration = reader.ReadUInt32();
            Unknown1 = reader.ReadInt32();
            FootprintOptions = (PlazaFootprintOptions)reader.ReadInt32();
            RoomType = (PlazaRoomTypes)reader.ReadByte();
            Season = (PlazaSeasons)reader.ReadByte();

            ushort schedLength = reader.ReadUInt16();
            Schedule = new PlazaScheduleEntry[schedLength];
            for (int i = 0; i < schedLength; i++)
            {
                Schedule[i] = new PlazaScheduleEntry(reader.ReadBytes(8));
            }
        }

        protected override void Save(BinaryWriter writer)
        {
            writer.Write(Duration);
            writer.Write(Unknown1);
            writer.Write((int)FootprintOptions);
            writer.Write((byte)RoomType);
            writer.Write((byte)Season);

            var schedule = Schedule;
            writer.Write((ushort)schedule.Length);
            foreach (var entry in schedule)
            {
                writer.Write(entry.Save());
            }
        }

        public override int Size
        {
            get
            {
                return 16 + (Schedule?.Length ?? 0) * 8;
            }
        }
    }

    public class PlazaScheduleEntry : BinarySerializableBase
    {
        public PlazaScheduleEntry()
        {
        }

        public PlazaScheduleEntry(int time, PlazaEventTypes eventType)
        {
            Time = time;
            EventType = eventType;
        }

        public PlazaScheduleEntry(byte[] data)
        {
            Load(data);
        }

        protected override void Load(BinaryReader reader)
        {
            Time = reader.ReadInt32();
            EventType = (PlazaEventTypes)reader.ReadInt32();
        }

        protected override void Save(BinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write((int)EventType);
        }

        public override int Size
        {
            get
            {
                return 8;
            }
        }

        public int Time;
        public PlazaEventTypes EventType;
    }

    public enum PlazaFootprintOptions : int
    {
        Normal = 0,
        Arceus = 1,
    }

    public enum PlazaRoomTypes : byte
    {
        Fire = 0,
        Water = 1,
        Electric = 2,
        Grass = 3,
        Mew = 4,
    }

    public enum PlazaSeasons : byte
    {
        None = 0,
        Spring = 1,
        Summer = 2,
        Fall = 3,
        Winter = 4,
    }

    public enum PlazaEventTypes : int
    {
        // todo: Catalogue all of these and identify which ones are valid when.
    }
}
