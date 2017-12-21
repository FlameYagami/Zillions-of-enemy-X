using Wrapper.Model;

namespace DeckEditor.Model
{
    public class DeckStatsModel : BaseModel
    {
        private string _exCountValue;
        private string _igCountValue;
        private string _lifeCountValue;
        private string _startCountValue;
        private string _ugCountValue;
        private string _voidCountValue;

//        private SolidColorBrush _lifeForeground;
//        private SolidColorBrush _startForeground;
//        private SolidColorBrush _voidForeground;

        public DeckStatsModel()
        {
            StartCountValue = "0";
            LifeCountValue = "0";
            VoidCountValue = "0";
            IgCountValue = "0";
            UgCountValue = "0";
            ExCountValue = "0";
        }

        public string StartCountValue
        {
            get { return _startCountValue; }
            set
            {
                _startCountValue = value;
                OnPropertyChanged(nameof(StartCountValue));
            }
        }

        public string LifeCountValue
        {
            get { return _lifeCountValue; }
            set
            {
                _lifeCountValue = value;
                OnPropertyChanged(nameof(LifeCountValue));
            }
        }

        public string VoidCountValue
        {
            get { return _voidCountValue; }
            set
            {
                _voidCountValue = value;
                OnPropertyChanged(nameof(VoidCountValue));
            }
        }

        public string IgCountValue
        {
            get { return _igCountValue; }
            set
            {
                _igCountValue = value;
                OnPropertyChanged(nameof(IgCountValue));
            }
        }

        public string UgCountValue
        {
            get { return _ugCountValue; }
            set
            {
                _ugCountValue = value;
                OnPropertyChanged(nameof(UgCountValue));
            }
        }

        public string ExCountValue
        {
            get { return _exCountValue; }
            set
            {
                _exCountValue = value;
                OnPropertyChanged(nameof(ExCountValue));
            }
        }

//            get { return _lifeForeground; }
//        {
//        public SolidColorBrush LifeForeground
//
//        }
//            }
//                OnPropertyChanged(nameof(StartForeground));
//                _startForeground = value;
//            {
//            set
//            get { return _startForeground; }
//        {

//        public SolidColorBrush StartForeground
//            set
//            {
//                _lifeForeground = value;
//                OnPropertyChanged(nameof(LifeForeground));
//            }
//        }
//
//        public SolidColorBrush VoidForeground
//        {
//            get { return _voidForeground; }
//            set
//            {
//                _voidForeground = value;
//                OnPropertyChanged(nameof(VoidForeground));
//            }
//        }
    }
}