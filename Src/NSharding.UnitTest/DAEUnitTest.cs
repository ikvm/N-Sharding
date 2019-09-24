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
    }
}
