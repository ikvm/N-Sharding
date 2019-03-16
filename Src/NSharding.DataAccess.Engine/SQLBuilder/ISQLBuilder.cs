using NSharding.DataAccess.Spi;
using NSharding.Sharding.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL语句构造器接口
    /// </summary>
    /// <remarks>SQL语句构造器接口</remarks>
    public interface ISQLBuilder
    {
        #region 方法

        /// <summary>
        /// 解析生成Insert语句。
        /// </summary>       
        /// <param name="domainModel">领域模型。</param>
        /// <param name="instance">要插入的数据。</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>Insert语句集合。</returns>
        SqlStatementCollection ParseInsertSql(DomainModel.Spi.DomainModel domainModel, object instance, ShardingValue shardingKeyValue = null);

        /// <summary>
        /// 解析生成指定领域对象的Insert语句。
        /// </summary>
        /// <param name="domainModel">通用中间对象</param>
        /// <param name="domainObject">节点对象</param>
        /// <param name="instance">要插入的数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>指定领域对象的Insert语句</returns>
        SqlStatementCollection ParseInsertSql(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, object instance, ShardingValue shardingKeyValue = null);

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>查询SQL</returns>
        SqlStatementCollection ParseQuerySqlByID(DomainModel.Spi.DomainModel domainModel, string dataID, ShardingValue shardingKeyValue = null);

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>            
        /// <param name="domainObject">领域对象</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>查询SQL</returns>
        SqlStatementCollection ParseQuerySqlByID(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, string dataID, ShardingValue shardingKeyValue = null);

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="filter">过滤器</param>
        /// <returns>查询SQL</returns>
        SqlStatementCollection ParseQuerySqlByFilter(DomainModel.Spi.DomainModel domainModel, QueryFilter filter);

        /// <summary>
        /// 解析生成查询SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>            
        /// <param name="domainObject">领域对象</param>               
        /// <param name="filter">过滤器</param>
        /// <returns>查询SQL</returns>
        SqlStatementCollection ParseQuerySqlByFilter(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, QueryFilter filter);

        /// <summary>
        /// 解析生成删除SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param>        
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        SqlStatementCollection ParseDeleteSqlByID(DomainModel.Spi.DomainModel domainModel, string dataID, ShardingValue shardingKeyValue = null);

        /// <summary>
        /// 解析生成删除SQL
        /// </summary>
        /// <remarks>按主键数据作为查询依据</remarks>
        /// <param name="domainModel">领域模型</param> 
        /// <param name="domainObject">领域对象</param>
        /// <param name="dataID">主键数据</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>删除SQL</returns>
        SqlStatementCollection ParseDeleteSqlByID(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject domainObject, string dataID, ShardingValue shardingKeyValue = null);

        /// <summary>
        /// 解析生成Update语句。
        /// </summary>       
        /// <param name="domainModel">领域模型。</param>
        /// <param name="instance">要更新的数据。</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>Update语句集合。</returns>
        SqlStatementCollection ParseUpdateSql(DomainModel.Spi.DomainModel domainModel, object instance, ShardingValue shardingKeyValue = null);

        /// <summary>
        /// 解析生成Update语句。
        /// </summary>       
        /// <param name="domainModel">领域模型。</param>
        /// <param name="instance">要更新的数据。</param>
        /// <param name="shardingKeyValue">分库分表键值对</param>
        /// <returns>Update语句集合。</returns>
        SqlStatementCollection ParseUpdateSql(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainObject momainObject, object instance, ShardingValue shardingKeyValue = null);

        #endregion
    }
}
