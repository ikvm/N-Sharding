// ===============================================================================
// 浪潮GSP平台
// SQLServer数据库下的Update语句
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/5/24 14:48:01        1.0        周国庆           初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/5/24 14:48:01 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQLServer数据库下的Update语句
    /// </summary>
    /// <remarks>SQLServer数据库下的Update语句</remarks>
    [Serializable]
    public class UpdateSqlStaForMSS : UpdateSqlStatement
    {
        #region 常量

        #endregion

        #region 字段

        #endregion

        #region 构造函数

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
            string updateSql = string.Format("UPDATE {0} SET {1} FROM {0} {2}",
                this.TableName, base.GetSetClause(this.UpdateFields, this.UpdateValues), this.TableCode);
            builder.Append(updateSql);
            string whereCondition = this.GetUpdateConditionString();
            if (!IsBlankString(whereCondition))
                builder.AppendFormat(" WHERE {0}", whereCondition);

            return builder.ToString();
        }

        #endregion
    }
}
