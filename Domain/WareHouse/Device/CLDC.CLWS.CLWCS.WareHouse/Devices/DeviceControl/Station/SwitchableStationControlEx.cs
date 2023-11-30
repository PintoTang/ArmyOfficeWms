using System.Windows.Controls;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Station.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 科陆控制的可入可出站台控制逻辑 扩展
    /// </summary>
    public class SwitchableStationControlEx : StationDeviceControlAbstract
    {
        public override OperateResult IsCanChangeMode(DeviceModeEnum destMode)
        {
            //1.读取是否运行切换的DB块的值
            OperateResult<bool> isCanChange = Communicate.ReadBool(DataBlockNameEnum.AllowModeSwitchDataBlock);
            if (!isCanChange.IsSuccess)
            {
                return OperateResult.CreateFailedResult(isCanChange.Message, 1);
            }
            if (isCanChange.Content)
            {
                return OperateResult.CreateSuccessResult();
            }
            else
            {
                return OperateResult.CreateFailedResult("PLC不允许切换模式", 1);
            }
        }

        public override OperateResult ChangeMode(DeviceModeEnum destMode)
        {
            int plcModeValue = 1;
            //1.写入切换模式的值
            if (destMode.Equals(DeviceModeEnum.In))
            {
                plcModeValue = 1;
            }
            else if (destMode.Equals(DeviceModeEnum.Out))
            {
                plcModeValue = 2;
            }
            else if(destMode.Equals(DeviceModeEnum.Stock))
            {
                plcModeValue = 3;
            }
            else 
            {
               //其它类型 需要根据实际情况 处理
                plcModeValue = 9999;
            }
            
            OperateResult writeResult = Communicate.Write(DataBlockNameEnum.WriteInOutModeDataBlock, plcModeValue);
            return writeResult;
        }

        public override bool CheckMode(DeviceModeEnum destMode)
        {
            if (CurMode.Equals(destMode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override OperateResult<SizeProperties> GetGoodsProperties()
        {
            SizeProperties properties = new SizeProperties();
            OperateResult<int> height = Communicate.ReadInt(DataBlockNameEnum.GoodHeightType);
            if (!height.IsSuccess)
            {
                return OperateResult.CreateFailedResult<SizeProperties>("无数据");
            }
            properties.Height = height.Content;
            return OperateResult.CreateSuccessResult(properties);
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
            return null;
        }
    }
}
