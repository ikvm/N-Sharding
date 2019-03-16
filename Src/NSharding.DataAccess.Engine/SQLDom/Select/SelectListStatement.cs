// ===============================================================================
// 浪潮GSP平台
// 查询字段SQL语句类
// 请查看《GSP7-数据访问引擎子系统概要设计说明书》来了解关于此类的更多信息。
// ===============================================================================
// 变更历史纪录
// 时间			             版本	    修改人	        描述
// 2013/2/12 22:28:35        1.0        周国庆          初稿。
// ===============================================================================
// 开发者: 周国庆
// 2013/2/12 22:28:35 
// (C) 2013 Genersoft Corporation 版权所有
// 保留所有权利。
// ===============================================================================

using System;
using System.Text;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 查询字段SQL语句类
    /// </summary>
    /// <remarks>类的补充说明</remarks>
    [Serializable]
    public class SelectFieldListStatement : SqlElement
    {
        #region 常量

        /// <summary> 
        /// SelectListStatement 
        /// </summary>
        public const string SELECTLISTSTATEMENT = "SelectFieldListStatement";

        #endregion       

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectFieldListStatement()
        {
            base.CreateChildCollection();
        }

        #endregion       

        #region 方法

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            StringBuilder result = new StringBuilder("");
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                result.Append(this.ChildCollection[index].ToSQL());
                if (index < this.ChildCollection.Count - 1)
                    result.Append(",");
            }
            
            return result.ToString();
        }

        /// <summary>
        /// 转换成SQL(字段别名方式)
        /// </summary>
        /// <param name="elements">字段列表</param>
        /// <returns>SQL语句</returns>
        public Tuple<string, List<string>> ToSQL(List<string> elements)
        {
            var tableAlixes = new List<string>();
            string result = string.Empty;
            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                var node = this.ChildCollection[index];
                if (elements.Contains((node as SelectListField).FieldAlias) == false)
                    continue;

                result += node.ToSQL();
                if (index < this.ChildCollection.Count - 1)
                    result += ", ";
                if (tableAlixes.Contains((node as Field).Table.TableAlias) == false)
                    tableAlixes.Add((node as Field).Table.TableAlias);
            }
         
            return new Tuple<string, List<string>>(string.Format(" {0} ", result), tableAlixes);
        }

        #endregion
    }
}
