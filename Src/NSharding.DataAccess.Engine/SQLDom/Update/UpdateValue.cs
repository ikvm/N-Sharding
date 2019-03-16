// ===============================================================================
// 浪潮GSP平台
// ***类说明***
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013-03-11 17:35:03        1.0        周祥国           新建。
// ===============================================================================
// 开发者: 周祥国
// 2013-03-11 17:35:03 
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
    /// Update语句中的插入值类。
    /// </summary>
    /// <remarks>Update语句中的插入值类。</remarks>
    public class UpdateValue : FieldValue
    {
        #region 常量

        public const string UPDATEVALUE = "UpdateValue";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public UpdateValue()
            : base()
        {

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
            return string.Empty;
        }

        #endregion
    }
}