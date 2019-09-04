using System;

namespace NetworkCheckersLib.Network
{
    public enum ConnectResult
    {
        Success,
        Fail
    }
    public class ConnectedEventArgs : EventArgs
    {
        public ConnectResult Result { get; }
        public ConnectedEventArgs(ConnectResult result)
        {
            Result = result;
        }
    }
}