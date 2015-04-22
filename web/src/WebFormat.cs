using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Structures;

namespace PkmnFoundations.Web
{
    public static class WebFormat
    {
        public static String Markings(Markings markings)
        {
            return Format.Markings(markings, "<span class=\"m\">{0}</span>", "<span>{0}</span>", "");
        }

        public static String Gender(Genders gender)
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

        public static String RenderProgress(int curr, int max)
        {
            float percent = (float)curr * 100.0f / (float)max;
            return "<div class=\"progress\" style=\"width: " + percent.ToString() + "%;\"></div>";
        }

        public static String RenderType(Pokedex.Type type)
        {
            return "<span class=\"type " + type.Identifier + "\">" + type.Name.ToString() + "</span>";
        }

        public static String PokemonImageLarge(PokemonPartyBase pokemon)
        {
            return (pokemon.IsShiny ? "~/images/pkmn-lg-s/" : "~/images/pkmn-lg/") +
                PokemonImage2(pokemon) + ".png";
        }

        public static String PokemonImageSmall(PokemonPartyBase pokemon)
        {
            // todo: shiny minis
            return (pokemon.IsShiny ? "~/images/pkmn-sm/" : "~/images/pkmn-sm/") +
                PokemonImage2(pokemon) + ".png";
        }

        public static String SpeciesImageLarge(Form f)
        {
            return "~/images/pkmn-lg/" + PokemonImage2(f, Genders.Male) + ".png";
        }

        public static String SpeciesImageLarge(Species s)
        {
            return "~/images/pkmn-lg/" + PokemonImage2(s.Forms(0), Genders.Male) + ".png";
        }

        public static String SpeciesImageSmall(Form f)
        {
            // fixme: this is appending forms to some pokemon eg. arceus, keldeo, deerling
            // todo: need to take gender here optionally
            return "~/images/pkmn-sm/" + PokemonImage2(f, Genders.Male) + ".png";
        }

        public static String SpeciesImageSmall(Species s)
        {
            return "~/images/pkmn-sm/" + PokemonImage2(s.Forms(0), Genders.Male) + ".png";
        }

        private static String PokemonImage2(PokemonPartyBase pokemon)
        {
            return PokemonImage2(pokemon.Form, pokemon.Gender);
        }

        private static String PokemonImage2(Form f, Genders g)
        {
            // todo: spinda
            StringBuilder builder = new StringBuilder();
            builder.Append(f.SpeciesID);
            if (f.Suffix.Length > 0)
            {
                builder.Append('-');
                builder.Append(f.Suffix);
            }
            if (f.Species.GenderVariations && g == Genders.Female)
                builder.Append("-f");

            return builder.ToString();
        }

        public static String ItemImage(Item item)
        {
            return "~/images/item-sm/" + item.ID.ToString() + ".png";
        }
    }
}