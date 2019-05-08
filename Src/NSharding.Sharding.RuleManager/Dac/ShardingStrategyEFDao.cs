using NSharding.Sharding.Rule;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.Sharding.RuleManager.Dac
{
    class ShardingStrategyEFDao : DbContext, IShardingStrategyDao
    {
        public DbSet<ShardingStrategy> ShardingStrategies { get; set; }

        public DbSet<ShardingColumn> ShardingColumns { get; set; }

        public ShardingStrategyEFDao() : base("Metadata")
        {

        }

        /// <summary>
        /// 保存分区策略
        /// </summary>
        /// <param name="strategy">分区策略</param>
        public void SaveShardingStrategy(ShardingStrategy strategy)
        {
            ShardingStrategies.Add(strategy);
            this.SaveChanges();
        }

        /// <summary>
        /// 获取分区策略
        /// </summary>
        /// <param name="id">分区策略ID</param>
        /// <returns>分区策略</returns>
        public ShardingStrategy GetShardingStrategy(string id)
        {
            return ShardingStrategies.FirstOrDefault(i => i.ID == id);
        }

        /// <summary>
        /// 获取分区策略
        /// </summary>        
        /// <returns>分区策略</returns>
        public List<ShardingStrategy> GetShardingStrategys()
        {
            return ShardingStrategies.ToList();
        }

        /// <summary>
        /// 删除分区策略
        /// </summary>
        /// <param name="id">分区策略ID</param>
        public void DeleteShardingStrategy(string id)
        {
            var strategy = ShardingStrategies.FirstOrDefault(i => i.ID == id);
            if (strategy != null)
            {
                ShardingStrategies.Remove(strategy);
            }

            this.SaveChanges();
        }

        /// <summary>
        /// 删除分区列配置
        /// </summary>
        /// <param name="dataObjectID">数据对象ID</param>
        public void DeleteShardingColumns(string dataObjectID)
        {
            ShardingColumns.RemoveRange(ShardingColumns.Where(i => i.DataObjectID == dataObjectID));
            this.SaveChanges();
        }

        /// <summary>
        /// 获取分区列配置 
        /// </summary>
        /// <param name="dataObjectID">数据对象ID</param>
        /// <returns>分区列配置</returns>
        public List<ShardingColumn> GetShardingColumns(string dataObjectID)
        {
            return ShardingColumns.Where(i => i.DataObjectID == dataObjectID).ToList();
        }

        /// <summary>
        /// 保存分区列配置
        /// </summary>
        /// <param name="columns">分区列配置</param>
        public void SaveShardingColumns(List<ShardingColumn> columns)
        {
            ShardingColumns.AddRange(columns);
            this.SaveChanges();
        }
    }
}
