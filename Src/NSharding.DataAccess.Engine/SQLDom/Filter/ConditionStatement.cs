using NSharding.DataAccess.Spi;
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

        private OperatorType logicalOperator = OperatorType.And;

        /// <summary>
        /// 关系运算符
        /// </summary>
        public OperatorType LogicalOperator
        {
            get
            {
                return logicalOperator;
            }
            set
            {
                logicalOperator = value;
            }
        }

        public void SetLogicalOperator(LogicalOperator loperator)
        {
            switch (loperator)
            {
                case Spi.LogicalOperator.OR:
                    LogicalOperator = OperatorType.Or;
                    break;
                case Spi.LogicalOperator.AND:
                default:
                    LogicalOperator = OperatorType.And;
                    break;
            }
        }

        /// <summary>
        /// 动态条件SQL字符串
        /// </summary>
        public string ConditionString { get; set; }

        public bool IsExpression { get; set; }

        public ConditionField ConditionField { get; set; } = new ConditionField();

        public ConditionFieldValue ConditionFieldValue { get; set; } = new ConditionFieldValue();

        public RelationalOperator RelationalOperator { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 转换成SQL语句
        /// </summary>
        /// <returns>SQL语句</returns>
        public override string ToSQL()
        {
            if (string.IsNullOrEmpty(ConditionString) == false)
                return ConditionString;

            var relationOper = RelationalOperatorUtis.ConvertToString(RelationalOperator);

            if (ConditionFieldValue.IsNull)
            {
                return string.Format("{0}.{1} is null ", ConditionField.Table.TableName, ConditionField.FieldName);
            }
            else
            {
                if (relationOper.Value)
                {
                    return string.Format("{0}.{1} {2} @{3}", ConditionField.Table.TableName, ConditionField.FieldName, relationOper.Key,
                        ConditionFieldValue.ConditionFieldName);
                }
                else
                {
                    return string.Format(ConditionField.Table + "." + ConditionField.FieldName + " " + relationOper.Key,
                       ConditionFieldValue.ConditionFieldName);
                }
            }

            //if (ConditionField.IsNumericField)
            //{

            //}
            //else
            //{
            //if (ConditionFieldValue.IsNull)
            //{
            //    return string.Format("{0}.{1} is null ", ConditionField.Table, ConditionField.FieldName);
            //}
            //else
            //{
            //    if (relationOper.Value)
            //    {
            //        return string.Format("{0} {1}'{2}'", element.Alias, relationOper.Key,
            //            filterClause.FilterFieldValue.FiledValue);
            //    }
            //    else
            //    {
            //        return string.Format(element.Alias + " " + relationOper.Key,
            //           filterClause.FilterFieldValue.FiledValue);
            //    }
            //}
            //}
        }

        #endregion
    }
}
