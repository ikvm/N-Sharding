using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.UnitTest
{
    class DataObjectAssert
    {
        public static void AreEqual(DataObject expected, DataObject actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.AreEqual(expected.ID, actual.ID);
            Assert.AreEqual(expected.IsDatabaseSharding, actual.IsDatabaseSharding);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.IsLogicallyDeleted, actual.IsLogicallyDeleted);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.ActualTableNames, actual.ActualTableNames);
            Assert.AreEqual(expected.CreateTime, actual.CreateTime);
            Assert.AreEqual(expected.Creator, actual.Creator);
            Assert.AreEqual(expected.DatabaseShardingStrategyID, actual.DatabaseShardingStrategyID);
            Assert.AreEqual(expected.DataSourceName, actual.DataSourceName);
            Assert.AreEqual(expected.Descriptions, actual.Descriptions);

            Assert.IsNotNull(expected.Columns);
            Assert.IsNotNull(actual.Columns);

            Assert.AreEqual(expected.Columns.Count, actual.Columns.Count);
        }
    }
}
