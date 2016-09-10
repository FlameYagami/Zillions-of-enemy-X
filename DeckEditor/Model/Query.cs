using System.Data;
using System.Linq;
using System.Text;
using DeckEditor.Constant;
using DeckEditor.Entity;
using DeckEditor.Utils;

namespace DeckEditor.Model
{
    internal interface IQuery
    {
        string QuerySqlMemory { get; set; }
        void SetCardList(DataSet dsPartCache);
        string GetQuerySql(CardEntity cardEntity, StringConst.PreviewOrderType previewOrderType);
    }

    internal class Query : SqliteConst, IQuery
    {
        public string QuerySqlMemory { get; set; }

        public Query()
        {
            QuerySqlMemory = string.Empty;
        }

        public void SetCardList(DataSet dsPartCache)
        {
            DataCache.InfoColl.Clear();
            var filePathlist = CardUtils.GetThumbnailFilePathList();
            foreach (var row in dsPartCache.Tables[TableCard].Rows.Cast<DataRow>())
            {
                var camp = row[ColumnCamp].ToString();
                var race = row[ColumnRace].ToString();
                var cost = row[ColumnCost].ToString();
                var power = row[ColumnPower].ToString();
                var number = row[ColumnNumber].ToString();
                var restrict = row[ColumnRestrict].ToString();
                var thumbnailPathList = CardUtils.GetThumbnailPathList(number, filePathlist);
                var restrictPath = CardUtils.GetRestrictPath(restrict);
                DataCache.InfoColl.Add(new PreviewEntity
                {
                    CName = row[ColumnCName].ToString(),
                    CampAndRace = camp + "/" + (race.Equals(string.Empty) ? StringConst.Hyphen : race),
                    PowerAndCost = (power.Equals(string.Empty) ? StringConst.Hyphen : power) + "/"
                                   + (cost.Equals(string.Empty) ? StringConst.Hyphen : cost),
                    Number = number,
                    ImagePath = thumbnailPathList.Count.Equals(0) ? string.Empty : thumbnailPathList[0],
                    Restrict = restrictPath
                });
            }
        }

        public string GetQuerySql(CardEntity cardEntity, StringConst.PreviewOrderType previewOrderType)
        {
            var builder = new StringBuilder();
            builder.Append(QueryBaseSql);
            builder.Append(SqlUtils.GetAllKeySql(cardEntity.Key)); // 关键字
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Type, ColumnType)); // 种类
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Camp, ColumnCamp)); // 阵营
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Race, ColumnRace)); // 种族
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Sign, ColumnSign)); // 标记
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Rare, ColumnRare)); // 罕贵
            builder.Append(SqlUtils.GetBaseSql(cardEntity.Illust, ColumnIllust)); // 画师
            builder.Append(SqlUtils.GetPackSql(cardEntity.Pack, ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Cost, ColumnCost)); // 费用
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Power, ColumnPower)); // 力量
            builder.Append(cardEntity.AbilityType); //  能力类型
            builder.Append(cardEntity.AbilityDetail); // 详细能力
            QuerySqlMemory = builder.ToString();
            builder.Append(previewOrderType.Equals(StringConst.PreviewOrderType.Number) ? OrderNumberSql : OrderValueSql);
                // 完整的查询语句
            return builder.ToString();
        }
    }
}