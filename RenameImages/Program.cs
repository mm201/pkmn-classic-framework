using PkmnFoundations.Data;
using PkmnFoundations.Pokedex;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace RenameImages
{
    /// <summary>
    /// Renames images obtained from Pokesprite. https://github.com/msikma/pokesprite
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            char slash = Path.DirectorySeparatorChar;
            ConnectionStringSettings css;

            if (args.Length < 1) path = "." + slash + "pkmn-sm";
            else path = args[0];

            if (path.Contains('?'))
            {
                Console.WriteLine("h e l p");
                return;
            }

            while (path.Length > 0 && (path[path.Length - 1] == '/' || path[path.Length - 1] == '\\'))
            {
                path = path.Substring(0, path.Length - 1);
            }
            path = path + slash;

            if (args.Length < 3)
                css = ConfigurationManager.ConnectionStrings["pkmnFoundationsConnectionString"];
            else
                css = new ConnectionStringSettings("", args[1], args[2]);

            Database db = Database.CreateInstance(css);
            Pokedex pokedex = new Pokedex(db, false);

            foreach (var pair in pokedex.Forms)
            {
                Form form = pair.Value;
                Species species = form.Species;
                StringBuilder speciesNameBuilder = new StringBuilder(species.Name["EN"].ToLowerInvariant());
                speciesNameBuilder.Replace("\'", "");
                speciesNameBuilder.Replace(".", "");
                speciesNameBuilder.Replace("♀", "-f");
                speciesNameBuilder.Replace("♂", "-m");
                speciesNameBuilder.Replace('é', 'e');
                speciesNameBuilder.Replace(' ', '-');
                string speciesName = speciesNameBuilder.ToString();

                // fixme: This is incorrect for Vivillon, Pumpkaboo, and Gourgeist, because their suffixless form is not form 0.
                string oldFilename = (form.Value == 0) 
                    ? String.Format("{0}.png", speciesName)
                    : String.Format("{0}-{1}.png", speciesName, form.Suffix);

                string newFilename = (form.Suffix.Trim().Length == 0)
                    ? String.Format("{0}.png", species.NationalDex)
                    : String.Format("{0}-{1}.png", species.NationalDex, form.Suffix);

                try
                {
                    File.Move(path + oldFilename, path + newFilename);
                    Console.Write("{0} {1}", oldFilename, newFilename);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

                if (form.Value == 0 && form.Suffix.Trim().Length != 0)
                {
                    try
                    {
                        string speciesFilename = String.Format("{0}.png", species.NationalDex);
                        File.Copy(path + newFilename, path + speciesFilename);
                        Console.Write(" {0}", speciesFilename);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.Write(ex.Message);
                    }
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }

        private static string NormalizeFilename(string filename, string path)
        {
            if (filename.Length >= path.Length)
            {
                string pathPart = filename.Substring(0, path.Length);
                if (pathPart == path) filename = filename.Substring(path.Length);
            }
            return filename.ToLowerInvariant().Trim();
        }

    }
}
