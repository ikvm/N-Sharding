using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL语句类
    /// </summary>
    /// <remarks>SQL语句</remarks>
    [Serializable]
    public abstract class SqlStatement : SqlElement
    {
        #region 常量

        /// <summary>
        /// SqlStatement 
        /// </summary>
        public const string SQLSTATEMENT = "SqlStatement";

        /// <summary> 
        /// TableName 
        /// </summary>
        public const string TABLENAME = "TableName";

        /// <summary> 
        /// TableCode 
        /// </summary>
        public const string TABLECODE = "TableCode";

        /// <summary> 
        /// CommonObjectID
        /// </summary>
        public const string COMMONOBJECTID = "CommonObjectID";

        /// <summary> 
        /// NodeObjectID 
        /// </summary>
        public const string NODEOBJECTID = "NodeObjectID";

        /// <summary> 
        /// RequestTokenID
        /// </summary>
        public const string REQUESTTOKENID = "RequestTokenID";

        /// <summary> 
        /// ModelVersion 
        /// </summary>
        public const string COMMONOBJECTVERSION = "ModelVersion";

        /// <summary>
        /// DataTableVersion 
        /// </summary>
        public const string NODEOBJECTVERSION = "DataTableVersion";

        #endregion

        #region 字段


        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlStatement()
            : base()
        {
            PrimaryKeys = new SqlPrimaryKey();
            SqlBuildingInfo = new SqlBuildingInfo();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        private string tableCode;

        /// <summary>
        /// 表编号
        /// </summary>
        public string TableCode
        {
            get
            {
                if (string.IsNullOrEmpty(tableCode))
                    return TableName;

                return tableCode;
            }
            set
            {
                tableCode = value;
            }
        }

        /// <summary>
        /// 主键列表
        /// </summary>
        public SqlPrimaryKey PrimaryKeys { get; set; }

        private SqlBuildingInfo sqlBuildingInfo;

        /// <summary>
        /// SQL语句构造中用到的中间变量
        /// </summary>
        public SqlBuildingInfo SqlBuildingInfo
        {
            get
            {
                return sqlBuildingInfo;
            }
            set
            {
                sqlBuildingInfo = value;
            }
        }

        /// <summary>
        /// 通用中间对象ID
        /// </summary>
        public string CommonObjectID { get; set; }

        /// <summary>
        /// 请求的唯一标识
        /// </summary>
        public string RequestTokenID { get; set; }

        /// <summary>
        /// 通用中间对象对应的元数据版本
        /// </summary>
        public string CommonObjectVersion { get; set; }

        /// <summary>
        /// 节点对象ID
        /// </summary>
        private string nodeID;

        /// <summary>
        /// 节点对象ID
        /// </summary>
        public string NodeID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(nodeID))
                    return this.SqlBuildingInfo.CurrentNode.ID;
                else
                    return nodeID;
            }
            set
            {
                nodeID = value;
            }
        }

        /// <summary>
        /// 节点对象的版本
        /// </summary>
        public string NodeVersion { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>SQL语句类副本</returns>
        public override object Clone()
        {
            SqlStatement sqlStatement = base.Clone() as SqlStatement;
            if (PrimaryKeys != null)
                sqlStatement.PrimaryKeys = PrimaryKeys.Clone() as SqlPrimaryKey;

            sqlStatement.SqlBuildingInfo = SqlBuildingInfo.Clone() as SqlBuildingInfo;

            return sqlStatement;
        }

        #region 序列化

        public override void ToXml(SqlElement sqlElement, XmlElement xmlParent)
        {
            base.ToXml(sqlElement, xmlParent);

            SqlStatement sqlStatement = sqlElement as SqlStatement;
            XmlElement xmlTableName = SerializerUtil.AddElement(xmlParent, TABLENAME, sqlStatement.TableName);
            XmlElement xmlTableCode = SerializerUtil.AddElement(xmlParent, TABLECODE, sqlStatement.TableCode);
            XmlElement xmlCOID = SerializerUtil.AddElement(xmlParent, COMMONOBJECTID, sqlStatement.CommonObjectID);
            XmlElement xmlNodeObjectID = SerializerUtil.AddElement(xmlParent, NODEOBJECTID, sqlStatement.NodeID);
            XmlElement xmlCOVersion = SerializerUtil.AddElement(xmlParent, COMMONOBJECTVERSION, sqlStatement.CommonObjectVersion);
            XmlElement xmlNodeObjectVersion = SerializerUtil.AddElement(xmlParent, NODEOBJECTVERSION, sqlStatement.NodeVersion);

            XmlElement xmlPrimaryKeyField = SerializerUtil.AddElement(xmlParent, SqlPrimaryKey.SQLPRIMARYKEY);
            sqlStatement.PrimaryKeys.ToXml(sqlStatement.PrimaryKeys, xmlPrimaryKeyField);
        }

        public override void FromXml(SqlElement sqlNode, XmlElement xmlParent, XmlNamespaceManager xnm)
        {
            base.FromXml(sqlNode, xmlParent, xnm);

            SqlStatement sqlStatement = sqlNode as SqlStatement;
            ParserUtil util = new ParserUtil(xnm);

            XmlElement xmlTableName = util.Child(xmlParent, TABLENAME);
            XmlElement xmlTableCode = util.Child(xmlParent, TABLECODE);
            XmlElement xmlCOID = util.Child(xmlParent, COMMONOBJECTID);
            XmlElement xmlNodeObjectID = util.Child(xmlParent, NODEOBJECTID);
            XmlElement xmlCOVersion = util.Child(xmlParent, COMMONOBJECTVERSION);
            XmlElement xmlNodeObjectVersion = util.Child(xmlParent, NODEOBJECTVERSION);

            sqlStatement.TableName = xmlTableName.InnerText;
            sqlStatement.TableCode = xmlTableCode.InnerText;
            sqlStatement.CommonObjectID = xmlCOID.InnerText;
            sqlStatement.NodeID = xmlNodeObjectID.InnerText;
            //sqlStatement.CommonObjectVersion = xmlCOVersion.InnerText;
            //sqlStatement.NodeVersion = xmlNodeObjectVersion.InnerText;

            XmlElement xmlPrimaryKeyField = util.Child(xmlParent, SqlPrimaryKey.SQLPRIMARYKEY);
            sqlStatement.PrimaryKeys.FromXml(sqlStatement.PrimaryKeys, xmlPrimaryKeyField, xnm);
        }

        #endregion 序列化

        #endregion
    }
}
