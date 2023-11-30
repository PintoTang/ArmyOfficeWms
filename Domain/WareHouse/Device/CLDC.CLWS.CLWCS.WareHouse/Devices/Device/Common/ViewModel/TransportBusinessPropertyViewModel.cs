using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel
{
    public class TransportBusinessPropertyViewModel : NotifyObject
    {
        private ObservableCollection<AddressItem> _startAddressLst = new ObservableCollection<AddressItem>();
        private ObservableCollection<AddressItem> _destAddressLst = new ObservableCollection<AddressItem>();
        public TransportBusinessHandleProperty DataModel { get; set; }

        public ObservableCollection<AddressItem> StartAddressLst
        {
            get { return _startAddressLst; }
            set
            {
                _startAddressLst = value;
            }
        }

        public ObservableCollection<AddressItem> DestAddressLst
        {
            get { return _destAddressLst; }
            set
            {
                _destAddressLst = value;
            }
        }
        public TransportBusinessPropertyViewModel(TransportBusinessHandleProperty model)
        {
            DataModel = model;
            InitStartAddressLst();
            InitDestAddressLst();
        }


        private MyCommand _addStartAddrCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddStartAddrCommand
        {
            get
            {
                if (_addStartAddrCommand == null)
                    _addStartAddrCommand = new MyCommand(AddStartAddr);
                return _addStartAddrCommand;
            }
        }
        private void AddStartAddr(object obj)
        {
            AddressItem item = new AddressItem();
            StartAddressLst.Add(item);
        }


        private MyCommand _deleteStartAddrCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteStartAddrCommand
        {
            get
            {
                if (_deleteStartAddrCommand == null)
                    _deleteStartAddrCommand = new MyCommand(DeleteStartAddr);
                return _deleteStartAddrCommand;
            }
        }

        private void DeleteStartAddr(object arg)
        {
            if (!(arg is AddressItem))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的地址信息");
                return;
            }
            AddressItem deleteItem = arg as AddressItem;
            MessageBoxResult result = MessageBoxEx.Show("确定删除该地址信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = StartAddressLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "地址信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("地址信息删除成功");
            }
        }

        private MyCommand _saveStartAddrCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SaveStartAddrCommand
        {
            get
            {
                if (_saveStartAddrCommand == null)
                    _saveStartAddrCommand = new MyCommand(SaveStartAddr);
                return _saveStartAddrCommand;
            }
        }

        private void SaveStartAddr(object arg)
        {
            StringBuilder address = new StringBuilder();
            foreach (AddressItem addr in StartAddressLst)
            {
                address.Append(addr.Value);
                address.Append('|');
            }
            DataModel.Config.StartAddress = address.ToString();
            SnackbarQueue.MessageQueue.Enqueue("地址信息保存成功");
        }




        private MyCommand _addDestAddrCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddDestAddrCommand
        {
            get
            {
                if (_addDestAddrCommand == null)
                    _addDestAddrCommand = new MyCommand(AddDestAddr);
                return _addDestAddrCommand;
            }
        }
        private void AddDestAddr(object obj)
        {
            AddressItem item = new AddressItem();
            DestAddressLst.Add(item);
        }


        private MyCommand _deleteDestAddrCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteDestAddrCommand
        {
            get
            {
                if (_deleteDestAddrCommand == null)
                    _deleteDestAddrCommand = new MyCommand(DeleteDestAddr);
                return _deleteDestAddrCommand;
            }
        }

        private void DeleteDestAddr(object arg)
        {
            if (!(arg is AddressItem))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的地址信息");
                return;
            }
            AddressItem deleteItem = arg as AddressItem;
            MessageBoxResult result = MessageBoxEx.Show("确定删除该地址信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = DestAddressLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "地址信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("地址信息删除成功");
            }
        }

        private MyCommand _saveDestAddrCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SaveDestAddrCommand
        {
            get
            {
                if (_saveDestAddrCommand == null)
                    _saveDestAddrCommand = new MyCommand(SaveDestAddr);
                return _saveDestAddrCommand;
            }
        }

        private void SaveDestAddr(object arg)
        {
            StringBuilder address = new StringBuilder();
            foreach (AddressItem addr in DestAddressLst)
            {
                address.Append(addr.Value);
                address.Append('|');
            }
            DataModel.Config.DestAddress = address.ToString();
            SnackbarQueue.MessageQueue.Enqueue("地址信息保存成功");
        }

        private void InitStartAddressLst()
        {
            _startAddressLst.Clear();
            string tempValue = DataModel.Config.StartAddress.Trim();
            string[] startAddressNames = tempValue.Split('|');
            for (int i = 0; i < startAddressNames.Length; i++)
            {
                if (string.IsNullOrEmpty(startAddressNames[i]))
                {
                    continue;
                }
                AddressItem item = new AddressItem();
                item.Id = i + 1;
                item.Value = startAddressNames[i];
                _startAddressLst.Add(item);
            }
        }

        private void InitDestAddressLst()
        {
            _destAddressLst.Clear();
            string tempValue = DataModel.Config.DestAddress.Trim();
            string[] startAddressNames = tempValue.Split('|');
            for (int i = 0; i < startAddressNames.Length; i++)
            {
                if (string.IsNullOrEmpty(startAddressNames[i]))
                {
                    continue;
                }
                AddressItem item = new AddressItem();
                item.Id = i + 1;
                item.Value = startAddressNames[i];
                _destAddressLst.Add(item);
            }
        }

       
    }

    public class AddressItem : NotifyObject
    {
        public int Id { get; set; }
        private string _value = string.Empty;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

    }

}
