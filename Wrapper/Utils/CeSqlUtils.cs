using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Wrapper.Constant;
using Wrapper.Model;

namespace Wrapper.Utils
{
    public class CeSqlUtils : SqlUtils
    {
        public static string GetAddSql(CeQueryModel card)
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
            builder.Append($"'{JsonUtils.Serializer(new List<string> {card.Number})}',");
            builder.Append($"'{GetAbilityDetailJson(card.AbilityDetailModels.ToList())}'");
            // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        public static string GetDeleteSql(string number)
        {
            return $"DELETE FROM {TableName} WHERE {ColumnNumber}='{number}'";
        }

        public static string GetUpdateSql(CeQueryModel card, string number)
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
            builder.Append($"{ColumnRe}= '{GetReValue(card.Re)}',"); // 只有修改时才会变更源数数据
            builder.Append(
                $"{ColumnImage}= '{JsonUtils.Serializer(new List<string> {card.Number})}',");
            builder.Append(
                $"{ColumnAbilityDetail}= '{GetAbilityDetailJson(card.AbilityDetailModels.ToList())}'");
            // 详细能力处理
            builder.Append($" WHERE {ColumnNumber}='{number}'");
            return builder.ToString();
        }

        /// <summary>
        ///     获取同卡密的修改语句
        /// </summary>
        public static string GetUpdateSql(CeQueryModel card)
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

        public static string GetEditorSql(CeQueryModel card, Enums.PreviewOrderType preOrderType)
        {
            var builder = new StringBuilder();
            builder.Append(GetHeaderSql());
            builder.Append(GetPackSql(card.Pack, ColumnPack)); // 卡包
            builder.Append(GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public static string GetQuerySql(CeQueryModel card, Enums.PreviewOrderType preOrderType)
        {
            // 提取排序参数
            var builder = new StringBuilder();
            builder.Append(GetHeaderSql());
            builder.Append(GetAccurateSql(card.Type, ColumnType)); // 种类
            builder.Append(GetAccurateSql(card.Camp, ColumnCamp)); // 阵营
            builder.Append(GetAccurateSql(card.Race, ColumnRace)); // 种族
            builder.Append(GetAccurateSql(card.Sign, ColumnSign)); // 标记
            builder.Append(GetAccurateSql(card.Rare, ColumnRare)); // 罕贵

            builder.Append(GetSimilarSql(card.CName, ColumnCName)); // 卡名
            builder.Append(GetSimilarSql(card.JName, ColumnJName)); // 日名
            builder.Append(GetSimilarSql(card.Illust, ColumnIllust)); // 画师
            builder.Append(GetSimilarSql(card.Number, ColumnNumber)); // 卡编
            builder.Append(GetSimilarSql(card.Ability, ColumnAbility)); // 能力

            builder.Append(GetIntervalSql(card.CostValue, ColumnCost)); // 费用
            builder.Append(GetIntervalSql(card.PowerValue, ColumnPower)); // 力量

            builder.Append(GetPackSql(card.Pack, ColumnPack)); // 卡包
            builder.Append(GetAbilityTypeSql(card.AbilityTypeModels.ToList())); //  能力类型
            builder.Append(GetAbilityDetailSql(card.AbilityDetailModels.ToList())); // 详细能力
            builder.Append(GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }
    }
}