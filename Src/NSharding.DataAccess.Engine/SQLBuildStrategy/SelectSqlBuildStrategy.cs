using NSharding.DomainModel.Spi;
using NSharding.Sharding.Database;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 领域对象查询SQL语句构造策略
    /// </summary>
    class SelectSqlBuildStrategy : BaseSqlBuildStrategy
    {
        /// <summary>
        /// 构造不包含数据的Sql（即SqlSchema）。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        public override SqlStatementCollection BuildTableSqlSchema(SqlBuildingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("SelectSqlBuildStrategy.BuildTableSqlSchema");

            var sqls = new SqlStatementCollection();

            var selectStatement = SQLStatementFactory.CreateSqlStatement(SqlStatementType.Select, context.DbType) as SelectSqlStatement;
            selectStatement.SqlBuildingInfo.DataSource = context.DataSource;

            //Parsing main part of query SQL statement
            BuildMainFrom(selectStatement, context.CommonObject, context.Node, context.DataObject, context.TableName, context);

            //Parsing query fields list
            BuildQueryFieldList(selectStatement, context.Node, context.DataObject);

            //Parsing inner join clause
            BuildMainInnerJoin(selectStatement, context);

            sqls.Add(selectStatement);
            sqls.ShardingInfo = context.RouteInfo;

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

            var querySQL = sql as SelectSqlStatement;

            //Parsing left join clause of query SQL statement
            BuildSQLJoin(querySQL, context);

            //Parsing where clause of query SQL statement
            BuildSQLCondition(querySQL, context);
        }

        public void BuildSQLJoin(SelectSqlStatement sql, SqlBuildingContext context)
        {
            //获取到当前节点的关联关系，排除主从类型的关联
            var associations = context.Node.Associations.Where(i => i.AssociateType == AssociateType.OuterLeftJoin);
            if (associations == null || associations.Count() == 0) return;

            foreach (var association in associations)
            {
                HandlingAssociation(sql, context, association);
            }
        }

        /// <summary>
        /// 处理关联
        /// </summary>
        /// <param name="sql">Select语句</param>
        /// <param name="context">SQL构造上下文</param>
        /// <param name="association">关联</param>
        private void HandlingAssociation(SelectSqlStatement sql, SqlBuildingContext context, Association association)
        {
            if (association.AssoDomainObject == null)
                throw new Exception("Association cannot find Associate Model: " + association.ID);
            if (association.AssoDomainObject == null)
                throw new Exception("Association cannot find Associate ModelObject: " + association.ID);
            if (association.AssoDomainObject.DataObject == null)
                throw new Exception("Association cannot find Associate DataObject: " + association.ID);

            //关联中使用的SqlTable,通过关联的ID进行标识，存在一个节点上的多个元素关联同一个表的情况
            SqlTable associatedSqlTable = base.FindSqlTable(association.ID, sql.SqlBuildingInfo);
            if (associatedSqlTable == null)
            {
                var assoTableName = context.DataObjectTableMapping[association.AssoDomainObject.DataObject.ID];
                associatedSqlTable = base.TryFindAndRegistSqlTable(association.AssoDomainObject.DataObject.ID, assoTableName, assoTableName, assoTableName, sql.SqlBuildingInfo);
            }

            var childAssociation = new InternalAssociation()
            {
                AssociatedCommonObject = association.AssoDomaiModel,
                AssociatedCoNode = association.AssoDomainObject,
                AssociatedDataObject = association.AssoDomainObject.DataObject,
                AssociatedTable = associatedSqlTable,
                LocatedCommonObject = sql.SqlBuildingInfo.CommonObject,
                LocatedDataObject = sql.SqlBuildingInfo.CurrentDataObject,
                LocatedNode = sql.SqlBuildingInfo.CurrentNode,
                LocatedTable = sql.SqlBuildingInfo.CurrentSqlTable,
                Association = association,
                AdditionalCondition = association.FilterCondition
            };

            foreach (var refElement in association.RefElements)
            {
                var element = association.AssoDomainObject.Elements.FirstOrDefault(i => i.ID == refElement.ElementID);
                var assElement = new InternalRefElement()
                {
                    Element = element,
                    Label = element.Alias
                };
                childAssociation.RefElements.Add(assElement);
            }

            BuildLeftJoin(sql, childAssociation, context);
        }

        #region 构造和主对象之间的关联

        /// <summary>
        /// 构造查询SQL语句中表主要的内连接部分
        /// </summary>
        /// <remarks>
        /// 处理时分为几种场景：
        /// 1. 当前节点是根节点时，不做任何处理
        /// 2. 当前节点不是根节点时，此时需要和父节点进行内连接关联过滤数据（例如主从结构），
        ///    如果关联的父节点不是根节点，上级也存在父节点(例如主从从结构)，此时要进行递归循环处理
        /// 提示：
        ///    所有内连接的场景均是在同一个领域模型情形下，不可能关联到其他CO的节点。
        /// </remarks>
        /// <param name="sql">Sql语句对象</param>
        /// <param name="domainModel">通用中间对象</param>
        /// <param name="currentObject">当前解析用到的节点对象</param>
        /// <param name="dataObject">当前解析用到的节点对象对应的数据对象</param>
        public void BuildMainInnerJoin(SelectSqlStatement sql, SqlBuildingContext context)
        {
            var currentObject = context.Node;
            var dataObject = context.DataObject;
            //当前节点是根节点时, 直接返回
            if (currentObject.IsRootObject || context.CommonObject.RootDomainObject.ID == currentObject.ID) return;

            //获取到当前节点的主从关联,主从关系中的主从关联关系是存放在子节点的关联集合上
            var associations = currentObject.Associations.Where(i => i.AssociateType == AssociateType.InnerJoin);
            if (associations == null || associations.Count() == 0) return;

            var currentTableName = context.DataObjectTableMapping[dataObject.ID];
            SqlTable cTable = base.FindSqlTable(currentTableName, sql.SqlBuildingInfo);
            if (cTable == null)
            {
                cTable = base.TryFindAndRegistSqlTable(currentTableName, currentTableName, currentTableName, currentTableName, sql.SqlBuildingInfo);
            }

            string parentDataObjectID = currentObject.ParentObject.DataObjectID;
            var parentTableName = context.DataObjectTableMapping[parentDataObjectID];
            SqlTable pTable = base.FindSqlTable(parentTableName, sql.SqlBuildingInfo);
            if (pTable == null)
            {
                pTable = base.TryFindAndRegistSqlTable(parentTableName, parentTableName, parentTableName, parentTableName, sql.SqlBuildingInfo);
            }

            var innerJoin = new InnerJoinItem();
            innerJoin.InnerJoinTable = pTable;

            foreach (Association association in associations)
            {
                foreach (var item in association.Items)
                {
                    var srcElement = currentObject.Elements.FirstOrDefault(i => i.ID == item.SourceElementID);
                    var srcColumn = dataObject.Columns.FirstOrDefault(i => i.ID == srcElement.DataColumnID);
                    if (srcColumn == null)
                        throw new Exception("Cannot find inner join column:" + srcElement.DataColumnID);

                    var targetElement = currentObject.ParentObject.Elements.FirstOrDefault(i => i.ID == item.TargetElementID);
                    var targetColumn = currentObject.ParentObject.DataObject.Columns.FirstOrDefault(i => i.ID == targetElement.DataColumnID);
                    if (srcColumn == null)
                        throw new Exception("Cannot find inner join column:" + targetElement.DataColumnID);

                    var joinItem = new JoinConditionItem();
                    joinItem.LeftField = new ConditionField();
                    joinItem.RightField = new ConditionField();

                    joinItem.LeftField.Table = cTable;
                    joinItem.LeftField.IsUseFieldPrefix = true;
                    joinItem.LeftField.FieldName = srcColumn.ColumnName;

                    joinItem.RightField.Table = pTable;
                    joinItem.RightField.IsUseFieldPrefix = true;
                    joinItem.RightField.FieldName = targetColumn.ColumnName;

                    innerJoin.ChildCollection.Add(joinItem);
                }

                sql.MainFromItem.ChildCollection.Add(innerJoin);

                //如果关联的父节点上还有存在父级节点，递归进行处理,直至根节点退出 
                if (association.AssoDomainObject.ParentObject != null)
                {
                    context.Node = association.AssoDomainObject.ParentObject;
                    context.DataObject = association.AssoDomainObject.ParentObject.DataObject;
                    BuildMainInnerJoin(sql, context);
                }
            }
        }

        /// <summary>
        /// 构造查询主体部分
        /// </summary>
        /// <remarks>查询主体是一个子查询，为了支持一个数据对象多个数据库表</remarks>
        /// <param name="sql">Select语句</param>
        /// <param name="domainModel">通用中间对象</param>
        /// <param name="domainObject">当前节点</param>
        /// <param name="dataObject">当前节点对应的数据对象</param>
        private void BuildMainFrom(SelectSqlStatement sql, DomainModel.Spi.DomainModel domainModel, DomainObject domainObject, DataObject dataObject, string tableName, SqlBuildingContext sqlContext)
        {
            //初始化主查询SQL语句

            var context = sqlContext.DataContext;
            sql.NodeID = domainObject.ID;
            sql.CommonObjectID = domainModel.ID;
            sql.NodeVersion = domainModel.Version.ToString();
            sql.CommonObjectVersion = domainModel.Version.ToString();
            sql.TableName = tableName;
            sql.TableCode = tableName;
            sql.SqlBuildingInfo = base.InitSqlBuildingInfo(domainModel, domainObject, dataObject, tableName, sqlContext.DataSource);

            if (context.Data.ContainsKey(domainObject.ID))
            {
                var dataContextItem = context.GetCurrentDataContextItem(domainObject.ID);
                if (dataContextItem.PrimaryKeyData.Count > 0)
                {
                    foreach (var column in dataObject.PKColumns)
                    {
                        var pkField = new SqlPrimaryKeyField(sql.SqlBuildingInfo.CurrentSqlTable, column.ColumnName);
                        var pkElement = domainObject.Elements.FirstOrDefault(i => i.DataColumnID == column.ID);
                        pkField.Value.Value = dataContextItem.PrimaryKeyData[pkElement.ID];
                        sql.PrimaryKeys.ChildCollection.Add(pkField);
                    }
                }
            }

            //将子查询加入到当前Select语句中
            sql.From.ChildCollection.Add(sql.SqlBuildingInfo.CurrentSqlTable);
        }

        /// <summary>
        /// 构造处理Select语句中查询字段部分
        /// </summary>
        /// <param name="sql">Select语句</param>
        /// <param name="co">通用中间对象</param>
        /// <param name="domainObject">当前节点对象</param>
        /// <param name="dataObject">当前节点对应的数据对象</param>
        public void BuildQueryFieldList(SelectSqlStatement sql, DomainObject domainObject, DataObject dataObject)
        {
            var elements = domainObject.Elements.Where(i => i.ElementType != ElementType.Virtual || i.ElementType != ElementType.Reference);

            foreach (var element in elements)
            {
                if (string.IsNullOrEmpty(element.DataColumnID)) continue;
                var column = dataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID);
                if (column == null)
                    throw new Exception("");

                var field = new SelectListField()
                {
                    Table = sql.SqlBuildingInfo.CurrentSqlTable,
                    IsUseAlias = true,
                    IsUseFieldPrefix = true,
                    FieldName = column.ColumnName,
                    FieldAlias = element.Alias
                };
                sql.SelectList.ChildCollection.Add(field);
            }

            //关联过来的字段不需要在此处理，在关联中处理   
        }

        #endregion

        /// <summary>
        /// 构造左连接SQL
        /// </summary>
        /// <param name="sql">Select语句</param>
        /// <param name="association">关联</param>
        private void BuildLeftJoin(SelectSqlStatement sql, InternalAssociation association, SqlBuildingContext context)
        {
            LeftJoinItem leftJoin = new LeftJoinItem();
            leftJoin.LeftJoinTable = association.LocatedTable;
            foreach (var item in association.Association.Items)
            {
                SqlTable srcElelemtTable = association.LocatedTable;
                var srcElement = association.LocatedNode.Elements.FirstOrDefault(i => i.ID == item.SourceElementID);
                var targetElement = association.AssociatedCoNode.Elements.FirstOrDefault(i => i.ID == item.TargetElementID);


                var leftCol = association.LocatedDataObject.Columns.FirstOrDefault(i => i.ID == srcElement.DataColumnID);
                var rightCol = association.AssociatedDataObject.Columns.FirstOrDefault(i => i.ID == targetElement.DataColumnID);

                JoinConditionItem joinItem = new JoinConditionItem();
                joinItem.LeftField.Table = srcElelemtTable;
                joinItem.LeftField.FieldName = leftCol.ColumnName;
                joinItem.LeftField.IsUseFieldPrefix = true;
                joinItem.RightField.Table = association.AssociatedTable;
                joinItem.RightField.FieldName = rightCol.ColumnName;
                joinItem.RightField.IsUseFieldPrefix = true;
                leftJoin.ChildCollection.Add(joinItem);
            }

            //关联上的条件
            leftJoin.AdditionalCondition = this.ParseCondition(association);

            sql.MainFromItem.ChildCollection.Add(leftJoin);
        }

        private string ParseCondition(InternalAssociation association)
        {
            return association.Association.FilterCondition;
        }

        /// <summary>
        /// 构造Select语句中过滤条件
        /// </summary>  
        /// <remarks>
        /// 根节点和子节点分开处理
        /// </remarks>
        /// <param name="sql">Sql语句对象</param>       
        /// <param name="context">SQL构造上下文</param>
        public void BuildSQLCondition(SelectSqlStatement sql, SqlBuildingContext context)
        {
            //根节点情况
            if (sql.SqlBuildingInfo.CurrentNode.ID == sql.SqlBuildingInfo.CommonObject.RootDomainObjectID ||
                sql.SqlBuildingInfo.CurrentNode.IsRootObject)
            {
                // 解析授权访问控制的约束条件:主表的查询支持权限关联条件。
                this.ParseSecurityCondition(sql, context);

                // 处理过滤条件中的引用元素
                //this.HandlingFilterConditionRefElement(sql, context.FilterCondition, context);

                // 解析外部传入的过滤条件（表单、帮助、报表等）,注:只处理主对象上的过滤条件。         
                base.GetSelectSqlCondition(sql, context, sql.SqlBuildingInfo.RootNode, sql.SqlBuildingInfo.RootDataObject);

                // 解析过滤条件
                var fiterCondition = base.ParseFilterCondition(context);
                if (fiterCondition != null)
                {
                    sql.FilterCondition.ChildCollection.Add(fiterCondition);
                }

                // 解析排序条件
                sql.OrderByCondition = base.ParseOrderByCondition(context);

                // 获取前多少条数据。
                if (context.QueryFilter != null && context.QueryFilter.LimitCount > 0)
                {
                    sql.TopSize = context.QueryFilter.LimitCount;
                }
            }
            else // 子节点情况
            {
                var mainDo = context.CommonObject.RootDomainObject.DataObject;

                // 处理过滤条件中的引用元素
                //this.HandlingFilterConditionRefElement(sql, context.FilterCondition, context);

                // 解析外部传入的过滤条件（表单、帮助、报表等）,注:只处理主对象上的过滤条件。         
                //var filterCondition = base.GeInputCondition(context, sql.SqlBuildingInfo.RootNode,
                //       sql.SqlBuildingInfo.RootDataObject, sql.SqlBuildingInfo);

                //// 解析模型上的过滤条件
                //if (filterCondition != null)
                //    sql.FilterCondition.ChildCollection.Add(filterCondition);
                if (context.DataContext.MainDomainObject.IsRootObject)
                    base.GetSelectSqlCondition(sql, context, sql.SqlBuildingInfo.RootNode, sql.SqlBuildingInfo.RootDataObject);
                else
                    base.GetSelectSqlCondition(sql, context, sql.SqlBuildingInfo.CurrentNode, sql.SqlBuildingInfo.CurrentDataObject);

                // 解析排序条件
                sql.OrderByCondition.ConditionString =
                                     base.ParseOrderByCondition(context.OrderByCondition, sql.SqlBuildingInfo);
            }

            // 分页设置
            if (context.QueryFilter != null && context.QueryFilter.PageParameter != null)
            {
                sql.PageCount = context.QueryFilter.PageParameter.PageSize;
                sql.PageIndex = context.QueryFilter.PageParameter.CurrentPageIndex;
            }
        }
    }
}
