using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Manager
{
    class DomainModelEFDao : DbContext, IDomainModelDao
    {
        public DbSet<DomainModel.Spi.DomainModel> DomainModels { get; set; }

        public DbSet<DomainObjectElement> DomainObjectElements { get; set; }

        public DbSet<Association> Associations { get; set; }

        public DbSet<AssociateItem> AssociateItems { get; set; }

        public DomainModelEFDao() : base("Metadata")
        {

        }

        /// <summary>
        /// 数据领域对象
        /// </summary>
        /// <param name="dataObject">领域对象</param>
        public void SaveDomainModel(DomainModel.Spi.DomainModel domainModel)
        {
            DomainModels.Add(domainModel);
            this.SaveChanges();
        }

        /// <summary>
        /// 保存领域对象元素集合
        /// </summary>
        /// <param name="elements">领域对象元素集合</param>
        public void SaveDomainObjectElements(List<DomainObjectElement> elements)
        {
            DomainObjectElements.AddRange(elements);
            this.SaveChanges();
        }

        /// <summary>
        /// 保存领域对象关联集合
        /// </summary>
        /// <param name="associations">领域对象关联集合</param>
        public void SaveDomainAssociation(List<Association> associations)
        {
            Associations.AddRange(associations);
            this.SaveChanges();
        }

        /// <summary>
        /// 保存领域对象关联项集合
        /// </summary>
        /// <param name="associations">领域对象关联项集合</param>
        public void SaveDomainAssociationItem(List<AssociateItem> associateItems)
        {
            AssociateItems.AddRange(associateItems);
            this.SaveChanges();
        }

        /// <summary>
        /// 删除领域对象
        /// </summary>
        /// <param name="id">领域对象ID</param>
        public void DeleteDomainModel(string id)
        {
            var model = new DomainModel.Spi.DomainModel { ID = id };
            this.DomainModels.Attach(model);
            this.DomainModels.Remove(model);

            this.SaveChanges();
        }

        /// <summary>
        /// 获取领域对象
        /// </summary>
        /// <param name="id">领域对象ID</param>
        /// <returns>领域对象</returns>
        public DomainModel.Spi.DomainModel GetDomainModel(string id)
        {
            return this.DomainModels.Find(id);
        }
    }
}
