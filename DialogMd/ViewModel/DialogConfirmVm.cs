namespace Dialog.ViewModel
{
    public class DialogConfirmVm
    {
        public DialogConfirmVm(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}