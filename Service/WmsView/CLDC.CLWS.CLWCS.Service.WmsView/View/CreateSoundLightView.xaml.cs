using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.Service.WmsView.Tools;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.Infrastructrue.UserCtrl;
using Infrastructrue.Ioc.DependencyFactory;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// CreateSoundLightView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSoundLightView : UserControl
    {
        private WmsDataService _wmsDataService;
        private long _soundLightId;
        private SoundLight _soundLight;

        public CreateSoundLightView()
        {
            InitializeComponent();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            BindComboBox();
            lbTitle.Content = "新增声光配置";
        }

        private void BindComboBox()
        {
            var list = _wmsDataService.GetAreaList(string.Empty);
            cbArea.ItemsSource = list;
        }

        public CreateSoundLightView(SoundLight model)
        {
            InitializeComponent();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            BindComboBox();
            lbTitle.Content = "编辑声光配置";
            _soundLightId = model.Id;
            cbArea.SelectedValue = model.Area;
            tbTeam.Text = model.Team;
            tbSoundContent.Text = model.SoundContent;
            tbLightCode.Text = model.LightCode;
            _soundLight = model;
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.cbArea.SelectedValue == null)
            {
                MessageBoxEx.Show("请选择任务分类", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.tbTeam.Text))
            {
                MessageBoxEx.Show("任务分队不能为空", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.tbSoundContent.Text))
            {
                MessageBoxEx.Show("播报内容不能为空", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.tbLightCode.Text))
            {
                MessageBoxEx.Show("灯光指令不能为空", "提示", MessageBoxButton.OK);
                return;
            }
            if (_soundLightId == 0)
            {
                SoundLight soundLight=new SoundLight();
                soundLight.Id = Snowflake.NewId();
                soundLight.Area = cbArea.SelectedValue.ToString();
                soundLight.Team =tbTeam.Text;
                soundLight.SoundContent = tbSoundContent.Text;
                soundLight.LightCode=tbLightCode.Text;
                soundLight.CreatedTime = DateTime.Now;
                soundLight.IsDeleted = false;
                soundLight.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;
                
                OperateResult createResult = _wmsDataService.CreateSoundLight(soundLight);
                if (!createResult.IsSuccess)
                {
                    MessageBoxEx.Show("新增声光配置失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                    return;
                }
                SnackbarQueue.MessageQueue.Enqueue("声光配置新增成功");
            }
            else
            {
                SoundLight soundLight = _soundLight;
                soundLight.Area = cbArea.SelectedValue.ToString();
                soundLight.Team = tbTeam.Text;
                soundLight.SoundContent = tbSoundContent.Text;
                soundLight.LightCode = tbLightCode.Text;
                soundLight.CreatedTime = DateTime.Now;
                soundLight.CreatedUserName = CookieService.CurSession.UserInfo.Account.AccCode;

                OperateResult createResult = _wmsDataService.UpdateSoundLight(soundLight);
                if (!createResult.IsSuccess)
                {
                    MessageBoxEx.Show("修改声光配置失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                    return;
                }
                SnackbarQueue.MessageQueue.Enqueue("声光配置修改成功");
            }
            SoundLightViewModel.SingleInstance.SearchCommand.Execute(null);
            DialogHost.CloseDialogCommand.Execute("操作完成", this);
        }


    }
}
