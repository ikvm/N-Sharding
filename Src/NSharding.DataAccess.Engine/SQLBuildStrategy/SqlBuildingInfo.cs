using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using NSharding.DomainModel.Spi;
using NSharding.Sharding.Database;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL拼装中的中间变量类。
    /// </summary>
    /// <remarks>SQL拼装时用到的中间变量</remarks>
    public class SqlBuildingInfo : ICloneable
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlBuildingInfo()
        {
            this.SqlTableCollection = new Hashtable();
            this.SqlTableAliasCollection = new Hashtable();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 通用中间对象
        /// </summary>
        public DomainModel.Spi.DomainModel CommonObject { get; set; }

        /// <summary>
        /// 构造SQL的当前节点对象
        /// </summary>
        public DomainObject CurrentNode { get; set; }

        /// <summary>
        /// 构造SQL用到的当前数据对象
        /// </summary>
        public DataObject CurrentDataObject { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 构造SQL用到的当前表
        /// </summary>
        public SqlTable CurrentSqlTable { get; set; }

        /// <summary>
        /// 根节点对象
        /// </summary>
        /// <remarks>
        /// 在Select语句中，当查询主从从节点中的从节点时，
        /// 需要使用关联主节点进行数据过滤，因此在SqlBuildingInfo存储RootNode节点类。
        /// 其他的用处再进行补充</remarks>
        public DomainObject RootNode { get; set; }

        /// <summary>
        ///  根数据对象
        /// </summary>
        public DataObject RootDataObject { get; set; }

        /// <summary>
        /// 根数据对象对应的表
        /// </summary>
        public SqlTable RootSqlTable { get; set; }

        /// <summary>
        /// 表的唯一标识，对应形成sql中的一个表，若一个表在sql中被使用多次，在此有不同的标识，分别对应多个Sql的Table实例。
        /// </summary>
        /// <remarks>
        /// 的唯一标识，对应形成sql中的一个表，若一个表在sql中被使用多次，在此应该有不同的标识，分别对应多个Sql的Table实例。
        /// 模型中的表有两种：
        /// 一种是包含关系（主从关系），是模型的主体，这些表的id是AbstractObject的id；
        /// 另一种是关联关系，由于每一个关联总是描述两个表之间的关系，并且发起关联的表总是一个模型主体的表或者已经被标识的表
        /// （关联的关联的形式），所以在一个关联中要标识的表就是要关联到的表，这样关联和关联到的表是一一对应的，并且不同关联
        /// 可以对应到同一个物理表，这样正好用关联的id来标识sql中的关联表，即这些表的id是Association的id。
        /// </remarks>
        public Hashtable SqlTableCollection { get; set; }

        /// <summary>
        /// 已使用表的别名HashTable
        /// </summary>
        /// <remarks>
        /// 同一张表，可能被关联多次，所以需要给表生成别名。
        /// </remarks>
        public Hashtable SqlTableAliasCollection { get; set; }

        #endregion

        #region 方法

        #endregion

        public object Clone()
        {
            SqlBuildingInfo newInfo = base.MemberwiseClone() as SqlBuildingInfo;
            newInfo.SqlTableCollection = new Hashtable();
            foreach (DictionaryEntry item in SqlTableCollection)
            {
                newInfo.SqlTableCollection.Add(item.Key, item.Value);
            }

            newInfo.SqlTableAliasCollection = new Hashtable();
            foreach (DictionaryEntry item in SqlTableAliasCollection)
            {
                newInfo.SqlTableAliasCollection.Add(item.Key, item.Value);
            }

            return newInfo;
        }
    }
}