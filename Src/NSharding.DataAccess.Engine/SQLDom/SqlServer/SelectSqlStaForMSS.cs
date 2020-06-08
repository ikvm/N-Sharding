// ===============================================================================
// 浪潮GSP平台
// SQLServer数据库下的查询SQL语句
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/5/24 13:39:26        1.0        周国庆           初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/5/24 13:39:26 
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
    /// SQLServer数据库下的查询SQL语句
    /// </summary>
    /// <remarks> SQLServer数据库下的查询SQL语句</remarks>    
    [Serializable]
    internal class SelectSqlStaForMSS : SelectSqlStatement
    {

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectSqlStaForMSS()
            : base() { }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>Select SQL语句</returns>
        public override string ToSQL()
        {
            if (PageCount == 0)
            {
                return base.ToSQL();
            }
            else
            {
                //判断元数据是否被签出
                var stringBuilder = new StringBuilder();
                var orderbyCondition = OrderByCondition.ToSQL();
                var rowNumber = string.Format("ROW_NUMBER() Over({0}) as row_num", orderbyCondition);
                stringBuilder.Append(string.Format("SELECT * FROM (SELECT {0},{1} FROM {2}", SelectList.ToSQL(), rowNumber, From.ToSQL()));
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
                        stringBuilder.AppendFormat(" WHERE {0} ", filterCondition);
                    }
                    else
                    {
                        stringBuilder.AppendFormat(" AND {0} ", filterCondition);
                    }
                }
                if (PageCount > 0)
                {
                    stringBuilder.AppendFormat(") as tabledata WHERE row_num BETWEEN ({0}-1)*{1}+1 AND ({0}-1)*{1}+{1}", PageIndex, PageCount);                   
                }

                return stringBuilder.ToString();
            }
        }

        #endregion
    }
}
