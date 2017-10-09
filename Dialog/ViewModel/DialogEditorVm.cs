namespace Dialog.ViewModel
{
    public class DialogEditorVm
    {
        public string Content { get; set; }
        public string Hint { get; set; }

        public DialogEditorVm(string content)
        {
            Content = content;
        }
    }
}
