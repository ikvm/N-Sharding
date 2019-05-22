using System.Collections.Generic;
using System.Data;
using System;

namespace NSharding.DataAccess.Core
{
    public interface ITeldDatabase : IDisposable
    {
        void ExecSqlStatement(string sqlString);
        void ExecSqlStatement(string sqlString, IDbDataParameter[] dbDataParameter);
        DataSet ExecuteDataSet(string sqlString);
        DataSet ExecuteDataSet(string sqlString, IDbDataParameter[] dbDataParameter);
        //DataSet ExecuteDataSet(string[] sqlStrings);
        object ExecuteScalar(string sqlString);
        IDataReader ExecuteReader(string sqlString);

        void ExecSqlStatement(string sqlString, List<IDbDataParameter> parameters);

        IDbDataParameter MakeInParam(string parameterName, DbType dbType, object value);
    }
}