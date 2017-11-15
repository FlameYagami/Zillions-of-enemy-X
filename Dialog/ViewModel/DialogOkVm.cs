namespace Dialog.ViewModel
{
    public class DialogOkVm
    {
        public DialogOkVm(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}