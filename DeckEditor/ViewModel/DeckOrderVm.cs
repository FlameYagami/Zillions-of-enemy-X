using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DeckEditor.Utils;
using Wrapper;
using Wrapper.Annotations;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class DeckOrderVm
    {
        private readonly DeckVm _deckVm;

        public DeckOrderVm(DeckVm deckVm)
        {
            _deckVm = deckVm;

            CmdValueOrder = new DelegateCommand {ExecuteCommand = ValueOrder_Click};
            CmdRandomOrder = new DelegateCommand {ExecuteCommand = RandomOrder_Click};
            CmdDeckStats = new DelegateCommand {ExecuteCommand = DeckStats_Click};
        }

        public DelegateCommand CmdValueOrder { get; set; }
        public DelegateCommand CmdRandomOrder { get; set; }
        public DelegateCommand CmdDeckStats { get; set; }

        public void DeckStats_Click(object obj)
        {
            var dekcStatisticalDic = new Dictionary<int, int>();
            var costIgList = _deckVm.IgModels.Select(deckEntity => deckEntity.Cost);
            var costUgList = _deckVm.UgModels.Select(deckEntity => deckEntity.Cost);
            var costDeckList = new List<int>();
            costDeckList.AddRange(costIgList);
            costDeckList.AddRange(costUgList);
            var costMax = costDeckList.Max();
            for (var i = 0; i != costMax + 1; i++)
                dekcStatisticalDic.Add(i + 1, costDeckList.Count(cost => cost.Equals(i + 1)));
            DialogUtils.ShowDekcStatistical(dekcStatisticalDic);
        }

        public void RandomOrder_Click(object obj)
        {
            Random(_deckVm.IgModels);
            Random(_deckVm.UgModels);
            Random(_deckVm.ExModels);
        }

        public void ValueOrder_Click(object obj)
        {
            Value(_deckVm.IgModels);
            Value(_deckVm.UgModels);
            Value(_deckVm.ExModels);
        }

        private static void Value([NotNull] ObservableCollection<DeckModel> deckModelList)
        {
            if (deckModelList == null) throw new ArgumentNullException(nameof(deckModelList));
            var deckModels = deckModelList
                .OrderBy(tempDeckEntity => tempDeckEntity.Camp)
                .ThenByDescending(tempDeckEntity => tempDeckEntity.Cost)
                .ThenByDescending(tempDeckEntity => tempDeckEntity.Power)
                .ThenBy(tempDeckEntity => tempDeckEntity.NumberEx)
                .ToList();
            deckModelList.Clear();
            deckModels.ForEach(deckModelList.Add);
        }

        private static void Random([NotNull] ObservableCollection<DeckModel> deckModelList)
        {
            if (deckModelList == null) throw new ArgumentNullException(nameof(deckModelList));
            var deckModels = new List<DeckModel>();
            var random = new Random();
            deckModelList.Select(model => model).ToList().ForEach(
                deckEntity => deckModels.Insert(random.Next(deckModels.Count + 1), deckEntity));
            deckModelList.Clear();
            deckModels.ForEach(deckModelList.Add);
        }
    }
}