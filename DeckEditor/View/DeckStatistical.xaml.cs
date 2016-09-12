using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visifire.Charts;

namespace DeckEditor.View
{
    /// <summary>
    /// DekcStatistical.xaml 的交互逻辑
    /// </summary>
    public partial class DekcStatistical
    {
        public DekcStatistical(Dictionary<int,int> statisticsDic)
        {
            InitializeComponent();
            CreateChartColumn(statisticsDic);
        }

        public void CreateChartColumn(Dictionary<int, int> statisticsDic)
        {
            //创建一个图标
            var chart = new Chart
            {
                //是否启用打印和保持图片
                ToolBarEnabled = false,
                //是否启用或禁用滚动
                ScrollingEnabled = false,
                //3D效果显示
                View3D = true
            };
            var yAxis = new Axis
            {
                AxisMinimum = 0,
                Suffix = "枚"
            };
            //设置图标中Y轴的最小值永远为0           
            //设置图表中Y轴的后缀          
            chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            var dataSeries = new DataSeries {RenderAs = RenderAs.StackedColumn};

            // 设置数据线的格式
            //柱状Stacked


            // 设置数据点              
            foreach (var item in statisticsDic)
            {
                // 创建一个数据点的实例。                   
                var dataPoint = new DataPoint
                {
                    // 设置X轴点       
                    AxisXLabel = item.Key.ToString(),
                    //设置Y轴点      
                    YValue = int.Parse(item.Value.ToString())
                };       
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }
            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);
            Grid.Children.Add(chart);
        }
    }
}
