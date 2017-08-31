using System.Collections.Generic;
using System.Linq;
using CardEditor.Utils;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class ExternQueryVm : BaseModel
    {
        private string _modeValue;

        private string _restrictValue;

        public ExternQueryVm()
        {
            ModeList = Dic.ModeDic.Values.ToList();
            RestrctList = CardUtils.GetRestrictList();
            CmdMd5Cover = new DelegateCommand {ExecuteCommand = Md5Cover_Click};
            CmdPackCover = new DelegateCommand {ExecuteCommand = PackCover_Click};
        }

        public List<string> RestrctList { get; set; }
        public List<string> ModeList { get; set; }
        public DelegateCommand CmdMd5Cover { get; set; }
        public DelegateCommand CmdPackCover { get; set; }

        public string ModeValue
        {
            get { return _modeValue; }
            set
            {
                _modeValue = value;
                OnPropertyChanged(nameof(ModeValue));
            }
        }

        public string RestrictValue
        {
            get { return _restrictValue; }
            set
            {
                _restrictValue = value;
                OnPropertyChanged(nameof(RestrictValue));
            }
        }

        public void PackCover_Click(object obj)
        {
            DialogUtils.ShowPackCover();
        }

        public void Md5Cover_Click(object obj)
        {
            if (!BaseDialogUtils.ShowDlgOkCancel("确认覆写?")) return;
            var sqlList = SqlUtils.GetMd5SqlList();
            var succeed = SqliteUtils.Execute(sqlList);
            BaseDialogUtils.ShowDlg(succeed ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
        }

        public void UpdateRestrictValue(int restrict)
        {
            RestrictValue = restrict == 4 ? StringConst.NotApplicable : restrict.ToString();
        }
    }
}