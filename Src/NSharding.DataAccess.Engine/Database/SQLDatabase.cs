using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    class SQLDatabase : DbContext, ITeldDatabase
    {
        public SQLDatabase(string databaseSource) :
            base(databaseSource)
        {

        }

        public void ExecSqlStatement(string sqlString)
        {
            this.Database.ExecuteSqlCommand(sqlString);
        }
        public void ExecSqlStatement(string sqlString, IDbDataParameter[] dbDataParameter)
        {
            this.Database.ExecuteSqlCommand(sqlString, dbDataParameter);
        }

        public DataSet ExecuteDataSet(string sqlString)
        {
            return this.Database.ExecuteSqlDataSet(sqlString, null);
        }

        public DataSet ExecuteDataSet(string sqlString, IDbDataParameter[] dbDataParameter)
        {
            return this.Database.ExecuteSqlDataSet(sqlString, dbDataParameter);
        }

        public object ExecuteScalar(string sqlString)
        {
            return this.Database.ExecuteScalar(sqlString, null);
        }

        public IDataReader ExecuteReader(string sqlString)
        {
            return this.Database.ExecuteReader(sqlString, null);
        }

        public void ExecSqlStatement(string sqlString, List<IDbDataParameter> parameters)
        {
            this.Database.ExecSqlStatement(sqlString, parameters);
        }

        public IDbDataParameter MakeInParam(string parameterName, DbType dbType, object value)
        {
            return this.Database.MakeInParam(parameterName, dbType, value);
        }
    }
}
