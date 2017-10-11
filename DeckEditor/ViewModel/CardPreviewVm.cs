using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using DeckEditor.Model;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace DeckEditor.ViewModel
{
    public class CardPreviewVm : BaseModel
    {
        private string _cardPreviewCountValue;
        private string _cardPreviewOrder;

        public CardPreviewVm()
        {
            CardPreviewModels = new ObservableCollection<CardPreviewModel>();
            PreviewOrderValues = Dic.PreviewOrderDic.Values.ToList();
        }

        public List<string> PreviewOrderValues { get; set; }

        public string CardPreviewCountValue
        {
            get { return _cardPreviewCountValue; }
            set
            {
                _cardPreviewCountValue = value;
                OnPropertyChanged(nameof(CardPreviewCountValue));
            }
        }

        public ObservableCollection<CardPreviewModel> CardPreviewModels { get; set; }

        public string CardPreviewOrder
        {
            get { return _cardPreviewOrder; }
            set
            {
                _cardPreviewOrder = value;
                OnPropertyChanged(nameof(CardPreviewOrder));
            }
        }

        private CardQueryModel MemoryQueryModel { get; set; }

        public void UpdateCardPreviewModels(CardQueryModel queryModel)
        {
            MemoryQueryModel = queryModel; // 保存查询的实例
            var dataSet = new DataSet();
            var sql = GetQuerySql(queryModel);
            SqliteUtils.FillDataToDataSet(sql, dataSet);

            var tempList = new List<CardPreviewModel>();
            foreach (var row in dataSet.Tables[SqliteConst.TableName].Rows.Cast<DataRow>())
            {
                var name = row[SqliteConst.ColumnCName].ToString();
                var camp = row[SqliteConst.ColumnCamp].ToString();
                var number = row[SqliteConst.ColumnNumber].ToString();
                var imageJson = row[SqliteConst.ColumnImage].ToString();
                var restrictPath = RestrictUtils.GetRestrictPath(row[SqliteConst.ColumnMd5].ToString());
                var imagePath = CardUtils.GetThumbnailPathList(imageJson)[0];
                var race = row[SqliteConst.ColumnRace].ToString();
                race = race.Equals(string.Empty) ? StringConst.Hyphen : race;
                var cost = row[SqliteConst.ColumnCost].ToString();
                cost = cost.Equals("0") || cost.Equals(string.Empty) ? StringConst.Hyphen : cost;
                var power = row[SqliteConst.ColumnPower].ToString();
                power = power.Equals("0") || power.Equals(string.Empty) ? StringConst.Hyphen : power;

                tempList.Add(new CardPreviewModel
                {
                    CName = name,
                    CampAndRace = camp + " / " + race,
                    PowerAndCost = power + " / " + cost,
                    Number = number,
                    ImagePath = imagePath,
                    ImageJson = imageJson,
                    RestrictPath = restrictPath
                });
            }
            CardPreviewModels.Clear();
            tempList.ForEach(CardPreviewModels.Add);
            CardPreviewCountValue = CardPreviewModels.Count.ToString();
        }

        /// <summary>
        ///     卡牌预览排序事件
        /// </summary>
        public void Order()
        {
            if (null == MemoryQueryModel) return;
            UpdateCardPreviewModels(MemoryQueryModel);
        }

        public string GetQuerySql(CardQueryModel card)
        {
            var previewOrderType = CardUtils.GetPreOrderType(CardPreviewOrder);
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql()); // 基础查询语句
            builder.Append(SqlUtils.GetAllKeySql(card.Key)); // 关键字
            builder.Append(SqlUtils.GetAccurateSql(card.Type, SqliteConst.ColumnType)); // 种类
            builder.Append(SqlUtils.GetAccurateSql(card.Camp, SqliteConst.ColumnCamp)); // 阵营
            builder.Append(SqlUtils.GetAccurateSql(card.Race, SqliteConst.ColumnRace)); // 种族
            builder.Append(SqlUtils.GetAccurateSql(card.Sign, SqliteConst.ColumnSign)); // 标记
            builder.Append(SqlUtils.GetAccurateSql(card.Rare, SqliteConst.ColumnRare)); // 罕贵
            builder.Append(SqlUtils.GetAccurateSql(card.Illust, SqliteConst.ColumnIllust)); // 画师
            builder.Append(SqlUtils.GetPackSql(card.Pack, SqliteConst.ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetIntervalSql(card.CostValue, SqliteConst.ColumnCost)); // 费用
            builder.Append(SqlUtils.GetIntervalSql(card.PowerValue, SqliteConst.ColumnPower)); // 力量
            builder.Append(SqlUtils.GetAbilityTypeSql(card.AbilityTypeModels.ToList())); //  能力类型
            builder.Append(SqlUtils.GetAbilityDetailSql(card.AbilityDetailModels.ToList())); // 详细能力
            builder.Append(SqlUtils.GetFooterSql(previewOrderType)); // 排序
            return builder.ToString(); // 完整的查询语句
        }
    }
}