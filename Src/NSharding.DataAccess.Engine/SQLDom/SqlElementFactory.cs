// ===============================================================================
// 浪潮GSP平台
// ***类说明***
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/6/10 22:54:17        1.0        周国庆           初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/6/10 22:54:17 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// ***类说明***
    /// </summary>
    /// <remarks>类的补充说明</remarks>
    internal class SqlElementFactory
    {
        #region 常量

        #endregion

        #region 字段

        #endregion

        #region 构造函数

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 获取SQLDOM元素类型
        /// </summary>
        /// <param name="sqlElement">SQLDOM元素</param>
        /// <returns>SQLDOM元素类型</returns>
        public static int GetSqlElementType(SqlElement sqlElement)
        {
            SqlElementType elementType = SqlElementType.UnKnown;

            var type = sqlElement.GetType();

            if (type == typeof(InsertField))
            {
                elementType =  SqlElementType.InsertField;
            }
            if (type == typeof(SqlPrimaryKeyField))
            {
                elementType = SqlElementType.SqlPrimaryKeyField;
            }
            if (type == typeof(SelectListField))
            {
                elementType = SqlElementType.SelectListField;
            }
            if (type == typeof(UpdateField))
            {
                elementType = SqlElementType.UpdateField;
            }

            return (int)elementType;
        }

        /// <summary>
        /// 根据SQLDOM元素类型初始化SQLDOM元素对象
        /// </summary>
        /// <param name="sqlElementType">SQLDOM元素类型</param>
        /// <returns>SQLDOM元素对象</returns>
        public static SqlElement GetSQLElement(int sqlElementType)
        {
            SqlElement element = null;
            SqlElementType type = (SqlElementType)sqlElementType;
            switch (type)
            {
                case SqlElementType.InsertField:
                    element = new InsertField();
                    break;
                case SqlElementType.SqlPrimaryKeyField:
                    element = new SqlPrimaryKeyField();
                    break;
                case SqlElementType.SelectListField:
                    element = new SelectListField();
                    break;
                case SqlElementType.UpdateField:
                    element = new UpdateField();
                    break;
                case SqlElementType.From:
                    element = new From();
                    break;
                case SqlElementType.FromItem:
                    element = new FromItem();
                    break;
                case SqlElementType.JoinCondition:
                    element = new JoinConditionStatement();
                    break;
                case SqlElementType.OrderByCondition:
                    element = new ConditionStatement();
                    break;
                case SqlElementType.FilterCondition:
                    element = new ConditionStatement();
                    break;
                case SqlElementType.UnKnown:
                    break;
            }

            return element;
        }

        #endregion
    }
}
