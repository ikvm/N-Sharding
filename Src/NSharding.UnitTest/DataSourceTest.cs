using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSharding.DomainModel.Manager;
using NSharding.Sharding.Database;

namespace NSharding.UnitTest
{
    [TestClass]
    public class DataSourceTest
    {
        [TestMethod]
        public void DataSourceTransactionTest()
        {
            var dataSource = new DataSource()
            {
                Name = "DB1",
                DbType = DbType.SQLServer,
                IsSharding = true,
                Description = "DB"
            };

            var dbLink = new DatabaseLink()
            {
                Name = "DB-SD",
                ConnectionString = "DB-SD",
                DataSourceName = "DB1",
                DataSource = dataSource
            };
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "CMChargeBills_QD" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "CMChargeBills_JN" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "CMChargeBills_LY" });
            dataSource.DbLinks.Add(dbLink);

            var dbLinkHB = new DatabaseLink()
            {
                Name = "DBHB",
                ConnectionString = "DBHB",
                DataSourceName = "DB1",
                DataSource = dataSource
            };
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "CMChargeBills_BJ" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "CMChargeBills_LF" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "CMChargeBills_SJZ" });
            dataSource.DbLinks.Add(dbLinkHB);

            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(
                Task.Factory.StartNew(() =>
                {
                    var manager = new DataSourceManager();
                    manager.SaveDataSource(dataSource);
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void DataSourceCRUDTest()
        {
            var dataSource = new DataSource()
            {
                Name = "DB1",
                DbType = DbType.SQLServer,
                IsSharding = true,
                Description = "DB"
            };

            var dbLink = new DatabaseLink()
            {
                Name = "DB-SD",
                ConnectionString = "DB-SD",
                DataSourceName = "DB1",
                DataSource = dataSource
            };
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "CMChargeBills_QD" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "CMChargeBills_JN" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "CMChargeBills_LY" });
            dataSource.DbLinks.Add(dbLink);

            var dbLinkHB = new DatabaseLink()
            {
                Name = "DBHB",
                ConnectionString = "DBHB",
                DataSourceName = "DB1",
                DataSource = dataSource
            };
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "CMChargeBills_BJ" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "CMChargeBills_LF" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "CMChargeBills_SJZ" });
            dataSource.DbLinks.Add(dbLinkHB);

            var manager = new DataSourceManager();

            try
            {
                manager.DeleteDataSource(dataSource.Name);
                manager.SaveDataSource(dataSource);

                var dataSourceQuery = manager.GetDataSource(dataSource.Name);
                Assert.IsNotNull(dataSourceQuery);
                Assert.AreEqual(dataSource.IsSharding, dataSourceQuery.IsSharding);
                Assert.AreEqual(dataSource.Description, dataSourceQuery.Description);
                Assert.AreEqual(dataSource.DbType, dataSourceQuery.DbType);

                Assert.AreEqual(dataSource.DbLinks.Count, dataSourceQuery.DbLinks.Count);

                var dblink0 = dataSourceQuery.DbLinks.FirstOrDefault(i => i.Name == dataSource.DbLinks[0].Name);
                Assert.AreEqual(dataSource.DbLinks[0].Name, dblink0.Name);
                Assert.AreEqual(dataSource.DbLinks[0].IsDefault, dblink0.IsDefault);
                Assert.AreEqual(dataSource.DbLinks[0].DataSourceName, dblink0.DataSourceName);
                Assert.AreEqual(dataSource.DbLinks[0].Tables.Count, dblink0.Tables.Count);
            }
            finally
            {
                manager.DeleteDataSource(dataSource.Name);
            }
        }

        [TestMethod]
        public void DataTransfer()
        {
            var chargebillStringBuilder = new StringBuilder(60000);
            var chargebillDetailStringBuilder = new StringBuilder(60000);
            var chargebillDeductStringBuilder = new StringBuilder(60000);
            var chargebillDeductStrategyStringBuilder = new StringBuilder(60000);

            var list = new List<string>() { "34", "64", "15", "45", "51", "21", "23", "11", "46", "41", "35", "65", "53", "31", "22", "54", "61", "52", "13", "43", "14", "12", "44", "50", "42", "37", "33", "63", "32", "62", "36" };
            foreach (var area in list)
            {
                var chargebill = String.Format(@"
                    CREATE TABLE dbo.CM_ChargeBills(
	                    ID varchar(36) NOT NULL,
	                    Code varchar(36) NOT NULL,
	                    OldCode varchar(36) NULL,
	                    OrderType char(1) NULL    DEFAULT ('0'),
	                    FiscalYear varchar(4) NULL,
	                    FiscalPeriod varchar(2) NULL,
	                    BookKeepFlag char(1) NULL,
	                    BookKeeper varchar(128) NULL,
	                    BookKeepDate datetime NULL,
	                    SettleFlag char(1) NULL,
	                    SettleDate datetime NULL,
	                    BeginTime datetime NULL,
	                    EndTime datetime NULL,
	                    InitPower decimal(20, 2) NULL   DEFAULT ((0)),
	                    Power decimal(20, 2) NULL  DEFAULT ((0)),
	                    SCTaxInPrice decimal(9, 2) NULL  DEFAULT ((0)),
	                    SCTaxExPrice decimal(9, 2) NULL   DEFAULT ((0)),
	                    ECTaxInPrice decimal(9, 2) NULL DEFAULT ((0)),
	                    ECTaxExPrice decimal(9, 2) NULL    DEFAULT ((0)),
	                    Type char(1) NULL,
	                    Way varchar(2) NULL,
	                    AdjustState char(1) NULL    DEFAULT ('1'),
	                    Status char(1) NULL,
	                    OperatorID varchar(36) NULL,
	                    StaID varchar(36) NULL,
	                    StaName varchar(128) NULL,
	                    PillID varchar(36) NULL,
	                    PillName varchar(128) NULL,
	                    CustID varchar(36) NULL,
	                    CustName varchar(128) NULL,
	                    PolicyID varchar(36) NULL,
	                    CustCardID varchar(36) NULL,
	                    EndReason varchar(280) NULL,
	                    LastPower decimal(9, 2) NULL DEFAULT ((0)),
	                    LastPowerTime datetime NULL,
	                    Auditor varchar(128) NULL,
	                    AuditTime datetime NULL,
	                    Note varchar(280) NULL,
	                    CtrlAddr varchar(36) NULL,
	                    PileCanSN varchar(36) NULL,
	                    IfSonStaPro char(1) NULL,
	                    ChargingPower decimal(9, 2) NOT NULL   DEFAULT ((0)),
	                    PileMaxValue decimal(14, 2) NOT NULL   DEFAULT ((0)),
	                    CreateTime datetime NULL   DEFAULT (getdate()),
	                    Creator varchar(128) NULL,
	                    LastModifyTime datetime NULL   DEFAULT (getdate()),
	                    LastModifier varchar(128) NULL,
	                    oldid int NULL,
	                    AdjustReason varchar(280) NULL,
	                    OpenBillCode varchar(36) NULL,
	                    NumberplateCode varchar(36) NULL,
	                    ChargingModel varchar(36) NULL,
	                    TwoClassCustomerID varchar(36) NULL,
	                    CustType char(1) NULL,
	                    GroupFlag varchar(36) NULL,
	                    ETRate decimal(9, 2) NULL,
	                    SCTRate decimal(9, 2) NULL,
	                    EvcInfoID varchar(36) NULL,
	                    ChargeGroup varchar(36) NULL,
	                    FacilityType char(1) NULL,
	                    WhetherTest char(1) NULL,
	                    FreeSCTaxInPrice decimal(9, 2) NULL,
	                    FreeSCTaxExPrice decimal(9, 2) NULL,
	                    EvcInfoNo varchar(36) NULL,
	                    FailReason int NULL,
	                    StartSoc decimal(9, 2) NULL,
	                    EndSoc decimal(9, 2) NULL,
	                    IsMeddle char(1) NULL,
	                    MeddleTime datetime2(7) NULL,
	                    StartCmdResponse int NULL,
	                    StartCmdExec int NULL,
	                    IsCredibleCust char(1) NULL,
	                    IsAdjust char(1) NULL,
	                    DeviceId varchar(128) NULL,
	                    ChargingRequestSource varchar(50) NULL,
	                    IsPostpaid int NULL DEFAULT ((0)),
	                    IsLocalScheduling int NULL DEFAULT ((0)),
	                    AccountingTime datetime NULL,
	                    IsProduction int NULL DEFAULT ((0)),
	                    ServiceTel varchar(36) NULL,
	                    IsAfford int NULL,
	                    AffordAccountId varchar(36) NULL,
	                    AffordAccoutType int NULL,
	                    MerchantCode varchar(36) NULL,
	                    TransformerId varchar(36) NULL,
	                    PileCode varchar(36) NULL,
	                    GunCount int NULL,
	                    StopCustomerId varchar(36) NULL,
	                    PlatformAccountId varchar(36) NULL,
	                    PlatformAccountType int NULL,
	                    ARAPStatus char(1) NULL DEFAULT ('0'),
	                    ARAPCreator varchar(128) NULL DEFAULT (''),
	                    ARAPCreateTime datetime NULL,
	                    ThirdPartyEquipmfrId varchar(36) NULL,
	                    ThirdPartyEquipmfrName varchar(36) NULL,
	                    ThirdPartyBillStatus int NULL,
                     CONSTRAINT PK_CM_CHARGINGBILLS PRIMARY KEY CLUSTERED (ID ASC))");
                chargebill = chargebill.Replace("PK_CM_CHARGINGBILLS", "PK_CM_CHARGINGBILLS" + area);
                chargebill = chargebill.Replace("CM_ChargeBills", "CM_ChargeBills" + area);

                chargebillStringBuilder.Append(chargebill).Append("\r\n");
                //var insertSQL = string.Format(@"insert into CM_ChargeBills{0} select CM_ChargeBills.* from CM_ChargeBills inner join CSM_Stas on CM_ChargeBills.StaID = CSM_Stas.ID and StaProvince='{0}'", area);
                //chargebillStringBuilder.Append(insertSQL).Append("\r\n");

                var chargebillDetail = string.Format(@"
                CREATE TABLE dbo.CM_ChargeBillDetail(
	                ID varchar(36) NOT NULL,
	                ChargeBillID varchar(36) NULL,
	                PolicyBeginTime datetime NULL,
	                PolicyEndTime datetime NULL,
	                RealBeginTime datetime NULL,
	                RealEndTime datetime NULL,
	                ECTaxInPrice decimal(9, 4) NULL,
	                ECTaxExPrice decimal(9, 4) NULL,
	                SCTaxInPrice decimal(9, 4) NULL,
	                SCTaxExPrice decimal(9, 4) NULL,
	                Power decimal(9, 2) NULL DEFAULT ((0)),
	                CreateTime datetime NULL DEFAULT (getdate()),
	                Creator varchar(128) NULL,
	                LastModifyTime datetime NULL DEFAULT (getdate()),
	                LastModifier varchar(128) NULL,
	                oldid int NULL,
                 CONSTRAINT PK_CM_CHARGEBILLDETAIL PRIMARY KEY CLUSTERED (ID ASC ))");

                chargebillDetail = chargebillDetail.Replace("PK_CM_CHARGEBILLDETAIL", "PK_CM_CHARGEBILLDETAIL" + area);
                chargebillDetail = chargebillDetail.Replace("CM_ChargeBillDetail", "CM_ChargeBillDetail" + area);

                chargebillDetailStringBuilder.Append(chargebillDetail).Append("\r\n");
                //var insertChargeDetail = string.Format(@"insert into CM_ChargeBillDetail{0} select CM_ChargeBillDetail.* from CM_ChargeBillDetail,CM_ChargeBills,CSM_Stas where CM_ChargeBillDetail.ChargeBillID = CM_ChargeBills.ID and  CM_ChargeBills.StaID = CSM_Stas.ID and StaProvince='{0}'", area);
                //chargebillDetailStringBuilder.Append(insertChargeDetail).Append("\r\n");

                var chargeBillDeduct = string.Format(@"CREATE TABLE dbo.CM_ChargeBillDeduct(
	                ID varchar(36) NOT NULL,
	                ChargeBillID varchar(36) NOT NULL,
	                PayAccID varchar(36) NOT NULL,
	                VoucherID varchar(36) NULL,
	                PayAccType char(1) NOT NULL,
	                PayAccSourceType char(1) NULL,
	                MoneyType char(1) NOT NULL,
	                TaxInMoney decimal(9, 2) NULL DEFAULT ((0)),
	                TaxExMoney decimal(9, 2) NULL DEFAULT ((0)),
	                FiscalCode varchar(84) NULL,
	                FiscalDate datetime NULL,
	                BillingStatus char(1) NULL  DEFAULT ((0)),
	                PayStatus char(1) NULL,
	                CreateTime datetime NULL DEFAULT (getdate()),
	                Creator varchar(128) NULL,
	                LastModifyTime datetime NULL DEFAULT (getdate()),
	                LastModifier varchar(128) NULL,
	                oldid int NULL,
	                AccountingTime datetime NULL,
	                GenBillsStatus char(1) NULL DEFAULT ('0'),
	                SettleFlag char(1) NULL DEFAULT ('0'),
	                SOVCForwardID varchar(36) NULL,
	                SOVCForwardFlag varchar(1) NULL,
	                SOVCForwardCode varchar(36) NULL,
	                TempVoucherCode varchar(50) NULL,
	                VoucherCode varchar(50) NULL,
	                FiscalYear varchar(4) NULL,
	                FiscalPeriod varchar(2) NULL,
	                VoucherOuter varchar(36) NULL,
	                VoucherOutTime datetime NULL,
	                CouponId varchar(36) NULL,
	                ThirdpartyPayType varchar(10) NULL,
	                ThirdpartyPayCode varchar(36) NULL,
	                AccountingId varchar(36) NULL,
	                PlatformAccountId varchar(36) NULL,
	                PlatformAccountType int NULL,
	                ARAPStatus char(1) NULL DEFAULT ('0'),
	                ARAPCreator varchar(128) NULL DEFAULT (''),
	                ARAPCreateTime datetime NULL,
                 CONSTRAINT PK_CM_CHARGEBILLDEDUCT PRIMARY KEY CLUSTERED (ID ASC)) ");

                chargeBillDeduct = chargeBillDeduct.Replace("PK_CM_CHARGEBILLDEDUCT", "PK_CM_CHARGEBILLDEDUCT" + area);
                chargeBillDeduct = chargeBillDeduct.Replace("CM_ChargeBillDeduct", "CM_ChargeBillDeduct" + area);

                chargebillDeductStringBuilder.Append(chargeBillDeduct).Append("\r\n");

                var chargeBillDeductStrategy = string.Format(@"CREATE TABLE dbo.CM_ChargeBillDeductStrategy(
	                ID varchar(36) NOT NULL,
	                ChargeBillID varchar(36) NULL,
	                PayAccID varchar(36) NULL,
	                VoucherID varchar(36) NULL,
	                PayAccType char(1) NULL,
	                PayAccSourceType char(1) NULL,
	                RunOrder int NULL DEFAULT ((0)),
	                TimeChargeID varchar(36) NULL,
	                SmartChargeID varchar(36) NULL,
	                CreateTime datetime NULL DEFAULT (getdate()),
	                Creator varchar(128) NULL,
	                LastModifyTime datetime NULL DEFAULT (getdate()),
	                LastModifier varchar(128) NULL,
	                CouponId varchar(36) NULL,
	                PlatformAccountId varchar(36) NULL,
	                PlatformAccountType int NULL,
                 CONSTRAINT PK_CM_CHARGEBILLDEDUCTSTRATEGY PRIMARY KEY CLUSTERED (ID ASC))");
                chargeBillDeductStrategy = chargeBillDeductStrategy.Replace("PK_CM_CHARGEBILLDEDUCTSTRATEGY", "PK_CM_CHARGEBILLDEDUCTSTRATEGY" + area);
                chargeBillDeductStrategy = chargeBillDeductStrategy.Replace("CM_ChargeBillDeductStrategy", "CM_ChargeBillDeductStrategy" + area);

                chargebillDeductStrategyStringBuilder.Append(chargeBillDeductStrategy).Append("\r\n");
            }

            var sql = chargebillStringBuilder.ToString();
            var sqlDetail = chargebillDetailStringBuilder.ToString();
            var sqlDeduct = chargebillDeductStringBuilder.ToString();
            var sqlDeductStrategy = chargebillDeductStrategyStringBuilder.ToString();
        }

        [TestMethod]
        public void ChargeDataTransfer()
        {
            var chargebill = new StringBuilder(60000);

            var list = new List<string>() { "34", "64", "15", "45", "51", "21", "23", "11", "46", "41", "35", "65", "53", "31", "22", "54", "61", "52", "13", "43", "14", "12", "44", "50", "42", "37", "33", "63", "32", "62", "36" };
            foreach (var area in list)
            {
                var chargebillStringBuilder = new StringBuilder(60000);
                var insertSQL = string.Format(@"insert into CM_ChargeBills{0} select CM_ChargeBills.* from CM_ChargeBills inner join CSM_Stas on CM_ChargeBills.StaID = CSM_Stas.ID and StaProvince='{0}' where CM_ChargeBills.CreateTime>'2016-09-02 10:32:37.060'", area);
                chargebillStringBuilder.Append(insertSQL).Append("\r\n");

                var insertChargeDetail = string.Format(@"insert into CM_ChargeBillDetail{0} select CM_ChargeBillDetail.* from CM_ChargeBillDetail,CM_ChargeBills{0} where CM_ChargeBillDetail.ChargeBillID = CM_ChargeBills{0}.ID ", area);
                chargebillStringBuilder.Append(insertChargeDetail).Append("\r\n");

                var insertChargeDeduct = string.Format(@"insert into CM_ChargeBillDeduct{0} select CM_ChargeBillDeduct.* from CM_ChargeBillDeduct,CM_ChargeBills{0} where CM_ChargeBillDeduct.ChargeBillID = CM_ChargeBills{0}.ID ", area);
                chargebillStringBuilder.Append(insertChargeDeduct).Append("\r\n");

                var insertChargeDeductStrategy = string.Format(@"insert into CM_ChargeBillDeductStrategy{0} select CM_ChargeBillDeductStrategy.* from CM_ChargeBillDeductStrategy,CM_ChargeBills{0} where CM_ChargeBillDeductStrategy.ChargeBillID = CM_ChargeBills{0}.ID ", area);
                chargebillStringBuilder.Append(insertChargeDeductStrategy).Append("\r\n");

                var tempInsert = chargebillStringBuilder.ToString();
                chargebill.Append(tempInsert);
            }

            var sql = chargebill.ToString();
        }
    }
}
