using System.Windows.Media;
using Common;
using Wrapper.Model;

namespace DeckEditor.Model
{
    public class DeckStatsModel : BaseModel
    {
        private int _lifeCount;

        private SolidColorBrush _lifeForeground;
        private int _startCount;

        private SolidColorBrush _startForeground;

        private int _voidCount;

        private SolidColorBrush _voidForeground;

        public int StartCount
        {
            get { return _startCount; }
            set
            {
                _startCount = value;
                OnPropertyChanged(nameof(StartCount));
            }
        }

        public int LifeCount
        {
            get { return _lifeCount; }
            set
            {
                _lifeCount = value;
                OnPropertyChanged(nameof(LifeCount));
            }
        }

        public int VoidCount
        {
            get { return _voidCount; }
            set
            {
                _voidCount = value;
                OnPropertyChanged(nameof(VoidCount));
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