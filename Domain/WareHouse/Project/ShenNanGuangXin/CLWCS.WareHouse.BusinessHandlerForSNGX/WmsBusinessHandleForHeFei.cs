using System.Windows;
using CL.WCS.DataModelPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.StringCharTaskHandle;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLWCS.UpperServiceForHeFei.Target;
using CLWCS.UpperServiceForHeFei.View;
using CLWCS.UpperServiceForHeFei.ViewModel;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLWCS.WareHouse.BusinessHandlerForHeFei
{
     public sealed class WmsBusinessHandleForHeFei : UpperServiceBusinessAbstract, IHandleStringCharTaskStatus
    {
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;
        public override OperateResult ParticularInitilize()
        {
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            IStringCharTaskNotifyCentre orderNotifyCentre = DependencyHelper.GetService<IStringCharTaskNotifyCentre>();
            if (orderNotifyCentre != null)
            {
                orderNotifyCentre.RegisterTaskStatusListener(this);
            }
            return OperateResult.CreateSuccessResult();
        }

        public override Window GetAssistantView()
        {
            WmsAssitantForHeFeiView assistantView = new WmsAssitantForHeFeiView();
            WmsAssitantForHeFeiViewModel viewModel = new WmsAssitantForHeFeiViewModel(UpperService);
            assistantView.DataContext = viewModel;
            return assistantView;
        }

        protected override OperateResult OrderFinishHandler(ExOrder order)
        {
            NotifyInstructFinishCmdMode finish = new NotifyInstructFinishCmdMode();
            OperateResult<string> address = _wmsWcsDataArchitecture.WcsToWmsAddr(order.CurrAddr.FullName);
            if (!address.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("Wcs的地址：{0} 转换为Wms的地址失败,原因：{1}", order.CurrAddr.FullName, address.Message), 1);
            }
            finish.DATA.DST_ADDR = address.Content;
            finish.DATA.ID = int.Parse(order.DocumentCode);
            finish.DATA.PACKAGE_BARCODE = order.PileNo;

            string upperCmd = finish.ToString();

            NotifyElement notify = new NotifyElement(order.PileNo, WmsServiceForHeFeiEnum.NotifyInstructFinish.ToString(), "指令执行完成上报", null, upperCmd);

            return BeginInvoke(notify);
        }

        public OperateResult FinishStringCharTask(DeviceBaseAbstract device, StringCharTask taskValue)
        {
            if (taskValue.TaskType.Equals(StringCharTaskTypeEnum.UnStack))
            {
                NotifyUnstackFinishCmdMode finishCmd=new NotifyUnstackFinishCmdMode();
                finishCmd.DATA.ACTION = StackFinishActionEnum.UP;

                finishCmd.DATA.ADDR = device.CurAddress.ToString();
                string upperCmd = finishCmd.ToString();
                NotifyElement notify = new NotifyElement(taskValue.UpperTaskCode, WmsServiceForHeFeiEnum.NotifyUnstackFinish.ToString(), "拆盘机动作完成", null, upperCmd);
                return BeginInvoke(notify);

            }
            if (taskValue.TaskType.Equals(StringCharTaskTypeEnum.Stack))
            {
               
                NotifyUnstackFinishCmdMode finishCmd = new NotifyUnstackFinishCmdMode();
                finishCmd.DATA.ACTION = StackFinishActionEnum.DOWN;

                finishCmd.DATA.ADDR = device.CurAddress.ToString();
                string upperCmd = finishCmd.ToString();
                NotifyElement notify = new NotifyElement(taskValue.UpperTaskCode, WmsServiceForHeFeiEnum.NotifyUnstackFinish.ToString(), "拆盘机动作完成", null, upperCmd);
                return BeginInvoke(notify);
            }
            return OperateResult.CreateSuccessResult();
        }
    }
}
