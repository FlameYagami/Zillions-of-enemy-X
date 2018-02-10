using System.Collections.Generic;
using CardEditor.Utils;
using Common;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class CardQueryExVm : BaseModel
    {
        private string _md5;
        private Enums.ModeType _modeType;
        private string _restrictValue;

        public CardQueryExVm()
        {
            ModeDic = Dic.ModeDic;
            RestrctList = CardUtils.GetRestrictList();
            CmdMd5Cover = new DelegateCommand {ExecuteCommand = Md5Cover_Click};
            CmdPackCover = new DelegateCommand {ExecuteCommand = PackCover_Click};
        }

        public List<string> RestrctList { get; set; }
        public Dictionary<Enums.ModeType, string> ModeDic { get; set; }
        public DelegateCommand CmdMd5Cover { get; set; }
        public DelegateCommand CmdPackCover { get; set; }

        public Enums.ModeType ModeType
        {
            get { return _modeType; }
            set
            {
                _modeType = value;
                OnPropertyChanged(nameof(ModeType));
            }
        }

        public string Md5
        {
            get { return _md5; }
            set
            {
                _md5 = value;
                OnPropertyChanged(nameof(Md5));
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
            if (!BaseDialogUtils.ShowDialogConfirm("确认覆写?")) return;
            var sqlList = SqlUtils.GetMd5SqlList();
            var succeed = DataManager.Execute(sqlList);
            BaseDialogUtils.ShowDialogAuto(succeed ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
        }

        public void UpdateRestrictValue(int restrict)
        {
            RestrictValue = restrict == 4 ? StringConst.NotApplicable : restrict.ToString();
        }
    }
}