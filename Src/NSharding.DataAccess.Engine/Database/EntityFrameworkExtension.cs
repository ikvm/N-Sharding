
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Entity Framework 扩展
    /// </summary>
    static class EntityFrameworkExtension
    {
        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteSqlDataSet(this System.Data.Entity.Database db, string sql, IDbDataParameter[] parameters)
        {
            using (var conn = new SqlConnection(db.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                var cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }
                var adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(this System.Data.Entity.Database db, string sql, IDbDataParameter[] parameters)
        {
            using (var conn = new SqlConnection(db.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                var cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }

                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(this System.Data.Entity.Database db, string sql, IDbDataParameter[] parameters)
        {
            using (var conn = new SqlConnection(db.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                var cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }

                return cmd.ExecuteReader();
            }
        }

        public static void ExecSqlStatement(this System.Data.Entity.Database db, string sql, List<IDbDataParameter> parameters)
        {
            using (var conn = new SqlConnection(db.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                var cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }

                cmd.ExecuteNonQuery();
            }
        }

        public static IDbDataParameter MakeInParam(this System.Data.Entity.Database db, string parameterName, DbType dataType, object value)
        {
            using (var conn = new SqlConnection(db.Connection.ConnectionString))
            {
                var parameter = conn.CreateCommand().CreateParameter();
                parameter.Direction = ParameterDirection.Input;
                parameter.SqlValue = value;
                parameter.ParameterName = parameterName;
                parameter.DbType = dataType;

                return parameter;
            }
        }
    }
}
