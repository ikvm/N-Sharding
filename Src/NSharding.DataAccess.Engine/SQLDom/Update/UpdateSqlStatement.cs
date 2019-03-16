// ===============================================================================
// 浪潮GSP平台
// Update SQL语句类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/14 11:37:59        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/14 11:37:59 
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
    /// Update SQL语句类
    /// </summary>
    /// <remarks>Update SQL语句</remarks>
    public class UpdateSqlStatement : SqlStatement
    {
        #region 常量

        /// <summary>
        /// UpdateSqlStatement 
        /// </summary>
        public const string UPDATESQLSTATEMENT = "UpdateSqlStatement";

        /// <summary> 
        /// UpdateCondition 
        /// </summary>
        public const string UPDATECONDITION = "UpdateCondition";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpdateSqlStatement()
            : base()
        {
            this.UpdateFields = new UpdateFieldList();
            this.UpdateValues = new UpdateValueList();
            UpdateCondition = new FilterConditionStatement();
        }

        #endregion

        #region 属性

        /// <summary>
        /// Update语句中要更新的字段，考虑乐观锁机制
        /// </summary>
        public UpdateFieldList UpdateFields { get; set; }

        /// <summary>
        /// 插入值列表（默认6个字段）
        /// </summary>
        public UpdateValueList UpdateValues { get; set; }

        /// <summary>
        /// Update语句中用到的更新条件，考虑乐观锁机制
        /// </summary>
        public FilterConditionStatement UpdateCondition { get; set; }

        /// <summary>
        /// 更新表中数据时使用的子查询。
        /// </summary>
        /// <remarks>
        /// 默认为null。通过是否为null，判断是否形成SQL。
        /// 目前是为了支持逻辑删除。
        /// </remarks>
        public SelectSqlForSubQuery SubQuerySql { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>Update SQL语句</returns>
        public override object Clone()
        {
            var newSql = base.Clone() as UpdateSqlStatement;
            if (this.UpdateFields != null)
                newSql.UpdateFields = this.UpdateFields.Clone() as UpdateFieldList;
            if (this.UpdateValues != null)
                newSql.UpdateValues = this.UpdateValues.Clone() as UpdateValueList;
            if (this.UpdateCondition != null)
                newSql.UpdateCondition = this.UpdateCondition.Clone() as FilterConditionStatement;
            if (this.SubQuerySql != null)
                newSql.SubQuerySql = this.SubQuerySql.Clone() as SelectSqlForSubQuery;

            return newSql;
        }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            string updateSql = string.Empty;
#warning 从缓存中获取已经缓存的SQL
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(updateSql))
            {
                updateSql = string.Format("UPDATE {0} SET {1}", this.TableName, GetSetClause(this.UpdateFields, this.UpdateValues));

#warning 缓存SQL
                //SqlStatementManager.Instance.SaveSqlStringByCache(ModelID, AbstractObjectID, queryType.ToString(), updateSql);
            }
            builder.Append(updateSql);

            string whereCondition = this.GetUpdateConditionString();
            if (!IsBlankString(whereCondition))
                builder.AppendFormat(" WHERE {0}", whereCondition);

            return builder.ToString();
        }

        /// <summary>
        /// 构造Set字句
        /// </summary>
        /// <param name="updateFieldList">更新字段列表</param>
        /// <param name="updateValueList">更新值列表</param>
        /// <returns>Set字句</returns>
        protected string GetSetClause(UpdateFieldList updateFieldList, UpdateValueList updateValueList)
        {
            StringBuilder sqlFuncStr = new StringBuilder();
            IList<UpdateField> removeFields = new List<UpdateField>();
            IList<SqlElement> removeValues = new List<SqlElement>();
            for (int i = 0; i < updateValueList.ChildCollection.Count; i++)
            {
                UpdateField field = updateFieldList.ChildCollection[i] as UpdateField;
                if (field.IsUseVarBinding) continue;
                string val = ((UpdateValue)updateValueList.ChildCollection[i]).Value as string;
                if (val != null)
                {
                    sqlFuncStr.AppendFormat("{0}={1},", field.FieldName, val);
                    removeFields.Add(field);
                    removeValues.Add(updateValueList.ChildCollection[i]);
                }
            }

            foreach (UpdateField i in removeFields)
            {
                updateFieldList.ChildCollection.Remove(i);
            }
            foreach (UpdateValue i in removeValues)
            {
                updateValueList.ChildCollection.Remove(i);
            }

            sqlFuncStr.Append(updateFieldList.ToSQL());

            return sqlFuncStr.ToString();
        }

        /// <summary>
        /// 获取UpdateSql中的Where条件后的部分。
        /// </summary>
        /// <returns>Where条件后的部分。</returns>
        public string GetUpdateConditionString()
        {
            var updateCondition = "1=1";
            if (UpdateCondition != null)
            {
                updateCondition = this.UpdateCondition.ToSQL();
            }
            if (this.SubQuerySql == null)
                return updateCondition;
            string subQuerySql = string.Format(" EXISTS ({0}) ", this.SubQuerySql.ToSQL());

            return IsBlankString(updateCondition) ? subQuerySql : this.UpdateCondition + " AND " + subQuerySql;
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            UpdateSqlStatement updateSql = sqlElement as UpdateSqlStatement;
            XmlElement xmlUpdateFields = SerializerUtil.AddElement(xmlParent, UpdateFieldList.UPDATEFIELDLIST);
            updateSql.UpdateFields.ToXml(updateSql.UpdateFields, xmlUpdateFields);

            XmlElement xmlUpdateCondition = SerializerUtil.AddElement(xmlParent, UPDATECONDITION);
            updateSql.UpdateCondition.ToXml(updateSql.UpdateCondition, xmlUpdateCondition);
        }

        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            UpdateSqlStatement updateSql = sqlElement as UpdateSqlStatement;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlUpdateFields = util.Child(xmlParent, UpdateFieldList.UPDATEFIELDLIST);
            updateSql.UpdateFields.FromXml(updateSql.UpdateFields, xmlUpdateFields, xnm);

            XmlElement xmlUpdateCondition = util.Child(xmlParent, UPDATECONDITION);
            updateSql.UpdateCondition.FromXml(updateSql.UpdateCondition, xmlUpdateCondition, xnm);
        }

        #endregion 序列化

        #endregion
    }
}