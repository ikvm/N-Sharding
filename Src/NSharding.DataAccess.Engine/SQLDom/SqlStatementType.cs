using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL语句类型枚举
    /// </summary>
    /// <remarks>SQL语句类型枚举</remarks>
    public enum SqlStatementType
    {
        /// <summary>
        /// 插入
        /// </summary>
        Insert = 0,

        /// <summary> 
        /// 删除
        /// </summary>
        Delete = 1,

        /// <summary>
        /// 更新
        /// </summary>
        Update = 2,

        /// <summary> 
        /// 查询
        /// </summary>
        Select = 3,
    }
}
