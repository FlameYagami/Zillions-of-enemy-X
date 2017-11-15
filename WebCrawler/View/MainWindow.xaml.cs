using System.IO;
using System.Windows;
using System.Windows.Input;
using Dialog;
using WebCrawler.ViewModel;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Utils;

namespace WebCrawler.View
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache))
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