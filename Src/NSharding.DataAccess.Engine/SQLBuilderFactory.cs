using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQLBuilder工厂类
    /// </summary>
    class SQLBuilderFactory
    {
        public static ISQLBuilder CreateSQLBuilder()
        {
            return SQLBuilderImpl.GetInstance();
        }

        public static ISQLBuilder CreateSQLBuilder(DomainModel.Spi.DomainModel domainModel)
        {
            var mainDbType = domainModel.RootDomainObject.DataObject.DataSource.DbType;

            switch (mainDbType)
            {
                case DbType.ES:
                    return new ESSQLBuilder();
                case DbType.SQLServer:
                case DbType.MySQL:
                case DbType.Oracle:
                    return CreateSQLBuilder();
                default:
                    return CreateSQLBuilder();
            }
        }
    }
}
