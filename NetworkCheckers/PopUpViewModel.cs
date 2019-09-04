using NetworkCheckersLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkCheckers
{
    public class PopUpViewModel
    {
        public string Text { get; }
        public PopUpViewModel(string text)
        {
            Text = text;
        }

        public ICommand OkCommand { get; } = new PopUpCommand(Ok);

        private static void Ok()
        {
        }
    }
}
