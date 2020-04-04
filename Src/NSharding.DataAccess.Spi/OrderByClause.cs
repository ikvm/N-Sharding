using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    /// <summary>
    /// 排序条件
    /// </summary>
    [Serializable]
    public class OrderByClause
    {
        public List<string> OrderByFields { get; set; }

        public OrderByType OrderByType { get; set; }

        public FieldType FieldType { get; set; }

        public OrderByClause(OrderByType orderByType, FieldType fieldType, params string[] fields)
        {
            FieldType = fieldType;
            OrderByType = orderByType;
            OrderByFields = fields.ToList();
        }
    }
}
