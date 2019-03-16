using NSharding.DomainModel.Spi;
using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 删除SQL构造策略
    /// </summary>
    class DeleteSqlBuildStrategy : BaseSqlBuildStrategy
    {
        /// <summary>
        /// 构造不包含数据的Sql（即SqlSchema）。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public override SqlStatementCollection BuildTableSqlSchema(SqlBuildingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("DeleteSqlBuildStrategy.BuildTableSqlSchema");

            var sqls = new SqlStatementCollection();

            //构造DeleteSql
            var sql = SQLStatementFactory.CreateSqlStatement(SqlStatementType.Delete, context.DbType) as DeleteSqlStatement;
            base.HandlingSqlStatement(sql, context);

            //构造DeleteSql的删除过滤条件
            this.HandlingJoin(sql, context);
            sqls.Add(sql);

            return sqls;
        }

        /// <summary>
        /// 在SqlSchema基础上，构造包含数据的Sql。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public override void BuildTableSqlDetail(SqlStatementCollection sqls, SqlBuildingContext context)
        {
            var sql = sqls.FirstOrDefault(i => i.SqlBuildingInfo.CurrentNode.ID == context.Node.ID);
            if (sql == null)
                throw new Exception("DomainObject cannot find SqlSchema, DomainObjectID: " + context.Node.ID);

            var deleteStatement = sql as DeleteSqlStatement;
            this.HandlingCondition(deleteStatement, context);
        }


        /// <summary>
        /// 形成包含数据的SQL语句过滤条件。
        /// </summary>
        /// <param name="sql">构造过滤条件的DeleteSql。</param>
        /// <param name="context">SQL构造的上下文信息。</param>
        protected void HandlingCondition(DeleteSqlStatement sql, SqlBuildingContext context)
        {
            var sqlInfo = sql.SqlBuildingInfo;
            var currentPrimaryKeyData = context.DataContext.GetCurrentPrimaryKeyData();

            //当前是从节点
            if (!sqlInfo.CurrentNode.IsRootObject || sqlInfo.CommonObject.RootDomainObjectID != sqlInfo.CurrentNode.ID)
            {
                DomainObject currentObject = null;

                //判断下context.DataContext.PrimaryKeyData数据来自于哪个节点
                DataObject currentDataObject = null;

                foreach (KeyValuePair<string, object> data in currentPrimaryKeyData)
                {
                    foreach (var modelObject in sqlInfo.CommonObject.DomainObjects)
                    {
                        var element = modelObject.Elements.FirstOrDefault(i => i.ID == data.Key);
                        if (element != null)
                        {
                            currentObject = modelObject;
                            currentDataObject = modelObject.DataObject;
                            break;
                        }
                    }
                }

                if (currentObject == null || currentDataObject == null)
                {
                    var e = new Exception("DomainModel Error PrimaryKeyData! DomainModelID" + sqlInfo.CommonObject.ID);
                    if (currentPrimaryKeyData != null)
                    {
                        foreach (var data in currentPrimaryKeyData)
                        {
                            e.Data.Add(data.Key, data.Value);
                        }
                    }

                    throw e;
                }

                //添加根节点的主键值条件
                sql.SubQuerySql.Condition.ChildCollection.Add(base.GetPrimaryKeyConditions(context,
                                                            sql.SqlBuildingInfo,
                                                            currentObject,
                                                            currentPrimaryKeyData, true));


            }
            else
            {
                // 生成SqlSchema时，形成了子查询。但使用时context.IsUsePrimaryCondition== true，则将子查询置空。
                sql.SubQuerySql = null;
                context.IsUsePrimaryCondition = true;
                sql.Conditions.ChildCollection.Add(base.GetPrimaryKeyConditions(context,
                                            sql.SqlBuildingInfo,
                                            sql.SqlBuildingInfo.CurrentNode, currentPrimaryKeyData));
            }
        }

        /// <summary>
        ///  根据SQL构造上下文信息，构造DeleteSql的过滤条件。
        /// </summary>
        /// <param name="context">SQL构造的上下文信息。</param>
        /// <param name="sql">构造过滤条件的DeleteSql。</param>
        protected void HandlingJoin(DeleteSqlStatement sql, SqlBuildingContext context)
        {
            var currentObject = sql.SqlBuildingInfo.CurrentNode;

            //1.如果是根节点
            if (currentObject.IsRootObject || currentObject.ID == sql.SqlBuildingInfo.CommonObject.RootDomainObjectID ||
                context.IsUsePrimaryCondition)
            {
                sql.SubQuerySql = null;
                return;
            }
            //2.如果是从节点或者从从节点
            else
            {
                var sqlInfo = sql.SqlBuildingInfo;
                var querySql = new SelectSqlForSubQuery();
                //获取当前节点的SQLTable
                var currentTableName = context.TableName;
                SqlTable table = base.FindSqlTable(currentTableName, sqlInfo);
                if (table == null)
                {
                    table = new SqlTable(currentTableName, currentTableName, currentTableName);
                    base.RegistSqlTable(currentTableName, table, sql.SqlBuildingInfo);
                }
                //指定为查询的主表
                //querySql.MainFromItem.Table = table;

                //构造和根节点的关联关系
                BuildParentInnerJoin(currentObject, sqlInfo, querySql, context);

                // 构造删除的主表和子查询的之间的关联关系,支持复合主键
                //var rTable = base.FindSqlTable(currentTableName, sql.SqlBuildingInfo);
                //foreach (var pkColumn in context.DataObject.PKColumns)
                //{
                //    querySql.JoinSubQueryConditionItem.LeftField.Table = sql.SqlBuildingInfo.CurrentSqlTable;
                //    querySql.JoinSubQueryConditionItem.RightField.Table = rTable;
                //    querySql.JoinSubQueryConditionItem.LeftField.IsUseFieldPrefix = true;
                //    querySql.JoinSubQueryConditionItem.RightField.IsUseFieldPrefix = true;
                //    querySql.JoinSubQueryConditionItem.LeftField.FieldName = pkColumn.ColumnName;
                //    querySql.JoinSubQueryConditionItem.RightField.FieldName = pkColumn.ColumnName;
                //    //先只循环一次
                //    break;
                //}

                sql.SubQuerySql = querySql;
            }
        }

        /// <summary>
        /// 构造跟主表的关联。
        /// </summary>
        /// <param name="currentObject">当前的数据模型对象。</param>
        /// <param name="sqlBuildInfo">SQL构造的中间变量。</param>
        /// <param name="querySql">删除过滤子查询SQL。</param>
        /// <remarks>
        /// 当前对象若是从对象，则需要建立跟主对象的关联，
        /// 然后根据主对象的唯一标识，形成删除的Sql。
        /// </remarks>
        private void BuildParentInnerJoin(DomainObject currentObject, SqlBuildingInfo sqlBuildInfo, SelectSqlForSubQuery querySql, SqlBuildingContext context)
        {
            //如果是根节点，退出
            if (currentObject.IsRootObject || currentObject.ID == sqlBuildInfo.CommonObject.RootDomainObjectID) return;

            #region 子节点（当前节点）

            var currentDataObject = currentObject.DataObject;
            var tableName = context.GetTableName(currentDataObject.ID);
            SqlTable currentTable = base.FindSqlTable(tableName, sqlBuildInfo);
            if (currentTable == null)
            {
                currentTable = new SqlTable(tableName, tableName, tableName);
                base.RegistSqlTable(tableName, currentTable, sqlBuildInfo);
            }

            #endregion

            #region 父节点

            var parentObjecct = currentObject.ParentObject;
            var parentDataObject = parentObjecct.DataObject;
            var parentTableName = context.GetTableName(parentDataObject.ID);
            SqlTable parentTable = base.FindSqlTable(parentTableName, sqlBuildInfo);
            if (parentTable == null)
            {
                parentTable = new SqlTable(parentTableName, parentTableName, parentTableName);
                base.RegistSqlTable(parentTableName, parentTable, sqlBuildInfo);
            }

            FromItem parentFromItem = new FromItem(parentTable);
            querySql.From.ChildCollection.Add(parentFromItem);

            #endregion

            #region 关联关系

            //目前只是支持单个主对象结构，因此应该只有一个主从关联
            var association = currentObject.Associations.FirstOrDefault(i => i.AssociateType == AssociateType.InnerJoin);
            foreach (var item in association.Items)
            {
                //主从关联中，源节点在子节点中，目标节点在父节点上
                var sourceElement = currentObject.Elements.FirstOrDefault(i => i.ID == item.SourceElementID);
                var targetElement = parentObjecct.Elements.FirstOrDefault(i => i.ID == item.TargetElementID);
                var childCol = currentDataObject.Columns.FirstOrDefault(i => i.ID == sourceElement.ID);
                var parentCol = parentDataObject.Columns.FirstOrDefault(i => i.ID == targetElement.ID);

                JoinConditionItem joinItem = new JoinConditionItem();
                joinItem.LeftField = new ConditionField();
                joinItem.RightField = new ConditionField();


                joinItem.LeftField.Table = currentTable;
                joinItem.LeftField.FieldName = childCol.ColumnName;
                joinItem.LeftField.IsUseFieldPrefix = true;


                joinItem.RightField.Table = parentTable;
                joinItem.RightField.FieldName = parentCol.ColumnName;
                joinItem.RightField.IsUseFieldPrefix = true;

                querySql.JoinCondition.ChildCollection.Add(joinItem);
            }

            #endregion

            BuildParentInnerJoin(parentObjecct, sqlBuildInfo, querySql, context);
        }
    }
}
