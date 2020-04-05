using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Spi
{
    public class RelationalOperatorUtis
    {
        public static KeyValuePair<string, bool> ConvertToString(RelationalOperator relationalOperator)
        {
            switch (relationalOperator)
            {
                case RelationalOperator.BETWEEN:
                    return new KeyValuePair<string, bool>("between {0} and {1}", false);
                case RelationalOperator.Equal:
                    return new KeyValuePair<string, bool>("=", true);
                case RelationalOperator.Exists:
                    return new KeyValuePair<string, bool>("Exists({0})", false);
                case RelationalOperator.GreaterEqualThan:
                    return new KeyValuePair<string, bool>(">=", true);
                case RelationalOperator.GreaterThan:
                    return new KeyValuePair<string, bool>(">", true);
                case RelationalOperator.IN:
                    return new KeyValuePair<string, bool>("in({0})", false);
                case RelationalOperator.LessEqualThan:
                    return new KeyValuePair<string, bool>("<=", true);
                case RelationalOperator.LessThan:
                    return new KeyValuePair<string, bool>("<", true);
                case RelationalOperator.LIKE:
                    return new KeyValuePair<string, bool>("like '%{0}%'", false);
                case RelationalOperator.NotEqual:
                    return new KeyValuePair<string, bool>("<>", true);
                case RelationalOperator.StartLike:
                    return new KeyValuePair<string, bool>("like '{0}%'", false);
                case RelationalOperator.EndLike:
                    return new KeyValuePair<string, bool>("like '%{0}'", false);
                default:
                    return new KeyValuePair<string, bool>("", true);
            }
        }
    }
}
