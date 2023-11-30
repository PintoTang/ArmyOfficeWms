using System.Windows.Controls;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Station.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 科陆控制的可入可出站台控制逻辑
    /// </summary>
    public class SwitchableStationControl : StationDeviceControlAbstract
    {
        public override OperateResult IsCanChangeMode(DeviceModeEnum destMode)
        {
            //1.读取是否运行切换的DB块的值
            OperateResult<int> isCanChange = Communicate.ReadInt(DataBlockNameEnum.AllowModeSwitchDataBlock);
            if (!isCanChange.IsSuccess)
            {
                return OperateResult.CreateFailedResult(isCanChange.Message, 1);
            }
            int isCanValue = isCanChange.Content;
            if (isCanValue.Equals(1))
            {
                return OperateResult.CreateSuccessResult();
            }
            else
            {
                return OperateResult.CreateFailedResult(string.Format("PLC 返回值：{0} 不允许切换模式", isCanValue), 1);
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
            else
            {
                plcModeValue = 3;
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
            SizeProperties properties=new SizeProperties();
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
