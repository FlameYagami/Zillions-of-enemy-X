using System.Data;
using System.Linq;
using System.Text;
using CardEditor.Constant;
using DeckEditor.Utils;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Entity;
using Wrapper.Utils;

namespace DeckEditor.Model
{
    internal interface IQuery
    {
        CardEntity MemoryCardEntity { get; set; }
        void SetPreCardList(DataSet dsPartCache,string restrictQuery);
        string GetQuerySql(CardEntity cardEntity, string preOrder);
    }

    internal class Query : SqliteConst, IQuery
    {
        public CardEntity MemoryCardEntity { get; set; }

        public void SetPreCardList(DataSet dsPartCache, string restrictQuery)
        {
            DataCache.PreEntityList.Clear();
            foreach (var row in dsPartCache.Tables[TableName].Rows.Cast<DataRow>())
            {
                var md5 = row[ColumnMd5].ToString();
                var name = row[ColumnCName].ToString();
                var camp = row[ColumnCamp].ToString();
                var race = row[ColumnRace].ToString();
                race = race.Equals(string.Empty) ? StringConst.Hyphen : race;
                var cost = row[ColumnCost].ToString();
                cost = cost.Equals(string.Empty) || cost.Equals("0") ? StringConst.Hyphen : cost;
                var power = row[ColumnPower].ToString();
                power = power.Equals(string.Empty) || power.Equals("0") ? StringConst.Hyphen : power;
                var number = row[ColumnNumber].ToString();
                var imageJson = row[ColumnImage].ToString();
                var imagePath = CardUtils.GetThumbnailPathList(imageJson)[0];
                var restrict = RestrictUtils.GetRestrict(md5);
                var restrictPath = CardUtils.GetRestrictPath(restrict);
                DataCache.PreEntityList.Add(new PreviewEntity
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
            DataCache.PreEntityList = RestrictUtils.GetRestrictCardList(DataCache.PreEntityList, restrictQuery);
        }

        public string GetQuerySql(CardEntity cardEntity, string preOrder)
        {
            MemoryCardEntity = cardEntity; // 保存查询的实例
            var previewOrderType = CardUtils.GetPreOrderType(preOrder);
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql()); // 基础查询语句
            builder.Append(SqlUtils.GetAllKeySql(cardEntity.Key)); // 关键字
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Type, ColumnType)); // 种类
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Camp, ColumnCamp)); // 阵营
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Race, ColumnRace)); // 种族
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Sign, ColumnSign)); // 标记
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Rare, ColumnRare)); // 罕贵
            builder.Append(SqlUtils.GetAccurateSql(cardEntity.Illust, ColumnIllust)); // 画师
            builder.Append(SqlUtils.GetPackSql(cardEntity.Pack, ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Cost, ColumnCost)); // 费用
            builder.Append(SqlUtils.GetIntervalSql(cardEntity.Power, ColumnPower)); // 力量
            builder.Append(cardEntity.AbilityTypeSql); //  能力类型
            builder.Append(cardEntity.AbilityDetailSql); // 详细能力
            builder.Append(SqlUtils.GetFooterSql(previewOrderType)); // 排序
            return builder.ToString(); // 完整的查询语句
        }
    }
}