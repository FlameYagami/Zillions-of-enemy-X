using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeckEditor.ViewModel;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace DeckEditor.View
{
    public partial class MainWindow
    {
        private CardDetailVm _cardDetailVm;
        private CardPictureVm _cardPictureVm;
        private CardPreviewVm _cardPreviewVm;
        private CardQueryVm _cardQueryVm;
        private DeckOperationVm _deckOperationVm;
        private DeckOrderVm _deckOrderVm;
        private DeckStatsVm _deckStatsVm;
        private DeckVm _deckVm;
        private PlayerVm _playerVm;

        public MainWindow()
        {
            InitializeComponent();

            if (SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache))
            {
                var uri = new Uri(PathManager.BackgroundPath, UriKind.Relative);
                var imageBrush = new ImageBrush {ImageSource = new BitmapImage(uri)};
                BorderView.Background = imageBrush;
                if (!Directory.Exists(PathManager.DeckFolderPath))
                    Directory.CreateDirectory(PathManager.DeckFolderPath);
            }
            else
                BaseDialogUtils.ShowDlgOk(StringConst.DbOpenError);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _deckVm = new DeckVm();
            _playerVm = new PlayerVm();
            _deckStatsVm = new DeckStatsVm();
            _cardPreviewVm = new CardPreviewVm();
            _cardPictureVm = new CardPictureVm();
            _cardQueryVm = new CardQueryVm(_cardPreviewVm);
            _cardDetailVm = new CardDetailVm(_cardPictureVm);
            _deckOrderVm = new DeckOrderVm(_deckVm);
            _deckOperationVm = new DeckOperationVm(_deckVm, _playerVm, _deckStatsVm);


            DeckView.DataContext = _deckVm;
            PlayerView.DataContext = _playerVm;
            DeckStatsView.DataContext = _deckStatsVm;
            CardPreviewView.DataContext = _cardPreviewVm;
            CardPictureView.DataContext = _cardPictureVm;
            CardQueryView.DataContext = _cardQueryVm;
            CardDetailView.DataContext = _cardDetailVm;
            DeckOrderView.DataContext = _deckOrderVm;
            DeckOperationView.DataContext = _deckOperationVm;
        }

        /************************************************** 查询操作 **************************************************/

        /// <summary>阵营选择事件</summary>
        private void Camp_DropDownClosed(object sender, EventArgs e)
        {
            _cardQueryVm.UpdateRaceList();
        }

        /// <summary>列表区域排序事件</summary>
        private void CmbOrder_DropDownClosed(object sender, EventArgs e)
        {
            _cardPreviewVm.Order();
        }

        /************************************************** 组卡操作 **************************************************/

        /// <summary>列表区域切换事件</summary>
        private void CardPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previewModel = LvCardPreview.SelectedItem as CardPreviewModel;
            if (null == previewModel) return;
            _cardDetailVm.UpdateCardModel(previewModel.Number);
        }

        /// <summary>列表区域右键事件</summary>
        private void CardPreviewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            var numberEx = CardUtils.GetNumberExList(grid.Tag.ToString())[CardPictureView.SelectedIndex];
            _deckOperationVm.AddCard(numberEx);
        }

        /// <summary>组卡区域左键事件</summary>
        private void DeckItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            if (e.ClickCount == 2)
                _deckOperationVm.AddCard(grid.Tag.ToString());
            else
                _cardDetailVm.UpdateCardModel(grid.Tag.ToString());
        }

        /// <summary>组卡区域右键事件</summary>
        private void DeckItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            _deckOperationVm.DeleteCard(grid.Tag.ToString());
        }

        /************************************************** 卡组操作 **************************************************/

        /// <summary>卡组加载事件</summary>
        private void CmbDeck_DropDownClosed(object sender, EventArgs e)
        {
            _deckOperationVm.LoadDeck();
        }

        /// <summary>卡组浏览事件</summary>
        private void CmbDeck_DropDownOpened(object sender, EventArgs e)
        {
            _deckOperationVm.UpdateDeckNameList();
        }

        /************************************************** 其他操作 **************************************************/

        /// <summary>退出</summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
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