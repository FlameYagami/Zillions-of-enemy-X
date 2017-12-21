using System.Collections.Generic;
using System.Linq;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class CardQueryExVm : BaseModel
    {
        private string _md5Value;
        private string _modeValue;

        private string _restrictValue;

        public CardQueryExVm()
        {
            ModeList = Dic.ModeDic.Values.ToList();
            RestrctList = CardUtils.GetRestrictList();
        }

        public List<string> RestrctList { get; set; }
        public List<string> ModeList { get; set; }

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