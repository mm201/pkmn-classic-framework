using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;

namespace PkmnFoundations.Data
{
    /// <summary>
    /// Provides extension and convenience methods for working with ADO.NET databases.
    /// </summary>
    public static class DatabaseExtender
    {
        #region Command Execution
        /// <summary>
        /// Runs a command and returns a DataTable containing its results.
        /// </summary>
        /// <param name="cmd">Command already initialized with an open connection</param>
        public static DataTable ExecuteDataTable(this DbCommand cmd)
        {
            IDataReader reader = cmd.ExecuteReader();
            DataTable result = new DataTable();
            result.Load(reader);
            return result;
        }

        /// <summary>
        /// Runs a command and returns a DataTable containing its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static DataTable ExecuteDataTable(this DbConnection db, String sqlstr, params IDataParameter[] _params)
        {
            IDataReader reader = db.ExecuteReader(sqlstr, _params);
            DataTable result = new DataTable();
            result.Load(reader);
            return result;
        }

        /// <summary>
        /// Runs a command and returns a DataTable containing its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static DataTable ExecuteDataTable(this DbConnection db, String sqlstr, IEnumerable<IDataParameter> _params)
        {
            IDataReader reader = db.ExecuteReader(sqlstr, _params.ToArray());
            DataTable result = new DataTable();
            result.Load(reader);
            return result;
        }

        /// <summary>
        /// Runs a command and returns a DataTable containing its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static DataTable ExecuteDataTable(this DbTransaction tran, String sqlstr, params IDataParameter[] _params)
        {
            IDataReader reader = tran.ExecuteReader(sqlstr, _params);
            DataTable result = new DataTable();
            result.Load(reader);
            return result;
        }

        /// <summary>
        /// Runs a command and returns a DataTable containing its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static DataTable ExecuteDataTable(this DbTransaction tran, String sqlstr, IEnumerable<IDataParameter> _params)
        {
            IDataReader reader = tran.ExecuteReader(sqlstr, _params.ToArray());
            DataTable result = new DataTable();
            result.Load(reader);
            return result;
        }

