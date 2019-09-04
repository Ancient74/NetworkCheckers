using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using log4net;

namespace NetworkCheckersLib.Network
{
    public abstract class HostBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HostBase));

        private TcpListener tcpListener;
        private int Port { get; } = 14447;

        private CancellationTokenSource source = new CancellationTokenSource();

        private IPAddress IPAddress { get; } = IPAddress.Parse("127.0.0.1");

        protected TcpClient[] clients = new TcpClient[2];

        public event Action NoResonse;
        public event Action Disconnected;
        public event Action<TcpClient, string> MessageRecieved;

        private Timer[] pingTimers;

        public HostBase()
        {
            Disconnected += OnDisconnected;
            NoResonse += OnNoResonse;
        }

        private void OnNoResonse()
        {
            if (pingTimers == null)
                return;
            foreach(var timer in pingTimers)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
            }
            pingTimers = null;
            source.Cancel();
        }

        public void Start()
        {
            tcpListener = new TcpListener(IPAddress, Port);
            Listen();
        }

        public void Stop()
        {
            Log.Info("Listener stopped...");
            tcpListener.Stop();
        }

        private void StartPinging()
        {
            pingTimers = new Timer[2];
            for (int i = 0; i < pingTimers.Length; i++)
            {
                int ii = i;
                pingTimers[i] = new Timer((object state) =>
                {
                    try
                    {
                        lock (clients[ii])
                        {
                            var writer = new StreamWriter(clients[ii].GetStream());
                            writer.WriteLine(MessageType.Ping.ToString());
                            writer.Flush();
                        }
                    }
                    catch(ObjectDisposedException)
                    {
                        return;
                    }
                    catch (IOException)
                    {
                        Log.Info($"Client{ii+1}: did not respond");
                        NoResonse?.Invoke();
                    }
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.0));
            }
        }

        private void Listen()
        {
            
            tcpListener.Start();
            source = new CancellationTokenSource();
            Log.Info("Server started...");
            Log.Info($"Listening {IPAddress.ToString()} on port {Port}...");
            var client1 = tcpListener.AcceptTcpClient();
            Log.Info("Client 1 connected");
            var client2 = tcpListener.AcceptTcpClient();
            Log.Info("Client 2 connected");
            clients = new[] { client1, client2 };
            void listen(TcpClient client, CancellationToken token)
            {
                var clientNo = client == client1 ? 1 : 2;
                using (client)
                {
                    using (var stream = client.GetStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            while (client.Connected)
                            {
                                try
                                {
                                    token.ThrowIfCancellationRequested();
                                    if (stream.DataAvailable)
                                    {
                                        lock (client)
                                        {
                                            string line = reader.ReadLine();
                                            if (line != MessageType.Ping.ToString())
                                                Log.Info($"Recieved from:Client{clientNo}: {line}");
                                            MessageRecieved?.Invoke(client, line);
                                        }
                                    }
                                }
                                catch
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            var task1 = Task.Run(() => listen(clients[0], source.Token));
            var task2 = Task.Run(() => listen(clients[1], source.Token));
            try
            {
                Stop();
                StartPinging();
                Task.WaitAll(new[] { task1, task2 }, source.Token);
            }
            catch (AggregateException ae)
            {
                foreach (var ie in ae.InnerExceptions)
                    Log.Info(ie);
                Disconnected?.Invoke();
            }
            catch (Exception e)
            {
                Log.Info(e.Message);
                Disconnected?.Invoke();

            }
        }
        private void OnDisconnected()
        {
            clients?[0]?.Close();
            clients?[1]?.Close();
            clients = new TcpClient[2];
            OnNoResonse();
            Listen();
        }
    }
}
