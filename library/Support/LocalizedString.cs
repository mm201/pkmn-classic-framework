using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PkmnFoundations.Support
{
    public class LocalizedString : Dictionary<string, string>
    {
        public LocalizedString()
        {

        }

        public string ToString(string lang)
        {
            if (Count == 0) return null;

            lang = lang.ToUpperInvariant();
            try
            {
                return this[lang];
            }
            catch (KeyNotFoundException)
            {

            }

            try
            {
                return this["EN"];
            }
            catch (KeyNotFoundException)
            {

            }

            return Values.First();
        }

        public override string ToString()
        {
            return ToString(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
        }
    }
}
