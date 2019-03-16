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
    }
}
