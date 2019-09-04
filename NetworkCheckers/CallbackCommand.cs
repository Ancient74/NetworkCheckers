using System;
using System.Windows.Input;

namespace NetworkCheckers
{
    public class CallbackCommand : ICommand
    {
        private readonly Action action;
        private bool canExecute = true;

        public CallbackCommand(Action action)
        {
            this.action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public void SetCanExecute(bool param)
        {
            if (canExecute == param)
                return;

            canExecute = param;
            CanExecuteChanged?.Invoke(this, null);
        }

        public virtual void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;
            action?.Invoke();
        }
    }
}