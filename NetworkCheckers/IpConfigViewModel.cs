using NetworkCheckersLib;
using NetworkCheckersLib.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace NetworkCheckers
{
    public class IpConfigViewModel : NotifiableObject
    {
        private string ip;
        private int port;

        public IpConfig IpConfig
        {
            get => new IpConfig { Ip = Ip, Port = Port };
            set
            {
                Ip = value.Ip;
                Port = value.Port;
            }
        }

        private IpConfigController IpConfigController = new IpConfigController();
        public IpConfigViewModel(IIpConfigMenu ipConfigMenu)
        {
            CancelCommand = new CallbackCommand(() => ipConfigMenu.Cancel());
            IpConfigController.IpConfigChanged += OnIpConfigChanged;
            OnIpConfigChanged();
        }

        private void OnIpConfigChanged()
        {
            var ipConfig = IpConfigController.IpConfig;
            Port = ipConfig.Port;
            Ip = ipConfig.Ip;
        }

        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged("Port");
            }
        }

        public ICommand SaveCommand => new CallbackCommand(Save);
        public ICommand CancelCommand { get; }

        private void Save()
        {
            IpConfig = new IpConfig { Ip = Ip, Port = Port };
            IpConfigController.IpConfig = IpConfig;
        }

        public string Ip
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged("Ip");
            }
        }
    }
}
