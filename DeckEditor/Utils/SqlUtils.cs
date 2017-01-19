using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using DeckEditor.Constant;
using DeckEditor.Entity;
using Enum = DeckEditor.Constant.Enum;

namespace DeckEditor.Utils
{
    internal class SqlUtils : SqliteConst
    {
        public static string GetQueryAllSql()
        {
            return "SELECT * FROM " + TableName + " ORDER BY " + ColumnNumber + " ASC";
        }

        /// <summary>
        /// 获取头部查询语句
        /// </summary>
        /// <returns></returns>
        public static string GetHeaderSql()
        {
            return "SELECT * FROM " + TableName + " WHERE 1=1";
        }

        /// <summary>
        /// 获取尾部查询语句
        /// </summary>
        /// <param name="previewOrderType">排序枚举类型</param>
        /// <returns></returns>
        public static string GetFooterSql(Enum.PreviewOrderType previewOrderType)
        {
            return previewOrderType.Equals(Enum.PreviewOrderType.Number) ? GetOrderNumberSql() : GetOrderValueSql();
        }

        /// <summary>
        /// 获取卡编排序方式查询语句
        /// </summary>
        /// <returns></returns>
        private static string GetOrderNumberSql()
        {
            return " ORDER BY " + ColumnNumber + " ASC";
        }

        /// <summary>
        /// 获取数值排序方式查询语句
        /// </summary>
        /// <returns></returns>
        private static string GetOrderValueSql()
        {
            return " ORDER BY " + ColumnCamp + " DESC," + ColumnRace + " ASC," + ColumnCost + " DESC," + ColumnPower + " DESC," +
            ColumnJName + " DESC";
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
                    ? $" AND {column}>={value.Split('-')[0]} AND {column}<={value.Split('-')[1]}"
                    : $" AND {column}={value}")
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
        ///     获取能力类型的查询语句
        /// </summary>
        /// <param name="checkIEnumerable">能力类型</param>
        /// <returns></returns>
        public static string GetAbilityTypeSql(IEnumerable<CheckBox> checkIEnumerable)
        {
            var value = new StringBuilder();
            foreach (var checkbox in checkIEnumerable)
                if ((checkbox.IsChecked != null) && (bool) checkbox.IsChecked)
                    foreach (var abilityTypeItem in Dictionary.AbilityTypeDic)
                        if (abilityTypeItem.Key.Equals(checkbox.Content))
                            value.Append($" AND {ColumnAbility} LIKE '%{abilityTypeItem.Value}%'");
            return value.ToString();
        }

        /// <summary>
        ///     获取详细能力的查询语句
        /// </summary>
        /// <param name="abilityDetialEntity">能力分类模型</param>
        /// <returns></returns>
        public static string GetAbilityDetailSql(AbilityDetialEntity abilityDetialEntity)
        {
            var value = new StringBuilder();
            var abilityDetialDic = abilityDetialEntity.GetAbilityDetailDic();
            foreach (var abilityDetialItem in abilityDetialDic)
                if (abilityDetialItem.Value.Equals(1))
                    value.Append($" AND {ColumnAbilityDetail} LIKE '%\"{abilityDetialItem.Key}\":1%'");
            return value.ToString();
        }

        /// <summary>
        ///     获取关键字段取值
        /// </summary>
        /// <returns></returns>
        public static string GetAllKeySql(string value)
        {
            if (value.Equals(string.Empty)) return string.Empty;
            var tempValue = new StringBuilder();
            var keyList = value.Split(' '); // 以空格分割关键字
            foreach (var key in keyList)
                tempValue.Append($" AND ( JName LIKE '%{key}%' " + GetPartKeySql(key) + ")");
            return tempValue.ToString();
        }

        private static string GetPartKeySql(string value)
        {
            var tempValue = new StringBuilder();
            foreach (var column in ColumKeyArray)
                tempValue.Append($" OR {column} LIKE '%{value}%'");
            return tempValue.ToString();
        }
    }
}