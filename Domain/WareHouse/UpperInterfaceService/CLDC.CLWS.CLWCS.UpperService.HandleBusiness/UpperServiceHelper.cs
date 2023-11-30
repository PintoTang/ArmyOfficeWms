using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.UpperService.HandleBusiness
{
    public static class UpperServiceHelper
    {
        public static OperateResult<object> WmsServiceInvoke(NotifyElement notifyElement)
        {
            OperateResult<object> invokeResult =new OperateResult<object>();
            UpperServiceBusinessAbstract upperService = UpperServiceBusinessManage.Instance.Find(UpperSystemEnum.Wms);
            if (upperService==null)
            {
                invokeResult.IsSuccess = false;
                invokeResult.Message = "Wms系统的上层服务尚未注册，无法调用";
                return invokeResult;
            }
           return upperService.Invoke(notifyElement);
        }

        public static OperateResult WmsServiceBeginInvoke(NotifyElement notifyElement)
        {
            OperateResult invokeResult = new OperateResult();
            UpperServiceBusinessAbstract upperService = UpperServiceBusinessManage.Instance.Find(UpperSystemEnum.Wms);
            if (upperService == null)
            {
                invokeResult.IsSuccess = false;
                invokeResult.Message = "Wms系统的上层服务尚未注册，无法调用";
                return invokeResult;
            }
            return upperService.BeginInvoke(notifyElement);
        }

        public static OperateResult<object> IoTServiceInvoke(NotifyElement notifyElement)
        {
            OperateResult<object> invokeResult = new OperateResult<object>();
            UpperServiceBusinessAbstract upperService = UpperServiceBusinessManage.Instance.Find(UpperSystemEnum.IoT);
            if (upperService == null)
            {
                invokeResult.IsSuccess = false;
                invokeResult.Message = "IoT系统的上层服务尚未注册，无法调用";
                return invokeResult;
            }
            return upperService.Invoke(notifyElement);
        }

        public static OperateResult IoTServiceBeginInvoke(NotifyElement notifyElement)
        {
            OperateResult invokeResult = new OperateResult();
            UpperServiceBusinessAbstract upperService = UpperServiceBusinessManage.Instance.Find(UpperSystemEnum.IoT);
            if (upperService == null)
            {
                invokeResult.IsSuccess = false;
                invokeResult.Message = "IoT系统的上层服务尚未注册，无法调用";
                return invokeResult;
            }
            return upperService.BeginInvoke(notifyElement);
        }


    }
}
