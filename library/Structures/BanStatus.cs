using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class BanStatus
    {
        public BanStatus()
        {

        }

        public BanStatus(BanLevels level, string reason, DateTime ? expires)
        {
            Level = level;
            Reason = reason;
            Expires = expires;
        }

        public BanLevels Level { get; set; }
        public string Reason { get; set; }
        public DateTime ? Expires { get; set; }
    }

    public enum BanLevels
    {
        None = 0,
        Restricted = 1,
        Banned = 2,
        IAmATotalPieceOfShit = 9,
    }
}
