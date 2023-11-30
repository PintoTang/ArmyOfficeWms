using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc;

namespace CLWCS.WareHouse.ArchitectureData.HeFei
{
    public sealed class PlcItemCalculateForHeFei : IPlcItemCalculate
    {

        private int _offset = 1001;
        
        public string CaculatePlcAddress(int opcId, DataBlockNameEnum datablockEnum)
        {
            string address = string.Empty;
            if (opcId >= 1001 && opcId <= 1050)
            {
                _offset = 1001;
            }
            else if (opcId >= 2001 && opcId <= 2050)
            {
                _offset = 1951;
            }
            else if (opcId >= 3001 && opcId <= 3050)
            {
                _offset = 2901;
            }else if (opcId >= 4001 && opcId <= 4050)
            {
                _offset = 3851;
            }
            switch (datablockEnum)
            {
                case DataBlockNameEnum.OPCOrderIdDataBlock://任务号
                case DataBlockNameEnum.WriteOrderIDDataBlock://任务号
                    address = string.Format("DB30,DINT{0}", (opcId - _offset) * 36);
                    break;
                case DataBlockNameEnum.WMSOPCOrderIdDataBlock:
                    address = string.Format("DB30,DBD{0}", (opcId - _offset) * 36 + 32);
                    break;
                case DataBlockNameEnum.DestinationDataBlock://目标地址
                case DataBlockNameEnum.WriteDirectionDataBlock://目标地址
                    address = string.Format("DB30,INT{0}", (opcId - _offset) * 36 + 4);
                    break;
                case DataBlockNameEnum.OPCBarcodeDataBlock://托盘条码
                    address = string.Format("DB30,STRING{0}.14", (opcId - _offset) * 36 + 6);
                    break;
                case DataBlockNameEnum.OrderTypeDataBlock://任务类型
                    address = string.Format("DB30,INT{0}", (opcId - _offset) * 36 + 30);
                    break;
                case DataBlockNameEnum.CountDataBlock://托盘数量
                    address = string.Format("DB30,INT{0}", (opcId - _offset) * 36 + 28);
                    break;
                //case DataBlockNameEnum.DePalletizeCountDataBlock://拆盘数量
                //    address = string.Format("DB30,DBW{0}", (opcId - _offset) * 34 + 40);
                //    break;
                case DataBlockNameEnum.PickingReadyDataBlock://分拣就绪
                    address = string.Format("DB30,INT{0}", (opcId - _offset) * 36 + 24);
                    break;
                case DataBlockNameEnum.OPCWeightDataBlock://货物称重
                    address = string.Format("DB30,DINT{0}", (opcId - _offset) * 36 + 22);
                    break;
                //故障
                case DataBlockNameEnum.IsTranslationAllFault:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 0, 0);
                    break;
                case DataBlockNameEnum.IsLoaded:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 1, 3);
                    break;
                case DataBlockNameEnum.IsTranslationFw:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 1, 5);
                    break;
                case DataBlockNameEnum.IsTranslationRv:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 1, 6);
                    break;
                case DataBlockNameEnum.ErrorCode:
                    address = string.Format("DB50,INT{0}", (opcId - _offset) * 10 + 306);
                    break;
                case DataBlockNameEnum.IsTranslationDriverFault:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 0, 1);
                    break;
                case DataBlockNameEnum.IsTranslationTimeOut:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 0, 2);
                    break;
                case DataBlockNameEnum.IsTranslationSwitchFault:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 0, 3);
                    break;
                case DataBlockNameEnum.IsCrashStop:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 1, 2);
                    break;
                case DataBlockNameEnum.IsTranslationSensorFault:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 0, 5);
                    break;
                case DataBlockNameEnum.IsUpDownFault:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 2, 0);
                    break;
                case DataBlockNameEnum.IsUpDownDriverFault:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 2, 1);
                    break;
                case DataBlockNameEnum.IsUpDownTimeOut:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 2, 2);
                    break;
                case DataBlockNameEnum.IsUpDownSwitchFault:
                    address = string.Format("DB50,X{0}.{1}", (opcId - _offset) * 10 + 2, 3);
                    break;
            }
            return address;
        }
    }
}
