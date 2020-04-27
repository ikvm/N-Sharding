using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSharding.UnitTest
{
    class DomainModelAssert
    {
        public static void AssertModel(DomainModel.Spi.DomainModel domainModel, DomainModel.Spi.DomainModel queryDomainModel)
        {
            Assert.AreEqual(domainModel.CacheStrategy, queryDomainModel.CacheStrategy);
            Assert.AreEqual(domainModel.IsCache, queryDomainModel.IsCache);
            Assert.AreEqual(domainModel.Name, queryDomainModel.Name);
            Assert.AreEqual(domainModel.RootDomainObjectID, queryDomainModel.RootDomainObjectID);
            Assert.AreEqual(domainModel.Version, queryDomainModel.Version);
            Assert.AreEqual(domainModel.DomainObjects.Count, queryDomainModel.DomainObjects.Count);

            Assert.AreEqual(domainModel.RootDomainObject.DataObjectID, queryDomainModel.RootDomainObject.DataObjectID);
            Assert.AreEqual(domainModel.RootDomainObject.DomainModelID, queryDomainModel.RootDomainObject.DomainModelID);
            Assert.AreEqual(domainModel.RootDomainObject.IsRootObject, queryDomainModel.RootDomainObject.IsRootObject);
            Assert.AreEqual(domainModel.RootDomainObject.PropertyName, queryDomainModel.RootDomainObject.PropertyName);
            Assert.AreEqual(domainModel.RootDomainObject.Elements.Count, queryDomainModel.RootDomainObject.Elements.Count);
        }
    }
}
