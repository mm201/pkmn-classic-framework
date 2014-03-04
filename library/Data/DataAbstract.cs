using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PokeFoundations.Data
{
    public abstract class DataAbstract
    {
        public static String SqlSanitize(String s)
        {
            return SqlSanitize(s, "");
        }

        public static String SqlSanitize(String s, String newChar)
        {
            String result = s.Replace("\'", newChar).Replace("[", newChar).Replace("]", newChar).Replace("`", newChar);
            int x = result.IndexOf("--");
            if (x != -1) result = result.Substring(0, x);
            return result;
        }
    }
}
