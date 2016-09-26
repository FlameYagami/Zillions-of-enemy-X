using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CardEditor.Constant;
using CardEditor.Entity;
using CardEditor.Presenter;
using CardEditor.Utils;
using Common;

namespace CardEditor.View
{
    public interface IView
    {
        void SetPackItems(List<object> itemList);
        void SetCardEntity(CardEntity cardEntity);
        void SetPasswordVisibility(bool isEncryptVisible, bool isDecryptVisible);
        void SetPicture(List<string> picturePathList);
        void Reset();
        void UpdateListView(List<PreviewEntity> previewEntityList);
        void UpdatePackLinkage(string packNumber);
        void UpdateTypeLinkage(string type);
        void UpdateCampLinkage(List<object> itemList);
        void UpdateAbilityLinkage(StringConst.AbilityType abilityType);
        CardEntity GetCardEntity();
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

        /************************************************** 接口实现 **************************************************/

        public void UpdateAbilityLinkage(StringConst.AbilityType abilityType)
        {
            switch (abilityType)
            {
                case StringConst.AbilityType.Ig:
                {
                    CmbType.Text = StringConst.TypeZx;
                    CmbSign.Text = StringConst.SignIg;
                    break;
                }
                case StringConst.AbilityType.Start:
                {
                    CmbType.Text = StringConst.TypeZx;
                    CmbSign.Text = StringConst.Hyphen;
                    break;
                }
                case StringConst.AbilityType.Event:
                {
                    CmbType.Text = StringConst.TypeEvent;
                    CmbRace.Text = StringConst.Hyphen;
                    TxtPower.Text = string.Empty;
                    break;
                }
                case StringConst.AbilityType.Extra:
                {
                    CmbType.Text = StringConst.TypeZxEx;
                    CmbSign.Text = StringConst.Hyphen;
                    break;
                }
            }
        }

        public CardEntity GetCardEntity()
        {
            var abilityDetialEntity = CardUtils.GetAbilityDetialModel(LstAbilityDetail);
            return new CardEntity
            {
                Type = CmbType.Text.Trim(),
                Camp = CmbCamp.Text.Trim(),
                Race = CmbRace.Text.Trim(),
                Sign = CmbSign.Text.Trim(),
                Rare = CmbRare.Text.Trim(),
                Pack = CmbPack.Text.Trim(),
                Restrict = CmbRestrict.Text.Trim(),
                CName = TxtCName.Text.Trim(),
                JName = TxtJName.Text.Trim(),
                Illust = TxtIllust.Text.Trim(),
                Number = TxtNumber.Text.Trim(),
                Cost = TxtCost.Text.Trim(),
                Power = TxtPower.Text.Trim(),
                Ability = TxtAbility.Text.Trim(),
                Lines = TxtLines.Text.Trim(),
                Faq = TxtFaq.Text.Trim(),
                AbilityType =
                    SqlUtils.GetAbilitySql(LstAbilityType, Const.AbilityTypeDic.Keys.ToList(), SqliteConst.Ability),
                AbilityDetail =
                    SqlUtils.GetAbilitySql(LstAbilityDetail, abilityDetialEntity.GetAbilityDetailDic().Keys.ToList(),
                        SqliteConst.AbilityDetail),
                AbilityDetialEntity = abilityDetialEntity
            };
        }

        public void SetCardEntity(CardEntity cardEntity)
        {
            CmbType.Text = cardEntity.Type;
            CmbCamp.Text = cardEntity.Camp;
            CmbRace.Text = cardEntity.Race;
            CmbSign.Text = cardEntity.Sign;
            CmbRare.Text = cardEntity.Rare;
            CmbPack.Text = cardEntity.Pack;
            CmbRestrict.Text = cardEntity.Restrict.Equals("4") ? StringConst.NotApplicable : cardEntity.Restrict;

            TxtCName.Text = cardEntity.CName;
            TxtJName.Text = cardEntity.JName;
            TxtIllust.Text = cardEntity.Illust;
            TxtNumber.Text = cardEntity.Number;
            TxtCost.Text = cardEntity.Cost;
            TxtPower.Text = cardEntity.Power;

            TxtAbility.Text = cardEntity.Ability;
            TxtLines.Text = cardEntity.Lines;
            TxtFaq.Text = cardEntity.Faq;

            foreach (var checkbox in LstAbilityType.Items.Cast<CheckBox>())
                foreach (var abilityType in Const.AbilityTypeDic)
                {
                    if (!checkbox.Content.Equals(abilityType.Key)) continue;
                    checkbox.IsChecked = cardEntity.Ability.Contains(abilityType.Value);
                    break;
                }

            var abilityDetialModel = JsonUtils.JsonDeserialize<AbilityDetialEntity>(cardEntity.AbilityDetail);
            var abilityDetialItems = abilityDetialModel.GetAbilityDetailDic();
            foreach (var checkbox in LstAbilityDetail.Items.Cast<CheckBox>())
                foreach (var abilityDetailItem in abilityDetialItems)
                {
                    if (!abilityDetailItem.Key.Equals(checkbox.Content)) continue;
                    checkbox.IsChecked = abilityDetailItem.Value.Equals(1);
                    break;
                }
        }

