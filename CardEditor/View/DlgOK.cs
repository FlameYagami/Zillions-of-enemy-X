using System;
using System.Windows.Forms;

namespace CardEditor.Utils.Dialog
{
    public partial class DlgOK : Form
    {
        public DlgOK(string value)
        {
            InitializeComponent();
            label.Text = value;
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}