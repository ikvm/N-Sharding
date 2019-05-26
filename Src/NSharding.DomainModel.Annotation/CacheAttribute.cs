using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Annotation
{
    /// <summary>
    /// 缓存选项
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// 缓存作用域
        /// </summary>
        public CacheScope Scope { get; set; }

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public long ExpiredTime { get; set; }

        public CacheAttribute(CacheScope scope, long expiredTime = 0)
        {
            this.Scope = scope;
            this.ExpiredTime = expiredTime;
        }
    }
}
