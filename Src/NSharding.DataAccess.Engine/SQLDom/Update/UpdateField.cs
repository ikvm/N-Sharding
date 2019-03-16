// ===============================================================================
// 浪潮GSP平台
// Update语句中更新字段类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/1/31 22:57:16        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/1/31 22:57:16 
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
    /// Update语句中更新字段类
    /// </summary>
    /// <remarks>Update语句中的更新字段</remarks>
    public class UpdateField : Field
    {
        #region 常量

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        #endregion

        #region 属性

        /// <summary>
        /// 是否使用绑定变量
        /// </summary>
        public bool IsUseVarBinding { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL。
        /// </summary>
        /// <returns>SQL。</returns>
        public override string ToSQL()
        {
            return base.ToSQL() + " = ";// +this.fieldValue.Write();
        }

        #endregion
    }
}