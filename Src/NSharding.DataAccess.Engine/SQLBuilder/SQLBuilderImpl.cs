using NSharding.DataAccess.Spi;
using NSharding.Sharding.Rule;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL构造实现类
    /// </summary>
    class SQLBuilderImpl : ISQLBuilder
    {
        private static object syncObj = new object();

        private static ISQLBuilder builder;

        private ShardingRouteService routeService;

        /// <summary>
        /// 构造函数
        /// </summary>
        private SQLBuilderImpl()
        {
            routeService = new ShardingRouteService();
        }

        /// <summary>
        /// 获取SQL构造实现
        /// </summary>
        /// <returns>SQL构造实现</returns>
        public static ISQLBuilder GetInstance()
        {
            if (builder == null)
            {
                lock (syncObj)
                {
                    if (builder == null)
                    {
                        builder = new SQLBuilderImpl();
                    }
                }
            }

            return builder;
        }

        /// <summary>
        /// 解析生成Insert语句。
        /// </summary>       
        /// <param name="domainModel">领域模型。</param>
        /// <param name="instance">要插入的数据。</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>Insert语句集合。</returns>
        public SqlStatementCollection ParseInsertSql(DomainModel.Spi.DomainModel domainModel, object instance, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var routeInfo = routeService.Route(domainModel, instance, shardingKeyValue);
            var dataContext = DataContextBuilder.CreateDataContext<object>(domainModel, domainModel.RootDomainObject, DataAccessOpType.I, instance);

            //解析SQL语句主干接口
            ParseInsertSqlSchema(sqls, domainModel, domainModel.RootDomainObject, routeInfo);

            //在SqlSchema上逐表添加数据
            return ParseInsertSqlDetail(sqls, domainModel, dataContext, routeInfo);
        }

        /// <summary>
        /// 解析生成Insert语句的主干部分。
        /// </summary>
        /// <param name="sqlSchemata">Insert语句的主干。</param>
        /// <param name="co">通用中间对象。</param>
        /// <param name="node">通用中间对象的节点。</param>
        /// <returns>Insert语句的主干部分。</returns>
        private void ParseInsertSqlSchema(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject,
            Dictionary<string, ShardingTarget> routeInfo)
        {
            var insertStrategy = new InsertSqlBuildStrategy();
            var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType);
            context.RouteInfo = routeInfo;

            var nodeSqlSchemata = insertStrategy.BuildTableSqlSchema(context);
            sqlSchemata.AddRange(nodeSqlSchemata);

            //递归处理子对象
            if (context.Node.ChildDomainObjects.Count == 0) return;

            foreach (var childModelObject in context.Node.ChildDomainObjects)
            {
                ParseInsertSqlSchema(sqlSchemata, domainModel, childModelObject, routeInfo);
            }
        }

        /// <summary>
        /// 根据已经解析的SqlSchema，解析生成带数据的Insert语句。
        /// </summary>
        /// <param name="sqlSchemata">已经解析的SqlSchema。</param>
        /// <param name="domainModel">领域对象</param>
        /// <param name="context">数据上下文</param>
        /// <returns>Insert语句集合</returns>
        private SqlStatementCollection ParseInsertSqlDetail(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DataContext context, Dictionary<string, ShardingTarget> routeInfo)
        {
            var result = new SqlStatementCollection();

            foreach (var node in domainModel.DomainObjects)
            {
                var data = context.Data[node.ID];
                if (data == null || data.Count == 0) continue;

                context.ResetCurrentDataIndex();
                var nodeSqls = ParseInsertSqlDetail(sqlSchemata, domainModel, node, context, routeInfo);
                result.AddRange(nodeSqls);
            }

            return result;
        }

        /// <summary>
        /// 根据已经解析的SqlSchema，解析生成带数据的Insert语句。
        /// </summary>
        /// <param name="sqlSchemata">已经解析的SqlSchema。</param>
        /// <param name="domainModel">领域对象</param>
        /// <param name="context">数据上下文</param>
        /// <returns>Insert语句集合</returns>
        private SqlStatementCollection ParseInsertSqlDetail(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject,
            DataContext dataContext, Dictionary<string, ShardingTarget> routeInfo)
        {
            var result = new SqlStatementCollection();
            var data = dataContext.Data[domainObject.ID];

            if (data == null || data.Count == 0) return result;

            var strategy = new InsertSqlBuildStrategy();

            for (int i = 0; i < dataContext.Data[domainObject.ID].Count; i++)
            {
                dataContext.CurrentDataIndex = i;
                var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType, dataContext);
                context.RouteInfo = routeInfo;

                var rowSqls = new SqlStatementCollection();
                var sqls = sqlSchemata.Where(s => s.SqlBuildingInfo.CurrentNode.ID == domainObject.ID).ToList();
                rowSqls.AddRangeClone(sqls);

                strategy.BuildTableSqlDetail(rowSqls, context);
                result.AddRange(rowSqls);
            }

            return result;
        }

        /// <summary>
        /// 解析生成指定领域对象的Insert语句。
        /// </summary>
        /// <param name="domainModel">通用中间对象</param>
        /// <param name="domainObject">节点对象</param>
        /// <param name="instance">要插入的数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>指定领域对象的Insert语句</returns>
        public SqlStatementCollection ParseInsertSql(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, object instance, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var routeInfo = routeService.Route(domainModel, domainObject, instance, shardingKeyValue);
            var dataContext = DataContextBuilder.CreateDataContext<object>(domainModel, domainObject, DataAccessOpType.I, instance);

            //解析SQL语句主干接口
            ParseInsertSqlSchema(sqls, domainModel, domainObject, routeInfo);

            //在SqlSchema上逐表添加数据
            return ParseInsertSqlDetail(sqls, domainModel, domainObject, dataContext, routeInfo);
        }

        /// <summary>
        /// 解析生成Update语句。
        /// </summary>       
        /// <param name="domainModel">领域模型。</param>
        /// <param name="instance">要更新的数据。</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>Update语句集合。</returns>
        public SqlStatementCollection ParseUpdateSql(DomainModel.Spi.DomainModel domainModel, object instance, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var routeInfo = routeService.Route(domainModel, instance, shardingKeyValue);
            var dataContext = DataContextBuilder.CreateDataContext<object>(domainModel, domainModel.RootDomainObject, DataAccessOpType.U, instance);

            //解析SQL语句主干接口
            ParseUpdateSqlSchema(sqls, domainModel, domainModel.RootDomainObject, routeInfo, dataContext);

            //在SqlSchema上逐表添加数据
            return ParseUpdateSqlDetail(sqls, domainModel, dataContext, routeInfo);
        }

        /// <summary>
        /// 解析生成Update语句。
        /// </summary>       
        /// <param name="domainModel">领域模型。</param>
        /// <param name="instance">要更新的数据。</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>Update语句集合。</returns>
        public SqlStatementCollection ParseUpdateSql(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject momainObject, object instance, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var routeInfo = routeService.Route(domainModel, instance, shardingKeyValue);
            var dataContext = DataContextBuilder.CreateDataContext<object>(domainModel, momainObject, DataAccessOpType.U, instance);

            //解析SQL语句主干接口
            ParseUpdateSqlSchema(sqls, domainModel, momainObject, routeInfo, dataContext);

            //在SqlSchema上逐表添加数据
            return ParseUpdateSqlDetail(sqls, domainModel, dataContext, routeInfo);
        }

        /// <summary>
        /// 解析生成Insert语句的主干部分。
        /// </summary>
        /// <param name="sqlSchemata">Insert语句的主干。</param>
        /// <param name="co">通用中间对象。</param>
        /// <param name="node">通用中间对象的节点。</param>
        /// <returns>Insert语句的主干部分。</returns>
        private void ParseUpdateSqlSchema(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject,
            Dictionary<string, ShardingTarget> routeInfo, DataContext dataContext)
        {
            var updateStrategy = new UpdateSqlBuildStrategy();
            var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType);
            context.RouteInfo = routeInfo;
            if (dataContext.GetCurrentDataContextItem(domainObject.ID) != null)
            {
                var nodeSqlSchemata = updateStrategy.BuildTableSqlSchema(context);
                sqlSchemata.AddRange(nodeSqlSchemata);
            }

            //递归处理子对象
            if (context.Node.ChildDomainObjects.Count == 0) return;

            foreach (var childModelObject in context.Node.ChildDomainObjects)
            {
                ParseUpdateSqlSchema(sqlSchemata, domainModel, childModelObject, routeInfo, dataContext);
            }
        }

        /// <summary>
        /// 根据已经解析的SqlSchema，解析生成带数据的Update语句。
        /// </summary>
        /// <param name="sqlSchemata">已经解析的SqlSchema。</param>
        /// <param name="domainModel">领域对象</param>
        /// <param name="context">数据上下文</param>
        /// <returns>Insert语句集合</returns>
        private SqlStatementCollection ParseUpdateSqlDetail(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DataContext context, Dictionary<string, ShardingTarget> routeInfo)
        {
            var result = new SqlStatementCollection();

            foreach (var node in domainModel.DomainObjects)
            {
                var data = context.Data[node.ID];
                if (data == null || data.Count == 0) continue;

                context.ResetCurrentDataIndex();
                var nodeSqls = ParseUpdateSqlDetail(sqlSchemata, domainModel, node, context, routeInfo);
                result.AddRange(nodeSqls);
            }

            return result;
        }

        /// <summary>
        /// 根据已经解析的SqlSchema，解析生成带数据的Update语句。
        /// </summary>
        /// <param name="sqlSchemata">已经解析的SqlSchema。</param>
        /// <param name="domainModel">领域对象</param>
        /// <param name="context">数据上下文</param>
        /// <returns>Insert语句集合</returns>
        private SqlStatementCollection ParseUpdateSqlDetail(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject,
            DataContext dataContext, Dictionary<string, ShardingTarget> routeInfo)
        {
            var result = new SqlStatementCollection();
            if (!dataContext.Data.ContainsKey(domainObject.ID))
            {
                return result;
            }
            var data = dataContext.Data[domainObject.ID];

            if (data == null || data.Count == 0) return result;

            var strategy = new UpdateSqlBuildStrategy();

            for (int i = 0; i < dataContext.Data[domainObject.ID].Count; i++)
            {
                dataContext.CurrentDataIndex = i;
                var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType, dataContext);
                context.RouteInfo = routeInfo;

                var rowSqls = new SqlStatementCollection();
                var sqls = sqlSchemata.Where(s => s.SqlBuildingInfo.CurrentNode.ID == domainObject.ID).ToList();
                rowSqls.AddRangeClone(sqls);

                strategy.BuildTableSqlDetail(rowSqls, context);
                result.AddRange(rowSqls);
            }

            return result;
        }

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>查询SQL</returns>
        public SqlStatementCollection ParseQuerySqlByID(DomainModel.Spi.DomainModel domainModel, string dataID, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var sqlSchema = new SqlStatementCollection();

            var routeInfo = routeService.RouteByDataID(domainModel, dataID, shardingKeyValue);
            var dataIDDic = CreatePkDataDictionary(domainModel, domainModel.RootDomainObject, dataID);
            var dataContext = DataContextBuilder.CreateDataContext<IDictionary<string, object>>(domainModel, domainModel.RootDomainObject, DataAccessOpType.Q, dataIDDic);

            //解析SQL语句主干接口
            ParseQuerySqlSchema(sqlSchema, domainModel, domainModel.RootDomainObject, routeInfo, dataContext);

            //在SqlSchema上逐表添加数据
            ParseQuerySqlDetail(sqlSchema, sqls, domainModel, domainModel.RootDomainObject, dataContext, routeInfo);

            return sqls;
        }

        private void ParseQuerySqlDetail(SqlStatementCollection sqlSchemata, SqlStatementCollection result, DomainModel.Spi.DomainModel domainModel,
            DomainObject domainObject, DataContext dataContext, Dictionary<string, ShardingTarget> routeInfo)
        {
            //构造SQL语句的条件
            var strategy = new SelectSqlBuildStrategy();
            var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType, dataContext);
            context.RouteInfo = routeInfo;
            var rowSqls = new SqlStatementCollection();
            var sqls = sqlSchemata.Where(s => s.SqlBuildingInfo.CurrentNode.ID == domainObject.ID).ToList();
            rowSqls.AddRangeClone(sqls);
            strategy.BuildTableSqlDetail(rowSqls, context);
            result.AddRange(rowSqls);

            //递归处理子对象
            if (domainObject.ChildDomainObjects.Count == 0) return;
            foreach (var childModelObject in domainObject.ChildDomainObjects)
            {
                ParseQuerySqlDetail(sqlSchemata, result, domainModel, childModelObject, dataContext, routeInfo);
            }
        }

        /// <summary>
        /// 构造查询SQL语句的主干结构
        /// </summary>
        /// <param name="dataObject">数据对象</param>
        /// <returns>查询SQL语句的主干结构</returns>
        private void ParseQuerySqlSchema(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, Dictionary<string, ShardingTarget> routeInfo, DataContext dataContext)
        {
            var selectStrategy = new SelectSqlBuildStrategy();
            var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType, dataContext);
            context.RouteInfo = routeInfo;

            var nodeSqlSchemata = selectStrategy.BuildTableSqlSchema(context);
            sqlSchemata.AddRange(nodeSqlSchemata);

            //递归处理子对象
            if (domainObject.ChildDomainObjects.Count == 0) return;

            foreach (var childModelObject in domainObject.ChildDomainObjects)
            {
                ParseQuerySqlSchema(sqlSchemata, domainModel, childModelObject, routeInfo, dataContext);
            }
        }

        private Dictionary<string, object> CreatePkDataDictionary(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, string dataID)
        {
            var dataIDDic = new Dictionary<string, object>();
            foreach (var column in domainObject.DataObject.PKColumns)
            {
                var pkElement = domainObject.Elements.FirstOrDefault(i => i.DataColumnID == column.ID);
                dataIDDic.Add(pkElement.ID, dataID);
                break;
            }
            return dataIDDic;
        }

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>            
        /// <param name="domainObject">领域对象</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>查询SQL</returns>
        public SqlStatementCollection ParseQuerySqlByID(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, string dataID, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var sqlSchema = new SqlStatementCollection();

            var routeInfo = routeService.RouteByDataID(domainModel, dataID, shardingKeyValue);
            var dataIDDic = CreatePkDataDictionary(domainModel, domainObject, dataID);
            var dataContext = DataContextBuilder.CreateDataContext<IDictionary<string, object>>(domainModel, domainObject, DataAccessOpType.Q, dataIDDic);

            //解析SQL语句主干接口
            ParseQuerySqlSchema(sqlSchema, domainModel, domainObject, routeInfo, dataContext);

            //在SqlSchema上逐表添加数据
            ParseQuerySqlDetail(sqlSchema, sqls, domainModel, domainObject, dataContext, routeInfo);

            return sqlSchema;
        }

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="filter">过滤器</param>
        /// <returns>查询SQL</returns>
        public SqlStatementCollection ParseQuerySqlByFilter(DomainModel.Spi.DomainModel domainModel, QueryFilter filter)
        {
            var sqls = new SqlStatementCollection();

            return sqls;
        }

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>            
        /// <param name="domainObject">领域对象</param>               
        /// <param name="filter">过滤器</param>
        /// <returns>查询SQL</returns>
        public SqlStatementCollection ParseQuerySqlByFilter(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, QueryFilter filter)
        {
            var sqls = new SqlStatementCollection();

            return sqls;
        }

        /// <summary>
        /// 解析生成删除SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        public SqlStatementCollection ParseDeleteSqlByID(DomainModel.Spi.DomainModel domainModel, string dataID, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var sqlSchema = new SqlStatementCollection();

            var routeInfo = routeService.RouteByDataID(domainModel, dataID, shardingKeyValue);
            var dataIDDic = CreatePkDataDictionary(domainModel, domainModel.RootDomainObject, dataID);
            var dataContext = DataContextBuilder.CreateDataContext<IDictionary<string, object>>(domainModel, domainModel.RootDomainObject, DataAccessOpType.D, dataIDDic);

            //解析SQL语句主干接口
            ParseDeleteSqlSchema(sqlSchema, domainModel, domainModel.RootDomainObject, routeInfo, dataContext);

            //在SqlSchema上逐表添加数据
            ParseDeleteSqlDetail(sqlSchema, sqls, domainModel, domainModel.RootDomainObject, dataContext, routeInfo);

            var result = new SqlStatementCollection();
            foreach (var domainObject in domainModel.ReverseDomainObjects)
            {
                var sql = sqls.FirstOrDefault(i => i.NodeID == domainObject.ID);
                if (sql != null)
                {
                    result.Add(sql);
                }
            }

            return result;
        }

        /// <summary>
        /// 解析生成删除SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param> 
        /// <param name="domainObject">领域对象</param>
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        public SqlStatementCollection ParseDeleteSqlByID(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, string dataID, ShardingValue shardingKeyValue = null)
        {
            var sqls = new SqlStatementCollection();
            var sqlSchema = new SqlStatementCollection();

            var routeInfo = routeService.RouteByDataID(domainModel, dataID, shardingKeyValue);
            var dataIDDic = CreatePkDataDictionary(domainModel, domainModel.RootDomainObject, dataID);
            var dataContext = DataContextBuilder.CreateDataContext<IDictionary<string, object>>(domainModel, domainObject, DataAccessOpType.D, dataIDDic);

            //解析SQL语句主干接口
            ParseDeleteSqlSchema(sqlSchema, domainModel, domainObject, routeInfo, dataContext);

            //在SqlSchema上逐表添加数据
            ParseDeleteSqlDetail(sqlSchema, sqls, domainModel, domainObject, dataContext, routeInfo);

            var result = new SqlStatementCollection();
            foreach (var tempDomainObject in domainModel.ReverseDomainObjects)
            {
                var sql = sqls.FirstOrDefault(i => i.NodeID == tempDomainObject.ID);
                if (sql != null)
                {
                    result.Add(sql);
                }
            }

            return result;
        }

        /// <summary>
        /// 构造查询SQL语句的主干结构
        /// </summary>
        /// <param name="dataObject">数据对象</param>
        /// <returns>查询SQL语句的主干结构</returns>
        private void ParseDeleteSqlSchema(SqlStatementCollection sqlSchemata, DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, Dictionary<string, ShardingTarget> routeInfo, DataContext dataContext)
        {
            var selectStrategy = new DeleteSqlBuildStrategy();
            var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType, dataContext);
            context.RouteInfo = routeInfo;

            var nodeSqlSchemata = selectStrategy.BuildTableSqlSchema(context);
            sqlSchemata.AddRange(nodeSqlSchemata);

            //递归处理子对象
            if (domainObject.ChildDomainObjects.Count == 0) return;

            foreach (var childModelObject in domainObject.ChildDomainObjects)
            {
                ParseDeleteSqlSchema(sqlSchemata, domainModel, childModelObject, routeInfo, dataContext);
            }
        }

        private void ParseDeleteSqlDetail(SqlStatementCollection sqlSchemata, SqlStatementCollection result, DomainModel.Spi.DomainModel domainModel,
            DomainModel.Spi.DomainObject domainObject, DataContext dataContext, Dictionary<string, ShardingTarget> routeInfo)
        {
            //构造SQL语句的条件
            var strategy = new DeleteSqlBuildStrategy();
            var context = new SqlBuildingContext(domainModel, domainObject, domainObject.DataObject.DataSource.DbType, dataContext);
            context.RouteInfo = routeInfo;
            var rowSqls = new SqlStatementCollection();
            var sqls = sqlSchemata.Where(s => s.SqlBuildingInfo.CurrentNode.ID == domainObject.ID).ToList();
            rowSqls.AddRangeClone(sqls);
            strategy.BuildTableSqlDetail(rowSqls, context);
            result.AddRange(rowSqls);

            //递归处理子对象
            if (domainObject.ChildDomainObjects.Count == 0) return;
            foreach (var childModelObject in domainObject.ChildDomainObjects)
            {
                ParseDeleteSqlDetail(sqlSchemata, result, domainModel, childModelObject, dataContext, routeInfo);
            }
        }
    }
}
