using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace PkmnFoundations.Data
{
    // todo
    public class DataSqlite// : DataAbstract
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

        public String DefaultFilename = "pokedex.sqlite";
    }
}
