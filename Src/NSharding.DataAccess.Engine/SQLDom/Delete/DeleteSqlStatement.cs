using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Delete删除SQL语句类
    /// </summary>
    /// <remarks>Delete删除SQL语句</remarks>
    public class DeleteSqlStatement : SqlStatement
    {
        #region 常量

        /// <summary> 
        /// DeleteSqlStatement 
        /// </summary>
        public const string DELETESQLSTATEMENT = "DeleteSqlStatement";

        /// <summary> 
        /// SubQuerySql 
        /// </summary>
        public const string SUBQUERYSQL = "SubQuerySql";

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeleteSqlStatement()
            : base()
        {
            this.Condition = new ConditionStatement();
            this.Conditions = new FilterConditionStatement();
            this.SubQuerySql = null;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 删除语句中动态条件
        /// </summary>
        public ConditionStatement Condition { get; set; }

        /// <summary>
        /// 删除语句中动态条件
        /// </summary>
        public FilterConditionStatement Conditions { get; set; }

        /// <summary>
        /// 删除表中数据时使用的子查询。
        /// </summary>
        /// <remarks>
        /// 默认为null，因为删除主表数据，不需要子查询。通过是否为null，判断是否形成SQL。
        /// </remarks>
        public SelectSqlForSubQuery SubQuerySql { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>Delete删除SQL语句</returns>
        public override object Clone()
        {
            DeleteSqlStatement newObject = base.Clone() as DeleteSqlStatement;
            if (Condition != null)
                newObject.Condition = Condition.Clone() as ConditionStatement;
            if (SubQuerySql != null)
                newObject.SubQuerySql = SubQuerySql.Clone() as SelectSqlForSubQuery;

            return newObject;
        }

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>SQL语句</returns>
        public override string ToSQL()
        {
            string whereCondition = this.GetDeleteConditionString();
            if (IsBlankString(whereCondition))
                return string.Format("DELETE FROM {0} ", this.TableName);
            return string.Format("DELETE FROM {0} WHERE {1} ", this.TableName, whereCondition);
        }

        /// <summary>
        /// 获取删除SQL中的Where条件后的部分。
        /// </summary>
        /// <returns>Where条件后的部分。</returns>
        public string GetDeleteConditionString()
        {
            var subQuerySqlCondition = string.Empty;
            if (this.SubQuerySql != null)
                subQuerySqlCondition = string.Format("EXISTS ({0})", this.SubQuerySql.ToSQL());

            string filterCondition = string.Empty;

            if (Conditions != null)
            {
                filterCondition = this.Conditions.ToSQL();
                if (!string.IsNullOrWhiteSpace(subQuerySqlCondition))
                {
                    if (!string.IsNullOrWhiteSpace(filterCondition))
                        return string.Format("{0} AND {1}", filterCondition, subQuerySqlCondition);
                    else
                        return subQuerySqlCondition;
                }
                else
                {
                    return filterCondition;
                }
            }
            else
            {
                return subQuerySqlCondition;
            }
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            DeleteSqlStatement deleteSql = sqlElement as DeleteSqlStatement;
            XmlElement xmlCondition = SerializerUtil.AddElement(xmlParent, ConditionStatement.CONDITIONSTATEMENT);
            deleteSql.Condition.ToXml(deleteSql.Condition, xmlCondition);
            if (deleteSql.SubQuerySql != null)// SubQuerySql 可以为空
            {
                XmlElement xmlSubQuerySql = SerializerUtil.AddElement(xmlParent, SUBQUERYSQL);
                deleteSql.SubQuerySql.ToXml(deleteSql.SubQuerySql, xmlSubQuerySql);
            }
        }

        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            DeleteSqlStatement deleteSql = sqlElement as DeleteSqlStatement;
            ParserUtil util = new ParserUtil(xnm);
            XmlElement xmlCondition = util.Child(xmlParent, ConditionStatement.CONDITIONSTATEMENT);
            deleteSql.Condition.FromXml(deleteSql.Condition, xmlCondition, xnm);
            XmlElement xmlSubQuerySql = util.Child(xmlParent, SUBQUERYSQL);
            if (xmlSubQuerySql != null)
            {
                deleteSql.SubQuerySql = new SelectSqlForSubQuery();
                deleteSql.SubQuerySql.FromXml(deleteSql.SubQuerySql, xmlSubQuerySql, xnm);
            }
        }

        #endregion 序列化

        #endregion
    }
}