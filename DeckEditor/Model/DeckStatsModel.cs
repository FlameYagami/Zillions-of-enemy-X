using System.Windows.Media;
using Wrapper.Model;

namespace DeckEditor.Model
{
    public class DeckStatsModel : BaseModel
    {
        private string _exCountValue;
        private string _igCountValue;

        private string _lifeCountValue;
        private SolidColorBrush _lifeForeground;
        private string _startCountValue;
        private SolidColorBrush _startForeground;
        private string _ugCountValue;
        private string _voidCountValue;
        private SolidColorBrush _voidForeground;

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
                OnPropertyChanged(nameof(IgCountValue));
            }
        }

        public string ExCountValue
        {
            get { return _exCountValue; }
            set
            {
                _exCountValue = value;
                OnPropertyChanged(nameof(IgCountValue));
            }
        }

        public SolidColorBrush StartForeground
        {
            get { return _startForeground; }
            set
            {
                _startForeground = value;
                OnPropertyChanged(nameof(StartForeground));
            }
        }

        public SolidColorBrush LifeForeground
        {
            get { return _lifeForeground; }
            set
            {
                _lifeForeground = value;
                OnPropertyChanged(nameof(LifeForeground));
            }
        }

        public SolidColorBrush VoidForeground
        {
            get { return _voidForeground; }
            set
            {
                _voidForeground = value;
                OnPropertyChanged(nameof(VoidForeground));
            }
        }
    }
}