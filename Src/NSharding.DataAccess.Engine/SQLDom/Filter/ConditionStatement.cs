using System;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 动态条件SQL语句类
    /// </summary>
    /// <remarks>动态条件</remarks>
    [Serializable]
    public class ConditionStatement : SqlElement
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConditionStatement()
            : base()
        {
            base.CreateChildCollection();
        }

        #region 常量

        /// <summary> 
        /// ConditionStatement 
        /// </summary>
        public const string CONDITIONSTATEMENT = "ConditionStatement";

        /// <summary> 
        /// ConditionString 
        /// </summary>
        public const string CONDITIONSTRING = "ConditionString";

        #endregion

        #region 属性

        public OperatorType relationOperator = OperatorType.And;

        /// <summary>
        /// 关系运算符
        /// </summary>
        public OperatorType RelationOperator
        {
            get
            {
                return relationOperator;
            }
            set
            {
                relationOperator = value;
            }
        }

        /// <summary>
        /// 动态条件SQL字符串
        /// </summary>
        public string ConditionString { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>SQL语句</returns>
        public override string ToSQL()
        {
            return ConditionString;
        }

        #endregion
    }
}
