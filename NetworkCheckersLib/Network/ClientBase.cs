using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkCheckersLib.Network
{
    public class ClientBase
    {
        protected TcpClient client;

        public event Action<MessageType, string> MessageRecieved;
        public event Action Connected;
        public event Action Disconnected;

        private int Port { get; } = 14447;
        private string HostAddr { get; } = "127.0.0.1";

        private Timer pingTimer;
        private readonly object locker = new object();

        private CancellationTokenSource source = new CancellationTokenSource();

        public ClientBase()
        {
        }
        public async void Connect()
        {
            client = new TcpClient();
            try
            {
                await client.ConnectAsync(HostAddr, Port);
                Connected?.Invoke();
                return;
            }
            catch
            {
                Disconnect();
            }
        }

        public async void Listen()
        {
            source?.Cancel();
            source = new CancellationTokenSource();
            var token = source.Token;
            pingTimer = new Timer(Ping, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.0));
            try
            {
                await Task.Run(() =>
                {
                    using (client)
                    {
                        using (var stream = client.GetStream())
                        {
                            using (var reader = new BinaryReader(stream))
                            {
                                while (true)
                                {
                                    token.ThrowIfCancellationRequested();
                                    if (stream.DataAvailable)
                                    {
                                        MessageRecieved?.Invoke((MessageType)reader.ReadInt32(), reader.ReadString());
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch
            {

            }
            finally
            {
                Disconnect();
            }
        }

        public bool IsConnected => client != null && client.Client != null && client.Client.Connected;

        private void Ping(object state)
        {
            try
            {
                WriteMessage(MessageType.Ping);
            }
            catch
            {
                source.Cancel();
            }
        }

        public void WriteMessage(MessageType messageType, string message = "")
        {
            lock(locker)
            {
                var stream = client.GetStream();
                var writer = new BinaryWriter(stream);
                writer.Write((int)messageType);
                writer.Write(message);
                writer.Flush();
            }
        }

        public void Disconnect()
        {
            pingTimer?.Dispose();
            pingTimer = null;
            client?.Dispose();
            client = null;
            Disconnected?.Invoke();
        }
    }
}
