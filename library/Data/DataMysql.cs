using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace PokeFoundations.Data
{
    class DataMysql : DataAbstract
    {
        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection("Server=localhost;Database=pfound;User ID=pfound;Password=goldeen need log;Pooling=true;");
        }

        #region Veekun conversions

        /// <summary>
        /// Removes veekun markdown, keeping what's inside the [] but erasing what's inside the {}.
        /// </summary>
        private String PruneVeekunMarkdown(String orig)
        {
            StringBuilder builder = new StringBuilder();
            int pos = 0;
            while (pos >= 0)
            {
                int startspot = orig.IndexOf('[', pos);
                if (startspot < 0)
                {
                    builder.Append(orig.Substring(pos));
                    break;
                }

                builder.Append(orig.Substring(pos, startspot - pos));
                int endspot = orig.IndexOf("]{", startspot);
                builder.Append(orig.Substring(startspot + 1, endspot - startspot - 1));

                pos = orig.IndexOf('}', endspot + 2);
            }

            return builder.ToString();
        }

        public override void AbilitiesFromVeekun(DataTable abilities)
        {
            using (MySqlConnection db = CreateConnection())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO abilities (id, generation, Name, Flavour) VALUES (@id, @generation, @name, @flavour)", db);
                cmd.Parameters.Add("@id", MySqlDbType.UInt32);
                cmd.Parameters.Add("@generation", MySqlDbType.UInt32);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar);
                cmd.Parameters.Add("@flavour", MySqlDbType.VarChar);
                db.Open();
                foreach (DataRow row in abilities.Rows)
                {
                    cmd.Parameters["@id"].Value = (int)row["id"];
                    cmd.Parameters["@generation"].Value = (int)row["generation_id"];
                    cmd.Parameters["@name"].Value = (String)row["name"];
                    cmd.Parameters["@flavour"].Value = PruneVeekunMarkdown((String)row["short_effect"]);
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }

        public override void ItemsFromVeekun(DataTable items, DataTable berries, DataTable berry_flavours)
        {
            using (MySqlConnection db = CreateConnection())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO items (id, generation, category, Name, Price, Flavour, FlingPower, FlingEffect) VALUES (@id, @generation, @category, @name, @price, @flavour, @fling_power, @fling_effect)", db);
                cmd.Parameters.Add("@id", MySqlDbType.UInt32);
                cmd.Parameters.Add("@generation", MySqlDbType.UInt32);
                cmd.Parameters.Add("@category", MySqlDbType.Byte);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar);
                cmd.Parameters.Add("@price", MySqlDbType.VarChar);
                cmd.Parameters.Add("@flavour", MySqlDbType.VarChar);
                cmd.Parameters.Add("@fling_power", MySqlDbType.UInt32);
                cmd.Parameters.Add("@fling_effect", MySqlDbType.Enum);
                db.Open();

                foreach (DataRow row in items.Rows)
                {
                    int cat = (int)row["category_id"];
                    int fling = (int)row["fling_effect_id"];
                    if (cat == 37) continue; // this is a machine so insert it elsewhere

                    cmd.Parameters["@id"].Value = (int)row["id"];
                    cmd.Parameters["@generation"].Value = (int)row["generation_id"];
                    cmd.Parameters["@name"].Value = (String)row["name"];
                    cmd.Parameters["@price"].Value = (int)row["cost"];
                    cmd.Parameters["@flavour"].Value = (String)row["short_effect"];
                    cmd.Parameters["@fling_power"].Value = (int)row["fling_power"];

                    switch (cat)
                    {
                        // item
                        case 1:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                        case 24:
                        case 26:
                        case 27:
                        case 28:
                        case 29:
                        case 30:
                        case 32:
                        case 33:
                        case 34:
                        case 35:
                        case 36:
                        case 38:
                        case 39:
                        case 42:
                        case 43:
                            cmd.Parameters["@category"].Value = 1;
                            break;
                        // key
                        case 20:
                        case 21:
                        case 22:
                        case 23:
                        case 40:
                        case 41:
                        case 44:
                            cmd.Parameters["@category"].Value = 2;
                            break;
                        // machine
                        case 37:
                            cmd.Parameters["@category"].Value = 3;
                            break;
                        // berry
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            cmd.Parameters["@category"].Value = 4;
                            break;
                        // mail
                        case 25:
                            cmd.Parameters["@category"].Value = 5;
                            break;
                        // unknown
                        default:
                            cmd.Parameters["@category"].Value = DBNull.Value;
                            break;
                    }

                    switch (fling)
                    {
                        case 1:
                            cmd.Parameters["@fling_effect"].Value = "toxic";
                            break;
                        case 2:
                            cmd.Parameters["@fling_effect"].Value = "burn";
                            break;
                        case 3:
                        case 4:
                            cmd.Parameters["@fling_effect"].Value = "use";
                            break;
                        case 5:
                            cmd.Parameters["@fling_effect"].Value = "paralyze";
                            break;
                        case 6:
                            cmd.Parameters["@fling_effect"].Value = "poison";
                            break;
                        case 7:
                            cmd.Parameters["@fling_effect"].Value = "flinch";
                            break;
                        default:
                            cmd.Parameters["@fling_effect"].Value = DBNull.Value;
                            break;
                    }

                    cmd.ExecuteNonQuery();
                }

                cmd = new MySqlCommand("UPDATE items SET BerryNumber = @berry_number, BerryFirmness = @berry_firmness, BerryNaturalGiftPower = @berry_natural_gift_power, " +
                                       "BerryNaturalGiftType = @berry_natural_gift_type, BerrySize = @berry_size, BerryHarvestMin = @berry_harvest_min, " +
                                       "BerryHarvestMax = @berry_harvest_max, BerryGrowthTime = @berry_growth_time, BerrySoilDryness = @berry_soil_dryness, " +
                                       "BerrySmoothness = @berry_smoothness WHERE id = @id", db);

                cmd.Parameters.Add("@id", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_number", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_firmness", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_natural_gift_power", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_natural_gift_type", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_size", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_harvest_min", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_harvest_max", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_smoothness", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_growth_time", MySqlDbType.UInt32);
                cmd.Parameters.Add("@berry_soil_dryness", MySqlDbType.UInt32);

                foreach (DataRow row in berries.Rows)
                {
                    cmd.Parameters["@id"].Value = (int)row["item_id"];
                    cmd.Parameters["@berry_number"].Value = (int)row["id"];
                    cmd.Parameters["@berry_firmness"].Value = (int)row["firmness_id"];
                    cmd.Parameters["@berry_natural_gift_power"].Value = (int)row["natural_gift_power"];
                    cmd.Parameters["@berry_natural_gift_type"].Value = (int)row["natural_gift_type_id"];
                    cmd.Parameters["@berry_size"].Value = (int)row["size"];
                    cmd.Parameters["@berry_harvest_min"].Value = 1;
                    cmd.Parameters["@berry_harvest_max"].Value = (int)row["max_harvest"];
                    cmd.Parameters["@berry_growth_time"].Value = (int)row["growth_time"];
                    cmd.Parameters["@berry_soil_dryness"].Value = (int)row["soil_dryness"];
                    cmd.Parameters["@berry_smoothness"].Value = (int)row["smoothness"];

                    cmd.ExecuteNonQuery();
                }

                cmd = new MySqlCommand("", db);
                cmd.Parameters.Add("@berry_number", MySqlDbType.UInt32);
                cmd.Parameters.Add("@value", MySqlDbType.UInt32);
                // pivot the berry flavours table
                foreach (DataRow row in berry_flavours.Rows)
                {
                    int flavour = (int)row["contest_type_id"];
                    switch (flavour)
                    {
                        case 1:
                            cmd.CommandText = "UPDATE items SET BerrySpicyValue = @value WHERE BerryNumber = @berry_number";
                            break;
                        case 2:
                            cmd.CommandText = "UPDATE items SET BerryDryValue = @value WHERE BerryNumber = @berry_number";
                            break;
                        case 3:
                            cmd.CommandText = "UPDATE items SET BerrySweetValue = @value WHERE BerryNumber = @berry_number";
                            break;
                        case 4:
                            cmd.CommandText = "UPDATE items SET BerryBitterValue = @value WHERE BerryNumber = @berry_number";
                            break;
                        case 5:
                            cmd.CommandText = "UPDATE items SET BerrySourValue = @value WHERE BerryNumber = @berry_number";
                            break;
                    }
                    cmd.Parameters["@berry_number"].Value = (int)row["berry_id"];
                    cmd.Parameters["@value"].Value = (int)row["flavor"];
                }

                db.Close();
            }
            
        }

        public override void MovesFromVeekun(DataTable moves)
        {
            using (MySqlConnection db = CreateConnection())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO moves (id, generation, Name, type, Damage, PP, Accuracy, Priority, Target, StatLine, " +
                                                    "Effect, EffectChance, ContestType, ContestEffect) " +
                                                    "VALUES (@id, @generation, @name, @type, @damage, @pp, @accuracy, @priority, @target, @stat_line, " +
                                                    "@effect, @effect_chance, @contest_type, @contest_effect)", db);
                cmd.Parameters.Add("@id", MySqlDbType.UInt32);
                cmd.Parameters.Add("@generation", MySqlDbType.UInt32);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar);
                cmd.Parameters.Add("@type", MySqlDbType.UInt32);
                cmd.Parameters.Add("@damage", MySqlDbType.UInt32);
                cmd.Parameters.Add("@pp", MySqlDbType.UInt32);
                cmd.Parameters.Add("@accuracy", MySqlDbType.UInt32);
                cmd.Parameters.Add("@priority", MySqlDbType.Int32);
                cmd.Parameters.Add("@target", MySqlDbType.UInt32);
                cmd.Parameters.Add("@stat_line", MySqlDbType.Enum);
                cmd.Parameters.Add("@effect", MySqlDbType.UInt32);
                cmd.Parameters.Add("@effect_chance", MySqlDbType.UInt32);
                cmd.Parameters.Add("@contest_type", MySqlDbType.UInt32);
                cmd.Parameters.Add("@contest_effect", MySqlDbType.UInt32);
                
                db.Open();
                foreach (DataRow row in moves.Rows)
                {
                    int stat_line = (int)row["damage_class_id"];

                    cmd.Parameters["@id"].Value = (int)row["id"];
                    cmd.Parameters["@generation"].Value = (int)row["generation_id"];
                    cmd.Parameters["@name"].Value = (String)row["name"];
                    cmd.Parameters["@type"].Value = (int)row["type_id"];
                    cmd.Parameters["@damage"].Value = (int)row["power"];
                    cmd.Parameters["@pp"].Value = (int)row["pp"];
                    cmd.Parameters["@accuracy"].Value = (int)row["accuracy"];
                    cmd.Parameters["@priority"].Value = (int)row["priority"];
                    cmd.Parameters["@target"].Value = (int)row["target_id"];
                    cmd.Parameters["@effect"].Value = (int)row["effect_id"];
                    cmd.Parameters["@effect_chance"].Value = (int)row["effect_chance"];
                    cmd.Parameters["@contest_type"].Value = (int)row["contest_type_id"];
                    cmd.Parameters["@contest_effect"].Value = (int)row["super_contest_effect_id"];

                    switch (stat_line)
                    {
                        case 1:
                            cmd.Parameters["@stat_line"].Value = "support";
                            break;
                        case 2:
                            cmd.Parameters["@stat_line"].Value = "physical";
                            break;
                        case 3:
                            cmd.Parameters["@stat_line"].Value = "special";
                            break;
                        default:
                            cmd.Parameters["@stat_line"].Value = DBNull.Value;
                            break;
                    }

                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }

        public override void PokemonFromVeekun(DataTable pokemon)
        {
            using (MySqlConnection db = CreateConnection())
            {
                // UIDs and national dex must be provided in advance, due to the creativity involved in assigning them.
                MySqlCommand cmd = new MySqlCommand("UPDATE pokemon SET () WHERE NationalDex = @id", db);
                cmd.Parameters.Add("@id", MySqlDbType.UInt32);
                cmd.Parameters.Add("@generation", MySqlDbType.UInt32);
                cmd.Parameters.Add("@name", MySqlDbType.VarChar);
                cmd.Parameters.Add("@flavour", MySqlDbType.VarChar);
                db.Open();
                foreach (DataRow row in abilities.Rows)
                {
                    cmd.Parameters["@id"].Value = (int)row["id"];
                    cmd.Parameters["@generation"].Value = (int)row["generation_id"];
                    cmd.Parameters["@name"].Value = (String)row["name"];
                    cmd.Parameters["@flavour"].Value = PruneVeekunMarkdown((String)row["short_effect"]);
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }

        #endregion

        #region Veekun retrieval

        public override DataTable GetVeekunAbilities()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetVeekunItems()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetVeekunBerries()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetVeekunBerryFlavours()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetVeekunMachines()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetVeekunMoves()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetVeekunPokemon()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
