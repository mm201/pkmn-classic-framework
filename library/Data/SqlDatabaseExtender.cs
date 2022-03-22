using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PkmnFoundations.Data
{
    public static class SqlDatabaseExtender
    {
        // I can't find anything like a "parameter factory" in ADO.NET nor a virutal clone method,
        // so we need an implementation of these for each database engine.

        /// <summary>
        /// Creates a clone of the SqlParameter collection
        /// </summary>
        /// <param name="collection">Collection to be cloned</param>
        public static SqlParameter[] CloneParameters(this IEnumerable<SqlParameter> collection)
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
        public static object ExecuteProcedure(this SqlConnection db, string name, SqlDbType return_type, params SqlParameter[] _params)
        {
            return ExecuteProcedureInternal(db.CreateCommand(), name, return_type, _params);
        }

        /// <summary>
        /// Runs a proc.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteProcedure(this SqlConnection db, string name, params SqlParameter[] _params)
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
        public static object ExecuteProcedure(this SqlTransaction tran, string name, SqlDbType return_type, params SqlParameter[] _params)
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
        public static int ExecuteProcedure(this SqlTransaction tran, string name, params SqlParameter[] _params)
        {
            SqlCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = name;
            cmd.Transaction = tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }

        private static object ExecuteProcedureInternal(SqlCommand cmd, string name, SqlDbType return_type, SqlParameter[] _params)
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
    }
}
