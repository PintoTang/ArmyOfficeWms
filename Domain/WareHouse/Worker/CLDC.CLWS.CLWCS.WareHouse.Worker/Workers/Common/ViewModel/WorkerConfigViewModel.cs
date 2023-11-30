using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.Infrastructrue.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel
{
    public class WorkerConfigViewModel<TWorker, TConfig> : ViewModelBase
        where TWorker : WorkerBaseAbstract
        where TConfig : ICreateMenuItem
    {
        public WorkerConfigViewModel(TWorker worker,TConfig config)
        {
            Worker = worker;
            CurConfig = config;
            ItemSelectedCommand = new RelayCommand<object>(OnItemSelected);
            InitMenuItem();
        }

        private void InitMenuItem()
        {
            MenuItems = CurConfig.CreateMenuItem();
            MenuItem firstOrDefault = MenuItems.FirstOrDefault();
            if (firstOrDefault != null) SelectedItem = firstOrDefault.UserView;
        }

        public TConfig CurConfig { get; set; }
        public TWorker Worker { get; set; }


        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(SaveConfig);
                }
                return _saveCommand;
            }
        }


        /// <summary>
        /// 添加数据到
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                MessageBoxResult rt = MessageBoxEx.Show("您确认保存？", "警告", MessageBoxButton.YesNo);
                if (rt.Equals(MessageBoxResult.Yes))
                {
                    OperateResult<string> projectPathResult = SystemConfig.Instance.GetProjectPath();
                    if (!projectPathResult.IsSuccess)
                    {
                        string msg = string.Format("保存失败，原因：{0}", projectPathResult.Message);
                        MessageBoxEx.Show(msg, "错误");
                        return;
                    }
                    XmlOperator doc = ConfigHelper.GetCoordinationConfig; 
                    XmlNode xmlNode = doc.GetXmlNode("Coordination", "Id", Worker.Id.ToString());

                    string outString = XmlSerializerHelper.Serialize(CurConfig, typeof(TConfig));

                    OperateResult replaceResult = doc.ReplaceChild(xmlNode, doc.GetXmlNodeFromString(outString));
                    if (!replaceResult.IsSuccess)
                    {
                        SnackbarQueue.MessageQueue.Enqueue("保存失败" + replaceResult.Message);
                        return;
                    }
                    OperateResult saveRunningPathResult = doc.Save();
                    if (!saveRunningPathResult.IsSuccess)
                    {
                        SnackbarQueue.MessageQueue.Enqueue("保存到运行文件失败" + saveRunningPathResult.Message);
                        return;
                    }
                    string projectPath = projectPathResult.Content + "\\CoordinationConfig.xml";

                    OperateResult saveProjectPathResult = doc.Save(projectPath);
                    if (!saveProjectPathResult.IsSuccess)
                    {
                        SnackbarQueue.MessageQueue.Enqueue("保存项目文件失败" + saveProjectPathResult.Message);
                        return;
                    }

                    OperateResult deviceUpateResult = Worker.UpdateProperty();
                    if (!deviceUpateResult.IsSuccess)
                    {
                        SnackbarQueue.MessageQueue.Enqueue("保存到内存失败" + deviceUpateResult.Message);
                        return;
                    }
                    SnackbarQueue.MessageQueue.Enqueue("保存成功");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("配置保存失败:" + OperateResult.ConvertException(ex));
            }
        }


        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                Set(ref _selectedItem, value);
            }
        }

        private void OnItemSelected(object selectedItem)
        {
            SelectedItem = selectedItem;
        }
        public ICommand ItemSelectedCommand { get; set; }
        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set { _menuItems = value; }
        }

    }
}
