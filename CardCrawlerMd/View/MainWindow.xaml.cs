using System.Windows;
using System.Windows.Input;
using CardCrawler.ViewModel;
using Dialog;
using Wrapper;
using Wrapper.Constant;

namespace WebCrawler.View
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DataManager.FillDataToDataSet())
                DataContext = new MainVm();
            else
                BaseDialogUtils.ShowDialogOk(StringConst.DbOpenError);
        }

        private void Title_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}