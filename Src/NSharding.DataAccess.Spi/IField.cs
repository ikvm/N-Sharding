using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    public interface IField
    {
        string Field { get; set; }

        FieldType FieldType { get; set; }

    }
}
