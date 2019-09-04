using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkCheckers
{
    public class MainMenuViewModel
    {
        private ICommand changeNameCommand;
        private ICommand findGameCommand;

        private readonly IStartGame startGame;
        private readonly IChangeName changeName;

        public MainMenuViewModel(IStartGame startGame, IChangeName changeName)
        {
            this.changeName = changeName;
            this.startGame = startGame;
        }

        public ICommand FindGameCommand
        {
            get
            {
                if (findGameCommand == null)
                    findGameCommand = new CallbackCommand(() => startGame.FindGame());
                return findGameCommand;
            }
        }

        public ICommand ChangeNameCommand
        {
            get
            {
                if (changeNameCommand == null)
                    changeNameCommand = new CallbackCommand(() => changeName.ChangeName());
                return changeNameCommand;
            }
        }

    }
}
