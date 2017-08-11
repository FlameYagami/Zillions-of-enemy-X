using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CardEditor.Model;
using CardEditor.Presenter;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using Enum = Wrapper.Constant.Enum;

namespace CardEditor.View
{
    public interface IView
    {
        void SetPackItems(List<object> itemList);
        void SetCardModel(CardModel card);
        void SetPasswordVisibility(bool isEncryptVisible, bool isDecryptVisible);
        void SetPicture(List<string> picturePathList);
        void Reset(Enum.ModeType modeType);
        void UpdatePreListView(List<CardPreviewModel> previewEntityList, string memoryNumber);
        void UpdatePackLinkage(string packNumber);
        void UpdateTypeLinkage(string type);
        void UpdateCampLinkage(List<object> itemList);
        void UpdateAbilityLinkage(Enum.AbilityType abilityType);
    }

    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : IView
    {
        private readonly IPresenter _presenter;

        public MainWindow()
        {
            InitializeComponent();
            _presenter = new Presenter.Presenter(this);
        }

        private bool IsPreviewChanged { get; set; }

        /************************************************** 接口实现 **************************************************/

        public void UpdateAbilityLinkage(Enum.AbilityType abilityType)
        {
            switch (abilityType)
            {
                case Enum.AbilityType.Ig:
                {
                    CmbType.Text = StringConst.TypeZx;
                    CmbSign.Text = StringConst.SignIg;
                    break;
                }
                case Enum.AbilityType.Start:
                {
                    CmbType.Text = StringConst.TypeZx;
                    CmbSign.Text = StringConst.Hyphen;
                    break;
                }
                case Enum.AbilityType.Event:
                {
                    CmbType.Text = StringConst.TypeEvent;
                    CmbRace.Text = StringConst.Hyphen;
                    TxtPower.Text = string.Empty;
                    break;
                }
                case Enum.AbilityType.Extra:
                {
                    CmbType.Text = StringConst.TypeZxEx;
                    CmbSign.Text = StringConst.Hyphen;
                    break;
                }
            }
        }

        /// <summary>
        /// 此模型用于增、删、改、查
        /// </summary>
        public CardEditorModel GetCardEditorModel()
        {
            return new CardEditorModel
            {
                Type = CmbType.Text.Trim(),
                Camp = CmbCamp.Text.Trim(),
                Race = CmbRace.Text.Trim(),
                Sign = CmbSign.Text.Trim(),
                Rare = CmbRare.Text.Trim(),
                Pack = CmbPack.Text.Trim(),
                CName = TxtCName.Text.Trim(),
                JName = TxtJName.Text.Trim(),
                Illust = TxtIllust.Text.Trim(),
                Number = TxtNumber.Text.Trim(),
                Ability = TxtAbility.Text.Trim(),
                Lines = TxtLines.Text.Trim(),
                Cost = TxtCost.Text.Trim().Equals(string.Empty) ? 0 : int.Parse(TxtCost.Text.Trim()),
                Power = TxtPower.Text.Trim().Equals(string.Empty) ? 0 : int.Parse(TxtPower.Text.Trim()),
                ImageJson = JsonUtils.JsonSerializer(new List<string> { $"/{TxtNumber.Text.Trim()}.jpg" }),

                AbilityTypeDic = LstAbilityType.Items.Cast<CheckBox>().ToDictionary(
                    checkbox => checkbox.Content.ToString(),
                    checkbox => (null != checkbox.IsChecked) && (bool)checkbox.IsChecked),
                AbilityDetailDic = LstAbilityDetail.Items.Cast<CheckBox>().ToDictionary(
                    checkbox => checkbox.Content.ToString(),
                    checkbox => (null != checkbox.IsChecked) && (bool)checkbox.IsChecked),

                Mode = CmbMode.Text.Trim(),
                Order = CmbOrder.Text.Trim(),
                Restrict = CmbRestrict.Text.Trim()
            };
        }

        public void SetCardModel(CardModel card)
        {
            IsPreviewChanged = true;
            CmbType.Text = card.Type;
            CmbCamp.Text = card.Camp;
            CmbRace.Text = card.Race;
            CmbSign.Text = card.Sign;
            CmbRare.Text = card.Rare;
            CmbPack.Text = card.Pack;
            CmbRestrict.Text = card.Restrict.Equals("4") ? StringConst.NotApplicable : card.Restrict;

            TxtCName.Text = card.CName;
            TxtJName.Text = card.JName;
            TxtIllust.Text = card.Illust;
            TxtNumber.Text = card.Number;
            TxtCost.Text = 0 == card.Cost  ? string.Empty: card.Cost.ToString();
            TxtPower.Text = 0 == card.Power ? string.Empty : card.Power.ToString();

            TxtAbility.Text = card.Ability;
            TxtLines.Text = card.Lines;
            TxtMd5.Text = card.Md5;

            foreach (var checkbox in LstAbilityType.Items.Cast<CheckBox>())
                foreach (var abilityType in Dictionary.AbilityTypeDic)
                {
                    if (!checkbox.Content.Equals(abilityType.Key)) continue;
                    checkbox.IsChecked = card.Ability.Contains(abilityType.Value);
                    break;
                }

            foreach (var checkbox in LstAbilityDetail.Items.Cast<CheckBox>())
                foreach (var abilityDetailItem in card.AbilityDetailDic)
                {
                    if (!abilityDetailItem.Key.Equals(checkbox.Content)) continue;
                    checkbox.IsChecked = abilityDetailItem.Value;
                    break;
                }

            UpdateTypeLinkage(card.Type);
            IsPreviewChanged = false;
        }

        public void Reset(Enum.ModeType modeType)
        {
            switch (modeType)
            {
                case Enum.ModeType.Query:
                    TxtNumber.Text = string.Empty;
                    CmbPack.Text = StringConst.NotApplicable;
                    break;
                case Enum.ModeType.Editor:
                    break;
            }

            TxtCName.Text = string.Empty;
            TxtJName.Text = string.Empty;
            TxtIllust.Text = string.Empty;
            TxtCost.Text = string.Empty;
            TxtPower.Text = string.Empty;
            TxtAbility.Text = string.Empty;
            TxtLines.Text = string.Empty;
            TxtMd5.Text = string.Empty;

            CmbType.Text = StringConst.NotApplicable;
            CmbCamp.Text = StringConst.NotApplicable;
            CmbRace.Text = StringConst.NotApplicable;
            CmbSign.Text = StringConst.NotApplicable;
            CmbRare.Text = StringConst.NotApplicable;
            CmbRestrict.Text = StringConst.NotApplicable;

            CmbRace.IsEnabled = false;

            foreach (var checkbox in LstAbilityType.Items.Cast<CheckBox>())
                checkbox.IsChecked = false;
            foreach (var checkbox in LstAbilityDetail.Items.Cast<CheckBox>())
                checkbox.IsChecked = false;
        }

        public void SetPicture(List<string> picturePathList)
        {
            var tabItemList = new List<TabItem> {TabItem0, TabItem1, TabItem2, TabItem3};
            var imageList = new List<Image> {Img0, Img1, Img2, Img3};
            for (var i = 0; i != tabItemList.Count; i++)
                if (i < picturePathList.Count)
                {
                    tabItemList[i].Visibility = Visibility.Visible;
                    try
                    {
                        imageList[i].Source = new BitmapImage(new Uri(picturePathList[i]));
                    }
                    catch (Exception)
                    {
                        imageList[i].Source = new BitmapImage();
                    }
                }
                else
                {
                    tabItemList[i].Visibility = Visibility.Hidden;
                }
            tabItemList[0].Focus();
            if (0 != picturePathList.Count) return;
            tabItemList[0].Visibility = Visibility.Hidden;
            imageList[0].Source = new BitmapImage();
        }

        public void UpdateCampLinkage(List<object> itemList)
        {
            CmbRace.Items.Clear();
            CmbRace.Text = StringConst.NotApplicable;
            itemList.ForEach(race => CmbRace.Items.Add(race.ToString()));
            CmbRace.IsEnabled = itemList.Count >= 2;
            if (!CmbType.Text.Equals(StringConst.TypeEvent) && !CmbType.Text.Equals(StringConst.TypePlayer)) return;
            CmbRace.IsEnabled = false;
            CmbRace.Text = StringConst.Hyphen;
        }

        public void SetPackItems(List<object> itemList)
        {
            CmbPack.Text = StringConst.NotApplicable;
            CmbPack.ItemsSource = itemList;
        }

        public void UpdatePreListView(List<CardPreviewModel> previewEntityList, string memoryNumber)
        {
            LvwPreview.ItemsSource = null;
            LvwPreview.ItemsSource = previewEntityList;
            LblCardCount.Content = StringConst.QueryResult + previewEntityList.Count;
            if (memoryNumber.Equals(string.Empty)) return;
            var firstOrDefault = previewEntityList
                .Select((previewEntity, index) => new {previewEntity.Number, Index = index})
                .FirstOrDefault(i => i.Number.Equals(memoryNumber));
            if (firstOrDefault == null) return;
            var position = firstOrDefault.Index;
            if (position == -1) return;
            LvwPreview.SelectedIndex = position;
            LvwPreview.ScrollIntoView(LvwPreview.SelectedItem);
            var item = LvwPreview.ItemContainerGenerator.ContainerFromIndex(position) as ListViewItem;
            item?.Focus();
        }

        public void UpdatePackLinkage(string packNumber)
        {
            TxtNumber.Text = packNumber;
            if (packNumber.IndexOf("P", StringComparison.Ordinal) == 0)
                CmbRare.Text = StringConst.RarePr;
        }

        public void UpdateTypeLinkage(string type)
        {
            switch (type)
            {
                case StringConst.NotApplicable:
                case StringConst.TypeZx:
                {
                    TxtCost.IsEnabled = true;
                    TxtPower.IsEnabled = true;
                    CmbRace.IsEnabled = true;
                    CmbSign.IsEnabled = true;
                    break;
                }
                case StringConst.TypeZxEx:
                {
                    CmbRace.IsEnabled = true;
                    CmbSign.IsEnabled = false;
                    TxtCost.IsEnabled = true;
                    TxtPower.IsEnabled = true;

                    CmbSign.Text = StringConst.Hyphen;
                    break;
                }
                case StringConst.TypePlayer:
                {
                    CmbRace.IsEnabled = false;
                    CmbSign.IsEnabled = false;
                    TxtCost.IsEnabled = false;
                    TxtPower.IsEnabled = false;

                    CmbRace.Text = StringConst.Hyphen;
                    CmbSign.Text = StringConst.Hyphen;
                    TxtCost.Text = string.Empty;
                    TxtPower.Text = string.Empty;
                    break;
                }
                case StringConst.TypeEvent:
                {
                    CmbRace.IsEnabled = false;
                    CmbSign.IsEnabled = true;
                    TxtCost.IsEnabled = true;
                    TxtPower.IsEnabled = false;

                    CmbRace.Text = StringConst.Hyphen;
                    TxtPower.Text = string.Empty;
                    break;
                }
            }
        }

        public void SetPasswordVisibility(bool isEncryptVisible, bool isDecryptVisible)
        {
            BtnEncrypt.Visibility = isEncryptVisible ? Visibility.Visible : Visibility.Hidden;
            BtnDecrypt.Visibility = isDecryptVisible ? Visibility.Visible : Visibility.Hidden;
        }

        /************************************************** 事件 **************************************************/

        /// <summary>程序载入</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _presenter.Init();
        }

