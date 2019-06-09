using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Annotation
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class AssociationAttribute : Attribute
    {
        /// <summary>
        /// 领域模型ID
        /// </summary>
        public Type DomainModel { get; set; }


        /// <summary>
        /// 是否懒加载
        /// </summary>
        public bool IsLazyLoad { get; set; }

        public AssociationAttribute(Type domainModel, bool isLazyLoad)
        {
            IsLazyLoad = isLazyLoad;
            DomainModel = domainModel;
        }
    }
}
