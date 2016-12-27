using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CardEditor.Constant;
using CardEditor.Entity;
using CardEditor.Utils;
using Common;

namespace CardEditor.Model
{
    public interface IQuery
    {
        void SetCardList();
        string GetQuerySql(CardEntity cardEntity, StringConst.PreviewOrderType previewOrderType);
        string GetEditorSql(CardEntity cardEntity, StringConst.PreviewOrderType previewOrderType);
        string GetUpdateSql(CardEntity cardEntity, string number);
        string GetAddSql(CardEntity cardEntity);
        string GetDeleteSql(string number);
        StringConst.AbilityType AnalysisAbility(string ability);
    }

    internal class Query : SqliteConst, IQuery
    {
        private static string _memoryQuerySql;

        private static string _memoryNumber;

        /// <summary>ListView数据缓存</summary>
        public static List<PreviewEntity> PreviewList { get; set; }

        /// <summary>记忆中的查询语句</summary>
        public static string MemoryQuerySql
        {
            get { return _memoryQuerySql ?? string.Empty; }
            set { _memoryQuerySql = value; }
        }

        /// <summary>记忆中的编号</summary>
        public static string MemoryNumber
        {
            get { return _memoryNumber ?? string.Empty; }
            set { _memoryNumber = value; }
        }

