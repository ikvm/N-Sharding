using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL语句工厂类
    /// </summary>
    class SQLStatementFactory
    {
        /// <summary>
        /// 根据类型构造SQL语句
        /// </summary>
        /// <param name="sqlStatementType">SQL语句类型</param>
        /// <param name="dbType">数据库类型</param>
        /// <returns>SQL语句</returns>
        public static SqlStatement CreateSqlStatement(SqlStatementType sqlStatementType, DbType dbType)
        {
            switch (sqlStatementType)
            {
                case SqlStatementType.Select:
                    switch (dbType)
                    {
                        case DbType.SQLServer:
                            return new SelectSqlStaForMSS();
                        case DbType.Oracle:
                            return new SelectSqlStaForORA();
                        default:
                            return new SelectSqlStatement();
                    }
                case SqlStatementType.Insert:
                    {
                        switch (dbType)
                        {
                            case DbType.SQLServer:
                                return new InsertSqlStaMSS();
                            case DbType.Oracle:
                                return new InsertSqlStaORA();
                            default:
                                return new InsertSqlStaMSS();
                        }
                    }
                case SqlStatementType.Delete:
                    return new DeleteSqlStatement();
                case SqlStatementType.Update:
                    switch (dbType)
                    {
                        case DbType.SQLServer:
                            return new UpdateSqlStaForMSS();
                        case DbType.Oracle:
                            return new UpdateSqlStaForORA();
                        default:
                            return new UpdateSqlStatement();
                    }
                default:
                    return null;
            }
        }
    }
}
