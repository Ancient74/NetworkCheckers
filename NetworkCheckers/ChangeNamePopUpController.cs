using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckers
{
    public class ChangeNamePopUpController : IChangeNamePopUpController
    {
        private string changedName;
        public string PopUp(string originalName)
        {
            ChangeNamePopUp popUp = new ChangeNamePopUp();
            ChangeNamePopUpViewModel vm = new ChangeNamePopUpViewModel(originalName);
            popUp.DataContext = vm;
            vm.NameChanged += OnNameChanged;
            popUp.ShowDialog();
            return changedName;
        }

        private void OnNameChanged(string obj)
        {
            changedName = obj;
        }
    }
}
