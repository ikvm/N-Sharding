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
        object ExecuteScalar(string sqlStrings);
        //IDataReader ExecuteReader(string sqlStrings);
        //void ExecSqlStatement(string sqlString, List<IDbDataParameter> parameters);
        //IDbDataParameter MakeInParam(string fieldName, object dateTime, object value);
    }
}