using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 内连接子项类
    /// </summary>
    /// <remarks>SQL语句中的内连接</remarks>
    public class InnerJoinItem : SqlElement
    {
        #region 常量

        /// <summary>
        /// 内连接子项常量
        /// </summary>
        public const string INNERJOINITEM = "InnerJoinItem";

        /// <summary>
        /// 内连接表常量
        /// </summary>
        public const string INNERJOINTABLE = "InnerJoinTable";

        /// <summary>
        /// 额外条件常量
        /// </summary>
        public const string ADDITIONALCONDITION = "AdditionalCondition";

        #endregion

        #region 字段

        /// <summary>
        /// 是否使用条件
        /// </summary>
        private bool isUseCondition = false;

        /// <summary>
        /// 
        /// </summary>
        private string additionalCondition;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public InnerJoinItem()
            : base()
        {
            this.InnerJoinTable = new SqlTable();
            base.CreateChildCollection();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 内连接表
        /// </summary>
        public SqlTable InnerJoinTable { get; set; }

        /// <summary>
        /// 是否使用条件
        /// </summary>
        public bool IsUseExCondition
        {
            get
            {
                return isUseCondition;
            }
        }

        /// <summary>
        /// 附加条件
        /// </summary>
        /// <remarks>
        /// 对应于“关联上的Where条件”和“引用模型上的过滤条件”的与运算
        /// </remarks>
        public string AdditionalCondition
        {
            get
            {
                return additionalCondition;
            }
            set
            {
                additionalCondition = value;
                if (string.IsNullOrEmpty(additionalCondition))
                {
                    isUseCondition = false;
                }
                else
                {
                    isUseCondition = true;
                }
            }
        }

        /// <summary>
        /// 是否扩展项
        /// </summary>
        public bool IsExtendItem { get; set; }


        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>内连接子项类</returns>
        public override object Clone()
        {
            var newItem = base.Clone() as InnerJoinItem;
            if (InnerJoinTable != null)
                newItem.InnerJoinTable = InnerJoinTable;

            return newItem;
        }

        /// <summary>
        /// 转换成SQL
        /// </summary>
        /// <returns>SQL</returns>
        public override string ToSQL()
        {
            if (this.IsExtendItem == true)
                return string.Empty;
            StringBuilder result = new StringBuilder();

            result.Append(" JOIN ").Append(this.InnerJoinTable.ToSQL()).Append(" ON ");

            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                result.Append(this.ChildCollection[index].ToSQL());
                if (index < this.ChildCollection.Count - 1)
                    result.Append(" AND ");
            }

            if (this.IsUseExCondition == true)
                result.Append(" AND ").Append(this.AdditionalCondition);

            return result.ToString();
        }

        /// <summary>
        /// 特殊的InnerJoin处理
        /// </summary>
        /// <returns>SQL</returns>
        public string ToSQLEx()
        {
            var result = new StringBuilder(600);

            result.Append(" JOIN ").Append(InnerJoinTable.ToSQL()).Append(" ON ");

            for (int index = 0; index < this.ChildCollection.Count; index++)
            {
                result.Append(this.ChildCollection[index].ToSQL());
                if (index < this.ChildCollection.Count - 1)
                    result.Append(" AND ");
            }

            if (IsUseExCondition)
                result.Append(" AND ").Append(AdditionalCondition);

            return result.ToString();
        }

        #endregion
    }
}
