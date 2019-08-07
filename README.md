![logo](https://github.com/zhouguoqing/N-Sharding/blob/master/Resource/N-sharding.png)  
# N-Sharding
支持分库分表的数据库访问框架(.Net)

# Features
* 提供领域模型和数据对象的定义，数据对象和数据库的表进行映射，领域模型由数据对象组成，1:m, 1:m:n都支持
* 领域模型和实体类进行映射关联，这是ORM的基础设置
* 支持POCO注解，简化领域模型定义
* 支持分库分表，Sharding策略目前支持单键的分库分表策略，可以是时间、地区、单据类型等业务维度
* 支持领域模型的CRUD操作
* 内置SQLDOM，根据请求的不同，生成对应CRUD SQL语句
* 通过领域模型实现关系实体映射，返回实体类/DataSet
* 数据库第一版支持SQLServer，后续支持MySQL
* 支持事件扩容，例如：数据同步到ES，为后续综合查询做支持


# Installation
* [NuGet package] 近期发布最新的Nuget包

#Demo
~~~CSharp
public void AddDataSourceTransaction()
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
~~~
