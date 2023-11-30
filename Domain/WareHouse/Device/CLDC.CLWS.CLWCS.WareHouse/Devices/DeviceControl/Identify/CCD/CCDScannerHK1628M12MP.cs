using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class CCDScannerHK1628M12MP : IIdentifyDeviceCom<List<string>>
    {

        public SocketAsyncClient SocketClient { get; set; }
        public bool IsConnected { get; private set; }
        public DeviceName DeviceName { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        private static int CurBoxType { get; set; }
        private static bool IsNeedChange { get; set; }
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

                SocketClient.ReceivedMessageNotifyEvent += DeviceCommunicate_RvDataEvent;
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

        void DeviceCommunicate_RvDataEvent(byte[] reciveData)
        {
            //接收到的数据，转换为条码
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

                DeviceBaseAbstract deviceBase =
                                  DeviceManage.DeviceManage.Instance.FindDeivceByDeviceId(1054);
                TransportPointStation transDevice = deviceBase as TransportPointStation;
                RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                OperateResult<bool> readOrderIDOpcResult = roller.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                //判断是否有货，防止人工现场操作上报数据，导致条件触发
                if (!readOrderIDOpcResult.IsSuccess || readOrderIDOpcResult.Content ==false)
                {
                    string strReadOpcStatu = readOrderIDOpcResult.IsSuccess ? "成功" : "失败";
                    string strReadOpcIsLoadedGoods = readOrderIDOpcResult.Content ? "有货" : "无货";
                    string errMsg = string.Format("读取1054 OPC就绪状态:{0},OPC值默认为0，具体OPC值:{1}", strReadOpcStatu, strReadOpcIsLoadedGoods);
                    MessageReport(errMsg, EnumLogLevel.Error);
                    return;
                }
                    
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
            if (para != null && para.Length>0)
            {
                //切换模板 传递的为箱子条码
                //根据传递过来的箱子条码类型 进行切换模板
                string strCurBoxCode = para[0].ToString();
                int modelType = GetCCDModelTypeForBoxCode(strCurBoxCode.Trim());
                if (CurBoxType != modelType)
                {
                    IsNeedChange = true;
                    CurBoxType = modelType;
                    //获取IP
                    //byte[] btArr = BitConverter.GetBytes(modelType);//1现场如果不行就用下面
                    byte[] btArr = Encoding.ASCII.GetBytes(modelType.ToString()); //2
                    OperateResult opResult = SocketClient.Send(btArr);
                    string logMsg = "发送切换模板，发送的模板参数：" + BitConverter.ToString(btArr);
                    if (opResult.IsSuccess)
                    {
                        logMsg += "   发送成功!";
                    }
                    else
                    {
                        logMsg += "   发送失败!";
                    }
                    logMsg += string.Format("  调用模板切换,切换参数: {0}", modelType.ToString());
                    LogHelper.WriteLog(this.DeviceName.ToString(), logMsg, EnumLogLevel.Info);
                }
                else
                {
                    IsNeedChange = false;
                }

            }
            else
            {
                //CCD 海康   SCNT
                if (IsNeedChange)
                {
                    Thread.Sleep(5000);
                }
                if (SocketClient.IsConnected)
                {
                    OperateResult opSenLonResult = SocketClient.Send(Encoding.ASCII.GetBytes("LON"));
                    Thread.Sleep(2000); //根据实际读取情况进行延迟 处理 100-500
                    OperateResult opSenLoffResult = SocketClient.Send(Encoding.ASCII.GetBytes("LOFF"));
                    string logmsg = "发送CCD读取和停止";
                    if (opSenLonResult.IsSuccess == true && opSenLoffResult.IsSuccess == true)
                    {
                        logmsg += "   发送成功!";
                    }
                    else
                    {
                        logmsg += "   发送失败!";
                    }
                    LogHelper.WriteLog(this.DeviceName.ToString(), logmsg, EnumLogLevel.Info);
                }
            }
        }
        /// <summary>
        /// 通过扫描的箱子条码获取箱子类型
        /// </summary>
        /// <param name="boxCode">箱条码</param>
        /// <returns>箱类型</returns>
        private int GetCCDModelTypeForBoxCode(string boxCode)
        {
            int mode = 0;
            if (!string.IsNullOrEmpty(boxCode))
            {
                mode = int.Parse(boxCode.Substring(4, 1));
            }
            return mode;
        }

        public OperateResult<List<string>> SendCommand(params object[] para)
        {
            throw new NotImplementedException();
        }


        public event BarcodeCallback<List<string>> OnReceiveBarcode;
    }
}
