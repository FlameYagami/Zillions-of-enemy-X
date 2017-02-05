using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CardEditor.Constant;
using CardEditor.Entity;
using CardEditor.Model;
using Common;
using Enum = CardEditor.Constant.Enum;

namespace CardEditor.Utils
{
    internal class SqlUtils : SqliteConst
    {
        public static string GetQueryAllSql()
        {
            return "SELECT * FROM " + TableName + " ORDER BY " + ColumnNumber + " ASC";
        }

        /// <summary>
        ///     获取头部查询语句
        /// </summary>
        /// <returns></returns>
        public static string GetHeaderSql()
        {
            return "SELECT * FROM " + TableName + " WHERE 1=1";
        }

        /// <summary>
        ///     获取尾部查询语句
        /// </summary>
        /// <param name="previewOrderType">排序枚举类型</param>
        /// <returns></returns>
        public static string GetFooterSql(Enum.PreviewOrderType previewOrderType)
        {
            return previewOrderType.Equals(Enum.PreviewOrderType.Number)
                ? GetOrderNumberSql()
                : GetOrderValueSql();
        }

        /// <summary>
        ///     获取卡编排序方式查询语句
        /// </summary>
        /// <returns></returns>
        private static string GetOrderNumberSql()
        {
            return " ORDER BY " + ColumnNumber + " ASC";
        }

        /// <summary>
        ///     获取数值排序方式查询语句
        /// </summary>
        /// <returns></returns>
        private static string GetOrderValueSql()
        {
            return " ORDER BY " + ColumnCamp + " DESC," + ColumnRace + " ASC," + ColumnCost + " DESC," + ColumnPower +
                   " DESC," +
                   ColumnJName + " DESC";
        }


        /// <summary>
        ///     获取精确取值
        /// </summary>
        /// <param name="value"></param>
        /// <returns>数据库查询语句</returns>
        public static string GetAccurateValue(string value)
        {
            return StringConst.NotApplicable.Equals(value) ? string.Empty : value.Replace("'", "''");
            // 处理字符串中对插入语句影响的'号
        }

        /// <summary>
        ///     获取精确查询语句
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column">数据库字段</param>
        /// <returns>数据库查询语句</returns>
        public static string GetAccurateSql(string value, string column)
        {
            return !StringConst.NotApplicable.Equals(value) ? $" AND {column}='{value}'" : string.Empty;
        }

        /// <summary>
        ///     获取模糊查询语句
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column">数据库字段</param>
        /// <returns>数据库查询语句</returns>
        public static string GetSimilarSql(string value, string column)
        {
            return !string.Empty.Equals(value) ? $" AND {column} LIKE '%{value}%'" : string.Empty;
        }

        /// <summary>
        ///     获取数值查询语句（适用范围:费用、力量）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column">数据库字段</param>
        /// <returns>数据库查询语句</returns>
        public static string GetIntervalSql(string value, string column)
        {
            return !string.Empty.Equals(value)
                ? (value.Contains(StringConst.Hyphen)
                    ? $" AND {column}>='{value.Split('-')[0]}' AND {column}<='{value.Split('-')[1]}'"
                    : $" AND {column}='{value}'")
                : string.Empty;
        }

        /// <summary>
        ///     获取卡包查询语句
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetPackSql(string value, string column)
        {
            if (StringConst.NotApplicable.Equals(value))
                return string.Empty;
            value = value.IndexOf(StringConst.Series, StringComparison.Ordinal) >= 0 ? value.Substring(0, 1) : value;
            return $" AND {column} LIKE '%{value}%'";
        }

        /// <summary>
        ///     获取能力查询语句（适用范围:能力类型、详细能力）
        /// </summary>
        /// <param name="listbox"></param>
        /// <param name="abilityitems"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetAbilitySql(ItemsControl listbox, List<string> abilityitems, string column)
        {
            var value = new StringBuilder();
            for (var i = 0; i != listbox.Items.Count; i++)
            {
                var isChecked = ((CheckBox) listbox.Items[i]).IsChecked;
                if ((isChecked != null) && (bool) isChecked)
                    value.Append($" AND {column} LIKE '%{abilityitems[i]}%'");
            }
            return value.ToString();
        }

        public static string GetExportSql(string pack)
        {
            return $"SELECT * FROM {TableName} WHERE {ColumnPack} LIKE '%{pack}%' {GetOrderNumberSql()}";
        }

        public static List<string> GetPciturePathList()
        {
            var numberList = DataCache.DsAllCache.Tables[TableName].AsEnumerable()
                .Select(column => $"Update TableCard Set Image = '{JsonUtils.JsonSerializer(new List<string> { "/" + column[ColumnNumber].ToString() + ".jpg" })}' WHERE Number='{column[ColumnNumber].ToString()}'")
                .ToList();
            return numberList;
        }

        public static List<string> GetMd5SqlList()
        {
            var cardEntities = DataCache.DsAllCache.Tables[TableName].AsEnumerable()
                .Select(column => new CardEntity()
                {
                    Number = column[ColumnNumber].ToString(),
                    Cost = column[ColumnCost].ToString(),
                    Power = column[ColumnPower].ToString()
                }).ToList();
            return (from entity in cardEntities let md5 = Md5Utils.GetMd5(entity.JName + entity.Cost + entity.Power) select $"UPDATE {TableName} SET Md5 = '{md5}' WHERE {ColumnNumber} = '{entity.Number}'").ToList();
        }
    }
}