using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CardEditor.Constant;
using CardEditor.Entity;
using CardEditor.Utils;
using CardEditor.Utils.Dialog;
using DeckEditor.Utils;

namespace CardEditor.MVP
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Constract.IView
    {
        private readonly Constract.IPresenter _presenter;

        public MainWindow()
        {
            InitializeComponent();
            _presenter = new Presenter(this);
        }

        public void SetBackground()
        {
            var uri = new Uri(Const.BackgroundPath, UriKind.Relative);
            var imageBrush = new ImageBrush {ImageSource = new BitmapImage(uri)};
            Background = imageBrush;
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
                AbilityType = SqlUtils.GetAbilitySql(LstAbilityType, Const.AbilityTypeDic.Keys.ToList(), SqliteConst.Ability),
                AbilityDetail = SqlUtils.GetAbilitySql(LstAbilityDetail, abilityDetialEntity.GetAbilityDetailDic().Keys.ToList(), SqliteConst.AbilityDetail),
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
            CmbRestrict.Text = cardEntity.Restrict.Equals("4") ? StringConst.NotApplicable : "0";

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

        public void ResetAbility()
        {
            foreach (var checkbox in LstAbilityType.Items.Cast<CheckBox>())
                checkbox.IsChecked = false;
            foreach (var checkbox in LstAbilityDetail.Items.Cast<CheckBox>())
                checkbox.IsChecked = false;
        }

        public void SetImage(List<string> imageListUri)
        {
            var tabItemList = new List<TabItem> {TabItem0, TabItem1, TabItem2, TabItem3};
            var imageList = new List<Image> {Img0, Img1, Img2, Img3};
            for (var i = 0; i != tabItemList.Count; i++)
                if (i < imageListUri.Count)
                {
                    tabItemList[i].Visibility = Visibility.Visible;
                    imageList[i].Source = new BitmapImage(new Uri(imageListUri[i]));
                }
                else
                {
                    tabItemList[i].Visibility = Visibility.Hidden;
                }
            tabItemList[0].Focus();
            if (0 != imageListUri.Count) return;
            tabItemList[0].Visibility = Visibility.Hidden;
            imageList[0].Source = new BitmapImage();
        }

        /// <summary>
        ///     得到ListView的选择索引
        /// </summary>
        /// <returns></returns>
        public int GetSelectIndex()
        {
            return LvCardPreview.SelectedIndex;
        }

        public void ShowDialog(string hint)
        {
            DialogUtils.ShowDlg(hint);
        }

        public void SetType(string message)
        {
            CmbType.Text = message;
        }

        public void SetCamp(string message)
        {
            CmbCamp.Text = message;
        }

        public void SetRace(string message)
        {
            CmbRace.Text = message;
        }

        public void SetSign(string message)
        {
            CmbSign.Text = message;
        }

        public void SetRare(string message)
        {
            CmbRare.Text = message;
        }

        public void SetCName(string message)
        {
            TxtCName.Text = message;
        }

        public void SetJName(string message)
        {
            TxtJName.Text = message;
        }

        public void SetIllust(string message)
        {
            TxtIllust.Text = message;
        }

        public void SetPack(string message)
        {
            CmbPack.Text = message;
        }

        public void SetNumber(string message)
        {
            if (message.Equals(string.Empty))
                TxtNumber.Text = string.Empty;
            else
                TxtNumber.Text = TxtNumber.Text.Length >= 4
                    ? TxtNumber.Text.Replace(TxtNumber.Text.Substring(0, 4), message)
                    : message;
        }

        public void SetCost(string message)
        {
            TxtCost.Text = message;
        }

        public void SetPower(string message)
        {
            TxtPower.Text = message;
        }

        public void SetAbility(string message)
        {
            TxtAbility.Text = message;
        }

        public void SetLimit(string message)
        {
            CmbRestrict.Text = message;
        }

        public void SetLines(string message)
        {
            TxtLines.Text = message;
        }

        public void SetFaq(string message)
        {
            TxtFaq.Text = message;
        }

        public void SetCampEnabled(bool isEnabled)
        {
            CmbCamp.IsEnabled = IsEnabled;
        }

        public void SetRaceEnabled(bool isEnabled)
        {
            CmbRace.IsEnabled = isEnabled;
        }

        public void SetSignEnabled(bool isEnabled)
        {
            CmbSign.IsEnabled = isEnabled;
        }

        public void SetCostEnabled(bool isEnabled)
        {
            TxtCost.IsEnabled = isEnabled;
        }

        public void SetPowerEnabled(bool isEnabled)
        {
            TxtPower.IsEnabled = isEnabled;
        }

        public void SetRaceItems(List<object> itemList)
        {
            CmbRace.ItemsSource = null;
            CmbRace.ItemsSource = itemList;
            CmbRace.Text = StringConst.NotApplicable;
        }

        public void SetPackItems(List<object> itemList)
        {
            CmbPack.Text = StringConst.NotApplicable;
            CmbPack.ItemsSource = itemList;
        }

        public string GetPack()
        {
            return CmbPack.Text.Trim();
        }

        public new string GetType()
        {
            return CmbType.Text.Trim();
        }

        public string GetCamp()
        {
            return CmbCamp.Text.Trim();
        }

        public string GetMode()
        {
            return CmbMode.Text.Trim();
        }

        public void UpdateListView(List<PreviewEntity> cardList)
        {
            LvCardPreview.ItemsSource = null;
            LvCardPreview.ItemsSource = cardList;
            LblCardCount.Content = StringConst.QueryResult + cardList.Count;
        }

        public string GetPassword()
        {
            var p = Marshal.SecureStringToBSTR(TxtPassword.SecurePassword);
            return Marshal.PtrToStringBSTR(p); // 使用.NET内部算法把IntPtr指向处的字符集合转换成字符串  
        }

        public void SetPasswordVisibility(bool isEncryptVisible, bool isDecryptVisible)
        {
            BtnEncrypt.Visibility = isEncryptVisible ? Visibility.Visible : Visibility.Hidden;
            BtnDecrypt.Visibility = isDecryptVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public string GetOrder()
        {
            return CmbOrder.Text.Trim();
        }

        /// <summary>程序载入</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _presenter.Init();
        }

        /// <summary>退出</summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Exit();
        }

        /// <summary>添加</summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            _presenter.AddCard();
        }

        /// <summary>更新</summary>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            _presenter.UpdateCard();
        }

        /// <summary>删除</summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Delete();
        }

        /// <summary>查询</summary>
        private void Query_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Query();
        }

        /// <summary>重置</summary>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Reset();
        }

        /// <summary>卡包选择</summary>
        private void Pack_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.PackChanged();
        }

        /// <summary>类型选择</summary>
        private void Type_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.TypeChanged();
        }

        /// <summary>阵营选择</summary>
        private void Camp_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.CampChanged();
        }

        /// <summary>列表选择</summary>
        private void LvCardPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.CardPreviewChanged();
        }

        private void CmbCamp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.CampChanged();
        }

        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _presenter.TypeChanged();
        }

        /// <summary>加密数据库</summary>
        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            _presenter.EncryptDatabase();
        }

        /// <summary>解密数据库</summary>
        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            _presenter.DecryptDatabase();
        }

        /// <summary>改变排序方式</summary>
        private void CmbOrder_DropDownClosed(object sender, EventArgs e)
        {
            _presenter.Order();
        }
    }
}