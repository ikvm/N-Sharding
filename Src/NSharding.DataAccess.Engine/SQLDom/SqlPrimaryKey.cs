// ===============================================================================
// 浪潮GSP平台
// ***类说明***
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/1 15:56:49        1.0        周国庆          初稿。
// 2013/3/5 15:56:49        1.1        周祥国        完善。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/1 15:56:49 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// ***类说明***
    /// </summary>
    /// <remarks>类的补充说明</remarks>
    public class SqlPrimaryKey : SqlElement
    {
        #region 常量

        /// <summary>
        /// SqlPrimaryKey
        /// </summary>
        public const string SQLPRIMARYKEY = "SqlPrimaryKey";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlPrimaryKey()
            : base()
        {
            base.CreateChildCollection();
        }

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            if (this.ChildCollection == null || this.ChildCollection.Count == 0)
                return string.Empty;

            var result = new StringBuilder();
            SqlPrimaryKeyField keyField = null;
            for (int i = 0; i < this.ChildCollection.Count; i++)
            {
                keyField = this.ChildCollection[i] as SqlPrimaryKeyField;
                result.Append(keyField.ToSQL());
                if (i < this.ChildCollection.Count - 1)
                    result.Append(" AND ");
            }
            return result.ToString();
        }

        /// <summary>
        /// 主键实体的克隆。
        /// </summary>
        /// <returns>主键实体的副本。</returns>
        public override object Clone()
        {
            SqlPrimaryKey newObj = this.MemberwiseClone() as SqlPrimaryKey;
                       
            return newObj;
        }

        #region 序列化

        //public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        //{
        //    base.ToXml(sqlElement, xmlParent);

        //    SqlPrimaryKey primaryKey = sqlElement as SqlPrimaryKey;           
        //    foreach (SqlPrimaryKeyField pkField in primaryKey.ChildCollection)
        //    {                
        //        XmlElement xmlField = SerializerUtil.AddElement(xmlParent, SqlPrimaryKeyField.SQLPRIMARYKEYFIELD);
        //        pkField.ToXml(pkField, xmlField);
        //    }           
        //}

        //public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        //{
        //    base.FromXml(sqlElement, xmlParent, xnm);

        //    SqlPrimaryKey primaryKey = sqlElement as SqlPrimaryKey;
        //    ParserUtil util = new ParserUtil(xnm);
        //    var sqlPrimaryKeyFields = util.Children(xmlParent, SqlPrimaryKeyField.SQLPRIMARYKEYFIELD);
        //    //foreach (SqlElement pkField in sqlPrimaryKeyFields)
        //    //{
        //    //    primaryKey.ChildCollection.Add(pkField);
        //    //}

        //    primaryKey.CreateChildCollection();
        //    foreach (XmlElement xmlChildNode in sqlPrimaryKeyFields)
        //    {              
        //        SqlElement node = new SqlPrimaryKeyField() as SqlElement;
        //        node.FromXml(node, xmlChildNode, xnm);
        //        primaryKey.ChildCollection.Add(node);
        //    }
        //}

        #endregion 序列化

        #endregion
    }
}