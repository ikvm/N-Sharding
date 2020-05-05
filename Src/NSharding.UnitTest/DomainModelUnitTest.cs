using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSharding.UnitTest
{
    [TestClass]
    public class DomainModelUnitTest
    {
        [TestMethod]
        public void DomainModelCRUDTest()
        {
            var dataObject = DomainModelBuilder.CreateDataObject();
            var domainModel = DomainModelBuilder.CreateDomainModel();

            var dataObjectManager = new NSharding.DomainModel.Manager.DataObjectManager();
            dataObjectManager.DeleteDataObject(dataObject.ID);
            dataObjectManager.SaveDataObject(dataObject);

            var domainModelManager = new NSharding.DomainModel.Manager.DomainModelManager();
            domainModelManager.DeleteDomainModel(domainModel.ID);
            domainModelManager.SaveDomainModel(domainModel);

            var queryModel = domainModelManager.GetDomainModel(domainModel.ID);
            DomainModelAssert.AssertModel(domainModel, queryModel);
        }
    }
}
