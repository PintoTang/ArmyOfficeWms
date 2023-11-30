using System;
using System.Windows.Controls;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Station.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 入库站台的控制业务逻辑
    /// </summary>
    public class StaticStationControl : StationDeviceControlAbstract
	{


        public override OperateResult IsCanChangeMode(DeviceModeEnum destMode)
        {
            throw new NotImplementedException();
        }

        public override OperateResult ChangeMode(DeviceModeEnum destMode)
        {
            throw new NotImplementedException();
        }

        public override bool CheckMode(DeviceModeEnum destMode)
        {
            throw new NotImplementedException();
        }

        public override OperateResult<SizeProperties> GetGoodsProperties()
        {
            SizeProperties properties = new SizeProperties();
            OperateResult<int> height = Communicate.ReadInt(DataBlockNameEnum.GoodHeightType);
            if (!height.IsSuccess)
            {
                return OperateResult.CreateFailedResult<SizeProperties>(height.Message);
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
