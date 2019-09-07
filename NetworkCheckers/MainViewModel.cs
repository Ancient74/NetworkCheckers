using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NetworkCheckersLib;
using NetworkCheckersLib.Network;

namespace NetworkCheckers
{
    public class MainViewModel : NotifiableObject, IStartGame, ICancelLoading, IMessageSender, IIpConfigMenu, IConfigServer
    {
        private GameViewViewModel gameViewViewModel;
        public MainScreenViewModel MainScreenViewModel { get; }
        public LoadingScreenViewModel LoadingScreenViewModel { get; }

        public IpConfigViewModel IpConfigViewModel { get; }

        public GameViewViewModel GameViewViewModel
        {
            get => gameViewViewModel;
            set
            {
                gameViewViewModel = value;
                if (gameViewViewModel == null)
                    return;
                gameViewViewModel.MoveDone += OnMoveDone;
                gameViewViewModel.TurnDone += OnTurnDone;
                OnPropertyChanged("GameViewViewModel");
            }
        }

        private void OnTurnDone()
        {
            lock (locker)
            {
                GameViewViewModel.Mover = GameViewViewModel.Mover == PlayerType.White ? PlayerType.Black : PlayerType.White;
            }
            client.WriteMessage(MessageType.TurnEnd, "");
        }

        private void OnMoveDone(BoardIndex from, BoardIndex to)
        {
            WriteStepsToMessanger(from, to);
        }

        private void WriteStepsToMessanger(BoardIndex from, BoardIndex to)
        {
            client?.WriteMessage(MessageType.Step, from.ToString() + "," + to.ToString());
        }

        public Visibility BackgroundVisibility
        {
            get => SwitchView == 2 ? Visibility.Collapsed : Visibility.Visible;
        }

        private GameClient client;

        private readonly object locker = new object();

        public MainViewModel()
        {
            MainScreenViewModel = new MainScreenViewModel(this, this);
            LoadingScreenViewModel = new LoadingScreenViewModel(this);
            IpConfigViewModel = new IpConfigViewModel(this);
        }
        private int switchView = 0;
        public int SwitchView
        {
            get => switchView;
            set
            {
                if (switchView == value)
                    return;
                switchView = value;
                OnPropertyChanged("SwitchView");
                OnPropertyChanged("BackgroundVisibility");
            }
        }

        private void SubscribeToClient(GameClient client)
        {
            client.Disconnected += OnDisconnected;
            client.Connected += OnConnected;
            client.GameStarted += OnGameStarted;
            client.PlayerTypeRecieved += OnPlayerTypeRecieved;
            client.TurnStarted += OnTurnStarted;
            client.TurnEnded += OnTurnEnded;
            client.MoveRecieved += OnMoveRecieved;
            client.QueenAppeared += OnQueenAppeared;
            client.ResultRecieved += OnResultRecieved;
            client.OpponentName += OnOpponentNameRecieved;
            client.TextMessageRecieved += OnTextMessageRecieved;
        }

        private void OnTextMessageRecieved(string text)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                GameViewViewModel.Messages.Add(new MessageViewModel(text, false));
            });
        }

        private void UnsubscribeToClient(GameClient client)
        {
            client.Disconnected -= OnDisconnected;
            client.Connected -= OnConnected;
            client.GameStarted -= OnGameStarted;
            client.PlayerTypeRecieved -= OnPlayerTypeRecieved;
            client.TurnStarted -= OnTurnStarted;
            client.TurnEnded -= OnTurnEnded;
            client.MoveRecieved -= OnMoveRecieved;
            client.QueenAppeared -= OnQueenAppeared;
            client.ResultRecieved -= OnResultRecieved;
            client.OpponentName -= OnOpponentNameRecieved;
            client.TextMessageRecieved -= OnTextMessageRecieved;
        }

        private void OnOpponentNameRecieved(string obj)
        {
            GameViewViewModel.OpponentName = obj;
        }

        private void OnResultRecieved(PlayerType playerType)
        {
            PopUpController.PopUp((playerType == GameViewViewModel.PlayerType ? ResultType.Win : ResultType.Lose).ToString());
            Reset();
        }

        private void OnQueenAppeared(BoardIndex where)
        {
            GameViewViewModel.GameBoardViewModel[where.Row, where.Col].Checker.IsQueen = true;
        }

        private void OnDisconnected()
        {
            PopUpController.PopUp("Connection error");
            Reset();
        }

        private void Reset()
        {
            if (client != null)
            {
                UnsubscribeToClient(client);
                client.Disconnect();
                client = null;
            }
            SwitchView = 0;
        }

        List<BoardIndex> moves;
        private void OnMoveRecieved(BoardIndex from, BoardIndex to)
        {
            lock(locker)
            {
                moves.Add(from);
                moves.Add(to);
            }
        }

        private void OnTurnEnded()
        {
            lock (locker)
            {
                var start = moves[0];
                bool queenAppear = false;
                void check(BoardIndex index)
                {
                    CheckerViewModel checker = GameViewViewModel.GameBoardViewModel[index.Row, index.Col].Checker;
                    if ((checker.PlayerType == PlayerType.White && index.Col == 0) || (checker.PlayerType == PlayerType.Black && index.Col == GameViewViewModel.GameBoardViewModel.Cols - 1))
                    {
                        if (!checker.IsQueen)
                            queenAppear = true;
                    }
                }
                check(start);
                for (int i = 1; i < moves.Count; i++)
                {
                    GameViewViewModel.MoveAndRemove(start, moves[i]);
                    start = moves[i];
                    check(start);
                }
                if (queenAppear)
                {
                    client.WriteMessage(MessageType.QueenAppeared, moves.Last().ToString());
                }
                if (GameViewViewModel.Winner(out PlayerType winner))
                {
                    client.WriteMessage(MessageType.Result, winner.ToString());
                }
            }
        }

        private void OnTurnStarted(PlayerType playerType)
        {
            moves = new List<BoardIndex>();
            GameViewViewModel.Mover = playerType; 
        }

        private void OnPlayerTypeRecieved(PlayerType obj)
        {
            lock(locker)
            {
                GameViewViewModel = new GameViewViewModel(obj, this)
                {
                    MyName = MainScreenViewModel.UserName
                };
            }
        }

        private void OnGameStarted()
        {
            client.WriteMessage(MessageType.Name, MainScreenViewModel.UserName);
            SwitchView = 2;
        }

        public void CancelLoading()
        {
            Reset();
        }

        public void FindGame()
        {
            SwitchView = 1;
            client = new GameClient(IpConfigViewModel.IpConfig);
            SubscribeToClient(client);
            client.Connect();
        }

        private void OnConnected()
        {
            client.Listen();
            client.WriteMessage(MessageType.StartGame, "");
        }

        public void Send(string message)
        {
            client.WriteMessage(MessageType.TextMessage, message);
        }

        public void Cancel()
        {
            SwitchView = 0;
        }

        public void Configure()
        {
            SwitchView = 3;
        }
    }
}
