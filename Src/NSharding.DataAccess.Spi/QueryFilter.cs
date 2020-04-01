﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSharding.Sharding.Rule;

namespace NSharding.DataAccess.Spi
{
    /// <summary>
    /// 查询过滤器
    /// </summary>
    [Serializable]
    public class QueryFilter
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public List<FilterClause> FilterClauses { get; set; } = new List<FilterClause>();

        /// <summary>
        /// 排序条件
        /// </summary>
        public List<OrderByClause> OrderByCondition { get; set; } = new List<OrderByClause>();

        /// <summary>
        /// 分页信息
        /// </summary>
        public PageParameter PageParameter { get; set; }

        /// <summary>
        /// 限制数据返回行数
        /// </summary>
        public int LimitCount { get; set; }

        //查询投影字段
        private List<QueryProjectionField> projectionFields;

        /// <summary>
        /// 查询投影字段
        /// </summary>
        public List<QueryProjectionField> ProjectionFields
        {
            get
            {
                if (projectionFields == null)
                    projectionFields = new List<QueryProjectionField>();

                return projectionFields;
            }
            set
            {
                projectionFields = value;
            }
        }

        /// <summary>
        /// 分区分表键值
        /// </summary>
        public ShardingValue ShardingValue { get; set; }
    }
}
