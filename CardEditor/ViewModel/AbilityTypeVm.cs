using System.Collections.ObjectModel;
using Wrapper.Model;

namespace CardEditor.ViewModel
{
    public class AbilityTypeVm : BaseModel
    {
        public AbilityTypeVm()
        {
            AbilityTypeModels = new ObservableCollection<AbilityModel>();
        }

        public ObservableCollection<AbilityModel> AbilityTypeModels { get; set; }

        public void UpdateAbilityType(ObservableCollection<AbilityModel> abilityTypeModels)
        {
            AbilityTypeModels = abilityTypeModels;
            OnPropertyChanged(nameof(AbilityTypeModels));
        }
    }
}