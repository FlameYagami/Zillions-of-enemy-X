namespace Dialog.ViewModel
{
    public class DialogConfirmVm
    {
        public DialogConfirmVm(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
    }
}