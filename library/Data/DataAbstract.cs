using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PokeFoundations.Data
{
    abstract class DataAbstract
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

        #region Veekun conversions
        /// <summary>
        /// Loads the PF database with abilities from Veekun.
        /// </summary>
        public abstract void AbilitiesFromVeekun(DataTable abilities);

        /// <summary>
        /// Loads the PF database with all items, berries, and TMs from Veekun.
        /// </summary>
        public abstract void ItemsFromVeekun(DataTable items, DataTable berries, DataTable machines);

        /// <summary>
        /// Loads the PF database with moves from Veekun.
        /// </summary>
        public abstract void MovesFromVeekun(DataTable moves);

        /// <summary>
        /// Loads the PF database with Pokémon from Veekun.
        /// </summary>
        public abstract void PokemonFromVeekun(DataTable pokemon);
        #endregion

        #region Veekun retrieval
        public abstract DataTable GetVeekunAbilities();
        public abstract DataTable GetVeekunItems();
        public abstract DataTable GetVeekunBerries();
        public abstract DataTable GetVeekunBerryFlavours();
        public abstract DataTable GetVeekunMachines();
        public abstract DataTable GetVeekunMoves();
        public abstract DataTable GetVeekunPokemon();
        #endregion
    }
}
