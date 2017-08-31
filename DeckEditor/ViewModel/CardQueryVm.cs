using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeckEditor.Model;
using DeckEditor.Utils;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

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

            PackList = CardUtils.GetPackList();
            IllustList = CardUtils.GetIllustList();
            RaceList = new ObservableCollection<string>();
            CardUtils.GetPartRace(StringConst.NotApplicable).ForEach(RaceList.Add);

            CardQueryModel = new CardQueryModel();
        }

        public CardQueryModel CardQueryModel { get; set; }
        public DelegateCommand CmdQuery { get; set; }
        public DelegateCommand CmdReset { get; set; }
        public DelegateCommand CmdAlilityDetail { get; set; }
        public List<string> PackList { get; set; }
        public List<string> IllustList { get; set; }
        public ObservableCollection<string> RaceList { get; set; }

        /// <summary>
        ///     详细能力查询事件
        /// </summary>
        public void AlilityDetail_Click(object obj)
        {
            DialogUtils.ShowAbilityDetail(CardQueryModel.AbilityDetailModels);
        }

        /// <summary>
        ///     查询条件重置事件
        /// </summary>
        public void Reset_Click(object obj)
        {
            CardQueryModel = new CardQueryModel();
            OnPropertyChanged(nameof(CardQueryModel));
        }

        /// <summary>
        ///     查询事件
        /// </summary>
        public void Query_Click(object obj)
        {
            OnPropertyChanged(nameof(CardQueryModel));
            _cardPreviewVm.UpdateCardPreviewList(CardQueryModel);
        }

        /// <summary>
        ///     阵营、种族联动事件
        /// </summary>
        public void UpdateRaceList()
        {
            OnPropertyChanged(nameof(CardQueryModel));
            RaceList.Clear();
            CardUtils.GetPartRace(CardQueryModel.Camp).ForEach(RaceList.Add);
            OnPropertyChanged(nameof(RaceList));
            CardQueryModel.Race = StringConst.NotApplicable;
            OnPropertyChanged(nameof(CardQueryModel));
        }
    }
}