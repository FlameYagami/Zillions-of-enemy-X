namespace Dialog.ViewModel
{
    public class DialogEditorVm
    {
        public DialogEditorVm(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
        public string Hint { get; set; }
    }
}