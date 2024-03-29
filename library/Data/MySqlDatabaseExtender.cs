﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PkmnFoundations.Data
{
    public static class MySqlDatabaseExtender
    {
        // I can't find anything like a "parameter factory" in ADO.NET nor a virutal clone method,
        // so we need an implementation of these for each database engine.

        /// <summary>
        /// Creates a clone of the SqlParameter collection
        /// </summary>
        /// <param name="collection">Collection to be cloned</param>
        public static MySqlParameter[] CloneParameters(this IEnumerable<MySqlParameter> collection)
        {
            int count = collection.Count();
            MySqlParameter[] result = new MySqlParameter[count];

            int x = 0;
            foreach (MySqlParameter p in collection)
            {
                result[x] = p.CloneParameter();
                x++;
            }

            return result;
        }

        public static MySqlParameter CloneParameter(this MySqlParameter param)
        {
            MySqlParameter result = new MySqlParameter(param.ParameterName, 
                (MySqlDbType)param.DbType, param.Size, param.Direction, 
                param.IsNullable, param.Precision, param.Scale, 
                param.SourceColumn, param.SourceVersion, param.Value);
            result.DbType = param.DbType;
            return result;
        }

        /// <summary>
        /// Runs a proc and returns its return value.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="return_type">Return value's expected type</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static object ExecuteProcedure(this MySqlConnection db, string name, MySqlDbType return_type, params MySqlParameter[] _params)
        {
            return ExecuteProcedureInternal(db.CreateCommand(), name, return_type, _params);
        }

        /// <summary>
        /// Runs a proc.
        /// </summary>
        /// <param name="db">Open data connection</param>
        /// <param name="sqlstr">SQL string</param>
        /// <param name="_params">List of parameters to use with the SQL</param>
        public static int ExecuteProcedure(this MySqlConnection db, string name, params MySqlParameter[] _params)
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
        public static object ExecuteProcedure(this MySqlTransaction tran, string name, MySqlDbType return_type, params MySqlParameter[] _params)
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
        public static int ExecuteProcedure(this MySqlTransaction tran, string name, params MySqlParameter[] _params)
        {
            MySqlCommand cmd = tran.Connection.CreateCommand();
            cmd.CommandText = name;
            cmd.Transaction = tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(_params);
            return cmd.ExecuteNonQuery();
        }

        private static object ExecuteProcedureInternal(MySqlCommand cmd, string name, MySqlDbType return_type, MySqlParameter[] _params)
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
    }
}
