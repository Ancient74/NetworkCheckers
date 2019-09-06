using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.Network
{
    public class GameHost : HostBase
    {
        private bool p1Ready;
        private bool p2Ready;
        private PlayerType playerTypeCurrent;

        public event Action GameStarted;
        public GameHost()
        {
            MessageRecieved += OnMessageRecieved;
            GameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            Random r = new Random();
            int num = r.Next(2);
            PlayerType player1 = num == 1 ? PlayerType.White : PlayerType.Black;
            PlayerType player2 = num == 1 ? PlayerType.Black : PlayerType.White;
            WriteMessage(clients[0].client, MessageType.PlayerType, player1.ToString());
            WriteMessage(clients[1].client, MessageType.PlayerType, player2.ToString());
            WriteMessage(MessageType.StartGame, "");
            WriteMessage(MessageType.StartTurn, PlayerType.White.ToString());
        }

        private void OnMessageRecieved(TcpClient sender, MessageType messageType, string message)
        {
            TcpClient opposite = null;
            if (clients.Count == 2)
            {
                opposite = clients[0].client == sender ? clients[1].client : clients[0].client;
            }
            switch (messageType)
            {
                case MessageType.Step:
                    {
                        WriteMessage(opposite, MessageType.Step, message);
                        break;
                    }
                case MessageType.QueenAppeared:
                    {
                        WriteMessage(MessageType.QueenAppeared, message);
                        break;
                    }
                case MessageType.TurnEnd:
                    {
                        playerTypeCurrent = playerTypeCurrent == PlayerType.White ? PlayerType.Black : PlayerType.White;
                        WriteMessage(opposite, MessageType.TurnEnd);
                        WriteMessage(MessageType.StartTurn, playerTypeCurrent.ToString());
                        break;
                    }
                case MessageType.StartGame:
                    {
                        if (clients.Count == 1)
                        {
                            p1Ready = true;
                            p2Ready = false;
                        }
                        else if (clients.Count == 2)
                            p2Ready = true;
                        if (p1Ready && p2Ready)
                            GameStarted?.Invoke();
                        break;
                    }
                case MessageType.Result:
                    {
                        WriteMessage(MessageType.Result, message);
                        Reset();
                        Log.Info("Game finished...");
                        Log.Info($"Winner is: {message}");
                        break;
                    }
                case MessageType.Name:
                    {
                        WriteMessage(opposite, MessageType.Name, message);
                        break;
                    }
                case MessageType.TextMessage:
                    {
                        WriteMessage(opposite, MessageType.TextMessage, message);
                        break;
                    }
            }
        }
    }
}
