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
using System.Diagnostics;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class KeyenceScanner2000W : IIdentifyDeviceCom<List<string>>
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

                SocketClient.ReceivedMessageNotifyEvent += PlcCommunicate_RvDataEvent;
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
        private bool IsReviced = false;

        private string strReviced = "false";

        List<string> readCurBarcodeList = new List<string>();
        void PlcCommunicate_RvDataEvent(byte[] reciveData)
        {
            List<string> curBarcodeList = new List<string>();
            IsReviced = false;
            curBarcodeList = new List<string>();
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
                string barcodes = "";
                //if (reciveData[0] == 30 && reciveData[1] == 29 && reciveData[reciveData.Length - 1] == 4)
                if (reciveData[0] == 30 && reciveData[1] == 29)
                {
                    //SCC自定义QR码
                    //30 RS 
                    //29 GS
                    //04 EQT
                    //第1个GS到第2个GS 物料编号 固定9码
                    //第2个GS到第3个GS 供应商编码
                    //第3个GS到第4个GS 供应商内部物料批次号
                    //第4个GS到第5个GS 供应商生产日期 固定6码数字格式
                    //第5个GS到第6个GS 物料失效日期 固定6码数字格式
                    //第6个GS到第7个GS 公司订单编号
                    //第7个GS到第8个GS 物料数量
                    //第8个GS到第9个GS 小包装的流水序号，固定4码数字格式
                    //第9个GS到第10个GS 预留信息栏位，当前设置位空 ""
                    //EQT:二维码终止符
                    string str = Encoding.ASCII.GetString(reciveData);
                    str = str.Replace("\u0004", "").Replace("\u001e", "");
                    string[] data = str.Split(new string[] { "\u001d" }, StringSplitOptions.None);
                    barcodes = string.Join("|", data).Remove(0, 1);

                    readCurBarcodeList.Add(barcodes);
                }
                else
                {
                    //XXXAO67510200/A2303-07-A33/495E20/2030327/50/20/495/SR1#1761/1009
                    barcodes = Encoding.ASCII.GetString(reciveData);
                    string[] barcodesList = barcodes.Replace("\u0000", "").Split(',');
                    //curBarcodeList = barcodesList.Select(t => t.Replace("\r", "")).ToList();
                    readCurBarcodeList.Add(barcodesList.Select(t => t.Replace("\r", "")).ToList()[0]);
                
                }
            }

            if (SystemConfig.Instance.WhCode.Equals("SNDL1"))
            {
                if (scannerCount == readCurBarcodeList.Count)
                {
                    IsReviced = true;
                }
                else
                {
                    //if (times == 0)
                    //{
                    //    //未扫到
                    //    var tsk = Task.Run(() =>
                    //    {
                    //        ScannerTimeOut();
                    //    });
                    //    times = 1;
                    //}
                }
            }
            else
            {
                if (OnReceiveBarcode != null)
                {
                    readCurBarcodeList = curBarcodeList;
                    IsReviced = true;
                    OnReceiveBarcode(DeviceName, curBarcodeList);
                    IsReviced = false;
                }
            }
        }
        /// <summary>
        /// 扫描超时
        /// </summary>
        private void ScannerTimeOut()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                if (watch.ElapsedMilliseconds >= 1000)
                {
                    IsReviced = true;
                    break;
                }
                Thread.Sleep(3);
            }
            watch.Stop();
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
                Thread.Sleep(500); //根据实际读取情况进行延迟 处理 100-500
                SocketClient.Send(Encoding.ASCII.GetBytes("LOFF"));
            }
            else
            {

                if (SystemConfig.Instance.WhCode.Equals("SNDL1"))
                {
                    //基恩士 需要空格  SNDL1    
                    SocketClient.Send(Encoding.ASCII.GetBytes("LON" + "\r\n"));
                    Thread.Sleep(150);//根据实际读取情况进行延迟 处理 100-500
                    SocketClient.Send(Encoding.ASCII.GetBytes("LOFF" + "\r\n"));
                }
                else
                {
                    //基恩士 需要空格    
                    SocketClient.Send(Encoding.ASCII.GetBytes("LON" + "\r\n"));
                    Thread.Sleep(2000);//根据实际读取情况进行延迟 处理 100-500
                    SocketClient.Send(Encoding.ASCII.GetBytes("LOFF" + "\r\n"));
                }
            }
        }
        int timeOut = 3000;//ms 等待时间 具体根据实际情况调整
        int scannerCount = 1;//扫描个数
        int times = 0;//第一次
        public OperateResult<List<string>> SendCommand(params object[] para)
        {
            times = 0;
            var nn = para.GetEnumerator();
            while (nn.MoveNext())
            {
               var cc= nn.Current;
                scannerCount = ((List<string>)cc).Count;
            }
         
            OperateResult<List<string>> tempResultList = new OperateResult<List<string>>();
            readCurBarcodeList = new List<string>();
            IsReviced = false;
            SocketClient.Send(Encoding.ASCII.GetBytes("LON" + "\r\n"));
            //Thread.Sleep(300);//根据实际读取情况进行延迟 处理 100-500
            //SocketClient.Send(Encoding.ASCII.GetBytes("LOFF" + "\r\n"));
            if (IsConnected)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (true)
                {
                    if(IsReviced)
                    {
                        tempResultList.Content = readCurBarcodeList;
                        if (OnReceiveBarcode != null)
                        {
                            SocketClient.Send(Encoding.ASCII.GetBytes("LOFF" + "\r\n"));
                            OnReceiveBarcode(DeviceName, readCurBarcodeList);
                        }
                        break;
                    }
                    if(watch.ElapsedMilliseconds >= timeOut)
                    {
                        tempResultList.Content = readCurBarcodeList;
                        if (OnReceiveBarcode != null)
                        {
                            SocketClient.Send(Encoding.ASCII.GetBytes("LOFF" + "\r\n"));
                            OnReceiveBarcode(DeviceName, readCurBarcodeList);
                        }
                        break;
                    }
                    Thread.Sleep(3);
                }
                watch.Stop();
                tempResultList.ErrorCode = 1;
                tempResultList.IsSuccess = true;
                tempResultList.Message = "";
            }
            else
            {
                //未连接上
                tempResultList.ErrorCode = 2;
                tempResultList.IsSuccess = false;
                tempResultList.Message = "未连接上RFID";
            }
            return tempResultList;
        }


        public event BarcodeCallback<List<string>> OnReceiveBarcode;
    }
}
