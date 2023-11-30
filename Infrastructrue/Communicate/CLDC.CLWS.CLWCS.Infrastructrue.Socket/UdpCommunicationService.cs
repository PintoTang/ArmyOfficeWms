using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets
{
    public static class UdpCommunicationService
    {
        public static event ReceiveDataEventHandler ReceiveData;
        private static UdpLibrary _udpLibrary;
        private static object _lockObj = new object();
        private static int _sendTimes;

        public static int SendTimes
        {
            get { return _sendTimes; }
        }
        
        public static void Start(string ip,int port,int sendTimes=1)
        {
            if (_udpLibrary == null)
            {
                lock (_lockObj)
                {
                    if (_udpLibrary == null)
                    {
                        if (sendTimes <= 0)
                        {
                            sendTimes = 1;
                        }
                        _sendTimes = sendTimes;
                        _udpLibrary = new UdpLibrary(ip,port);
                        _udpLibrary.ReceiveData += _udpLibrary_ReceiveData;
                        _udpLibrary.Start();
                    }
                }
            }
        }

        private static void _udpLibrary_ReceiveData(object sender, ReceiveDataEventArgs e)
        {
            try
            {
                if (ReceiveData != null)
                {
                    ReceiveData(sender, e);
                }

            }
            catch (Exception ex)
            {

            }
        }

        public static bool SendCmd(byte[] buffer,IPEndPoint remoteIP)
        {
            _udpLibrary.Send(buffer, remoteIP);
            //for (int i = 0; i < _sendTimes; i++)
            //{
            //    _udpLibrary.Send(buffer, remoteIP);
            //    Thread.Sleep(100);
            //}
            return true;
        }
    }
}
