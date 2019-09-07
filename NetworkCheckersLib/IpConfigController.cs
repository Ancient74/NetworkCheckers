using NetworkCheckersLib.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetworkCheckersLib
{
    public class IpConfigController
    {
        const string IP_CONFIG_FILE = "IpConfig.xml";
        FileSystemWatcher fileSystemWatcher;
        public IpConfigController()
        {
            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = Path.GetDirectoryName(IP_CONFIG_FILE);
            fileSystemWatcher.Filter = Path.GetFileName(IP_CONFIG_FILE);
            fileSystemWatcher.Changed += OnFileChanged;
            fileSystemWatcher.Deleted += OnFileDeleted;
            fileSystemWatcher.Renamed += OnFileDeleted;
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            IpConfig = IpConfig.Default;
            IpConfigChanged?.Invoke();
        }

        public event Action IpConfigChanged;

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            IpConfigChanged?.Invoke();
        }

        public IpConfig IpConfig
        {
            get => Load();
            set
            {
                Save(value);
            }
        }

        private IpConfig Load()
        {
            FileInfo file = new FileInfo(IP_CONFIG_FILE);
            var serializer = new XmlSerializer(typeof(IpConfig));
            IpConfig ipConfig;
            bool doSave = false;
            using (FileStream fileStream = new FileStream(file.FullName, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    try
                    {
                        ipConfig = (IpConfig)serializer.Deserialize(reader);
                    }
                    catch(Exception e) when (e is InvalidCastException || e is InvalidOperationException)
                    {
                        ipConfig = IpConfig.Default;
                        doSave = true;
                    }
                }
            }
            if (doSave)
                Save(ipConfig);
            return ipConfig;
        }

        private void Save(IpConfig ipConfig)
        {
            FileInfo file = new FileInfo(IP_CONFIG_FILE);
            var serializer = new XmlSerializer(typeof(IpConfig));
            using (FileStream fileStream = new FileStream(file.FullName, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    serializer.Serialize(writer, ipConfig);
                }
            }
        }
    }
}
