using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using PkmnFoundations.Pokedex;
using PkmnFoundations.Structures;

namespace PkmnFoundations.Web.controls
{
    /// <summary>
    /// Summary description for PokemonSource
    /// </summary>
    public class PokemonSource : ForeignLookupSource
    {
        protected override System.Data.DataTable GetData(HttpContext context, String query, int rows, Structures.Languages lang)
        {
            Pokedex.Pokedex pokedex = AppStateHelper.Pokedex(context.Application);
            String iso = Format.ToIso639_1(lang);
            query = query.ToLowerInvariant();
            IEnumerable<Species> data = pokedex.Species.Where(pair => pair.Value.Name[iso].ToLowerInvariant().Contains(query)).OrderBy(pair => pair.Key).Take(rows).Select(pair => pair.Value);

            DataTable dt = new DataTable();
            dt.Columns.Add("Text", typeof(String));
            dt.Columns.Add("Value", typeof(int));

            foreach (Species s in data)
            {
                dt.Rows.Add(s.Name[iso], s.NationalDex);
            }

            return dt;
        }
    }
}
