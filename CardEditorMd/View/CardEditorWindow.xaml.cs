using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CardEditor.ViewModel;
using Dialog;
using MaterialDesignThemes.Wpf;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.View
{
    /// <summary>
    ///     MainWindow.xaml �Ľ����߼�
    /// </summary>
    public partial class MainWindow
    {
        private CardQueryVm _cardEditorVm;
        private CardPictureVm _cardPictureVm;
        private CardPreviewVm _cardPreviewVm;
        private DbOperationVm _dbOperationVm;
        private CardQueryExVm _externQueryVm;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>��������</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dbOperationVm = new DbOperationVm(this);
            DbOperationView.DataContext = _dbOperationVm;
            if (!_dbOperationVm.UpdateDataset())
            {
                BaseDialogUtils.ShowDialogOk(StringConst.DbOpenError);
                return;
            }
            InitView();
        }

        public void InitView()
        {
            _cardPreviewVm = new CardPreviewVm();
            _cardPictureVm = new CardPictureVm();
            _externQueryVm = new CardQueryExVm();
            _cardEditorVm = new CardQueryVm(_externQueryVm, _cardPreviewVm, _cardPictureVm);

            ExternQueryView.DataContext = _externQueryVm;
            CardPreviewView.DataContext = _cardPreviewVm;
            CardPictureView.DataContext = _cardPictureVm;
            CardEditorView.DataContext = _cardEditorVm;
        }

        /// <summary>�˳�</summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>�б�ѡ��</summary>
        private void LvCardPreview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previewModel = LvCardPreview.SelectedItem as CardPreviewModel;
            if (null == previewModel) return;
            var cardmodel = CardUtils.GetCardModel(previewModel.Number);
            _cardEditorVm.UpdateCardEditorModel(cardmodel);
        }

        /// <summary>�����ı��ı��¼�</summary>
        private void TxtAbility_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Xaml��Textbox��TextChanged�¼�����������Vm�����Binding���Ըı䣬������������ֶ�ȡֵ
            _cardEditorVm?.UpdateAbilityLinkage((sender as TextBox).Text);
        }

        /// <summary>����ѡ��</summary>
        private void CmbType_TextChanged(object sender, RoutedEventArgs e)
        {
            _cardEditorVm?.UpdateTypeLinkage();
        }

        /// <summary>��Ӫѡ��</summary>
        private void CmbCamp_TextChanged(object sender, EventArgs e)
        {
            _cardEditorVm?.UpdateRaceList();
        }

        /// <summary>����ѡ��</summary>
        private void CmbPack_TextChanged(object sender, RoutedEventArgs e)
        {
            _cardEditorVm?.UpdatePackLinkage();
        }

        /// <summary>������С���¼�</summary>
        private void Title_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>������ק�¼�</summary>
        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CmbOrder_DropDownClosed(object sender, EventArgs e)
        {
            _cardPreviewVm.Order();
        }

        private void CmdMode_OnDropDownClosed(object sender, EventArgs e)
        {
            _cardEditorVm.Query_Click(null);
        }

        private async void PackCover_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = new PackCoverWindow();
            await DialogHost.Show(vm);
        }

        private async void Md5Cover_OnClick(object sender, RoutedEventArgs e)
        {
            if (!await BaseDialogUtils.ShowDialogConfirm("ȷ�ϸ�д?")) return;
            var sqlList = SqlUtils.GetMd5SqlList();
            var succeed = DataManager.Execute(sqlList);
            BaseDialogUtils.ShowDialogAuto(succeed ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
        }
    }
}