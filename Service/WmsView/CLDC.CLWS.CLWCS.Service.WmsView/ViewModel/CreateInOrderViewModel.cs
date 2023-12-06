using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using static CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.ClouRFID.Model.RFIDForClou;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class CreateInOrderViewModel: ViewModelBase
    {
        public DeviceName DeviceName { get; set; }
        public int DeviceId { get; set; }
        public IdentifyDeviceCommForClouRfid HopelandRfid { get; set; }

        public event BarcodeEventHandler BarcodeChangedEvent;
        private ObservableCollection<RfidBarcode> _barcodeList { get; set; }

        private WmsDataService _wmsDataService;
        public ObservableCollection<RfidBarcode> BarcodeList 
        {
            get
            {
                return _barcodeList;
            }
            set
            {
                _barcodeList = value;
                RaisePropertyChanged(() => BarcodeList);
            }
        }
        private string _barcodeCount;
        public string BarcodeCount
        {
            get
            {
                return _barcodeCount;
            }
            set
            {
                _barcodeCount = value;
                RaisePropertyChanged(() => BarcodeCount);
            }
        }

        private static readonly Lazy<CreateInOrderViewModel> lazy = new Lazy<CreateInOrderViewModel>(() => 
                                        new CreateInOrderViewModel(), LazyThreadSafetyMode.PublicationOnly);
        public static CreateInOrderViewModel SingleInstance
        {
            get
            {
                return lazy.Value;
            }
        }

        public CreateInOrderViewModel()
        {
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            this.BarcodeList=new ObservableCollection<RfidBarcode>();
            this.DeviceName = new DeviceName("电子标签识别", 1);
            this.DeviceId = 102401;
            HopelandRfid = new IdentifyDeviceCommForClouRfid();
            HopelandRfid.BarcodeChangedEvent += HopelandRfid_BarcodeChangedEvent;
            HopelandRfid.Initialize(DeviceId, DeviceName);

            ///////测试数据///////
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "Ex1111", SN = this.BarcodeList.Count + 1 });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "Ex2222", SN = this.BarcodeList.Count + 1 });
        }

        private void HopelandRfid_BarcodeChangedEvent(string barcode)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (this.BarcodeList.FirstOrDefault(x => x.Barcode == barcode) == null)
                {
                    this.BarcodeList.Add(new RfidBarcode() { Barcode = barcode, SN = this.BarcodeList.Count + 1 });
                    BarcodeCount = this.BarcodeList.Count.ToString();
                }
            }));
        }

        private RelayCommand _scanCommand;

        public RelayCommand ScanCommand
        {
            get
            {
                if (_scanCommand == null)
                {
                    _scanCommand = new RelayCommand(BeginScan);
                }
                return _scanCommand;
            }
        }

        private RelayCommand _stopCommand;

        public RelayCommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand(StopScan);
                }
                return _stopCommand;
            }
        }

        private  void BeginScan()
        {
            this.BarcodeList.Clear();
            HopelandRfid.SendCommand();
        }

        private void StopScan()
        {
            HopelandRfid.StopCommand();
        }


    }
}
