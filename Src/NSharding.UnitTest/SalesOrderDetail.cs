using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.UnitTest
{
    public class SalesOrderDetail
    {
        public string ID { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public DateTime CreateTime { get; set; }

        public string OrderId { get; set; }

        public decimal SCTaxExPrice { get; set; }

        public decimal Price { get; set; }
    }
}
