using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据访问接口适配器工厂
    /// </summary>
    class DatabaseFactory
    {
        public static IDatabase CreateDefaultDatabase()
        {
            return new DatabaseImpl();
        }

        public static IDatabase CreateDatabase(DomainModel.Spi.DomainModel domainModel)
        {
            var mainDbType = domainModel.RootDomainObject.DataObject.DataSource.DbType;

            switch (mainDbType)
            {
                //case Metadata.Database.DbType.ES:
                //    return new ESDatabase();
                case DbType.SQLServer:
                case DbType.MySQL:
                case DbType.Oracle:
                    return CreateDefaultDatabase();
                default:
                    return CreateDefaultDatabase();
            }
        }
    }
}
