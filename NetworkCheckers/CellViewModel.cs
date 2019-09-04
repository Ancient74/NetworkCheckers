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
    public class CellViewModel : NotifiableObject
    {
        private CheckerViewModel checker;
        private bool isHighlighted;
        private readonly ICheckerSelector checkerSelector;
        private ICommand cellClickCommand;
        public CheckerViewModel Checker
        {
            get => checker;
            set
            {
                checker = value;
                OnPropertyChanged("Checker");
                OnPropertyChanged("CheckerVisibility");
            }
        }

        public CellViewModel(bool even, ICheckerSelector checkerSelector)
        {
            IsEven = even;
            this.checkerSelector = checkerSelector;
        }

        public ICommand CellClickCommand
        {
            get
            {
                if (cellClickCommand == null)
                    cellClickCommand = new CallbackCommand(() => checkerSelector.SelectCell(this));
                return cellClickCommand;
            }
        }

        public bool IsHighlighted
        {
            get => isHighlighted;
            set
            {
                if (isHighlighted == value)
                    return;
                isHighlighted = value;
                OnPropertyChanged("IsHighlighted");
            }
        }

        public bool IsEven { get; }
        public Visibility CheckerVisibility => Checker != null ? Visibility.Visible : Visibility.Collapsed;
    }
}
