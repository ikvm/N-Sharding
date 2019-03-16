using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 关联条件项
    /// </summary>
    /// <remarks>关联条件项</remarks>
    [Serializable]
    public class JoinConditionItem : SqlElement
    {
        #region 常量

        /// <summary>
        /// JoinConditionItem
        /// </summary>
        public const string JOINCONDITIONITEM = "JoinConditionItem";

        /// <summary> 
        /// LeftField 
        /// </summary>
        public const string LEFTFIELD = "LeftField";

        /// <summary> 
        /// RightField 
        /// </summary>
        public const string RIGHTFIELD = "RightField";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public JoinConditionItem()
        {
            LeftField = new ConditionField();
            RightField = new ConditionField();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 左连接主表的条件中的对应字段
        /// </summary>
        public ConditionField LeftField { get; set; }

        /// <summary>
        /// 左连接的表的条件对应字段
        /// </summary>
        public ConditionField RightField { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            return string.Format("{0}={1}", LeftField.ToSQL(), RightField.ToSQL());
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>关联条件项</returns>
        public override object Clone()
        {
            JoinConditionItem newValue = base.Clone() as JoinConditionItem;

            if (LeftField != null)
                newValue.LeftField = LeftField.Clone() as ConditionField;
            if (RightField != null)
                newValue.RightField = RightField.Clone() as ConditionField;

            return newValue;
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            JoinConditionItem joinConditionItem = sqlElement as JoinConditionItem;

            XmlElement xmlLeftField = SerializerUtil.AddElement(xmlParent, LEFTFIELD);
            joinConditionItem.LeftField.ToXml(joinConditionItem.LeftField, xmlLeftField);

            XmlElement xmlRightField = SerializerUtil.AddElement(xmlParent, RIGHTFIELD);
            joinConditionItem.RightField.ToXml(joinConditionItem.RightField, xmlRightField);
        }

        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            JoinConditionItem joinConditionItem = sqlElement as JoinConditionItem;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlLeftField = util.Child(xmlParent, LEFTFIELD);
            joinConditionItem.LeftField.FromXml(joinConditionItem.LeftField, xmlLeftField, xnm);

            XmlElement xmlRightField = util.Child(xmlParent, RIGHTFIELD);
            joinConditionItem.RightField.FromXml(joinConditionItem.RightField, xmlRightField, xnm);
        }

        #endregion 序列化

        #endregion
    }
}