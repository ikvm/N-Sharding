// ===============================================================================
// 浪潮GSP平台
// 更新字段列表类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/12 22:39:26        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/12 22:39:26 
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
    /// 更新字段列表类
    /// </summary>
    /// <remarks>Update语句中更新字段集合</remarks>
    [Serializable]
    public class UpdateFieldList : SqlElement
    {
        #region 常量

        /// <summary>
        /// UpdateFieldList
        /// </summary>
        public const string UPDATEFIELDLIST = "UpdateFieldList";

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpdateFieldList()
            : base()
        {
            base.CreateChildCollection();
        }

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>SQL语句</returns>
        public override string ToSQL()
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < this.ChildCollection.Count; i++)
            {
                UpdateField field = ChildCollection[i] as UpdateField;
                result.Append(this.ChildCollection[i].ToSQL()).Append(":").Append(field.FieldName);
                if (i < this.ChildCollection.Count - 1)
                    result.Append(", ");
            }

            return result.ToString();
        }

        #endregion
    }
}