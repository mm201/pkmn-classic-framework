using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public class BattleVideoHeader4
    {
        public BattleVideoHeader4()
        {
        }

        public BattleVideoHeader4(int pid, long serial_number, byte[] data)
        {
            if (data.Length != 228) throw new ArgumentException("Battle video header data must be 228 bytes.");

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
                    result[x] = BitConverter.ToUInt16(Data, 0x7c + x * 2);
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

        public BattleVideoMetagames4 Metagame
        {
            get
            {
                return (BattleVideoMetagames4)Data[0xa6];
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

        public BattleVideoHeader4 Clone()
        {
            return new BattleVideoHeader4(PID, SerialNumber, Data.ToArray());
        }

        #region ID number calculation
        // Generating battle video IDs:
        //
        // STEP 1: Prepping the Template:
        // Start with an auto-incrementing primary key. Treat as a string of 12
        // decimal digits. Pad with 0s. We call this the Key.
        //
        // The ones place (rightmost) is used to select one of ten fixed 
        // Templates. A Template has 12 (11 useful) digits. They are basically
        // hardcoded random-looking numbers. The rightmost digit of each 
        // template is undefined/unused. The leftmost digit is never 0.
        //
        // The Key’s tens place and hundreds place are added together mod-10 to
        // get a constant which is used to roll each of the digits in the
        // Template by. Here, rolling means adding in mod-10.
        //
        // The Template’s leftmost digit is rolled in mod-9 instead of 10.
        // (While rolling, skip the number 0.) If the value in the 
        // second-leftmost digit wraps around to or past 0, this leftmost digit
        // is rolled one less time than it would be otherwise.
        //
        // Note: There is probably a better way to represent this algorithm
        // that doesn't require special handling for the leftmost digit based
        // on what the second one is doing. It would involve choosing different
        // Template constants and using a different roll formula.
        //
        // STEP 2: Prepping the Key:
        // Remove the tens place from the Key, then shift all the digits
        // EXCEPT the leftmost and rightmost one place to the right to fill the
        // gap. The second leftmost digit is filled with a constant 0.
        // (Example: 123333345678 becomes 102333334568)
        //
        // Note: This handling of the left two digits is an extrapolation on
        // my part. It gives us the maximum possible range of 10-00000-00000
        // through to 99-99999-99999 in a 1:1 mapping.
        //
        // STEP 3: Putting it together:
        // Overwrite the rightmost digit of the Template with the Key’s ones
        // place digit.
        //
        // For each remaining digit, roll the Template *backwards* a number of
        // times equal to the Key’s digit in that place.

        private static byte[][] m_templates = new byte[][]{
            new byte[]{6, 9, 3, 6, 1, 2, 7, 5, 2, 2, 4, 0},
            new byte[]{8, 0, 7, 2, 6, 2, 4, 9, 1, 4, 4, 1},
            new byte[]{1, 4, 9, 7, 3, 8, 5, 7, 6, 4, 7, 2},
            new byte[]{9, 4, 1, 7, 6, 3, 9, 6, 2, 7, 5, 3},
            new byte[]{4, 6, 4, 2, 9, 9, 7, 3, 4, 6, 1, 4},
            new byte[]{9, 5, 1, 0, 9, 7, 8, 6, 6, 3, 5, 5},
            new byte[]{8, 8, 7, 4, 4, 4, 3, 4, 6, 2, 9, 6},
            new byte[]{6, 3, 7, 7, 5, 6, 7, 6, 1, 9, 5, 7},
            new byte[]{2, 6, 7, 2, 3, 2, 4, 5, 8, 9, 7, 8},
            new byte[]{2, 1, 9, 1, 7, 9, 9, 1, 6, 5, 6, 9}
        };

        /// <summary>
        /// Converts a primary key (auto incrementing) into a Battle Video ID.
        /// </summary>
        public static long KeyToSerial(long key)
        {
            if (key > 899999999999L || key < 0L) throw new ArgumentOutOfRangeException();

            byte[] keyDigits = LongToDigits(key);
            byte[] serialDigits = m_templates[keyDigits[11]].ToArray();

            byte valueShift = 0;
            valueShift += keyDigits[9];
            valueShift += keyDigits[10];
            valueShift %= 10;

            if (valueShift + serialDigits[1] > 9)
                serialDigits[0]--;

            for (int x = 0; x < 11; x++)
            {
                serialDigits[x] += valueShift;
            }

            serialDigits[0] += 9;
            serialDigits[0] -= keyDigits[0];
            serialDigits[0] %= 9;
            if (serialDigits[0] == 0) serialDigits[0] = 9;

            serialDigits[1] %= 10;

            for (int x = 1; x < 10; x++)
            {
                serialDigits[x + 1] += 10;
                serialDigits[x + 1] -= keyDigits[x];
                serialDigits[x + 1] %= 10;
            }

            return DigitsToLong(serialDigits);
        }

        /// <summary>
        /// Converts Battle Video ID back into a primary key.
        /// </summary>
        public static long SerialToKey(long serial)
        {
            if (serial > 999999999999L || serial < 100000000000L)
                throw new ArgumentOutOfRangeException();

            byte[] serialDigits = LongToDigits(serial);
            byte[] templateDigits = m_templates[serialDigits[11]];

            serialDigits[0] = (byte)(10 + templateDigits[0] - serialDigits[0]);
            for (int x = 1; x < 11; x++)
            {
                serialDigits[x] = (byte)(10 + templateDigits[x] - serialDigits[x]);
            }

            byte valueShift = (byte)(serialDigits[1] % 10);
            if (templateDigits[1] - valueShift >= 0)
                serialDigits[0]--;

            serialDigits[0] += (byte)(9 - valueShift);
            serialDigits[0] %= 9;

            for (int x = 1; x < 11; x++)
            {
                serialDigits[x] += (byte)(10 - valueShift);
                serialDigits[x] %= 10;
            }
            for (int x = 1; x < 10; x++)
            {
                serialDigits[x] = serialDigits[x + 1];
            }

            serialDigits[10] = (byte)((20 - valueShift - serialDigits[9]) % 10);

            return DigitsToLong(serialDigits);
        }

        private static byte[] LongToDigits(long value)
        {
            if (value > 999999999999L || value < 0L) throw new ArgumentException();
            byte[] result = new byte[12];
            for (int x = 11; x >= 0; x--)
            {
                result[x] = (byte)(value % 10);
                value /= 10;
            }
            return result;
        }

        private static long DigitsToLong(byte[] digits)
        {
            if (digits.Length != 12) throw new ArgumentException();
            long result = 0;
            long pow = 1;
            for (int x = 11; x >= 0; x--)
            {
                if (digits[x] > 9) throw new ArgumentException();
                result += digits[x] * pow;
                pow *= 10;
            }
            return result;
        }

        #endregion

        public static String FormatSerial(long serial)
        {
            String number = serial.ToString("D12");
            String[] split = new String[3];
            split[0] = number.Substring(0, number.Length - 10);
            split[1] = number.Substring(number.Length - 10, 5);
            split[2] = number.Substring(number.Length - 5, 5);
            return String.Join("-", split);
        }
    }

    public enum BattleVideoMetagames4 : byte
    {
        Latest30 = 0xff,

        ColosseumSingleNoRestrictions = 0xfa,
        ColosseumSingleCupMatch = 0xfb,
        ColosseumDoubleNoRestrictions = 0xfc,
        ColosseumDoubleCupMatch = 0xfd,
        ColosseumMulti = 0x0e,

        BattleTowerSingle = 0x0f,
        BattleTowerDouble = 0x10,
        BattleTowerMulti = 0x11,

        BattleFactoryLv50Single = 0x12,
        BattleFactoryLv50Double = 0x13,
        BattleFactoryLv50Multi = 0x14,

        BattleFactoryOpenSingle = 0x15,
        BattleFactoryOpenDouble = 0x16,
        BattleFactoryOpenMulti = 0x17,

        BattleHallSingle = 0x18,
        BattleHallDouble = 0x19,
        BattleHallMulti = 0x1a,

        BattleCastleSingle = 0x1b,
        BattleCastleDouble = 0x1c,
        BattleCastleMulti = 0x1d,

        BattleArcadeSingle = 0x1e,
        BattleArcadeDouble = 0x1f,
        BattleArcadeMulti = 0x20,
    }
}
