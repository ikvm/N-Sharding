// ===============================================================================
// 浪潮GSP平台
// 子查询SQL语句类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/3/20 16:22:16        1.0        周国庆           新建。
// ===============================================================================
// 开发者: 周国庆
// 2013/3/20 16:22:16 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 子查询SQL语句类
    /// </summary>
    /// <remarks>为了支持一个数据对象包含多个数据库表的场景，使用子查询统一处理</remarks>
    public class SubQuerySqlStatement : SqlTable
    {
        #region 常量

        /// <summary>
        /// SelectSqlStatement
        /// </summary>
        public const string SELECTSQLSTATEMENT = "SubQuerySqlStatement";

        /// <summary>
        /// JoinCondition 
        /// </summary>
        public const string JOINCONDITION = "JoinCondition";

        /// <summary> 
        /// FilterCondition
        /// </summary>
        public const string FILTERCONDITION = "FilterCondition";

        /// <summary> 
        /// OrderByCondition 
        /// </summary>
        public const string ORDERBYCONDITION = "OrderByCondition";

        /// <summary>
        /// MainFromItem 
        /// </summary>
        public const string MAINFROMITEM = "MainFromItem";

        #endregion

        #region 字段

        /// <summary>
        /// Select语句中的核心查询主体
        /// </summary>
        private FromItem mainFromItem;

        /// <summary>
        /// 返回获取数据的前多少条，默认值-1
        /// </summary>
        private int topSize = -1;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SubQuerySqlStatement()
            : base()
        {           
            From = new From();
            mainFromItem = new FromItem();
            From.ChildCollection.Add(mainFromItem);
            SelectList = new SelectFieldListStatement();
            JoinCondition = new JoinConditionStatement();
            FilterCondition = new ConditionStatement();
            OrderByCondition = new ConditionStatement();
            DictFieldAliasMapping = new Dictionary<string, string>();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 查询字段列表
        /// </summary>
        public SelectFieldListStatement SelectList { get; set; }

        /// <summary>
        /// From子句
        /// </summary>
        public From From { get; set; }

        /// <summary>
        /// 查询中主表的From语句项。
        /// </summary>
        public FromItem MainFromItem
        {
            get
            {
                return this.mainFromItem;
            }
        }

        /// <summary>
        /// 多表的连接条件
        /// </summary>
        public JoinConditionStatement JoinCondition { get; set; }

        /// <summary>
        /// 过滤条件
        /// </summary>
        public ConditionStatement FilterCondition { get; set; }

        /// <summary>
        /// 排序条件
        /// </summary>
        public ConditionStatement OrderByCondition { get; set; }

        /// <summary>
        /// 表编号
        /// </summary>
        public string TableCode { get; set; }

        /// <summary>
        /// 主键列表
        /// </summary>
        public SqlPrimaryKey PrimaryKeys { get; set; }

        /// <summary>
        /// SQL语句构造中用到的中间变量
        /// </summary>
        public SqlBuildingInfo SqlBuildingInfo { get; set; }

        /// <summary>
        /// 返回获取数据的前多少条，默认值-1
        /// </summary>
        public int TopSize
        {
            get
            {
                return topSize;
            }
            set
            {
                topSize = value;
            }
        }

        /// <summary>
        /// 如果使用字段别名，那么此属性表示映射后的顺序号
        /// </summary>
        public int AliasCount { get; set; }

        /// <summary>
        /// 字段别名映射
        /// </summary>
        public Dictionary<string,string> DictFieldAliasMapping { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>Select SQL语句</returns>
        public override string ToSQL()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("(SELECT {0} FROM {1} ", SelectList.ToSQL(), From.ToSQL()));
            foreach (SqlElement element in MainFromItem.ChildCollection)
            {
                if (element is InnerJoinItem && (element as InnerJoinItem).IsExtendItem)
                    stringBuilder.Append((element as InnerJoinItem).ToSQLEx());
            }

            var joinCondition = JoinCondition.ToSQL();
            if (!string.IsNullOrWhiteSpace(joinCondition))
            {
                stringBuilder.AppendFormat("WHERE {0} ", joinCondition);
            }

            var filterCondition = FilterCondition.ToSQL();
            if (!string.IsNullOrWhiteSpace(filterCondition))
            {
                if (string.IsNullOrWhiteSpace(joinCondition))
                {
                    stringBuilder.AppendFormat("WHERE {0} ", filterCondition);
                }
                else
                {
                    stringBuilder.AppendFormat("AND {0} ", filterCondition);
                }
            }
            var orderbyCondition = OrderByCondition.ToSQL();
            if (!string.IsNullOrWhiteSpace(orderbyCondition))
            {
                stringBuilder.AppendFormat("ORDER BY {0}", orderbyCondition);
            }

            stringBuilder.AppendFormat(") {0}", TableAlias);

            return stringBuilder.ToString();
        }

        #region 序列化

        /// <summary>
        /// 转换成XmlElement
        /// </summary>
        /// <param name="sqlElement">要转换的对象</param>
        /// <param name="xmlParent">附加到的XmlElement</param>
        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            SubQuerySqlStatement selectSql = sqlElement as SubQuerySqlStatement;
            XmlElement xmlSelectList = SerializerUtil.AddElement(xmlParent, SelectFieldListStatement.SELECTLISTSTATEMENT);
            selectSql.SelectList.ToXml(selectSql.SelectList, xmlSelectList);
            XmlElement xmlFrom = SerializerUtil.AddElement(xmlParent, From.FROM);
            selectSql.From.ToXml(selectSql.From, xmlFrom);
            XmlElement xmlJoinCondition = SerializerUtil.AddElement(xmlParent, JoinConditionStatement.JOINCONDITIONSTATEMENT);
            selectSql.JoinCondition.ToXml(selectSql.JoinCondition, xmlJoinCondition);
            XmlElement xmlFilterCondition = SerializerUtil.AddElement(xmlParent, FILTERCONDITION);
            selectSql.FilterCondition.ToXml(selectSql.FilterCondition, xmlFilterCondition);
            XmlElement xmlOrderByCondition = SerializerUtil.AddElement(xmlParent, ORDERBYCONDITION);
            selectSql.OrderByCondition.ToXml(selectSql.OrderByCondition, xmlOrderByCondition);

            /*
              * MainFromItem只序列化，不反序列化。
              * 其反序列化操作已包含在Froms的集合中，直接从集合中取即可。
              * */
            XmlElement xmlMainFromItem = SerializerUtil.AddElement(xmlParent, MAINFROMITEM);
            selectSql.MainFromItem.ToXml(selectSql.MainFromItem, xmlMainFromItem);
        }

        /// <summary>
        /// 由XmlElement转换成SqlElement
        /// </summary>
        /// <param name="sqlElement">附加到的SqlElement</param>
        /// <param name="xmlParent">反序列化的XmlElement</param>
        /// <param name="xnm">命名空间</param>
        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            SubQuerySqlStatement selectSql = sqlElement as SubQuerySqlStatement;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlSelectList = util.Child(xmlParent, SelectFieldListStatement.SELECTLISTSTATEMENT);
            XmlElement xmlFrom = util.Child(xmlParent, From.FROM);
            XmlElement xmlJoinCondition = util.Child(xmlParent, JoinConditionStatement.JOINCONDITIONSTATEMENT);
            XmlElement xmlFilterCondition = util.Child(xmlParent, FILTERCONDITION);
            XmlElement xmlOrderByCondition = util.Child(xmlParent, ORDERBYCONDITION);

            selectSql.SelectList.FromXml(selectSql.SelectList, xmlSelectList, xnm);
            selectSql.From.FromXml(selectSql.From, xmlFrom, xnm);
            selectSql.JoinCondition.FromXml(selectSql.JoinCondition, xmlJoinCondition, xnm);
            selectSql.FilterCondition.FromXml(selectSql.FilterCondition, xmlFilterCondition, xnm);
            selectSql.OrderByCondition.FromXml(selectSql.OrderByCondition, xmlOrderByCondition, xnm);

            /*
              * MainFromItem只序列化，不反序列化。
              * 其反序列化操作已包含在Froms的集合中，直接从集合中取即可。
              * */
            selectSql.mainFromItem = this.From.ChildCollection[0] as FromItem;
        }

        #endregion 序列化

        #endregion
    }
}
