using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeckEditor.Constant;
using DeckEditor.Entity;
using DeckEditor.Model;
using DeckEditor.Presenter;
using DeckEditor.Utils;

namespace DeckEditor.View
{
    public interface IView
    {
        void Init();
        void Reset();
        void UpdatePreviewListView(List<PreviewEntity> cardList);
        void UpdateDeckListView(StringConst.AreaType areaType, List<DeckEntity> deckColl);
        void UpdateStartAndLifeAndVoid(List<int> countStartAandLifeAndVoid);
        void SetPicture(List<string> numberList, List<string> picturePathList);
        void SetCardModel(CardEntity cardmodel);
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

        /************************************************** 接口实现 **************************************************/

        public CardEntity GetCardModel()
        {
            return new CardEntity
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
                AbilityType = SqlUtils.GetAbilityTypeSql(LstAbilityType.Items.Cast<CheckBox>()),
                AbilityDetail = SqlUtils.GetAbilityDetailSql(DataCache.AbilityDetialEntity)
            };
        }

        public void Init()
        {
            CmbPack.Text = StringConst.NotApplicable;
            CmbPack.ItemsSource = CardUtils.GetAllPack();
            CmbIllust.Text = StringConst.NotApplicable;
            CmbIllust.ItemsSource = CardUtils.GetIllust();
            CmbRace.IsEnabled = false;
            var uri = new Uri(Const.BackgroundPath, UriKind.Relative);
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
            DataCache.AbilityDetialEntity.ResetAbilityDetailDic();
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

        public void SetCardModel(CardEntity cardmodel)
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
            LblIFaq.Text = cardmodel.Faq;
            var signUri = CardUtils.GetSignPath(cardmodel.Sign);
            var campUriList = CardUtils.GetCampPathList(cardmodel.Camp);
            try
            {
                ImgISign.Source = signUri.Equals(string.Empty) ? new BitmapImage() : new BitmapImage(new Uri(signUri));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            try
            {
                ImgICamp0.Source = campUriList[0].Equals(string.Empty)
                    ? new BitmapImage()
                    : new BitmapImage(new Uri(campUriList[0]));
                ImgICamp1.Source = campUriList[1].Equals(string.Empty)
                    ? new BitmapImage()
                    : new BitmapImage(new Uri(campUriList[1]));
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
                    imageList[i].Source = new BitmapImage(new Uri(picturePathList[i]));
                }
                else
                {
                    tabItemList[i].Visibility = Visibility.Hidden;
                }
            TbiPicture1.IsSelected = true;
            if (0 != picturePathList.Count) return;
            tabItemList[0].Visibility = Visibility.Hidden;
        }

        public void UpdatePreviewListView(List<PreviewEntity> cardList)
        {
            LvwPreview.ItemsSource = null;
            LvwPreview.ItemsSource = cardList;
            LblCardCount.Content = StringConst.QueryResult + cardList.Count;
        }

        public void UpdateDeckListView(StringConst.AreaType areaType, List<DeckEntity> deckColl)
        {
            switch (areaType)
            {
                case StringConst.AreaType.Pl:
                    PlayerListView.ItemsSource = null;
                    PlayerListView.ItemsSource = deckColl;
                    break;
                case StringConst.AreaType.Ig:
                    IgListView.ItemsSource = null;
                    IgListView.ItemsSource = deckColl;
                    break;
                case StringConst.AreaType.Ug:
                    UgListView.ItemsSource = null;
                    UgListView.ItemsSource = deckColl;
                    break;
                case StringConst.AreaType.Ex:
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _presenter.Init();
        }

        /************************************************** 查询操作 **************************************************/

        /// <summary>阵营选择事件</summary>
        private void Camp_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.CampChanged(CmbCamp.Text.Trim());
        }

        /// <summary>查询区域重置事件</summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ResetClick();
        }

        /// <summary>查询区域查询事件</summary>
        private void Query_Click(object sender, RoutedEventArgs e)
        {
            _presenter.QueryClick(GetCardModel(),CmbOrder.Text.Trim());
        }

        /// <summary>列表区域排序事件</summary>
        private void CmbOrder_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.OrderClick(CmbOrder.Text.Trim());
        }

        /// <summary>查询区域能力分类事件</summary>
        private void BtnAbilityDetail_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ShowAbilityDetail();
        }

        /************************************************** 组卡操作 **************************************************/

        /// <summary>列表区域切换事件</summary>
        private void PreviewListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.PreivewListViewChanged(LvwPreview.SelectedIndex);
        }

        /// <summary>列表区域右键事件</summary>
        private void PreviewGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _presenter.PreviewMouseRightClick(sender as Grid, TcImage.SelectedIndex);
        }

        /// <summary>组卡区域左键事件</summary>
        private void AreaGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                _presenter.ImgAreaMouseDoubleClick(sender as Grid);
            else
                _presenter.ImgAreaMouseLeftClick(sender as Grid);
        }

        /// <summary>组卡区域右键事件</summary>
        private void AreaGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _presenter.ImgAreaMouseRightClick(sender as Grid);
        }

        /// <summary>大图区鼠标右击事件</summary>
        private void Picture_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _presenter.PictureMouseRightClick(sender as Image);
        }

        /************************************************** 卡组操作 **************************************************/

        /// <summary>卡组保存事件</summary>
        private void Sava_Click(object sender, RoutedEventArgs e)
        {
            _presenter.SaveClick(CmbDeck.Text.Trim());
        }

        /// <summary>另存卡组事件</summary>
        private void Resave_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ResaveClick(CmbDeck.Text.Trim());
        }

        /// <summary>卡组清空事件</summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ClearClick();
        }

        /// <summary>卡组删除事件</summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeleteClick(CmbDeck.Text.Trim());
        }

        /// <summary>卡组加载事件</summary>
        private void CmbDeck_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.DeckNameChanged(CmbDeck.Text.Trim());
        }

        /// <summary>卡组浏览事件</summary>
        private void CmbDeck_DropDownOpened(object sender, EventArgs e)
        {
            _presenter.ShowAllDeckName();
        }

        /// <summary>数值排序事件</summary>
        private void BtnValueOrder_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ValueOrder();
        }

        /// <summary>随机排序事件</summary>
        private void BtnRandomOrder_Click(object sender, RoutedEventArgs e)
        {
            _presenter.RandomOrder();
        }

        /// <summary>卡组统计事件</summary>
        private void BtnDeckStatistical_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeckStatisticalClick();
        }

        /// <summary>退出</summary>
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