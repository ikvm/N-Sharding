using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    public class FilterFieldValue
    {
        public FilterFieldValue()
        {

        }

        public FilterFieldValue(string fieldValue, FieldValueType fieldValueType, bool isNull = false)
        {
            this.FiledValue = fieldValue;
            this.ValueType = fieldValueType;
            IsNull = isNull;
        }

        public FieldValueType ValueType { get; set; }

        public string FiledValue { get; set; }

        public bool IsNull { get; set; }
    }
}
