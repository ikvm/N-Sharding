using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NSharding.DomainModel.Annotation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DbTableAttribute : Attribute
    {
        public string ID { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否分片
        /// </summary>
        public bool IsSharding { get; set; }

        public bool IsView { get; set; }

        public string TableShardingStrategyID { get; set; }

        public string DBShardingStrategyID { get; set; }

        public string DataSourceName { get; set; }

        public DbTableAttribute(string id)
        {
            this.ID = id;
        }

        public DbTableAttribute(string name,string dataSourceName , bool isSharding = false, bool isView=false, string tableShardingStrategyId = "", string dbShardingStrategyID = "")
        {
            this.Name = name;
            this.IsSharding = isSharding;
            this.IsView = isView;
            this.TableShardingStrategyID = tableShardingStrategyId;
            this.DBShardingStrategyID = dbShardingStrategyID;
            this.DataSourceName = dataSourceName;
        }
    }
}