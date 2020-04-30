using NSharding.Sharding.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.UnitTest
{
    class DomainModelBuilder
    {
        /// <summary>
        /// 充电订单数据对象
        /// </summary>
        /// <returns>充电订单数据对象</returns>
        public DataObject CreateCM_ChargeBillDataObject()
        {
            var dataObject = new DataObject
            {
                ID = "SalesOrders",
                LogicTableName = "SalesOrders",
                Name = "销售订单",
                DataSourceName = "SD",
                IsTableSharding = false,
                IsView = false,              
                Version = 1
            };

            #region Columns

            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_ID", ColumnName = "ID", DisplayName = "ID", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "varchar" }, IsNullable = false, IsPkColumn = true, Length = 36 });
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_Code", ColumnName = "Code", DisplayName = "Code", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "varchar" }, IsNullable = false, IsPkColumn = false, Length = 36 });
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_Description", ColumnName = "Description", DisplayName = "Description", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "varchar" }, IsNullable = true, IsPkColumn = false, Length = 36 });
           
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_Price", ColumnName = "Price", DisplayName = "Price", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "decimal" }, IsNullable = true, IsPkColumn = false, Length = 9, Precision = 2 });
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_Tax", ColumnName = "Tax", DisplayName = "Tax", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "decimal" }, IsNullable = true, IsPkColumn = false, Length = 9, Precision = 2 });
           
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_Customer", ColumnName = "Customer", DisplayName = "Customer", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "varchar" }, IsNullable = true, IsPkColumn = false, Length = 36 });
            
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_CreateTime", ColumnName = "CreateTime", DisplayName = "CreateTime", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "datetime" }, IsNullable = true, IsPkColumn = false, Length = 8 });
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_AdjustReason", ColumnName = "AdjustReason", DisplayName = "AdjustReason", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "varchar" }, IsNullable = true, IsPkColumn = false, Length = 128 });
            dataObject.Columns.Add(new DataColumn() { ID = "SalesOrder_AccountingTime", ColumnName = "AccountingTime", DisplayName = "AccountingTime", DataObjectID = dataObject.ID, DataType = new DataType() { ID = "datetime" }, IsNullable = true, IsPkColumn = false, Length = 8 });

            #endregion

            return dataObject;
        }
    }
}
