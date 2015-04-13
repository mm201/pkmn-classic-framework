using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PkmnFoundations.Structures;

namespace PkmnFoundations.Web
{
    public static class WebFormat
    {
        public static String Markings(Markings markings)
        {
            return Format.Markings(markings, "<span class=\"m\">{0}</span>", "<span>{0}</span>", "");
        }

        public static String PokemonImage(PokemonPartyBase pokemon)
        {
            return (pokemon.IsShiny ? "~/images/pkmn-lg-s/" : "~/images/pkmn-lg/") +
                PokemonImage2(pokemon) + ".png";
        }

        private static String PokemonImage2(PokemonPartyBase pkmn)
        {
            // todo: spinda
            StringBuilder builder = new StringBuilder();
            builder.Append(pkmn.SpeciesID);
            if (pkmn.Form.Suffix.Length > 0)
            {
                builder.Append('-');
                builder.Append(pkmn.Form.Suffix);
            }
            if (pkmn.Species.GenderVariations && pkmn.Gender == Genders.Female)
                builder.Append("-f");

            return builder.ToString();
        }
    }
}