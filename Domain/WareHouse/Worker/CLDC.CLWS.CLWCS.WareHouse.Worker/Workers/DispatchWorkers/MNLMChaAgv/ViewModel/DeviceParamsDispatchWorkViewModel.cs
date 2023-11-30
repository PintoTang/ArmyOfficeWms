using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Manage;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.Infrastructrue.UserCtrl;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.ViewModel
{
    public class AddrPrefixsInfo
    {
        public string type{get;set;}
         public string addr{get;set;}
    }

    public class DeviceParamsDispatchWorkViewModel:WareHouseViewModelBase
    {
        private List<AssistantDevice> _deviceBaseAbstractList = new List<AssistantDevice>();
        public List<AssistantDevice> DeviceBaseAbstractList
        {
            get { return _deviceBaseAbstractList; }
            set
            {
                _deviceBaseAbstractList = value;
                RaisePropertyChanged("DeviceBaseAbstractList");
            }
        }
        private List<AddrPrefixsInfo> addrPrefixsInfoList=new List<AddrPrefixsInfo>();
        public List<AddrPrefixsInfo> AddrPrefixsInfoList
        {
            get
            {
                return addrPrefixsInfoList;
            }
            set
            {
                addrPrefixsInfoList = value;
                RaisePropertyChanged("AddrPrefixsInfoList");
            }
        }

        private WorkerBaseAbstract workerBaseAbstract;
        public WorkerBaseAbstract WorkerBaseAbstract
        {
            get { return workerBaseAbstract; }
            set
            {
                workerBaseAbstract = value;
                RaisePropertyChanged("WorkerBaseAbstract");
            }
        }
        public DeviceParamsDispatchWorkViewModel(object obj)
        {
            WareHouseViewModelBase viewModelBase = obj as WareHouseViewModelBase;
            MNLMChaWorkerViewModel hangChaWorkerViewModel = viewModelBase as MNLMChaWorkerViewModel;

            WorkerBaseAbstract = WorkerManage.Instance.FindDeivceByDeviceId(viewModelBase.Id);
            DeviceBaseAbstractList = WorkerBaseAbstract.WorkerAssistants;
        }


        #region Event
        private MyCommand editOrderParmsCommand;
        public MyCommand EditOrderParmsCommand
        {
            get
            {
                if (editOrderParmsCommand == null)
                    editOrderParmsCommand = new MyCommand(
                        new Action<object>(
                            o => EditOrderData(o)),
                null);
                return editOrderParmsCommand;
            }
        }
        #endregion
        /// <summary>
        /// 编辑 
        /// </summary>
        /// <param name="obj">DataGrid选中的行 ExOrder 对象</param>
        private void EditOrderData(object obj)
        {
            try
            {
                //var exorder = obj as DeviceBaseAbstract;

                //if (exorder == null) return;
                //UCXChaAGVView ucXChaAGVView = new UCXChaAGVView();
                //ucXChaAGVView.DataContext = new XChaAGVViewModel( obj);
                //ucXChaAGVView.ShowDialog();
            
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("打开AGV界面失败:" + OperateResult.ConvertException(ex));
            }
        }
        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
           
        }
    }
}
