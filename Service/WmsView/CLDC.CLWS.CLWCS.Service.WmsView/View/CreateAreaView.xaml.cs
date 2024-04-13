using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.Service.WmsView.Tools;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using Infrastructrue.Ioc.DependencyFactory;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// CreateAreaView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateAreaView : UserControl
    {
        private WmsDataService _wmsDataService;
        private long _areaId;
        private Area _area;

        public CreateAreaView()
        {
            InitializeComponent();
            lbTitle.Content= "新增任务分类";
        }

        public CreateAreaView(Area model)
        {
            InitializeComponent();
            lbTitle.Content = "编辑任务分类";
            _areaId = model.Id;
            tbAreaCode.Text = model.AreaCode;
            tbAreaName.Text = model.AreaName;
            tbRow.Value = (int)model.ROW;
            tbColumn.Value = (int)model.COLUMN;
            if (model.Status == 1)
                isShow.IsChecked = true;
            else
                isHide.IsChecked = true;
            _area=model;
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbAreaCode.Text))
            {
                MessageBoxEx.Show("任务分类编码不能为空", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.tbAreaName.Text))
            {
                MessageBoxEx.Show("任务分类名称不能为空", "提示", MessageBoxButton.OK);
                return;
            }
            if (_areaId == 0)
            {
                Area area = new Area();
                area.Id = Snowflake.NewId();
                area.AreaCode = this.tbAreaCode.Text;
                area.AreaName = this.tbAreaName.Text;
                area.ROW = this.tbRow.Value;
                area.COLUMN = this.tbColumn.Value;
                area.Status = isShow.IsChecked == true ? 1 : 0;
                area.CreatedTime = DateTime.Now;
                area.DisplayOrder = 1;
                area.IsDeleted = false;
                area.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;

                _wmsDataService = DependencyHelper.GetService<WmsDataService>();
                OperateResult createResult = _wmsDataService.CreateNewArea(area);
                if (!createResult.IsSuccess)
                {
                    MessageBoxEx.Show("新增任务分类失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                    return;
                }
                SnackbarQueue.MessageQueue.Enqueue("任务分类新增成功");
            }
            else
            {
                Area area = _area;
                area.AreaCode = this.tbAreaCode.Text;
                area.AreaName = this.tbAreaName.Text;
                area.ROW = this.tbRow.Value;
                area.COLUMN = this.tbColumn.Value;
                area.Status = isShow.IsChecked == true ? 1 : 0;
                area.CreatedTime= DateTime.Now;
                area.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;

                _wmsDataService = DependencyHelper.GetService<WmsDataService>();
                OperateResult createResult = _wmsDataService.UpdateArea(area);
                if (!createResult.IsSuccess)
                {
                    MessageBoxEx.Show("修改任务分类失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                    return;
                }
                SnackbarQueue.MessageQueue.Enqueue("任务分类修改成功");       
            }
            AreaListViewModel.SingleInstance.SearchCommand.Execute(null);
            DialogHost.CloseDialogCommand.Execute("操作完成", this);
        }


    }
}
