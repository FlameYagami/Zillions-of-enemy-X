using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Common;
using Wrapper.Constant;
using Wrapper.Model;

namespace Wrapper.Utils
{
    public class SqlUtils : SqliteConst
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
        public static string GetFooterSql(Enums.PreviewOrderType previewOrderType)
        {
            return previewOrderType.Equals(Enums.PreviewOrderType.Number)
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
        ///     获取精确查询语句
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column">数据库字段</param>
        /// <returns>数据库查询语句</returns>
        public static string GetAccurateSql(int value, string column)
        {
            return !0.Equals(value) ? $" AND {column}='{value}'" : string.Empty;
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

        public static string GetExportSql(string pack)
        {
            return $"SELECT * FROM {TableName} WHERE {ColumnPack} LIKE '%{pack}%' {GetOrderNumberSql()}";
        }

        public static List<string> GetPciturePathList()
        {
            var numberList = DataManager.DsAllCache.Tables[TableName].AsEnumerable()
                .Select(
                    column =>
                            $"Update {TableName} Set {ColumnImage} = '{JsonUtils.Serializer(new List<string> {column[ColumnNumber].ToString()})}' WHERE Number='{column[ColumnNumber].ToString()}'")
                .ToList();
            return numberList;
        }

        public static List<string> GetMd5SqlList()
        {
            var cardModels = DataManager.DsAllCache.Tables[TableName].AsEnumerable()
                .Select(column => new CardModel
                {
                    JName = column[ColumnJName].ToString(),
                    Number = column[ColumnNumber].ToString(),
                    Cost = int.Parse(column[ColumnCost].ToString()),
                    Power = int.Parse(column[ColumnPower].ToString())
                }).ToList();
            return (from entity in cardModels
                let md5 = Md5Utils.GetMd5(entity.JName + entity.Cost + entity.Power).ToUpper()
                select $"UPDATE {TableName} SET Md5 = '{md5}' WHERE {ColumnNumber} = '{entity.Number}'").ToList();
        }

        /// <summary>
        ///     获取能力类型的查询语句
        /// </summary>
        /// <param name="abilityModels">能力类型模型列表</param>
        /// <returns></returns>
        public static string GetAbilityTypeSql(List<AbilityModel> abilityModels)
        {
            var value = new StringBuilder();
            foreach (var abilityModel in abilityModels)
                if (abilityModel.Checked)
                    foreach (var abilityTypeItem in Dic.AbilityTypeDic)
                        if (abilityTypeItem.Key.Equals(abilityModel.Name))
                            value.Append($" AND {ColumnAbility} LIKE '%{abilityTypeItem.Value}%'");
            return value.ToString();
        }

        /// <summary>
        ///     获取详细能力的查询语句
        /// </summary>
        /// <param name="abilityModels">能力分类模型列表</param>
        /// <returns></returns>
        public static string GetAbilityDetailSql(List<AbilityModel> abilityModels)
        {
            var value = new StringBuilder();
            foreach (var abilityModel in abilityModels)
                if (abilityModel.Checked)
                    value.Append($" AND {ColumnAbilityDetail} LIKE '%[{abilityModel.Code},1]%'");
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


        protected static string GetAbilityDetailJson(IEnumerable<AbilityModel> abilityModels)
        {
            var abilityDetailList =
                abilityModels.Select(abilityModel => new List<int> {abilityModel.Code, abilityModel.Checked ? 1 : 0})
                    .ToList();
            return JsonUtils.Serializer(abilityDetailList);
        }

        protected static int GetReValue(bool re)
        {
            return re ? 1 : 0;
        }
    }
}