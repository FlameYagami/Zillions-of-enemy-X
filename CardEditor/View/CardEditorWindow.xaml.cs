using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CardEditor.ViewModel;
using Common;
using Dialog;
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
        private AbilityTypeVm _abilityTypeVm;
        private CardQueryVm _cardQueryVm;
        private CardPictureVm _cardPictureVm;
        private CardPreviewVm _cardPreviewVm;
        private DbOperationVm _dbOperationVm;
        private CardQueryExVm _externQueryVm;

        public MainWindow()
        {
            InitializeComponent();
//            LogUtils.Show();
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
            _abilityTypeVm = new AbilityTypeVm();
            _externQueryVm = new CardQueryExVm();
            _cardQueryVm = new CardQueryVm(_abilityTypeVm, _externQueryVm, _cardPreviewVm, _cardPictureVm);

            AbilityTypeView.DataContext = _abilityTypeVm;
            ExternQueryView.DataContext = _externQueryVm;
            CardPreviewView.DataContext = _cardPreviewVm;
            CardPictureView.DataContext = _cardPictureVm;
            CardEditorView.DataContext = _cardQueryVm;
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
            _cardQueryVm.UpdateCardQueryModel(cardmodel);
        }

        /// <summary>�����ı��ı��¼�</summary>
        private void TxtAbility_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Xaml��Textbox��TextChanged�¼�����������Vm�����Binding���Ըı䣬������������ֶ�ȡֵ
            _cardQueryVm?.UpdateAbilityLinkage((sender as TextBox).Text);
        }

        /// <summary>����ѡ��</summary>
        private void CmbType_TextChanged(object sender, RoutedEventArgs e)
        {
            _cardQueryVm?.UpdateTypeLinkage();
        }

        /// <summary>��Ӫѡ��</summary>
        private void CmbCamp_TextChanged(object sender, EventArgs e)
        {
            _cardQueryVm?.UpdateRaceLinkage();
        }

        /// <summary>����ѡ��</summary>
        private void CmbPack_TextChanged(object sender, RoutedEventArgs e)
        {
            _cardQueryVm?.UpdatePackLinkage();
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
            _cardQueryVm.Query_Click(null);
        }
    }
}