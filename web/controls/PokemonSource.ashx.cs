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
            int natDex = 0;
            Int32.TryParse(query, out natDex);

            Func<KeyValuePair<int, Species>, bool> filter;
            if (natDex > 0)
                filter = pair => pair.Key == natDex;
            else
                filter = pair => pair.Value.Name[iso].ToLowerInvariant().Contains(query);

            IEnumerable<Species> data;
            data = pokedex.Species.Where(filter).OrderBy(pair => pair.Key).Take(rows).Select(pair => pair.Value);

            DataTable dt = new DataTable();
            dt.Columns.Add("Text", typeof(String));
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("html", typeof(String));

            foreach (Species s in data)
            {
                String name = s.Name[iso];
                String html = "<img src=\"" + Common.ResolveUrl(WebFormat.SpeciesImageSmall(s)) +
                    "\" alt=\"" + Common.HtmlEncode(name) +
                    "\" class=\"sprite speciesSmall\" width=\"40px\" height=\"32px\" />" +
                    String.Format("{0} (#{1})",
                    Common.HtmlEncode(name),
                    s.NationalDex);

                dt.Rows.Add(name, s.NationalDex, html);
            }

            return dt;
        }
    }
}
