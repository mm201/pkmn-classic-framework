using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using PkmnFoundations.Data;

namespace MakeBaseStatTables
{
    /// <summary>
    /// Builds some .txt data files from the Veekun data which require manual
    /// edits.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            String veekunFilename;

            if (args.Length < 1) veekunFilename = "pokedex.sqlite";
            else veekunFilename = args[0];

            if (veekunFilename.Contains(';')) throw new NotSupportedException("The character ; in filenames is not supported.");
            if (veekunFilename.Contains('?'))
            {
                Console.WriteLine("Usage: MakeBaseStatTables [filename]");
                Console.WriteLine("filename: Filename of Veekun sqlite database. Default: pokedex.sqlite");
                return;
            }

            using (SQLiteConnection connVeekun = new SQLiteConnection("Data Source=" + veekunFilename + "; Version=3"))
            {
                connVeekun.Open();

                SQLiteDataReader reader = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, " +
                    "(SELECT base_stat FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 1) AS base_hp, " +
                    "(SELECT base_stat FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 2) AS base_attack, " +
                    "(SELECT base_stat FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 3) AS base_defense, " +
                    "(SELECT base_stat FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 4) AS base_sp_attack, " +
                    "(SELECT base_stat FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 5) AS base_sp_defense, " +
                    "(SELECT base_stat FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 6) AS base_speed, " +
                    "(SELECT effort FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 1) AS reward_hp, " +
                    "(SELECT effort FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 2) AS reward_attack, " +
                    "(SELECT effort FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 3) AS reward_defense, " +
                    "(SELECT effort FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 4) AS reward_sp_attack, " +
                    "(SELECT effort FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 5) AS reward_sp_defense, " +
                    "(SELECT effort FROM pokemon_stats WHERE pokemon_id = pokemon_forms.pokemon_id AND stat_id = 6) AS reward_speed, " +
                    "(SELECT type_id FROM pokemon_types WHERE pokemon_id = pokemon_forms.pokemon_id AND slot = 1) AS type1, " +
                    "(SELECT type_id FROM pokemon_types WHERE pokemon_id = pokemon_forms.pokemon_id AND slot = 2) AS type2 " +
                    "FROM pokemon_forms ORDER BY id");

                using (FileStream fs = File.Open("form_stats1.txt", FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs);

                    while (reader.Read())
                    {
                        sw.Write("{0:00000}\t", reader["id"]);
                        sw.Write("{0:00}\t", reader["type1"] is DBNull ? 0 : Convert.ToInt32(reader["type1"]));
                        sw.Write("{0:00}\t", reader["type2"] is DBNull ? 0 : Convert.ToInt32(reader["type2"]));
                        sw.Write("{0:000}\t", Convert.ToInt32(reader["base_hp"]));
                        sw.Write("{0:000}\t", Convert.ToInt32(reader["base_attack"]));
                        sw.Write("{0:000}\t", Convert.ToInt32(reader["base_defense"]));
                        sw.Write("{0:000}\t", Convert.ToInt32(reader["base_speed"]));
                        sw.Write("{0:000}\t", Convert.ToInt32(reader["base_sp_attack"]));
                        sw.Write("{0:000}\t", Convert.ToInt32(reader["base_sp_defense"]));
                        sw.Write("{0:0}\t", Convert.ToByte(reader["reward_hp"]));
                        sw.Write("{0:0}\t", Convert.ToByte(reader["reward_attack"]));
                        sw.Write("{0:0}\t", Convert.ToByte(reader["reward_defense"]));
                        sw.Write("{0:0}\t", Convert.ToByte(reader["reward_speed"]));
                        sw.Write("{0:0}\t", Convert.ToByte(reader["reward_sp_attack"]));
                        sw.WriteLine("{0:0}", Convert.ToByte(reader["reward_sp_defense"]));
                    }
                    sw.Close();
                    fs.Close();
                }
                reader.Close();

                connVeekun.Close();
            }
        }
    }
}
