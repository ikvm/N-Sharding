// ===============================================================================
// 浪潮GSP平台
// SQLDOM元素类型
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/6/10 22:46:37        1.0        周国庆           初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/6/10 22:46:37 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQLDOM元素类型
    /// </summary>
    /// <remarks>SQLDOM元素类型</remarks>
    internal enum SqlElementType
    {
        InsertField = 0,
        SqlPrimaryKeyField = 1,
        SelectListField = 2,
        UpdateField = 3,
        SubQuerySqlStatement = 4,
        From = 5,
        FromItem = 6,
        JoinCondition = 7,
        FilterCondition = 8,
        OrderByCondition = 9,

        UnKnown = 10
    }
}
