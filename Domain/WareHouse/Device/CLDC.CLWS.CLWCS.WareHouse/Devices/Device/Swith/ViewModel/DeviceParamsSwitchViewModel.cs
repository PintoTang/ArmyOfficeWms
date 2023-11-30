using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CL.Framework.OPCClientAbsPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.ViewModel
{
    public class DeviceParamsSwitchViewModel : DeviceDataBase
    {
        #region 属性


        #endregion

        private Dictionary<DeviceModeEnum, string> dicEmOrderStatu = new Dictionary<DeviceModeEnum, string>();
        public Dictionary<DeviceModeEnum, string> DicEmOrderStatu
        {
            get
            {
                if (dicEmOrderStatu.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(DeviceModeEnum)))
                    {
                        Enum em = (DeviceModeEnum)Enum.Parse(typeof(DeviceModeEnum), strName);
                        dicEmOrderStatu.Add((DeviceModeEnum)em, GetDescription(em));
                    }
                }
                return dicEmOrderStatu;
            }
        }

        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }
        public IPlcDeviceCom IPlcDeviceCom { get; set; }


        /// <summary>  
        /// 获取枚举描述
        /// </summary>  
        /// <param name="en">枚举</param>  
        /// <returns>返回枚举的描述 </returns>  
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();   //获取类型  
            MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员  
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                as DescriptionAttribute[];   //获取描述特性  
                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;    //返回当前描述
                }
            }
            return en.ToString();

        }

        private SwitchDeviceViewModel DeviceViewModel { get; set; }
        /// <summary>
        /// 上一个对象 传递过来的 DataContext
        /// </summary>
        /// <param name="PreDataContext"></param>
        public DeviceParamsSwitchViewModel(object PreDataContext)
        {
            DeviceViewModel = PreDataContext as SwitchDeviceViewModel;
            XmlNodeName = "SwitchDevices";
            DeviceId = DeviceViewModel.Id;
            Datablocks = DeviceViewModel.Device.DeviceControl.Communicate.OpcElement.Datablocks;
            List<CLDC.CLWS.CLWCS.Service.ConfigService.DeviceInfoConfig> deviceInfoConfigLst = CLDC.CLWS.CLWCS.Service.ConfigService.DeviceInfoConfigManage.Instance.GetAllDeviceInfoData();
            DeviceHandleConn.ConnConfig.DataBlockItems.DatablockList = Datablocks;//赋值转换过的真实数据 用于真实OPC读写

            IPlcDeviceCom = DeviceViewModel.Device.DeviceControl.Communicate;
            LoadData_templateOne();
            //读取OPC地址
            if (Datablocks != null)
            {
                foreach (Datablock dBlock in Datablocks)
                {
                    OperateResult<string> opResult = IPlcDeviceCom.ReadString(dBlock.DatablockEnum);
                    dBlock.RealValue = opResult == null ? "" : opResult.Content;
                }
            }
        }




        private MyCommand editOpcDataCommand;
        public MyCommand EditOpcDataCommand
        {
            get
            {
                if (editOpcDataCommand == null)
                    editOpcDataCommand = new MyCommand(
                        new Action<object>(
                            o => UpdateOpcData(o)),
                null);
                return editOpcDataCommand;
            }
        }
        private void UpdateOpcData(object obj)
        {
            //Datablock datablock = obj as Datablock; 

            ////转换成xml格式的对象 进行保存
            //string conn = DeviceHandleConn.ConnConfig.Connection;
            //List<DataBlockItemsItemConfig> dataBlockItems = DeviceHandleConn.ConnConfig.DataBlockItems.DBlockItemXmlList;
            //DataBlockItemsItemConfig dataBlockItem = obj as DataBlockItemsItemConfig;
            ////xml保存 OPC项 采用全删全增 （无唯一标识）
            ////1、移除OPC项


            ////2、新增 插入OPC项

        }

        /// <summary>
        /// 保存 （1、本地内存和xml）
        /// </summary>
        /// <param name="vmObj">对象ViewModel</param>
        public OperateResult Save(object vmObj)
        {
            OperateResult opResult = OperateResult.CreateFailedResult();
            try
            {
                //更新缓存
                //SwitchDeviceViewModel deviceViewModel = vmObj as SwitchDeviceViewModel;
                SaveXml_templateOne();
                opResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Message = ex.ToString();
            }
            return opResult;
        }
     

    }
}
