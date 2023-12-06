using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using Infrastructrue.Ioc.DependencyFactory;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// CreateInOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateInOrderView : UserControl
    {
        private WmsDataService _wmsDataService;

        public CreateInOrderView()
        {
            InitializeComponent();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            InitCbTaskType();
            InitCbMaterialDesc();
            DataContext = CreateInOrderViewModel.SingleInstance;
            CreateInOrderViewModel.SingleInstance.BarcodeList.Clear();
        }


        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnScanRfid_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CbMaterialDesc_TextChanged(object sender, RoutedEventArgs e)
        {

            CbMaterialDesc.ItemsSource = _wmsDataService.GetMaterialList(CbMaterialDesc.Text);
            CbMaterialDesc.IsDropDownOpen = true;


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

        private void InitCbMaterialDesc()
        {
            CbMaterialDesc.SelectedValuePath = "MaterialCode";
            CbMaterialDesc.DisplayMemberPath = "MaterialDesc";
            CbMaterialDesc.ItemsSource = _wmsDataService.GetMaterialList(string.Empty);
        }


    }
}
