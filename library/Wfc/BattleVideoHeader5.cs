using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Wfc
{
    public class BattleVideoHeader5
    {
        public BattleVideoHeader5()
        {
        }

        public BattleVideoHeader5(int pid, ulong serial_number, byte[] data)
        {
            if (data.Length != 196) throw new ArgumentException("Battle video header data must be 196 bytes.");

            PID = pid;
            SerialNumber = serial_number;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public ulong SerialNumber;
        public byte[] Data;

        public ushort Streak
        {
            get
            {
                return BitConverter.ToUInt16(Data, 0xa4);
            }
        }

        public ushort[] Party
        {
            get
            {
                ushort[] result = new ushort[12];
                for (int x = 0; x < result.Length; x++)
                {
                    result[x] = BitConverter.ToUInt16(Data, 0x80 + x * 2);
                }
                return result;
            }
        }

        public byte[] TrainerName
        {
            get
            {
                byte[] result = new byte[16];
                Array.Copy(Data, 0, result, 0, 16);
                return result;
            }
        }

        public BattleVideoMetagames5 Metagame
        {
            get
            {
                return (BattleVideoMetagames5)Data[0xa6];
            }
        }

        public byte Country
        {
            get
            {
                return Data[0x17];
            }
        }

        public byte Region
        {
            get
            {
                return Data[0x18];
            }
        }

        public BattleVideoHeader5 Clone()
        {
            return new BattleVideoHeader5(PID, SerialNumber, Data.ToArray());
        }
    }

    public enum BattleVideoMetagames5 : byte
    {
        ColosseumSingleNoLauncher = 0x18,
        ColosseumDoubleNoLauncher = 0x19,
        ColosseumTripleNoLauncher = 0x1a,
        ColosseumRotationNoLauncher = 0x1b,
        ColosseumMultiNoLauncher = 0x1c,

        ColosseumSingleLauncher = 0x98,
        ColosseumDoubleLauncher = 0x99,
        ColosseumTripleLauncher = 0x9a,
        ColosseumRotationLauncher = 0x9b,
        ColosseumMultiLauncher = 0x9c,

        BattleSubwaySingle = 0x00,
        BattleSubwayDouble = 0x01,
        BattleSubwayMulti = 0x04,

        RandomMatchupSingle = 0x28,
        RandomMatchupDouble = 0x29,
        RandomMatchupTriple = 0x2a,
        RandomMatchupRotation = 0x2b,
        RandomMatchupLauncher = 0xaa,

        RatingSingle = 0x68,
        RatingDouble = 0x69,
        RatingTriple = 0x6a,
        RatingRotation = 0x6b,

        BattleCompetitionSingle = 0x38,
        BattleCompetitionDouble = 0x39,
        BattleCompetitionTriple = 0x3a,
        BattleCompetitionRotation = 0x3b,

        SearchBattleCompetition = 0x38,
        // This is not a legal value in either a search or a record.
        // I'm using it to indicate that no search is being done
        // (byte 0x148 in the search is 0).
        // Otherwise, the value 00 collides with Battle Subway Single
        // which is a legitimate search.
        SearchNone = 0xff,
    }

    public enum BattleVideoRankings5 : uint
    {
        None = 0x00000000,
        Newest30 = 0x00000001,
        LinkBattles = 0x00000003,
        SubwayBattles = 0x00000002,
    }
}
