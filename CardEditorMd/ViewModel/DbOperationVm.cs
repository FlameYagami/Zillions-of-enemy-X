using System.Windows;
using CardEditor.View;
using Common;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class DbOperationVm : BaseModel
    {
        private readonly MainWindow _cardEditor;
        private Visibility _decryptVisibility;

        private Visibility _encryptVisibility;

        private string _password;

        public DbOperationVm(MainWindow cardEditor)
        {
            _cardEditor = cardEditor;
            CmdDecrypt = new DelegateCommand {ExecuteCommand = Decrypt_Click};
            CmdEncrypt = new DelegateCommand {ExecuteCommand = Encrypt_Click};
        }

        public DelegateCommand CmdEncrypt { get; set; }
        public DelegateCommand CmdDecrypt { get; set; }

        public Visibility EncryptVisibility
        {
            get { return _encryptVisibility; }
            set
            {
                _encryptVisibility = value;
                OnPropertyChanged(nameof(EncryptVisibility));
            }
        }

        public Visibility DecryptVisibility
        {
            get { return _decryptVisibility; }
            set
            {
                _decryptVisibility = value;
                OnPropertyChanged(nameof(DecryptVisibility));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool UpdateDataset()
        {
            if (DataManager.FillDataToDataSet(DataManager.DsAllCache, SqlUtils.GetQueryAllSql()))
            {
                UpdatePasswordVisibility(false, true);
                return true;
            }
            UpdatePasswordVisibility(true, false);
            return false;
        }

        public void Encrypt_Click(object obj)
        {
            if (Password.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDialogOk(StringConst.PasswordNone);
                return;
            }
            if (DataManager.Encrypt(DataManager.DsAllCache, SqlUtils.GetQueryAllSql()))
            {
                UpdatePasswordVisibility(false, true);
                BaseDialogUtils.ShowDialogAuto(StringConst.EncryptSucced);
                _cardEditor.InitView();
                return;
            }
            BaseDialogUtils.ShowDialogOk(StringConst.EncryptFailed);
        }

        public void Decrypt_Click(object obj)
        {
            if (Password.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDialogOk(StringConst.PasswordNone);
                return;
            }
            if (DataManager.Decrypt())
            {
                UpdatePasswordVisibility(true, false);
                BaseDialogUtils.ShowDialogAuto(StringConst.DncryptSucced);
                return;
            }
            BaseDialogUtils.ShowDialogOk(StringConst.DncryptFailed);
        }

        private void UpdatePasswordVisibility(bool isEncryptVisible, bool isDecryptVisible)
        {
            EncryptVisibility = isEncryptVisible ? Visibility.Visible : Visibility.Hidden;
            DecryptVisibility = isDecryptVisible ? Visibility.Visible : Visibility.Hidden;
        }
    }
}