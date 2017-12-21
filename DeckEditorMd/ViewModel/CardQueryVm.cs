using Common;
using DeckEditor.View;
using MaterialDesignThemes.Wpf;
using Wrapper.Constant;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class CardQueryVm : BaseModel
    {
        private readonly CardPreviewVm _cardPreviewVm;

        public CardQueryVm(CardPreviewVm cardPreviewVm)
        {
            _cardPreviewVm = cardPreviewVm;

            CmdQuery = new DelegateCommand {ExecuteCommand = Query_Click};
            CmdReset = new DelegateCommand {ExecuteCommand = Reset_Click};
            CmdAlilityDetail = new DelegateCommand {ExecuteCommand = AlilityDetail_Click};

            CardQueryModel = new DeQueryModel();
            SearchSourceModel = new QuerySourceModel();
        }

        public DeQueryModel CardQueryModel { get; set; }
        public QuerySourceModel SearchSourceModel { get; set; }

        public DelegateCommand CmdQuery { get; set; }
        public DelegateCommand CmdReset { get; set; }
        public DelegateCommand CmdAlilityDetail { get; set; }

        /// <summary>
        ///     详细能力查询事件
        /// </summary>
        public async void AlilityDetail_Click(object obj)
        {
            await DialogHost.Show(new AbilityDetailDialog(CardQueryModel.AbilityDetailModels),
                (sender, eventArgs) => { }, (sender, eventArgs) => { });
        }

        /// <summary>
        ///     查询条件重置事件
        /// </summary>
        public void Reset_Click(object obj)
        {
            CardQueryModel = new DeQueryModel();
            OnPropertyChanged(nameof(CardQueryModel));
        }

        /// <summary>
        ///     查询事件
        /// </summary>
        public void Query_Click(object obj)
        {
            OnPropertyChanged(nameof(CardQueryModel));
            _cardPreviewVm.UpdateCardPreviewModels(CardQueryModel);
        }

        /// <summary>
        ///     阵营、种族联动事件
        /// </summary>
        public void UpdateRaceList()
        {
            OnPropertyChanged(nameof(CardQueryModel));
            SearchSourceModel.UpdateRaceList(CardQueryModel.Camp);
            CardQueryModel.Race = StringConst.NotApplicable;
            OnPropertyChanged(nameof(CardQueryModel));
        }
    }
}