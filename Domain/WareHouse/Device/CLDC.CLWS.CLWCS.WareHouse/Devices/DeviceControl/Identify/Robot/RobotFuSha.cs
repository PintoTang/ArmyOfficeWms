using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class RobotFuSha : IIdentifyDeviceCom<List<string>>
    {
        public SocketAsyncClient SocketClient { get; set; }
        public SocketAsyncClient SocketClient2003 { get; set; }
        public bool IsConnected { get; private set; }
        public DeviceName DeviceName { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }

        private OperateResult InitCommunicate()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                SocketClient = new SocketAsyncClient();
                OperateResult initializeResult = SocketClient.Initilize(DeviceId);
                if (!initializeResult.IsSuccess)
                {
                    return initializeResult;
                }

                SocketClient.ReceivedMessageNotifyEvent += SocketCommunicate_RvDataEvent;
                SocketClient.NotifyConnectedStatusChangeEvent += SocketClient_NotifyConnectedStatusChangeEvent;
                SocketClient.MessageReportEvent += MessageReport;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        public MessageReportDelegate MessageReportEvent { get; set; }

        public  void MessageReport(string message,EnumLogLevel messageLevel)
        {
            if (MessageReportEvent != null)
            {
                MessageReportEvent(message, messageLevel);
            }
        }

        void SocketClient_NotifyConnectedStatusChangeEvent(bool isConnected)
        {
            IsConnected = isConnected;
        }

        void SocketCommunicate_RvDataEvent(byte[] reciveData)
        {
            List<string> curBarcodeList = new List<string>();
            if (SystemConfig.Instance.WhCode.Equals("SCNT1"))
            {
                //四川能投CCD CCDScannerHK1628M12MP
                string barcodes = Encoding.ASCII.GetString(reciveData);
                string[] barcodesList = barcodes.Split(',');
                //1:D0,2:D0
                foreach (string meterBarCode in barcodesList)
                {
                    if (!string.IsNullOrEmpty(meterBarCode.Replace("\r", "")))
                    {
                        string strCode = meterBarCode.Split(':')[1].Trim().Replace("\r", "");
                        if (string.IsNullOrEmpty(strCode)) continue;
                        curBarcodeList.Add(strCode);
                    }
                }
            }
            else
            {
                //接收到的数据，转换为条码
                string barcodes = Encoding.ASCII.GetString(reciveData);
                string[] barcodesList = barcodes.Replace("\u0000","").Split(',');
                curBarcodeList = barcodesList.Select(t => t.Replace("\r", "")).ToList();
            }
            if (OnReceiveBarcode != null)
            {
                OnReceiveBarcode(DeviceName, curBarcodeList);
            }
        }

        public OperateResult Initialize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
            return InitCommunicate();
        }
        /// <summary>
        /// LON 开始读取条码 LOFF 结束读取条码
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public void SendCommandAsync(params object[] para)
        {
            if (SystemConfig.Instance.WhCode.Equals("SCNT1"))
            {
                //CCD 海康   SCNT
                SocketClient.Send(Encoding.ASCII.GetBytes("LON"));
                Thread.Sleep(5000); //根据实际读取情况进行延迟 处理 100-500
                SocketClient.Send(Encoding.ASCII.GetBytes("LOFF"));
            }
            else
            {
                //基恩士 需要空格    
                SocketClient.Send(Encoding.ASCII.GetBytes("LON" + "\r\n"));
                Thread.Sleep(2000);//根据实际读取情况进行延迟 处理 100-500
                SocketClient.Send(Encoding.ASCII.GetBytes("LOFF" + "\r\n"));
            }
        }

        public OperateResult<List<string>> SendCommand(params object[] para)
        {
            throw new NotImplementedException();
        }


        public event BarcodeCallback<List<string>> OnReceiveBarcode;
    }
}
