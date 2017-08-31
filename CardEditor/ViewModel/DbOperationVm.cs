using System.Windows;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class DbOperationVm : BaseModel
    {
        private Visibility _decryptVisibility;

        private Visibility _encryptVisibility;

        private string _password;

        public DbOperationVm()
        {
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
            if (SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache))
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
                BaseDialogUtils.ShowDlgOk(StringConst.PasswordNone);
                return;
            }
            if (SqliteUtils.Encrypt(DataCache.DsAllCache))
            {
                UpdatePasswordVisibility(false, true);
                BaseDialogUtils.ShowDlg(StringConst.EncryptSucced);
                return;
            }
            BaseDialogUtils.ShowDlgOk(StringConst.EncryptFailed);
        }

        public void Decrypt_Click(object obj)
        {
            if (Password.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDlgOk(StringConst.PasswordNone);
                return;
            }
            if (SqliteUtils.Decrypt())
            {
                UpdatePasswordVisibility(true, false);
                BaseDialogUtils.ShowDlg(StringConst.DncryptSucced);
                return;
            }
            BaseDialogUtils.ShowDlgOk(StringConst.DncryptFailed);
        }

        private void UpdatePasswordVisibility(bool isEncryptVisible, bool isDecryptVisible)
        {
            EncryptVisibility = isEncryptVisible ? Visibility.Visible : Visibility.Hidden;
            DecryptVisibility = isDecryptVisible ? Visibility.Visible : Visibility.Hidden;
        }
    }
}