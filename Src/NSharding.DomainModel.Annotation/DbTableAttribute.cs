using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NSharding.DomainModel.Annotation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DbTableAttribute : Attribute
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否分片
        /// </summary>
        public bool IsSharding { get; set; }

        public DbTableAttribute(string name, bool isSharding = false)
        {
            this.Name = name;
            this.IsSharding = isSharding;
        }
    }
}