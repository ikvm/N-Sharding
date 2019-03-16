using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 内部数据访问服务
    /// </summary>
    class DatabaseImpl : IDatabase
    {
        private static readonly string DataAccessSqlError = "DataAccessSqlError";
        /// <summary>
        /// 获取数据库服务
        /// </summary>
        /// <param name="sqlStatement">SQL语句</param>
        /// <returns>数据库服务</returns>
        public ITeldDatabase GetDatabase(SqlStatement sqlStatement)
        {
            var dataSourceName = sqlStatement.SqlBuildingInfo.DataSource;

            return null;
            //return TeldDatabaseFactory.GetDataBase(dataSourceName);
        }

        /// <summary>
        /// 获取数据库服务
        /// </summary>
        /// <param name="dataSourceName">数据源</param>
        /// <returns>数据库服务</returns>
        public ITeldDatabase GetDatabase(string dataSourceName = "")
        {
            return null;
            //return TeldDatabaseFactory.GetDataBase(dataSourceName);
        }

        /// <summary>
        /// 在数据库事务中执行SQL语句返回影响行数
        /// </summary>
        /// <param name="sqls">SQL语句</param>        
        public void ExecuteSQLWithTransaction(SqlStatementCollection sqls)
        {
            if (sqls == null)
                throw new ArgumentNullException("DatabaseImpl.ExecuteSQLWithTransaction.sqls");

            var sqlGroups = sqls.GroupBy(i => i.SqlBuildingInfo.DataSource);

            using (var ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                try
                {
                    foreach (var sqlGroup in sqlGroups)
                    {
                        if (sqlGroup.Count() == 0) continue;
                        var dataSourceName = sqlGroup.FirstOrDefault().SqlBuildingInfo.DataSource;

                        using (var db = GetDatabase(dataSourceName))
                        {
                            foreach (var sqlstatment in sqlGroup)
                            {
                                var sqlString = sqlstatment.ToSQL();
                                try
                                {
                                    var parameters = new List<IDbDataParameter>();
                                    parameters = CreateParameters(db, sqlstatment, parameters);

                                    if (parameters == null || parameters.Count == 0)
                                    {
                                        db.ExecSqlStatement(sqlString);
                                    }
                                    else
                                    {
                                        db.ExecSqlStatement(sqlString, parameters.ToArray());
                                    }
                                }
                                catch (Exception e)
                                {
                                    MonitorError(e, sqlString);
                                    throw;
                                }
                            }
                        }
                    }

                    ts.Complete();

                }
                catch (Exception e)
                {
                    MonitorError(e, new string[sqls.Count]);
                    throw;
                }
        }

        private List<IDbDataParameter> CreateParameters(ITeldDatabase db, SqlStatement sqlStatement, List<IDbDataParameter> parameters)
        {
            if (sqlStatement is InsertSqlStatement)
                parameters = ParameterHandlerForInsert(sqlStatement, db);

            else if (sqlStatement is UpdateSqlStatement)
            {
                parameters = ParameterHandlerForUpdate(sqlStatement, db);
            }
            else if (sqlStatement is DeleteSqlStatement)
            {
                parameters = ParameterHandlerForDelete(sqlStatement, db);
            }

            return parameters;
        }

        /// <summary>
        /// 执行SQL获取数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据</returns>
        public DataSet GetData(SqlStatement sql)
        {
            if (sql == null)
                throw new ArgumentNullException("DatabaseImpl.GetData.sql");

            var sqlStrings = new string[1] { sql.ToSQL() };
            var tableNames = new string[1] { sql.SqlBuildingInfo.CurrentNode.ID };

            return GetData(sql.SqlBuildingInfo.DataSource, sqlStrings, tableNames);
        }

        /// <summary>
        /// 执行SQL获取数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据</returns>
        public List<DataTable> GetDataCollection(SqlStatementCollection sqls)
        {
            if (sqls == null)
                throw new ArgumentNullException("DatabaseImpl.GetData.sqls");

            var dataTables = new List<DataTable>();
            var sqlGroups = sqls.GroupBy(i => i.SqlBuildingInfo.DataSource);

            try
            {
                foreach (var sqlGroup in sqlGroups)
                {
                    if (sqlGroup.Count() == 0) continue;
                    var dataSourceName = sqlGroup.FirstOrDefault().SqlBuildingInfo.DataSource;

                    using (var db = GetDatabase(dataSourceName))
                    {
                        foreach (var sqlstatment in sqlGroup)
                        {
                            var sqlString = sqlstatment.ToSQL();
                            try
                            {
                                var parameters = new List<IDbDataParameter>();

                                if (sqlstatment is SelectSqlStatement)
                                    parameters = ParameterHandlerForSelect(sqlstatment, db);

                                var ds = new DataSet();
                                if (parameters == null || parameters.Count == 0)
                                {
                                    ds = db.ExecuteDataSet(sqlString);
                                }
                                else
                                {
                                    ds = db.ExecuteDataSet(sqlString, parameters.ToArray());
                                }

                                if (ds != null)
                                {
                                    foreach (DataTable dt in ds.Tables)
                                    {
                                        dataTables.Add(dt);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                MonitorError(e, sqlString);
                                throw;
                            }
                        }
                    }
                }

                return dataTables;
            }
            catch (Exception e)
            {
                MonitorError(e, new string[sqls.Count]);
                throw;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="dataSourceName">数据源名称</param>
        /// <param name="sqlStrings">SQL语句</param>
        /// <param name="tableNames">表名称</param>
        /// <returns>数据集</returns>
        private DataSet GetData(string dataSourceName, string[] sqlStrings, string[] tableNames)
        {
            using (var db = GetDatabase(dataSourceName))
            {
                try
                {
                    DataSet dataset = db.ExecuteDataSet(sqlStrings);
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        dataset.Tables[i].TableName = tableNames[i];
                    }

                    return dataset;
                }
                catch (Exception e)
                {
                    MonitorError(e, sqlStrings);
                    throw;
                }
            }
        }

        /// <summary>
        /// 执行SQL获取数据的第一行第一列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据的第一行第一列</returns>
        public object ExecuteScalar(SqlStatement sql)
        {
            var sqlStrings = sql.ToSQL();
            using (var db = GetDatabase(sql.SqlBuildingInfo.DataSource))
            {
                try
                {
                    return db.ExecuteScalar(sqlStrings);
                }
                catch (Exception e)
                {
                    MonitorError(e, sqlStrings);
                    throw;
                }
            }
        }

        /// <summary>
        /// 执行SQL返回IDataReader
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>IDataReader</returns>
        public IDataReader GetDataReader(SqlStatement sql)
        {
            var sqlStrings = sql.ToSQL();
            using (var db = GetDatabase(sql.SqlBuildingInfo.DataSource))
            {
                try
                {
                    return db.ExecuteReader(sqlStrings);
                }
                catch (Exception e)
                {
                    MonitorError(e, sqlStrings);
                    throw;
                }
            }
        }

        /// <summary>
        /// 处理sql集合中的参数，调用底层数据访问接口，返回受影响的行数据。
        /// </summary>
        /// <param name="sqls">待处理的SQL语句集合</param>
        /// <returns>受影响的行数据</returns>
        public void ExecuteSQLs(SqlStatementCollection sqls)
        {
            if (sqls == null)
                throw new ArgumentNullException("DatabaseImpl.ExecuteSQLs.sqls");
            if (sqls.Count == 0)
                return;

            foreach (var sqlstatment in sqls)
            {
                using (var db = GetDatabase(sqlstatment.SqlBuildingInfo.DataSource))
                {
                    var sqlString = sqlstatment.ToSQL();
                    try
                    {
                        var parameters = new List<IDbDataParameter>();

                        if (sqlstatment is InsertSqlStatement)
                            parameters = ParameterHandlerForInsert(sqlstatment, db);

                        else if (sqlstatment is UpdateSqlStatement)
                        {
                            parameters = ParameterHandlerForUpdate(sqlstatment, db);
                        }

                        if (parameters == null || parameters.Count == 0)
                        {
                            db.ExecSqlStatement(sqlString);
                        }
                        else
                        {
                            db.ExecSqlStatement(sqlString, parameters);
                        }
                    }
                    catch (Exception e)
                    {
                        MonitorError(e, sqlString);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 对Update类SQL语句进行参数处理
        /// </summary>
        /// <param name="sqlStatement">待处理的SQL语句</param>
        /// <param name="db">数据访问对象</param>
        /// <returns>处理后的参数数组</returns>
        private List<IDbDataParameter> ParameterHandlerForUpdate(SqlStatement sqlStatement, ITeldDatabase db)
        {
            var updateSql = sqlStatement as UpdateSqlStatement;
            var parameters = new List<IDbDataParameter>(updateSql.UpdateFields.ChildCollection.Count);

            for (int parameterIndex = 0; parameterIndex < updateSql.UpdateFields.ChildCollection.Count; parameterIndex++)
            {
                UpdateField updateField = updateSql.UpdateFields.ChildCollection[parameterIndex] as UpdateField;
                if (updateField == null)
                    throw new Exception("ParameterHandlerForUpdate.updateField, parameterIndex:" + parameterIndex);

                FieldValue feildValue = updateSql.UpdateValues.ChildCollection[parameterIndex] as FieldValue;
                if (feildValue == null)
                    throw new Exception("ParameterHandlerForUpdate.feildValue, parameterIndex:" + parameterIndex);

                parameters.Add(ParameterHandler(db, feildValue, updateField));
            }

            return parameters;
        }

        /// <summary>
        /// 对Select类SQL语句进行参数处理
        /// </summary>
        /// <param name="sqlStatement">待处理的SQL语句</param>
        /// <param name="db">数据访问对象</param>
        /// <returns>处理后的参数数组</returns>
        private List<IDbDataParameter> ParameterHandlerForSelect(SqlStatement sqlStatement, ITeldDatabase db)
        {
            var selectSql = sqlStatement as SelectSqlStatement;
            var parameters = new List<IDbDataParameter>(selectSql.FilterCondition.ChildCollection.Count);

            for (int parameterIndex = 0; parameterIndex < selectSql.FilterCondition.ChildCollection.Count; parameterIndex++)
            {
                var filterCondition = selectSql.FilterCondition.ChildCollection[parameterIndex];
                if (filterCondition is SqlPrimaryKey)
                {
                    foreach (var keyField in filterCondition.ChildCollection)
                    {
                        SqlPrimaryKeyField pkField = keyField as SqlPrimaryKeyField;
                        if (pkField == null)
                            throw new Exception("ParameterHandlerForUpdate.updateField, parameterIndex:" + parameterIndex);

                        parameters.Add(ParameterHandler(db, pkField.Value, pkField));
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// 对Select类SQL语句进行参数处理
        /// </summary>
        /// <param name="sqlStatement">待处理的SQL语句</param>
        /// <param name="db">数据访问对象</param>
        /// <returns>处理后的参数数组</returns>
        private List<IDbDataParameter> ParameterHandlerForDelete(SqlStatement sqlStatement, ITeldDatabase db)
        {
            var deleteSql = sqlStatement as DeleteSqlStatement;
            var parameters = new List<IDbDataParameter>();

            if (deleteSql.SubQuerySql != null)
            {
                for (int parameterIndex = 0; parameterIndex < deleteSql.SubQuerySql.Condition.ChildCollection.Count; parameterIndex++)
                {
                    var filterCondition = deleteSql.SubQuerySql.Condition.ChildCollection[parameterIndex];
                    if (filterCondition is SqlPrimaryKey)
                    {
                        foreach (var keyField in filterCondition.ChildCollection)
                        {
                            SqlPrimaryKeyField pkField = keyField as SqlPrimaryKeyField;
                            if (pkField == null)
                                throw new Exception("ParameterHandlerForDelete.PkField, parameterIndex:" + parameterIndex);

                            parameters.Add(ParameterHandler(db, pkField.Value, pkField));
                        }
                    }
                }
            }
            else
            {
                for (int parameterIndex = 0; parameterIndex < deleteSql.Conditions.ChildCollection.Count; parameterIndex++)
                {
                    var filterCondition = deleteSql.Conditions.ChildCollection[parameterIndex];
                    if (filterCondition is SqlPrimaryKey)
                    {
                        foreach (var keyField in filterCondition.ChildCollection)
                        {
                            SqlPrimaryKeyField pkField = keyField as SqlPrimaryKeyField;
                            if (pkField == null)
                                throw new Exception("ParameterHandlerForDelete.PkField, parameterIndex:" + parameterIndex);

                            parameters.Add(ParameterHandler(db, pkField.Value, pkField));
                        }
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// 对Insert类SQL语句进行参数处理
        /// </summary>
        /// <param name="sqlStatement">待处理的SQL语句</param>
        /// <param name="db">数据访问对象</param>
        /// <returns>处理后的参数数组</returns>
        private List<IDbDataParameter> ParameterHandlerForInsert(SqlStatement sqlStatement, ITeldDatabase db)
        {
            var insertSql = sqlStatement as InsertSqlStatement;
            var parameters = new List<IDbDataParameter>(insertSql.InsertFields.ChildCollection.Count);

            for (int parameterIndex = 0; parameterIndex < insertSql.InsertFields.ChildCollection.Count; parameterIndex++)
            {
                InsertField insertField = insertSql.InsertFields.ChildCollection[parameterIndex] as InsertField;
                if (insertField == null)
                    throw new Exception("ParameterHandlerForInsert.insertField, parameterIndex:" + parameterIndex);
                InsertValue insertValue = insertSql.InsertValues.ChildCollection[parameterIndex] as InsertValue;
                if (insertValue == null)
                    throw new Exception("ParameterHandlerForInsert.insertValue, parameterIndex:" + parameterIndex);

                parameters.Add(ParameterHandler(db, insertValue, insertField));
            }

            return parameters;
        }

        /// <summary>
        /// 处理特殊数据类型参数
        /// </summary>
        /// <param name="db">数据访问对象</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="fieldValue">字段值</param>
        /// <param name="field">待处理字段</param>
        private IDbDataParameter ParameterHandler(ITeldDatabase db, FieldValue fieldValue, Field field)
        {
            IDbDataParameter parameter = null;
            switch ((ElementDataType)fieldValue.DataType)
            {
                case ElementDataType.DateTime:
                case ElementDataType.Date:
                    parameter = db.MakeInParam(field.FieldName, DbDataType.DateTime, fieldValue.Value);
                    break;
                case ElementDataType.Binary:
                    {
                        byte[] byteAry = fieldValue.Value as Byte[];
                        parameter = db.MakeInParam(field.FieldName, DbDataType.Blob, fieldValue.Value);
                        break;
                    }
                case ElementDataType.Boolean:
                case ElementDataType.Integer:
                    parameter = db.MakeInParam(field.FieldName, DbDataType.Int, fieldValue.Value);
                    break;
                case ElementDataType.Decimal:
                    parameter = db.MakeInParam(field.FieldName, DbDataType.Decimal, fieldValue.Value);
                    break;
                case ElementDataType.String:
                    parameter = db.MakeInParam(field.FieldName, DbDataType.VarChar, fieldValue.Value);
                    break;
                case ElementDataType.Text:
                    parameter = db.MakeInParam(field.FieldName, DbDataType.Clob, fieldValue.Value);
                    break;
                default:
                    throw new NotSupportedException(((ElementDataType)fieldValue.DataType).ToString());
            }

            return parameter;
        }

        private void MonitorError(Exception e, params string[] sqls)
        {
            if (sqls == null)
                sqls = new string[0];

            var context = new Dictionary<string, string>()
            {
                {"Error", e.ToString()},
                {"sqls",string.Join(",", sqls)}
            };

            //TODO
            //MonitorClient.Send("DataAccessSqlError", 1, null, context);
        }
    }
}
