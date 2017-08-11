using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.Model
{
    public interface ICardEditor
    {
        string MemoryNumber { get; set; }
        CardEditorModel MemoryEditorCardModel { get; set; }
        List<CardPreviewModel> GetCardPreviewList(string sql, string restrictQuery);
        string GetQuerySql(CardEditorModel card);
        string GetEditorSql(CardEditorModel card);
        bool UpdateCard(CardEditorModel card, string number);
        bool AddCard(CardEditorModel card);
        bool DeleteCard(string number);
        Enum.AbilityType AnalysisAbility(string ability);
    }

    internal class CardEditor : SqliteConst, ICardEditor
    {
        private string _memoryNumber;

        public CardEditorModel MemoryEditorCardModel { get; set; }

        public string MemoryNumber
        {
            get { return _memoryNumber ?? string.Empty; }
            set { _memoryNumber = value; }
        }

        public string GetEditorSql(CardEditorModel card)
        {
            MemoryEditorCardModel = card;
            var preOrderType = CardUtils.GetPreOrderType(card.Order);
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());
            builder.Append(SqlUtils.GetPackSql(card.Pack, ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public bool AddCard(CardEditorModel card)
        {
            var addSql = GetAddSql(card);
            // 添加数据是否成功
            if (!SqliteUtils.Execute(addSql)) return false;
            SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache); // 刷新总数据缓存
            MemoryNumber = card.Number; // 添加成功后记录添加的编号，以便显示位置
            MemoryEditorCardModel = card;
            return true;
        }

        public static string GetAddSql(CardEditorModel card)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO " + TableName);
            builder.Append(ColumnCard);
            builder.Append("VALUES(");
            builder.Append($"'{Md5Utils.GetMd5(card.JName + card.Cost + card.Power)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Type)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Camp)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Race)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Sign)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Rare)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Pack)}',");
            builder.Append($"'{card.CName}',");
            builder.Append($"'{card.JName}',");
            builder.Append($"'{card.Illust}',");
            builder.Append($"'{card.Number}',");
            builder.Append($"'{card.Cost}',");
            builder.Append($"'{card.Power}',");
            builder.Append($"'{card.Ability}',");
            builder.Append($"'{card.Lines}',");
            builder.Append($"'{card.ImageJson}',");
            builder.Append($"'{JsonUtils.JsonSerializer(new AbilityDetialModel(card.AbilityDetailDic, true))}'"); // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        ///     返回删除语句
        /// </summary>
        /// <param name="number">卡片编号</param>
        /// <returns></returns>
        public bool DeleteCard(string number)
        {
            var deleteSql = GetDeleteSql(number);
            if (!SqliteUtils.Execute(deleteSql)) return false;
            SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
            MemoryNumber = string.Empty; // 删除成功后清空记录的编号，不显示位置
            return true;
        }

        public string GetDeleteSql(string number)
        {
            return $"DELETE FROM {TableName} WHERE {ColumnNumber}='{number}'";
        }

        /// <summary>
        ///     返回更新语句
        /// </summary>
        /// <param name="card"></param>
        /// <param name="number">卡片编号</param>
        /// <returns></returns>
        public bool UpdateCard(CardEditorModel card, string number)
        {
            var updateSql = GetUpdateSql(card, number);
            if (!SqliteUtils.Execute(updateSql)) return false;
            SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
            MemoryEditorCardModel= card;
            return true;
        }

        public string GetUpdateSql(CardEditorModel card, string number)
        {
            var builder = new StringBuilder();
            builder.Append($"UPDATE {TableName} SET ");
            builder.Append($"{ColumnMd5}='{Md5Utils.GetMd5(card.JName + card.Cost + card.Power)}',");
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
            builder.Append($"{ColumnCost}= '{card.Cost}',");
            builder.Append($"{ColumnPower}= '{card.Power}',");
            builder.Append($"{ColumnAbility}= '{card.Ability}',");
            builder.Append($"{ColumnLines}= '{card.Lines}',");
            builder.Append($"{ColumnImage}= '{card.ImageJson}',");
            builder.Append($"{ColumnAbilityDetail}= '{JsonUtils.JsonSerializer(new AbilityDetialModel(card.AbilityDetailDic, true))}'");
            // 详细能力处理
            builder.Append($" WHERE {ColumnNumber}='{number}'");
            return builder.ToString();
        }

        public string GetQuerySql(CardEditorModel card)
        {
            MemoryEditorCardModel = card;
            var preOrderType = CardUtils.GetPreOrderType(card.Order);
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());
            builder.Append(SqlUtils.GetAccurateSql(card.Type, ColumnType)); // 种类
            builder.Append(SqlUtils.GetAccurateSql(card.Camp, ColumnCamp)); // 阵营
            builder.Append(SqlUtils.GetAccurateSql(card.Race, ColumnRace)); // 种族
            builder.Append(SqlUtils.GetAccurateSql(card.Sign, ColumnSign)); // 标记
            builder.Append(SqlUtils.GetAccurateSql(card.Rare, ColumnRare)); // 罕贵
            builder.Append(SqlUtils.GetSimilarSql(card.CName, ColumnCName)); // 卡名
            builder.Append(SqlUtils.GetSimilarSql(card.JName, ColumnJName)); // 日名
            builder.Append(SqlUtils.GetSimilarSql(card.Illust, ColumnIllust)); // 画师
            builder.Append(SqlUtils.GetPackSql(card.Pack, ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetSimilarSql(card.Number, ColumnNumber)); // 卡编
            builder.Append(SqlUtils.GetAccurateSql(card.Cost, ColumnCost)); // 费用
            builder.Append(SqlUtils.GetAccurateSql(card.Power, ColumnPower)); // 力量
            builder.Append(SqlUtils.GetAbilityTypeSql(card.AbilityTypeDic)); // 
            builder.Append(SqlUtils.GetAbilityTypeSql(card.AbilityDetailDic)); //
            builder.Append(SqlUtils.GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public List<CardPreviewModel> GetCardPreviewList(string sql, string restrictQuery)
        {
            var dataSet = new DataSet();
            SqliteUtils.FillDataToDataSet(sql, dataSet);
            var previewModelList = new List<CardPreviewModel>();
            var tempPreviewModleList =
                dataSet.Tables[TableName].Rows.Cast<DataRow>().Select(row => new CardPreviewModel()
                {
                    CName = row[ColumnCName].ToString(),
                    Cost = row[ColumnCost].Equals("0") ? StringConst.Hyphen : row[ColumnCost].ToString(),
                    Power = row[ColumnPower].Equals("0") ? StringConst.Hyphen : row[ColumnPower].ToString(),
                    Number = row[ColumnNumber].ToString(),
                    ImageJson = row[ColumnImage].ToString(),
                    Restrict = RestrictUtils.GetRestrict(row[ColumnMd5].ToString()).ToString()
                }).ToList();
            previewModelList.AddRange(tempPreviewModleList);
            return RestrictUtils.GetRestrictCardList(previewModelList, restrictQuery);
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