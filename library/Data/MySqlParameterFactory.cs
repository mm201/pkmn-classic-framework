using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PkmnFoundations.Data
{
    internal class MySqlParameterFactory : DbParameterFactoryBase
    {
        internal MySqlParameterFactory()
            : base()
        {

        }

        internal override System.Data.Common.DbParameter CreateParameter(string name, object value, Type t)
        {
            MySqlDbType ? dbType = GetDbType(t);

            if (dbType == null)
                return new MySqlParameter(name, value);
            else
            {
                MySqlParameter result = new MySqlParameter(name, (MySqlDbType)dbType);
                result.Value = value;
                return result;
            }
        }

        private static Dictionary<Type, MySqlDbType> m_dbtype_map = new Dictionary<Type, MySqlDbType>()
            {
                {typeof(bool), MySqlDbType.Byte},
                {typeof(sbyte), MySqlDbType.Byte},
                {typeof(short), MySqlDbType.Int16},
                {typeof(int), MySqlDbType.Int32},
                {typeof(long), MySqlDbType.Int64},
                {typeof(byte), MySqlDbType.UByte},
                {typeof(ushort), MySqlDbType.UInt16},
                {typeof(uint), MySqlDbType.UInt32},
                {typeof(ulong), MySqlDbType.UInt64},
                {typeof(float), MySqlDbType.Float},
                {typeof(double), MySqlDbType.Double},
                {typeof(decimal), MySqlDbType.Decimal},
                {typeof(DateTime), MySqlDbType.DateTime},
                {typeof(TimeSpan), MySqlDbType.Time},
                {typeof(String), MySqlDbType.VarChar},
                {typeof(Guid), MySqlDbType.Guid},
            };

        private MySqlDbType GetDbType(Type t)
        {
            if (m_dbtype_map.ContainsKey(t)) return m_dbtype_map[t];
            if (t.BaseType == typeof(Enum)) return MySqlDbType.Int32; // fixme: wrong for enums inheriting from other types
            return MySqlDbType.Blob;
        }

        internal override System.Data.Common.DbParameter CloneParameter(System.Data.Common.DbParameter orig)
        {
            if (orig == null) throw new ArgumentNullException();
            if (!(orig is MySqlParameter)) throw new ArgumentException();

            MySqlParameter p = (MySqlParameter)orig;
            MySqlParameter param = new MySqlParameter(p.ParameterName, (MySqlDbType)p.DbType, p.Size, p.Direction, p.IsNullable, p.Precision, p.Scale, p.SourceColumn, p.SourceVersion, p.Value);
            param.DbType = p.DbType;
            return param;
        }
    }
}