        public void Reset()
        {
            foreach (var control in GridQuery.Children)
            {
                var box = control as ComboBox;
                if (box != null)
                    box.Text = StringConst.NotApplicable;
                var textBox = control as TextBox;
                if (textBox != null)
                    textBox.Text = string.Empty;
            }
            foreach (var checkbox in LstAbilityType.Items.Cast<CheckBox>())
                checkbox.IsChecked = false;
            foreach (var checkbox in LstAbilityDetail.Items.Cast<CheckBox>())
                checkbox.IsChecked = false;
            TxtAbility.Text = string.Empty;
            TxtLines.Text = string.Empty;
            TxtFaq.Text = string.Empty;
            CmbRestrict.Text = StringConst.NotApplicable;
            CmbRace.IsEnabled = false;
        }

        public void SetPicture(List<string> picturePathList)
        {
            var tabItemList = new List<TabItem> {TabItem0, TabItem1, TabItem2, TabItem3};
            var imageList = new List<Image> {Img0, Img1, Img2, Img3};
            for (var i = 0; i != tabItemList.Count; i++)
                if (i < picturePathList.Count)
                {
                    tabItemList[i].Visibility = Visibility.Visible;
                    imageList[i].Source = new BitmapImage(new Uri(picturePathList[i]));
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
        }

        public void SetPackItems(List<object> itemList)
        {
            CmbPack.Text = StringConst.NotApplicable;
            CmbPack.ItemsSource = itemList;
        }

        public void UpdateListView(List<PreviewEntity> previewEntityList)
        {
            LvwPreview.ItemsSource = null;
            LvwPreview.ItemsSource = previewEntityList;
            LblCardCount.Content = StringConst.QueryResult + previewEntityList.Count;
        }

        public void UpdatePackLinkage(string packNumber)
        {
            if (packNumber.Equals(string.Empty))
                TxtNumber.Text = string.Empty;
            else
                TxtNumber.Text = TxtNumber.Text.Length >= 4
                    ? TxtNumber.Text.Replace(TxtNumber.Text.Substring(0, 4), packNumber)
                    : packNumber;
            if (packNumber.Contains("P"))
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
            _presenter.AddClick(CmbOrder.Text.Trim());
        }

        /// <summary>更新</summary>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            _presenter.UpdateClick(LvwPreview.SelectedIndex, CmbOrder.Text.Trim());
        }

        /// <summary>删除</summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DeleteClick(LvwPreview.SelectedIndex, CmbOrder.Text.Trim());
        }

        /// <summary>查询</summary>
        private void Query_Click(object sender, RoutedEventArgs e)
        {
            _presenter.QueryClick(CmbMode.Text.Trim(), CmbOrder.Text.Trim(), CmbPack.Text.Trim());
        }

        /// <summary>重置</summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ResetClick();
        }

        /// <summary>列表选择</summary>
        private void LvCardPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.PreviewChanged(LvwPreview.SelectedIndex);
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
            _presenter.OrderChanged(CmbOrder.Text.Trim());
        }

        /// <summary>一键导出事件</summary>
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            _presenter.ExportClick(CmbPack.Text.Trim());
        }

        /// <summary>能力文本改变事件</summary>
        private void TxtAbility_TextChanged(object sender, TextChangedEventArgs e)
        {
            _presenter.AbilityChanged(TxtAbility.Text.Trim());
        }

        /// <summary>类型选择（因为Style关系,下拉不会触发改事件）</summary>
        private void CmbType_TextChanged(object sender, RoutedEventArgs e)
        {
            _presenter.TypeChanged(CmbType.Text.Trim());
        }

        /// <summary>阵营选择</summary>
        private void CmbCamp_TextChanged(object sender, EventArgs e)
        {
            _presenter.CampChanged(CmbCamp.Text.Trim());
        }

        /// <summary>卡包选择</summary>
        private void CmbPack_TextChanged(object sender, RoutedEventArgs e)
        {
            _presenter.PackChanged(CmbPack.Text.Trim());
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
    }
}