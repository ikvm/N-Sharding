using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 领域对象UpdateSQL构造策略
    /// </summary>
    class UpdateSqlBuildStrategy : BaseSqlBuildStrategy
    {
        /// <summary>
        /// 构造不包含数据的Sql（即SqlSchema）。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public override SqlStatementCollection BuildTableSqlSchema(SqlBuildingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("UpdateSqlBuildStrategy.BuildTableSqlSchema");

            var sqls = new SqlStatementCollection();

            var updateStatement = SQLStatementFactory.CreateSqlStatement(SqlStatementType.Update, context.DbType) as UpdateSqlStatement;
            updateStatement.SqlBuildingInfo.DataSource = context.DataSource;

            base.HandlingSqlStatement(updateStatement, context);
            sqls.Add(updateStatement);

            return sqls;
        }

        /// <summary>
        /// 在SqlSchema基础上，构造包含数据的Sql。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public override void BuildTableSqlDetail(SqlStatementCollection sqls, SqlBuildingContext context)
        {
            var sql = sqls.FirstOrDefault(x => x.SqlBuildingInfo.TableName == context.TableName);
            if (sql == null) return;

            var updateSql = sql as UpdateSqlStatement;

            this.HandlingUpdateFields(updateSql, context);
            this.HandlingConditionInfoAddData(updateSql, context);
        }

        /// <summary>
        /// 设置UpdateSql的更新数据列。
        /// </summary>
        /// <param name="updateSql">UpdateSql对象。</param>
        /// <param name="context">Sql构造上下文信息。</param>
        protected virtual void HandlingUpdateFields(UpdateSqlStatement updateSql, SqlBuildingContext context)
        {
            var dataContext = context.DataContext.GetCurrentDataContextItem(context.Node.ID);

            if (dataContext == null || dataContext.Data == null || dataContext.Data.Count == 0)
            {
                updateSql = null;
                return;
            }
            if (updateSql.SqlBuildingInfo.CurrentSqlTable == null)
            {
                var tableName = context.TableName;
                updateSql.SqlBuildingInfo.CurrentSqlTable =
                    base.TryFindAndRegistSqlTable(tableName, tableName, tableName, tableName, updateSql.SqlBuildingInfo);
            }

            foreach (string dataItem in dataContext.Data.Keys)
            {
                var updateElement = context.Node.Elements.FirstOrDefault(i => i.ID == dataItem);
                var updateColumn = context.DataObject.Columns.FirstOrDefault(i => i.ID == updateElement.DataColumnID);

                //字段设置
                var field = new UpdateField();
                field.IsUseFieldPrefix = false;
                field.IsUseVarBinding = true;
                field.Table = updateSql.SqlBuildingInfo.CurrentSqlTable;
                field.FieldName = updateColumn.ColumnName;
                updateSql.UpdateFields.ChildCollection.Add(field);

                //字段值设置
                var fieldValue = new UpdateValue();
                fieldValue.Value = dataContext.Data[dataItem];
                updateSql.UpdateValues.ChildCollection.Add(fieldValue);
            }
        }

        /// <summary>
        /// 构造UpdateSql的过滤条件。
        /// </summary>       
        /// <param name="sql">Sql语句对象。</param>
        /// <param name="context">Sql构造上下文信息。</param>
        private void HandlingConditionInfoAddData(UpdateSqlStatement sql, SqlBuildingContext context)
        {
            base.GetUpdateSqlCondition(sql, context, context.Node, context.DataObject);
        }
    }
}
