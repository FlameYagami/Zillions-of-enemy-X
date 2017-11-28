using Common;
using DeckEditor.Model;
using DeckEditor.View;
using MaterialDesignThemes.Wpf;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class CardSearchVm : BaseModel
    {
        private readonly CardPreviewVm _cardPreviewVm;

        public CardSearchVm(CardPreviewVm cardPreviewVm)
        {
            _cardPreviewVm = cardPreviewVm;

            CmdQuery = new DelegateCommand {ExecuteCommand = Query_Click};
            CmdReset = new DelegateCommand {ExecuteCommand = Reset_Click};
            CmdAlilityDetail = new DelegateCommand {ExecuteCommand = AlilityDetail_Click};

            SearchModel = new DeSearchModel();
            SearchSourceModel = new SearchSourceModel();
        }

        public DeSearchModel SearchModel { get; set; }
        public SearchSourceModel SearchSourceModel { get; set; }

        public DelegateCommand CmdQuery { get; set; }
        public DelegateCommand CmdReset { get; set; }
        public DelegateCommand CmdAlilityDetail { get; set; }

        /// <summary>
        ///     详细能力查询事件
        /// </summary>
        public async void AlilityDetail_Click(object obj)
        {
            await DialogHost.Show(new AbilityDetailDialog(SearchModel.AbilityDetailModels),
                (sender, eventArgs) => { }, (sender, eventArgs) => { });
        }

        /// <summary>
        ///     查询条件重置事件
        /// </summary>
        public void Reset_Click(object obj)
        {
            SearchModel = new DeSearchModel();
            OnPropertyChanged(nameof(SearchModel));
        }

        /// <summary>
        ///     查询事件
        /// </summary>
        public void Query_Click(object obj)
        {
            OnPropertyChanged(nameof(SearchModel));
            _cardPreviewVm.UpdateCardPreviewModels(SearchModel);
        }

        /// <summary>
        ///     阵营、种族联动事件
        /// </summary>
        public void UpdateRaceList()
        {
            OnPropertyChanged(nameof(SearchModel));
            SearchSourceModel.UpdateRaceList(SearchModel.Camp);
            SearchModel.Race = StringConst.NotApplicable;
            OnPropertyChanged(nameof(SearchModel));
        }
    }
}