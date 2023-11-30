using System;
using System.IO;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Infrastructrue.Xml;
using Newtonsoft.Json;
using System.Reflection;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using System.Collections.Generic;
//using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    #region 机器人接口参数类(一期)
    /// <summary>
    /// 机器人下发任务返回信息
    /// </summary>
    public class XYZABB_single_class_depal_task_Response
    {
        /// <summary>
        /// 返回结果代码 (0: 成功   1: 异常 (无该任务)  2: 资料格式错误)
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string error_message { get; set; }

        public static explicit operator XYZABB_single_class_depal_task_Response(string json)
        {
            return JsonConvert.DeserializeObject<XYZABB_single_class_depal_task_Response>(json);
        }
    }
    /// <summary>
    /// 机器人就绪返回信息
    /// </summary>
    public class XYZABB_is_system_ready_Response
    {
        /// <summary>
        /// 表示系统状态值，0代表系统准备好，非0值表示没准备好或异常
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }

        public static explicit operator XYZABB_is_system_ready_Response(string json)
        {
            return JsonConvert.DeserializeObject<XYZABB_is_system_ready_Response>(json);
        }
    }


    public class XYZABBTaskMode
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string task_id { get; set; }
        public sku_infoMode sku_info { get; set; }
        /// <summary>
        /// 抓取工作空间号，与XYZ工作空间一致（入库为0,1，出库为0）
        /// </summary>
        public string from_ws { get; set; }

        /// <summary>
        /// 放置工作空间号，与XYZ工作空间一致（入库为2，出库为1）
        /// </summary>
        public string to_ws { get; set; }
        /// <summary>
        /// 是否去扫码，只在出库起作用
        /// </summary>
        public bool to_scanner { get; set; }
        /// <summary>
        /// 拆垛托盘号，入库托盘id没有，出库有托盘id
        /// </summary>
        public string pallet_id { get; set; }
    }

    public class XYZABB_is_system_readyMode
    {
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
    }


    public class notice_place_ws_readyMode
    {
        /// <summary>
        /// 拆垛任务的id，具有唯一性
        /// </summary>
        public string task_id { get; set; }
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 放置工作空间号，与XYZ工作空间一致（入库为2，出库为0）
        /// </summary>
        public string ws_id { get; set; }
        /// <summary>
        /// 码垛托盘号，(让XYZ回报托盘已满时夹带托盘id),出库没有id
        /// </summary>
        public string pallet_id { get; set; }
    }
    public class XYZABB_notice_place_ws_ready_Response
    {
        /// <summary>
        /// 返回结果代码 (0: 成功   1: 异常 (无该任务)  2: 资料格式错误)
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string error_message { get; set; }

        public static explicit operator XYZABB_notice_place_ws_ready_Response(string json)
        {
            return JsonConvert.DeserializeObject<XYZABB_notice_place_ws_ready_Response>(json);
        }
    }



    /// <summary>
    /// 强制结束码垛
    /// </summary>
    public class terminate_palletMode
    {
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 放置工作空间号，与XYZ工作空间一致（入库为2，出库为0）
        /// </summary>
        public string ws_id { get; set; }

        /// <summary>
        /// 码垛托盘号，(让XYZ回报托盘已满时夹带托盘id),出库没有id
        /// </summary>
        public string pallet_id { get; set; }

    }
    public class terminate_pallet_Response
    {
        /// <summary>
        /// 返回结果代码 (0: 成功   1: 异常 (无该任务)  2: 资料格式错误)
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string error_message { get; set; }

        public static explicit operator terminate_pallet_Response(string json)
        {
            return JsonConvert.DeserializeObject<terminate_pallet_Response>(json);
        }
    }


    /// <summary>
    /// 强制结束拆垛
    /// </summary>
    public class terminate_taskMode
    {
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 任务号
        /// </summary>
        public string task_id { get; set; }
    }
    public class terminate_task_Response
    {
        /// <summary>
        /// 返回结果代码 (0: 成功   1: 异常 (无该任务)  2: 资料格式错误)
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string error_message { get; set; }

        public static explicit operator terminate_task_Response(string json)
        {
            return JsonConvert.DeserializeObject<terminate_task_Response>(json);
        }
    }



    /// <summary>
    /// sku信息
    /// </summary>
    public class sku_infoMode
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string sku_id { get; set; }
        /// <summary>
        /// 商品长度（单位:公尺）
        /// </summary>
        public int length { get; set; }
        /// <summary>
        /// 商品宽度（单位:公尺）
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 商品高度（单位:公尺）
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// 商品重量（单位:公斤）
        /// </summary>
        public int weight { get; set; }
        /// <summary>
        /// 该次任务要处理的数量。-1则为把码垛盘码满
        /// </summary>
        public int sku_num { get; set; }

        /// <summary>
        /// sku_type:商品类型（预设字符串） sku类型 (box)
        /// </summary>
        public string sku_type { get; set; }
    }

    #endregion

    #region 机器人接口参数类（二期）
    public class GetInitCmd
    {
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        //public string area_code { get; set; }

    }
    /// <summary>
    /// 查询机器人是否在初始位信息
    /// </summary>
    public class GetInit_Response
    {
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public bool data { get; set; }

        public static explicit operator GetInit_Response(string json)
        {
            return JsonConvert.DeserializeObject<GetInit_Response>(json);
        }
    }



    public class QueryRobotStatus
    {
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
    }

    /// <summary>
    /// 机器人查询作业状态返回信息
    /// </summary>
    public class QueryRobotStatus_Response
    {
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }

        public static explicit operator QueryRobotStatus_Response(string json)
        {
            return JsonConvert.DeserializeObject<QueryRobotStatus_Response>(json);
        }
    }

    public class SendExeOrderMode
    {
        public SendExeOrderMode()
        {
            this.ext_data = new List<SendExeOrder_ext_data>();
        }
        /// <summary>
        /// 指令编号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 作业类型
        /// </summary>
        public string job_type { get; set; }
        /// <summary>
        /// 工作区域 01入库 02出库
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 起始地址 起始地址
        /// </summary>
        public string start_addr { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string dest_addr { get; set; }
        /// <summary>
        /// 抓取数量（机器人夹爪 启用个数）
        /// </summary>
        public int pick_num { get; set; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        public List<SendExeOrder_ext_data> ext_data { get; set; }
    }
    /// <summary>
    /// 扩展数据
    /// </summary>
    public class SendExeOrder_ext_data
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string index { get; set; }
        /// <summary>
        /// 箱条码
        /// </summary>
        public string box_barcode { get; set; }
        /// <summary>
        /// 抓取位置号
        /// </summary>
        public string start_position { get; set; }
    }

    public class SendExeOrder_Response
    {
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }

        public static explicit operator SendExeOrder_Response(string json)
        {
            return JsonConvert.DeserializeObject<SendExeOrder_Response>(json);
        }
    }

    public class InvokRobotVisionMode
    {
        /// <summary>
        /// 工作区域 01入库 02出库
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 视觉拍照地址
        /// </summary>
        public string visi_addr { get; set; }
    }

    public class InvokRobotVision_Response
    {
        //public InvokRobotVision_Response()
        //{
        //    this.data = new InvokRobotVision_Response_ParmsData();
        //}
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }

        public InvokRobotVision_Response_ParmsData data { get; set; }


        public static explicit operator InvokRobotVision_Response(string json)
        {
            return JsonConvert.DeserializeObject<InvokRobotVision_Response>(json);
        }
    }
    public class InvokRobotVision_Response_ParmsData
    {
        /// <summary>
        /// 是否已空 0不空，1空
        /// </summary>
        public int is_empty { get; set; }
        /// <summary>
        /// 参考数量(箱子)
        /// </summary>
        public int count { get; set; }
    }

    public class NoticeRobotReplaceCarrierMode
    {
        /// <summary>
        /// 工作区域 01入库 02出库
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 作业地址
        /// </summary>
        public string job_addr { get; set; }
    }
    public class NoticeRobotReplaceCarrier_Response
    {
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }



        public static explicit operator NoticeRobotReplaceCarrier_Response(string json)
        {
            return JsonConvert.DeserializeObject<NoticeRobotReplaceCarrier_Response>(json);
        }
    }


    public class GetNgStationMode
    {

    }
    public class GetNgStation_Response
    {
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int data { get; set; }



        public static explicit operator GetNgStation_Response(string json)
        {
            return JsonConvert.DeserializeObject<GetNgStation_Response>(json);
        }
    }

    public class QueryRobotWorkInfoMode 
    {
        /// <summary>
        /// 工作区域 01入库 02出库
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 指令编号
        /// </summary>
        public string order_no { get; set; }
    }
    public class QueryRobotWorkInfo_Response
    {
        public QueryRobotWorkInfo_Response()
        {
            this.data = new List<QueryRobotWorkInfo_Response_Data>();
        }
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }

        public List<QueryRobotWorkInfo_Response_Data> data { get; set; }

        public static explicit operator QueryRobotWorkInfo_Response(string json)
        {
            return JsonConvert.DeserializeObject<QueryRobotWorkInfo_Response>(json);
        }
    }
    public class QueryRobotWorkInfo_Response_Data
    {
        /// <summary>
        /// 作业指令状态
        /// </summary>
        public string order_status { get; set; }
        /// <summary>
        /// 工作状态
        /// </summary>
        public string work_status { get; set; }
        /// <summary>
        /// 工作状态标识
        /// </summary>
        public string work_flag { get; set; }
        /// <summary>
        /// 工作状态描述
        /// </summary>
        public string work_status_desc { get; set; }
    }


    public class DetelExeOrderMode
    {
        /// <summary>
        /// 工作区域 01入库 02出库
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 指令编号
        /// </summary>
        public string order_no { get; set; }
    }
    public class DetelExeOrder_Response
    {
        /// <summary>
        /// 表示系统状态值，1成功，0失败
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }


        public static explicit operator DetelExeOrder_Response(string json)
        {
            return JsonConvert.DeserializeObject<DetelExeOrder_Response>(json);
        }
    }


    #endregion
    public class XYZABBControl : OrderDeviceControlAbstract
    {
        #region 基础参数方法
        public string Http { get; set; }

        internal IWebNetInvoke WebNetInvoke;


        public override bool IsLoad()
        {
            return false;
        }

        public override OperateResult ClearFaultCode()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override Package GetCurrentPackage()
        {
            return null;
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            throw new NotImplementedException();
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcBoolValue monitervaluechange)
        {
            throw new NotImplementedException();
        }

        public override OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum dbBlockEnum, int startValue, int endValue, MonitorSpecifiedOpcValueCallback callbackAction)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 机器人接口调用 （一期）
        public OperateResult is_system_ready(string areaCode)
        {
            XYZABB_is_system_readyMode is_system_readyMode = new XYZABB_is_system_readyMode { area_code = areaCode };
            string is_system_ready_cmd = JsonConvert.SerializeObject(is_system_readyMode);
            OperateResult<string> abbIsReadyResponse = WebNetInvoke.ServiceRequest<XYZABBResSucMsg>(Http, "is_system_ready", is_system_ready_cmd);
            if (!abbIsReadyResponse.IsSuccess)
            {
                return OperateResult.CreateFailedResult(abbIsReadyResponse.Message, 1);
            }
            XYZABB_is_system_ready_Response abbIsReadyRespone = (XYZABB_is_system_ready_Response)abbIsReadyResponse.Content;
            if (abbIsReadyRespone.status != 0)
            {
                return OperateResult.CreateFailedResult(abbIsReadyRespone.message, 1);
            }
            return OperateResult.CreateSuccessResult();
        }
       


        public OperateResult single_class_depal_task(string areaCode, XYZABBTaskMode task)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            //2、下发机器人 任务
            try
            {

                OperateResult isSystemReady = is_system_ready(areaCode);
                if (!isSystemReady.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(isSystemReady.Message, 1);
                }
                string cmd = JsonConvert.SerializeObject(task);//SyncResReMsg
                OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg>(Http, "single_class_depal_task", cmd);
                if (!response.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(response.Message, 1);
                }
                string message = response.Content.Trim();
                try
                {
                    XYZABB_single_class_depal_task_Response abbRespone = (XYZABB_single_class_depal_task_Response)message;
                    if (abbRespone.error.Equals(0))
                    {
                        return OperateResult.CreateSuccessResult();
                    }
                    return OperateResult.CreateFailedResult(string.Format("XYZ机器人系统返回失败，失败原因：\r\n {0}", abbRespone.error_message), 1);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("XYZ机器人系统返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                }
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult notice_place_ws_ready(notice_place_ws_readyMode readyMode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
           

            string cmd = JsonConvert.SerializeObject(readyMode);
            OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg>(Http, "notice_place_ws_ready", cmd);
            if (!response.IsSuccess)
            {
                return OperateResult.CreateFailedResult(response.Message, 1);
            }
            string message = response.Content.Trim();
            try
            {
                XYZABB_notice_place_ws_ready_Response abbRespone = (XYZABB_notice_place_ws_ready_Response)message;
                if (!abbRespone.error.Equals(0))
                {
                    return OperateResult.CreateFailedResult(string.Format("XYZ机器人系统返回notice_place_ws_ready失败，失败原因：\r\n {0}", abbRespone.error_message), 1);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format("XYZ机器人系统返回notice_place_ws_ready的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                return result;
            }
            return OperateResult.CreateSuccessResult();
        }


        public OperateResult terminate_pallet(terminate_palletMode mode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            string cmd = JsonConvert.SerializeObject(mode);
            OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg>(Http, "terminate_pallet", cmd);
            if (!response.IsSuccess)
            {
                return OperateResult.CreateFailedResult(response.Message, 1);
            }
            string message = response.Content.Trim();
            try
            {
                terminate_pallet_Response abbRespone = (terminate_pallet_Response)message;
                if (!abbRespone.error.Equals(0))
                {
                    return OperateResult.CreateFailedResult(string.Format("XYZ机器人系统返回terminate_pallet失败，失败原因：\r\n {0}", abbRespone.error_message), 1);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format("XYZ机器人系统返回terminate_pallet的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                return result;
            }
            return OperateResult.CreateSuccessResult();
        }


        public OperateResult terminate_task(terminate_taskMode mode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            string cmd = JsonConvert.SerializeObject(mode);
            OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg>(Http, "terminate_task", cmd);
            if (!response.IsSuccess)
            {
                return OperateResult.CreateFailedResult(response.Message, 1);
            }
            string message = response.Content.Trim();
            try
            {
                terminate_task_Response abbRespone = (terminate_task_Response)message;
                if (!abbRespone.error.Equals(0))
                {
                    return OperateResult.CreateFailedResult(string.Format("XYZ机器人系统返回terminate_task失败，失败原因：\r\n {0}", abbRespone.error_message), 1);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format("XYZ机器人系统返回terminate_task的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                return result;
            }
            return OperateResult.CreateSuccessResult();
        }



        ///// <summary>
        ///// 视觉探货接口
        ///// </summary>
        ///// <param name="emptyMode"></param>
        ///// <returns>0 无货、111有货，未知数量 999异常</returns>
        //public OperateResult<int> is_pallet_empty(is_pallet_emptyMode emptyMode)
        //{
        //    OperateResult<int> result = new OperateResult<int>();
        //    try
        //    {
        //        string cmd = JsonConvert.SerializeObject(emptyMode);
        //        OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg>(Http, "is_pallet_empty", cmd);
        //        if (!response.IsSuccess)
        //        {
        //            result.Content = 999;
        //            result.IsSuccess = false;
        //            result.Message = response.Message;
        //            return result;
        //        }
        //        string message = response.Content.Trim();
        //        XYZABB_is_pallet_empty_Response abbRespone = (XYZABB_is_pallet_empty_Response)message;
        //        //--正常返回数量时为0，报错为-1，有物体但数量未知为1（此时result为-1）
        //        if (abbRespone.error.Equals(0))
        //        {
        //            result.Content = abbRespone.result;
        //            result.IsSuccess = true;
        //            result.Message = "调用机器人视觉接口成功，机器人error返回为0，正常返回数量";
        //            return result;
        //        }
        //        else if (abbRespone.error.Equals(1))
        //        {
        //            result.Content = 111;
        //            result.IsSuccess = true;
        //            result.Message = "调用机器人视觉接口成功，机器人error返回为1，有物体但数量未知";
        //            return result;
        //        }
        //        else if (abbRespone.error.Equals(-1))
        //        {
        //            result.Content = 999;
        //            result.IsSuccess = true;
        //            result.Message = "调用机器人视觉接口成功，机器人error返回为-1，报错，数量未知，具体原因：" + abbRespone.error_message;
        //            return result;
        //        }
        //        else
        //        {
        //            result.Content = 999;
        //            result.IsSuccess = false;
        //            result.Message = "机器人返回的ErrorCode 非 0、-1、1 实际为:" + abbRespone.error.ToString();
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Content = 999;
        //        result.IsSuccess = false;
        //        result.Message = string.Format("XYZ机器人系统返回is_pallet_empty格式转换出错，信息：{0} ", ex.Message);
        //        return result;
        //    }
        //}
        #endregion

        private bool IsDispatched = false;
        /// <summary>
        /// 调用机器人状态接口和下发任务接口
        /// </summary>
        /// <param name="transMsg"></param>
        /// <returns></returns>
        public override OperateResult DoJob(TransportMessage transMsg)
        {
            string areaCode = "";
            //1、读取机器人状态
            if (transMsg.StartAddr.FullName.Contains("GetGoodsPort:2_1_1")
                || transMsg.StartAddr.FullName.Contains("GetGoodsPort:1_1_1"))
            {
                areaCode = "01";
            }
            else if (transMsg.StartAddr.FullName.Contains("GetGoodsPort:3_1_1"))
            {
                areaCode = "02";
            }
            else
            {
                return OperateResult.CreateFailedResult("搬运指令起始地址不存在，请人工检查！" + transMsg.StartAddr.FullName, 1);
            }
            OperateResult result = OperateResult.CreateFailedResult();
            //if (areaCode.Equals("01"))
            //{
            //    //下发前 判断 放表位是否有货
            //    DeviceBaseAbstract curDevice = DeviceManage.DeviceManage.Instance.FindDeivceByDeviceId(1008);
            //    TransportPointStation transDevice = curDevice as TransportPointStation;
            //    RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
            //    OperateResult<int> opcResult1008 = roller.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
            //    if (opcResult1008.IsSuccess)
            //    {
            //        if (opcResult1008.Content <= 0)
            //        {
            //            //无货
            //            //判断是否存在申请WMS空箱接口  和出库到1008的出库指令
            //            if (IsDispatched == false)
            //            {
            //                roller.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, -1000);
            //                IsDispatched = true;
            //                return OperateResult.CreateFailedResult("已申请空托盘达到1008 放货位没有空托盘，等待托盘到位后下发指令给ABB机器人", 1);
            //            }

            //            return OperateResult.CreateFailedResult("1008 放货位没有空托盘，不能下发指令给ABB机器人", 1);
            //        }
            //        else
            //        {
            //            // 通知码垛托盘就绪
            //            //transMsg.OwnerId;
            //            IsDispatched = false;


            //            notice_place_ws_readyMode readyMode = new notice_place_ws_readyMode
            //            {
            //                area_code = areaCode,
            //                pallet_id = "",
            //                task_id = transMsg.TransportOrder.OrderId.ToString(),
            //                ws_id = "2"
            //            };

            //            string cmd = JsonConvert.SerializeObject(readyMode);
            //            OperateResult<string> response = WebNetInvoke.ServiceRequest<SyncResReMsg>(Http, "notice_place_ws_ready", cmd);
            //            if (!response.IsSuccess)
            //            {
            //                return OperateResult.CreateFailedResult(response.Message, 1);
            //            }
            //            string message = response.Content.Trim();
            //            try
            //            {
            //                XYZABB_notice_place_ws_ready_Response abbRespone = (XYZABB_notice_place_ws_ready_Response)message;
            //                if (!abbRespone.error.Equals(0))
            //                {
            //                    return OperateResult.CreateFailedResult(string.Format("XYZ机器人系统返回notice_place_ws_ready失败，失败原因：\r\n {0}", abbRespone.error_message), 1);
            //                }

            //            }
            //            catch (Exception ex)
            //            {
            //                result.IsSuccess = false;
            //                result.Message = string.Format("XYZ机器人系统返回notice_place_ws_ready的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
            //                return result;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return OperateResult.CreateFailedResult(opcResult1008.Message, 1);
            //    }
            //}

            //OperateResult isSystemReady = is_system_ready(areaCode);
            //if (!isSystemReady.IsSuccess)
            //{
            //    return OperateResult.CreateFailedResult(isSystemReady.Message, 1);
            //}

            ////2、下发机器人 任务
            //try
            //{
            //    sku_infoMode skuMode = new sku_infoMode
            //    {
            //        sku_id = "",
            //        length = 0,
            //        width = 0,
            //        height = 0,
            //        weight = 0,
            //        sku_num = -1,
            //        sku_type = "box"
            //    };
               
            //    XYZABBTaskMode task = new XYZABBTaskMode
            //    {
            //        task_id = transMsg.TransportOrder.OrderId.ToString(),
            //        sku_info = skuMode,
            //        pallet_id = "",
            //        to_scanner = true,
            //    };
            //    switch (transMsg.StartAddr.FullName)
            //    {
            //        case "GetGoodsPort:2_1_1":
            //            task.from_ws = "1";
            //            task.to_ws = "2";
            //            break;
            //        case "GetGoodsPort:1_1_1":
            //            task.from_ws = "0";
            //            task.to_ws = "2";
            //            break;
            //        case "GetGoodsPort:3_1_1":
            //            task.from_ws = "0";
            //            task.to_ws = "1";
            //            break;
            //        default:
            //            break;
            //    }
            //    string cmd = JsonConvert.SerializeObject(task);
            //    OperateResult<string> response = WebNetInvoke.ServiceRequest<SyncResReMsg>(Http, "single_class_depal_task", cmd);
            //    if (!response.IsSuccess)
            //    {
            //        return OperateResult.CreateFailedResult(response.Message, 1);
            //    }
            //    string message = response.Content.Trim();
            //    try
            //    {
            //        XYZABB_single_class_depal_task_Response abbRespone = (XYZABB_single_class_depal_task_Response)message;
            //        if (abbRespone.error.Equals(0))
            //        {
            //            return OperateResult.CreateSuccessResult();
            //        }
            //        return OperateResult.CreateFailedResult(string.Format("XYZ机器人系统返回失败，失败原因：\r\n {0}", abbRespone.error_message), 1);
            //    }
            //    catch (Exception ex)
            //    {
            //        result.IsSuccess = false;
            //        result.Message = string.Format("XYZ机器人系统返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
            //    }
            //    return result;

            //}
            //catch (Exception ex)
            //{
            //    result.IsSuccess = false;
            //    result.Message = OperateResult.ConvertException(ex);
            //}
            return result;
        }
        public override OperateResult<int> GetFinishedOrder()
        {
            int order = 0;
            return OperateResult.CreateFailedResult(order, "无数据");
        }
        #region 机器人接口调用（二期）

        /// <summary>
        /// 6.1	查询机器人作业状态 0失败，1成功
        /// </summary>
        /// <param name="areaCode">01 入库 02出库</param>
        /// <returns></returns>
        public OperateResult QueryRobotStatus(string areaCode)
        {
            QueryRobotStatus queryRobotStatusMode = new QueryRobotStatus { area_code = areaCode };
            string queryRobotStatus_cmd = JsonConvert.SerializeObject(queryRobotStatusMode);
            OperateResult<string> abbResponse = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "QueryRobotStatus", queryRobotStatus_cmd);
            if (!abbResponse.IsSuccess)
            {
                return OperateResult.CreateFailedResult(abbResponse.Message, 1);
            }
            QueryRobotStatus_Response queryRobotStatusRespone = (QueryRobotStatus_Response)abbResponse.Content;
            if (!queryRobotStatusRespone.result.Equals("1"))
            {
                return OperateResult.CreateFailedResult(queryRobotStatusRespone.message, 1);
            }
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 6.2	下发作业指令 下发机器人指令
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public OperateResult SendExeOrder(SendExeOrderMode sendExeOrderMode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                //OperateResult isAbbReady = QueryRobotStatus(sendExeOrderMode.area_code);
                //if (!isAbbReady.IsSuccess)
                //{
                //    return OperateResult.CreateFailedResult(isAbbReady.Message, 1);
                //}
                string cmd = JsonConvert.SerializeObject(sendExeOrderMode);
                OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "SendExeOrder", cmd);
                if (!response.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(response.Message, 1);
                }
                string message = response.Content.Trim();
                try
                {

                    SendExeOrder_Response abbRespone = (SendExeOrder_Response)message;
                    if (abbRespone.result.Equals("1"))
                    {
                        return OperateResult.CreateSuccessResult();
                    }
                    return OperateResult.CreateFailedResult(string.Format("XYZ机器人系统返回失败，失败原因：\r\n {0}", abbRespone.message), 1);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("XYZ机器人系统返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                }
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// 6.3	调度机器人视觉 (机器人视觉拍照功能 查询箱子数量)
        /// </summary>
        /// <param name="invokRobotVisionMode">InvokRobotVisionMode</param>
        /// <returns>OperateResult<InvokRobotVision_Response></returns>
        public OperateResult<InvokRobotVision_Response> InvokRobotVision(InvokRobotVisionMode invokRobotVisionMode)
        {
            OperateResult<InvokRobotVision_Response> result = OperateResult.CreateFailedResult<InvokRobotVision_Response>("");
            try
            {
                //OperateResult isAbbReady = QueryRobotStatus(invokRobotVisionMode.area_code);
                //if (!isAbbReady.IsSuccess)
                //{
                //    result.IsSuccess = false;
                //    result.Message = "查询机器人状态失败";
                //    result.Content.result = "0";
                //    result.Content.message = result.Message;
                //    return result;
                //}
                string cmd = JsonConvert.SerializeObject(invokRobotVisionMode);
                OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "InvokRobotVision", cmd);
                if (!response.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "调用机器人InvokRobotVision接口 失败 ";
                    result.Content.result = "0";
                    result.Content.message = result.Message;
                    return result;
                }
                string message = response.Content.Trim();
                try
                {
                    InvokRobotVision_Response abbRespone = (InvokRobotVision_Response)message;
                    if (abbRespone!=null && abbRespone.result.Equals("1"))
                    {
                        result.IsSuccess = true;
                        result.Message = "调用机器人InvokRobotVision接口 成功 ";
                        result.Content= abbRespone;
                        return result;
                    }

                    result.IsSuccess = false;
                    result.Message = "调用机器人InvokRobotVision接口 返回 失败 ";
                    result.Content.result = "0";
                    result.Content.message = result.Message;
                    return result;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("XYZ机器人系统返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                }
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// 6.4	通知机器人载具更换
        /// </summary>
        /// <param name="noticeRobotReplaceCarrierMode"></param>
        /// <returns></returns>
        public OperateResult NoticeRobotReplaceCarrier(NoticeRobotReplaceCarrierMode noticeRobotReplaceCarrierMode)
        {
            string noticeRobotReplaceCarrier = JsonConvert.SerializeObject(noticeRobotReplaceCarrierMode);
            OperateResult<string> abbResponse = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "NoticeRobotReplaceCarrier", noticeRobotReplaceCarrier);
            if (!abbResponse.IsSuccess)
            {
                return OperateResult.CreateFailedResult(abbResponse.Message, 1);
            }
            NoticeRobotReplaceCarrier_Response noticeRobotReplaceCarrierRespone = (NoticeRobotReplaceCarrier_Response)abbResponse.Content;
            if (!noticeRobotReplaceCarrierRespone.result.Equals("1"))
            {
                return OperateResult.CreateFailedResult(noticeRobotReplaceCarrierRespone.message, 1);
            }
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult GetNgStation(GetNgStationMode mode)
        {
            string strMode = JsonConvert.SerializeObject(mode);
            OperateResult<string> abbResponse = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "GetNgStation", strMode);
            if (!abbResponse.IsSuccess)
            {
                return OperateResult.CreateFailedResult(abbResponse.Message, 1);
            }
            GetNgStation_Response getNgStation_Response = (GetNgStation_Response)abbResponse.Content;
            if (!getNgStation_Response.result.Equals("1"))
            {
                return OperateResult.CreateFailedResult(getNgStation_Response.message, 1);
            }
            if (getNgStation_Response.data >= 1)
            {
                return OperateResult.CreateSuccessResult();
            }
            return OperateResult.CreateFailedResult(getNgStation_Response.message, 1);
        }



        /// <summary>
        /// 6.5	查询机器人作业信息
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public OperateResult<QueryRobotWorkInfo_Response> QueryRobotWorkInfo(QueryRobotWorkInfoMode queryRobotWorkInfoMode)
        {
            OperateResult<QueryRobotWorkInfo_Response> result = OperateResult.CreateFailedResult<QueryRobotWorkInfo_Response>("");
            try
            {
                string cmd = JsonConvert.SerializeObject(queryRobotWorkInfoMode);
                OperateResult<string> response = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "QueryRobotWorkInfo", cmd);
                if (!response.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "调用机器人 QueryRobotWorkInfo 接口 失败 ";
                    result.Content.result = "0";
                    result.Content.message = result.Message;
                    return result;
                }
                string message = response.Content.Trim();
                try
                {
                    QueryRobotWorkInfo_Response abbRespone = (QueryRobotWorkInfo_Response)message;
                    if (abbRespone != null && abbRespone.result.Equals("1"))
                    {
                        result.IsSuccess = true;
                        result.Message = "调用机器人 QueryRobotWorkInfo 接口 成功 ";
                        result.Content = abbRespone;
                        return result;
                    }

                    result.IsSuccess = false;
                    result.Message = "调用机器人 QueryRobotWorkInfo 接口 返回 失败 ";
                    result.Content.result = "0";
                    result.Content.message = result.Message;
                    return result;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("XYZ机器人系统返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// 6.6	删除作业指令
        /// </summary>
        /// <param name="detelExeOrderMode"></param>
        /// <returns></returns>
        public OperateResult DetelExeOrder(DetelExeOrderMode detelExeOrderMode)
        {
            string DetelExeOrder_cmd = JsonConvert.SerializeObject(detelExeOrderMode);
            OperateResult<string> abbResponse = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "DetelExeOrder", DetelExeOrder_cmd);
            if (!abbResponse.IsSuccess)
            {
                return OperateResult.CreateFailedResult(abbResponse.Message, 1);
            }
            DetelExeOrder_Response detelExeOrderRespone = (DetelExeOrder_Response)abbResponse.Content;
            if (!detelExeOrderRespone.result.Equals("1"))
            {
                return OperateResult.CreateFailedResult(detelExeOrderRespone.message, 1);
            }
            return OperateResult.CreateSuccessResult();
        }


        /// <summary>
        /// 查询机器人是否在初始位
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public OperateResult GetInit(string areaCode)
        {
            GetInitCmd getInitMode = new GetInitCmd {  };
            string GetInit_cmd = JsonConvert.SerializeObject(getInitMode);
            OperateResult<string> abbResponse = WebNetInvoke.ServiceRequest<XYZABBResSucMsg2>(Http, "GetInit", GetInit_cmd);
            if (!abbResponse.IsSuccess)
            {
                return OperateResult.CreateFailedResult(abbResponse.Message, 1);
            }
            GetInit_Response getInit_Response = (GetInit_Response)abbResponse.Content;
            if (!getInit_Response.result.Equals("1"))
            {
                return OperateResult.CreateFailedResult("调用接口返回失败", 1);
            }
            OperateResult result= OperateResult.CreateSuccessResult();
            result.Message = getInit_Response.data ? "true" : "false";
            return result;
        }


        #endregion

        /// <summary>
        /// 更具固定的XML配置结构读取参数
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                result.IsSuccess = false;
                result.Message = "xmlNode 节点为NULL";
                return result;
            }
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals("Http"))
                {
                    Http = node.InnerText.Trim();
                    continue;
                }
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitConfig()
        {
            //获取Http 的值
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }

                string communicationConfigXml = xmlNode.OuterXml;
                WebClientCommunicationProperty communicationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        communicationProperty = (WebClientCommunicationProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebClientCommunicationProperty));
                        if (communicationProperty == null)
                        {
                            return OperateResult.CreateFailedResult(string.Format("获取：{0} 通讯初始化参数失败", this.Name));
                        }
                        WebNetInvoke = (IWebNetInvoke)Assembly.Load(communicationProperty.NameSpace).CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

                        if (WebNetInvoke == null)
                        {
                            return OperateResult.CreateFailedResult(string.Format("初始化：{0} 通讯接口：{1} 失败，命名空间：{2} 类名：{3}", this.Name, "IWebNetInvoke", communicationProperty.NameSpace, communicationProperty.ClassName));
                        }

                        WebNetInvoke.LogDisplayName = communicationProperty.Name;
                        WebNetInvoke.CommunicationMode = communicationProperty.CommunicationMode;
                        WebNetInvoke.TimeOut = communicationProperty.Config.TimeOut * 1000;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (communicationProperty == null)
                {
                    return result;
                }

                Http = communicationProperty.Config.Http.Trim();

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
                result = OperateResult.CreateFailedResult();
            }
            return result;
        }

        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetPropertyView()
        {
            WebApiViewVertical controlView = new WebApiViewVertical { Height = 250 };
            controlView.HttpUrl = this.Http;
            return controlView;
        }
    }
}
