using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
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
        private string _querySqlMemory;

        public string QuerySqlMemory
        {
            get { return _querySqlMemory ?? string.Empty; }
            set { _querySqlMemory = value; }
        }

        public void SetCardList(DataSet dsPartCache)
        {
            DataCache.InfoColl.Clear();
            var filePathlist = CardUtils.GetThumbnailFilePathList();
            foreach (var row in dsPartCache.Tables[TableName].Rows.Cast<DataRow>())
            {
                var camp = row[ColumnCamp].ToString();
                var race = row[ColumnRace].ToString();
                race = race.Equals(string.Empty) ? StringConst.Hyphen : race;
                var cost = row[ColumnCost].ToString();
                cost = cost.Equals(string.Empty) || cost.Equals("0") ? StringConst.Hyphen : cost;
                var power = row[ColumnPower].ToString();
                power = power.Equals(string.Empty) || power.Equals("0") ? StringConst.Hyphen : power;
                var number = row[ColumnNumber].ToString();
                var restrict = row[ColumnRestrict].ToString();
                var thumbnailPathList = CardUtils.GetThumbnailPathList(number, filePathlist);
                var imagePath = thumbnailPathList.Count.Equals(0) ? string.Empty : thumbnailPathList[0];
                var restrictPath = CardUtils.GetRestrictPath(restrict);
                DataCache.InfoColl.Add(new PreviewEntity
                {
                    CampAndRace = camp + " / " + race,
                    PowerAndCost = power + " / "+ cost,
                    Number = number,
                    ImagePath = imagePath,
                    RestrictPath = restrictPath
                });
            }
        }

        public string GetQuerySql(CardEntity cardEntity, StringConst.PreviewOrderType previewOrderType)
        {
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());// 基础查询语句
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
            builder.Append(cardEntity.AbilityType); //  能力类型
            builder.Append(cardEntity.AbilityDetail); // 详细能力
            QuerySqlMemory = builder.ToString(); // 将排序之前的查询语句记录在内存当中
            builder.Append(SqlUtils.GetFooterSql(previewOrderType)); // 排序
            return builder.ToString(); // 完整的查询语句
        }
    }
}