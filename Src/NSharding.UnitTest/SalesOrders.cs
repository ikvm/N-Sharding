using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.UnitTest
{
    public class SalesOrders
    {
        public string ID { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Customer { get; set; }

        public decimal Tax { get; set; }

        public DateTime CreateTime { get; set; }

        public string AdjustReason { get; set; }

        public DateTime AccountingTime { get; set; }

        public List<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();
    }
}
