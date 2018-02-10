using CardCrawler.ViewModel;
using Dialog;
using Wrapper;
using Wrapper.Constant;

namespace CardCrawler.View
{
    /// <summary>
    ///     DataPreivewView.xaml 的交互逻辑
    /// </summary>
    public partial class DataPreivewView
    {
        public DataPreivewView()
        {
            InitializeComponent();
            if (DataManager.FillDataToDataSet())
                DataContext = new DataPreviewVm();
            else
                BaseDialogUtils.ShowDialogOk(StringConst.DbOpenError);
        }
    }
}