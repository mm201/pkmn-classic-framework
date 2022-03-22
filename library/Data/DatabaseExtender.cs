using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
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
        public static DataTable ExecuteDataTable(this DbConnection db, string sqlstr, params IDataParameter[] _params)
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
        public static DataTable ExecuteDataTable(this DbConnection db, string sqlstr, IEnumerable<IDataParameter> _params)
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
        public static DataTable ExecuteDataTable(this DbTransaction tran, string sqlstr, params IDataParameter[] _params)
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
        public static DataTable ExecuteDataTable(this DbTransaction tran, string sqlstr, IEnumerable<IDataParameter> _params)
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
        public static IDataReader ExecuteReader(this DbConnection db, string sqlstr, params IDataParameter[] _params)
        {
            // hooray DbConnection provides a command factory
            DbCommand cmd = db.CreateCommand();
            cmd.CommandText = sqlstr;
            // fixme: catch "System.ArgumentException: The SqlParameter is already contained 
            // by another SqlParameterCollection." and add a clone instead
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// Runs a command and returns a reader that iterates its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static IDataReader ExecuteReader(this DbConnection db, string sqlstr, IEnumerable<IDataParameter> _params)
        {
            return db.ExecuteReader(sqlstr, _params.ToArray());
        }

        /// <summary>
        /// Runs a command and returns a reader that iterates its results.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static IDataReader ExecuteReader(this DbTransaction tran, string sqlstr, params IDataParameter[] _params)
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
        public static IDataReader ExecuteReader(this DbTransaction tran, string sqlstr, IEnumerable<IDataParameter> _params)
        {
            return tran.ExecuteReader(sqlstr, _params.ToArray());
        }

        /// <summary>
        /// Runs a command and returns the first column of the first row in the query.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteScalar(this DbConnection db, string sqlstr, params IDataParameter[] _params)
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
        public static int ExecuteNonQuery(this DbConnection db, string sqlstr, params IDataParameter[] _params)
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
        public static object ExecuteScalar(this DbTransaction tran, string sqlstr, params IDataParameter[] _params)
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
        public static int ExecuteNonQuery(this DbTransaction tran, string sqlstr, params IDataParameter[] _params)
        {
            DbCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = sqlstr;
            cmd.Transaction = tran;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
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
        public static string GetStringOrDefault(this IDataReader reader, int column, string _default)
        {
            return reader.IsDBNull(column) ? _default : reader.GetString(column);
        }

        /// <summary>
        /// Obtains a string value or returns an empty string if null.
        /// </summary>
        /// <param name="reader">Active reader with data</param>
        /// <param name="column">Column ordinal</param>
        /// <returns></returns>
        public static string GetStringOrDefault(this IDataReader reader, int column)
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
        public static string GetStringOrDefault(this IDataReader reader, string column, string _default)
        {
            return (reader[column] is DBNull) ? _default : (string)reader[column];
        }

        /// <summary>
        /// Obtains a string value or returns an empty string if null.
        /// </summary>
        /// <param name="reader">Active reader with data</param>
        /// <param name="column">Column name</param>
        /// <returns></returns>
        public static string GetStringOrDefault(this IDataReader reader, string column)
        {
            return (reader[column] is DBNull) ? "" : (string)reader[column];
        }

        public static void GetBytes(this IDataReader reader, string column, long fieldOffset, byte[] buffer, int bufferOffset, int length)
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

        public static byte[] GetByteArray(this IDataReader reader, string column)
        {
            return GetByteArray(reader, reader.GetOrdinal(column));
        }

        public static byte[] GetByteArray(this IDataReader reader, string column, int length)
        {
            return GetByteArray(reader, reader.GetOrdinal(column), length);
        }

        public static bool IsDBNull(this IDataReader reader, string column)
        {
            return reader.IsDBNull(reader.GetOrdinal(column));
        }
        #endregion

        public static T Cast<T>(object value)
        {
            if (value is DBNull) value = null;
            return (T)value; // Allow InvalidCastException to escape.
        }
    }
}