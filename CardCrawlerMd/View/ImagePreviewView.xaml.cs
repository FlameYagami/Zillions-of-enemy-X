using System;
using System.Windows.Controls;
using System.Windows.Input;
using CardCrawler.ViewModel;

namespace CardCrawler.View
{
    /// <summary>
    ///     ImagePreviewView.xaml 的交互逻辑
    /// </summary>
    public partial class ImagePreviewView : UserControl
    {
        public ImagePreviewView()
        {
            InitializeComponent();
            DataContext = new ImagePreviewVm();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}