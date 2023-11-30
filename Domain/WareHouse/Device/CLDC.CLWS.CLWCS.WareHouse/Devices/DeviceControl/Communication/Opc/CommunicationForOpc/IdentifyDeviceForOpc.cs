using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 对于识别设备的OPC协议
    /// </summary>
    public class IdentifyDeviceForOpc : IIdentifyDeviceCom<List<string>>
    {

        public PlcDeviceForOpc PlcCommunicate { get; set; }

        private OperateResult InitCommunicate()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                PlcCommunicate = new PlcDeviceForOpc();
                return PlcCommunicate.Initialize(DeviceId, DeviceName);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public string Name { get; set; }
        public int DeviceId { get; set; }

        public MessageReportDelegate MessageReportEvent { get; set; }

        public void MessageReport(string message,EnumLogLevel messageLevel)
        {
            if (MessageReportEvent != null)
            {
                MessageReportEvent(message, messageLevel);
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            private set { _isConnected = value; }
        }

        public DeviceName DeviceName { get; set; }


        public OperateResult Initialize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
            return InitCommunicate();
        }

        private List<DataBlockNameEnum> barcodeDatablocks = new List<DataBlockNameEnum>();
        private List<string> barcodes = new List<string>();
        private bool _isConnected = true;

        public event BarcodeCallback<List<string>> OnReceiveBarcode;

        /// <summary>
        /// 对于OPC条码枪para可能传的是Datablock的名称或者枚举
        /// </summary>
        /// <param name="para"></param>
        public async void SendCommandAsync(params object[] para)
        {
            barcodeDatablocks.Clear();
            barcodes.Clear();

            if (para != null)
            {
                foreach (object element in para)
                {
                    if (element is DataBlockNameEnum)
                    {
                        barcodeDatablocks.Add((DataBlockNameEnum)element);
                    }
                }
            }
            if (barcodeDatablocks.Count <= 0)
            {
                string barcode = await GetBarcodeAsync(DataBlockNameEnum.OPCBarcodeDataBlock);
                barcodes.Add(barcode);
                if (OnReceiveBarcode != null)
                {
                    OnReceiveBarcode(DeviceName, barcodes);
                }
            }
            else
            {
                foreach (DataBlockNameEnum dataBlockEnum in barcodeDatablocks)
                {
                    string barcode = await GetBarcodeAsync(dataBlockEnum);
                    barcodes.Add(barcode);
                }
                if (OnReceiveBarcode != null)
                {
                    OnReceiveBarcode(DeviceName, barcodes);
                }
            }
        }

        private Task<string> GetBarcodeAsync(DataBlockNameEnum datablockEnum)
        {
            var task = Task.Run(() =>
            {
                OperateResult<string> barcodeResult = PlcCommunicate.ReadString(datablockEnum);
                return barcodeResult.Content;
            });
            return task;
        }
        private Task<int> GetGoodsHeightAsync(DataBlockNameEnum datablockEnum)
        {
            var task = Task.Run(() =>
            {
                OperateResult<int> goodsHeightResult = PlcCommunicate.ReadInt(datablockEnum);
                return goodsHeightResult.Content;
            });
            return task;
        }
        public OperateResult<List<string>> SendCommand(params object[] para)
        {
            barcodeDatablocks.Clear();
            barcodes.Clear();
            OperateResult<List<string>> result = OperateResult.CreateFailedResult(barcodes, "无数据");
            try
            {
                if (para != null)
                {
                    foreach (object element in para)
                    {
                        if (element is DataBlockNameEnum)
                        {
                            barcodeDatablocks.Add((DataBlockNameEnum)element);
                        }
                    }
                }
                if (barcodeDatablocks.Count <= 0)
                {
                    OperateResult<string> barcodeResult = PlcCommunicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    barcodes.Add(barcodeResult.Content);
                    result.Content = barcodes;
                    result.IsSuccess = true;
                }
                else
                {
                    foreach (DataBlockNameEnum datablockEnum in barcodeDatablocks)
                    {
                        OperateResult<string> barcodeResult = PlcCommunicate.ReadString(datablockEnum);
                        barcodes.Add(barcodeResult.Content);
                    }
                    result.Content = barcodes;
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }
    }
}
