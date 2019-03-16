using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public class DataContext
    {
        private Dictionary<string, List<DataContextItem>> data;

        /// <summary>
        /// 数据字典
        /// </summary>
        public Dictionary<string, List<DataContextItem>> Data
        {
            get
            {
                if (data == null)
                    data = new Dictionary<string, List<DataContextItem>>();

                return data;
            }
        }

        /// <summary>
        /// 获取当前数据上下文项
        /// </summary>
        /// <param name="modelObjectID">领域对象ID</param>
        /// <returns>当前数据上下文项</returns>
        public DataContextItem GetCurrentDataContextItem(string modelObjectID)
        {
            if (!Data.ContainsKey(modelObjectID)) return null;
            return Data[modelObjectID][CurrentDataIndex];
        }

        /// <summary>
        /// 获取当前上下文中的主键数据
        /// </summary>
        /// <returns>当前上下文中的主键数据</returns>
        public IDictionary<string, object> GetCurrentPrimaryKeyData()
        {
            IDictionary<string, object> pkData = null;

            foreach (var dataItem in data)
            {
                pkData = dataItem.Value[CurrentDataIndex].PrimaryKeyData;
                if (pkData != null && pkData.Count() > 0)
                    return pkData;
            }

            return pkData;
        }

        /// <summary>
        /// 当前的数据索引
        /// </summary>
        public int CurrentDataIndex { get; set; }

        public void ResetCurrentDataIndex()
        {
            CurrentDataIndex = 0;
        }

        public void Add(string modelObjectID, List<DataContextItem> dataItems)
        {
            this.Data.Add(modelObjectID, dataItems);
        }

        public void Remove(string modelObjectID)
        {
            if (Data.ContainsKey(modelObjectID))
            {
                this.Data.Remove(modelObjectID);
            }
        }
    }
}
