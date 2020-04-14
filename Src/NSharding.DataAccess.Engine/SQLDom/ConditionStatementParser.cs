using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    class ConditionStatementParser
    {
        public static ConditionStatement ParseOrderByClauses(List<OrderByClause> orderByClauses, DomainObject domainObject, DataObject dataObject)
        {
            var orderbyCondition = new ConditionStatement();
            if (orderByClauses == null || orderByClauses.Count == 0)
                return orderbyCondition;

            var list = new List<string>();
            foreach (var orderByClause in orderByClauses)
            {
                var orderByString = ParseOrderByClause(orderByClause, domainObject, dataObject);
                if (!string.IsNullOrWhiteSpace(orderByString))
                    list.Add(orderByString);
            }

            if (list.Count > 0)
            {
                orderbyCondition.ConditionString = string.Format("ORDER BY {0}", string.Join(",", list));
            }

            return orderbyCondition;
        }

        public static string ParseOrderByClause(OrderByClause orderByClause, DomainObject domainObject, DataObject dataObject)
        {
            var fieldType = orderByClause.FieldType;
            var columns = new Dictionary<string, DataColumn>();
            var elements = new Dictionary<string, DomainObjectElement>();
            foreach (var field in orderByClause.OrderByFields)
            {
                DataColumn column = null;
                DomainObjectElement element = null;

                switch (fieldType)
                {
                    case FieldType.Column:
                        column = dataObject.Columns.FirstOrDefault(i => i.ColumnName == field);
                        element = domainObject.Elements.FirstOrDefault(i => i.DataColumnID == column.ID);
                        break;
                    case FieldType.Element:
                        element = domainObject.Elements.FirstOrDefault(i => i.Name == field);
                        column = dataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID);
                        break;
                    case FieldType.FunctionExpression:
                        throw new NotSupportedException("OrderByClause.FieldType.FunctionExpression");
                }

                columns.Add(field, column);
                elements.Add(field, element);
            }

            return string.Format(" {0} {1}", string.Join(",", elements.Values.Select(i => i.Alias)),
                orderByClause.OrderByType.ToString());
        }

        public static FilterConditionStatement ParseFiletrClauses(List<FilterClause> filterClauses, DomainObject domainObject, DataObject dataObject)
        {
            var filterConditionStatement = new FilterConditionStatement();
            var filterConditions = new List<ConditionStatement>();

            if (filterClauses == null || filterClauses.Count == 0)
                return filterConditionStatement;

            var filterGroups = filterClauses.GroupBy(i => i.FilterField.GroupName);
            if (filterGroups.Count() > 1)
            {
                foreach (var group in filterGroups)
                {
                    var subFilterConditionState = new FilterConditionStatement();
                    foreach (var filter in group)
                    {
                        var conditionStatement = new ConditionStatement();
                        conditionStatement.SetRelationOperator(filter.LogicalOperator);
                        conditionStatement.ConditionString = ConditionStatementParser.ParseFilterClause(filter, domainObject, dataObject);
                        subFilterConditionState.ChildCollection.Add(conditionStatement);
                    }

                    filterConditionStatement.ChildCollection.Add(subFilterConditionState);
                }
            }
            else
            {
                foreach (var filter in filterClauses)
                {
                    var conditionStatement = new ConditionStatement();
                    conditionStatement.SetRelationOperator(filter.LogicalOperator);
                    conditionStatement.ConditionString = ConditionStatementParser.ParseFilterClause(filter, domainObject, dataObject);
                    filterConditionStatement.ChildCollection.Add(conditionStatement);
                }
            }

            return filterConditionStatement;
        }

        public static string ParseFilterClause(FilterClause filterClause, NSharding.DomainModel.Spi.DomainObject domainObject, DataObject dataObject)
        {
            var fieldType = filterClause.FilterField.FieldType;
            DataColumn column = null;
            DomainObjectElement element = null;
            switch (fieldType)
            {
                case FieldType.Column:
                    column = dataObject.Columns.FirstOrDefault(i => i.ColumnName == filterClause.FilterField.Field);
                    element = domainObject.Elements.FirstOrDefault(i => i.DataColumnID == column.ID);
                    break;
                case FieldType.Element:
                    element = domainObject.Elements.FirstOrDefault(i => i.Name == filterClause.FilterField.Field);
                    column = dataObject.Columns.FirstOrDefault(i => i.ID == element.DataColumnID);
                    break;
                case FieldType.FunctionExpression:
                    return filterClause.FilterField.Field;
            }

            var relationOper = RelationalOperatorUtis.ConvertToString(filterClause.RelationalOperator);
            if (column.IsNumeric)
            {
                if (filterClause.FilterFieldValue.IsNull)
                {
                    return string.Format("{0} is null ", element.Alias);
                }
                else
                {
                    if (relationOper.Value)
                    {
                        return string.Format("{0}{1}{2}", element.Alias, relationOper.Key,
                            filterClause.FilterFieldValue.FiledValue);
                    }
                    else
                    {
                        return string.Format("{0}{1}", element.Alias, relationOper.Key,
                           filterClause.FilterFieldValue.FiledValue);
                    }
                }
            }
            else
            {
                if (filterClause.FilterFieldValue.IsNull)
                {
                    return string.Format("{0} is null ", element.Alias);
                }
                else
                {
                    if (relationOper.Value)
                    {
                        return string.Format("{0}{1}'{2}'", element.Alias, relationOper.Key,
                            filterClause.FilterFieldValue.FiledValue);
                    }
                    else
                    {
                        return string.Format("{0}{1}", element.Alias, relationOper.Key,
                           filterClause.FilterFieldValue.FiledValue);
                    }
                }
            }
        }
    }
}
