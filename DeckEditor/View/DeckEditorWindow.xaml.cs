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

        /************************************************** ��ѯ���� **************************************************/

        /// <summary>��Ӫѡ���¼�</summary>
        private void Camp_DropDownClosed(object sender, EventArgs e)
        {
            _cardQueryVm.UpdateRaceList();
        }

        /// <summary>�б����������¼�</summary>
        private void CmbOrder_DropDownClosed(object sender, EventArgs e)
        {
            _cardPreviewVm.Order();
        }

        /************************************************** �鿨���� **************************************************/

        /// <summary>�б������л��¼�</summary>
        private void CardPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previewModel = LvCardPreview.SelectedItem as CardPreviewModel;
            if (null == previewModel) return;
            _cardDetailVm.UpdateCardModel(previewModel.Number);
        }

        /// <summary>�б������Ҽ��¼�</summary>
        private void CardPreviewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            var numberEx = CardUtils.GetNumberExList(grid.Tag.ToString())[CardPictureView.SelectedIndex];
            _deckOperationVm.AddCard(numberEx);
        }

        /// <summary>�鿨��������¼�</summary>
        private void DeckItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            if (e.ClickCount == 2)
                _deckOperationVm.AddCard(grid.Tag.ToString());
            else
                _cardDetailVm.UpdateCardModel(grid.Tag.ToString());
        }

        /// <summary>�鿨�����Ҽ��¼�</summary>
        private void DeckItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            _deckOperationVm.DeleteCard(grid.Tag.ToString());
        }

        /************************************************** ������� **************************************************/

        /// <summary>��������¼�</summary>
        private void CmbDeck_DropDownClosed(object sender, EventArgs e)
        {
            _deckOperationVm.LoadDeck();
        }

        /// <summary>��������¼�</summary>
        private void CmbDeck_DropDownOpened(object sender, EventArgs e)
        {
            _deckOperationVm.UpdateDeckNameList();
        }

        /************************************************** �������� **************************************************/

        /// <summary>�˳�</summary>
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