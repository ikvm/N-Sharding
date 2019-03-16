// ===============================================================================
// 浪潮GSP平台
// 主键类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/1 11:55:28        1.0        周国庆          初稿。
// 2013/3/5 15:56:49        1.1        周祥国        完善。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/1 11:55:28 
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
    /// 主键类
    /// </summary>
    /// <remarks>表的主键</remarks>
    public class SqlPrimaryKeyField : Field
    {
        #region 常量

        /// <summary>
        /// SqlPrimaryKeyField
        /// </summary>
        public const string SQLPRIMARYKEYFIELD = "SqlPrimaryKeyField";

        #endregion

        #region 字段

        //private Field field;//主键字段
        private FieldValue fieldValue; //主键字段值

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlPrimaryKeyField()
            : base()
        {
            //this.field = new Field();
            this.fieldValue = new FieldValue();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlPrimaryKeyField(SqlTable table, string fieldName)
            : this()
        {
            //this.field = new Field(table, fieldName);
            this.FieldName = fieldName;
            this.Table = table;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 主键字段值
        /// </summary>
        public FieldValue Value
        {
            get { return this.fieldValue; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>主键类</returns>
        public override object Clone()
        {
            SqlPrimaryKeyField newObj = this.MemberwiseClone() as SqlPrimaryKeyField;
            if (this.Value != null)
                newObj.fieldValue = this.Value.Clone() as FieldValue;
            return newObj;
        }

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
            stringBuilder.Append(FieldName);

            return string.Format("{0}=:{1} ", stringBuilder.ToString(), FieldName);
            //return string.Format("{0} = {1} ", base.ToSQL(), this.fieldValue.ToSQL());
        }

        #region 序列化

        //public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        //{
        //    base.ToXml(sqlElement, xmlParent);

        //    SqlPrimaryKeyField primaryKey = new SqlPrimaryKeyField();
        //    primaryKey = sqlElement as SqlPrimaryKeyField;
        //    XmlElement xmlField = SerializerUtil.AddElement(xmlParent, Field.FIELD);
        //    primaryKey.Field.ToXml(primaryKey.Field, xmlField);

        //    //不需要对FieldValue进行序列化
        //    //XmlElement xmlValue = SerializerUtil.AddElement(xmlParent, FieldValue.FIELDVALUE);
        //    //primaryKey.Value.ToXml(primaryKey.Value, xmlValue);
        //}

        //public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        //{
        //    base.FromXml(sqlElement, xmlParent, xnm);

        //    SqlPrimaryKeyField primaryKey = sqlElement as SqlPrimaryKeyField;
        //    ParserUtil util = new ParserUtil(xnm);

        //    XmlElement xmlField = util.Child(xmlParent, Field.FIELD);
        //    primaryKey.Field.FromXml(primaryKey.Field, xmlField, xnm);
        //    //XmlElement xmlValue = util.Child(xmlParent, FieldValue.FIELDVALUE);
        //    //primaryKey.Value.FromXml(primaryKey.Value, xmlValue, xnm);
        //}

        #endregion 序列化

        #endregion
    }
}