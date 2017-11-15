using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper.Model
{
    public class CheckModel<T> : BaseModel
    {
        private bool _checked;

        public CheckModel(T model)
        {
            Checked = false;
            Model = model;
        }

        public T Model { get; set; }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }
    }
}
