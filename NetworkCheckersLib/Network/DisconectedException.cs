using System;
using System.Runtime.Serialization;

namespace NetworkCheckersLib.Network
{
    [Serializable]
    public class DisconectedException : Exception
    {
        public DisconectedException()
        {
        }

        public DisconectedException(string message) : base(message)
        {
        }

        public DisconectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DisconectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}