        /// <summary>
        /// Runs a command and returns a reader that iterates its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static IDataReader ExecuteReader(this DbConnection db, String sqlstr, params IDataParameter[] _params)
        {
            // hooray DbConnection provides a command factory
            DbCommand cmd = db.CreateCommand();
            cmd.CommandText = sqlstr;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// Runs a command and returns a reader that iterates its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static IDataReader ExecuteReader(this DbConnection db, String sqlstr, IEnumerable<IDataParameter> _params)
        {
            return db.ExecuteReader(sqlstr, _params.ToArray());
        }

        /// <summary>
        /// Runs a command and returns a reader that iterates its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static IDataReader ExecuteReader(this DbTransaction tran, String sqlstr, params IDataParameter[] _params)
        {
            // hooray DbConnection provides a command factory
            DbCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = sqlstr;
            cmd.Transaction = tran;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// Runs a command and returns a reader that iterates its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static IDataReader ExecuteReader(this DbTransaction tran, String sqlstr, IEnumerable<IDataParameter> _params)
        {
            return tran.ExecuteReader(sqlstr, _params.ToArray());
        }

        /// <summary>
        /// Runs a command and returns the first column of the first row in the query.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteScalar(this DbConnection db, String sqlstr, params IDataParameter[] _params)
        {
            DbCommand cmd = db.CreateCommand();
            cmd.CommandText = sqlstr;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Runs a command and returns the number of rows affected, subject to the usual quirkiness of ExecuteNonQuery.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteNonQuery(this DbConnection db, String sqlstr, params IDataParameter[] _params)
        {
            DbCommand cmd = db.CreateCommand();
            cmd.CommandText = sqlstr;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Runs a command and returns the first column of the first row in the query.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteScalar(this DbTransaction tran, String sqlstr, params IDataParameter[] _params)
        {
            DbCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = sqlstr;
            cmd.Transaction = tran;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Runs a command and returns the number of rows affected, subject to the usual quirkiness of ExecuteNonQuery.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteNonQuery(this DbTransaction tran, String sqlstr, params IDataParameter[] _params)
        {
            DbCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = sqlstr;
            cmd.Transaction = tran;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }
        #endregion

        #region MS-SQL
        // I can't find anything like a "parameter factory" in ADO.NET nor a virutal clone method,
        // so we need an implementation of these for each database engine.

        /// <summary>
        /// Creates a clone of the SqlParameter collection
        /// </summary>
        /// <param name="collection">Collection to be cloned</param>
        public static SqlParameter[] Clone(this IEnumerable<SqlParameter> collection)
        {
            int count = collection.Count();
            SqlParameter[] result = new SqlParameter[count];

            int x = 0;
            foreach (SqlParameter p in collection)
            {
                SqlParameter param = new SqlParameter(p.ParameterName, (SqlDbType)p.DbType, p.Size, p.Direction, p.IsNullable, p.Precision, p.Scale, p.SourceColumn, p.SourceVersion, p.Value);
                param.DbType = p.DbType;
                result[x] = param;
                x++;
            }

            return result;
        }

        /// <summary>
        /// Runs a proc and returns its return value.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="return_type">Return value's expected type</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteProcedure(this SqlConnection db, String name, SqlDbType return_type, params SqlParameter[] _params)
        {
            return ExecuteProcedureInternal(db.CreateCommand(), name, return_type, _params);
        }

        /// <summary>
        /// Runs a proc.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteProcedure(this SqlConnection db, String name, params SqlParameter[] _params)
        {
            SqlCommand cmd = db.CreateCommand();
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Runs a proc and returns its return value.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="return_type">Return value's expected type</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteProcedure(this SqlTransaction tran, String name, SqlDbType return_type, params SqlParameter[] _params)
        {
            SqlCommand cmd = tran.Connection.CreateCommand();
            cmd.Transaction = tran;
            return ExecuteProcedureInternal(cmd, name, return_type, _params);
        }

        /// <summary>
        /// Runs a proc.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteProcedure(this SqlTransaction tran, String name, params SqlParameter[] _params)
        {
            SqlCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = name;
            cmd.Transaction = tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }

        private static object ExecuteProcedureInternal(SqlCommand cmd, String name, SqlDbType return_type, SqlParameter[] _params)
        {
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            string pname = "result";
            while (cmd.Parameters.Contains("@" + pname)) pname = "x" + pname;
            pname = "@" + pname;
            SqlParameter p = new SqlParameter(pname, return_type);
            p.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            return cmd.Parameters[pname].Value;
        }
        #endregion

        #region MySQL
        // I can't find anything like a "parameter factory" in ADO.NET nor a virutal clone method,
        // so we need an implementation of these for each database engine.

        /// <summary>
        /// Creates a clone of the SqlParameter collection
        /// </summary>
        /// <param name="collection">Collection to be cloned</param>
        public static MySqlParameter[] Clone(this IEnumerable<MySqlParameter> collection)
        {
            int count = collection.Count();
            MySqlParameter[] result = new MySqlParameter[count];

            int x = 0;
            foreach (MySqlParameter p in collection)
            {
                MySqlParameter param = new MySqlParameter(p.ParameterName, (MySqlDbType)p.DbType, p.Size, p.Direction, p.IsNullable, p.Precision, p.Scale, p.SourceColumn, p.SourceVersion, p.Value);
                param.DbType = p.DbType;
                result[x] = param;
                x++;
            }

            return result;
        }

        /// <summary>
        /// Runs a proc and returns its return value.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="return_type">Return value's expected type</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteProcedure(this MySqlConnection db, String name, MySqlDbType return_type, params MySqlParameter[] _params)
        {
            return ExecuteProcedureInternal(db.CreateCommand(), name, return_type, _params);
        }

        /// <summary>
        /// Runs a proc.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteProcedure(this MySqlConnection db, String name, params MySqlParameter[] _params)
        {
            MySqlCommand cmd = db.CreateCommand();
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Runs a proc and returns its return value.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="return_type">Return value's expected type</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteProcedure(this MySqlTransaction tran, String name, MySqlDbType return_type, params MySqlParameter[] _params)
        {
            MySqlCommand cmd = tran.Connection.CreateCommand();
            cmd.Transaction = tran;
            return ExecuteProcedureInternal(cmd, name, return_type, _params);
        }

        /// <summary>
        /// Runs a proc.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteProcedure(this MySqlTransaction tran, String name, params MySqlParameter[] _params)
        {
            MySqlCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = name;
            cmd.Transaction = tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }

        private static object ExecuteProcedureInternal(MySqlCommand cmd, String name, MySqlDbType return_type, MySqlParameter[] _params)
        {
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            string pname = "result";
            while (cmd.Parameters.Contains("@" + pname)) pname = "x" + pname;
            pname = "@" + pname;
            MySqlParameter p = new MySqlParameter(pname, return_type);
            p.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            return cmd.Parameters[pname].Value;
        }
        #endregion

        #region DataReader convenience
        /// <summary>
        /// Obtains a string value or returns a default value if null.
        /// </summary>
        /// <param name="reader">Active reader with data</param>
        /// <param name="column">Column ordinal</param>
        /// <param name="_default">Default value</param>
        /// <returns></returns>
        public static String GetStringOrDefault(this IDataReader reader, int column, String _default)
        {
            return reader.IsDBNull(column) ? _default : reader.GetString(column);
        }

        /// <summary>
        /// Obtains a string value or returns an empty string if null.
        /// </summary>
        /// <param name="reader">Active reader with data</param>
        /// <param name="column">Column ordinal</param>
        /// <returns></returns>
        public static String GetStringOrDefault(this IDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? "" : reader.GetString(column);
        }

        /// <summary>
        /// Obtains a string value or returns a default value if null.
        /// </summary>
        /// <param name="reader">Active reader with data</param>
        /// <param name="column">Column name</param>
        /// <param name="_default">Default value</param>
        /// <returns></returns>
        public static String GetStringOrDefault(this IDataReader reader, String column, String _default)
        {
            return (reader[column] is DBNull) ? _default : (String)reader[column];
        }

        /// <summary>
        /// Obtains a string value or returns an empty string if null.
        /// </summary>
        /// <param name="reader">Active reader with data</param>
        /// <param name="column">Column name</param>
        /// <returns></returns>
        public static String GetStringOrDefault(this IDataReader reader, String column)
        {
            return (reader[column] is DBNull) ? "" : (String)reader[column];
        }

        public static void GetBytes(this IDataReader reader, String column, long fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            reader.GetBytes(reader.GetOrdinal(column), fieldOffset, buffer, bufferOffset, length);
        }

        public static byte[] GetByteArray(this IDataReader reader, int column)
        {
            // optimized version of http://msdn.microsoft.com/en-us/library/87z0hy49%28v=vs.110%29.aspx

            MemoryStream m = new MemoryStream();
            const int BUFFER_LENGTH = 256;
            byte[] buffer = new byte[BUFFER_LENGTH];

            long progress = 0;
            long lastProgress;

            do
            {
                lastProgress = reader.GetBytes(column, progress, buffer, 0, BUFFER_LENGTH);
                m.Write(buffer, 0, (int)lastProgress);
                progress += lastProgress;

            } while (lastProgress == BUFFER_LENGTH);

            m.Flush();
            return m.GetBuffer();
        }

        public static byte[] GetByteArray(this IDataReader reader, int column, int length)
        {
            byte[] result = new byte[length];
            reader.GetBytes(column, 0, result, 0, length);
            return result;
        }

        public static byte[] GetByteArray(this IDataReader reader, String column)
        {
            return GetByteArray(reader, reader.GetOrdinal(column));
        }

        public static byte[] GetByteArray(this IDataReader reader, String column, int length)
        {
            return GetByteArray(reader, reader.GetOrdinal(column), length);
        }
        #endregion
    }
}