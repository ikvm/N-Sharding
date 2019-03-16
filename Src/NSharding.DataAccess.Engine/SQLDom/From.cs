// ===============================================================================
// 浪潮GSP平台
// From类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/31 15:29:54        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/1/31 15:29:54 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// From类
    /// </summary>
    /// <remarks>SQL语句中的From</remarks>
    [Serializable]
    public class From : SqlElement
    {
        #region 常量

        /// <summary>
        /// From常量
        /// </summary>
        public const string FROM = "From";

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public From()
            : base()
        {
            base.CreateChildCollection();
        }

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            var fromList = new List<string>();
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                var from = this.ChildCollection[index].ToSQL();
                if (!string.IsNullOrWhiteSpace(from))
                    fromList.Add(from);
            }

            return string.Join(",", fromList);
            //if (ChildCollection == null || ChildCollection.Count == 0)
            //    return string.Empty;

            //return string.Join(",", ChildCollection.ToSQL());
        }

        #endregion

    }
}