using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace DeckEditor.Model
{
    internal interface IQuery
    {
        CardQueryModel MemoryCardQueryModel { get; set; }
        List<CardPreviewModel> GetCardPreviewList(string sql, string restrictQuery);
        string GetQuerySql(CardQueryModel card);
    }

    internal class Query : SqliteConst, IQuery
    {
        public CardQueryModel MemoryCardQueryModel { get; set; }

        public List<CardPreviewModel> GetCardPreviewList(string sql, string restrictQuery)
        {
            var dataSet = new DataSet();
            SqliteUtils.FillDataToDataSet(sql, dataSet);
            var cardPreviewModelList = new List<CardPreviewModel>();
            foreach (var row in dataSet.Tables[TableName].Rows.Cast<DataRow>())
            {
                var md5 = row[ColumnMd5].ToString();
                var name = row[ColumnCName].ToString();
                var camp = row[ColumnCamp].ToString();
                var race = row[ColumnRace].ToString();
                race = race.Equals(string.Empty) ? StringConst.Hyphen : race;
                var cost = row[ColumnCost].ToString();
                cost = cost.Equals("0") ? StringConst.Hyphen : cost;
                var power = row[ColumnPower].ToString();
                power = power.Equals("0") ? StringConst.Hyphen : power;
                var number = row[ColumnNumber].ToString();
                var imageJson = row[ColumnImage].ToString();
                var imagePath = CardUtils.GetThumbnailPathList(imageJson)[0];
                var restrict = RestrictUtils.GetRestrict(md5);
                var restrictPath = CardUtils.GetRestrictPath(restrict);
                cardPreviewModelList.Add(new CardPreviewModel
                {
                    CName = name,
                    CampAndRace = camp + " / " + race,
                    PowerAndCost = power + " / " + cost,
                    Number = number,
                    ImageJson = imageJson,
                    ImagePath = imagePath,
                    RestrictPath = restrictPath
                });
            }
            return RestrictUtils.GetRestrictCardList(cardPreviewModelList, restrictQuery);
        }

        public string GetQuerySql(CardQueryModel card)
        {
            MemoryCardQueryModel = card; // 保存查询的实例
            var previewOrderType = CardUtils.GetPreOrderType(card.Order);
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql()); // 基础查询语句
            builder.Append(SqlUtils.GetAllKeySql(card.Key)); // 关键字
            builder.Append(SqlUtils.GetAccurateSql(card.Type, ColumnType)); // 种类
            builder.Append(SqlUtils.GetAccurateSql(card.Camp, ColumnCamp)); // 阵营
            builder.Append(SqlUtils.GetAccurateSql(card.Race, ColumnRace)); // 种族
            builder.Append(SqlUtils.GetAccurateSql(card.Sign, ColumnSign)); // 标记
            builder.Append(SqlUtils.GetAccurateSql(card.Rare, ColumnRare)); // 罕贵
            builder.Append(SqlUtils.GetAccurateSql(card.Illust, ColumnIllust)); // 画师
            builder.Append(SqlUtils.GetPackSql(card.Pack, ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetIntervalSql(card.Cost, ColumnCost)); // 费用
            builder.Append(SqlUtils.GetIntervalSql(card.Power, ColumnPower)); // 力量
            builder.Append(SqlUtils.GetAbilityTypeSql(card.AbilityTypeDic)); //  能力类型
            builder.Append(SqlUtils.GetAbilityDetailSql(card.AbilityDetailDic)); // 详细能力
            builder.Append(SqlUtils.GetFooterSql(previewOrderType)); // 排序
            return builder.ToString(); // 完整的查询语句
        }
    }
}