using System;
using System.Web;
using System.IO;
using System.Text;
using NSharding.Sharding.Database;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQLDOM内部工具类
    /// </summary>
    /// <remarks>SQLDOM内部工具类</remarks>
    public static class SQLBuilderUtils
    {
        #region 常量

        /// <summary>
        /// 创建时间
        /// </summary>
        public static readonly string CREATETIME = "@CreateTime@";

        /// <summary>
        /// 创建时间列名称
        /// </summary>
        public static readonly string CREATETIMECOLUMNNAME = "CreateTime";

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public static readonly string LASTCHANGEDTIME = "@LastChangeTime@";

        /// <summary>
        /// 最后修改时间列名称
        /// </summary>
        public static readonly string LASTCHANGEDTIMECOLUMNNAME = "LastChangeTime";

        /// <summary>
        /// 版本加一
        /// </summary>
        public static readonly string VERSIONPLUS1 = "GSPDataVersion+1";

        /// <summary>
        /// 标记为逻辑删除
        /// </summary>
        public static readonly int MARKEDDELETED = 1;

        /// <summary>
        /// 逻辑删除列
        /// </summary>
        public static readonly string ISDELETED = "ISDELETED";

        /// <summary>
        /// 创建人列名称
        /// </summary>
        public static readonly string CREATEUSERCOLUMNNAME = "CreatedBy";

        /// <summary>
        /// 最后修改人列名称
        /// </summary>
        public static readonly string LASTCHANGEDBYCOLUMNNAME = "LastChangedBy";

        /// <summary>
        /// 版本列名称
        /// </summary>
        public static readonly string VERSIONCOLUMNNAME = "GSPDataVersion";

        /// <summary>
        /// 增量数据中操作类型列名称
        /// </summary>
        public static readonly string OPTYPECOLUMNNAME = "GSPOPTYPE";

        /// <summary>
        /// 国际化多语言字段的列名称
        /// </summary>
        public static readonly string CULTURECOLUMNNAME = "GSPCulture";

        /// <summary>
        /// 修改的字段列名称
        /// </summary>
        public static readonly string UPDATACOLUMNSCOLUMNNAME = "UPDATACOLUMNS";

        /// <summary>
        /// 语言类型
        /// </summary>
        public static string CULTURETYPE = "zh-CN";

        /// <summary>
        /// 语言类型
        /// </summary>
        public static string ENCULTURETYPE = "en-US";

        #endregion

        #region 方法

        /// <summary>
        /// 获取当前数据库类型
        /// </summary>
        /// <param name="dbCode">数据库编号</param>
        /// <returns>当前数据库类型</returns>
        public static DbType GetCurrentDbType(string dbCode = "")
        {
            //return GSPDbType.SQLServer;

            return DbType.SQLServer;
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <returns>当前用户ID</returns>
        public static string GetCurrentUserID()
        {
            //try
            //{
            //    return GSPAppContext.Current["USERID"].ToString();
            //}
            //catch
            //{
            //    //TODO: 集成测试屏蔽
            //}

            return "9999";
        }

        /// <summary>
        /// 获取当前事件变量
        /// </summary>
        /// <returns>当前事件变量</returns>
        public static string GetCreateTime()
        {
            return CREATETIME;
        }

        public static string GetLastChangeTime()
        {
            return LASTCHANGEDTIME;
        }
        /// <summary>
        /// 获取实际数据库中获取当前时间函数
        /// </summary>
        /// <returns>实际数据库中获取当前时间函数</returns>
        public static string GetReallyDbDateTime()
        {
            switch (GetCurrentDbType())
            {
                case DbType.Oracle:
                    return "sysdate";
                case DbType.SQLServer:
                    return "GetDate()";
                default:
                    throw new NotSupportedException(GetCurrentDbType().ToString());
            }
        }


        #endregion

        internal static object GetColumnValue(DataColumn column)
        {
            if (column == null) return null;
            switch (column.ColumnName.ToUpper())
            {
                case "CREATEDBY":
                case "LASTCHANGEDBY":
                    return GetCurrentUserID();
                case "LASTCHANGETIME":
                case "CREATETIME":
                    return GetReallyDbDateTime();
                case "GSPDATAVERSION":
                    return string.Format("{0}+1", column.ColumnName);
                case "ISDELETED":
                    return 1;
                default:
                    return column.ColumnName;
            }
        }
    }
}
