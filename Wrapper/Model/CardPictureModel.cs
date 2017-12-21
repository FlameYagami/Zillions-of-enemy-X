using System.Collections.Generic;
using System.Windows;

namespace Wrapper.Model
{
    public class CardPictureModel
    {
        public List<string> NubmerExList { get; set; }
        public List<string> PicturePathList { get; set; }
        public int SelectedIndex { get; set; }
        public List<Visibility> TabItemVisibilityList { get; set; }
    }
}