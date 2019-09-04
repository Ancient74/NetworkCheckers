using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.Network
{
    public interface IClient : IAsyncMessanger
    {
        void Connect();

        void Disconnect();

        bool IsConnected { get; }

        event Action Connected;
        event Action Disconnected;
    }
}
