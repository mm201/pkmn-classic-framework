using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PkmnFoundations.Data;

namespace PkmnFoundations.GTS
{
    public static class AppStateHelper
    {
        public static Pokedex.Pokedex Pokedex(HttpApplicationState application)
        {
            return GetTypedApplicationObject(application, "pkmncfPokedex", () => new Pokedex.Pokedex(Database.Instance, false));
        }

        public static T GetTypedApplicationObject<T>(HttpApplicationState application, String key, Func<T> initializer) where T : class
        {
            object o = application[key];
            T t = o as T;

            if (t == null)
            {
                t = initializer();
                application.Add(key, t);
            }

            return t;
        }
    }
}