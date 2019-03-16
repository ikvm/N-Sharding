using System;
using System.Linq;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// And运算符
    /// </summary>
    /// <remarks>And运算符</remarks>
    internal class And
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
        /// and字句
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string Clause(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return string.Empty;

            return string.Format(" And {0}", condition);
        }

        #endregion
    }
}
