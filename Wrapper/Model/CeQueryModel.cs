using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using Wrapper.Constant;
using Wrapper.Utils;

namespace Wrapper.Model
{
    public class CeQueryModel : BaseModel
    {
        private string _ability;

        private ObservableCollection<AbilityModel> _abilityDetailModels;

        private ObservableCollection<AbilityModel> _abilityTypeModels;
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

        public CeQueryModel()
        {
            InitCeQueryModel();
        }

        public void InitCeQueryModel()
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

        public ObservableCollection<AbilityModel> AbilityTypeModels
        {
            get { return _abilityTypeModels; }
            set
            {
                _abilityTypeModels = value;
                OnPropertyChanged(nameof(AbilityTypeModels));
            }
        }

        public ObservableCollection<AbilityModel> AbilityDetailModels
        {
            get { return _abilityDetailModels; }
            set
            {
                _abilityDetailModels = value;
                OnPropertyChanged(nameof(AbilityDetailModels));
            }
        }

        public void UpdateBaseProperty(CardModel cardModel)
        {
            Type = cardModel.Type;
            Camp = cardModel.Camp;
            Race = cardModel.Race;
            Sign = cardModel.Sign;
            Rare = cardModel.Rare;
            Pack = cardModel.Pack;
            CName = cardModel.CName;
            JName = cardModel.JName;
            Number = cardModel.Number;
            Illust = cardModel.Illust;
            CostValue = cardModel.Cost.ToString();
            PowerValue = cardModel.Power.ToString();
            Ability = cardModel.Ability;
            Lines = cardModel.Lines;
        }

        public void UpdateAbilityTypeModels(CardModel cardModel)
        {
            for (var i = 0; i != AbilityTypeModels.Count; i++)
            {
                var model = AbilityTypeModels[i];
                AbilityTypeModels[i] = new AbilityModel
                {
                    Checked = cardModel.Ability.Contains(model.Name),
                    Name = model.Name
                };
            }
        }

        public void UpdateAbilityDetailModel(CardModel cardModel)
        {
            var abilityDetailModelList = string.IsNullOrWhiteSpace(cardModel.AbilityDetailJson)
                ? new List<List<int>>()
                : JsonUtils.Deserialize<List<List<int>>>(cardModel.AbilityDetailJson);

            foreach (var pair in abilityDetailModelList)
                for (var i = 0; i != AbilityDetailModels.Count; i++)
                {
                    var model = AbilityDetailModels[i];
                    if (!model.Code.Equals(pair[0])) continue;
                    AbilityDetailModels[i] = new AbilityModel
                    {
                        Checked = pair[1] == 1,
                        Name = model.Name,
                        Code = pair[0]
                    };
                    break;
                }
        }

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

        public void UpdateTypeLinkage()
        {
            switch (Type)
            {
                case StringConst.NotApplicable:
                case StringConst.TypeZx:
                {
                    CostEnabled = true;
                    PowerEnabled = true;
                    RaceEnabled = true;
                    SignEnabled = true;
                    break;
                }
                case StringConst.TypeZxEx:
                {
                    CostEnabled = true;
                    PowerEnabled = true;
                    RaceEnabled = true;
                    SignEnabled = false;
                    Sign = StringConst.Hyphen;
                    break;
                }
                case StringConst.TypePlayer:
                {
                    CostEnabled = false;
                    PowerEnabled = false;
                    RaceEnabled = false;
                    SignEnabled = false;
                    Race = StringConst.Hyphen;
                    Sign = StringConst.Hyphen;
                    CostValue = "-1";
                    PowerValue = "-1";
                    break;
                }
                case StringConst.TypeEvent:
                {
                    CostEnabled = true;
                    PowerEnabled = false;
                    RaceEnabled = false;
                    SignEnabled = true;
                    Race = StringConst.Hyphen;
                    PowerValue = "-1";
                    break;
                }
            }
        }

        public void UpdateAbilityLinkage(string ability)
        {
            if (ability.Contains("降临条件") || ability.Contains("觉醒条件"))
            {
                Type = StringConst.TypeZxEx;
                Sign = StringConst.Hyphen;
            }
            if (ability.Contains("【★】"))
            {
                Type = StringConst.TypeEvent;
                Race = StringConst.Hyphen;
                PowerValue = string.Empty;
            }
            if (ability.Contains("【常】生命恢复") || ability.Contains("【常】虚空使者"))
            {
                Type = StringConst.TypeZx;
                Sign = StringConst.SignIg;
            }
            if (ability.Contains("【常】起始卡"))
            {
                Type = StringConst.TypeZx;
                Sign = StringConst.Hyphen;
            }
        }

        public void UpdatePackLinkage()
        {
            var packNumber = CardUtils.GetPackNumber(Pack);
            if (Number.Contains(StringConst.Hyphen))
                packNumber +=
                    Number.Substring(
                        Number.IndexOf(StringConst.Hyphen, StringComparison.Ordinal) + 1);
            Number = packNumber;
            if (packNumber.IndexOf("P", StringComparison.Ordinal) == 0)
                Rare = StringConst.RarePr;
        }
    }
}