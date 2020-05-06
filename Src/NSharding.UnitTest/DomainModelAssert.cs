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
        public static void AreEqual(DomainModel.Spi.DomainModel expected, DomainModel.Spi.DomainModel actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.AreEqual(expected.CacheStrategy, actual.CacheStrategy);
            Assert.AreEqual(expected.IsCache, actual.IsCache);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.RootDomainObjectID, actual.RootDomainObjectID);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.DomainObjects.Count, actual.DomainObjects.Count);

            Assert.AreEqual(expected.RootDomainObject.DataObjectID, actual.RootDomainObject.DataObjectID);
            Assert.AreEqual(expected.RootDomainObject.DomainModelID, actual.RootDomainObject.DomainModelID);
            Assert.AreEqual(expected.RootDomainObject.IsRootObject, actual.RootDomainObject.IsRootObject);
            Assert.AreEqual(expected.RootDomainObject.PropertyName, actual.RootDomainObject.PropertyName);
            Assert.AreEqual(expected.RootDomainObject.Elements.Count, actual.RootDomainObject.Elements.Count);
        }
    }
}
