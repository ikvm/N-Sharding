using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    /// <summary>
    /// 关系运算符
    /// </summary>
    public enum RelationalOperator
    {
        /// <summary>
        /// =
        /// </summary>
        Equal,

        /// <summary>
        /// <>
        /// </summary>
        NotEqual,

        /// <summary>
        /// <
        /// </summary>
        LessThan,

        /// <summary>
        /// >
        /// </summary>
        GreaterThan,

        /// <summary>
        /// >=
        /// </summary>
        GreaterEqualThan,

        /// <summary>
        /// <=
        /// </summary>
        LessEqualThan,

        /// <summary>
        /// Like
        /// </summary>
        LIKE,

        /// <summary>
        /// EndLike
        /// </summary>
        EndLike,

        /// <summary>
        /// StartLike
        /// </summary>
        StartLike,

        /// <summary>
        /// In
        /// </summary>
        IN,

        /// <summary>
        /// Between
        /// </summary>
        BETWEEN,

        /// <summary>
        /// Exists
        /// </summary>
        Exists
    }
}
