using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public static class Format
    {
        private static String[] m_marks = new String[] { "●", "▲", "■", "♥", "★", "♦" };

        public static String Markings(Markings markings, String trueFormat, String falseFormat, String delimiter)
        {
            StringBuilder result = new StringBuilder();
            int marking = 1;
            for (int value = 0; value < 6; value++)
            {
                if (marking > 1) result.Append(delimiter);
                String format = (((int)markings & marking) != 0) ? trueFormat : falseFormat;
                result.Append(String.Format(format, m_marks[value]));

                marking <<= 1;
            }

            return result.ToString();
        }

        public static String GenderSymbol(Genders gender)
        {
            switch (gender)
            {
                case Genders.Male:
                    return "♂";
                case Genders.Female:
                    return "♀";
                default:
                    return "";
            }
        }
    }
}
