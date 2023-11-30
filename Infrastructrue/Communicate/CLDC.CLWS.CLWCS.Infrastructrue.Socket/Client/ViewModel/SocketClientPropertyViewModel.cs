using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.ViewModel
{
    public class SocketClientPropertyViewModel
    {
        public SocketClientProperty DataModel { get; set; }
        public SocketClientPropertyViewModel(SocketClientProperty dataModel)
        {
            DataModel = dataModel;
        }

        private readonly Dictionary<ProtocolType, string> _dicProtocolType=new Dictionary<ProtocolType, string>(); 
        public Dictionary<ProtocolType, string> DicProtocolType
        {
            get
            {
                if (_dicProtocolType.Count==0)
                {
                    _dicProtocolType.Add(ProtocolType.Tcp,ProtocolType.Tcp.ToString());
                    _dicProtocolType.Add(ProtocolType.Udp,ProtocolType.Udp.ToString());
                }
                return _dicProtocolType;
            }
        }

    }
}
