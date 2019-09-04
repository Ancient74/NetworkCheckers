using NetworkCheckersLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;

namespace CheckersServer
{
    class Program
    {
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("LogConfig.xml"));
            GameHost host = new GameHost();
            host.Start();
            Console.ReadKey();
        }
    }
}
