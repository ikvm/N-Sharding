using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Oracle数据库下的Update语句
    /// </summary>
    /// <remarks>Oracle数据库下的Update语句</remarks>
    [Serializable]
    public class UpdateSqlStaForORA : UpdateSqlStatement
    {
        #region 常量

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpdateSqlStaForORA()
            : base() { }

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            StringBuilder builder = new StringBuilder();
            string updateSql = string.Format("UPDATE {0} {1} SET {2}",
                this.TableName, this.TableCode, base.GetSetClause(this.UpdateFields, this.UpdateValues));
            builder.Append(updateSql);
            string whereCondition = this.GetUpdateConditionString();
            if (!IsBlankString(whereCondition))
                builder.AppendFormat(" WHERE {0}", whereCondition);

            return builder.ToString();
        }

        #endregion
    }
}
