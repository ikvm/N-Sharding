using NSharding.DataAccess.Spi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据上下文
    /// </summary>    
    public class DataContextItem : ICloneable
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataContextItem()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="columnOrdinals">列的位置集合</param>
        /// <param name="opType">数据操作类型</param>
        public DataContextItem(Hashtable data, Dictionary<int, string> columnOrdinals, DataAccessOpType opType)
        {
            this.data = data;
            this.OpType = opType;
            this.columnOrdinals = columnOrdinals;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="columnOrdinals">列的位置集合</param>
        /// <param name="primaryKeyData">主键数据</param>
        /// <param name="opType">数据操作类型</param>
        public DataContextItem(Hashtable data, Dictionary<int, string> columnOrdinals, Dictionary<string, object> primaryKeyData,
            DataAccessOpType opType)
            : this(data, columnOrdinals, opType)
        {
            this.primaryKeyData = primaryKeyData;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 数据
        /// </summary>
        private Hashtable data;

        /// <summary>
        /// 数据
        /// </summary>
        public Hashtable Data
        {
            get
            {
                if (data == null)
                    data = new Hashtable();

                return data;
            }
            private set
            {
                data = value;
            }
        }

        /// <summary>
        /// 列的位置集合
        /// 1 Name 2 Code
        /// </summary>
        public Dictionary<int, string> columnOrdinals;

        /// <summary>
        /// 列的位置集合
        /// 1 Name 2 Code
        /// </summary>
        public Dictionary<int, string> ColumnOrdinals
        {
            get
            {
                if (columnOrdinals == null)
                    columnOrdinals = new Dictionary<int, string>();

                return columnOrdinals;
            }
            private set
            {
                columnOrdinals = value;
            }
        }

        /// <summary>
        /// 主键数据
        /// </summary>
        private IDictionary<string, object> primaryKeyData;

        /// <summary>
        /// 主键数据
        /// </summary>
        public IDictionary<string, object> PrimaryKeyData
        {
            get
            {
                if (primaryKeyData == null)
                    primaryKeyData = new Dictionary<string, object>();

                return primaryKeyData;
            }
            set
            {
                primaryKeyData = value;
            }
        }

        /// <summary>
        /// 数据操作类型
        /// </summary>
        public DataAccessOpType OpType { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>节点对象的数据项的副本</returns>
        public object Clone()
        {
            var newData = this.MemberwiseClone() as DataContextItem;
            if (Data != null && Data.Count > 0)
            {
                newData.Data = new Hashtable();
                foreach (DictionaryEntry item in Data)
                {
                    newData.Data.Add(item.Key, item.Value);
                }
            }

            if (ColumnOrdinals != null && ColumnOrdinals.Count > 0)
            {
                newData.ColumnOrdinals = new Dictionary<int, string>();
                foreach (var item in ColumnOrdinals)
                {
                    newData.ColumnOrdinals.Add(item.Key, item.Value);
                }
            }

            return newData;
        }

        #endregion
    }
}
