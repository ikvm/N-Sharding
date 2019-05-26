using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Spi
{
    /// <summary>
    /// 缓存作用域
    /// </summary>
    public enum CacheScope
    {
        /// <summary>
        /// 会话级
        /// </summary>
        Session,

        /// <summary>
        /// 进程级
        /// </summary>
        Application,

        /// <summary>
        /// 全局分布式缓存
        /// </summary>
        GLobal
    }
}
