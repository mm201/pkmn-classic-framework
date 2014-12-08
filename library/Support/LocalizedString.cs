using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PkmnFoundations.Support
{
    public class LocalizedString : Dictionary<String, String>
    {
        public LocalizedString()
        {
            
        }

        public override string ToString()
        {
            if (Count == 0) return null;

            String lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpperInvariant();
            // fixme: this is not O(1) but should be
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
    }
}
