using System.Collections.Generic;
using Visifire.Charts;

namespace DeckEditor.View
{
    /// <summary>
    ///     DekcStatistical.xaml 的交互逻辑
    /// </summary>
    public partial class DekcStatistical
    {
        public DekcStatistical(Dictionary<int, int> statisticsDic)
        {
            InitializeComponent();
            CreateChartColumn(statisticsDic);
        }

        public void CreateChartColumn(Dictionary<int, int> statisticsDic)
        {
            var yAxis = new Axis
            {
                //设置图表中Y轴的后缀       
                Suffix = "枚"
            };
            chart.AxesY.Add(yAxis);

            var xAxis = new Axis
            {
                //设置图表中X轴的后缀       
                Suffix = "费"
            };
            chart.AxesX.Add(xAxis);

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
            chart.Series.Add(dataSeries);
        }
    }
}