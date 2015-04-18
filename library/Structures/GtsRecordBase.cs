using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public abstract class GtsRecordBase : BinarySerializableBase
    {
        public GtsRecordBase(Pokedex.Pokedex pokedex) : base()
        {
            m_pokedex = pokedex;
        }

        protected Pokedex.Pokedex m_pokedex;
        public Pokedex.Pokedex Pokedex
        {
            get
            {
                return m_pokedex;
            }
            set
            {
                m_pokedex = value;
            }
        }

        public static bool CheckLevels(byte min, byte max, byte other)
        {
            if (max == 0) max = 255;
            return other >= min && other <= max;
        }

        public static ulong DateToTimestamp(DateTime? date)
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

        public static DateTime? TimestampToDate(ulong timestamp)
        {
            if (timestamp == 0) return null;

            ushort year = (ushort)((timestamp >> 0x30) & 0xffff);
            byte month = (byte)((timestamp >> 0x28) & 0xff);
            byte day = (byte)((timestamp >> 0x20) & 0xff);
            byte hour = (byte)((timestamp >> 0x18) & 0xff);
            byte minute = (byte)((timestamp >> 0x10) & 0xff);
            byte second = (byte)((timestamp >> 0x08) & 0xff);
            //byte fractional = (byte)(timestamp & 0xff); // always 0

            try
            {
                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }
}
