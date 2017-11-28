using System.Collections.Generic;
using System.Data;
using Common;
using Wrapper.Constant;

namespace Wrapper
{
    public class DataManager
    {
        /// <summary>�����ݻ���</summary>
        public static DataSet DsAllCache = new DataSet();

        public const string DbName = "Data.db";
        public const string DbCfgKey = "DatabasePassword";

        public static string DatabasePath = PathManager.RootPath + DbName;

        public static bool FillDataToDataSet(DataSet dataSet, string sql, bool defTableName = true)
        {
            return defTableName
                ? SqliteUtils.FillDataToDataSet(dataSet, SqliteConst.TableName, DatabasePath, DbCfgKey, sql)
                : SqliteUtils.FillDataToDataSet(dataSet, DatabasePath, DbCfgKey, sql);
        }

        public static bool FillDataToDataSet(DataSet dataSet, string sql, string dsTableName)
        {
            return SqliteUtils.FillDataToDataSet(dataSet, dsTableName, DatabasePath, DbCfgKey, sql);
        }

        public static bool Execute(string sql)
        {
            return SqliteUtils.Execute(DatabasePath, DbCfgKey, sql);
        }

        public static bool Execute(List<string> sqlList)
        {
            return SqliteUtils.Execute(DatabasePath, DbCfgKey, sqlList);
        }

        public static bool Encrypt()
        {
            return SqliteUtils.Encrypt(DatabasePath, DbCfgKey);
        }

        public static bool Encrypt(DataSet dataSet, string sql, string dtTableName)
        {
            return SqliteUtils.Encrypt(dataSet, dtTableName, DatabasePath, DbCfgKey, sql);
        }

        public static bool Encrypt(DataSet dataSet, string sql, bool defTableName = true)
        {
            return defTableName 
                ? SqliteUtils.Encrypt(dataSet, SqliteConst.TableName, DatabasePath, DbCfgKey, sql)
                : SqliteUtils.Encrypt(dataSet, null, DatabasePath, DbCfgKey, sql);
        }

        public static bool Decrypt()
        {
            return SqliteUtils.Decrypt(DatabasePath, DbCfgKey);
        }
    }
}