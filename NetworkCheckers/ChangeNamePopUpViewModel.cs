using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkCheckers
{
    public class ChangeNamePopUpViewModel : NotifiableObject
    {
        private readonly string originalName;
        private string editedName;
        public ChangeNamePopUpViewModel(string originalName)
        {
            this.originalName = originalName;
            OkCommand = new PopUpCommand(Ok);
            CancelCommand = new PopUpCommand(Cancel);
            ClosingCommand = new CallbackCommand(Cancel);
        }

        public string EditedName
        {
            get => editedName;
            set
            {
                editedName = value;
                OnPropertyChanged("EditedName");
            }
        }

        public event Action<string> NameChanged;

        public CallbackCommand OkCommand { get; }
        public CallbackCommand CancelCommand { get; }

        public CallbackCommand ClosingCommand { get; }

        private void Ok()
        {
            NameChanged?.Invoke(EditedName);
            DisableCommands();
        }

        private void Cancel()
        {
            NameChanged?.Invoke(originalName);
            DisableCommands();
        }

        private void DisableCommands()
        {
            OkCommand.SetCanExecute(false);
            CancelCommand.SetCanExecute(false);
            ClosingCommand.SetCanExecute(false);
        }

        public string ChangeNameText => "Current name is " + originalName;
    }
}
