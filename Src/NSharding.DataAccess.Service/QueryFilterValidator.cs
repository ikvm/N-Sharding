using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Service
{
    /// <summary>
    /// QueryFilter合法性校验类 
    /// </summary>
    class QueryFilterValidator
    {
        /// <summary>
        /// QueryFilter合法性校验
        /// </summary>
        /// <param name="domainObject">领域对象</param>
        /// <param name="queryFilter">查询过滤器</param>
        public static void Validate(DomainObject domainObject, QueryFilter queryFilter)
        {
            if (queryFilter == null)
            {
                throw new ArgumentNullException("QueryFilterValidator.Validate.queryFilter");
            }

            if (queryFilter.FilterClauses != null && queryFilter.FilterClauses.Count > 0)
            {
                foreach (var filterClause in queryFilter.FilterClauses)
                {
                    if (filterClause.FilterField != null)
                    {
                        switch (filterClause.FilterField.FieldType)
                        {
                            case FieldType.Column:
                                var column = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == filterClause.FilterField.Field || i.ColumnName == filterClause.FilterField.Field);
                                if (column == null)
                                {
                                    throw new Exception(string.Format("查询过滤条件中未找到列:" + filterClause.FilterField.Field));
                                }
                                break;
                            case FieldType.Element:
                                var element = domainObject.Elements.FirstOrDefault(i => i.ID == filterClause.FilterField.Field || i.Name == filterClause.FilterField.Field);
                                if (element == null)
                                {
                                    throw new Exception(string.Format("查询过滤条件中未找到元素:" + filterClause.FilterField.Field));
                                }
                                break;
                            default:
                                throw new NotSupportedException(filterClause.FilterField.FieldType.ToString());
                        }
                    }
                }
            }

            if (queryFilter.ProjectionFields != null && queryFilter.ProjectionFields.Count > 0)
            {
                foreach (var projectionField in queryFilter.ProjectionFields)
                {
                    if (projectionField != null)
                    {
                        switch (projectionField.FieldType)
                        {
                            case QueryProjectionFieldType.Column:
                                var column = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == projectionField.Content || i.ColumnName == projectionField.Content);
                                if (column == null)
                                {
                                    throw new Exception(string.Format("查询字段列表中未找到列:" + projectionField.Content));
                                }
                                break;
                            case QueryProjectionFieldType.Element:
                                var element = domainObject.Elements.FirstOrDefault(i => i.ID == projectionField.Content || i.Name == projectionField.Content);
                                if (element == null)
                                {
                                    throw new Exception(string.Format("查询字段列表中未找到元素:" + projectionField.Content));
                                }
                                break;
                            default:
                                throw new NotSupportedException(projectionField.FieldType.ToString());
                        }
                    }
                }
            }

            if (queryFilter.OrderByCondition != null && queryFilter.OrderByCondition.Count > 0)
            {
                foreach (var orderByClause in queryFilter.OrderByCondition)
                {
                    if (orderByClause != null)
                    {
                        if (orderByClause.OrderByFields == null || orderByClause.OrderByFields.Count == 0)
                        {
                            throw new Exception(string.Format("排序条件未设置字段:"));
                        }
                        switch (orderByClause.FieldType)
                        {
                            case FieldType.Column:
                                foreach (var col in orderByClause.OrderByFields)
                                {
                                    var column = domainObject.DataObject.Columns.FirstOrDefault(i => i.ID == col || i.ColumnName == col);
                                    if (column == null)
                                    {
                                        throw new Exception(string.Format("排序条件中未找到列:" + col));
                                    }
                                }
                                break;
                            case FieldType.Element:
                                foreach (var ele in orderByClause.OrderByFields)
                                {
                                    var element = domainObject.Elements.FirstOrDefault(i => i.ID == ele || i.Name == ele);
                                    if (element == null)
                                    {
                                        throw new Exception(string.Format("排序条件中未找到元素:" + ele));
                                    }
                                }
                                break;
                            default:
                                throw new NotSupportedException(orderByClause.FieldType.ToString());
                        }
                    }
                }
            }

        }
    }
}
