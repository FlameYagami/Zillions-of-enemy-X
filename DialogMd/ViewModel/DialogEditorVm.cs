namespace Dialog.ViewModel
{
    public class DialogEditorVm
    {
        public DialogEditorVm(string message, string hint)
        {
            Message = message;
            Hint = hint;
        }

        public string Message { get; set; }
        public string Hint { get; set; }
    }
}