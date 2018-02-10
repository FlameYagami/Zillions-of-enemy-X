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
        public CeQueryExModel CeQueryExModel { get; set; }
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

        public void UpdateCardPreviewList(CeQueryExModel ceQueryExModel)
        {
            var dataSet = new DataSet();
            var sql = GetModelSql(ceQueryExModel);
            // 保存上次查询的实例
            CeQueryExModel = ceQueryExModel;
            DataManager.FillDataToDataSet(dataSet, sql);
            var tempModels = CardUtils.GetCardPreviewModels(dataSet);
            CardPreviewModels.Clear();
            RestrictUtils.GetRestrictCardList(tempModels, ceQueryExModel?.Restrict ?? -1).ForEach(CardPreviewModels.Add);
            // 更新统计
            CardPreviewCountValue = CardPreviewModels.Count.ToString();
            OnPropertyChanged(nameof(CardPreviewCountValue));
            // 跟踪历史
            if (CeQueryExModel.CeQueryModel.Number.Equals(string.Empty)) return;
            var firstOrDefault = CardPreviewModels
                .Select((previewModel, index) => new {previewModel.Number, Index = index})
                .FirstOrDefault(i => i.Number.Equals(CeQueryExModel.CeQueryModel.Number));
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
            if (null == CeQueryExModel) return;
            UpdateCardPreviewList(CeQueryExModel);
        }

        private string GetModelSql(CeQueryExModel ceQueryExModel)
        {
            OnPropertyChanged(nameof(PreviewOrderType));
            var sql = string.Empty;
            switch (ceQueryExModel.ModeType)
            {
                case Enums.ModeType.Query:
                    sql = CeSqlUtils.GetQuerySql(ceQueryExModel.CeQueryModel, PreviewOrderType);
                    break;
                case Enums.ModeType.Editor:
                    if (!ceQueryExModel.CeQueryModel.Pack.Equals(string.Empty))
                        sql = CeSqlUtils.GetEditorSql(ceQueryExModel.CeQueryModel, PreviewOrderType);
                    break;
                case Enums.ModeType.Develop:
                    sql = CeSqlUtils.GetQuerySql(ceQueryExModel.CeQueryModel, PreviewOrderType);
                    break;
            }
            return sql;
        }
    }
}