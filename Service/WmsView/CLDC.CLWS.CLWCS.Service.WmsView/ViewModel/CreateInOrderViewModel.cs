using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
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
        private ObservableCollection<Barcodex> _barcodeList { get; set; }
        public ObservableCollection<Barcodex> BarcodeList 
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

        private static readonly Lazy<CreateInOrderViewModel> lazy = new Lazy<CreateInOrderViewModel>(
            () => new CreateInOrderViewModel(), LazyThreadSafetyMode.PublicationOnly);//测试是否复现： ValueFactory 尝试访问此实例的 Value 属性
        public static CreateInOrderViewModel SingleInstance
        {
            get
            {
                return lazy.Value;
            }
        }

        private WmsDataService _wmsDataService;

        public CreateInOrderViewModel()
        {
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            this.BarcodeList=new ObservableCollection<Barcodex>();
            this.DeviceName = new DeviceName("电子标签识别", 1);
            this.DeviceId = 102401;
            HopelandRfid = new IdentifyDeviceCommForClouRfid();
            HopelandRfid.BarcodeChangedEvent += HopelandRfid_BarcodeChangedEvent;
            HopelandRfid.Initialize(DeviceId, DeviceName);
        }

        private void HopelandRfid_BarcodeChangedEvent(string barcode)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
               
                if (this.BarcodeList.FirstOrDefault(x => x.Barcode == barcode)==null)
                    this.BarcodeList.Add(new Barcodex() { Barcode = barcode, SN = this.BarcodeList.Count + 1 });
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

    public class Barcodex
    {
        public int SN { get; set; }
        public string Barcode { get; set; }
    }

}
