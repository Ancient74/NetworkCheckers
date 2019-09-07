using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.Network
{
    [Serializable]
    public class IpConfig
    {
        public int Port { get; set; }
        public string Ip { get; set; }

        public static IpConfig Default = new IpConfig { Ip = "127.0.0.1", Port = 14447 };
    }
}
