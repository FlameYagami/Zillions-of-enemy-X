using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeckEditor.Presenter;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using Enum = Wrapper.Constant.Enum;

namespace DeckEditor.View
{
    public interface IView
    {
        void Init();
        void Reset();
        void UpdatePreviewListView(List<CardPreviewModel> cardList);
        void UpdateDeckListView(Enum.AreaType areaType, List<DeckModel> deckColl);
        void UpdateStartAndLifeAndVoid(List<int> countStartAandLifeAndVoid);
        void SetPicture(List<string> numberList, List<string> picturePathList);
        void SetCardModel(CardModel cardmodel);
        void SetRaceItems(List<object> itemList);
        void SetDeckName(string name);
        void SetDeckName(List<string> deckNameList);
    }

    public partial class MainWindow : IView
    {
        private readonly IPresenter _presenter;

        public MainWindow()
        {
            InitializeComponent();
            _presenter = new Presenter.Presenter(this);
        }

        public void Init()
        {
            CmbPack.Text = StringConst.NotApplicable;
            CmbPack.ItemsSource = CardUtils.GetAllPack();
            CmbIllust.Text = StringConst.NotApplicable;
            CmbIllust.ItemsSource = CardUtils.GetIllust();
            CmbRace.IsEnabled = false;
            var uri = new Uri(PathManager.BackgroundPath, UriKind.Relative);
            var imageBrush = new ImageBrush {ImageSource = new BitmapImage(uri)};
            BorderView.Background = imageBrush;
        }

        public void Reset()
        {
            CmbRace.ItemsSource = null;
            CmbRace.IsEnabled = false;
            foreach (var control in GridQuery.Children)
            {
                var comboBox = control as ComboBox;
                if (comboBox != null)
                    comboBox.Text = StringConst.NotApplicable;
                var textBox = control as TextBox;
                if (textBox != null)
                    textBox.Text = string.Empty;
            }
            foreach (var checkbox in LstAbilityType.Items.Cast<CheckBox>())
                checkbox.IsChecked = false;
            DataCache.AbilityDetialModel.ResetAbilityDetailDic();
        }

        public void SetRaceItems(List<object> itemList)
        {
            if (null == itemList)
            {
                CmbRace.IsEnabled = false;
                return;
            }
            CmbRace.Items.Clear();
            itemList.ForEach(value => CmbRace.Items.Add(value.ToString()));
            CmbRace.Text = StringConst.NotApplicable;
            CmbRace.IsEnabled = true;
        }

        public void SetCardModel(CardModel cardmodel)
        {
            LblICName.Content = cardmodel.CName;
            LblINumber.Content = cardmodel.Number;
            LblIType.Content = cardmodel.Type;
            LblIRare.Content = cardmodel.Rare;
            LblIPower.Content = cardmodel.Power;
            LblICost.Content = cardmodel.Cost;
            LblIRace.Content = cardmodel.Race;
            LblIAbility.Text = cardmodel.Ability;
            LblIJName.Content = cardmodel.JName;
            LblIPack.Content = cardmodel.Pack;
            LblIIllust.Content = cardmodel.Illust;
            LblILines.Text = cardmodel.Lines;
            var signUri = CardUtils.GetSignPath(cardmodel.Sign);
            var campUriList = CardUtils.GetCampPathList(cardmodel.Camp);
            var imageCampList = new List<Image> {ImgICamp0, ImgICamp1, ImgICamp2, ImgICamp3, ImgICamp4};
            try
            {
                ImgISign.Source = signUri.Equals(string.Empty) ? new BitmapImage() : new BitmapImage(new Uri(signUri));
                for (var i = 0; i != imageCampList.Count; i++)
                    imageCampList[i].Source = campUriList[i].Equals(string.Empty)
                        ? new BitmapImage()
                        : new BitmapImage(new Uri(campUriList[i]));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void SetDeckName(string name)
        {
            CmbDeck.Text = name;
        }

        public void SetDeckName(List<string> deckNameList)
        {
            CmbDeck.ItemsSource = null;
            CmbDeck.ItemsSource = deckNameList;
        }

        public void SetPicture(List<string> numberList, List<string> picturePathList)
        {
            var tabItemList = new List<TabItem> {TbiPicture1, TbiPicture2, TbiPicture3, TbiPicture4};
            var imageList = new List<Image> {ImgPicture1, ImgPicture2, ImgPicture3, ImgPicture4};
            for (var i = 0; i != tabItemList.Count; i++)
                if (i < picturePathList.Count)
                {
                    tabItemList[i].Visibility = Visibility.Visible;
                    imageList[i].Tag = numberList[i];
                    imageList[i].Source = File.Exists(picturePathList[i])
                        ? new BitmapImage(new Uri(picturePathList[i]))
                        : new BitmapImage();
                }
                else
                {
                    tabItemList[i].Visibility = Visibility.Hidden;
                }
            TbiPicture1.IsSelected = true;
            if (0 != picturePathList.Count) return;
            tabItemList[0].Visibility = Visibility.Hidden;
        }

        public void UpdatePreviewListView(List<CardPreviewModel> cardList)
        {
            LvwPreview.ItemsSource = null;
            LvwPreview.ItemsSource = cardList;
            LblCardCount.Content = StringConst.QueryResult + cardList.Count;
        }

        public void UpdateDeckListView(Enum.AreaType areaType, List<DeckModel> deckColl)
        {
            switch (areaType)
            {
                case Enum.AreaType.Pl:
                    PlayerListView.ItemsSource = null;
                    PlayerListView.ItemsSource = deckColl;
                    break;
                case Enum.AreaType.Ig:
                    IgListView.ItemsSource = null;
                    IgListView.ItemsSource = deckColl;
                    break;
                case Enum.AreaType.Ug:
                    UgListView.ItemsSource = null;
                    UgListView.ItemsSource = deckColl;
                    break;
                case Enum.AreaType.Ex:
                    ExListView.ItemsSource = null;
                    ExListView.ItemsSource = deckColl;
                    break;
            }
        }

        public void UpdateStartAndLifeAndVoid(List<int> countStartAandLifeAndVoid)
        {
            var startCount = countStartAandLifeAndVoid[0];
            var lifeCount = countStartAandLifeAndVoid[1];
            var voidCount = countStartAandLifeAndVoid[2];
            LblStartCount.Content = startCount;
            LblLifeCount.Content = lifeCount;
            LblVoidCount.Content = voidCount;
            LblStartCount.Foreground = startCount == 0
                ? new SolidColorBrush(Colors.Red)
                : startCount == 1
                    ? new SolidColorBrush(Colors.Lime)
                    : new SolidColorBrush(Colors.Yellow);
            LblLifeCount.Foreground = (lifeCount == 0) || (lifeCount == 1)
                ? new SolidColorBrush(Colors.Red)
                : lifeCount == 2
                    ? new SolidColorBrush(Colors.Yellow)
                    : new SolidColorBrush(Colors.Lime);
            LblVoidCount.Foreground = (voidCount == 0) || (voidCount == 1)
                ? new SolidColorBrush(Colors.Red)
                : voidCount == 2
                    ? new SolidColorBrush(Colors.Yellow)
                    : new SolidColorBrush(Colors.Lime);
        }

        /************************************************** �ӿ�ʵ�� **************************************************/

        public CardQueryModel GetCardQueryModel()
        {
            return new CardQueryModel()
            {
                Type = CmbType.Text.Trim(),
                Camp = CmbCamp.Text.Trim(),
                Race = CmbRace.Text.Trim(),
                Sign = CmbSign.Text.Trim(),
                Rare = CmbRare.Text.Trim(),
                Pack = CmbPack.Text.Trim(),
                Illust = CmbIllust.Text.Trim(),
                Key = TxtKey.Text.Trim(),
                Cost = TxtCost.Text.Trim(),
                Power = TxtPower.Text.Trim(),
                AbilityTypeDic = LstAbilityType.Items.Cast<CheckBox>().ToDictionary(
                    checkbox => checkbox.Content.ToString(),
                    checkbox => (checkbox.IsChecked != null) && (bool)checkbox.IsChecked),
                AbilityDetailDic = DataCache.AbilityDetialModel.GetAbilityDetailExDic(),
                Order = CmbOrder.Text.Trim()
               
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _presenter.Init();
        }

        /************************************************** ��ѯ���� **************************************************/

        /// <summary>��Ӫѡ���¼�</summary>
        private void Camp_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.CampChanged(CmbCamp.Text.Trim());
        }

        /// <summary>��ѯ���������¼�</summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ResetClick();
        }

        /// <summary>��ѯ�����ѯ�¼�</summary>
        private void Query_Click(object sender, RoutedEventArgs e)
        {
            _presenter.QueryClick(GetCardQueryModel());
        }

        /// <summary>�б����������¼�</summary>
        private void CmbOrder_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.OrderClick(CmbOrder.Text.Trim());
        }

