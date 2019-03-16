using NSharding.DataAccess.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// 数据访问引擎内部统一入口
    /// </summary>
    public class DataAccessEngine
    {
        private static object syncObj = new object();

        private static DataAccessEngine instance;


        /// <summary>
        /// 构造函数
        /// </summary>
        private DataAccessEngine()
        {

        }

        /// <summary>
        /// 获取数据访问引擎实例
        /// </summary>
        /// <returns>数据访问引擎实例</returns>
        public static DataAccessEngine GetInstance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new DataAccessEngine();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 获取SQL构造器
        /// </summary>
        /// <returns>SQL构造器</returns>
        public ISQLBuilder GetSqlBuilder()
        {
            return SQLBuilderImpl.GetInstance();
        }

        /// <summary>
        /// 获取数据查询服务
        /// </summary>
        /// <returns>数据查询服务</returns>
        public IDataQueryService GetDataQueryService()
        {
            return new DataQueryService();
        }

        /// <summary>
        /// 获取数据保存服务
        /// </summary>
        /// <returns>数据保存服务</returns>
        public IDataSaveService GetDataSaveService()
        {
            return new DataSaveService();
        }

        /// <summary>
        /// 获取数据保存服务
        /// </summary>
        /// <returns>数据保存服务</returns>
        public IDataUpdateService GetDataUpdateService()
        {
            return new DataUpdateService();
        }

        /// <summary>
        /// 获取数据删除服务
        /// </summary>
        /// <returns>数据删除服务</returns>
        public IDataDeleteService GetDataDeleteService()
        {
            return new DataDeleteService();
        }
    }
}
