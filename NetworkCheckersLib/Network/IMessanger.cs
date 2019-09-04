using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.Network
{
    public interface IAsyncMessanger
    {
        void Listen();

        event EventHandler<string> MessageRecieved;

        void WriteMessage(MessageType messageType, string message);
    }
}