        public string GetQuerySql(CardEntity cardEntity, StringConst.PreviewOrderType previewOrderType)
        {
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Type, ColumnType)); // 种类
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Camp, ColumnCamp)); // 阵营
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Race, ColumnRace)); // 种族
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Sign, ColumnSign)); // 标记
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Rare, ColumnRare)); // 罕贵
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.CName, ColumnCName)); // 卡名
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.JName, ColumnJName)); // 日名
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.Illust, ColumnIllust)); // 画师
            builder.Append(SqlUtils.GetPackSql(cardEntity.Pack, ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetSimilarSql(cardEntity.Number, ColumnNumber)); // 卡编
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Cost, ColumnCost)); // 费用
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Power, ColumnPower)); // 力量
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Restrict, ColumnRestrict)); // 限制
            builder.Append(cardEntity.AbilityType); // 
            builder.Append(cardEntity.AbilityDetail); //
            MemoryQuerySql = builder.ToString(); // 除排序外的查询语句
            builder.Append(SqlUtils.GetFooterSql(previewOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public string GetEditorSql(CardEntity cardEntity, StringConst.PreviewOrderType previewOrderType)
        {
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());
            builder.Append(SqlUtils.GetPackSql(cardEntity.Pack, ColumnPack)); // 卡包
            MemoryQuerySql = builder.ToString(); // 除排序外的查询语句
            builder.Append(SqlUtils.GetFooterSql(previewOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public string GetAddSql(CardEntity cardEntity)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO " + TableName);
            builder.Append(ColumnCard);
            builder.Append("VALUES(");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Type)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Camp)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Race)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Sign)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Rare)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Pack)}',");
            builder.Append($"'{(cardEntity.Restrict.Equals(StringConst.NotApplicable) ? "4" : cardEntity.Restrict)}',");
            builder.Append($"'{cardEntity.CName}',");
            builder.Append($"'{cardEntity.JName}',");
            builder.Append($"'{cardEntity.Illust}',");
            builder.Append($"'{cardEntity.Number}',");
            builder.Append($"'{cardEntity.Cost}',");
            builder.Append($"'{cardEntity.Power}',");
            builder.Append($"'{cardEntity.Ability}',");
            builder.Append($"'{cardEntity.Lines}',");
            builder.Append($"'{cardEntity.Faq}',");
            builder.Append($"'{JsonUtils.JsonSerializer(new List<string> { cardEntity.Image })}',");
            builder.Append($"'{JsonUtils.JsonSerializer(cardEntity.AbilityDetialEntity)}'"); // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        public void SetCardList()
        {
            if (null == PreviewList)
                PreviewList = new List<PreviewEntity>();
            else
                PreviewList.Clear();
            foreach (var row in DataCache.DsPartCache.Tables[TableName].Rows.Cast<DataRow>())
            {
                var cost = row[ColumnCost].ToString();
                cost = cost.Equals(string.Empty) || cost.Equals("0")  ? StringConst.Hyphen : cost;
                var power = row[ColumnPower].ToString();
                power = power.Equals(string.Empty) || power.Equals("0") ? StringConst.Hyphen : power;
                var imageJson = row[ColumnImage].ToString();
                PreviewList.Add(new PreviewEntity
                {
                    CName = row[ColumnCName].ToString(),
                    Cost = cost,
                    Power = power,
                    Number = row[ColumnNumber].ToString(),
                    ImageJson = imageJson
                });
            }
        }

        /// <summary>
        ///     返回删除语句
        /// </summary>
        /// <param name="number">卡片编号</param>
        /// <returns></returns>
        public string GetDeleteSql(string number)
        {
            return $"DELETE FROM {TableName} WHERE {ColumnNumber}='{number}'";
        }

        /// <summary>
        ///     返回更新语句
        /// </summary>
        /// <param name="cardEntity"></param>
        /// <param name="number">卡片编号</param>
        /// <returns></returns>
        public string GetUpdateSql(CardEntity cardEntity, string number)
        {
            var builder = new StringBuilder();
            builder.Append($"UPDATE {TableName} SET ");
            builder.Append($"{ColumnType}='{cardEntity.Type}',");
            builder.Append($"{ColumnCamp}= '{cardEntity.Camp}',");
            builder.Append($"{ColumnRace}= '{cardEntity.Race}',");
            builder.Append($"{ColumnSign}= '{cardEntity.Sign}',");
            builder.Append($"{ColumnRare}= '{cardEntity.Rare}',");
            builder.Append($"{ColumnPack}= '{cardEntity.Pack}',");
            builder.Append(
                $"{ColumnRestrict}='{(cardEntity.Restrict.Equals(StringConst.NotApplicable) ? "4" : cardEntity.Restrict)}',");
            builder.Append($"{ColumnCName}= '{cardEntity.CName}',");
            builder.Append($"{ColumnJName}= '{cardEntity.JName}',");
            builder.Append($"{ColumnIllust}= '{cardEntity.Illust}',");
            builder.Append($"{ColumnNumber}= '{cardEntity.Number}',");
            builder.Append($"{ColumnCost}= '{cardEntity.Cost}',");
            builder.Append($"{ColumnPower}= '{cardEntity.Power}',");
            builder.Append($"{ColumnAbility}= '{cardEntity.Ability}',");
            builder.Append($"{ColumnLines}= '{cardEntity.Lines}',");
            builder.Append($"{ColumnFaq}= '{cardEntity.Faq}',");
            builder.Append($"{ColumnImage}= '{JsonUtils.JsonSerializer(new List<string> { cardEntity.Image })}',");
            builder.Append($"{ColumnAbilityDetail}= '{JsonUtils.JsonSerializer(cardEntity.AbilityDetialEntity)}'");
                // 详细能力处理
            builder.Append($" WHERE {ColumnNumber}='{number}'");
            return builder.ToString();
        }

        public StringConst.AbilityType AnalysisAbility(string ability)
        {
            if (ability.Contains("降临条件") || ability.Contains("觉醒条件"))
                return StringConst.AbilityType.Extra;
            if (ability.Contains("【★】"))
                return StringConst.AbilityType.Event;
            if (ability.Contains("【常】生命恢复") || ability.Contains("【常】虚空使者"))
                return StringConst.AbilityType.Ig;
            if (ability.Contains("【常】起始卡"))
                return StringConst.AbilityType.Start;
            return StringConst.AbilityType.None;
        }

        public List<PreviewEntity> GetCardList()
        {
            return PreviewList;
        }
    }
}