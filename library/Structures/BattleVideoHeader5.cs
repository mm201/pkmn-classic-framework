using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class BattleVideoHeader5
    {
        public BattleVideoHeader5()
        {
        }

        public BattleVideoHeader5(int pid, long serial_number, byte[] data)
        {
            if (data.Length != 196) throw new ArgumentException("Battle video header data must be 196 bytes.");

            PID = pid;
            SerialNumber = serial_number;
            Data = data;
        }

        // todo: encapsulate these so calculated fields are always correct
        public int PID;
        public long SerialNumber;
        public byte[] Data;

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
        None = 0x00,

        ColosseumSingleNoLauncher = 0x18,
        ColosseumSingleLauncher = 0x98,
        ColosseumDoubleNoLauncher = 0x19,
        ColosseumDoubleLauncher = 0x99,
        ColosseumTripleNoLauncher = 0x1a,
        ColosseumTripleLauncher = 0x9a,
        ColosseumRotationNoLauncher = 0x1b,
        ColosseumRotationLauncher = 0x9b,
        ColosseumMultiNoLauncher = 0x1c,
        ColosseumMultiLauncher = 0x9c,

        BattleSubwaySingle = 0x00,
        BattleSubwayDouble = 0x01,
        BattleSubwayMulti = 0x04,

        RandomMatchupSingle = 0x28,
        RandomMatchupDouble = 0x29,
        RandomMatchupTriple = 0x2a,
        RandomMatchupRotation = 0x2b,
        RandomMatchupLauncher = 0xaa,

        BattleCompetition = 0x38,
    }

    public enum BattleVideoRankings5 : uint
    {
        None = 0x00000000,
        Newest30 = 0x00000001,
        LinkBattles = 0x00000003,
        SubwayBattles = 0x00000002,
    }
}
