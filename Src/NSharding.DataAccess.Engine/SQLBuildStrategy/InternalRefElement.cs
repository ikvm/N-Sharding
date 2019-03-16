using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 关联元素内部类
    /// </summary>
    class InternalRefElement
    {
        /// <summary>
        /// 标签。在Sql形成时，必须是主表上的标签，而不能是引用的Element的标签。
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 关联的Element。
        /// </summary>
        public DomainObjectElement Element { get; set; }
    }
}
