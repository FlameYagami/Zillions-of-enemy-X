using System;
using System.Windows.Forms;

namespace CardEditor.Utils.Dialog
{
    public partial class DlgOKCANCEL : Form
    {
        public DlgOKCANCEL(string value)
        {
            InitializeComponent();
            lbl_info.Text = value;
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}