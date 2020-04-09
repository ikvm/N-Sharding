using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using NSharding.Sharding.Database;
using NSharding.Sharding.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    class SqlBuildingContext
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataObject">数据对象</param>
        public SqlBuildingContext(DataObject dataObject)
        {
            DataObject = dataObject;
            DbType = DbType.SQLServer;
            PrimaryKeyDataCollection = new List<Dictionary<string, object>>();
            this.DbType = dataObject.DataSource.DbType;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataObject">数据对象</param>
        /// <param name="filter">查询过滤器</param>
        public SqlBuildingContext(DataObject dataObject, string filterCondition, string orderByCondition, int pageIndex, int pageSize)
            : this(dataObject)
        {
            this.PageSize = pageSize;
            this.CurrentPageIndex = pageIndex;
            this.FilterCondition = filterCondition;
            this.OrderByCondition = orderByCondition;
            this.DbType = dataObject.DataSource.DbType;
        }

        /// <summary>
        /// 构造函数(Default\Insert\Delete场景)
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="dbType">数据库类型</param>
        public SqlBuildingContext(DomainModel.Spi.DomainModel domainModel, DomainObject domainObject, DbType dbType)
        {
            this.DbType = dbType;
            this.Node = domainObject;
            this.DataObject = domainObject.DataObject;
            this.CommonObject = domainModel;
        }

        /// <summary>
        /// 构造函数(Default\Insert\Delete场景)
        /// </summary>
        /// <param name="domainModel">通用中领域模型间对象</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dataContext">数据上下文</param>
        public SqlBuildingContext(DomainModel.Spi.DomainModel domainModel, DomainObject domainObject, DbType dbType, DataContext dataContext)
            : this(domainModel, domainObject, dbType)
        {
            this.DataContext = dataContext;
            this.QueryFilter = dataContext.QueryFilter;
            this.DbType = domainObject.DataObject.DataSource.DbType;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="domainModel">领域模型</param>
        /// <param name="domainObject">领域对象</param>
        /// <param name="dbType">数据库类型</param>                
        /// <param name="filterCondition">过滤条件</param>
        /// <param name="orderByCondition">排序条件</param>        
        public SqlBuildingContext(DomainModel.Spi.DomainModel domainModel, DomainObject domainObject, DbType dbType, string filterCondition = "", string orderByCondition = "")
            : this(domainModel, domainObject, dbType)
        {
            FilterCondition = filterCondition;
            OrderByCondition = orderByCondition;
            this.DbType = domainObject.DataObject.DataSource.DbType;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 领域模型
        /// </summary>
        public DomainModel.Spi.DomainModel CommonObject { get; set; }

        /// <summary>
        /// 领域对象
        /// </summary>
        public DomainObject Node { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        public DataObject DataObject { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 数据上下文
        /// </summary>
        public DataContext DataContext { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName
        {
            get
            {
                return RouteInfo[DataObject.ID].TableName;
            }
        }

        /// <summary>
        /// 获取数据对象的表名称
        /// </summary>
        /// <param name="dataObjectID">数据对象ID</param>
        /// <returns>表名称</returns>
        public string GetTableName(string dataObjectID)
        {
            return DataObjectTableMapping[dataObjectID];
        }

        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource
        {
            get
            {
                return RouteInfo[DataObject.ID].DataSource;
            }
        }

        //数据对象表名映射关系
        private Dictionary<string, string> dataObjectTableMapping;

        /// <summary>
        /// 数据对象表名映射关系
        /// </summary>
        public Dictionary<string, string> DataObjectTableMapping
        {
            get
            {
                if (dataObjectTableMapping == null)
                {
                    dataObjectTableMapping = new Dictionary<string, string>();
                    foreach (var item in RouteInfo)
                    {
                        dataObjectTableMapping.Add(item.Key, item.Value.TableName);
                    }
                }

                return dataObjectTableMapping;
            }
        }

        private Dictionary<string, ShardingTarget> routeInfo;

        /// <summary>
        /// 路由信息
        /// </summary>
        public Dictionary<string, ShardingTarget> RouteInfo
        {
            get
            {
                if (routeInfo == null)
                    routeInfo = new Dictionary<string, ShardingTarget>();

                return routeInfo;
            }
            set
            {
                routeInfo = value;
            }
        }

        /// <summary>
        /// 多条主键数据集合
        /// </summary>        
        /// <remarks>key 是列的ColumnID，value是列的值。</remarks>
        public List<Dictionary<string, object>> PrimaryKeyDataCollection { get; set; }

        /// <summary>
        /// 返回数据的行数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否使用条件
        /// </summary>
        /// <remarks>若使用条件，要求DataID为<code>null</code>值。</remarks>
        public bool IsUseCondition
        {
            get { return !string.IsNullOrWhiteSpace(this.FilterCondition); }
        }

        /// <summary>
        /// 判断是否是一定使用主键作为条件约束。
        /// 因为在删除子对象的时候可能指定只删除一条记录
        /// </summary>
        public bool IsUsePrimaryCondition { get; set; }

        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        /// <remarks>
        /// 用于查询数据过滤
        /// </remarks>
        public bool IsLogicalDelete { get; set; }

        /// <summary>
        /// 投影字段集合
        /// </summary>
        private IList<string> projectionFields;

        /// <summary>
        /// 投影字段集合
        /// </summary>
        /// <remarks>Column ID 集合</remarks>
        public IList<string> ProjectionFields
        {
            get
            {
                if (projectionFields == null)
                    projectionFields = new List<string>();

                return projectionFields;
            }
            set
            {
                projectionFields = value;
            }
        }

        /// <summary>
        /// 过滤条件
        /// </summary>
        public string FilterCondition { get; set; }

        /// <summary>
        /// 排序条件
        /// </summary>
        public string OrderByCondition { get; set; }

        /// <summary>
        /// 过滤条件实体类
        /// </summary>
        public QueryFilter QueryFilter { get; set; }

        #endregion
    }
}
