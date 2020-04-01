using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    public class FilterField : IField
    {
        public FilterField()
        {

        }

        public FilterField(string field, FieldType fieldType)
        {
            Field = field;
            FieldType = fieldType;
        }

        public FilterField(string field, FieldType fieldType, string groupName)
        {
            Field = field;
            FieldType = fieldType;
            GroupName = groupName;
        }

        public string GroupName { get; set; }

        public string Field { get; set; }

        public FieldType FieldType { get; set; }
    }
}
