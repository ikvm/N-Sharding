using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 键值对条件类
    /// </summary>
    /// <remarks>键值对条件类</remarks>
    internal class KeyValueConditionStatement<T> : ConditionStatement
    {
        #region 属性

        private Field field;

        /// <summary>
        /// 键对应的字段
        /// </summary>
        public Field Field
        {
            get
            {
                if (field == null)
                    field = new Field();
                return field;
            }
            set
            {
                field = value;
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            var sql = Field.ToSQL();
            return string.Format("{0}=:{0}", sql);

            var dataType = typeof(T);
            if (dataType == typeof(int) || dataType == typeof(decimal) ||
               dataType == typeof(float) || dataType == typeof(double) || dataType == typeof(long))
            {
                return string.Format("{0}={1}", Field.ToSQL(), Value);
            }
            else
            {
                return string.Format("{0}='{1}'", Field.ToSQL(), Value);
            }
        }

        #endregion
    }
}
