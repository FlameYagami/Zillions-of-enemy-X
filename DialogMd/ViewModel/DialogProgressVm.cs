namespace Dialog.ViewModel
{
    public class DialogProgressVm
    {
        public DialogProgressVm(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}