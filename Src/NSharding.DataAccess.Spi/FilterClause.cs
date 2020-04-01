using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    /// <summary>
    /// 查询过滤条件
    /// </summary>
    [Serializable]
    public class FilterClause
    {
        public FilterField FilterField { get; set; }

        public FilterFieldValue FilterFieldValue { get; set; }

        public RelationalOperator RelationalOperator { get; set; }

        public LogicalOperator LogicalOperator { get; set; }
    }
}
