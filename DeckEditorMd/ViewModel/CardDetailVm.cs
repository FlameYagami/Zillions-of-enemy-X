using System.Linq;
using Common;
using DeckEditor.Model;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace DeckEditor.ViewModel
{
    public class CardDetailVm : BaseModel
    {
        private readonly CardPictureVm _cardPictureVm;

        public CardDetailVm(CardPictureVm cardPictureVm)
        {
            CardDetailModel = new CardDetailModel();
            _cardPictureVm = cardPictureVm;
        }

        public CardDetailModel CardDetailModel { get; set; }

        public void UpdateCardModel(string number)
        {
            var cardModel = CardUtils.GetCardModel(number);
            CardDetailModel.CName = cardModel.CName;
            CardDetailModel.Number = cardModel.Number;
            CardDetailModel.Type = cardModel.Type;
            CardDetailModel.RarePath = Dic.ImgRarePathDic.FirstOrDefault(pair => pair.Key.Equals(cardModel.Rare)).Value;
            CardDetailModel.PowerValue = cardModel.Power.Equals(-1) ? StringConst.Hyphen : cardModel.Power.ToString();
            CardDetailModel.CostValue = cardModel.Cost.Equals(-1) ? StringConst.Hyphen : cardModel.Cost.ToString();
            CardDetailModel.Race = cardModel.Race;
            CardDetailModel.Ability = cardModel.Ability;
            CardDetailModel.JName = cardModel.JName;
            CardDetailModel.Pack = cardModel.Pack;
            CardDetailModel.Illust = cardModel.Illust;
            CardDetailModel.Lines = cardModel.Lines;
            CardDetailModel.SignPath = CardUtils.GetSignPath(cardModel.Sign);
            CardDetailModel.CampPathList = CardUtils.GetCampPathList(cardModel.Camp);
            OnPropertyChanged(nameof(CardDetailModel));
            _cardPictureVm.UpdatePicture(cardModel);
        }
    }
}