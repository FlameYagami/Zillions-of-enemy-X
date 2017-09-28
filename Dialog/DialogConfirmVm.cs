using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dialog
{
    public class DialogConfirmVm
    {
        public string Content { get; set; }

        public DialogConfirmVm(string content)
        {
            Content = content;
        }
    }
}
