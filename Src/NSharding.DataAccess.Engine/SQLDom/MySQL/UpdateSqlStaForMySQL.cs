using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    class UpdateSqlStaForMySQL : UpdateSqlStatement
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public UpdateSqlStaForMySQL()
            : base() { }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            StringBuilder builder = new StringBuilder();
            string updateSql = string.Format("UPDATE {0} {1} SET {2}",
                this.TableName, this.TableCode, base.GetSetClause(this.UpdateFields, this.UpdateValues));
            builder.Append(updateSql);
            string whereCondition = this.GetUpdateConditionString();
            if (!IsBlankString(whereCondition))
                builder.AppendFormat(" WHERE {0}", whereCondition);

            return builder.ToString();
        }
    }
}
