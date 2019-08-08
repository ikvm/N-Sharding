using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSharding.Sharding.Rule;
using NSharding.Sharding.RuleManager;

namespace NSharding.UnitTest
{
    /// <summary>
    /// ShardingRuleUnitTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ShardingRuleUnitTest
    {
        public ShardingRuleUnitTest()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ShardingStrategyCRUDTest()
        {
            var strategy = new ShardingStrategy()
            {
                ID = "AreaTableSharding",
                DisplayName = "地区分表策略",
                PostFixListConfig = "sd,hb,gd,bj,sz,tj,sx,nm,hb,hn,sh",
                ShardingType = ShardingType.Enum
            };

            try
            {
                ShardingStrategyService.GetInstance().DeleteShardingStrategy(strategy.ID);
                ShardingStrategyService.GetInstance().SaveShardingStrategy(strategy);
                var queryStrategy = ShardingStrategyService.GetInstance().GetShardingStrategy(strategy.ID);

                Assert.IsNotNull(queryStrategy);
                Assert.AreEqual(queryStrategy.DisplayName, strategy.DisplayName);
                Assert.AreEqual(queryStrategy.PostFixListConfig, strategy.PostFixListConfig);
                Assert.AreEqual(queryStrategy.ShardingType, strategy.ShardingType);

                TableShardingStrategy table = new TableShardingStrategy(queryStrategy);
                Assert.IsNotNull(table);
                Assert.AreEqual(queryStrategy.DisplayName, table.DisplayName);
                Assert.AreEqual(queryStrategy.PostFixListConfig, table.PostFixListConfig);
                Assert.AreEqual(queryStrategy.ShardingType, table.ShardingType);
            }
            finally
            {
                ShardingStrategyService.GetInstance().DeleteShardingStrategy(strategy.ID);
            }
        }
    }
}
