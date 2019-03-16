// ===============================================================================
// 浪潮GSP平台
// 数据表类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/30 18:40:56        1.0        周国庆          初稿。
// 2013/1/31 15:24:00        1.1        周国庆           完成初稿
// ===============================================================================
// 开发者: 周国庆
// 2013/1/30 18:40:56 
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
    /// 数据表类
    /// </summary>
    /// <remarks>数据表</remarks>
    [Serializable]
    public class SqlTable : SqlElement
    {
        #region 字段

        /// <summary> Table </summary>
        public const string TABLE = "Table";
        /// <summary> TableName </summary>
        public const string TABLENAME = "TableName";
        /// <summary> TableAlias </summary>
        public const string TABLEALIAS = "TableAlias";
        /// <summary> RealTableName </summary>
        public const string REALTABLENAME = "RealTableName";

        /// <summary>
        /// 是否使用别名
        /// </summary>
        private bool isUseAlias = false;

        /// <summary>
        /// 表别名
        /// </summary>
        private string tableAlias;

        #endregion

        #region 构造函数

        #endregion

        #region 属性

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 是否使用别名
        /// </summary>
        public bool IsUseAlias
        {
            get { return isUseAlias; }
        }

        /// <summary>
        /// 表别名
        /// </summary>
        public string TableAlias
        {
            get
            {
                return tableAlias;
            }
            set
            {
                tableAlias = value;
                isUseAlias = true;
            }
        }

        /// <summary>
        /// 真实的表名
        /// </summary>
        /// <remarks>处理年度表、模块表等需求</remarks>
        public string RealTableName { get; set; }

        /// <summary>
        /// 表名前缀
        /// </summary>
        /// <remarks>用于字段上的表名前缀</remarks>
        public string TablePrefix
        {
            get { return IsUseAlias ? TableAlias : TableName; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlTable()
            : base()
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="realTableName">实表名称</param>
        public SqlTable(string tableName, string tableAlias = "", string realTableName = "")
            : this()
        {
            TableName = tableName;
            TableAlias = tableAlias;
            RealTableName = realTableName;
        }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            this.RealTableName = string.IsNullOrEmpty(this.RealTableName) ? this.TableName : this.RealTableName;
            if (!string.IsNullOrEmpty(TableAlias) && !string.Equals(TableAlias, RealTableName, StringComparison.OrdinalIgnoreCase))
                return string.Format("{0} {1}", this.RealTableName, this.TableAlias);

            return this.RealTableName;
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            // 先处理基类
            base.ToXml(sqlElement, xmlParent);

            SqlTable table = sqlElement as SqlTable;
            XmlElement tableNameElement = SerializerUtil.AddElement(xmlParent, TABLENAME, table.TableName);
            XmlElement tableAliasElement = SerializerUtil.AddElement(xmlParent, TABLEALIAS, table.TableAlias);
            XmlElement realTableNameElement = SerializerUtil.AddElement(xmlParent, REALTABLENAME, table.RealTableName);
        }

        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            ParserUtil util = new ParserUtil(xnm);
            XmlElement tableNameElement = util.Child(xmlParent, TABLENAME);
            XmlElement tableAliasElement = util.Child(xmlParent, TABLEALIAS);
            XmlElement realTableNameElement = util.Child(xmlParent, REALTABLENAME);
            if (tableNameElement != null)
            {
                (sqlElement as SqlTable).TableName = tableNameElement.InnerText;
            }
            if (tableAliasElement != null)
            {
                (sqlElement as SqlTable).TableAlias = tableAliasElement.InnerText;
            }
            if (realTableNameElement != null)
            {
                (sqlElement as SqlTable).RealTableName = realTableNameElement.InnerText;
            }
        }

        #endregion 序列化

        #endregion
    }
}