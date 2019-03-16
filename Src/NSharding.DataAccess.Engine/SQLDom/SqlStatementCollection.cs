using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL语句集合类
    /// </summary>
    /// <remarks>SQL语句集合</remarks>
    public class SqlStatementCollection : Collection<SqlStatement>
    {
        #region 常量

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlStatementCollection()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">SQL语句集合</param>
        public SqlStatementCollection(IList<SqlStatement> list)
            : base(list)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enumerable">可枚举的SQL语句集合</param>
        public SqlStatementCollection(IEnumerable<SqlStatement> enumerable)
            : this(enumerable.ToList()) { }

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 批量增加SQL语句
        /// </summary>
        /// <param name="sqls">SQL语句数组</param>
        public void AddRange(SqlStatement[] sqls)
        {
            foreach (var sql in sqls)
            {
                this.Add(sql);
            }
        }

        /// <summary>
        /// 批量增加SQL语句List的副本。
        /// </summary>
        /// <param name="sqls">SQL语句List。</param>
        public void AddRangeClone(IList<SqlStatement> sqls)
        {
            foreach (var sql in sqls)
            {
                this.Add(sql.Clone() as SqlStatement);
            }
        }

        /// <summary>
        /// 批量增加SQL语句集合
        /// </summary>
        /// <param name="childSqlCollection">SQL语句集合</param>
        public void AddRange(SqlStatementCollection childSqlCollection)
        {
            foreach (var sql in childSqlCollection)
            {
                this.Add(sql);
            }
        }

        /// <summary>
        /// 批量增加SQL语句集合的副本。
        /// </summary>
        /// <param name="childSqlCollection">SQL语句集合。</param>
        public void AddRangeClone(SqlStatementCollection childSqlCollection)
        {
            foreach (var sql in childSqlCollection)
            {
                this.Add(sql.Clone() as SqlStatement);
            }
        }

        #endregion
    }
}