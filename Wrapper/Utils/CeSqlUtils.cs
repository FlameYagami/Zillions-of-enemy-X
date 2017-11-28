using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Wrapper.Constant;
using Wrapper.Model;

namespace Wrapper.Utils
{
    public class CeSqlUtils : SqlUtils
    {
        public static string GetAddSql(CeSearchModel card)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO " + TableName);
            builder.Append(ColumnCard);
            builder.Append("VALUES(");
            builder.Append($"'{Md5Utils.GetMd5(card.JName + card.CostValue + card.PowerValue)}',");
            builder.Append($"'{GetAccurateValue(card.Type)}',");
            builder.Append($"'{GetAccurateValue(card.Camp)}',");
            builder.Append($"'{GetAccurateValue(card.Race)}',");
            builder.Append($"'{GetAccurateValue(card.Sign)}',");
            builder.Append($"'{GetAccurateValue(card.Rare)}',");
            builder.Append($"'{GetAccurateValue(card.Pack)}',");
            builder.Append($"'{card.CName}',");
            builder.Append($"'{card.JName}',");
            builder.Append($"'{card.Illust}',");
            builder.Append($"'{card.Number}',");
            builder.Append($"'{card.CostValue}',");
            builder.Append($"'{card.PowerValue}',");
            builder.Append($"'{card.Ability}',");
            builder.Append($"'{card.Lines}',");
            builder.Append($"'{JsonUtils.Serializer(new List<string> { card.Number })}',");
            builder.Append($"'{GetAbilityDetailJson(card.AbilityDetailModels.ToList())}'");
            // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        public static string GetDeleteSql(string number)
        {
            return $"DELETE FROM {TableName} WHERE {ColumnNumber}='{number}'";
        }

        public static string GetUpdateSql(CeSearchModel card, string number)
        {
            var builder = new StringBuilder();
            builder.Append($"UPDATE {TableName} SET ");
            builder.Append(
                $"{ColumnMd5}='{Md5Utils.GetMd5(card.JName + card.CostValue + card.PowerValue)}',");
            builder.Append($"{ColumnType}='{card.Type}',");
            builder.Append($"{ColumnCamp}= '{card.Camp}',");
            builder.Append($"{ColumnRace}= '{card.Race}',");
            builder.Append($"{ColumnSign}= '{card.Sign}',");
            builder.Append($"{ColumnRare}= '{card.Rare}',");
            builder.Append($"{ColumnPack}= '{card.Pack}',");
            builder.Append($"{ColumnCName}= '{card.CName}',");
            builder.Append($"{ColumnJName}= '{card.JName}',");
            builder.Append($"{ColumnIllust}= '{card.Illust}',");
            builder.Append($"{ColumnNumber}= '{card.Number}',");
            builder.Append($"{ColumnCost}= '{card.CostValue}',");
            builder.Append($"{ColumnPower}= '{card.PowerValue}',");
            builder.Append($"{ColumnAbility}= '{card.Ability}',");
            builder.Append($"{ColumnLines}= '{card.Lines}',");
            builder.Append(
                $"{ColumnImage}= '{JsonUtils.Serializer(new List<string> { card.Number })}',");
            builder.Append(
                $"{ColumnAbilityDetail}= '{GetAbilityDetailJson(card.AbilityDetailModels.ToList())}'");
            // 详细能力处理
            builder.Append($" WHERE {ColumnNumber}='{number}'");
            return builder.ToString();
        }

        /// <summary>
        ///     获取同卡密的修改语句
        /// </summary>
        public static string GetUpdateSql(CeSearchModel card)
        {
            var md5 = Md5Utils.GetMd5(card.JName + card.CostValue + card.PowerValue);
            var builder = new StringBuilder();
            builder.Append($"UPDATE {TableName} SET ");
            builder.Append($"{ColumnType}='{card.Type}',");
            builder.Append($"{ColumnCamp}= '{card.Camp}',");
            builder.Append($"{ColumnRace}= '{card.Race}',");
            builder.Append($"{ColumnSign}= '{card.Sign}',");
            builder.Append($"{ColumnRare}= '{card.Rare}',");
            builder.Append($"{ColumnCName}= '{card.CName}',");
            builder.Append($"{ColumnJName}= '{card.JName}',");
            builder.Append($"{ColumnCost}= '{card.CostValue}',");
            builder.Append($"{ColumnPower}= '{card.PowerValue}',");
            builder.Append($"{ColumnAbility}= '{card.Ability}',");
            builder.Append(
                $"{ColumnAbilityDetail}= '{GetAbilityDetailJson(card.AbilityDetailModels.ToList())}'");
            // 详细能力处理
            builder.Append($" WHERE {ColumnMd5}='{md5}'");
            return builder.ToString();
        }
    }
}
