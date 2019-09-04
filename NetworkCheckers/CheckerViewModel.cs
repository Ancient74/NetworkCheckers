using NetworkCheckersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NetworkCheckers
{
    public class CheckerViewModel : NotifiableObject
    {
        public PlayerType PlayerType => Checker.PlayerType;
        private ICommand selectCommand;
        private bool selected;

        public bool IsQueen
        {
            get => Checker.IsQueen;
            set
            {
                if (Checker.IsQueen == value)
                    return;
                Checker.IsQueen = value;
                OnPropertyChanged("IsQueen");
                OnPropertyChanged("CrownVisibility");
            }
        }

        public bool Selected
        {
            get => selected;
            set
            {
                if (selected == value)
                    return;
                selected = value;
                OnPropertyChanged("Selected");
                OnPropertyChanged("SelectedColor");
            }
        }

        public ICommand SelectCommand
        {
            get
            {
                if (selectCommand == null)
                    selectCommand = new CallbackCommand(() => checkerSelector.SelectChecker(this));
                return selectCommand;
            }
        }

        public Visibility CrownVisibility => Checker.IsQueen ? Visibility.Visible : Visibility.Collapsed;
        private readonly bool rotateCrown = false;
        public int CrownRotation => rotateCrown ? 180 : 0;

        private readonly ICheckerSelector checkerSelector;

        public NetworkCheckersLib.Checker Checker { get; }
        public CheckerViewModel(NetworkCheckersLib.Checker checker, bool rotateCrown, ICheckerSelector checkerSelector)
        {
            this.Checker = checker;
            this.checkerSelector = checkerSelector;
            this.rotateCrown = rotateCrown;
        }

        public Color Color2 => PlayerType == PlayerType.White ? Colors.White : Colors.Black;
        public Color Color1 => PlayerType == PlayerType.White ? Colors.LightGray : Colors.DarkGray;

        public Color SelectedColor => Selected ? Colors.Red : Color2;
    }
}
