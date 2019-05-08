using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core.Database
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
            return new DataSet();
        }

        public DataSet ExecuteDataSet(string sqlString, IDbDataParameter[] dbDataParameter)
        {
            return new DataSet();
        }

    }
}
