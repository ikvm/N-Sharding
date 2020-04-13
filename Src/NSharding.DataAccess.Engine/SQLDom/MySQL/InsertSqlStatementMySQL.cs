using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    class InsertSqlStaMySQL : InsertSqlStatement
    {
        #region 构造函数

        public InsertSqlStaMySQL()
            : base()
        {
        }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>Insert插入语句</returns>
        public override object Clone()
        {
            var sql = base.Clone() as InsertSqlStaMySQL;
            if (InsertFields != null)
                sql.InsertFields = InsertFields.Clone() as InsertFieldList;
            if (InsertValues != null)
                sql.InsertValues = InsertValues.Clone() as InsertValueList;

            return sql;
        }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            string insertSQL = string.Empty;
            //先从内存缓存中获取
            //insertSQL = SQLStatementManager.Instance.
            //    GetSqlStringByCache(CommonObjectID, NodeID, TableName, "ForInsert", RequestTokenID);

            if (string.IsNullOrWhiteSpace(insertSQL))
            {
                insertSQL = string.Format("INSERT INTO {0} {1}", this.TableName, InsertFields.ToSQL().Replace("\"", ""));
                // 将解析好的SQL插入缓存中
                //SQLStatementManager.Instance.
                //    SaveSqlStringToCache(CommonObjectID, NodeID, TableName, queryType.ToString(), RequestTokenID, insertSQL);
            }

            return insertSQL;
        }

        #endregion
    }
}
