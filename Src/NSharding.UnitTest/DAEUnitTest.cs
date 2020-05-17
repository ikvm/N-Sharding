using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSharding.DataAccess.Service;
using NSharding.Sharding.Rule;

namespace NSharding.UnitTest
{
    [TestClass]
    public class DAEUnitTest
    {
        [TestMethod]
        public void ChargeBillCRUDTest()
        {
            var orders = OrderAssert.CreateOrders();
            var shardingValue = new ShardingValue("Orders", "StationProvince", "21");

            DataAccessService.GetInstance().Save("Orders", orders, shardingValue);
            var dataTables = DataAccessService.GetInstance().GetData("Orders", orders.ID, shardingValue);
            Assert.IsNotNull(dataTables);

            orders.AdjustReason = "Begin Charging";
            orders.AccountingTime = DateTime.Now;
            orders.SalesOrderDetails[0].SCTaxExPrice = new decimal(19.00);

            DataAccessService.GetInstance().Update("Orders", orders, shardingValue);

            DataAccessService.GetInstance().Delete("Orders", orders.ID, shardingValue);
        }

        [TestMethod]
        public void SalesOrderCRUDTest()
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

                var orders = OrderAssert.CreateOrders();

                DataAccessService.GetInstance().Save(domainModel.ID, orders);
                var dataTables = DataAccessService.GetInstance().GetData(domainModel.ID, orders.ID);
                Assert.IsNotNull(dataTables);

            }
            finally
            {
                dataObjectManager.DeleteDataObject(dataObject.ID);
                domainModelManager.DeleteDomainModel(domainModel.ID);
            }
        }
    }
}
