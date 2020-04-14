// ===============================================================================
// 浪潮GSP平台
// ***类说明***
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/12 17:19:00        1.0        周国庆         初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/12 17:19:00 
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
    /// Select语句中的查询字段
    /// </summary>
    /// <remarks>Select语句中的查询字段</remarks>
    [Serializable]
    public class SelectListField : Field
    {
        #region 常量

        /// <summary> 
        /// SelectListField 
        /// </summary>
        public const string SELECTLISTFIELD = "SelectListField";

        /// <summary> 
        /// FieldAlias 
        /// </summary>
        public const string FIELDALIAS = "FieldAlias";

        #endregion

        #region 字段

        /// <summary>
        /// 是否使用字段别名
        /// </summary>
        private bool isUseAlias;

        /// <summary>
        /// 字段别名
        /// </summary>
        private string fieldAlias;

        #endregion

        #region 构造函数

        public SelectListField()
            : base()
        {
            isUseAlias = false;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 字段别名
        /// </summary>
        public string FieldAlias
        {
            get
            {
                return fieldAlias;
            }
            set
            {
                fieldAlias = value;
                if (!string.IsNullOrEmpty(fieldAlias))
                {
                    isUseAlias = true;
                }
                else
                {
                    isUseAlias = false;
                }
            }
        }

        /// <summary>
        /// 字段是否使用别名
        /// </summary>
        public bool IsUseAlias { get; set; }

        /// <summary>
        /// 字段是否动态（根据条件构建出来的字段）
        /// </summary>
        public bool IsDynamic { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            StringBuilder result = new StringBuilder();
            result.Append(base.ToSQL());
            if (IsUseAlias)
                result.Append(" AS \"").Append(this.fieldAlias).Append("\"");

            return result.ToString();
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            SelectListField selectListField = sqlElement as SelectListField;
            XmlElement xmlFieldAlias = SerializerUtil.AddElement(xmlParent, FIELDALIAS, selectListField.FieldAlias);
        }

        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            SelectListField selectListField = sqlElement as SelectListField;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlFieldAlias = util.Child(xmlParent, FIELDALIAS);
            selectListField.FieldAlias = xmlFieldAlias.InnerText;
        }

        #endregion 序列化

        #endregion       
    }
}
