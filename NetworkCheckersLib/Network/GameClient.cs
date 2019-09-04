using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.Network
{
    public class GameClient : ClientBase
    {
        public GameClient()
        {
            MessageRecieved += OnMessageRecieved;
        }

        public event Action GameStarted;
        public event Action TurnEnded;
        public event Action<PlayerType> TurnStarted;
        public event Action<PlayerType> PlayerTypeRecieved;
        public event Action<BoardIndex, BoardIndex> MoveRecieved;
        public event Action<BoardIndex> QueenAppeared;
        public event Action<PlayerType> ResultRecieved;
        public event Action<string> OpponentName;
        public event Action<string> TextMessageRecieved;

        private void OnMessageRecieved(object sender, string e)
        {
            string[] typeAndMessage = e.Split(':');
            string type = typeAndMessage[0];
            string message = "";
            MessageType messageType = (MessageType)Enum.Parse(typeof(MessageType), type);
            if (typeAndMessage.Length > 1)
                message = typeAndMessage[1];
            switch (messageType)
            {
                case MessageType.StartGame:
                    {
                        GameStarted?.Invoke();
                        break;
                    }
                case MessageType.PlayerType:
                    {
                        PlayerType playerType = (PlayerType)Enum.Parse(typeof(PlayerType), message);
                        PlayerTypeRecieved?.Invoke(playerType);
                        break;
                    }
                case MessageType.Step:
                    {
                        string[] moves = message.Split(',');
                        BoardIndex from = BoardIndex.Parse(moves[0]);
                        BoardIndex to = BoardIndex.Parse(moves[1]);
                        MoveRecieved?.Invoke(from, to);
                        break;
                    }
                case MessageType.TurnEnd:
                    {
                        TurnEnded?.Invoke();
                        break;
                    }
                case MessageType.StartTurn:
                    {
                        PlayerType playerType = (PlayerType)Enum.Parse(typeof(PlayerType), message);
                        TurnStarted?.Invoke(playerType);
                        break;
                    }
                case MessageType.QueenAppeared:
                    {
                        BoardIndex where = BoardIndex.Parse(message);
                        QueenAppeared?.Invoke(where);
                        break;
                    }
                case MessageType.Result:
                    {
                        PlayerType winner = (PlayerType)Enum.Parse(typeof(PlayerType), message);
                        ResultRecieved?.Invoke(winner);
                        break;
                    }
                case MessageType.Name:
                    {
                        OpponentName?.Invoke(message);
                        break;
                    }
                case MessageType.TextMessage:
                    {
                        TextMessageRecieved?.Invoke(message);
                        break;
                    }
            }
        }
    }
}
