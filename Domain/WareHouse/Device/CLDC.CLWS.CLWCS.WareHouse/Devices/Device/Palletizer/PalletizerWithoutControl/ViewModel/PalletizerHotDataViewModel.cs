using System.Collections.ObjectModel;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PalletizerWithoutControl.ViewModel
{
    public class PalletizerHotDataViewModel
    {
        public PalletizerDeviceAbstract Device { get; set; }
        public PalletizerHotDataViewModel(PalletizerDeviceAbstract device)
        {
            Device = device;
            PalletizerHotData = device.CurContent.DataPool;
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

        public ObservableCollection<PalletizeContent> PalletizerHotData { get; set; }


        private MyCommand _addHotDataCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddHotDataCommand
        {
            get
            {
                if (_addHotDataCommand == null)
                    _addHotDataCommand = new MyCommand(AddHotData);
                return _addHotDataCommand;
            }
        }
        private void AddHotData(object obj)
        {
            PalletizeContent content = new PalletizeContent(0, "Barcode");
            PalletizerHotData.Add(content);
            string recordMsg = string.Format("添加跺条码：{0}", "Barcode");
            OperateLogHelper.Record(recordMsg);
        }

        private MyCommand _notifyPalletizerFinishCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand NotifyPalletizerFinishCommand
        {
            get
            {
                if (_notifyPalletizerFinishCommand == null)
                    _notifyPalletizerFinishCommand = new MyCommand(NotifyPalletizerFinish);
                return _notifyPalletizerFinishCommand;
            }
        }
        private void NotifyPalletizerFinish(object obj)
        {
          OperateResult reNotifyResult=  Device.ReNotifyPalletizerFinish();
            string msg = string.Format("重新调用叠盘完成结果：{0} 信息：{1}", reNotifyResult.IsSuccess, reNotifyResult.Message);
            string recordMsg = string.Format("手动调用得盘完成接口，调用结果：{0} 信息：{1}", reNotifyResult.IsSuccess, reNotifyResult.Message);
            OperateLogHelper.Record(recordMsg);
            SnackbarQueue.MessageQueue.Enqueue(msg);
        }



        private MyCommand _deleteHotDataCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteHotDataCommand
        {
            get
            {
                if (_deleteHotDataCommand == null)
                    _deleteHotDataCommand = new MyCommand(DeleteHotData);
                return _deleteHotDataCommand;
            }
        }

        private void DeleteHotData(object arg)
        {
            if (!(arg is PalletizeContent))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的实时数据");
                return;
            }
            PalletizeContent deleteItem = arg as PalletizeContent;
            MessageBoxResult result = MessageBoxEx.Show("确定删除该实时数据？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = Device.RemoveContent(deleteItem).IsSuccess;
            if (!removeResult)
            {
                string msg = "实时数据删除失败";
                string recordMsg = string.Format("手动删除实时数据：{1}，操作结果：{0}", msg, deleteItem.ContentBarcode);
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                string recordMsg = string.Format("手动删除实时数据: {1}，操作结果：{0}", "删除成功", deleteItem.ContentBarcode);
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue("实时数据删除成功");
            }
        }

        private MyCommand _saveHotDataCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SaveHotDataCommand
        {
            get
            {
                if (_saveHotDataCommand == null)
                    _saveHotDataCommand = new MyCommand(SaveHotData);
                return _saveHotDataCommand;
            }
        }

        private void SaveHotData(object arg)
        {
            if (!(arg is PalletizeContent))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要保存的实时数据");
                return;
            }
            PalletizeContent saveItem = arg as PalletizeContent;
            MessageBoxResult result = MessageBoxEx.Show("确定保存该实时数据？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            bool removeResult = Device.AddContent(saveItem).IsSuccess;
            string recordMsg = string.Format("手动保存实时数据: {1}，操作结果：{0}", "成功", saveItem.ContentBarcode);
            OperateLogHelper.Record(recordMsg);
            SnackbarQueue.MessageQueue.Enqueue(string.Format("实时数据保存结果：{0}", removeResult));
        }

    }
}
