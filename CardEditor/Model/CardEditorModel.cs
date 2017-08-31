using System.Collections.ObjectModel;
using System.Linq;
using Wrapper.Constant;
using Wrapper.Model;

namespace CardEditor.Model
{
    public class CardEditorModel : BaseModel
    {
        private string _ability;
        private string _camp;
        private string _cname;
        private bool _costEnabled;
        private string _costValue;
        private string _illust;
        private string _jname;
        private string _lines;

        private string _md5;
        private string _number;
        private string _pack;
        private bool _powerEnabled;
        private string _powerValue;
        private string _race;
        private bool _raceEnabled;
        private string _rare;
        private string _sign;
        private bool _signEnabled;
        private string _type;

        public CardEditorModel()
        {
            Type = StringConst.NotApplicable;
            Camp = StringConst.NotApplicable;
            Race = StringConst.NotApplicable;
            Sign = StringConst.NotApplicable;
            Rare = StringConst.NotApplicable;
            Pack = StringConst.NotApplicable;

            CName = string.Empty;
            JName = string.Empty;
            Number = string.Empty;
            Illust = string.Empty;
            CostValue = string.Empty;
            PowerValue = string.Empty;
            Ability = string.Empty;
            Lines = string.Empty;

            InitAbilityTypeModels();
            InitAbilityDetailModels();
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public string Camp
        {
            get { return _camp; }
            set
            {
                _camp = value;
                OnPropertyChanged(nameof(Camp));
            }
        }

        public string Race
        {
            get { return _race; }
            set
            {
                _race = value;
                OnPropertyChanged(nameof(Race));
            }
        }

        public string Sign
        {
            get { return _sign; }
            set
            {
                _sign = value;
                OnPropertyChanged(nameof(Sign));
            }
        }

        public string Rare
        {
            get { return _rare; }
            set
            {
                _rare = value;
                OnPropertyChanged(nameof(Rare));
            }
        }

        public string Pack
        {
            get { return _pack; }
            set
            {
                _pack = value;
                OnPropertyChanged(nameof(Pack));
            }
        }

        public string Illust
        {
            get { return _illust; }
            set
            {
                _illust = value;
                OnPropertyChanged(nameof(Illust));
            }
        }

        public string CName
        {
            get { return _cname; }
            set
            {
                _cname = value;
                OnPropertyChanged(nameof(CName));
            }
        }

        public string JName
        {
            get { return _jname; }
            set
            {
                _jname = value;
                OnPropertyChanged(nameof(JName));
            }
        }

        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        public string CostValue
        {
            get { return _costValue; }
            set
            {
                _costValue = value;
                OnPropertyChanged(nameof(CostValue));
            }
        }

        public string PowerValue
        {
            get { return _powerValue; }
            set
            {
                _powerValue = value;
                OnPropertyChanged(nameof(PowerValue));
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

        public string Ability
        {
            get { return _ability; }
            set
            {
                _ability = value;
                OnPropertyChanged(nameof(Ability));
            }
        }

        public string Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                OnPropertyChanged(nameof(Lines));
            }
        }

        public string ImageJson { get; set; }

        public bool CostEnabled
        {
            get { return _costEnabled; }
            set
            {
                _costEnabled = value;
                OnPropertyChanged(nameof(CostEnabled));
            }
        }

        public bool PowerEnabled
        {
            get { return _powerEnabled; }
            set
            {
                _powerEnabled = value;
                OnPropertyChanged(nameof(PowerEnabled));
            }
        }

        public bool RaceEnabled
        {
            get { return _raceEnabled; }
            set
            {
                _raceEnabled = value;
                OnPropertyChanged(nameof(RaceEnabled));
            }
        }

        public bool SignEnabled
        {
            get { return _signEnabled; }
            set
            {
                _signEnabled = value;
                OnPropertyChanged(nameof(SignEnabled));
            }
        }

        public ObservableCollection<AbilityModel> AbilityTypeModels { get; set; }
        public ObservableCollection<AbilityModel> AbilityDetailModels { get; set; }

        private void InitAbilityTypeModels()
        {
            AbilityTypeModels = new ObservableCollection<AbilityModel>();
            Dic.AbilityTypeDic.Keys.ToList().ForEach(key => AbilityTypeModels.Add(new AbilityModel
            {
                Name = key,
                Checked = false
            }));
        }

        private void InitAbilityDetailModels()
        {
            AbilityDetailModels = new ObservableCollection<AbilityModel>();
            Dic.AbilityDetailDic.ToList().ForEach(pair => AbilityDetailModels.Add(new AbilityModel
            {
                Name = pair.Key,
                Code = pair.Value,
                Checked = false
            }));
        }
    }
}