using NetworkCheckersLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckers
{
    public static class PopUpController
    {
        public static void PopUp(string text)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                PopUpViewModel vm = new PopUpViewModel(text);
                PopUp popUp = new PopUp
                {
                    DataContext = vm
                };
                popUp.ShowDialog();
            });
        }
    }
}
