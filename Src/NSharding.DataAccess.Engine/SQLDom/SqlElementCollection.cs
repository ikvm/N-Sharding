// ===============================================================================
// 浪潮GSP平台
// SQL元素集合类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/28 22:59:23        1.0        周国庆         初稿。
// 2013/1/29 08:56:00        1.1        周国庆          基本实现。
// ===============================================================================
// 开发者: 周国庆
// 2013/1/28 22:59:23 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL元素集合类
    /// </summary>
    /// <remarks>SQL元素集合，实现ICollection接口</remarks>
    public class SqlElementCollection:Collection<SqlElement>,ICloneable
    {
        #region 字段

        /// <summary>
        /// SQL元素集合
        /// </summary>
        private List<SqlElement> SqlElements;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlElementCollection()
        {
            SqlElements = new List<SqlElement>();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 按ID获取SQL元素
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>SQL元素</returns>
        public SqlElement this[string id]
        {
            get
            {
                return (from element in SqlElements
                        where element.Id == id
                        select element).First();
            }
            set
            {
                //DataValidator.CheckForNullReference(id, "id");
                var ele = (from element in SqlElements
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
            return element != null;
        }

        /// <summary>
        /// 移除指定ID的SQL元素
        /// </summary>
        /// <param name="id">SQL元素的ID</param>
        /// <returns>如果已从 ICollection<T> 中成功移除 item，则为 true；否则为 false</returns>
        public bool Remove(string id)
        {
            var sqlElement = (from element in SqlElements
                              where element.Id == id
                              select element).First();

            return Remove(sqlElement);
        }        

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>SQL元素集合</returns>
        public virtual object Clone()
        {
            SqlElementCollection collection = new SqlElementCollection();

            foreach (SqlElement item in this)
            {
                collection.Add(item.Clone() as SqlElement);
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
 
            if (SqlElements == null) return sqls;

            foreach (var element in SqlElements)
            {
                sqls.Add(element.ToSQL());
            }


            return sqls;
        }


        #endregion                         
    }
}
