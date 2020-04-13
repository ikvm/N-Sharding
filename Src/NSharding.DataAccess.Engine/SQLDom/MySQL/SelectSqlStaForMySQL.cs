using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    public class SelectSqlStaForMySQL : SelectSqlStatement
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectSqlStaForMySQL()
            : base() { }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>Select SQL语句</returns>
        public override string ToSQL()
        {
            if (this.PageIndex <= 0 && this.PageCount <= 0)
            {
                return base.ToSQL();
            }
            else
            {
                var startIndex = (this.PageIndex - 1) * this.PageCount;
                return string.Format("SELECT * FROM ({0}) A Limit {1},{2};", base.ToSQL(), startIndex, this.PageCount);
            }
        }

        #endregion
    }
}
