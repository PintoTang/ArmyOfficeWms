using System.Windows;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    /// <summary>
    /// 设备界面基类
    /// </summary>
    public abstract class ViewAbstract : UserControl
    {
        protected void InitilizeViewModel(WareHouseViewModelBase viewModel)
        {
            ViewModel = viewModel;
            Id = viewModel.Id;
            ShowName = viewModel.Name;
            GroupName = viewModel.GroupName;
        }

        protected void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.NewValue as WareHouseViewModelBase;
            InitilizeViewModel(viewModel);
        }
        public WareHouseViewModelBase ViewModel { get; set; }

        public int Id { get; set; }

        public string ShowName { get; set; }

        public string GroupName { get; set; }

        public virtual  bool IsHasAnyTask()
        {
            IDoTask taskDevice=ViewModel as IDoTask;
            if (taskDevice != null)
            {
                return taskDevice.IsHasTask();
            }
            return false;
        }

        public virtual bool IsHasError()
        {
            IDoTask taskDevice = ViewModel as IDoTask;
            if (taskDevice != null)
            {
                return taskDevice.IsHasError();
            }
            return false;
        }

        public void Show()
        {
            this.Visibility= Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

    }
}
