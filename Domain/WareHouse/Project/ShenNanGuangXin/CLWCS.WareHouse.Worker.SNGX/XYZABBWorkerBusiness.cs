using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using Newtonsoft.Json;
using CLDC.Infrastructrue.Xml;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.InAndOutCell.Model;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using Infrastructrue.Ioc.DependencyFactory;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CLWCS.WareHouse.Device.HeFei.Simulate3D.Model;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel;
using ResponseResult = CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel.ResponseResult;
using System.Threading;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    /// XYZABB接口业务处理
    /// </summary>
    public class XYZABBWorkerBusiness : InAndOutCellBusinessAbstract, IXYZABBServiceApi
    {
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;
        protected List<AgvInfo> AgvInfoDic = new List<AgvInfo>();
        private int sim3DDeviceId = 3333;
        private OrderManage _orderManage;
        protected override OperateResult ParticularInitlize()
        {
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            DependencyFactory.GetDependency().RegisterInstance<IXYZABBServiceApi>(this);
             _orderManage = DependencyHelper.GetService<OrderManage>();

            if (inDic.Count == 0)
            {
                //加载与3D仿真系统的基础数据
                LoadBaseData();
            }
            return OperateResult.CreateSuccessResult();
        }

        #region  系统默认方法 
        public override OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg)
        {
            return OperateResult.CreateSuccessResult();
        }


        public override OperateResult BeforeFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult FinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ForceFinishOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult CancleOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult BeforeTransportBusinessHandle(TransportMessage transportMsg)
        {

            //1.判断搬运设备是否空闲
            //2.判断搬运设备的运行模式
            //1.询问目标设备是否空闲
            DeviceBaseAbstract startDevice = transportMsg.StartDevice;
            DeviceBaseAbstract destDevice = transportMsg.DestDevice;
            DeviceBaseAbstract transportDevice = transportMsg.TransportDevice;

            if (transportDevice == null)
            {
                return OperateResult.CreateFailedResult("搬运设备为空", 1);
            }
            OperateResult transportIsavailable = transportDevice.Availabe();
            if (!transportIsavailable.IsSuccess)
            {
                return
                    OperateResult.CreateFailedResult(
                        string.Format(" 搬运设备：{0} 当前状态不可用，原因：{1}", transportDevice.Name, transportIsavailable.Message), 1);
            }
            if (destDevice == null)
            {
                return OperateResult.CreateSuccessResult();
            }
            OperateResult destDeviceIsavailable = destDevice.Availabe();
            if (!destDeviceIsavailable.IsSuccess)
            {
                return
                    OperateResult.CreateFailedResult(
                        string.Format(" 目标设备：{0} 当前状态不可用，原因：{1}", destDevice.Name, destDeviceIsavailable.Message), 1);
            }
            return OperateResult.CreateSuccessResult();
        }
      
        private OperateResult NotifyWmsDeviceException(string barcode, string cmdPara)
        {
            NotifyElement element = new NotifyElement(barcode,"NotifyInstructException", "上报搬运指令异常", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口 NotifyInstructException 失败，详情：\r\n {0}", result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            try
            {
                SyncResReErr serviceReturn = (SyncResReErr)result.Content.ToString();
                if (serviceReturn.IsSuccess)
                {
                    return OperateResult.CreateSuccessResult("调用接口 NotifyInstructException 成功，并且WMS返回成功信息");
                }
                else
                {
                    return OperateResult.CreateSuccessResult(string.Format("调用接口 NotifyInstructException 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.ERR_MSG));
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        /// <summary>
        /// ABB上报异常的业务处理
        /// </summary>
        /// <param name="exceptionMsg"></param>
        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            //1.调用NotifyInstructException通知WMS异常信息 需要的入参：指令编号 包装条码 异常代号 当前地址
            //01 - 放货异常，仓位有货
            //02 - 取货异常，仓位取空
            //03 - 取货异常，取深仓位，浅仓位有货
            OperateResult<NotifyInstructExceptionMode> getMode = null; //To Do
            if (!getMode.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("异常转换协议失败，失败原因：\r\n{0}", getMode.Message), 1);
            }
            string cmdPara = JsonConvert.SerializeObject(getMode.Content);
            return NotifyWmsDeviceException(getMode.Content.PACKAGE_BARCODE, cmdPara);
        }

        protected override OperateResult ParticularConfig()
        {

            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetCoordinationConfig;
                string path = "BusinessHandle";
                XmlElement xmlElement = doc.GetXmlElement("Coordination", "Id", WorkerId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", WorkerId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                return InitalizeConfig(xmlNode);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }
        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            AgvInfoDic.Clear();
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                FourWayVehicleWorkerBusinessHandleProperty handleProperty = null;
                string businessPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(businessPropertyXml))
                {
                    try
                    {
                        handleProperty = (FourWayVehicleWorkerBusinessHandleProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(FourWayVehicleWorkerBusinessHandleProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess || handleProperty == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：IdentifyWorkerProperty", businessPropertyXml));
                }
                string content = handleProperty.Config.DeviceNoConvert.Trim();
                InitilizeAgvInfo(content);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }

            return result;
        }
        private void InitilizeAgvInfo(string deviceConvert)
        {
            string[] keyValue = deviceConvert.Split(';');
            foreach (string key_value in keyValue)
            {
                if (string.IsNullOrEmpty(key_value))
                {
                    continue;
                }
                string[] wcs_hangCha = key_value.Split('_');
                if (wcs_hangCha.Length >= 2)
                {
                    AgvInfo agvinfo = new AgvInfo
                    {
                        WcsId = int.Parse(wcs_hangCha[0]),
                        HangChaId = int.Parse(wcs_hangCha[1])
                    };
                    AgvInfoDic.Add(agvinfo);
                }
            }
        }
        private int XChaToWcs(int xChaId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return 9999;
            }
            if (AgvInfoDic.Exists(a => a.HangChaId.Equals(xChaId)))
            {
                return AgvInfoDic.Find(a => a.HangChaId.Equals(xChaId)).WcsId;
            }
            return 9999;
        }

        public Func<int, int, string, ResponseResult> NotifyExeResultEvent;
        #endregion


        /// <summary>
        /// 1、告知机器人状态是否OK接口 （仿真 不需要接收） OK
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string notice_system_is_ready(Notice_System_Is_ReadyCmd cmd)
        {
            ResponseResult resResponseResult = new ResponseResult();
            try
            {
                string logMsg = string.Format("接收到ABB上报的notice_system_is_ready接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);
                if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                {

                }
                else
                {
                    resResponseResult.error = 2;
                    resResponseResult.error_message = "传入的area_code错误:" + cmd.area_code;
                    return resResponseResult.ToString();
                }


                if (cmd.status.Equals(0) || cmd.status.Equals(1))
                {

                }
                else
                {
                    resResponseResult.error = 2;
                    resResponseResult.error_message = "传入的status错误:" + cmd.status;
                    return resResponseResult.ToString();
                }
                resResponseResult.error = 0;
            }
            catch (Exception ex)
            {
                resResponseResult.error = 2;
                resResponseResult.error_message = ex.ToString();
            }
            return resResponseResult.ToString();
        }

        /// <summary>
        /// 2、机器人请求WCS扫码接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string get_sku_info(Get_Sku_InfoCmd cmd)
        {
            Get_Sku_Info_ResponseResult resResponseResult = new Get_Sku_Info_ResponseResult();
            bool isTest = false;
            try
            {
                //string logMsg = string.Format("接收到ABB上报的get_sku_info接口信息:{0}", cmd.ToJson());
                //LogMessage(logMsg, EnumLogLevel.Info, true);
                //if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                //{
                //}
                //else
                //{
                //    resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //    {
                //        sku_id = "",//sku_id
                //        height = 1,
                //        length = 1,
                //        weight = 1,
                //        width = 1
                //    };
                //    resResponseResult.error = 2;
                //    resResponseResult.message = "传入的area_code错误:" + cmd.area_code;
                //    LogMessage("扫码返回机器人数据："+ resResponseResult.ToString(), EnumLogLevel.Info, false);
                //    return resResponseResult.ToString();
                //}
            
                //bool isPackOut = false;
                //NotifyPackageSkuBindBarcodeCmdMode wmsData = new NotifyPackageSkuBindBarcodeCmdMode();
                
                //PackageSkuBindParaMode para = new PackageSkuBindParaMode();
                //para.PackageBarcode = GetScanSkuInfo(cmd.area_code); //1、触发扫描业务(同步)
                //if (cmd.area_code.Equals("01"))
                //{
                //    para.BusinessType = "01";
                //}
                //else if (cmd.area_code.Equals("02"))
                //{
                //    DeviceBaseAbstract curDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
                //    TransportPointStation transDevice2001 = curDevice2001 as TransportPointStation;
                //    RollerDeviceControl roller2001 = transDevice2001.DeviceControl as RollerDeviceControl;
                //    OperateResult<int> opReadOrderType2001 = roller2001.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                 
                //    if (opReadOrderType2001.IsSuccess)
                //    {
                //        if (opReadOrderType2001.Content == 2 || opReadOrderType2001.Content == 6)
                //        {
                //            //出库、拣选
                //            para.BusinessType = "02";
                //        }
                //        else if (opReadOrderType2001.Content == 4)
                //        {
                //            //盘点
                //            para.BusinessType = "03";
                //            isPackOut = true;
                //        }
                //    }
                //    else
                //    {
                //        LogMessage("读取2001 任务类型 opc 失败！", EnumLogLevel.Error, false);
                //    }
                //}


                //if (string.IsNullOrEmpty(para.PackageBarcode) || para.PackageBarcode.Equals("ERROR"))
                //{
                //    resResponseResult.error = 1;
                //    resResponseResult.message = "条码未扫到!";

                //    resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //    {
                //        sku_id = "",//sku_id
                //        height = 1,
                //        length = 1,
                //        weight = 1,
                //        width = 1
                //    };

                //    LogMessage("条码未扫到！ 区域:" + cmd.area_code, EnumLogLevel.Error, false);
                //    LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);

                //    //写入888
                //    DeviceBaseAbstract curDevice2020 = DeviceManage.Instance.FindDeivceByDeviceId(2020);
                //    TransportPointStation transDevice2020 = curDevice2020 as TransportPointStation;
                //    RollerDeviceControl roller2020 = transDevice2020.DeviceControl as RollerDeviceControl;
                //    OperateResult opcResult2020 = roller2020.Communicate.Write(DataBlockNameEnum.NotifyOpcBackInException, 888);
                //    if(opcResult2020.IsSuccess)
                //    {
                //        LogMessage("写入箱子回退信息给OPC NotifyOpcBackInException数据 888 成功：", EnumLogLevel.Info, false);
                //    }
                //    else
                //    {
                //        LogMessage("写入箱子回退信息给OPC NotifyOpcBackInException数据 888 失败!：", EnumLogLevel.Error, false);
                //    }

                //    return resResponseResult.ToString();
                //}
                //wmsData.DATA = para;
                ////TEST
                ////2、调用WMS接口进行上报校验(同步)
                //string cmdPara = JsonConvert.SerializeObject(wmsData);
                //NotifyElement element = new NotifyElement(para.PackageBarcode, "NotifyPackageBarcodeCheck", "扫码数据上报", null, cmdPara);
                //OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                //if (!result.IsSuccess || result.Content == null)
                //{
                //    string msg = string.Format("调用上层接口失败，详情：\r\n {0}", result.Message);
                //    LogMessage(msg, EnumLogLevel.Error, true);

                //    //写入888
                //    DeviceBaseAbstract curDevice2020 = DeviceManage.Instance.FindDeivceByDeviceId(2020);
                //    TransportPointStation transDevice2020 = curDevice2020 as TransportPointStation;
                //    RollerDeviceControl roller2020 = transDevice2020.DeviceControl as RollerDeviceControl;
                //    OperateResult opcResult2020 = roller2020.Communicate.Write(DataBlockNameEnum.NotifyOpcBackInException, 888);
                //    if (opcResult2020.IsSuccess)
                //    {
                //        LogMessage("写入箱子回退信息给OPC NotifyOpcBackInException数据 888 成功：", EnumLogLevel.Info, false);
                //    }
                //    else
                //    {
                //        LogMessage("写入箱子回退信息给OPC NotifyOpcBackInException数据 888 失败!：", EnumLogLevel.Error, false);
                //    }

                //    //测试用 待删除
                //    if (isTest)
                //    {
                //        resResponseResult.error = 0;
                //        resResponseResult.message = "";
                //        resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //        {
                //            sku_id = "1",//sku_id
                //            height = 1,
                //            length = 1,
                //            weight = 1,
                //            width = 1
                //        };
                //        LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
                //        return resResponseResult.ToString();
                //    }
                //    resResponseResult.error = 2;
                //    resResponseResult.message = result.Message;
                //    resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //    {
                //        sku_id = "",//sku_id
                //        height = 1,
                //        length = 1,
                //        weight = 1,
                //        width = 1
                //    };
                //    LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
                //    return resResponseResult.ToString();
                //}
               
                ////3、将WMS同步返回的数据，返回给机器人
                ////解析WMS返回的PackageId
                //NotifyPackageBarcodeCheckResponse packgeBarcodeResponse = JsonConvert.DeserializeObject<NotifyPackageBarcodeCheckResponse>(result.Content.ToString());
                //if (packgeBarcodeResponse.RESULT==1)
                //{
                //    if (isPackOut)
                //    {
                //        resResponseResult.error = 2;
                //        resResponseResult.message = "盘点任务，WCS默认盘点扫描异常！";
                //        resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //        {
                //            sku_id = packgeBarcodeResponse.DATA.PackageId.ToString(),//sku_id
                //            height = 1,
                //            length = 1,
                //            weight = 1,
                //            width = 1
                //        };
                //        LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
                //        return resResponseResult.ToString();
                //    }
                //    //测试用 待删除
                //    if (isTest)
                //    {
                //        resResponseResult.error = 0;
                //        resResponseResult.message = "";
                //        resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //        {
                //            sku_id = "1",//sku_id
                //            height = 1,
                //            length = 1,
                //            weight = 1,
                //            width = 1
                //        };
                //        LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
                //        return resResponseResult.ToString();
                //    }

                //    resResponseResult.error = 0;
                //    resResponseResult.message = packgeBarcodeResponse.MESSAGE;
                //    resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //    {
                //        sku_id = packgeBarcodeResponse.DATA.PackageId.ToString(),//sku_id
                //        height = 1,
                //        length = 1,
                //        weight = 1,
                //        width = 1
                //    };
                //    LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
                //    return resResponseResult.ToString();
                //}
                //else
                //{
                //    //测试用 待删除
                //    if (isTest)
                //    {
                //        resResponseResult.error = 0;
                //        resResponseResult.message = "";
                //        resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //        {
                //            sku_id = "1",//sku_id
                //            height = 1,
                //            length = 1,
                //            weight = 1,
                //            width = 1
                //        };
                //        LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
                //        return resResponseResult.ToString();
                //    }

                //    resResponseResult.error = 2;
                //    resResponseResult.message = packgeBarcodeResponse.MESSAGE;
                //    resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                //    {
                //        sku_id = packgeBarcodeResponse.DATA.PackageId.ToString(),//sku_id
                //        height = 1,
                //        length = 1,
                //        weight = 1,
                //        width = 1
                //    };
                //}
            }
            catch (Exception ex)
            {
                //测试用 待删除
                if (isTest)
                {
                    resResponseResult.error = 0;
                    resResponseResult.message = "";
                    resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                    {
                        sku_id = "1",//sku_id
                        height = 1,
                        length = 1,
                        weight = 1,
                        width = 1
                    };
                    LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
                    return resResponseResult.ToString();
                }
                resResponseResult.sku_info = new XYZABB.XYZABBApiService.CmdModel.skuInfo
                {
                    sku_id = "",//sku_id
                    height = 1,
                    length = 1,
                    weight = 1,
                    width = 1
                };
                resResponseResult.error = 2;
                resResponseResult.message = ex.ToString();
            }
            LogMessage("扫码返回机器人数据：" + resResponseResult.ToString(), EnumLogLevel.Info, false);
            return resResponseResult.ToString();
        }

     
        /// <summary>
        /// 3、机器人请求WCS来料托盘退库接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string report_task_status(Report_Task_StatusCmd cmd)
        {
            ResponseResult resResponseResult = new ResponseResult();
            try
            {
                string logMsg = string.Format("接收到ABB上报的report_task_status接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);
                int nodeId = 0;
                if (string.IsNullOrEmpty(cmd.task_id))
                {
                    resResponseResult.error = 2;
                    resResponseResult.error_message = "任务号不能为空";
                    return resResponseResult.ToString();
                }
                if(cmd.area_code.Equals("01"))
                {
                    //1、查询未完成指令，丢弃，并生成退库指令
                    DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(1007);
                    TransportPointStation transDevice = curDevice as TransportPointStation;
                    RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                    OperateResult<int> opcResult1007 = roller.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    if (opcResult1007.IsSuccess)
                    {
                        if (cmd.task_id.Equals(opcResult1007.Content.ToString()))
                        {
                            nodeId = 1007;
                        }
                        else
                        {
                            DeviceBaseAbstract curDevice1004 = DeviceManage.Instance.FindDeivceByDeviceId(1004);
                            TransportPointStation transDevice1004 = curDevice1004 as TransportPointStation;
                            RollerDeviceControl roller1004 = transDevice1004.DeviceControl as RollerDeviceControl;
                            OperateResult<int> opcResult1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                            if (opcResult1004.IsSuccess)
                            {
                                if (cmd.task_id.Equals(opcResult1004.Content.ToString()))
                                {
                                    nodeId = 1004;
                                }
                            }
                            else
                            {
                                //读取1004 OPC失败
                                string msg = string.Format("读取1004 包号 OPC失败", sim3DDeviceId);
                                LogMessage(msg, EnumLogLevel.Error, true);
                            }
                        }
                    }
                    else
                    {
                        //读取1007 OPC失败
                        string msg = string.Format("读取1007 包号 OPC失败", sim3DDeviceId);
                        LogMessage(msg, EnumLogLevel.Error, true);
                    }

                    //生成回退指令
                    Order order = new Order();
                    if (nodeId == 1007 || nodeId == 1004)
                    {
                        DeviceBaseAbstract deviceNode = DeviceManage.Instance.FindDeivceByDeviceId(nodeId);
                        TransportPointStation transDeviceNode = deviceNode as TransportPointStation;
                        RollerDeviceControl rollerNode = transDeviceNode.DeviceControl as RollerDeviceControl;
                        OperateResult opcResultNode = rollerNode.Communicate.Write(DataBlockNameEnum.WriteDirectionDataBlock, 1001);
                        if (opcResultNode.IsSuccess)
                        {
                            string msg = string.Format("退库操作: 写入 设备ID：{0} 目标地址 OPC成", nodeId);
                            LogMessage(msg, EnumLogLevel.Info, true);
                        }
                        else
                        {
                            string msg = string.Format("退库操作: 写入 设备ID：{0} 目标地址 OPC失败", nodeId);
                            LogMessage(msg, EnumLogLevel.Error, true);
                        }
                    }
                }
                else if (cmd.area_code.Equals("02"))
                {
                    ////调用机器人视觉 探货接口
                    //OperateResult<int> abbResult = CheckABBHaveGoods(3002, 0);
                    //if (abbResult.IsSuccess)
                    //{
                    //    //0 无货、111有货，未知数量 999异常
                    //    if(abbResult.Content==0)
                    //    {

                    //    }
                    //    else if (abbResult.Content == 111)
                    //    {

                    //    }
                    //    else if (abbResult.Content == 999)
                    //    {

                    //    }
                    //    else
                    //    {

                    //    }
                    //}
                    //else
                    //{

                    //}

                    //处理托盘回退  调用ABB视觉   无货调用 wms接口ApplyEmptyTrayIn  有货调用NotifyOutFinish 

                    //0 正常结束   99  98 条码故障， 97 强制结束    96 拣选
                    if(cmd.task_status==0)
                    {
                        //正常结束
                        DeviceBaseAbstract curDevice2020 = DeviceManage.Instance.FindDeivceByDeviceId(2020);
                        TransportPointStation transDevice2020 = curDevice2020 as TransportPointStation;
                        RollerDeviceControl roller2020 = transDevice2020.DeviceControl as RollerDeviceControl;
                        OperateResult<int> opcResult2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                        if (opcResult2020.IsSuccess)
                        {
                            if (cmd.task_id.Equals(opcResult2020.Content.ToString()))
                            {
                                nodeId = 2001;
                                OperateResult<int> opcDestDataResult2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.WriteDirectionDataBlock);
                                if (opcDestDataResult2020.IsSuccess)
                                {
                                    DeviceBaseAbstract curDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
                                    TransportPointStation transDevice2001 = curDevice2001 as TransportPointStation;
                                    RollerDeviceControl roller2001 = transDevice2001.DeviceControl as RollerDeviceControl;
                                    OperateResult<int> opcResult2001 = roller2001.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                                    if (opcResult2001.IsSuccess)
                                    {
                                        if (opcDestDataResult2020.Content == opcResult2001.Content)
                                        {
                                            //1、调用WMS上报完成接口
                                            OperateResult<string> opcBarCodeResult2001 = roller2001.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

                                            OperateResult opResult = NotifyWmsOutFinish("GetGoodsPort:3_1_1");
                                            if (opResult.IsSuccess)
                                            {
                                                LogMessage("通知WMS出库完成 成功！", EnumLogLevel.Info, false);
                                                resResponseResult.error = 0;
                                                resResponseResult.error_message = "生成回退指令成功，回退信息";
                                            }
                                            else
                                            {
                                                LogMessage("通知WMS出库完成 失败！", EnumLogLevel.Error, false);
                                                resResponseResult.error = 0;
                                                resResponseResult.error_message = "生成回退指令成功，回退信息";
                                            }
                                          
                                            //2、申请空托盘入库
                                            Thread.Sleep(400);

                                            //通知plc出库 清除 2015
                                            DeviceBaseAbstract curDevice2015 = DeviceManage.Instance.FindDeivceByDeviceId(2015);
                                            TransportPointStation transDevice2015 = curDevice2015 as TransportPointStation;
                                            RollerDeviceControl roller2015 = transDevice2015.DeviceControl as RollerDeviceControl;
                                            //2007、2009、2010   包号不等于0，不写2015 888  TO DO
                                            OperateResult opcWriteOrderIdResult2015 = roller2015.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, 888);
                                            if (opcWriteOrderIdResult2015.IsSuccess)
                                            {
                                                LogMessage("写入2015 opc 包号 888 全部出货到2008成功！", EnumLogLevel.Info, false);
                                                //清空2019 数据
                                                DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                                                TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                                                RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;

                                                OperateResult opcWriteOrderIdResult2019 = roller2019.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, 0);
                                                if (opcWriteOrderIdResult2019.IsSuccess)
                                                {
                                                    LogMessage("写入2019 opc 包号 0 成功！", EnumLogLevel.Info, false);
                                                }
                                                else
                                                {
                                                    LogMessage("写入2019 opc 包号 0 失败！", EnumLogLevel.Error, false);
                                                }
                                                OperateResult opcWriteBarCodeResult2019 = roller2019.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                                                if (opcWriteOrderIdResult2019.IsSuccess)
                                                {
                                                    LogMessage("写入2019 opc 条码 0 成功！", EnumLogLevel.Info, false);
                                                }
                                                else
                                                {
                                                    LogMessage("写入2019 opc 条码 0 失败！", EnumLogLevel.Error, false);
                                                }
                                            }
                                            else
                                            {
                                                LogMessage("写入2015 opc 包号 888 全部出货到2008失败", EnumLogLevel.Error, false);
                                            }
                                        }
                                        else
                                        {
                                            LogMessage("读取2020 目标地址 OPC 失败！", EnumLogLevel.Error, false);
                                        }
                                    }
                                    else
                                    {
                                        LogMessage("读取2020目标地址 与2001任务号 指令ID 不一致！", EnumLogLevel.Error, false);
                                    }
                                }
                                else
                                {
                                    LogMessage("读取2001 指令ID OPC 失败！", EnumLogLevel.Error, false);
                                }
                            }
                        }
                    }
                    else if (cmd.task_status == 99 || cmd.task_status == 98)
                    {
                        //条码故障 不匹配
                       OperateResult opResult= NotifyWmsOutFinish("GetGoodsPort:3_1_1");
                       if (opResult.IsSuccess)
                       {
                           LogMessage("通知WMS出库完成 成功！", EnumLogLevel.Info, false);
                           resResponseResult.error = 0;
                           resResponseResult.error_message = "生成回退指令成功，回退信息";
                           return resResponseResult.ToString();
                       }
                       else
                       {
                           LogMessage("通知WMS出库完成 失败！", EnumLogLevel.Error, false);
                           resResponseResult.error = 0;
                           resResponseResult.error_message = "生成回退指令成功，回退信息";
                           return resResponseResult.ToString();
                       }
                    }
                    else if (cmd.task_status == 97)
                    {
                        //强制结束
                        //申请空托盘入库
                        OperateResult opResult = NotifyWmsOutFinish("GetGoodsPort:3_1_1");
                        if (opResult.IsSuccess)
                        {
                            //申请空托盘入库
                            resResponseResult.error = 0;
                            resResponseResult.error_message = "生成回退指令成功，回退信息";
                            return resResponseResult.ToString();
                        }
                        else
                        {
                            LogMessage("通知WMS出库完成 失败！", EnumLogLevel.Error, false);
                            //申请空托盘入库
                            resResponseResult.error = 0;
                            resResponseResult.error_message = "生成回退指令成功，回退信息";
                            return resResponseResult.ToString();
                        }
                    }
                    else if (cmd.task_status == 96)
                    {
                        //拣选
                        OperateResult opResult = NotifyWmsOutFinish("GetGoodsPort:3_1_1");
                        if (opResult.IsSuccess)
                        {
                            //通知plc出库 清除 2015
                            DeviceBaseAbstract curDevice2015 = DeviceManage.Instance.FindDeivceByDeviceId(2015);
                            TransportPointStation transDevice2015 = curDevice2015 as TransportPointStation;
                            RollerDeviceControl roller2015 = transDevice2015.DeviceControl as RollerDeviceControl;
                            //2007、2009、2010   包号不等于0，不写2015 888  TO DO
                            OperateResult opcWriteOrderIdResult2015 = roller2015.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, 888);
                            if (opcWriteOrderIdResult2015.IsSuccess)
                            {
                                LogMessage("写入2015 opc 包号 888 全部出货到2008成功！", EnumLogLevel.Info, false);
                                //清空2019 数据
                                DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                                TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                                RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;

                                OperateResult opcWriteOrderIdResult2019 = roller2019.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, 0);
                                if (opcWriteOrderIdResult2019.IsSuccess)
                                {
                                    LogMessage("写入2019 opc 包号 0 成功！", EnumLogLevel.Info, false);
                                }
                                else
                                {
                                    LogMessage("写入2019 opc 包号 0 失败！", EnumLogLevel.Error, false);
                                }
                                OperateResult opcWriteBarCodeResult2019 = roller2019.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                                if (opcWriteOrderIdResult2019.IsSuccess)
                                {
                                    LogMessage("写入2019 opc 条码 0 成功！", EnumLogLevel.Info, false);
                                }
                                else
                                {
                                    LogMessage("写入2019 opc 条码 0 失败！", EnumLogLevel.Error, false);
                                }
                            }
                            else
                            {
                                LogMessage("写入2015 opc 包号 888 全部出货到2008失败", EnumLogLevel.Error, false);
                            }



                            //申请空托盘入库
                            resResponseResult.error = 0;
                            resResponseResult.error_message = "生成回退指令成功，回退信息";
                            return resResponseResult.ToString();
                        }
                        else
                        {
                            LogMessage("通知WMS出库完成 失败！", EnumLogLevel.Error, false);
                            //申请空托盘入库
                            resResponseResult.error = 0;
                            resResponseResult.error_message = "生成回退指令成功，回退信息";
                            return resResponseResult.ToString();
                        }
                    }
                }
              
                //存在该任务
                //2、"target_num": -1 调用3D仿真 通知清空货物
                if (cmd.target_num == -1)
                {
                    //调用仿真接口 转发给模拟3D仿真
                    DeviceBaseAbstract xDevice = DeviceManage.Instance.FindDeivceByDeviceId(sim3DDeviceId);
                    if (xDevice == null)
                    {
                        string msg = string.Format("查找不到设备ID：{0} 的设备，请核实设备信息", sim3DDeviceId);
                        LogMessage(msg, EnumLogLevel.Error, true);
                    }
                    Simulate3D transDevice3D = xDevice as Simulate3D;
                    Simulate3DControl roller3D = transDevice3D.DeviceControl as Simulate3DControl;
                    string deviceCode = "";
                    if (nodeId != 0)
                    {
                        deviceCode = nodeId.ToString();
                    }

                    if (!string.IsNullOrEmpty(deviceCode))
                    {
                        ClearGoodsCmd clearCmd = new ClearGoodsCmd { DEVICECODE = deviceCode }; //（1004,1007,2001）
                        OperateResult opResult = roller3D.ClearGoodsDataUpload(clearCmd);
                        if (opResult.IsSuccess)
                        {
                            LogMessage("调用3D仿真接口成功： ClearGoodsDataUpload", EnumLogLevel.Info, true);
                        }
                        else
                        {
                            LogMessage("调用3D仿真接口失败： ClearGoodsDataUpload" + opResult.Message, EnumLogLevel.Info,
                                true);
                        }
                    }
                }

                resResponseResult.error = 0;
                resResponseResult.error_message = "生成回退指令成功，回退信息";
                return resResponseResult.ToString();
            }
            catch (Exception ex)
            {
                resResponseResult.error = 2;
                resResponseResult.error_message = ex.ToString();
            }
            return resResponseResult.ToString();
        }

        /// <summary>
        /// 通过机器人视觉检测出入库 
        /// </summary>
        /// <param name="abbDeviceID">3001、3002</param>
        /// <param name="postionNum">扫码的位置号</param>
        /// <returns></returns>
        private OperateResult<int> CheckABBHaveGoods(int abbDeviceID, int postionNum)
        {
            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(abbDeviceID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;
            string tempWs_id = "";
            if (abbDeviceID == 3002)
            {
                tempWs_id = "0";
            }
            else if (abbDeviceID == 3001)
            {
                tempWs_id = "1";// 有多个位置 待接口 增加位置号（需与机器人协商）
            }

            is_pallet_emptyMode emptyMode = new is_pallet_emptyMode { ws_id = tempWs_id };
            //return abbControl.is_pallet_empty(emptyMode);  TO DO

            return new OperateResult<int>();
        }

        public OperateResult NotifyWmsOutFinish(string addr)
        {

            OperateResult opResult = OperateResult.CreateFailedResult("默认失败");

            NotifyOutFinishParaMode parmData = new NotifyOutFinishParaMode
            {
                Addr = addr,
               
            };
            NotifyOutFinishCmdMode mode = new NotifyOutFinishCmdMode
            {
                DATA = parmData
            };

            string cmdPara = JsonConvert.SerializeObject(mode);
            string interfaceName = "NotifyOutFinish";
            NotifyElement element = new NotifyElement("", interfaceName, "出库完成上报", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                opResult.IsSuccess = false;
            }
            else
            {
                string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                opResult.IsSuccess = true;
            }
            opResult.Message = result.Message;
            return opResult;
        }


        /// <summary>
        /// 4、回报WCS单次码垛结果接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string report_action_status(Report_Action_StatusCmd cmd)
        {
            ResponseResult resResponseResult = new ResponseResult();
            try
            {
                string logMsg = string.Format("接收到ABB上报的report_action_status接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);
             
                if(cmd.area_code.Equals("02"))
                {
                    //ABB通知第一次码箱完成，写2007包号 ，当前的包号由2019提供
                    DeviceBaseAbstract curDevice2007 = DeviceManage.Instance.FindDeivceByDeviceId(2007);
                    TransportPointStation transDevice2007 = curDevice2007 as TransportPointStation;
                    RollerDeviceControl roller2007 = transDevice2007.DeviceControl as RollerDeviceControl;
                    OperateResult<int> opReadOrderId2007 = roller2007.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    if(opReadOrderId2007.IsSuccess)
                    {
                        if (opReadOrderId2007.Content == 0)
                        {
                            //无货 需要从2019 节点 获取当前包号
                            DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                            TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                            RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                            OperateResult<int> opReadOrderId2019 = roller2019.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                            if (opReadOrderId2019.IsSuccess)
                            {
                                if (opReadOrderId2019.Content != 0)
                                {
                                    OperateResult opWriteOrderID2007 = roller2007.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, opReadOrderId2019.Content);
                                 if(opWriteOrderID2007.IsSuccess)
                                 {
                                      LogMessage("写2007任务号成功，任务号："+opReadOrderId2019.Content.ToString(), EnumLogLevel.Info, false);
                                 }
                                 else
                                 {
                                      LogMessage("写2007任务号失败，待写任务号："+opReadOrderId2019.Content.ToString(), EnumLogLevel.Error, false);
                                 }
                                }
                                else
                                {
                                    LogMessage("读取2019 包号为0，不得下发，请检查2019是否下发拆垛任务给ABB", EnumLogLevel.Error, false);
                                }
                            }
                            else
                            {
                                LogMessage("读取2019 包号OPC 失败", EnumLogLevel.Error, false);
                            }
                        }
                    }
                    else
                    {
                        LogMessage("读取2007 包号OPC 失败", EnumLogLevel.Error, false);
                    }
                    //调用wms接口 NotifyPackagePutFinishForOut

                    //NotifyPackagePutFinishNewModel putFinishModel = new NotifyPackagePutFinishNewModel(); 新接口

                    NotifyPackagePutFinishForOutMode outfinishMode = new NotifyPackagePutFinishForOutMode
                    {
                        DATA = new NotifyPackagePutFinishForOutDataMode
                        {
                            PalletBarcode = "",//出库机器人不给条码
                            PosIndexs = cmd.customized_result.pick_position
                        }
                    };
                    string outCmdPara = JsonConvert.SerializeObject(outfinishMode);
                    string outInterfaceName = "NotifyPackagePutFinishForOut";
                    NotifyElement outElement = new NotifyElement("", outInterfaceName, "出库 单次码垛完成上报", null, outCmdPara);
                    OperateResult<object> outResult = UpperServiceHelper.WmsServiceInvoke(outElement);
                    if (!outResult.IsSuccess)
                    {
                        string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", outInterfaceName, outResult.Message);
                        resResponseResult.error = 2;
                        resResponseResult.error_message = outResult.Message;
                    }
                    else
                    {
                        string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", outInterfaceName, outResult.Message);
                        resResponseResult.error = 0;
                        resResponseResult.error_message = "上报成功！";
                    }
                    return resResponseResult.ToString();
                }

                NotifyPackagePutFinishMode finishMode = new NotifyPackagePutFinishMode
                {
                    DATA = new NotifyPackagePutFinishDataMode
                    {
                        Addr = "PutGoodsPort:1_1_1",
                        PackageId = int.Parse(cmd.customized_result.barcode[0]),
                        PosIndex = cmd.customized_result.place_position.Count == 0 ? 0 : cmd.customized_result.place_position[0]
                    }
                };
                string cmdPara = JsonConvert.SerializeObject(finishMode);
                string interfaceName = "NotifyPackagePutFinish";
                NotifyElement element = new NotifyElement("", interfaceName, "入库 单次码垛完成上报", null, cmdPara);
                OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                if (!result.IsSuccess)
                {
                    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                    resResponseResult.error = 2;
                    resResponseResult.error_message = result.Message;
                }
                else
                {
                    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                    resResponseResult.error = 0;
                    resResponseResult.error_message = "上报成功！";
                }
            }
            catch (Exception ex)
            {
                resResponseResult.error = 2;
                resResponseResult.error_message = ex.ToString();
            }
            return resResponseResult.ToString();
        }

        /// <summary>
        /// 5、通知WCS码垛箱已满接口（请求wcs入库码垛托盘）
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string notice_place_ws_is_full(Notice_Place_Ws_Is_FullCmd cmd)
        {
            ResponseResult resResponseResult = new ResponseResult();
            try
            {
                string logMsg = string.Format("接收到ABB上报的notice_place_ws_is_full接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);
                if (cmd.area_code.Equals("01"))
                {
                    //转换WMS接口参数传递 
                    DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                    TransportPointStation transDevice = curDevice as TransportPointStation;
                    RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                    OperateResult<string> opcResult1008 = roller.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

                    int weight = 0;
                    OperateResult<int> goodsWeightResult = roller.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);
                    if (goodsWeightResult.IsSuccess)
                    {
                        weight = goodsWeightResult.Content;
                    }
                    PalletizerFinishParaMode parmData = new PalletizerFinishParaMode
                    {
                        ADDR = "PutGoodsPort:1_1_1",
                        PALLETBARCODE = "",
                        Weight = weight
                    };
                    if (opcResult1008.IsSuccess && !string.IsNullOrEmpty(opcResult1008.Content))
                    {
                        parmData.PALLETBARCODE = opcResult1008.Content;
                    }

                    NotifyPalletizerFinishCmdMode mode = new NotifyPalletizerFinishCmdMode
                    {
                        DATA = parmData
                    };

                    string cmdPara = JsonConvert.SerializeObject(mode);
                    string interfaceName = "NotifyPalletizerFinish";
                    NotifyElement element = new NotifyElement("", interfaceName, "入库码盘完成上报", null, cmdPara);
                    OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                    if (!result.IsSuccess)
                    {
                        string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                        resResponseResult.error = 2;
                        resResponseResult.error_message = result.Message;
                    }
                    else
                    {
                        string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                        resResponseResult.error = 0;
                        resResponseResult.error_message = "上报码垛箱已满接口成功!";
                    }
                }
                else if (cmd.area_code.Equals("02"))
                {
                    //出库调度
                    DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(2007);
                    TransportPointStation transDevice = curDevice as TransportPointStation;
                    RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                    OperateResult<int> opcOrderIdResult = roller.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
                    if (opcOrderIdResult.IsSuccess)
                    {
                        if (opcOrderIdResult.Content != 0)
                        {
                            //清空 2019 条码 为空
                            DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                            TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                            RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                            OperateResult opWriteBarCodeResult = roller2019.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                            if(opWriteBarCodeResult.IsSuccess)
                            {
                                LogMessage("清空2019 opc条码成功！ ", EnumLogLevel.Info, false);
                            }
                            else
                            {
                                LogMessage("清空2019 opc条码失败！", EnumLogLevel.Error, false);
                            }

                            OperateResult opWriteDestDataResult = roller.Communicate.Write(DataBlockNameEnum.WriteDirectionDataBlock, 2007);
                            if (opWriteDestDataResult.IsSuccess)
                            {
                                //通知PLC 往前行走一格成功
                                LogMessage("通知PLC  2007 往前行走一格成功", EnumLogLevel.Info, false);
                                resResponseResult.error = 0;
                                resResponseResult.error_message = "通知PLC往前走一步成功！";
                            }
                            else
                            {
                                //写入2007 目标失败
                                LogMessage("写 2007 opc目标位 2007失败 ", EnumLogLevel.Error, false);
                                resResponseResult.error = 0;
                                resResponseResult.error_message = "写 2007 opc目标位 2007失败！";
                            }
                        }
                        else
                        {
                            //2007 无货 ，
                            LogMessage("当前2007位无货", EnumLogLevel.Error, false);
                            resResponseResult.error = 0;
                            resResponseResult.error_message = "当前2007位无货！";
                        }
                    }
                    else
                    {
                        LogMessage("读取 2007 opc包号失败 ", EnumLogLevel.Error, false);
                        resResponseResult.error = 0;
                        resResponseResult.error_message = "读取 2007 opc包号失败！";
                    }
                }
                else
                {
                    resResponseResult.error = 2;
                    resResponseResult.error_message = "不存在的 area_code：" + cmd.area_code;
                }
            }
            catch (Exception ex)
            {
                resResponseResult.error = 2;
                resResponseResult.error_message = ex.ToString();
            }
            return resResponseResult.ToString();
        }

        /// <summary>
        /// 6、上报WCS机器人工作状态接口（仿真）OK
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string notice_system_status(Notice_System_StatusCmd cmd)
        {
            ResponseEmptyResult resResponseResult = new ResponseEmptyResult();
            try
            {
                string logMsg = string.Format("接收到ABB上报的notice_system_status接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, false);

                if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                {
                    //resResponseResult.error = 2;
                    //resResponseResult.error_message = "传入的area_code错误:" + cmd.area_code;
                    //return resResponseResult.ToString();
                }
                if (cmd.error.Equals(0) || cmd.error.Equals(1))
                {
                    //resResponseResult.error = 2;
                    //resResponseResult.error_message = "传入的error错误:" + cmd.error;
                    //return resResponseResult.ToString();
                }
                //调用仿真接口
                //转发给模拟3D仿真
                DeviceBaseAbstract xDevice = DeviceManage.Instance.FindDeivceByDeviceId(sim3DDeviceId);
                if (xDevice == null)
                {
                    string msg = string.Format("查找不到设备ID：{0} 的设备，请核实设备信息", sim3DDeviceId);
                    LogMessage(msg, EnumLogLevel.Error, true);
                }
                Simulate3D transDevice = xDevice as Simulate3D;
                Simulate3DControl roller = transDevice.DeviceControl as Simulate3DControl;
                GoodsTrackDataUploadCmd actionCmd = GetRobotTrackDataUploadCmd(cmd.area_code,
                    cmd.return_data.work_status);
                OperateResult opResult = roller.RobotTrackDataUpload(actionCmd);
                if (opResult.IsSuccess)
                {
                    LogMessage("调用3D仿真接口成功： RobotTrackDataUpload", EnumLogLevel.Info, false);
                }
                else
                {
                    LogMessage("调用3D仿真接口失败： RobotTrackDataUpload" + opResult.Message, EnumLogLevel.Error, false);
                }
                //resResponseResult.error = 0;
            }
            catch (Exception ex)
            {
                //resResponseResult.error = 2;
                //resResponseResult.error_message = ex.ToString();
            }
            return resResponseResult.ToString();
        }

        Dictionary<string, GoodsTrackDataUploadCmd> inDic = new Dictionary<string, GoodsTrackDataUploadCmd>();
        Dictionary<string, GoodsTrackDataUploadCmd> outDic = new Dictionary<string, GoodsTrackDataUploadCmd>();
        private void LoadBaseData()
        {
            //入库
            string In_A_Id = "1004";//取料位A
            string In_B_Id = "1007";//取料位B
            string In_R_Id = "10071";//扫描位R
            string In_S_Id = "1008";//放料位S
            string In_NG_Id = "10091";//NG台
            string In_O_Id = "10081";//机器人原点
            string In_DeviceId = "3001";//入库设备ID

            inDic.Add("0", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_O_Id,
                NEXTPOSITION = In_O_Id,
            });
            inDic.Add("1", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_O_Id,
                NEXTPOSITION = In_A_Id,
            });
            inDic.Add("2", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_A_Id,
                NEXTPOSITION = In_R_Id,
            });
            inDic.Add("3", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_R_Id,
                NEXTPOSITION = In_S_Id,
            });
            inDic.Add("4", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_R_Id,
                NEXTPOSITION = In_NG_Id,
            });
            inDic.Add("5", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_S_Id,
                NEXTPOSITION = In_A_Id,
            });
            inDic.Add("6", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_NG_Id,
                NEXTPOSITION = In_A_Id,
            });
            inDic.Add("7", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_S_Id,
                NEXTPOSITION = In_O_Id,
            });
            inDic.Add("8", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_NG_Id,
                NEXTPOSITION = In_O_Id,
            });
            inDic.Add("11", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_O_Id,
                NEXTPOSITION = In_B_Id,
            });
            inDic.Add("12", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_B_Id,
                NEXTPOSITION = In_R_Id,
            });
            inDic.Add("15", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_S_Id,
                NEXTPOSITION = In_B_Id,
            });
            inDic.Add("16", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_NG_Id,
                NEXTPOSITION = In_B_Id,
            });
            //出库
            string Out_O_Id = "20021";//机器人原点
            string Out_P_Id = "2001";//出库位
            string Out_R_Id = "20011";//扫描位R
            string Out_C_Id = "2007";//输送线C
            string Out_DeviceId = "3002";//出库设备ID
            outDic.Add("0", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_O_Id,
                NEXTPOSITION = Out_O_Id,
            });
            outDic.Add("1", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_O_Id,
                NEXTPOSITION = Out_P_Id,
            });
            outDic.Add("2", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_P_Id,
                NEXTPOSITION = Out_R_Id,
            });
            outDic.Add("3", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_R_Id,
                NEXTPOSITION = Out_C_Id,
            });
            outDic.Add("4", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_R_Id,
                NEXTPOSITION = Out_P_Id,
            });
            outDic.Add("5", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_P_Id,
                NEXTPOSITION = Out_O_Id,
            });
            outDic.Add("6", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_C_Id,
                NEXTPOSITION = Out_P_Id,
            });
            outDic.Add("7", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_P_Id,
                NEXTPOSITION = Out_C_Id,
            });
            outDic.Add("8", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_C_Id,
                NEXTPOSITION = Out_O_Id,
            });
        }


        private GoodsTrackDataUploadCmd GetRobotTrackDataUploadCmd(string areaCode, string status)
        {
            return areaCode.Equals("01") ? inDic[status] : outDic[status];
        }

        /// <summary>
        /// 7、上报WCS机器人工作状态接口（仿真） OK
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string report_exception(Report_ExceptionCmd cmd)
        {
            ResponseResult resResponseResult = new ResponseResult();
            try
            {
                string logMsg = string.Format("接收到ABB上报的report_exception接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);

                if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                {
                   
                }
                else
                {
                    resResponseResult.error = 2;
                    resResponseResult.error_message = "传入的area_code错误:" + cmd.area_code;
                    return resResponseResult.ToString();
                }
                //调用仿真接口
                //转发给模拟3D仿真
                DeviceBaseAbstract xDevice = DeviceManage.Instance.FindDeivceByDeviceId(sim3DDeviceId);
                if (xDevice == null)
                {
                    string msg = string.Format("查找不到设备ID：{0} 的设备，请核实设备信息", sim3DDeviceId);
                    LogMessage(msg, EnumLogLevel.Error, true);
                }
                Simulate3D transDevice = xDevice as Simulate3D;
                Simulate3DControl roller = transDevice.DeviceControl as Simulate3DControl;
                DeviceStautsDataUploadCmd sendData = new DeviceStautsDataUploadCmd
                {
                    DEVICECODE = cmd.area_code.Equals("01") ? "3001" : "3002",
                    ONLINE_STATUS = 1,
                    JOB_STATUS = 2,
                    EXCEPTION_STATUS = 2,
                    EXCEPTION_CODE =cmd.error_code,
                    EXCEPTION_MSG = cmd.error_msg,
                    EXT1 = "",
                    EXT2 =""
                };
                OperateResult opResult = roller.DeviceStautsDataUpload(sendData);
                if (opResult.IsSuccess)
                {
                    LogMessage("调用3D仿真接口成功： DeviceStautsDataUpload", EnumLogLevel.Info, true);
                }
                else
                {
                    LogMessage("调用3D仿真接口失败： DeviceStautsDataUpload" + opResult.Message, EnumLogLevel.Info, true);
                }
                resResponseResult.error = 0;
            }
            catch (Exception ex)
            {
                resResponseResult.error = 2;
                resResponseResult.error_message = ex.ToString();
            }
            return resResponseResult.ToString();
        }

        /// <summary>
        /// 触发扫描枪业务 同步返回
        /// </summary>
        /// <param name="areCode"></param>
        /// <returns></returns>
        private string GetScanSkuInfo(string areCode)
        {
            string code = "";
            int deviceID = areCode.Equals("01") ? 111101 : 222201;
            DeviceBaseAbstract device = DeviceManage.Instance.FindDeivceByDeviceId(deviceID);
            if (device is KeyenceScanner)
            {
                KeyenceScanner scanner = (KeyenceScanner)device;
                List<string> tempBarCodeList = new List<string>();
                OperateResult<List<string>> opResult = scanner.GetIdentifyMessageSync(tempBarCodeList);
                if (opResult.IsSuccess)
                {
                    //调用读取条码成功
                    if (opResult.Content != null) return opResult.Content[0];
                }
            }
            return code;
        }

        public string report_orderFinish(report_orderFinishCmd cmd)
        {
            throw new NotImplementedException();
        }
    }
}
