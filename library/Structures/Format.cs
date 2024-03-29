﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Structures
{
    public static class Format
    {
        private static string[] m_marks = new string[] { "●", "▲", "■", "♥", "★", "♦" };

        public static string Markings(Markings markings, string trueFormat, string falseFormat, string delimiter)
        {
            StringBuilder result = new StringBuilder();
            int marking = 1;
            for (int value = 0; value < 6; value++)
            {
                if (marking > 1) result.Append(delimiter);
                string format = (((int)markings & marking) != 0) ? trueFormat : falseFormat;
                result.Append(String.Format(format, m_marks[value]));

                marking <<= 1;
            }

            return result.ToString();
        }

        public static string GenderSymbol(Genders gender)
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

        public static string ToIso639_1(Languages lang)
        {
            switch (lang)
            {
                case Languages.Japanese:
                    return "JA";
                case Languages.English:
                    return "EN";
                case Languages.French:
                    return "FR";
                case Languages.Italian:
                    return "IT";
                case Languages.German:
                    return "DE";
                case Languages.Spanish:
                    return "ES";
                case Languages.Korean:
                    return "KO";
                default:
                    throw new ArgumentException();
            }
        }

        public static Languages FromIso639_1(string lang)
        {
            switch (lang.ToUpperInvariant())
            {
                case "JA":
                    return Languages.Japanese;
                case "EN":
                    return Languages.English;
                case "FR":
                    return Languages.French;
                case "IT":
                    return Languages.Italian;
                case "DE":
                    return Languages.German;
                case "ES":
                    return Languages.Spanish;
                case "KO":
                    return Languages.Korean;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
