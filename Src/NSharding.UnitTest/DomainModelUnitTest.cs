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
            var domainModelManager = new NSharding.DomainModel.Manager.DomainModelManager();
            try
            {
                dataObjectManager.DeleteDataObject(dataObject.ID);
                dataObjectManager.SaveDataObject(dataObject);
                
                domainModelManager.DeleteDomainModel(domainModel.ID);
                domainModelManager.SaveDomainModel(domainModel);

                var queryModel = domainModelManager.GetDomainModel(domainModel.ID);
                DomainModelAssert.AreEqual(domainModel, queryModel);
            }
            finally 
            {
                dataObjectManager.DeleteDataObject(dataObject.ID);
                domainModelManager.DeleteDomainModel(domainModel.ID);
            }
        }

        [TestMethod]
        public void DomainObjectCRUDTest()
        {
            var dataObject = DomainModelBuilder.CreateDataObject();
          
            var dataObjectManager = new NSharding.DomainModel.Manager.DataObjectManager();
            try
            {
                dataObjectManager.DeleteDataObject(dataObject.ID);
                dataObjectManager.SaveDataObject(dataObject);

                var queryDataObject = dataObjectManager.GetDataObject(dataObject.ID);

                DataObjectAssert.AreEqual(dataObject, queryDataObject);
            }
            finally 
            {
                dataObjectManager.DeleteDataObject(dataObject.ID);
            }
        }
    }
}
