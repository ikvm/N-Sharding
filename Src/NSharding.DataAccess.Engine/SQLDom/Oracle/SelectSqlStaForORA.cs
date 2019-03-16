using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Oracle数据库下的查询SQL语句
    /// </summary>
    /// <remarks>Oracle数据库下的查询SQL语句</remarks>
    internal class SelectSqlStaForORA : SelectSqlStatement
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectSqlStaForORA()
            : base() { }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>Select SQL语句</returns>
        public override string ToSQL()
        {
            if (this.TopSize < 0)
            {
                return base.ToSQL();
            }
            else
            {
                return string.Format("SELECT * FROM ( {0} ) WHERE ROWNUM <= {1}", base.ToSQL(), this.TopSize);
            }
        }

        #endregion
    }
}
