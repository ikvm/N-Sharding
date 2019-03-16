// ===============================================================================
// 浪潮GSP平台
// 删除SQL语句中子查询语句类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/15 17:46:53        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/15 17:46:53 
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
    /// 删除SQL语句中子查询语句类
    /// </summary>
    /// <remarks>删除SQL语句中子查询语句</remarks>
    public class SelectSqlForSubQuery : SqlStatement
    {
        #region 常量

        /// <summary> 
        /// SelectSqlForDelete 
        /// </summary>
        public const string SELECTSQLFORDELETE = "SelectSqlForDelete";

        /// <summary> 
        /// MainFromItem 
        /// </summary>
        public const string MAINFROMITEM = "MainFromItem";

        /// <summary> 
        /// JoinSubQueryConditionItem 
        /// </summary>
        public const string JOINSUBQUERYCONDITIONITEM = "JoinSubQueryConditionItem";

        #endregion

        #region 字段

        /// <summary>
        /// 查询中主表的From语句项
        /// </summary>
        private FromItem mainFromItem;

        /// <summary>
        /// 构造删除的主表和（where条件中的）子查询之间的关联关系。
        /// </summary>
        private JoinConditionItem joinSubQueryConditionItem;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectSqlForSubQuery()
            : base()
        {
            From = new From();
            MainFromItem = new FromItem();
            Condition = new FilterConditionStatement();
            JoinCondition = new JoinConditionStatement();
            joinSubQueryConditionItem = new JoinConditionItem();
            //JoinCondition.ChildCollection.Add(this.joinSubQueryConditionItem);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 查询中From子句
        /// </summary>
        /// <remarks>
        /// 其ChildCollection属性中包括所有的From语句项。
        /// </remarks>
        public From From { get; set; }

        /// <summary>
        /// 查询中主表的From语句项。
        /// </summary>
        /// <remarks>
        /// 该FromItem项在构造方法中就加到From子句中了，所以只能对其修改，不能替换。
        /// </remarks>
        public FromItem MainFromItem
        {
            get
            {
                return this.mainFromItem;
            }
            private set
            {
                this.mainFromItem = value;
            }
        }

        /// <summary>
        /// 多表的连接条件定义
        /// </summary>
        public JoinConditionStatement JoinCondition { get; set; }

        /// <summary>
        /// 构造删除的主表和（where条件中的）子查询之间的关联关系。
        /// </summary>
        /// <remarks>
        /// 该JoinConditionItem项在构造方法中就加到JoinConditionStatement条件中了，所以只能对其修改，不能替换。
        /// </remarks>
        public JoinConditionItem JoinSubQueryConditionItem
        {
            get { return joinSubQueryConditionItem; }
        }

        /// <summary>
        /// 动态条件定义语句
        /// </summary>
        public FilterConditionStatement Condition { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>删除SQL语句中子查询语句</returns>
        public override object Clone()
        {
            SelectSqlForSubQuery newValue = base.Clone() as SelectSqlForSubQuery;

            if (From != null)
                newValue.From = From.Clone() as From;
            if (MainFromItem != null)
                newValue.MainFromItem = MainFromItem.Clone() as FromItem;
            if (JoinCondition != null)
                newValue.JoinCondition = JoinCondition.Clone() as JoinConditionStatement;
            //if (JoinSubQueryConditionItem != null)
            //    newValue.joinSubQueryConditionItem = JoinSubQueryConditionItem.Clone() as JoinConditionItem;
            if (Condition != null)
                newValue.Condition = Condition.Clone() as FilterConditionStatement;

            return newValue;
        }

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>SQL语句</returns>
        public override string ToSQL()
        {           
            StringBuilder result = new StringBuilder();
            result.Append("SELECT 1 FROM ").Append(From.ToSQL());

            string joinStr = JoinCondition.ToSQL();
            string conditionStr = Condition.ToSQL();

            if (!string.IsNullOrWhiteSpace(joinStr))
            {
                result.Append(" WHERE ").Append(joinStr);
            }

            if (!string.IsNullOrWhiteSpace(conditionStr))
            {
                if (!string.IsNullOrWhiteSpace(joinStr))
                    result.Append(" AND ").Append(conditionStr);
                else
                    result.Append(" WHERE ").Append(conditionStr);
            }

            return result.ToString();
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            SelectSqlForSubQuery subQuery = sqlElement as SelectSqlForSubQuery;
            XmlElement xmlFrom = SerializerUtil.AddElement(xmlParent, From.FROM);
            subQuery.From.ToXml(subQuery.From, xmlFrom);
            XmlElement xmlJoinCondition = SerializerUtil.AddElement(xmlParent, JoinConditionStatement.JOINCONDITIONSTATEMENT);
            subQuery.JoinCondition.ToXml(subQuery.JoinCondition, xmlJoinCondition);
            XmlElement xmlCondition = SerializerUtil.AddElement(xmlParent, ConditionStatement.CONDITIONSTATEMENT);
            subQuery.Condition.ToXml(subQuery.Condition, xmlCondition);

            /*
              * MainFromItem和JoinSubQueryConditionItem只序列化，不反序列化。
              * 其反序列化操作已包含在From和JoinCondition的集合中，直接从集合中取即可。
              * */
            XmlElement xmlMainFromItem = SerializerUtil.AddElement(xmlParent, MAINFROMITEM);
            subQuery.MainFromItem.ToXml(subQuery.MainFromItem, xmlMainFromItem);
            XmlElement xmlJoinSubQueryConditionItem = SerializerUtil.AddElement(xmlParent, JOINSUBQUERYCONDITIONITEM);
            subQuery.JoinSubQueryConditionItem.ToXml(subQuery.JoinSubQueryConditionItem, xmlJoinSubQueryConditionItem);
        }

        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            SelectSqlForSubQuery subQuery = sqlElement as SelectSqlForSubQuery;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlFrom = util.Child(xmlParent, From.FROM);
            XmlElement xmlJoinCondition = util.Child(xmlParent, JoinConditionStatement.JOINCONDITIONSTATEMENT);
            XmlElement xmlCondition = util.Child(xmlParent, ConditionStatement.CONDITIONSTATEMENT);

            subQuery.From.FromXml(subQuery.From, xmlFrom, xnm);
            subQuery.JoinCondition.FromXml(subQuery.JoinCondition, xmlJoinCondition, xnm);
            subQuery.Condition.FromXml(subQuery.Condition, xmlCondition, xnm);

            /*
              * MainFromItem和JoinSubQueryConditionItem只序列化，不反序列化。 
              * 其反序列化操作已包含在From和JoinCondition的集合中，直接从集合中取即可。
              * */
            subQuery.mainFromItem = this.From.ChildCollection[0] as FromItem;
            subQuery.joinSubQueryConditionItem = this.JoinCondition.ChildCollection[0] as JoinConditionItem;
        }

        #endregion 序列化

        #endregion
    }
}