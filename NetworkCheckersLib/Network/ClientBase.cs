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
    public class ClientBase : IClient
    {
        protected TcpClient client;

        public event EventHandler<string> MessageRecieved;
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
                Disconnected?.Invoke();
            }
        }

        public async void Listen()
        {
            source = new CancellationTokenSource();
            try
            {
                await Task.Run(() =>
                {
                    using (client)
                    {
                        using (var stream = client.GetStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                using (var writer = new StreamWriter(stream))
                                {
                                    if (pingTimer != null)
                                        pingTimer.Dispose();
                                    pingTimer = new Timer((object state) =>
                                    {
                                        try
                                        {
                                            source.Token.ThrowIfCancellationRequested();
                                            lock (locker)
                                            {
                                                writer.WriteLine(MessageType.Ping.ToString());
                                                writer.Flush();
                                            }
                                        }
                                        catch
                                        {
                                            pingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                                            Disconnect();
                                        }
                                    }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.0));
                                    while (true)
                                    {
                                        try
                                        {
                                            source.Token.ThrowIfCancellationRequested();
                                            if (stream.DataAvailable)
                                            {
                                                lock (locker)
                                                {
                                                    MessageRecieved?.Invoke(this, reader.ReadLine());
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            Disconnect();
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }, source.Token);
            }
            catch (TaskCanceledException)
            {

            }
        }

        public bool IsConnected => client != null && client.Client != null && client.Client.Connected;

        public void WriteMessage(MessageType messageType, string message)
        {
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine(messageType.ToString() + ":" + message);
            writer.Flush();
        }

        public void Disconnect()
        {
            Close();
            Disconnected?.Invoke();
        }

        public void Close()
        {
            source.Cancel();
        }
    }
}
