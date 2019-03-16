// ===============================================================================
// 浪潮GSP平台
// ***类说明***
// 请查看《GSP7-******子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/27 16:04:34        1.0        maorx           添加头注释。
// ===============================================================================
// 开发者: maorx
// 2013/2/27 16:04:34 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// ***类说明***
    /// </summary>
    /// <remarks>类的补充说明</remarks>
    internal class SelectSqlStatementCollection : Collection<SelectSqlStatement>,ICloneable
    {
         #region 字段

        /// <summary>
        /// SQL元素集合
        /// </summary>
        private List<SelectSqlStatement> selectSqlStatements;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectSqlStatementCollection()
        {
            selectSqlStatements = new List<SelectSqlStatement>();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 按ID获取SQL元素
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>SQL元素</returns>
        public SelectSqlStatement this[string id]
        {
            get
            {
                return (from element in selectSqlStatements
                        where element.Id == id
                        select element).First();
            }
            set
            {
                //DataValidator.CheckForNullReference(id, "id");
                var ele = (from element in selectSqlStatements
                        where element.Id == id
                        select element).First();

                ele = value;
            }
        }

        /// <summary>
        /// 确定SQL元素是否存在
        /// </summary>
        /// <param name="id">SQL元素ID</param>
        /// <returns>true: 存在; false: 不存在</returns>
        public bool Contains(string id)
        {
            var element = this[id];
            return element == null;
        }

        /// <summary>
        /// 移除指定ID的SQL元素
        /// </summary>
        /// <param name="id">SQL元素的ID</param>
        /// <returns>如果已从 ICollection<T> 中成功移除 item，则为 true；否则为 false</returns>
        public bool Remove(string id)
        {
            var selectSqlStatement = (from element in selectSqlStatements
                              where element.Id == id
                              select element).First();

            return Remove(selectSqlStatement);
        }        

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>SQL元素集合</returns>
        public virtual object Clone()
        {
            SelectSqlStatementCollection collection = new SelectSqlStatementCollection();

            foreach (SelectSqlStatement item in this)
            {
                collection.Add(item.Clone() as SelectSqlStatement);
            }

            return collection;
        }

        /// <summary>
        /// 转换成SQL集合
        /// </summary>
        /// <returns>SQL集合</returns>
        public IEnumerable<string> ToSQL()
        {
            var sqls = new List<string>();

            if (selectSqlStatements == null) return sqls;

            foreach (var element in selectSqlStatements)
            {
                sqls.Add(element.ToSQL());
            }


            return sqls;
        }


        #endregion                         
    }
}
