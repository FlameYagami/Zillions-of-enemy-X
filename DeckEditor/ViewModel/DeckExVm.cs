using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using Wrapper.Constant;
using Wrapper.Model;

namespace DeckEditor.ViewModel
{
    public class DeckExVm
    {
        public DeckExVm()
        {
            _deckView = new DeckVm();
            IgExModels = new ObservableCollection<DeckExModel>();
            UgExModels = new ObservableCollection<DeckExModel>();
            ExExModels = new ObservableCollection<DeckExModel>();
        }

        private readonly DeckVm _deckView;

        public  DeckVm GetDeckVm()
        {
            return _deckView;
        }

        /// <summary>点燃数据缓存</summary>
        public ObservableCollection<DeckExModel> IgExModels { get; set; }

        /// <summary>非点燃数据缓存</summary>
        public ObservableCollection<DeckExModel> UgExModels { get; set; }

        /// <summary>额外数据缓存</summary>
        public ObservableCollection<DeckExModel> ExExModels { get; set; }

        public void UpdateDeckExModels(Enums.AreaType areaType)
        {
            switch (areaType)
            {
                case Enums.AreaType.Ig:
                    UpdateDeckExModels(_deckView.IgModels, IgExModels);
                    break;
                case Enums.AreaType.Ug:
                    UpdateDeckExModels(_deckView.UgModels, UgExModels);
                    break;
                case Enums.AreaType.Ex:
                    UpdateDeckExModels(_deckView.ExModels, ExExModels);
                    break;
            }
        }

        protected virtual void UpdateDeckExModels(ObservableCollection<DeckModel> deckList, ObservableCollection<DeckExModel> deckExList)
        {
            // 对象去重
            var numberExList = deckList.Select(deck => deck.NumberEx).Distinct().ToList();
            var tempDeckList = new List<DeckModel>();
            tempDeckList.AddRange(numberExList.Select(numberEx => deckList.First(deck=> deck.NumberEx.Equals(numberEx))).ToList());
            // 重新生成对象
            var tempdDeckExList = new List<DeckExModel>();
            tempdDeckExList.AddRange(tempDeckList
                    .Select(deck => new DeckExModel()
                    {
                        DeckModel = deck,
                        Count = deckList.Count(temp => temp.NumberEx.Equals(deck.NumberEx))
                    })
                    .ToList());
            deckExList.Clear();
            tempdDeckExList.ForEach(deckExList.Add);
        }

        public void Clear()
        {
            IgExModels.Clear();
            UgExModels.Clear();
            ExExModels.Clear();
        }
    }
}