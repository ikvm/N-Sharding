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
    public class DataSourceUnitTest
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
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "Orders_QD" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "Orders_JN" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "Orders_LY" });
            dataSource.DbLinks.Add(dbLink);

            var dbLinkHB = new DatabaseLink()
            {
                Name = "DBHB",
                ConnectionString = "DBHB",
                DataSourceName = "DB1",
                DataSource = dataSource
            };
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "Orders_BJ" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "Orders_LF" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "Orders_SJZ" });
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
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "Orders_QD" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "Orders_JN" });
            dbLink.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DB-SD", Name = "Orders_LY" });
            dataSource.DbLinks.Add(dbLink);

            var dbLinkHB = new DatabaseLink()
            {
                Name = "DBHB",
                ConnectionString = "DBHB",
                DataSourceName = "DB1",
                DataSource = dataSource
            };
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "Orders_BJ" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "Orders_LF" });
            dbLinkHB.Tables.Add(new DatabaseTable() { DatabaseLinkName = "DBHB", Name = "Orders_SJZ" });
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
        public void MySQLDataSourceTest()
        {
            var dataSource = new DataSource()
            {
                Name = "MYSQL",
                DbType = DbType.MySQL,
                IsSharding = true,
                Description = "MYSQL"
            };

            var dbLink = new DatabaseLink()
            {
                Name = "MYSQL",
                ConnectionString = "MYSQL",
                DataSourceName = "MYSQL",
                DataSource = dataSource
            };

            dataSource.DbLinks.Add(dbLink);

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
                //manager.DeleteDataSource(dataSource.Name);
            }
        }
    }
}
