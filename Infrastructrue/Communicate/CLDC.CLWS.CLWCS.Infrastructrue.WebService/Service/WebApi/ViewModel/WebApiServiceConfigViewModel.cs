using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.ViewModel
{
    public sealed class WebApiServiceConfigViewModel : ViewModelBase
    {
        public WebApiServiceConfigViewModel()
        {
            //GetIpValueList();
        }

       public List<string> IpValueList=new List<string>();

       private void GetIpValueList()
        {
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                {
                    IpValueList.Add(ipa.ToString());
                }
            }
        }

        /// <summary>
        /// 服务IP值
        /// </summary>
        public string IpValue { get; set; }
        /// <summary>
        /// 服务端口值
        /// </summary>
        public int PortValue { get; set; }
        /// <summary>
        /// 服务的控制器
        /// </summary>
        public string ControllerValue { get; set; }

    }
}
