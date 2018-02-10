using CardCrawler.View;
using Common;
using Wrapper.Model;

namespace CardCrawler.ViewModel
{
    public class MainVm : BaseModel
    {
        private static MainWindow _mainWindow;
        private DataPreivewView _dataPreivewView;

        private ImagePreviewView _imagePreviewView;

        public MainVm(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            CmdDataPreview = new DelegateCommand {ExecuteCommand = DataPreview_Click};
            CmdImagePreview = new DelegateCommand {ExecuteCommand = ImagePreview_Click};
            DataPreview_Click(null);
        }

        public DelegateCommand CmdDataPreview { get; set; }
        public DelegateCommand CmdImagePreview { get; set; }

        public DataPreivewView DataPreivewView
        {
            get { return _dataPreivewView ?? (_dataPreivewView = new DataPreivewView()); }
            set { _dataPreivewView = value; }
        }

        public ImagePreviewView ImagePreviewView
        {
            get { return _imagePreviewView ?? (_imagePreviewView = new ImagePreviewView()); }
            set { _imagePreviewView = value; }
        }

        public void ImagePreview_Click(object obj)
        {
            _mainWindow.ChildView.Children.Clear();
            _mainWindow.ChildView.Children.Add(ImagePreviewView);
        }

        public void DataPreview_Click(object obj)
        {
            _mainWindow.ChildView.Children.Clear();
            _mainWindow.ChildView.Children.Add(DataPreivewView);
        }
    }
}