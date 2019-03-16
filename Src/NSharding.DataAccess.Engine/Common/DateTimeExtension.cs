using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 日期类型类扩展
    /// </summary>
    /// <remarks>对DateTime类型的扩展</remarks>
    public static class DateTimeExtension
    {
        #region 常量

        /// <summary>
        /// 日期类型的ISO8601格式常量
        /// </summary>
        public const string DATETIMEISO8601 = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

        #endregion

        #region 方法

        /// <summary>
        /// 将日期数据转换为ISO8601格式字符串。
        /// </summary>
        /// <param name="dateTime">日期数据</param>
        /// <returns>日期数据的ISO8601格式字符串。</returns>
        public static string ToStringByISO8601(this DateTime dateTime)
        {
            return dateTime.ToString(DATETIMEISO8601, System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        #endregion
    }
}
