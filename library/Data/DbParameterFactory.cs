using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PkmnFoundations.Data
{
    public class DbParameterFactory
    {
        public DbParameterFactory(DbConnection db)
        {
            if (db is MySqlConnection) m_parameter_factory = new MySqlParameterFactory();
            else throw new NotSupportedException();
        }

        private DbParameterFactoryBase m_parameter_factory;

        public DbParameter CreateParameter(String name, object value, Type t)
        {
            return m_parameter_factory.CreateParameter(name, value, t);
        }

        public DbParameter CloneParameter(DbParameter orig)
        {
            return m_parameter_factory.CloneParameter(orig);
        }
    }
}
