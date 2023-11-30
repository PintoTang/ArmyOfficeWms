using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Stackingcrane.OpcStackingcrane.RobotTec.ViewModel
{
    public class StackingcraneProtocolTranslationViewModel : NotifyObject
    {

        private ObservableCollection<WcsToDeviceDic> _columnLst = new ObservableCollection<WcsToDeviceDic>();
        private ObservableCollection<WcsToDeviceDic> _faultCodeLst = new ObservableCollection<WcsToDeviceDic>();

        public ObservableCollection<WcsToDeviceDic> ColumnLst
        {
            get { return _columnLst; }
            set
            {
                _columnLst = value;
            }
        }

        public ObservableCollection<WcsToDeviceDic> FaultCodeLst
        {
            get { return _faultCodeLst; }
            set
            {
                _faultCodeLst = value;
            }
        }
        public StackingcraneProtocolTranslationProperty DataModel { get; set; }
        public StackingcraneProtocolTranslationViewModel(StackingcraneProtocolTranslationProperty model)
        {
            DataModel = model;
            InitColumnLst();
            InitFaultCodeLst();
        }

        private void InitColumnLst()
        {
            _columnLst.Clear();
            string tempValue = DataModel.Config.RowChange.Trim();
            string[] key_value = tempValue.Split(';');
            foreach (string keyValue in key_value)
            {
                if (string.IsNullOrEmpty(keyValue))
                { continue; }
                string[] from_to = keyValue.Split('_');
                if (from_to.Length >= 2)
                {
                    string from = from_to[0];
                    string to = from_to[1];
                    WcsToDeviceDic dic = new WcsToDeviceDic()
                    {
                        WcsValue = from,
                        DeviceValue = to
                    };
                    _columnLst.Add(dic);
                }
            }
        }

        private void InitFaultCodeLst()
        {
            _faultCodeLst.Clear();
            string tempValue = DataModel.Config.FaultCode.Trim();
            string[] key_value = tempValue.Split(';');
            foreach (string keyValue in key_value)
            {
                if (string.IsNullOrEmpty(keyValue))
                { continue; }
                string[] from_to = keyValue.Split('_');
                if (from_to.Length >= 2)
                {
                    string from = from_to[0];
                    string to = from_to[1];
                    WcsToDeviceDic dic = new WcsToDeviceDic()
                    {
                        WcsValue = from,
                        DeviceValue = to
                    };
                    _faultCodeLst.Add(dic);
                }

            }
        }


        private MyCommand _addColumnCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddColumnCommand
        {
            get
            {
                if (_addColumnCommand == null)
                    _addColumnCommand = new MyCommand(AddColumn);
                return _addColumnCommand;
            }
        }
        private void AddColumn(object obj)
        {
            WcsToDeviceDic item = new WcsToDeviceDic();
            ColumnLst.Add(item);
        }


        private MyCommand _deleteColumnCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteColumnCommand
        {
            get
            {
                if (_deleteColumnCommand == null)
                    _deleteColumnCommand = new MyCommand(DeleteColumn);
                return _deleteColumnCommand;
            }
        }

        private void DeleteColumn(object arg)
        {
            if (!(arg is WcsToDeviceDic))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的排转换信息");
                return;
            }
            WcsToDeviceDic deleteItem = arg as WcsToDeviceDic;
            MessageBoxResult result = MessageBoxEx.Show("确定删除排转换信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = ColumnLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "排转换信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("排转换信息删除成功");
            }
        }

        private MyCommand _saveColumnCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SaveColumnCommand
        {
            get
            {
                if (_saveColumnCommand == null)
                    _saveColumnCommand = new MyCommand(SaveColumn);
                return _saveColumnCommand;
            }
        }

        private void SaveColumn(object arg)
        {
            StringBuilder columnKeyValue = new StringBuilder();
            foreach (WcsToDeviceDic keyValue in ColumnLst)
            {
                columnKeyValue.Append(keyValue.WcsValue);
                columnKeyValue.Append('_');
                columnKeyValue.Append(keyValue.DeviceValue);
                columnKeyValue.Append(';');
            }
            DataModel.Config.RowChange = columnKeyValue.ToString();
            SnackbarQueue.MessageQueue.Enqueue("排转换信息保存成功");
        }



        private MyCommand _addFaultCodeCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddFaultCodeCommand
        {
            get
            {
                if (_addFaultCodeCommand == null)
                    _addFaultCodeCommand = new MyCommand(AddFaultCode);
                return _addFaultCodeCommand;
            }
        }
        private void AddFaultCode(object obj)
        {
            WcsToDeviceDic item = new WcsToDeviceDic();
            FaultCodeLst.Add(item);
        }


        private MyCommand _deleteFaultCodeCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteFaultCodeCommand
        {
            get
            {
                if (_deleteFaultCodeCommand == null)
                    _deleteFaultCodeCommand = new MyCommand(DeleteFaultCode);
                return _deleteFaultCodeCommand;
            }
        }

        private void DeleteFaultCode(object arg)
        {
            if (!(arg is WcsToDeviceDic))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的错误代码信息");
                return;
            }
            WcsToDeviceDic deleteItem = arg as WcsToDeviceDic;
            MessageBoxResult result = MessageBoxEx.Show("确定删除错误代码信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = FaultCodeLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "错误代码信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("错误代码信息删除成功");
            }
        }

        private MyCommand _saveFaultCodeCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SaveFaultCodeCommand
        {
            get
            {
                if (_saveFaultCodeCommand == null)
                    _saveFaultCodeCommand = new MyCommand(SaveFaultCode);
                return _saveFaultCodeCommand;
            }
        }

        private void SaveFaultCode(object arg)
        {
            StringBuilder FaultCodeKeyValue = new StringBuilder();
            foreach (WcsToDeviceDic keyValue in FaultCodeLst)
            {
                FaultCodeKeyValue.Append(keyValue.WcsValue);
                FaultCodeKeyValue.Append('_');
                FaultCodeKeyValue.Append(keyValue.DeviceValue);
                FaultCodeKeyValue.Append(';');
            }
            DataModel.Config.FaultCode = FaultCodeKeyValue.ToString();
            SnackbarQueue.MessageQueue.Enqueue("错误代码信息保存成功");
        }

    }
    public class WcsToDeviceDic : NotifyObject
    {
        private string _wcsValue;
        private string _deviceValue;

        public string WcsValue
        {
            get { return _wcsValue; }
            set
            {
                _wcsValue = value;
                RaisePropertyChanged("WcsValue");
            }
        }

        public string DeviceValue
        {
            get { return _deviceValue; }
            set
            {
                _deviceValue = value;
                RaisePropertyChanged("DeviceValue");
            }
        }
    }
}
