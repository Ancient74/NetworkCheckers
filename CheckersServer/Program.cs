using NetworkCheckersLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;
using System.Xml.Serialization;
using NetworkCheckersLib;

namespace CheckersServer
{
    class Program
    {
        const string LOG_CONFIG_FILE = "LogConfig.xml";
        static IpConfigController IpConfigController = new IpConfigController();

        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(LOG_CONFIG_FILE));
            ILog log = LogManager.GetLogger(typeof(Program));

            IpConfig ipConfig;
            try
            {
                ipConfig = IpConfigController.IpConfig;
            }
            catch
            {
                log.Error($"Error reading {LOG_CONFIG_FILE}");
                log.Warn($"Creating new {LOG_CONFIG_FILE} with default configuration");
                ipConfig = IpConfig.Default;
                IpConfigController.IpConfig = ipConfig;
            }
            GameHost host;
            try
            {
               host = new GameHost(ipConfig);
            }
            catch(FormatException)
            {
                log.Error("Wrong ip config");
                log.Warn("Using default ip config");
                host = new GameHost(IpConfig.Default);
            }
            host.Start();
            Console.ReadKey();
        }

    }
}
