using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using log4net;

namespace NetworkCheckersLib.Network
{
    public struct HostClient
    {
        public TcpClient client;
        public Timer pingTimer;
        public int Id;
    }
    public abstract class HostBase
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(HostBase));

        private TcpListener tcpListener;
        private int Port { get; }

        private CancellationTokenSource source = new CancellationTokenSource();

        private IPAddress IPAddress { get; }

        protected List<HostClient> clients = new List<HostClient>();

        public event Action<TcpClient, MessageType, string> MessageRecieved;

        public HostBase(IpConfig ipConfig)
        {
            IPAddress = IPAddress.Parse(ipConfig.Ip);
            Port = ipConfig.Port;
        }

        public void Start()
        {
            Log.Info("Server started...");
            Log.Info($"Listening {IPAddress.ToString()} on port {Port}...");
            tcpListener = new TcpListener(IPAddress, Port);
            Listen();
        }

        private void Listen()
        {
            while(true)
            {
                if(clients.Count != 2)
                {
                    Reset();
                    WaitForClients();
                }
            }
        }

        private void WaitForClients()
        {
            source?.Cancel();
            source = new CancellationTokenSource();
            tcpListener.Start();
            Log.Info("Waiting for new clients");
            while (clients.Count != 2)
            {
                var client = tcpListener.AcceptTcpClient();
                var clientHost = new HostClient() { client = client, Id = clients.Count };
                lock (clients)
                    clients.Add(clientHost);
                HandleClient(clientHost, source.Token);

                Log.Info($"Client {clientHost.Id} connected");
            }
            Log.Info("Clients are found");
            tcpListener.Stop();
        }
        private async void HandleClient(HostClient clientHost, CancellationToken token)
        {
            TcpClient client = clientHost.client;
            clientHost.pingTimer = new Timer(Ping, client, TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(1.0));
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
                                while (client.Connected)
                                {
                                    token.ThrowIfCancellationRequested();
                                    if (stream.DataAvailable)
                                    {
                                        MessageType messageType = (MessageType)reader.ReadInt32();
                                        string message = reader.ReadString();
                                        if(messageType != MessageType.Ping)
                                            Log.Info(messageType.ToString() + ":" + message);
                                        MessageRecieved?.Invoke(client, messageType, message);
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
                DisconnectClient(clientHost);
            }
        }

        private void DisconnectClient(HostClient client)
        {
            if (clients.Count == 0)
                return;

            lock(clients)
                clients.RemoveAll(x=>x.Id == client.Id);
            client.client?.Dispose();
            client.pingTimer?.Dispose();
            Log.Info($"Client {client.Id} has disconnected");
        }

        private void Ping(object client)
        {
            if (!(client is TcpClient tcpClient))
                return;
            if (!tcpClient.Connected)
                return;
            try
            {
                WriteMessage(tcpClient, MessageType.Ping);
            }
            catch
            {
                source.Cancel();
                source = new CancellationTokenSource();
            }
        }
        protected void Reset()
        {
            for(int i = 0;i<clients.Count; i++)
            {
                DisconnectClient(clients[i]);
                i--;
            }
            clients = new List<HostClient>();
        }

        protected void WriteMessage(TcpClient client, MessageType messageType, string message = "")
        {
            if (client == null)
                return;

            lock(client)
            {
                var stream = client.GetStream();
                var writer = new BinaryWriter(stream);
                writer.Write((int)messageType);
                writer.Write(message);
                writer.Flush();
            }
        }

        protected void WriteMessage(MessageType messageType, string message = "")
        {
            foreach(var client in clients)
            {
                WriteMessage(client.client, messageType, message);
            }
        }
    }
}