        /// <summary>��ѯ�������������¼�</summary>
        private void BtnAbilityDetail_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ShowAbilityDetail();
        }

        /************************************************** �鿨���� **************************************************/

        /// <summary>�б������л��¼�</summary>
        private void CardPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.PreivewListViewChanged(LvwPreview.SelectedItem);
        }

        /// <summary>�б������Ҽ��¼�</summary>
        private void CardPreviewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            _presenter.CardPreviewItemMouseRightClick(grid.Tag.ToString(), TcImage.SelectedIndex);
        }

        /// <summary>�鿨��������¼�</summary>
        private void DeckItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            if (e.ClickCount == 2)
                _presenter.DeckItemMouseDoubleClick(grid.Tag.ToString());
            else
                _presenter.DeckItemMouseLeftClick(grid.Tag.ToString());
        }

        /// <summary>�鿨�����Ҽ��¼�</summary>
        private void DeckItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (null == grid) return;
            _presenter.DeckItemMouseRightClick(grid.Uid);
        }

        /// <summary>��ͼ������һ��¼�</summary>
        private void Picture_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            if (null == image) return;
            _presenter.PictureMouseRightClick(image.Tag.ToString());
        }

        /************************************************** ������� **************************************************/

        /// <summary>���鱣���¼�</summary>
        private void Sava_Click(object sender, RoutedEventArgs e)
        {
            _presenter.SaveClick(CmbDeck.Text.Trim());
        }

        /// <summary>��濨���¼�</summary>
        private void Resave_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ResaveClick(CmbDeck.Text.Trim());
        }

        /// <summary>��������¼�</summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeckClearClick();
        }

        /// <summary>����ɾ���¼�</summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeleteClick(CmbDeck.Text.Trim());
        }

        /// <summary>��������¼�</summary>
        private void CmbDeck_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.DeckNameChanged(CmbDeck.Text.Trim());
        }

        /// <summary>��������¼�</summary>
        private void CmbDeck_DropDownOpened(object sender, EventArgs e)
        {
            _presenter.ShowAllDeckName();
        }

        /// <summary>��ֵ�����¼�</summary>
        private void BtnValueOrder_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ValueOrder();
        }

        /// <summary>��������¼�</summary>
        private void BtnRandomOrder_Click(object sender, RoutedEventArgs e)
        {
            _presenter.RandomOrder();
        }

        /// <summary>����ͳ���¼�</summary>
        private void BtnDeckStatistical_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeckStatisticalClick();
        }

        /// <summary>�˳�</summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ExitClick();
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