using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DeckEditor.View;
using Visifire.Charts;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class DeckDetailVm
    {
        private readonly DeckDetailDialog _deckDetailDialog;
        private readonly DeckManager _deckManager;

        public DeckDetailVm(DeckDetailDialog deckDetailDialog, DeckManager deckManager)
        {
            _deckDetailDialog = deckDetailDialog;
            _deckManager = deckManager;
            IgModels = new ObservableCollection<DeckModel>();
            UgModels = new ObservableCollection<DeckModel>();
            ExModels = new ObservableCollection<DeckModel>();
            _deckManager.IgModels.ForEach(IgModels.Add);
            _deckManager.UgModels.ForEach(UgModels.Add);
            _deckManager.ExModels.ForEach(ExModels.Add);
            UpdateChart(GetDeckStatisDic());
        }

        public ObservableCollection<DeckModel> IgModels { get; set; }
        public ObservableCollection<DeckModel> UgModels { get; set; }
        public ObservableCollection<DeckModel> ExModels { get; set; }

        private Dictionary<int, int> GetDeckStatisDic()
        {
            var dekcStatisticalDic = new Dictionary<int, int>();
            var costIgList = _deckManager.IgModels.Select(deckModel => deckModel.Cost);
            var costUgList = _deckManager.UgModels.Select(deckModel => deckModel.Cost);
            var costDeckList = new List<int>();
            costDeckList.AddRange(costIgList);
            costDeckList.AddRange(costUgList);
            if (0 == costDeckList.Count) return new Dictionary<int, int>();
            var costMax = costDeckList.Max();
            for (var i = 0; i != costMax + 1; i++)
                dekcStatisticalDic.Add(i + 1, costDeckList.Count(cost => cost.Equals(i + 1)));
            return dekcStatisticalDic;
        }

        public void UpdateChart(Dictionary<int, int> statisticsDic)
        {
            if (statisticsDic.Count == 0) return;
            var yAxis = new Axis
            {
                //设置图表中Y轴的后缀       
                Suffix = "枚"
            };
            _deckDetailDialog.ChartDeck.AxesY.Add(yAxis);

            var xAxis = new Axis
            {
                //设置图表中X轴的后缀       
                Suffix = "费"
            };
            _deckDetailDialog.ChartDeck.AxesX.Add(xAxis);

            // 创建一个新的数据线。               
            var dataSeries = new DataSeries
            {
                RenderAs = RenderAs.StackedColumn,
                LabelEnabled = true,
                LabelText = "#YValue"
            };
            // 设置数据点              
            foreach (var item in statisticsDic)
            {
                // 创建一个数据点的实例。                   
                var dataPoint = new DataPoint
                {
                    // 设置X轴点
                    XValue = int.Parse(item.Key.ToString()),
                    //设置Y轴点      
                    YValue = int.Parse(item.Value.ToString())
                };
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }
            // 添加数据线到数据序列。                
            _deckDetailDialog.ChartDeck.Series.Add(dataSeries);
        }
    }
}