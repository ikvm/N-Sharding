using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Insert插入语句类
    /// </summary>
    /// <remarks>Insert插入语句</remarks>
    public abstract class InsertSqlStatement : SqlStatement
    {
        #region 常量

        /// <summary>
        /// InsertSqlStatement
        /// </summary>
        public const string INSERTSQLSTATEMENT = "InsertSqlStatement";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        public InsertSqlStatement()
            : base()
        {
            InsertFields = new InsertFieldList();
            InsertValues = new InsertValueList();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 缓存的SQL语句是否可用
        /// </summary>
        public bool SqlCacheAvailability { get; set; }

        /// <summary>
        /// 插入字段列表（默认6个字段）
        /// </summary>
        public InsertFieldList InsertFields { get; set; }

        /// <summary>
        /// 插入值列表（默认6个字段）
        /// </summary>
        public InsertValueList InsertValues { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>Insert插入语句</returns>
        public override object Clone()
        {
            var sql = base.Clone() as InsertSqlStatement;
            if (InsertFields != null)
                sql.InsertFields = InsertFields.Clone() as InsertFieldList;
            if (InsertValues != null)
                sql.InsertValues = InsertValues.Clone() as InsertValueList;

            return sql;
        }

        /// <summary>
        /// 将SQL语句中最后修改时间和创建时间字段值替换为获取数据库时间函数。
        /// </summary>
        /// <param name="insertFieldList">插入的字段</param>
        /// <param name="insertValueList">插入的字段值</param>
        /// <returns>替换最后修改时间和创建时间后的SQL语句</returns>
        private string GetFieldValueClause(InsertFieldList insertFieldList, InsertValueList insertValueList)
        {
            StringBuilder insertFields = new StringBuilder();
            StringBuilder insertValues = new StringBuilder();
            List<InsertField> removeFields = new List<InsertField>();
            List<SqlElement> removeValues = new List<SqlElement>();
            for (int i = 0; i < insertValueList.ChildCollection.Count; i++)
            {
                InsertField field = insertFieldList.ChildCollection[i] as InsertField;
                string val = ((InsertValue)insertValueList.ChildCollection[i]).Value as string;
                if (val != null)
                {
                    //将最后修改时间和创建时间两个字段的值替换为获取数据库时间的方法
                    if (val == SQLBuilderUtils.CREATETIME || val == SQLBuilderUtils.LASTCHANGEDTIME)
                    {
                        insertFields.AppendFormat("{0},", field.FieldName);
                        removeFields.Add(field);
                        removeValues.Add(insertValueList.ChildCollection[i]);
                        insertValues.AppendFormat("{0},", SQLBuilderUtils.GetReallyDbDateTime());
                    }
                    else
                    {
                        insertFields.AppendFormat("{0},", field.FieldName);
                        insertValues.AppendFormat(":{0},", field.FieldName);
                    }
                }
                //对于一些如Byte[]类型的参数，无法转换为string类型，将其直接加到insertFields和insertValues中
                else
                {
                    insertFields.AppendFormat("{0},", field.FieldName);
                    insertValues.AppendFormat(":{0},", field.FieldName);
                }
            }

            //最后修改时间和创建时间两个字段已完成参数替换，因此将它们从insertFieldList中删除
            foreach (var field in removeFields)
            {
                insertFieldList.ChildCollection.Remove(field);
            }
            //最后修改时间和创建时间两个字段已完成参数替换，因此将它们从insertValueList中删除
            foreach (var value in removeValues)
            {
                insertValueList.ChildCollection.Remove(value);
            }
            //去换字符串中最后的那个“，”
            if (insertFields.ToString().EndsWith(","))
            {
                insertFields.Remove(insertFields.Length - 1, 1);
                insertValues.Remove(insertValues.Length - 1, 1);
            }

            //此时返回的string中有最后修改时间和创建时间两个字段和值，
            //因为它们没有在insertFields和insertValues中被删除
            return string.Format("({0})VALUES({1})", insertFields, insertValues);
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

            InsertSqlStaMSS insertSql = sqlElement as InsertSqlStaMSS;
            XmlElement xmlInsertFields = SerializerUtil.AddElement(xmlParent, InsertFieldList.INSERTFIELDLIST);
            insertSql.InsertFields.ToXml(insertSql.InsertFields, xmlInsertFields);

            //不需要对FieldValue序列化
            //XmlElement xmlInsertValues = SerializerUtil.AddElement(xmlParent, InsertValueList.INSERTVALUELIST);
            //insertSql.InsertValues.ToXml(insertSql.InsertValues, xmlInsertValues);
        }

        /// <summary>
        /// 转换成Insert语句
        /// </summary>
        /// <param name="sqlElement">SQL元素</param>
        /// <param name="xmlParent">XML父节点元素</param>
        /// <param name="xnm">集合命名空间范围管理类的对象</param>
        public override void FromXml(SqlElement sqlElement, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlElement, xmlParent, xnm);

            var xnm2 = new XmlNamespaceManager(xmlParent.OwnerDocument.NameTable);
            InsertSqlStaMSS insertSql = sqlElement as InsertSqlStaMSS;
            ParserUtil util = new ParserUtil(xnm);
            XmlElement xmlInsertFields = util.Child(xmlParent, InsertFieldList.INSERTFIELDLIST);
            //XmlElement xmlInsertValues = util.Child(xmlParent, InsertValueList.INSERTVALUELIST);
            insertSql.InsertFields.FromXml(insertSql.InsertFields, xmlInsertFields, xnm);
            //insertSql.InsertValues.FromXml(insertSql.InsertValues, xmlInsertValues, xnm);
        }

        #endregion 序列化

        #endregion
    }
}