        /// <summary>退出</summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ExitClick();
        }

        /// <summary>添加</summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            _presenter.AddClick(GetCardEditorModel());
        }

        /// <summary>更新</summary>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            _presenter.UpdateClick(GetCardEditorModel(), LvwPreview.SelectedItem);
        }

        /// <summary>删除</summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeleteClick(GetCardEditorModel(), LvwPreview.SelectedItem);
        }

        /// <summary>查询</summary>
        private void Query_Click(object sender, RoutedEventArgs e)
        {
            _presenter.QueryClick(GetCardEditorModel());
        }

        /// <summary>重置</summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ResetClick(CmbMode.Text.Trim());
        }

        /// <summary>列表选择</summary>
        private void LvCardPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.PreviewChanged(LvwPreview.SelectedItem);
        }

        /// <summary>加密数据库</summary>
        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            // 使用.NET内部算法把IntPtr指向处的字符集合转换成字符串  
            _presenter.EncryptDatabaseClick(
                Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(TxtPassword.SecurePassword)));
        }

        /// <summary>解密数据库</summary>
        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DecryptDatabaseClick(
                Marshal.PtrToStringBSTR(Marshal.SecureStringToBSTR(TxtPassword.SecurePassword)));
        }

        /// <summary>改变排序方式</summary>
        private void CmbOrder_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.PreOrderChanged(CmbMode.Text.Trim(), CmbOrder.Text.Trim());
        }

        /// <summary>一键导出事件</summary>
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ExportClick(CmbPack.Text.Trim());
        }

        /// <summary>能力文本改变事件</summary>
        private void TxtAbility_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsPreviewChanged) return;
            _presenter.AbilityChanged(TxtAbility.Text.Trim());
        }

        /// <summary>类型选择</summary>
        private void CmbType_TextChanged(object sender, RoutedEventArgs e)
        {
            if (IsPreviewChanged) return;
            _presenter.TypeChanged(CmbType.Text.Trim());
        }

        /// <summary>阵营选择</summary>
        private void CmbCamp_TextChanged(object sender, EventArgs e)
        {
            if (IsPreviewChanged) return;
            _presenter.CampChanged(CmbCamp.Text.Trim());
        }

        /// <summary>卡包选择</summary>
        private void CmbPack_TextChanged(object sender, RoutedEventArgs e)
        {
            if (IsPreviewChanged) return;
            _presenter.PackChanged(CmbPack.Text.Trim(), TxtNumber.Text.Trim());
        }

        /// <summary>窗口最小化事件</summary>
        private void Title_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>标题拖拽事件</summary>
        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CmbMode_TextChanged(object sender, RoutedEventArgs e)
        {
            _presenter.ModeChanged(GetCardEditorModel());
        }

        /// <summary>Md5覆写事件</summary>
        private void Md5_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Md5Click();
        }

        /// <summary>
        ///     卡包覆写事件
        /// </summary>
        private void PackCover_Click(object sender, RoutedEventArgs e)
        {
            _presenter.PackCoverClick();
        }
    }
}