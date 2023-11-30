using System;
using CL.Framework.CmdDataModelPckg;
using CL.Framework.OPCClientAbsPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc
{
    public class OpcDataViewModel : NotifyObject
    {
        private IPlcDeviceCom PlcDeviceCom { get; set; }
        private DataBlockItemsItemConfig _dataBlockItemsItemConfig;
        public DataBlockItemsItemConfig DataBlockItemsItemConfig
        {
            get { return _dataBlockItemsItemConfig; }
            set
            {
                _dataBlockItemsItemConfig = value;
                RaisePropertyChanged("DataBlockItemsItemConfig");
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

        private Datablock _datablock;
        public Datablock Datablock
        {
            get { return _datablock; }
            set
            {
                _datablock = value;
                RaisePropertyChanged("Datablock");
            }
        }
        private string _sInputOpcValue = "0";
        public string S_InputOpcValue
        {
            get { return _sInputOpcValue; }
            set
            {
                _sInputOpcValue = value;
                RaisePropertyChanged("S_InputOpcValue");
            }
        }

        public OpcDataViewModel()
        {

        }
        public OpcDataViewModel(IPlcDeviceCom iPlcDeviceCom,object obj)
        {
            PlcDeviceCom = iPlcDeviceCom;
            if (obj.GetType() == typeof(DataBlockItemsItemConfig))
            {
                DataBlockItemsItemConfig = obj as DataBlockItemsItemConfig;
            }
            else if (obj.GetType() == typeof(Datablock))
            {
                Datablock = obj as Datablock;
            }
        }

        private MyCommand _cmdOpcDataUpdate;
        /// <summary>
        /// OPC数据更新
        /// </summary>
        public MyCommand CmdOpcDataUpdate
        {
            get
            {
                if (_cmdOpcDataUpdate == null)
                    _cmdOpcDataUpdate = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              WriteOpc();
                          }
                        ));
                return _cmdOpcDataUpdate;
            }
        }
        private void WriteOpc()
        {
            OperateResult opResult = OperateResult.CreateFailedResult();
            try
            {
                opResult = PlcDeviceCom.Write(Datablock.DatablockEnum, S_InputOpcValue);
                opResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Message = "写入OPC失败:  " + Datablock.DatablockEnum + "__" + Datablock.RealDataBlockAddr + "__" + S_InputOpcValue;
            }
           
        }

    }
}
