using NSharding.DomainModel.Spi;
using NSharding.Sharding.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 表SQL构造基础类
    /// </summary>
    internal abstract class BaseSqlBuildStrategy : ISqlBuildStrategy
    {
        /// <summary>
        /// 构造GSPAbsDBTable不包含数据的Sql（即SqlSchema）。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public abstract SqlStatementCollection BuildTableSqlSchema(SqlBuildingContext context);

        /// <summary>
        /// 在SqlSchema基础上，构造GSPAbsDBTable包含数据的Sql。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public abstract void BuildTableSqlDetail(SqlStatementCollection sqls, SqlBuildingContext context);

        /// <summary>
        /// 处理通用中间对象的节点的GSPAbsDBTable。
        /// </summary>
        /// <param name="sql">Sql语句对象。</param>
        /// <param name="domainModel">领域模型。</param>
        /// <param name="domainObject">领域对象。</param>
        /// <param name="dataObject">领域对象对应的数据对象。</param>        
        /// <param name="tableName">表名称。</param>
        protected void HandlingSqlStatement(SqlStatement sql, SqlBuildingContext context)
        {
            sql.NodeID = context.Node.ID;
            sql.CommonObjectID = context.CommonObject.ID;
            sql.NodeVersion = context.CommonObject.Version.ToString();
            sql.CommonObjectVersion = context.CommonObject.Version.ToString();
            sql.TableName = context.TableName;

            //构造SqlBuildingInfo  
            sql.SqlBuildingInfo = this.InitSqlBuildingInfo(context.CommonObject, context.Node, context.DataObject, context.TableName, context.DataSource);

            //复合主键                       
            foreach (var item in context.DataObject.PKColumns)
            {
                var keyField = new SqlPrimaryKeyField();
                keyField.FieldName = item.ColumnName;
                sql.PrimaryKeys.ChildCollection.Add(keyField);
            }
        }

        /// <summary>
        /// 初始化SQL构造中间变量
        /// </summary>
        /// <param name="commonObject">通用中间对象</param>
        /// <param name="coNode">节点</param>
        /// <param name="dataObject">数据对象</param>
        /// <param name="currentDbTable">当前操作对应的数据库表</param>
        /// <returns>SQL构造中间变量</returns>
        protected SqlBuildingInfo InitSqlBuildingInfo(DomainModel.Spi.DomainModel commonObject, DomainObject coNode, DataObject dataObject, string tableName, string dataSourceName)
        {
            //构造SqlBuildingInfo
            var sqlInfo = new SqlBuildingInfo();
            sqlInfo.CommonObject = commonObject;
            sqlInfo.RootNode = commonObject.RootDomainObject;
            sqlInfo.CurrentNode = coNode;
            sqlInfo.CurrentDataObject = dataObject;
            sqlInfo.TableName = tableName;
            sqlInfo.DataSource = dataSourceName;
            sqlInfo.CurrentSqlTable = new SqlTable(sqlInfo.TableName, sqlInfo.TableName, sqlInfo.TableName);

            //SqlTable注册处理
            RegistSqlTable(sqlInfo.CurrentSqlTable.TableName, sqlInfo.CurrentSqlTable, sqlInfo);

            //如果当前节点是根节点:
            if (sqlInfo.RootNode.ID == sqlInfo.CurrentNode.ID)
            {
                sqlInfo.RootDataObject = dataObject;
                sqlInfo.RootSqlTable = sqlInfo.CurrentSqlTable;
            }
            else
            {
                sqlInfo.RootDataObject = commonObject.RootDomainObject.DataObject;

                //sqlInfo.RootSqlTable = GetSubQuerySql(commonObject, commonObject.RootNode);
            }

            //SqlTable再次注册处理
            RegistSqlTable(sqlInfo.CurrentSqlTable.TableName, sqlInfo.CurrentSqlTable, sqlInfo);

            return sqlInfo;
        }

        /// <summary>
        /// 初始化SQL构造中间变量
        /// </summary>
        /// <param name="commonObject">通用中间对象</param>
        /// <param name="coNode">节点</param>
        /// <param name="dataObject">数据对象</param>
        /// <param name="currentDbTable">当前操作对应的数据库表</param>
        /// <returns>SQL构造中间变量</returns>
        protected SqlBuildingInfo InitSqlBuildingInfoForSelect(DomainModel.Spi.DomainModel commonObject, DomainObject coNode, DataObject dataObject, string tableName)
        {
            var sqlInfo = new SqlBuildingInfo()
            {
                CommonObject = commonObject,
                RootNode = commonObject.RootDomainObject,
                CurrentNode = coNode,
                CurrentDataObject = dataObject,
                TableName = tableName
            };
            sqlInfo.CurrentSqlTable = new SqlTable(sqlInfo.TableName, sqlInfo.TableName, sqlInfo.TableName);

            //如果当前节点是根节点:
            if (sqlInfo.CurrentNode.IsRootObject || sqlInfo.RootNode.ID == sqlInfo.CurrentNode.ID)
            {
                sqlInfo.RootDataObject = dataObject;
                sqlInfo.RootSqlTable = sqlInfo.CurrentSqlTable;
            }
            else
            {
                sqlInfo.RootDataObject = commonObject.RootDomainObject.DataObject;
                //sqlInfo.RootSqlTable = CoHelper.GetRootSQLDomTable(commonObject);
            }

            RegistSqlTable(sqlInfo.CurrentDataObject.ID, sqlInfo.CurrentSqlTable, sqlInfo);

            return sqlInfo;
        }

        /// <summary>
        /// 注册SQL中的表，并根据情况更改表别名
        /// </summary>
        /// <param name="tableKey">表唯一标识</param>
        /// <param name="table">描述SQL语句的表</param>
        /// <param name="sqlInfo"></param>
        protected void RegistSqlTable(string tableKey, SqlTable table, SqlBuildingInfo sqlInfo)
        {
            if (string.IsNullOrWhiteSpace(tableKey))
                throw new ArgumentNullException("RegistSqlTable.tableKey");
            if (table == null)
                throw new ArgumentNullException("RegistSqlTable.SqlTable");

            Hashtable htTable = sqlInfo.SqlTableCollection;
            if (!sqlInfo.SqlTableCollection.Contains(tableKey))
            {
                table.TableAlias = this.GetSqlTableAlias(table.TableAlias, sqlInfo);
                htTable.Add(tableKey, table);
            }
        }

        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sqlInfo"></param>
        /// <returns>考虑到在表关联的时候同一张表，被关联多次，所以需要给表生成别名</returns>
        protected string GetSqlTableAlias(string tableName, SqlBuildingInfo sqlInfo)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException("GetSqlTableAlias.tableName");

            Hashtable htAlias = sqlInfo.SqlTableAliasCollection;
            if (!htAlias.Contains(tableName))
            {
                htAlias.Add(tableName, 0);
                return tableName;
            }
            int index = Convert.ToInt32(htAlias[tableName]) + 1;
            htAlias[tableName] = index;
            return this.GetSqlTableAlias(tableName + index.ToString(), sqlInfo);
        }

        /// <summary>
        /// 获取SQL中的表
        /// </summary>
        /// <param name="tableKey">表的标识 -- 主从关系ID、或者关联关系ID</param>
        /// <param name="sqlInfo">SQL构造信息</param>
        /// <returns>Sql中的表</returns>
        protected SqlTable FindSqlTable(string tableKey, SqlBuildingInfo sqlInfo)
        {
            if (String.IsNullOrWhiteSpace(tableKey))
                throw new ArgumentNullException("FindSqlTable.tableKey");
            if (sqlInfo.SqlTableCollection.Contains(tableKey))
                return (SqlTable)sqlInfo.SqlTableCollection[tableKey];
            return null;
        }


        /// <summary>
        /// 先从缓存中查找，未找到时创建SqlTable。
        /// </summary>
        /// <param name="tableKey">唯一标识。</param>
        /// <param name="tableName">表名。</param>
        /// <param name="tableAlias">表别名。</param>
        /// <param name="realTableName">实表名。</param>
        /// <param name="sqlInfo">Sql构造中间变量。</param>
        /// <returns></returns>
        protected SqlTable TryFindAndRegistSqlTable(string tableKey, string tableName, string tableAlias, string realTableName, SqlBuildingInfo sqlInfo)
        {
            SqlTable table = this.FindSqlTable(tableKey, sqlInfo);
            if (table == null)
                table = new SqlTable(tableName, tableAlias, realTableName);
            this.RegistSqlTable(tableKey, table, sqlInfo);
            return table;
        }

        /// <summary>
        /// 获取CO上某GSPNode的GSPAbsDBTable主键的过滤条件。
        /// </summary>
        /// <param name="context">SQL构造上下文信息。</param>
        /// <param name="sqlInfo">SQL拼装的中间变量。</param>
        /// <param name="coNode">主键过滤条件对应的GSPNode。</param>
        /// <param name="dbTable">主键过滤条件对应的GSPAbsDBTable。</param>
        /// <param name="IsUseFieldPrefix">是否使用表名</param>       
        /// <returns>主键的过滤条件。</returns>
        protected string GetPrimaryKeyCondition(SqlBuildingContext context, SqlBuildingInfo sqlInfo,
            DomainObject coNode, bool IsUseFieldPrefix = false)
        {
            var tableName = context.TableName;
            var tableKey = tableName;
            var sqlPrimaryKey = new SqlPrimaryKey();
            var sqlTable = this.TryFindAndRegistSqlTable(tableKey, tableName, tableName, tableName, sqlInfo);
            var pkColumnData = context.DataContext.GetCurrentDataContextItem(context.Node.ID).PrimaryKeyData;

            foreach (DataColumn item in context.DataObject.PKColumns)
            {
                var pkElement = context.Node.Elements.FirstOrDefault(i => i.DataColumnID == item.ID);
                if (pkElement == null)
                    throw new Exception("Cannot find PkElement, Column ID: " + item.ID);

                if (pkColumnData.ContainsKey(pkElement.ID))
                {
                    var keyField = new SqlPrimaryKeyField();
                    keyField.Table = sqlTable;
                    keyField.FieldName = item.ColumnName;
                    keyField.Value.Value = pkColumnData[pkElement.ID];
                    keyField.IsUseFieldPrefix = IsUseFieldPrefix;
                    sqlPrimaryKey.ChildCollection.Add(keyField);
                }
            }

            return sqlPrimaryKey.ToSQL();
        }

        /// <summary>
        /// 解析外部传入的过滤条件。
        /// </summary>
        /// <param name="context">SQL构造上下文信息。</param>
        /// <param name="currentObject">主键过滤条件对应的GSPNode。</param>
        /// <param name="dataObject">主键过滤条件对应的GSPDataTable。</param>
        /// <param name="sqlInfo">SQL拼装的中间变量。</param>
        /// <returns>解析后的过滤条件。</returns>
        protected void GetSelectSqlCondition(SelectSqlStatement sql, SqlBuildingContext context, DomainObject currentObject, DataObject dataObject)
        {
            //若是非主键形式条件定义。
            if (context.IsUseCondition)
            {
                var condition = this.GetOrdinaryCondition(context, sql.SqlBuildingInfo);
                if (condition != null)
                {
                    sql.FilterCondition.ChildCollection.Add(condition);
                }
            }

            //若是根据主键值，则获取主键值条件Sql语句。
            var sqlPrimaryKey = this.GetPrimaryKeyCondition(context, currentObject, dataObject, sql.SqlBuildingInfo);

            if (sqlPrimaryKey != null)
            {
                sql.FilterCondition.ChildCollection.Add(sqlPrimaryKey);
            }
        }

        /// <summary>
        /// 解析外部传入的过滤条件。
        /// </summary>
        /// <param name="context">SQL构造上下文信息。</param>
        /// <param name="currentObject">主键过滤条件对应的GSPNode。</param>
        /// <param name="dataObject">主键过滤条件对应的GSPDataTable。</param>
        /// <param name="sqlInfo">SQL拼装的中间变量。</param>
        /// <returns>解析后的过滤条件。</returns>
        protected void GetUpdateSqlCondition(UpdateSqlStatement sql, SqlBuildingContext context, DomainObject currentObject, DataObject dataObject)
        {
            //若是非主键形式条件定义。
            if (context.IsUseCondition)
            {
                var condition = this.GetOrdinaryCondition(context, sql.SqlBuildingInfo);
                if (condition != null)
                {
                    sql.UpdateCondition.ChildCollection.Add(condition);
                }
            }

            //若是根据主键值，则获取主键值条件Sql语句。
            var sqlPrimaryKey = this.GetPrimaryKeyCondition(context, currentObject, dataObject, sql.SqlBuildingInfo);

            if (sqlPrimaryKey != null)
            {
                sql.UpdateCondition.ChildCollection.Add(sqlPrimaryKey);
            }
        }

        //非主键的SQL过滤条件。
        private FilterConditionStatement GetOrdinaryCondition(SqlBuildingContext context, SqlBuildingInfo sqlBuildInfo)
        {
            return null;
        }

        ///// <summary>
        ///// 获取CO上某GSPNode的主键的过滤条件。
        ///// </summary>
        ///// <param name="context">SQL构造上下文信息。</param>
        ///// <param name="curNode">主键过滤条件对应的GSPNode。</param>
        ///// <param name="curDataObject">主键过滤条件对应的GSPDataTable。</param>
        ///// <param name="sqlInfo">SQL拼装的中间变量。</param>
        ///// <returns>主键的过滤条件。</returns>
        //private string GetPrimaryKeyCondition(SqlBuildingContext context, DomainObject curNode, DataObject curDataObject, SqlBuildingInfo sqlInfo)
        //{
        //    var tableName = context.DataObjectTableMapping[curDataObject.ID];
        //    SqlTable curTable = this.FindSqlTable(tableName, sqlInfo);
        //    if (curTable == null)
        //    {
        //        curTable = new SqlTable(tableName, tableName, tableName);
        //        this.RegistSqlTable(tableName, curTable, sqlInfo);
        //    }

        //    var data = context.DataContext.GetCurrentDataContextItem(curNode.ID);
        //    var primaryKey = new SqlPrimaryKey();
        //    foreach (var column in curDataObject.PKColumns)
        //    {
        //        var keyField = new SqlPrimaryKeyField(curTable, column.ColumnName);
        //        var pkElement = curNode.Elements.FirstOrDefault(i => i.DataColumnID == column.ID);
        //        keyField.Value.Value = data.PrimaryKeyData[pkElement.ID];
        //        keyField.IsUseFieldPrefix = true;
        //        primaryKey.ChildCollection.Add(keyField);
        //    }

        //    return primaryKey.ToSQL();
        //}


        //var conditionItem = new FilterConditionStatement();
        //var tableName = context.DataObjectTableMapping[curDataObject.ID];

        //var data = context.DataContext.GetCurrentDataContextItem(domainObject.ID);
        //foreach (var pkdata in data.PrimaryKeyData)
        //{
        //    var pkElement = domainObject.Elements.FirstOrDefault(i => i.ID == pkdata.Key);
        //    var pkColumn = domainObject.DataObject.PKColumns.FirstOrDefault(c => c.ID == pkElement.DataColumnID);
        //    bool isTextType = DataTypeUtils.IsTextType(pkColumn.DataObjectID);

        //    var key = new SqlPrimaryKeyField();


        //    if (isTextType)
        //    {
        //        var keyCondition = new KeyValueConditionStatement<string>();
        //        keyCondition.Field.FieldName = pkColumn.ColumnName;

        //        var sqlTable = this.TryFindAndRegistSqlTable(tableName, tableName, tableName, tableName, sqlInfo);
        //        keyCondition.Field.IsUseFieldPrefix = true;
        //        keyCondition.Field.Table = sqlTable;

        //        keyCondition.Value = Convert.ToString(pkdata.Value);
        //        keyCondition.RelationOperator = OperatorType.And;
        //        conditionItem.ChildCollection.Add(keyCondition);
        //    }
        //    else
        //    {
        //        var keyCondition = new KeyValueConditionStatement<long>();
        //        keyCondition.Field.FieldName = pkColumn.ColumnName;

        //        var sqlTable = this.TryFindAndRegistSqlTable(tableName, tableName, tableName, tableName, sqlInfo);
        //        keyCondition.Field.IsUseFieldPrefix = true;
        //        keyCondition.Field.Table = sqlTable;

        //        keyCondition.Value = Convert.ToInt64(pkdata.Value);
        //        keyCondition.RelationOperator = OperatorType.And;
        //        conditionItem.ChildCollection.Add(keyCondition);
        //    }

        //}

        //return conditionItem;

        /// <summary>
        /// 获取主键过滤条件。
        /// </summary>
        /// <param name="context">SQL构造上下文信息。</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="curDataObject">数据对象</param>
        /// <param name="sqlInfo">SQL拼装的中间变量。</param>
        /// <returns>主键的过滤条件。</returns>
        private SqlPrimaryKey GetPrimaryKeyCondition(SqlBuildingContext context, DomainObject domainObject, DataObject curDataObject, SqlBuildingInfo sqlInfo)
        {
            var tableName = context.DataObjectTableMapping[curDataObject.ID];
            SqlTable curTable = this.FindSqlTable(tableName, sqlInfo);
            if (curTable == null)
            {
                curTable = new SqlTable(tableName, tableName, tableName);
                this.RegistSqlTable(tableName, curTable, sqlInfo);
            }

            var data = context.DataContext.GetCurrentDataContextItem(domainObject.ID);
            if (data.PrimaryKeyData.Count == 0) return null;

            var primaryKey = new SqlPrimaryKey();
            foreach (var column in curDataObject.PKColumns)
            {
                var keyField = new SqlPrimaryKeyField(curTable, column.ColumnName);
                var pkElement = domainObject.Elements.FirstOrDefault(i => i.DataColumnID == column.ID);
                keyField.Value.Value = data.PrimaryKeyData[pkElement.ID];
                keyField.IsUseFieldPrefix = true;
                primaryKey.ChildCollection.Add(keyField);
            }

            return primaryKey;

        }

        /// <summary>
        /// 构造主键的过滤条件。
        /// </summary>
        /// <param name="context">SQL构造上下文信息。</param>
        /// <param name="sqlInfo">SQL拼装的中间变量。</param>
        /// <param name="currentObject">主键过滤条件对应的领域对象</param>        
        /// <param name="pkColumnData">主键值Dictionary，Key是ElemetID，Value是列的值。</param>
        /// <param name="IsUseFieldPrefix">是否使用列全名。</param>
        /// <returns>主键过滤条件</returns>
        protected SqlPrimaryKey GetPrimaryKeyConditions(SqlBuildingContext context, SqlBuildingInfo sqlInfo,
            DomainObject currentObject, IDictionary<string, object> data, bool IsUseFieldPrefix = false)
        {
            var curDataObject = currentObject.DataObject;
            var tableName = context.DataObjectTableMapping[curDataObject.ID];
            SqlTable curTable = this.FindSqlTable(tableName, sqlInfo);
            if (curTable == null)
            {
                curTable = new SqlTable(tableName, tableName, tableName);
                this.RegistSqlTable(tableName, curTable, sqlInfo);
            }

            //var data = context.DataContext.GetCurrentDataContextItem(currentObject.ID);
            var primaryKey = new SqlPrimaryKey();
            foreach (var column in curDataObject.PKColumns)
            {
                var keyField = new SqlPrimaryKeyField(curTable, column.ColumnName);
                var pkElement = currentObject.Elements.FirstOrDefault(i => i.DataColumnID == column.ID);
                keyField.Value.Value = data[pkElement.ID];
                keyField.IsUseFieldPrefix = true;
                primaryKey.ChildCollection.Add(keyField);
            }

            return primaryKey;
        }

        /// <summary>
        /// 构造主键的过滤条件。
        /// </summary>
        /// <param name="context">SQL构造上下文信息。</param>
        /// <param name="sqlInfo">SQL拼装的中间变量。</param>
        /// <param name="currentObject">主键过滤条件对应的领域对象</param>        
        /// <param name="pkColumnData">主键值Dictionary，Key是ElemetID，Value是列的值。</param>
        /// <param name="IsUseFieldPrefix">是否使用列全名。</param>
        /// <returns>主键过滤条件</returns>
        protected FilterConditionStatement GetPrimaryKeyConditionsEx(SqlBuildingContext context, SqlBuildingInfo sqlInfo,
            DomainObject currentObject, IDictionary<string, object> pkColumnData, bool IsUseFieldPrefix = false)
        {
            var conditionItem = new FilterConditionStatement();
            foreach (KeyValuePair<string, object> pkdata in pkColumnData)
            {
                var pkElement = currentObject.Elements.FirstOrDefault(i => i.ID == pkdata.Key);
                var pkColumn = currentObject.DataObject.PKColumns.FirstOrDefault(c => c.ID == pkElement.DataColumnID);

                var tableName = context.GetTableName(currentObject.DataObject.ID);
                bool isTextType = DataTypeUtils.IsTextType(pkColumn.DataObjectID);
                if (isTextType)
                {
                    var keyCondition = new KeyValueConditionStatement<string>();
                    keyCondition.Field.FieldName = pkColumn.ColumnName;
                    if (IsUseFieldPrefix)
                    {
                        var sqlTable = this.TryFindAndRegistSqlTable(tableName, tableName, tableName, tableName, sqlInfo);
                        keyCondition.Field.IsUseFieldPrefix = IsUseFieldPrefix;
                        keyCondition.Field.Table = sqlTable;
                    }
                    keyCondition.Value = Convert.ToString(pkdata.Value);
                    keyCondition.LogicalOperator = OperatorType.And;
                    conditionItem.ChildCollection.Add(keyCondition);
                }
                else
                {
                    var keyCondition = new KeyValueConditionStatement<long>();
                    keyCondition.Field.FieldName = pkColumn.ColumnName;
                    if (IsUseFieldPrefix)
                    {
                        var sqlTable = this.TryFindAndRegistSqlTable(tableName, tableName, tableName, tableName, sqlInfo);
                        keyCondition.Field.IsUseFieldPrefix = IsUseFieldPrefix;
                        keyCondition.Field.Table = sqlTable;
                    }
                    keyCondition.Value = Convert.ToInt64(pkdata.Value);
                    keyCondition.LogicalOperator = OperatorType.And;
                    conditionItem.ChildCollection.Add(keyCondition);
                }
            }

            return conditionItem;
        }

        /// <summary>
        /// 解析权限过滤条件
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="context"></param>
        internal void ParseSecurityCondition(SelectSqlStatement sql, SqlBuildingContext context)
        {

        }

        internal string ParseOrderByCondition(string orderByCondition, SqlBuildingInfo sqlBuildingInfo)
        {
            return orderByCondition;
        }

        internal FilterConditionStatement ParseFilterCondition(SqlBuildingContext context)
        {
            return ConditionStatementParser.ParseFiletrClauses(context.QueryFilter.FilterClauses,
                context.Node, context.DataObject);
        }

        internal ConditionStatement ParseOrderByCondition(SqlBuildingContext context)
        {
            return ConditionStatementParser.ParseOrderByClauses(context.QueryFilter.OrderByCondition,
               context.Node, context.DataObject);
        }
    }
}
