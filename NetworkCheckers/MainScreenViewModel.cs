using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckers
{
    public class MainScreenViewModel : NotifiableObject, IChangeName
    {
        public MainMenuViewModel MainMenu { get; private set; }
        private readonly IUserNameProvider userNameProvider;
        private readonly IChangeNamePopUpController popUpController;

        public string UserName
        {
            get
            {
                return userNameProvider.GetName();
            }
            set
            {
                userNameProvider.SaveName(value);
                OnPropertyChanged("UserName");
                OnPropertyChanged("GreetingText");
            }
        }

        public string GreetingText => "Hello: " + UserName;

        public MainScreenViewModel(IStartGame startGame, IConfigServer configServer)
        {
            MainMenu = new MainMenuViewModel(startGame, this, configServer);
            userNameProvider = new CachedUserNameProvider(new FileUserNameProvider("user_name.txt"));
            popUpController = new ChangeNamePopUpController();
        }

        public void ChangeName()
        {
            var res = popUpController.PopUp(UserName);
            if (res != UserName)
                UserName = res;
        }
    }
}
