using System.Windows;
using Wrapper.Model;

namespace CardCrawler.Model
{
    public class PrgModel : BaseModel
    {
        private string _prgHint;
        private Visibility _prgHintVisibility;
        private int _prgValue;
        private Visibility _prgVlaueVisibility;

        public PrgModel()
        {
            PrgValue = 0;
            PrgValueVisibility = Visibility.Hidden;
            PrgHint = string.Empty;
            PrgHintVisibility = Visibility.Hidden;
        }

        public int PrgValue
        {
            get { return _prgValue; }
            set
            {
                _prgValue = value;
                OnPropertyChanged(nameof(PrgValue));
            }
        }

        public string PrgHint
        {
            get { return _prgHint; }
            set
            {
                _prgHint = value;
                OnPropertyChanged(nameof(PrgHint));
            }
        }

        public Visibility PrgValueVisibility
        {
            get { return _prgVlaueVisibility; }
            set
            {
                _prgVlaueVisibility = value;
                OnPropertyChanged(nameof(PrgValueVisibility));
            }
        }

        public Visibility PrgHintVisibility
        {
            get { return _prgHintVisibility; }
            set
            {
                _prgHintVisibility = value;
                OnPropertyChanged(nameof(PrgHintVisibility));
            }
        }

        public void Start()
        {
            PrgHintVisibility = Visibility.Visible;
            PrgValueVisibility = Visibility.Visible;
        }

        public void Finish(int succeed, int failded)
        {
            PrgValue = 100;
            PrgHint = $"成功{succeed}|失败{failded}";
        }

        public void Update(int prgValue, string prgHint)
        {
            PrgValue = prgValue;
            PrgHint = prgHint;
        }
    }
}