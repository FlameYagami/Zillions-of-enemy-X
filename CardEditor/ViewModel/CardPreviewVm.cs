using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Wrapper;
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
            PreviewOrderDic = Dic.PreviewOrderDic;
        }

        public bool IsPreviewChanged { get; set; }
        public Dictionary<Enums.PreviewOrderType, string> PreviewOrderDic { get; set; }
        public string CardPreviewCountValue { get; set; }
        public Enums.PreviewOrderType PreviewOrderType { get; set; }
        public CeQueryExModel MemoryQueryModel { get; set; }
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

        public void UpdateCardPreviewList(CeQueryExModel cardQueryMdoel)
        {
            var dataSet = new DataSet();
            var sql = GetModelSql(cardQueryMdoel);
            // 保存上次查询的实例
            MemoryQueryModel = cardQueryMdoel;
            DataManager.FillDataToDataSet(dataSet, sql);

            var previewModels = CardUtils.GetCardPreviewModels(dataSet);
            CardPreviewModels.Clear();
            RestrictUtils.GetRestrictCardList(previewModels, cardQueryMdoel.Restrict).ForEach(CardPreviewModels.Add);
            // 更新统计
            CardPreviewCountValue = "查询结果:" + CardPreviewModels.Count;
            OnPropertyChanged(nameof(CardPreviewCountValue));
            // 跟踪历史
            if (MemoryQueryModel.CeQueryModel.Number.Equals(string.Empty)) return;
            var firstOrDefault = CardPreviewModels
                .Select((previewModel, index) => new {previewModel.Number, Index = index})
                .FirstOrDefault(i => i.Number.Equals(MemoryQueryModel.CeQueryModel.Number));
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

        private string GetModelSql(CeQueryExModel cardQueryMdoel)
        {
            OnPropertyChanged(nameof(PreviewOrderType));
            var sql = string.Empty;
            switch (cardQueryMdoel.ModeType)
            {
                case Enums.ModeType.Query:
                    sql = CeSqlUtils.GetQuerySql(cardQueryMdoel.CeQueryModel, PreviewOrderType);
                    break;
                case Enums.ModeType.Editor:
                    if (!cardQueryMdoel.CeQueryModel.Pack.Equals(string.Empty))
                        sql = CeSqlUtils.GetEditorSql(cardQueryMdoel.CeQueryModel, PreviewOrderType);
                    break;
                case Enums.ModeType.Develop:
                    sql = CeSqlUtils.GetQuerySql(cardQueryMdoel.CeQueryModel, PreviewOrderType);
                    break;
            }
            return sql;
        }
    }
}