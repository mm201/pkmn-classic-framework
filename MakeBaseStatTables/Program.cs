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

                reader = (SQLiteDataReader)connVeekun.ExecuteReader("SELECT id, " +
                    "(SELECT ability_id FROM pokemon_abilities WHERE pokemon_id = pokemon_forms.pokemon_id AND slot = 1) AS ability1, " +
                    "(SELECT ability_id FROM pokemon_abilities WHERE pokemon_id = pokemon_forms.pokemon_id AND slot = 2) AS ability2, " +
                    "(SELECT ability_id FROM pokemon_abilities WHERE pokemon_id = pokemon_forms.pokemon_id AND is_hidden = 1) AS ability_hidden " +
                    "FROM pokemon_forms ORDER BY id");
                using (FileStream fs3 = File.Open("form_abilities3.txt", FileMode.Create), 
                    fs4 = File.Open("form_abilities4.txt", FileMode.Create),
                    fs5 = File.Open("form_abilities5.txt", FileMode.Create),
                    fs6 = File.Open("form_abilities6.txt", FileMode.Create))
                {
                    StreamWriter sw3 = new StreamWriter(fs3);
                    StreamWriter sw4 = new StreamWriter(fs4);
                    StreamWriter sw5 = new StreamWriter(fs5);
                    StreamWriter sw6 = new StreamWriter(fs6);

                    while (reader.Read())
                    {
                        long id = Convert.ToInt64(reader["id"]);

                        if ((id <= 386) || (id >= 10000 && id <= 10033))
                        {
                            sw3.Write("{0:00000}\t", reader["id"]);
                            sw3.Write("{0:000}\t", reader["ability1"] is DBNull ? 0 : Convert.ToInt32(reader["ability1"]));
                            sw3.Write("{0:000}\t", reader["ability2"] is DBNull ? 0 : Convert.ToInt32(reader["ability2"]));
                            sw3.WriteLine("{0:000}", 0);
                        }
                        else if ((id <= 493) || (id >= 10034 && id <= 10065))
                        {
                            sw4.Write("{0:00000}\t", reader["id"]);
                            sw4.Write("{0:000}\t", reader["ability1"] is DBNull ? 0 : Convert.ToInt32(reader["ability1"]));
                            sw4.Write("{0:000}\t", reader["ability2"] is DBNull ? 0 : Convert.ToInt32(reader["ability2"]));
                            sw4.WriteLine("{0:000}", 0);
                        }

                        if ((id <= 649) || (id >= 10000 && id <= 10084))
                        {
                            sw5.Write("{0:00000}\t", reader["id"]);
                            sw5.Write("{0:000}\t", reader["ability1"] is DBNull ? 0 : Convert.ToInt32(reader["ability1"]));
                            sw5.Write("{0:000}\t", reader["ability2"] is DBNull ? 0 : Convert.ToInt32(reader["ability2"]));
                            sw5.WriteLine("{0:000}", reader["ability_hidden"] is DBNull ? 0 : Convert.ToInt32(reader["ability_hidden"]));
                        }
                        else
                        {
                            sw6.Write("{0:00000}\t", reader["id"]);
                            sw6.Write("{0:000}\t", reader["ability1"] is DBNull ? 0 : Convert.ToInt32(reader["ability1"]));
                            sw6.Write("{0:000}\t", reader["ability2"] is DBNull ? 0 : Convert.ToInt32(reader["ability2"]));
                            sw6.WriteLine("{0:000}", reader["ability_hidden"] is DBNull ? 0 : Convert.ToInt32(reader["ability_hidden"]));
                        }
                    }

                    sw3.Close();
                    sw4.Close();
                    sw5.Close();
                    sw6.Close();

                    fs3.Close();
                    fs4.Close();
                    fs5.Close();
                    fs6.Close();
                }

                connVeekun.Close();
            }
        }
    }
}
