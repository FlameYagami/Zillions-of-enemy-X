using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Common;
using DeckEditor.Model;
using Wrapper;
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

        private DeSearchModel MemorySearchModel { get; set; }

        public void UpdateCardPreviewModels(DeSearchModel searchModel)
        {
            MemorySearchModel = searchModel; // 保存查询的实例
            var dataSet = new DataSet();
            var sql = DeSqlUtils.GetQuerySql(searchModel, _cardPreviewOrder);
            DataManager.FillDataToDataSet(dataSet,sql);
            var tempList = CardUtils.GetCardPreviewModels(dataSet);
            CardPreviewModels.Clear();
            tempList.ForEach(CardPreviewModels.Add);
            CardPreviewCountValue = CardPreviewModels.Count.ToString();
        }

        /// <summary>
        ///     卡牌预览排序事件
        /// </summary>
        public void Order()
        {
            if (null == MemorySearchModel) return;
            UpdateCardPreviewModels(MemorySearchModel);
        }
    }
}