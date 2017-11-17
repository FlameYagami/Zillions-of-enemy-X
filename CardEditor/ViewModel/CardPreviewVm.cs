using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using CardEditor.Model;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class CardPreviewVm : BaseModel
    {
        private CardPreviewModel _selectedItem;

        public CardPreviewVm()
        {
            CardPreviewModels = new ObservableCollection<CardPreviewModel>();
            PreviewOrderValues = Dic.PreviewOrderDic.Values.ToList();
        }

        public bool IsPreviewChanged { get; set; }
        public List<string> PreviewOrderValues { get; set; }
        public string CardPreviewCountValue { get; set; }
        public string CardPreviewOrder { get; set; }
        public CardQueryMdoel MemoryQueryModel { get; set; }
        public ObservableCollection<CardPreviewModel> CardPreviewModels { get; set; }

        public CardPreviewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public void UpdateCardPreviewList(CardQueryMdoel cardQueryMdoel)
        {
            var dataSet = new DataSet();
            var sql = GetModelSql(cardQueryMdoel);
            // 保存上次查询的实例
            MemoryQueryModel = cardQueryMdoel;
            SqliteUtils.FillDataToDataSet(sql, dataSet);

            var previewModels = CardUtils.GetCardPreviewModels(dataSet);
            CardPreviewModels.Clear();
            RestrictUtils.GetRestrictCardList(previewModels, cardQueryMdoel.Restrict).ForEach(CardPreviewModels.Add);
            // 更新统计
            CardPreviewCountValue = StringConst.QueryResult + CardPreviewModels.Count;
            OnPropertyChanged(nameof(CardPreviewCountValue));
            // 跟踪历史
            if (MemoryQueryModel.CardEditorModel.Number.Equals(string.Empty)) return;
            var firstOrDefault = CardPreviewModels
                .Select((previewModel, index) => new { previewModel.Number, Index = index })
                .FirstOrDefault(i => i.Number.Equals(MemoryQueryModel.CardEditorModel.Number));
            if (null == firstOrDefault) return;
            var position = firstOrDefault.Index;
            if (position == -1) return;
            _selectedItem = CardPreviewModels[position];
        }

        /// <summary>
        ///     卡牌预览排序事件
        /// </summary>
        public void Order()
        {
            if (null == MemoryQueryModel) return;
            UpdateCardPreviewList(MemoryQueryModel);
        }

        private string GetModelSql(CardQueryMdoel cardQueryMdoel)
        {
            var modeType = CardUtils.GetModeType(cardQueryMdoel.ModeValue);
            var sql = string.Empty;
            switch (modeType)
            {
                case Enums.ModeType.Query:
                    sql = GetQuerySql(cardQueryMdoel.CardEditorModel);
                    break;
                case Enums.ModeType.Editor:
                    if (!cardQueryMdoel.CardEditorModel.Pack.Equals(string.Empty))
                        sql = GetEditorSql(cardQueryMdoel.CardEditorModel);
                    break;
                case Enums.ModeType.Develop:
                    break;
            }
            return sql;
        }

        public string GetEditorSql(CardEditorModel card)
        {
            var preOrderType = CardUtils.GetPreOrderType(CardPreviewOrder);
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());
            builder.Append(SqlUtils.GetPackSql(card.Pack, SqliteConst.ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }

        public string GetQuerySql(CardEditorModel card)
        {
            // 提取排序参数
            var preOrderType = CardUtils.GetPreOrderType(CardPreviewOrder);
            OnPropertyChanged(nameof(CardPreviewOrder));
            var builder = new StringBuilder();
            builder.Append(SqlUtils.GetHeaderSql());
            builder.Append(SqlUtils.GetAccurateSql(card.Type, SqliteConst.ColumnType)); // 种类
            builder.Append(SqlUtils.GetAccurateSql(card.Camp, SqliteConst.ColumnCamp)); // 阵营
            builder.Append(SqlUtils.GetAccurateSql(card.Race, SqliteConst.ColumnRace)); // 种族
            builder.Append(SqlUtils.GetAccurateSql(card.Sign, SqliteConst.ColumnSign)); // 标记
            builder.Append(SqlUtils.GetAccurateSql(card.Rare, SqliteConst.ColumnRare)); // 罕贵
            builder.Append(SqlUtils.GetSimilarSql(card.CName, SqliteConst.ColumnCName)); // 卡名
            builder.Append(SqlUtils.GetSimilarSql(card.JName, SqliteConst.ColumnJName)); // 日名
            builder.Append(SqlUtils.GetSimilarSql(card.Illust, SqliteConst.ColumnIllust)); // 画师
            builder.Append(SqlUtils.GetPackSql(card.Pack, SqliteConst.ColumnPack)); // 卡包
            builder.Append(SqlUtils.GetSimilarSql(card.Number, SqliteConst.ColumnNumber)); // 卡编
            builder.Append(SqlUtils.GetIntervalSql(card.CostValue, SqliteConst.ColumnCost)); // 费用
            builder.Append(SqlUtils.GetIntervalSql(card.PowerValue, SqliteConst.ColumnPower)); // 力量
            builder.Append(SqlUtils.GetAbilityTypeSql(card.AbilityTypeModels.ToList())); //  能力类型
            builder.Append(SqlUtils.GetAbilityDetailSql(card.AbilityDetailModels.ToList())); // 详细能力
            builder.Append(SqlUtils.GetSimilarSql(card.Ability, SqliteConst.ColumnAbility)); // 详细能力
            builder.Append(SqlUtils.GetFooterSql(preOrderType)); // 完整的查询语句
            return builder.ToString();
        }
    }
}