// ===============================================================================
// 浪潮GSP平台
// 字段类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/31 22:21:23        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/1/31 22:21:23 
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
    /// 字段类
    /// </summary>
    /// <remarks>字段类</remarks>
    [Serializable]
    public class Field : SqlElement
    {
        #region 常量

        /// <summary>
        /// 字段常量
        /// </summary>
        public const string FIELD = "Field";

        /// <summary>
        /// 字段名称常量
        /// </summary>
        public const string FIELDNAME = "FieldName";

        /// <summary>
        /// 是否使用字段前缀
        /// </summary>
        public const string ISUSEFIELDPREFIX = "IsUseFieldPrefix";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public Field()
            : base()
        {
            Table = new SqlTable();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table">隶属的表</param>
        /// <param name="fieldName">字段名称</param>
        public Field(SqlTable table, string fieldName)
            : base()
        {
            Table = table;
            FieldName = fieldName;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 隶属的表
        /// </summary>
        public SqlTable Table { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 是否使用字段前缀
        /// </summary>
        public bool IsUseFieldPrefix { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            var stringBuilder = new StringBuilder();
            if (IsUseFieldPrefix)
            {
                stringBuilder.Append(Table.TablePrefix).Append(".");
            }
#warning 关键字判断
            stringBuilder.Append(FieldName);

            return stringBuilder.ToString();
        }

        #region 序列化

        /// <summary>
        /// 转换成XML
        /// </summary>
        /// <param name="sqlElement">SQL元素</param>
        /// <param name="xmlParent">XML父节点元素</param>
        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            Field field = sqlElement as Field;
            if (field.Table != null)// Table 可以为空
            {
                XmlElement xmlTable = SerializerUtil.AddElement(xmlParent, SqlTable.TABLE);
                field.Table.ToXml(field.Table, xmlTable);
            }

            SerializerUtil.AddElement(xmlParent, FIELDNAME, field.FieldName);
            SerializerUtil.AddElement(xmlParent, ISUSEFIELDPREFIX, field.IsUseFieldPrefix.ToString());
        }

        /// <summary>
        /// 转换成Insert语句
        /// </summary>
        /// <param name="sqlNode">SQL元素</param>
        /// <param name="xmlParent">XML父节点元素</param>
        /// <param name="xnm">集合命名空间范围管理类的对象</param>
        public override void FromXml(SqlElement sqlNode, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlNode, xmlParent, xnm);

            Field field = sqlNode as Field;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlTable = util.Child(xmlParent, SqlTable.TABLE);
            if (xmlTable != null)
            {
                field.Table.FromXml(field.Table, xmlTable, xnm);
            }

            XmlElement xmlFieldName = util.Child(xmlParent, FIELDNAME);
            XmlElement xmlIsUseFieldPrefix = util.Child(xmlParent, ISUSEFIELDPREFIX);
            field.FieldName = xmlFieldName.InnerText;
            field.IsUseFieldPrefix = bool.Parse(xmlIsUseFieldPrefix.InnerText);
        }

        #endregion 序列化

        #endregion
    }
}