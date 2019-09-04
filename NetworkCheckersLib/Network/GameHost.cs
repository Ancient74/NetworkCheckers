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

        private void WriteMessage(TcpClient client, MessageType messageType, string message = "")
        {
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine(messageType.ToString() + ":" + message);
            writer.Flush();
        }

        private void WriteMessage(MessageType messageType, string message = "")
        {
            WriteMessage(clients[0], messageType, message);
            WriteMessage(clients[1], messageType, message);
        }

        private void OnGameStarted()
        {
            Random r = new Random();
            int num = r.Next(2);
            PlayerType player1 = num == 1 ? PlayerType.White : PlayerType.Black;
            PlayerType player2 = num == 1 ? PlayerType.Black : PlayerType.White;
            WriteMessage(clients[0], MessageType.PlayerType, player1.ToString());
            WriteMessage(clients[1], MessageType.PlayerType, player2.ToString());
            WriteMessage(MessageType.StartGame, "");
            WriteMessage(MessageType.StartTurn, PlayerType.White.ToString());
        }

        private void OnMessageRecieved(TcpClient sender, string e)
        {
            TcpClient opposite = clients[0] == sender ? clients[1] : clients[0];
            string[] typeAndMessage = e.Split(':');
            string type = typeAndMessage[0];
            string message = "";
            MessageType messageType = (MessageType)Enum.Parse(typeof(MessageType), type);
            if (typeAndMessage.Length > 1)
                message = typeAndMessage[1];
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
                        if (clients[0] == sender)
                            p1Ready = true;
                        else if (clients[1] == sender)
                            p2Ready = true;
                        if (p1Ready && p2Ready)
                            GameStarted?.Invoke();
                        break;
                    }
                case MessageType.Result:
                    {
                        WriteMessage(MessageType.Result, message);
                        clients[0].Close();
                        clients[0].Close();
                        clients = null;
                        Console.WriteLine("Game finished...");
                        Console.WriteLine($"Winner is: {message}");
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
