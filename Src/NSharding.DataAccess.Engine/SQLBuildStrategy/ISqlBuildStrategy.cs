using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL构造策略接口
    /// </summary>
    interface ISqlBuildStrategy
    {
        /// <summary>
        /// 构造不包含数据的Sql（即SqlSchema）。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        SqlStatementCollection BuildTableSqlSchema(SqlBuildingContext context);

        /// <summary>
        /// 在SqlSchema基础上，构造包含数据的Sql。
        /// </summary>
        /// <param name="sqls">Sql语句对象集合。</param>
        /// <param name="context">Sql构造的上下文信息。</param>
        void BuildTableSqlDetail(SqlStatementCollection sqls, SqlBuildingContext context);
    }
}
