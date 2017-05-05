using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using CardEditor.Constant;
using CardEditor.Utils;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Entity;
using Wrapper.Utils;

namespace CardEditor.Model
{
    public interface IQuery
    {
        string MemoryNumber { get; set; }
        CardEntity MemoryCardEntity { get; set; }
        void SetPreCardList(DataSet dataSet, string restrictQuery);
        string GetQuerySql(CardEntity cardEntity, string preOrder);
        string GetEditorSql(CardEntity cardEntity, string preOrder);
        string GetUpdateSql(CardEntity cardEntity, string number);
        string GetAddSql(CardEntity cardEntity);
        string GetDeleteSql(string number);
        Enum.AbilityType AnalysisAbility(string ability);
    }

    internal class Query : SqliteConst, IQuery
    {
        private string _memoryNumber;

        public CardEntity MemoryCardEntity { get; set; }

        public string MemoryNumber {
            get { return _memoryNumber ?? string.Empty; }
            set { _memoryNumber = value; }
        }

        public string GetQuerySql(CardEntity cardEntity, string preOrder)
        {
            MemoryCardEntity = cardEntity;
            var preOrderType = CardUtils.GetPreOrderType(preOrder);
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
            builder.Append(cardEntity.AbilityTypeSql); // 
            builder.Append(cardEntity.AbilityDetailSql); //
            builder.Append(SqlUtils.GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public string GetEditorSql(CardEntity cardEntity, string preOrder)
        {
            MemoryCardEntity = cardEntity;
            var preOrderType = CardUtils.GetPreOrderType(preOrder);
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());
            builder.Append(SqlUtils.GetPackSql(cardEntity.Pack, ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public string GetAddSql(CardEntity cardEntity)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO " + TableName);
            builder.Append(ColumnCard);
            builder.Append("VALUES(");
            builder.Append($"'{Md5Utils.GetMd5(cardEntity.JName + cardEntity.Cost + cardEntity.Power)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Type)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Camp)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Race)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Sign)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Rare)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(cardEntity.Pack)}',");
            builder.Append($"'{cardEntity.CName}',");
            builder.Append($"'{cardEntity.JName}',");
            builder.Append($"'{cardEntity.Illust}',");
            builder.Append($"'{cardEntity.Number}',");
            builder.Append($"'{cardEntity.Cost}',");
            builder.Append($"'{cardEntity.Power}',");
            builder.Append($"'{cardEntity.Ability}',");
            builder.Append($"'{cardEntity.Lines}',");
            builder.Append($"'{cardEntity.Faq}',");
            builder.Append($"'{cardEntity.ImageJson}',");
            builder.Append($"'{cardEntity.AbilityDetailJson}'"); // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        public void SetPreCardList(DataSet dataSet, string restrictQuery)
        {
            DataCache.PreEntityList.Clear();
            foreach (var row in dataSet.Tables[TableName].Rows.Cast<DataRow>())
            {
                var md5 = row[ColumnMd5].ToString();
                var cost = row[ColumnCost].ToString();
                cost = cost.Equals(string.Empty) || cost.Equals("0") ? StringConst.Hyphen : cost;
                var power = row[ColumnPower].ToString();
                power = power.Equals(string.Empty) || power.Equals("0") ? StringConst.Hyphen : power;
                var imageJson = row[ColumnImage].ToString();
                var restrict = RestrictUtils.GetRestrict(md5);
                DataCache.PreEntityList.Add(new PreviewEntity
                {
                    CName = row[ColumnCName].ToString(),
                    Cost = cost,
                    Power = power,
                    Number = row[ColumnNumber].ToString(),
                    ImageJson = imageJson,
                    Restrict = restrict.ToString()
                });
            }
            DataCache.PreEntityList = RestrictUtils.GetRestrictCardList(DataCache.PreEntityList, restrictQuery);
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
            builder.Append($"{ColumnMd5}='{Md5Utils.GetMd5(cardEntity.JName + cardEntity.Cost + cardEntity.Power)}',");
            MessageBox.Show(Md5Utils.GetMd5(cardEntity.JName + cardEntity.Cost + cardEntity.Power));
            builder.Append($"{ColumnType}='{cardEntity.Type}',");
            builder.Append($"{ColumnCamp}= '{cardEntity.Camp}',");
            builder.Append($"{ColumnRace}= '{cardEntity.Race}',");
            builder.Append($"{ColumnSign}= '{cardEntity.Sign}',");
            builder.Append($"{ColumnRare}= '{cardEntity.Rare}',");
            builder.Append($"{ColumnPack}= '{cardEntity.Pack}',");
            builder.Append($"{ColumnCName}= '{cardEntity.CName}',");
            builder.Append($"{ColumnJName}= '{cardEntity.JName}',");
            builder.Append($"{ColumnIllust}= '{cardEntity.Illust}',");
            builder.Append($"{ColumnNumber}= '{cardEntity.Number}',");
            builder.Append($"{ColumnCost}= '{cardEntity.Cost}',");
            builder.Append($"{ColumnPower}= '{cardEntity.Power}',");
            builder.Append($"{ColumnAbility}= '{cardEntity.Ability}',");
            builder.Append($"{ColumnLines}= '{cardEntity.Lines}',");
            builder.Append($"{ColumnFaq}= '{cardEntity.Faq}',");
            builder.Append($"{ColumnImage}= '{cardEntity.ImageJson}',");
            builder.Append($"{ColumnAbilityDetail}= '{cardEntity.AbilityDetailJson}'");
            // 详细能力处理
            builder.Append($" WHERE {ColumnNumber}='{number}'");
            return builder.ToString();
        }

        public Enum.AbilityType AnalysisAbility(string ability)
        {
            if (ability.Contains("降临条件") || ability.Contains("觉醒条件"))
                return Enum.AbilityType.Extra;
            if (ability.Contains("【★】"))
                return Enum.AbilityType.Event;
            if (ability.Contains("【常】生命恢复") || ability.Contains("【常】虚空使者"))
                return Enum.AbilityType.Ig;
            if (ability.Contains("【常】起始卡"))
                return Enum.AbilityType.Start;
            return Enum.AbilityType.None;
        }
    }
}