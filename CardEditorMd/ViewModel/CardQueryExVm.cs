using System.Collections.Generic;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class CardQueryExVm : BaseModel
    {
        private string _md5Value;
        private Enums.ModeType _modeType;

        private string _restrictValue;

        public CardQueryExVm()
        {
            ModeDic = Dic.ModeDic;
            RestrctList = CardUtils.GetRestrictList();
        }

        public List<string> RestrctList { get; set; }
        public Dictionary<Enums.ModeType, string> ModeDic { get; set; }

        public Enums.ModeType ModeType
        {
            get { return _modeType; }
            set
            {
                _modeType = value;
                OnPropertyChanged(nameof(ModeType));
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

        public string Md5Value
        {
            get { return _md5Value; }
            set
            {
                _md5Value = value;
                OnPropertyChanged(nameof(Md5Value));
            }
        }

        public void UpdateRestrictValue(int restrict)
        {
            RestrictValue = restrict == 4 ? StringConst.NotApplicable : restrict.ToString();
        }
    }
}