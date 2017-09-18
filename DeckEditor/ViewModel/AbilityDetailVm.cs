using System.Collections.ObjectModel;
using System.Windows;
using DeckEditor.View;
using Wrapper;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class AbilityDetailVm : BaseModel
    {
        private readonly AbilityDetailWindow _abilityDetailWindow;
        public DelegateCommand CmdOk { get; set; }
        public DelegateCommand CmdCancel { get; set; }

        public AbilityDetailVm(AbilityDetailWindow abilityDetailWindow,ObservableCollection<AbilityModel> abilityDetailModels)
        {
            _abilityDetailWindow = abilityDetailWindow;
            AbilityDetailModels = abilityDetailModels;
            CmdOk = new DelegateCommand {ExecuteCommand = BtnOK_Click };
            CmdCancel = new DelegateCommand { ExecuteCommand = BtnCancel_Click };
        }

        public ObservableCollection<AbilityModel> AbilityDetailModels { get; set; }

        private void BtnCancel_Click(object sender)
        {
            _abilityDetailWindow.Close();
        }

        private void BtnOK_Click(object sender)
        {
            OnPropertyChanged(nameof(AbilityDetailModels));
            _abilityDetailWindow.Close();
        }
    }
}