using System;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using Common;
using DeckEditor.Constant;

namespace DeckEditor.Utils
{
    public class SqliteUtils
    {
        public const string DatabaseName = "Data.db";
        public const string DatabasePassword = "DatabasePassword";
        public static string DatabasePath = $"Data Source='{Const.RootPath + DatabaseName}'";

        /// <summary>数据填充至DataSet</summary>
        public static bool FillDataToDataSet(string sql, DataSet dts)
        {
            var password = StringUtils.Decrypt(ConfigUtils.Get(DatabasePassword));
            using (var con = new SQLiteConnection(DatabasePath))
            {
                try
                {
                    con.SetPassword(password);
                    con.Open();
                    var cmd = new SQLiteCommand(sql, con);
                    var dap = new SQLiteDataAdapter(cmd);
                    dts.Clear();
                    dap.Fill(dts, SqliteConst.TableName);
                    con.Close();
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }
    }
}