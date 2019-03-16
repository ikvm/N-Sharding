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
    }
}
