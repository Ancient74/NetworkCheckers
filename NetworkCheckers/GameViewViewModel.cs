using NetworkCheckersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using static NetworkCheckersLib.CheckerBoard;

namespace NetworkCheckers
{
    public class GameViewViewModel : NotifiableObject
    {
        private PlayerType mover; // current mover

        public PlayerType PlayerType { get; }

        private MoveBuilder moveBuilder;

        public int BoardRotation { get; }
        public GameBoardViewModel GameBoardViewModel { get; }

        public int[] Numbers { get; } = new int[8];
        public char[] Letters { get; } = new char[8];

        public ObservableCollection<MessageViewModel> Messages { get; } = new ObservableCollection<MessageViewModel>();
        private string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        public ICommand SendMessageCommand { get; }

        public PlayerType Mover
        {
            get => mover;
            set
            {
                mover = value;
                MoverChanged?.Invoke();
                OnPropertyChanged("Mover");
                OnPropertyChanged("MoverColor");
            }
        }

        public Color MoverColor => mover == PlayerType.White ? Colors.White : Colors.Black;
        public Color PlayerColor => PlayerType == PlayerType.White ? Colors.White : Colors.Black;

        private string myName = "";
        private string opponentName = "";
        public string MyName
        {
            get => myName;
            set
            {
                myName = value;
                OnPropertyChanged("MyName");
            }
        }
        public string OpponentName
        {
            get => opponentName;
            set
            {
                opponentName = value;
                OnPropertyChanged("OpponentName");
            }
        }

        public IMessageSender MessageSender { get; }

        public bool Winner(out PlayerType playerType)
        {
            playerType = PlayerType.White;
            bool black = false;
            bool white = false;
            for (int i = 0; i < GameBoardViewModel.Rows; i++)
            {
                for (int j = 0; j < GameBoardViewModel.Cols; j++)
                {
                    var checker = GameBoardViewModel[i, j].Checker;
                    if (checker == null)
                        continue;

                    if (checker.PlayerType == PlayerType.White)
                        white = true;
                    if (checker.PlayerType == PlayerType.Black)
                        black = true;
                    if (black && white)
                        return false;
                }
            }
            if (white)
            {
                playerType = PlayerType.White;
                return true;
            }
            else if (black)
            {
                playerType = PlayerType.Black;
                return true;
            }
            return false;
        }

        public event Action MoverChanged;

        public GameViewViewModel(PlayerType playerType, IMessageSender messageSender)
        {
            this.MessageSender = messageSender;
            SendMessageCommand = new CallbackCommand(() =>
            {
                messageSender.Send(Message);
                Messages.Add(new MessageViewModel(Message, true));
                Message = "";
            });
            this.PlayerType = playerType;
            string letters = "abcdefgh";
            if (playerType == PlayerType.Black)
                BoardRotation = 180;
            if(playerType == PlayerType.White)
            {
                for (int i = 0; i < Numbers.Length; i++)
                {
                    Numbers[i] = Numbers.Length - i;
                    Letters[i] = letters[i];
                }
            }
            else
            {
                for (int i = 0; i < Numbers.Length; i++)
                {
                    Numbers[i] = i + 1;
                    Letters[i] = letters[Numbers.Length - i - 1];
                }
            }
            Messages.CollectionChanged += OnMessagesChanged;
            GameBoardViewModel = new GameBoardViewModel(playerType);
            GameBoardViewModel.ChekerSelected += OnCheckerSelected;
            GameBoardViewModel.CellSelected += OnCellSelected;
            GameBoardViewModel.Init();
            MoverChanged += OnMoverChanged;
            MoveDone += OnMoveDone;
            Mover = PlayerType.White;
            OnPropertyChanged("GameBoardViewModel");
            OnPropertyChanged("Numbers");
        }

        private void OnMessagesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Messages");
        }

        private void OnMoveDone(BoardIndex from, BoardIndex to)
        {
            MoveAndRemove(from, to);
            GameBoardViewModel.SelectedChecker = null;
            GameBoardViewModel.UnHighlightAll();
        }

        public void MoveAndRemove(BoardIndex from, BoardIndex to)
        {
            int diffX = to.Row - from.Row;
            int diffY = to.Col - from.Col;
            int dirX = Math.Sign(diffX);
            int dirY = Math.Sign(diffY);
            int x = from.Row + dirX;
            int y = from.Col + dirY;
            while (x != to.Row && y != to.Col)
            {
                if (this.GameBoardViewModel[x, y].Checker != null)
                {
                    this.GameBoardViewModel.Remove(new BoardIndex(x, y));
                    break;
                }
                x += dirX;
                y += dirY;
            }
            GameBoardViewModel.Move(from, to);
        }

        private void OnCellSelected(BoardIndex arg1, CellViewModel arg2)
        {
            if (moveBuilder == null || !moveBuilder.IsStarted)
                return;

            if (!moveBuilder.Move(arg1))
                return;
            var lastMoves = moveBuilder.Build();
            var possiblePositions = moveBuilder.GetPossibleMoves(arg1);
            MoveDone?.Invoke(lastMoves[lastMoves.Count - 2], lastMoves.Last());
            if (possiblePositions.Count != 0)
            {
                OnCheckerSelected(arg1, GameBoardViewModel[arg1.Row, arg1.Col].Checker);
            }
            else
            {
                TurnDone?.Invoke();
            }
        }

        public event Action<BoardIndex, BoardIndex> MoveDone;
        public event Action TurnDone;

        private void OnMoverChanged()
        {
            GameBoardViewModel.SelectedChecker = null;
            moveBuilder = GameBoardViewModel.GetMoveBuilder();
        }

        private void OnCheckerSelected(BoardIndex arg1, CheckerViewModel arg2)
        {
            GameBoardViewModel.UnHighlightAll();

            if (mover != PlayerType)
                return;
            if (arg2 != null && arg2.PlayerType == mover)
                GameBoardViewModel.SelectedChecker = arg2;

            if (GameBoardViewModel.SelectedChecker == null)
                return;

            moveBuilder.Start(arg1);
            var possibleMoves = moveBuilder.GetPossibleMoves(arg1);

            GameBoardViewModel.HighlightCells(possibleMoves);
        }

    }
}
