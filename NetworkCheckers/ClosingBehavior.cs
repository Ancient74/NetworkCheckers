using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NetworkCheckers
{
    public class ClosingBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Closing += OnClosing;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Closing -= OnClosing;
        }

        public ICommand ClosingCommand
        {
            get { return (ICommand)GetValue(ClosingCommandProperty); }
            set { SetValue(ClosingCommandProperty, value); }
        }

        public static readonly DependencyProperty ClosingCommandProperty =
            DependencyProperty.Register("ClosingCommand", typeof(ICommand), typeof(ClosingBehavior), new PropertyMetadata(null));


        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ClosingCommand.CanExecute(AssociatedObject))
                ClosingCommand.Execute(AssociatedObject);
        }
    }
}
