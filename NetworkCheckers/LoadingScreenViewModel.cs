using NetworkCheckersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckers
{
    public class LoadingScreenViewModel : NotifiableObject
    {
        private readonly ICancelLoading cancelLoading;

        public LoadingScreenViewModel(ICancelLoading cancelLoading)
        {
            CancelCommand = new CallbackCommand(Cancel);
            this.cancelLoading = cancelLoading;
        }

        private void Cancel()
        {
            cancelLoading.CancelLoading();
        }

        public CallbackCommand CancelCommand { get; }
    }
}
