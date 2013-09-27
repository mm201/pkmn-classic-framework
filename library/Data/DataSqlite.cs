using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace PokeFoundations.Data
{
    class DataSqlite : DataAbstract
    {
        public SQLiteConnection CreateConnection()
        {
            return CreateConnection(DefaultFilename);
        }

        public SQLiteConnection CreateConnection(String filename)
        {
            if (filename.Contains('\"')) throw new ArgumentException();
            return new SQLiteConnection("Data Source=" + filename + ";Version=3;");
        }

        public String DefaultFilename;

        #region Veekun conversions
        public override void AbilitiesFromVeekun(DataTable abilities)
        {
            throw new NotImplementedException();
        }

        public override void ItemsFromVeekun(DataTable items, DataTable berries, DataTable machines)
        {
            throw new NotImplementedException();
        }

        public override void MovesFromVeekun(DataTable moves)
        {
            throw new NotImplementedException();
        }

        public override void PokemonFromVeekun(DataTable pokemon)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Veekun retrieval
        private DataTable GetVeekunWhatever(String filename, String table)
        {
            using (SQLiteConnection db = CreateConnection(filename))
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM `" + SqlSanitize(table) + "`", db);
                DataTable result = new DataTable();
                db.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                result.Load(reader);
                reader.Close();
                db.Close();
                return result;
            }
        }

        public override DataTable GetVeekunAbilities()
        {
            return GetVeekunAbilities(DefaultFilename);
        }

        public DataTable GetVeekunAbilities(String filename)
        {
            return GetVeekunWhatever(filename, "abilities");
        }

        public override DataTable GetVeekunItems()
        {
            return GetVeekunItems(DefaultFilename);
        }

        public DataTable GetVeekunItems(String filename)
        {
            return GetVeekunWhatever(filename, "items");
        }

        public override DataTable GetVeekunBerries()
        {
            return GetVeekunBerries(DefaultFilename);
        }

        public DataTable GetVeekunBerries(String filename)
        {
            return GetVeekunWhatever(filename, "berries");
        }

        public override DataTable GetVeekunBerryFlavours()
        {
            return GetVeekunBerryFlavours(DefaultFilename);
        }

        public DataTable GetVeekunBerryFlavours(String filename)
        {
            return GetVeekunWhatever(filename, "berry_flavors");
        }

        public override DataTable GetVeekunMachines()
        {
            return GetVeekunMachines(DefaultFilename);
        }

        public DataTable GetVeekunMachines(String filename)
        {
            using (SQLiteConnection db = CreateConnection(filename))
            {
                // we don't want the redundant data from third versions and remakes
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM machines WHERE version_group_id IN (1, 3, 5, 8, 11)", db);
                DataTable result = new DataTable();
                db.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                result.Load(reader);
                reader.Close();
                db.Close();
                return result;
            }
        }

        public override DataTable GetVeekunMoves()
        {
            return GetVeekunMoves(DefaultFilename);
        }

        public DataTable GetVeekunMoves(String filename)
        {
            return GetVeekunWhatever(filename, "moves");
        }

        public override DataTable GetVeekunPokemon()
        {
            return GetVeekunPokemon(DefaultFilename);
        }

        public DataTable GetVeekunPokemon(String filename)
        {
            return GetVeekunWhatever(filename, "pokemon");
        }
        #endregion

    }
}
