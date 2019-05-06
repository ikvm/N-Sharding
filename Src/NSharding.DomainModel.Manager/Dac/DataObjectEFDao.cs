using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace NSharding.DomainModel.Manager
{
    class DataObjectEFDao : DbContext, IDataObjectDao
    {
        public DbSet<DataObject> DataObjects { get; set; }

        public DataObjectEFDao() : base("Metadata")
        {

        }

        /// <summary>
        /// 数据对象保存
        /// </summary>
        /// <param name="dataObject">数据对象</param>
        public void SaveDataObject(DataObject dataObject)
        {
            DataObjects.Add(dataObject);
            this.SaveChanges();
        }

        /// <summary>
        /// 删除数据对象
        /// </summary>
        /// <param name="id">数据对象ID</param>
        public void DeleteDataObject(string id)
        {
            var obj = DataObjects.FirstOrDefault(i => i.ID == id);
            if (obj != null)
            {
                DataObjects.Remove(obj);
                this.SaveChanges();
            }
        }

        /// <summary>
        /// 获取数据对象
        /// </summary>
        /// <param name="id">数据对象ID</param>
        /// <returns>数据对象</returns>
        public DataObject GetDataObject(string id)
        {
            return DataObjects.FirstOrDefault(i => i.ID == id);
        }
    }
}