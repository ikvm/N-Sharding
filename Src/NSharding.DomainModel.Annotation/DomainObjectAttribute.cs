using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Annotation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DomainObjectAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否为根结点对象
        /// </summary>
        public bool IsRootObject { get; set; }


        /// <summary>
        /// 父级领域对象
        /// </summary>
        public string ParentObject { get; set; }

        public DomainObjectAttribute(string name, string parentObject = "", bool isRootObject = false)
        {
            this.Name = name;
            this.ParentObject = parentObject;
            this.IsRootObject = isRootObject;
        }
    }
}
