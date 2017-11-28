using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace Common
{
    public class SqliteUtils
    {
        /// <summary>
        /// 数据填充至DataSet
        /// </summary>
        /// <param name="dataSet">DataSet数据集</param>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="dbCfgKey">数据库配置键名</param>
        /// <param name="sql">数据库语句</param>
        /// <returns>True:成功|Flase:失败</returns>
        public static bool FillDataToDataSet(DataSet dataSet, string dbPath, string dbCfgKey, string sql)
        {
            return FillDataToDataSet(dataSet, null, dbPath, dbCfgKey, sql);
        }

        /// <summary>
        /// 数据填充至DataSet
        /// </summary>
        /// <param name="dataSet">DataSet数据集</param>
        /// <param name="dtTableName">DataSet数据集表名称</param>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="dbCfgKey">数据库配置键名</param>
        /// <param name="sql">数据库语句</param>
        /// <returns>True:成功|Flase:失败</returns>
        public static bool FillDataToDataSet(DataSet dataSet, string dtTableName, string dbPath, string dbCfgKey, string sql)
        {
            var dataSource = GetDataSource(dbPath);
            var dataPassword = GetDataPassword(dbCfgKey);
            using (var con = new SQLiteConnection(dataSource))
            {
                try
                {
                    con.SetPassword(dataPassword);
                    con.Open();
                    var cmd = new SQLiteCommand(sql, con);
                    var dap = new SQLiteDataAdapter(cmd);
                    dataSet.Clear();
                    if (null == dtTableName)
                        dap.Fill(dataSet);
                    else
                        dap.Fill(dataSet, dtTableName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 数据库执行Sql语句
        /// </summary>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="dbCfgKey">数据库配置键名</param>
        /// <param name="sql">数据库语句</param>
        /// <returns>True:成功|Flase:失败</returns>
        public static bool Execute(string dbPath, string dbCfgKey, string sql)
        {
            var dataSource = GetDataSource(dbPath);
            var dataPassword = GetDataPassword(dbCfgKey);
            using (var con = new SQLiteConnection(dataSource))
            {
                using (var trans = con.BeginTransaction())
                {
                    try
                    {
                        con.SetPassword(dataPassword);
                        con.Open();
                        var cmd = new SQLiteCommand(sql, con);
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// 数据库执行Sql语句集合
        /// </summary>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="dbCfgKey">数据库配置键名</param>
        /// <param name="sqlList">数据库语句集合</param>
        /// <returns>True:成功|Flase:失败</returns>
        public static bool Execute(string dbPath, string dbCfgKey, List<string> sqlList)
        {
            var dataSource = GetDataSource(dbPath);
            var dataPassword = GetDataPassword(dbCfgKey);
            using (var con = new SQLiteConnection(dataSource))
            {
                con.SetPassword(dataPassword);
                con.Open();
                using (var trans = con.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(con))
                    {
                        try
                        {
                            cmd.Transaction = trans;
                            foreach (var sql in sqlList)
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            trans.Rollback();
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 加密数据库
        /// </summary>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="dbCfgKey">数据库配置键名</param>
        /// <returns>True:成功|Flase:失败</returns>
        public static bool Encrypt(string dbPath, string dbCfgKey)
        {
            return Encrypt(null, null, dbPath, dbCfgKey, null);
        }

        /// <summary>
        /// 加密数据库,并执行查询操作,填充DataSet数据集
        /// </summary>
        /// <param name="dataSet">DataSet数据集</param>
        /// <param name="dtTableName">DataSet数据集表名称</param>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="dbCfgKey">数据库配置键名</param>
        /// <param name="sql">数据库语句</param>
        /// <returns>True:成功|Flase:失败</returns>
        public static bool Encrypt(DataSet dataSet, string dtTableName, string dbPath, string dbCfgKey, string sql)
        {
            try
            {
                var dataSource = GetDataSource(dbPath);
                var dataPassword = GetDataPassword(dbCfgKey);
                using (var con = new SQLiteConnection(dataSource))
                {
                    con.Open();
                    con.ChangePassword(dataPassword);
                }
                return null == sql || FillDataToDataSet(dataSet, dtTableName, dbPath, dbCfgKey, sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 解密数据库
        /// </summary>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="dbCfgKey">数据库配置键名</param>
        /// <returns></returns>
        public static bool Decrypt(string dbPath, string dbCfgKey)
        {
            try
            {
                var dataSource = GetDataSource(dbPath);
                var dataPassword = GetDataPassword(dbCfgKey);
                using (var con = new SQLiteConnection(dataSource))
                {
                    con.SetPassword(dataPassword);
                    con.Open();
                    con.ChangePassword(string.Empty);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private static string GetDataSource(string dbPath)
        {
            return $"Data Source='{dbPath}'";
        }

        private static string GetDataPassword(string dbCfgKey)
        {
            var dbPwd = ConfigUtils.Get(dbCfgKey);
            return StringUtils.Decrypt(dbPwd);
        }
    }
}