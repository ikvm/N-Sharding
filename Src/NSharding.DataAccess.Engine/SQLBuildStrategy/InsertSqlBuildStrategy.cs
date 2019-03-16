using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 领域对象Insert SQL构造策略
    /// </summary>
    class InsertSqlBuildStrategy : BaseSqlBuildStrategy
    {
        /// <summary>
        /// 构造不包含数据的Sql（即SqlSchema）。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public override SqlStatementCollection BuildTableSqlSchema(SqlBuildingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("InsertSqlBuilder.BuildTableSqlSchema");

            var sqls = new SqlStatementCollection();

            var insertStatement = SQLStatementFactory.CreateSqlStatement(SqlStatementType.Insert, context.DbType) as InsertSqlStatement;
            base.HandlingSqlStatement(insertStatement, context);
            HandlingInsertFields(insertStatement, context);
            insertStatement.SqlBuildingInfo.DataSource = context.DataSource;

            sqls.Add(insertStatement);

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

            var insertStatement = sql as InsertSqlStatement;
            //insertStatement.SqlBuildingInfo = InitSqlBuildingInfo(context.CommonObject, context.Node, context.DataObject, context.TableName);

            this.HandlingFieldsAddData(insertStatement, context);
        }

        /// <summary>
        /// 构造Insert语句中的插入字段
        /// </summary>
        /// <param name="sql">Insert语句</param>
        /// <param name="context">SQL构造上下文</param>
        protected void HandlingInsertFields(InsertSqlStatement sql, SqlBuildingContext context)
        {
            foreach (var col in context.DataObject.Columns)
            {
                sql.InsertFields.ChildCollection.Add(new InsertField()
                {
                    FieldName = col.ColumnName,
                    IsUseVarBinding = true
                });
            }
        }

        /// <summary>
        /// 处理带数据的元素
        /// </summary>
        /// <param name="sql">Insert语句</param>
        /// <param name="context">策略的上下文</param>
        protected void HandlingFieldsAddData(InsertSqlStatement sql, SqlBuildingContext context)
        {
            var excludeField = new List<InsertField>();
            foreach (InsertField insertField in sql.InsertFields.ChildCollection)
            {
                var insertCol = context.DataObject.Columns.FirstOrDefault(i => i.ColumnName == insertField.FieldName);
                if (insertCol == null)
                {
                    throw new Exception("Cannot find column: " + insertField.FieldName);
                }
                var insertElement = context.Node.Elements.FirstOrDefault(i => i.DataColumnID == insertCol.ID);
                if (insertElement == null)
                {
                    throw new Exception("Cannot find DomainObjectElement, DataColumnID: " + insertCol.ID);
                }
                if (!context.DataContext.GetCurrentDataContextItem(context.Node.ID).Data.ContainsKey(insertElement.ID))
                {
                    excludeField.Add(insertField);
                    continue;
                }

                var fieldValue = new InsertValue();

                fieldValue.DataType = Convert.ToInt32(insertElement.DataType);
                var value = context.DataContext.GetCurrentDataContextItem(context.Node.ID).Data[insertElement.ID];
                fieldValue.Value = ElementValueWrapper.ConvertElementValue(insertElement, value);
                sql.InsertValues.ChildCollection.Add(fieldValue);

            }

            if (excludeField != null && excludeField.Count > 0)
            {
                foreach (var field in excludeField)
                {
                    sql.InsertFields.ChildCollection.Remove(field);
                }
            }

            if (sql.InsertFields.ChildCollection.Count == 0)
                throw new Exception(string.Format("数据访问服务-插入的字段为空, DomainModel:{0}, DomainObject:{1} ",
                    context.CommonObject.ID, context.Node.ID));
            if (sql.InsertFields.ChildCollection.Count != sql.InsertValues.ChildCollection.Count)
                throw new Exception(string.Format("数据访问服务-插入的字段与值个数不匹配, DomainModel:{0}, DomainObject:{1} ",
                    context.CommonObject.ID, context.Node.ID));
        }

        ///// <summary>
        ///// 处理通用中间对象的节点的GSPAbsDBTable。
        ///// </summary>
        ///// <param name="sql">Sql语句对象。</param>
        ///// <param name="domainModel">领域模型。</param>
        ///// <param name="domainObject">领域对象。</param>
        ///// <param name="dataObject">领域对象对应的数据对象。</param>        
        ///// <param name="tableName">表名称。</param>
        //protected virtual void HandlingSqlStatement(SqlStatement sql, DomainModel domainModel, DomainObject domainObject, DataObject dataObject, string tableName, string dataSource)
        //{
        //    sql.NodeID = domainObject.ID;
        //    sql.CommonObjectID = domainModel.ID;
        //    sql.NodeVersion = domainModel.Version.ToString();
        //    sql.CommonObjectVersion = domainModel.Version.ToString();
        //    sql.TableName = tableName;

        //    //构造SqlBuildingInfo  
        //    sql.SqlBuildingInfo = this.InitSqlBuildingInfo(domainModel, domainObject, dataObject, tableName, dataSource);

        //    //复合主键                       
        //    foreach (var item in dataObject.PKColumns)
        //    {
        //        var keyField = new SqlPrimaryKeyField();
        //        keyField.FieldName = item.ColumnName;
        //        sql.PrimaryKeys.ChildCollection.Add(keyField);
        //    }
        //}
    }
}
