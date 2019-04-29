using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Manager
{
    class DataSourceEFDao : DbContext, IDataSourceDao
    {

        public DbSet<DataSource> DataSources { get; set; }

        public DbSet<DatabaseTable> DatabaseTables { get; set; }

        // <summary>
        /// 数据源保存
        /// </summary>
        /// <param name="dataSource">数据源</param>
        public void SaveDataSource(DataSource dataSource)
        {
            this.DataSources.Add(dataSource);
            this.SaveChanges();
        }

        /// <summary>
        /// 删除数据源
        /// </summary>
        /// <param name="id">数据源ID</param>
        public void DeleteDataSource(string id)
        {
            var deleteObj = DataSources.FirstOrDefault(i => i.Name == id);
            if (deleteObj != null)
                DataSources.Remove(deleteObj);

            this.SaveChanges();
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="id">数据源ID</param>
        /// <returns>数据源</returns>
        public DataSource GetDataSource(string id)
        {
            return DataSources.FirstOrDefault(i => i.Name == id);
        }

        /// <summary>
        /// 保存数据表
        /// </summary>
        /// <param name="tables">数据表</param>
        public void SaveDatabaseTables(List<DatabaseTable> tables)
        {
            this.DatabaseTables.AddRange(tables);
            this.SaveChanges();
        }


        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="name">数据源ID</param>
        /// <returns>数据源</returns>
        public List<DataSource> GetDataSources()
        {
            return this.DataSources.ToList();
        }
    }
}
