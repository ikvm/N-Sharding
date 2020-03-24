using NSharding.Sharding.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    /// <summary>
    /// 查询结果集
    /// </summary>
    [Serializable]
    public class QueryResultSet
    {
        /// <summary>
        /// 数据集
        /// </summary>
        public List<DataTable> DataTables { get; set; }

        /// <summary>
        /// Sharding路由信息
        /// </summary>
        public Dictionary<string, ShardingTarget> ShardingInfo { get; set; } = new Dictionary<string, ShardingTarget>();

        /// <summary>
        /// 获取指定数据对象对应的DataTable
        /// </summary>
        /// <param name="dataObjectId">数据对象标识</param>
        /// <returns>数据对象对应的DataTable</returns>
        public DataTable GetDataTable(string dataObjectId)
        {
            if (!ShardingInfo.ContainsKey(dataObjectId))
                throw new Exception($"Cannot find {dataObjectId}'s sharding info.");

            var tableName = ShardingInfo[dataObjectId].TableName;

            return DataTables.FirstOrDefault(i => i.TableName.ToLower() == tableName.ToLower());
        }
    }
}
