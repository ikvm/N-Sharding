using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Insert语句中的插入字段类
    /// </summary>
    /// <remarks>Insert语句中的插入字段类</remarks>
    public class InsertField : Field
    {
        #region 常量

        /// <summary>
        /// Insert语句中的插入字段常量
        /// </summary>
        public const string INSERTFIELD = "InsertField";

        #endregion

        /// <summary>
        /// 是否使用绑定变量
        /// </summary>
        public bool IsUseVarBinding { get; set; }

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            return FieldName;
        }

        #endregion
    }
}