// ===============================================================================
// 浪潮GSP平台
// Or 运算符
// 请查看《GSP7-******子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/11/27 15:05:04        1.0        zhougq           添加头注释。
// ===============================================================================
// 开发者: zhougq
// 2013/11/27 15:05:04 
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
    /// Or 运算符
    /// </summary>
    /// <remarks>Or 运算符</remarks>
    internal class Or
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
        /// Or 字句
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string Clause(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return string.Empty;

            return string.Format(" Or {0}", condition);
        }

        #endregion
    }
}
