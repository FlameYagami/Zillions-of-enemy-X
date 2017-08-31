using System.Collections.Generic;
using System.Windows;
using DeckEditor.Model;
using Wrapper;
using Wrapper.Model;
using Wrapper.Utils;

namespace DeckEditor.ViewModel
{
    public class CardPictureVm : BaseModel
    {
        public CardPictureVm()
        {
            CardPictureModel = new CardPictureModel();
        }

        public DelegateCommand CmdPictureAdd { get; set; }
        public CardPictureModel CardPictureModel { get; set; }


        public void UpdatePicture(CardModel cardModel)
        {
            CardPictureModel.SelectedIndex = 0;
            CardPictureModel.TabItemVisibilityList = new List<Visibility>
            {
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible,
                Visibility.Visible
            };
            CardPictureModel.NubmerExList = CardUtils.GetNumberExList(cardModel.ImageJson);
            CardPictureModel.PicturePathList = CardUtils.GetPicturePathList(cardModel.ImageJson);
            for (var i = 0; i != 4; i++)
                if (i < CardPictureModel.PicturePathList.Count)
                    CardPictureModel.TabItemVisibilityList[i] = Visibility.Visible;
                else
                    CardPictureModel.TabItemVisibilityList[i] = Visibility.Hidden;
            if (0 == CardPictureModel.PicturePathList.Count)
                CardPictureModel.TabItemVisibilityList[0] = Visibility.Visible;
            ;
            OnPropertyChanged(nameof(CardPictureModel));
        }
    }
}