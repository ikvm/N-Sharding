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
    /// 内部关联类
    /// </summary>
    class InternalAssociation
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public InternalAssociation()
        {
            RefElements = new List<InternalRefElement>();
            ID = Guid.NewGuid().ToString();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// 对应的关联定义
        /// </summary>
        public Association Association { get; set; }

        /// <summary>
        /// 关联的表。
        /// </summary>
        public SqlTable AssociatedTable { get; set; }

        /// <summary>
        /// 关联的通用中间结构
        /// </summary>
        public DomainModel.Spi.DomainModel AssociatedCommonObject { get; set; }

        /// <summary>
        /// 关联的节点对象
        /// </summary>
        public DomainObject AssociatedCoNode { get; set; }

        /// <summary>
        /// 关联的数据对象
        /// </summary>
        public DataObject AssociatedDataObject { get; set; }

        /// <summary>
        /// 关联带过来的元素集合
        /// </summary>
        public List<InternalRefElement> RefElements { get; set; }

        /// <summary>
        /// 附加条件
        /// </summary>
        public string AdditionalCondition { get; set; }

        /// <summary>
        /// 该关联所属的通用中间对象
        /// </summary>
        public DomainModel.Spi.DomainModel LocatedCommonObject { get; set; }

        /// <summary>
        /// 所属的节点对象
        /// </summary>
        public DomainObject LocatedNode { get; set; }

        /// <summary>
        /// 所属的数据对象
        /// </summary>
        public DataObject LocatedDataObject { get; set; }

        /// <summary>
        /// 所属的SQLDOM Table
        /// </summary>
        public SqlTable LocatedTable { get; set; }

        #endregion
    }
}
