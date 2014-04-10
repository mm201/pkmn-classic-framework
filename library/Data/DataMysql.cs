using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace PokeFoundations.Data
{
    public class DataMysql : DataAbstract
    {
        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection("Server=localhost;Database=pfound;User ID=pfound;Password=goldeen need log;Pooling=true;");
        }
    }
}
