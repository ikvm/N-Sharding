// ===============================================================================
// 浪潮GSP平台
// ***类说明***
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013-03-11 17:35:21        1.0        周祥国           新建。
// ===============================================================================
// 开发者: 周祥国
// 2013-03-11 17:35:21 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Update语句中的插入值集合类。
    /// </summary>
    /// <remarks>Update语句中的插入值集合类。</remarks>
    public class UpdateValueList : SqlElement
    {
        #region 常量

        public const string UPDATEVALUELIST = "UpdateValueList";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpdateValueList()
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
        /// <remarks>InsertSQL中的参数以:p1这种形式进行拼装</remarks>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            var result = new StringBuilder();

            result.Append(" VALUES(");
            for (int i = 0; i < this.ChildCollection.Count; i++)
            {
                result.Append(":P").Append(Convert.ToString(i));
                if (i < this.ChildCollection.Count - 1)
                    result.Append(", ");
            }

            return result.Append(") ").ToString();
        }

        #endregion
    }
}