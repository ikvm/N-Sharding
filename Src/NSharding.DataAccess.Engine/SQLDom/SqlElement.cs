// ===============================================================================
// 浪潮GSP平台
// SQL元素类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/28 22:38:55        1.0        周国庆         初稿。
// 2013/1/29 09:36:00        1.1        周国庆          基本实现。
// ===============================================================================
// 开发者: 周国庆
// 2013/1/28 22:38:55 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL元素类
    /// </summary>
    /// <remarks>SQL语句中的最基本粒度实体类</remarks>
    [Serializable]
    public abstract class SqlElement : ICloneable
    {
        #region 常量

        /// <summary>
        /// ID
        /// </summary>
        public const string ID = "ID";

        /// <summary>
        /// Version
        /// </summary>
        public const string VERSION = "Version";

        /// <summary>
        /// SQLELEMENT
        /// </summary>
        public const string SQLELEMENT = "SqlElement";

        /// <summary>
        /// CLASSFULLNAME
        /// </summary>
        public const string CLASSFULLNAME = "ClassFullName";

        /// <summary>
        /// SQLELEMENTTYPE
        /// </summary>
        public const string SQLELEMENTTYPE = "Type";

        /// <summary>
        /// CHILDCOLLECTION
        /// </summary>
        public const string CHILDCOLLECTION = "ChildCollection";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlElement()
        {
            //Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 子结点
        /// </summary>
        private SqlElementCollection childCollection;

        /// <summary>
        /// 子结点。
        /// </summary>
        public virtual SqlElementCollection ChildCollection
        {
            get
            {
                if (childCollection == null)
                    childCollection = new SqlElementCollection();

                return this.childCollection;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL，返回SQL的一部分
        /// </summary>
        /// <returns>转换成的SQL</returns>
        public abstract string ToSQL();

        /// <summary>
        /// 创建子对象集合。
        /// </summary>
        protected void CreateChildCollection()
        {
            this.childCollection = new SqlElementCollection();
        }

        /// <summary>
        /// 检查是否为空串。
        /// </summary>
        /// <param name="variable">输入的字符串。</param>
        /// <returns>是否为空串的bool值。</returns>
        protected static bool IsBlankString(string variable)
        {
            return string.IsNullOrWhiteSpace(variable);
        }

        /// <summary>
        /// 为字符串两边添加单引号。
        /// </summary>
        /// <param name="variable">输入的字符串。</param>
        /// <returns>添加单引号后的字符串</returns>
        protected static string QuotedString(string variable)
        {
            return variable != null ? string.Format("'{0}'", variable) : "NULL";
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>Sql元素</returns>
        public virtual object Clone()
        {
            SqlElement newSqlElement = this.MemberwiseClone() as SqlElement;
            if (childCollection != null)
                newSqlElement.childCollection = this.childCollection.Clone() as SqlElementCollection;

            return newSqlElement;
        }

        /// <summary>
        /// 将SqlElement的属性写入XmlElement。
        /// </summary>
        /// <param name="sqlNode">SqlElement类的对象（或继承自SqlElement类的对象）</param>
        /// <param name="xmlParent">XmlDocument中的元素</param>
        public virtual void ToXml(SqlElement sqlNode, XmlElement xmlParent)
        {
            //XmlElement xmlId = SerializerUtil.AddElement(xmlParent, ID, sqlNode.Id);
            //XmlElement xmlVersion = SerializerUtil.AddElement(xmlParent, VERSION, sqlNode.Version);
            if (sqlNode.ChildCollection != null)
            {
                XmlElement xmlchildList = SerializerUtil.AddElement(xmlParent, CHILDCOLLECTION);
                foreach (SqlElement childNode in sqlNode.ChildCollection)
                {
                    XmlElement childNodeElement = SerializerUtil.AddElement(xmlchildList, SQLELEMENT);

                    /*
                     * FullName：一个包含 Type 的完全限定名的字符串，其中包括 Type 的命名空间但不包括程序集；
                     * 例如，C# 字符串类型的完全限定名为 System.String。
                     * */
                    //childNodeElement.SetAttribute(CLASSFULLNAME, childNode.GetType().FullName);
                    childNodeElement.SetAttribute(SQLELEMENTTYPE, SqlElementFactory.GetSqlElementType(childNode).ToString());
                    childNode.ToXml(childNode, childNodeElement);
                }
            }
        }

        /// <summary>
        /// 用XmlElement的信息构造SqlElement类的对象（或继承自SqlElement类的对象）。
        /// </summary>
        /// <param name="sqlNode">SqlElement类的对象（或继承自SqlElement类的对象）</param>
        /// <param name="xmlParent">XmlDocument中的元素</param>
        /// <param name="xnm">集合命名空间范围管理类的对象</param>
        public virtual void FromXml(SqlElement sqlNode, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            ParserUtil util = new ParserUtil(xnm);
            XmlElement xmlId = util.Child(xmlParent, ID);
            XmlElement xmlVersion = util.Child(xmlParent, VERSION);

            //sqlNode.Id = xmlId.InnerText;
            //sqlNode.Version = xmlVersion.InnerText;

            XmlElement xmlChildList = util.Child(xmlParent, CHILDCOLLECTION);
            if (xmlChildList != null)
            {
                XmlNodeList childNodeList = util.Children(xmlChildList, SQLELEMENT);
                if (childNodeList != null)
                {
                    sqlNode.CreateChildCollection();
                    foreach (XmlElement xmlChildNode in childNodeList)
                    {
                        //// 通过反射构造对象。                        
                        //string fullname = util.AttrStr(xmlChildNode, CLASSFULLNAME);
                        //Assembly ass = Assembly.GetExecutingAssembly();
                        //SqlElement node = ass.CreateInstance(fullname) as SqlElement;

                        int sqlElementType = util.AttrInt(xmlChildNode, SQLELEMENTTYPE);
                        SqlElement node = SqlElementFactory.GetSQLElement(sqlElementType);
                        node.FromXml(node, xmlChildNode, xnm);
                        sqlNode.ChildCollection.Add(node);
                    }
                }
            }
        }
        #endregion
    }
}
