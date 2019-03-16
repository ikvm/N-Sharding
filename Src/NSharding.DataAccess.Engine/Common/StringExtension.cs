using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// String类的扩展方法类
    /// </summary>
    /// <remarks>提供对String类的扩展</remarks>
    public static class StringExtension
    {
        /// <summary>
        /// 字符串两边添加单引号
        /// </summary>
        /// <param name="variable">字符串</param>
        /// <returns>两边添加单引号后的字符串</returns>
        public static string AddQuote(this string variable)
        {
            return variable != null ? string.Format("'{0}'", variable) : "NULL";
        }
    }
}
