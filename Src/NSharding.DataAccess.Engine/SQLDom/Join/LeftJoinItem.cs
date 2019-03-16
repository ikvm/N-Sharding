using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 左连接子项类
    /// </summary>
    /// <remarks>SQL语句中的左连接子项</remarks>
    public class LeftJoinItem : SqlElement
    {
        #region 常量

        /// <summary>
        /// 左连接子项常量
        /// </summary>
        public const string LEFTJOINITEM = "LeftJoinItem";

        /// <summary>
        /// 左连接表常量
        /// </summary>
        public const string LEFTJOINTABLE = "LeftJoinTable";

        /// <summary>
        /// 额外条件常量
        /// </summary>
        public const string ADDITIONALCONDITION = "AdditionalCondition";

        #endregion

        #region 字段

        /// <summary>
        /// 是否使用条件
        /// </summary>
        private bool isUseCondition = false;

        /// <summary>
        /// 
        /// </summary>
        private string additionalCondition;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public LeftJoinItem()
            : base()
        {
            LeftJoinTable = new SqlTable();
            base.CreateChildCollection();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 左连接表
        /// </summary>
        public SqlTable LeftJoinTable { get; set; }

        /// <summary>
        /// 是否使用条件
        /// </summary>
        public bool IsUseCondition
        {
            get
            {
                return isUseCondition;
            }
        }

        /// <summary>
        /// 附加条件
        /// </summary>
        /// <remarks>
        /// 对应于“关联上的Where条件”和“引用模型上的过滤条件”的与运算
        /// </remarks>
        public string AdditionalCondition
        {
            get
            {
                return additionalCondition;
            }
            set
            {
                additionalCondition = value;
                if (string.IsNullOrEmpty(additionalCondition))
                {
                    isUseCondition = false;
                }
                else
                {
                    isUseCondition = true;
                }
            }
        }

        #endregion

        #region 方法

        #endregion

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>左连接子项类</returns>
        public override object Clone()
        {
            var newItem = base.Clone() as LeftJoinItem;

            if (LeftJoinTable != null)
                newItem.LeftJoinTable = LeftJoinTable;

            return newItem;
        }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            StringBuilder result = new StringBuilder();
            result.Append(" LEFT OUTER JOIN ").Append(this.LeftJoinTable.ToSQL()).Append(" ON ");
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                result.Append(this.ChildCollection[index].ToSQL());
                if (index < this.ChildCollection.Count - 1)
                    result.Append(" AND ");
            }

            if (IsUseCondition)
                result.Append(" AND ").Append(this.AdditionalCondition);

            return result.ToString();
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            LeftJoinItem leftJoinItem = sqlElement as LeftJoinItem;

            XmlElement xmlLeftJoinTable = SerializerUtil.AddElement(xmlParent, LEFTJOINTABLE);
            leftJoinItem.LeftJoinTable.ToXml(leftJoinItem.LeftJoinTable, xmlLeftJoinTable);

            XmlElement xmlAddtionalCondition = SerializerUtil.AddElement(xmlParent, ADDITIONALCONDITION, leftJoinItem.AdditionalCondition);
        }

        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            LeftJoinItem leftJoinItem = sqlElement as LeftJoinItem;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlLeftJoinTable = util.Child(xmlParent, LEFTJOINTABLE);
            leftJoinItem.LeftJoinTable.FromXml(leftJoinItem.LeftJoinTable, xmlLeftJoinTable, xnm);

            XmlElement xmlAddtionalCondition = util.Child(xmlParent, ADDITIONALCONDITION);
            leftJoinItem.AdditionalCondition = xmlAddtionalCondition.InnerText;
        }

        #endregion 序列化
    }
}
