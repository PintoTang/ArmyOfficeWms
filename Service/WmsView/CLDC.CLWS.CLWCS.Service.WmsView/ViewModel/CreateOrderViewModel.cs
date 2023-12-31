﻿using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using static CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.ClouRFID.Model.RFIDForClou;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class CreateOrderViewModel: ViewModelBase
    {
        #region 属性与变量
        public DeviceName DeviceName { get; set; }
        public int DeviceId { get; set; }
        public IdentifyDeviceCommForClouRfid HopelandRfid { get; set; }

        public event BarcodeEventHandler BarcodeChangedEvent;
        private ObservableCollection<RfidBarcode> _barcodeList { get; set; }
        public ObservableCollection<Reason> ReasonList { get; set; }
        public ObservableCollection<Area> AreaList { get; set; }
        public ObservableCollection<TaskTeam> TeamList { get; set; }
        public ObservableCollection<Shelf> ShelfList { get; set; }

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
        private string _curReason;
        /// <summary>
        /// 当前选择的事由
        /// </summary>
        public string CurReason
        {
            get { return _curReason; }
            set
            {
                _curReason = value;
                RaisePropertyChanged();
            }
        }

        private string _curShelfName;
        /// <summary>
        /// 当前选择的事由
        /// </summary>
        public string CurShelfName
        {
            get { return _curShelfName; }
            set
            {
                _curShelfName = value;
                RaisePropertyChanged();
            }
        }

        private string _curShelf;
        /// <summary>
        /// 当前选择的事由
        /// </summary>
        public string CurShelf
        {
            get { return _curShelf; }
            set
            {
                _curShelf = value;
                RaisePropertyChanged();
            }
        }

        private string _curArea;
        /// <summary>
        /// 当前选择的事由
        /// </summary>
        public string CurArea
        {
            get { return _curArea; }
            set
            {
                _curArea = value;
                RaisePropertyChanged();
            }
        }

        private string _curAreaName;
        /// <summary>
        /// 当前选择的事由
        /// </summary>
        public string CurAreaName
        {
            get { return _curAreaName; }
            set
            {
                _curAreaName = value;
                RaisePropertyChanged();
            }
        }

        private string _curTeam;
        /// <summary>
        /// 当前选择的分队
        /// </summary>
        public string CurTeam
        {
            get { return _curTeam; }
            set
            {
                _curTeam = value;
                RaisePropertyChanged();
            }
        }        

        private static readonly Lazy<CreateOrderViewModel> lazy = new Lazy<CreateOrderViewModel>(() => 
                                        new CreateOrderViewModel(), LazyThreadSafetyMode.PublicationOnly);
        public static CreateOrderViewModel SingleInstance
        {
            get
            {
                return lazy.Value;
            }
        }
        #endregion

        public CreateOrderViewModel()
        {
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            this.BarcodeList = new ObservableCollection<RfidBarcode>();
            this.ReasonList = new ObservableCollection<Reason>();
            this.AreaList = new ObservableCollection<Area>();
            this.TeamList = new ObservableCollection<TaskTeam>();
            this.ShelfList =new ObservableCollection<Shelf>();
            this.DeviceName = new DeviceName("电子标签识别", 1);
            this.DeviceId = 102401;
            HopelandRfid = new IdentifyDeviceCommForClouRfid();
            HopelandRfid.BarcodeChangedEvent += HopelandRfid_BarcodeChangedEvent;
            HopelandRfid.Initialize(DeviceId, DeviceName);

            InitCbReason(); InitCbArea(); InitTeam(); InitShelf();

            #region  测试数据
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780220000000001E3D28", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780220000000001E3B4D", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780200000000001E3C3D", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780200000000001E34E3", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780200000000001E3330", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780220000000001E3C3B", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780220000000001E3D28", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780220000000001E3B4D", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780200000000001E3C3D", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeList.Add(new RfidBarcode() { Barcode = "E182780200000000001E34E3", SN = this.BarcodeList.Count + 1, MaterialDesc = "灭火器" });
            this.BarcodeCount=this.BarcodeList.Count.ToString();
            #endregion
        }

        private void InitCbReason()
        {
            ReasonList.Clear();
            try
            {
                List<Reason> accountListResult = ReasonConfig.Instance.ReasonList;
                if (accountListResult.Count > 0)
                {
                    accountListResult.ForEach(ite => ReasonList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private void InitCbArea()
        {
            AreaList.Clear();
            try
            {
                List<Area> accountListResult = _wmsDataService.GetAreaList(string.Empty);
                if (accountListResult.Count > 0)
                {
                    accountListResult.ForEach(ite => AreaList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }
        private void InitTeam()
        {
            TeamList.Clear();
            {
                try
                {
                    List<TaskTeam> accountListResult = TaskTeamConfig.Instance.TaskTeamList;
                    if (accountListResult.Count > 0)
                    {
                        accountListResult.ForEach(ite => TeamList.Add(ite));
                    }
                }
                catch (Exception ex)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
                }
            }
        }
        private void InitShelf()
        {
            ShelfList.Clear();
            try
            {
                List<Shelf> accountListResult = _wmsDataService.GetShelfList(string.Empty);
                if (accountListResult.Count > 0)
                {
                    accountListResult.ForEach(ite => ShelfList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private void HopelandRfid_BarcodeChangedEvent(string barcode)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (this.BarcodeList.FirstOrDefault(x => x.Barcode == barcode) == null)
                {
                    Inventory inventory = _wmsDataService.GetInventory(barcode);
                    Shelf shelf = _wmsDataService.GetShelf(barcode);
                    if (shelf != null)
                    {
                        CurShelf = shelf.Code;
                        CurShelfName = shelf.Name;
                        CurArea = shelf.AreaCode;
                        CurAreaName=shelf.AreaName;
                        CurTeam = inventory?.AreaTeam;
                    }
                    else
                    {
                        this.BarcodeList.Add(new RfidBarcode()
                        {
                            Barcode = barcode,
                            SN = this.BarcodeList.Count + 1,
                            MaterialDesc = inventory?.MaterialDesc
                        });
                        BarcodeCount = this.BarcodeList.Count.ToString();
                    }
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
