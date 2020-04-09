// ===============================================================================
// 浪潮GSP平台
// Select SQL语句类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/14 16:38:49        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/14 16:38:49 
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
    /// Select SQL语句类
    /// </summary>
    /// <remarks>Select SQL语句</remarks>
    public class SelectSqlStatement : SqlStatement
    {
        #region 常量

        /// <summary>
        /// SelectSqlStatement
        /// </summary>
        public const string SELECTSQLSTATEMENT = "SelectSqlStatement";

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
        public SelectSqlStatement()
            : base()
        {
            From = new From();
            mainFromItem = new FromItem();
            //From.ChildCollection.Add(mainFromItem);
            SelectList = new SelectFieldListStatement();
            JoinCondition = new JoinConditionStatement();
            FilterCondition = new FilterConditionStatement();
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
            private set
            {
                this.mainFromItem = value;
            }
        }

        /// <summary>
        /// 多表的连接条件
        /// </summary>
        public JoinConditionStatement JoinCondition { get; set; }

        /// <summary>
        /// 过滤条件
        /// </summary>
        public FilterConditionStatement FilterCondition { get; set; }

        /// <summary>
        /// 排序条件
        /// </summary>
        public ConditionStatement OrderByCondition { get; set; }

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

        public int PageCount { get; set; }

        public int PageIndex { get; set; }

        /// <summary>
        /// 如果使用字段别名，那么此属性表示映射后的顺序号
        /// </summary>
        public int AliasCount { get; set; }

        /// <summary>
        /// 字段别名映射
        /// </summary>
        public Dictionary<string, string> DictFieldAliasMapping { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>Select SQL语句</returns>
        public override string ToSQL()
        {
            var sqlFrom = string.Empty;
            var stringBuilder = new StringBuilder();

            //Retrieve cached query SQL statement
            //if (base.SqlBuildingInfo.CurrentNode == null)
            //{
            //    sqlFrom = SQLStatementManager.Instance.
            //        GetSqlStringByCache(SqlBuildingInfo.CurrentDataObject.ID, RequestTokenID);
            //}
            //else
            //{                
            //    sqlFrom = SQLStatementManager.Instance.
            //        GetSqlStringByCache(CommonObjectID, NodeID, TableName, queryType.ToString(), RequestTokenID);
            //}

            if (string.IsNullOrWhiteSpace(sqlFrom))
            {
                sqlFrom = string.Format("SELECT {0} FROM {1}", SelectList.ToSQL(), From.ToSQL());

                #region Put into cache
                // Put into cache
                //if (base.SqlBuildingInfo.CurrentNode == null)
                //{
                //    SQLStatementManager.Instance.
                //        SaveSqlStringToCache(SqlBuildingInfo.CurrentDataObject.ID, RequestTokenID, sqlFrom);
                //}
                //else
                //{
                //    SQLStatementManager.Instance.
                //        SaveSqlStringToCache(CommonObjectID, NodeID, TableName, queryType.ToString(), RequestTokenID, sqlFrom);
                //}
                #endregion
            }
            stringBuilder.Append(sqlFrom);

            foreach (SqlElement element in MainFromItem.ChildCollection)
            {
                if (element is InnerJoinItem)
                {
                    stringBuilder.Append((element as InnerJoinItem).ToSQLEx());
                }
                else if (element is LeftJoinItem)
                {
                    stringBuilder.Append(element.ToSQL());
                }
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
                    stringBuilder.AppendFormat(" WHERE {0} ", filterCondition);
                }
                else
                {
                    stringBuilder.AppendFormat(" AND {0} ", filterCondition);
                }
            }
            var orderbyCondition = OrderByCondition.ToSQL();
            if (!string.IsNullOrWhiteSpace(orderbyCondition))
            {
                stringBuilder.AppendFormat(" ORDER BY {0}", orderbyCondition);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>查询SQL副本</returns>
        public override object Clone()
        {
            var newObject = base.Clone() as SelectSqlStatement;
            if (SelectList != null)
                newObject.SelectList = SelectList.Clone() as SelectFieldListStatement;
            if (From != null)
                newObject.From = From.Clone() as From;
            if (JoinCondition != null)
                newObject.JoinCondition = JoinCondition.Clone() as JoinConditionStatement;
            if (OrderByCondition != null)
                newObject.OrderByCondition = OrderByCondition.Clone() as ConditionStatement;
            if (FilterCondition != null)
                newObject.FilterCondition = FilterCondition.Clone() as FilterConditionStatement;
            if (MainFromItem != null)
                newObject.MainFromItem = MainFromItem.Clone() as FromItem;

            newObject.AliasCount = AliasCount;
            newObject.DictFieldAliasMapping = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> map in DictFieldAliasMapping)
            {
                newObject.DictFieldAliasMapping.Add(map.Key, map.Value);
            }

            return newObject;
        }

        #endregion
    }
}
