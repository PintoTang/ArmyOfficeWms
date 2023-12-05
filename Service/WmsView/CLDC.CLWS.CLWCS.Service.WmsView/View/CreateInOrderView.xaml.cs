using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Framework;
using System.Text.RegularExpressions;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// CreateInOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateInOrderView : UserControl
    {
        public event BarcodeCallback<List<string>> OnReceiveBarcode;
        public DeviceName DeviceName { get; set; }
        public int DeviceId { get; set; }
        public IdentifyDeviceCommForClouRfid HopelandRfid { get; set; }

        public CreateInOrderView()
        {
            InitializeComponent();
            InitCbTaskType();
            this.DeviceName = new DeviceName("电子标签识别", 1);
            this.DeviceId = 102401;
            HopelandRfid = new IdentifyDeviceCommForClouRfid();
            HopelandRfid.OnReceiveBarcode += HopelandRfid_OnReceiveBarcode;
            HopelandRfid.Initialize(DeviceId, DeviceName);
        }

        private void HopelandRfid_OnReceiveBarcode(DeviceName deviceName, List<string> barcode, params object[] para)
        {
            List<Barcodex> BarcodeList = new List<Barcodex>();
            for (int i = 0; i < barcode.Count; i++)
            {
                BarcodeList.Add(new Barcodex() { Barcode = barcode[i], SN = i+1 });
            }
            this.Dispatcher.BeginInvoke(new Action(() => InOrderGrid.ItemsSource = BarcodeList));            
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnScanRfid_Click(object sender, RoutedEventArgs e)
        {
            HopelandRfid.SendCommandAsync("test");
        }

        private readonly Dictionary<TaskTypeEnum, string> _taskTypeDict = new Dictionary<TaskTypeEnum, string>();
        public Dictionary<TaskTypeEnum, string> TaskTypeDict
        {
            get
            {
                if (_taskTypeDict.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(TaskTypeEnum)))
                    {
                        TaskTypeEnum em = (TaskTypeEnum)value;
                        _taskTypeDict.Add(em, em.GetDescription());
                    }
                }
                return _taskTypeDict;
            }
        }

        private void InitCbTaskType()
        {
            CbTaskType.SelectedValuePath = "Key";
            CbTaskType.DisplayMemberPath = "Value";
            CbTaskType.ItemsSource = TaskTypeDict;
        }


    }

    public class Barcodex
    {
        public int SN { get; set; }
        public string Barcode { get; set; }
    }
}
