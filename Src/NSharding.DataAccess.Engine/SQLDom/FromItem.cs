// ===============================================================================
// 浪潮GSP平台
// From子句类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/31 16:02:47        1.0        周国庆          初稿。
// 2013/1/31 17:31:00        1.1        周国庆           完成初稿
// ===============================================================================
// 开发者: 周国庆
// 2013/1/31 16:02:47 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// From子句类
    /// </summary>
    /// <remarks>From子句</remarks>
    [Serializable]
    public class FromItem : SqlElement
    {
        #region 常量

        /// <summary>
        /// From子句常量
        /// </summary>
        public const string FROMITEM = "MainFromItem";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public FromItem()
            : base()
        {
            //子元素集合存储的是Leftjoin/InnerJoin等
            base.CreateChildCollection();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table">表</param>
        public FromItem(SqlTable table)
            : this()
        {
            Table = table;
        }

        #endregion

        #region 属性

        public SqlTable Table { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            StringBuilder result = new StringBuilder();
            result.Append(this.Table.ToSQL());
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                result.Append(this.ChildCollection[index].ToSQL());
                if (index < this.ChildCollection.Count - 1)
                    result.Append(" ");
            }

            return result.ToString();
            //return Table.ToSQL() + string.Join(" ", ChildCollection.ToSQL());
        }

        #region 序列化

        /// <summary>
        /// 将SqlNode的属性写入XmlElement。
        /// </summary>
        /// <param name="sqlElement">SqlNode类的对象（或继承自SqlNode类的对象）</param>
        /// <param name="xmlParent">XmlDocument中的元素</param>
        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            FromItem fromItem = sqlElement as FromItem;
            XmlElement xmlTable = SerializerUtil.AddElement(xmlParent, SqlTable.TABLE);
            fromItem.Table.ToXml(fromItem.Table, xmlTable);
        }

        /// <summary>
        /// 用XmlElement的信息构造SqlNode类的对象（或继承自SqlNode类的对象）。
        /// </summary>
        /// <param name="sqlElement">SqlNode类的对象（或继承自SqlNode类的对象）</param>
        /// <param name="xmlParent">XmlDocument中的元素</param>
        /// <param name="xnm">集合命名空间范围管理类的对象</param>
        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            FromItem fromItem = sqlElement as FromItem;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlTable = util.Child(xmlParent, SqlTable.TABLE);
            fromItem.Table.FromXml(fromItem.Table, xmlTable, xnm);
        }

        #endregion 序列化


        #endregion

    }
}
