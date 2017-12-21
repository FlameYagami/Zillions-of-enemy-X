using System.Collections.ObjectModel;
using System.Linq;
using Wrapper.Constant;

namespace Wrapper.Model
{
    public class DeQueryModel : BaseModel
    {
        public DeQueryModel()
        {
            Type = StringConst.NotApplicable;
            Camp = StringConst.NotApplicable;
            Race = StringConst.NotApplicable;
            Sign = StringConst.NotApplicable;
            Rare = StringConst.NotApplicable;
            Pack = StringConst.NotApplicable;
            Illust = StringConst.NotApplicable;

            Key = string.Empty;
            CostValue = string.Empty;
            PowerValue = string.Empty;

            InitAbilityTypeModels();
            InitAbilityDetailModels();
        }

        public string Type { get; set; }
        public string Camp { get; set; }
        public string Race { get; set; }
        public string Sign { get; set; }
        public string Rare { get; set; }
        public string Pack { get; set; }
        public string Illust { get; set; }
        public string Key { get; set; }
        public string CostValue { get; set; }
        public string PowerValue { get; set; }
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
            OnPropertyChanged(nameof(AbilityTypeModels));
        }

        private void InitAbilityDetailModels()
        {
            AbilityDetailModels = new ObservableCollection<AbilityModel>();
            Dic.AbilityDetailDic.Keys.ToList().ForEach(key => AbilityDetailModels.Add(new AbilityModel
            {
                Name = key,
                Checked = false
            }));
            OnPropertyChanged(nameof(AbilityDetailModels));
        }
    }
}