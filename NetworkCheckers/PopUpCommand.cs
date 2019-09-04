using System;
using System.Windows;
using System.Windows.Input;

namespace NetworkCheckers
{
    public class PopUpCommand : CallbackCommand
    {
        public PopUpCommand(Action action) : base(action)
        {
        }
        public override void Execute(object parameter)
        {
            if (!(parameter is Window window))
                return;

            base.Execute(parameter);

            window.Close();
        }
    }
}