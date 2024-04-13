using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class SoundLightViewModel : ViewModelBase
    {
        private string _curArea;
        private int? _curStatus;
        private string _curTaskType;
        private WmsDataService _wmsDataService;
        public ObservableCollection<SoundLight> SoundLightList { get; set; }


        /// <summary>
        /// 当前选中的任务分类信息
        /// </summary>
        public SoundLight SelectedValue { get; set; }


        /// <summary>
        /// 当前搜索的区域
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

        /// <summary>
        /// 当前搜索的启用状态
        /// </summary>
        public int? CurStatus
        {
            get { return _curStatus; }
            set
            {
                _curStatus = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的任务分类
        /// </summary>
        public string CurTaskType
        {
            get { return _curTaskType; }
            set
            {
                _curTaskType = value;
                RaisePropertyChanged();
            }
        }

        private readonly Dictionary<InvStatusEnum, string> _invStatusDict = new Dictionary<InvStatusEnum, string>();
        public Dictionary<InvStatusEnum, string> InvStatusDict
        {
            get
            {
                if (_invStatusDict.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(InvStatusEnum)))
                    {
                        InvStatusEnum em = (InvStatusEnum)value;
                        _invStatusDict.Add(em, em.GetDescription());
                    }
                }
                return _invStatusDict;
            }
        }

        private static readonly Lazy<SoundLightViewModel> lazy = new Lazy<SoundLightViewModel>(() =>
                                        new SoundLightViewModel(), LazyThreadSafetyMode.PublicationOnly);
        public static SoundLightViewModel SingleInstance
        {
            get
            {
                return lazy.Value;
            }
        }

        public SoundLightViewModel()
        {
            SoundLightList = new ObservableCollection<SoundLight>();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
        }


        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(Search);
                }
                return _searchCommand;
            }
        }

        private void Search()
        {
            SoundLightList.Clear();
            try
            {
                var where = CombineSearchSql();
                OperateResult<List<SoundLight>> accountListResult = _wmsDataService.GetSoundLightPageList(where);
                if (!accountListResult.IsSuccess)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                    return;
                }
                if (accountListResult.Content != null && accountListResult.Content.Count > 0)
                {
                    accountListResult.Content.ForEach(ite =>
                    {
                        SoundLightList.Add(ite);
                    });
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private Expression<Func<SoundLight, bool>> CombineSearchSql()
        {
            Expression<Func<SoundLight, bool>> whereLambda = t => t.IsDeleted == false;
            if (!string.IsNullOrEmpty(CurArea))
            {
                whereLambda = whereLambda.AndAlso(t => t.Area == CurArea);
            }
            return whereLambda;
        }


        private RelayCommand _createSoundLightCommand;
        public RelayCommand CreateSoundLightCommand
        {
            get
            {
                if (_createSoundLightCommand == null)
                {
                    _createSoundLightCommand = new RelayCommand(CreatSoundLight);
                }
                return _createSoundLightCommand;
            }
        }

        private async void CreatSoundLight()
        {
            CreateSoundLightView soundLightView = new CreateSoundLightView();
            await MaterialDesignThemes.Wpf.DialogHost.Show(soundLightView, "DialogHostWait");
        }


        private RelayCommand _editSoundLightCommand;
        public RelayCommand EditSoundLightCommand
        {
            get
            {
                if (_editSoundLightCommand == null)
                {
                    _editSoundLightCommand = new RelayCommand(EditSoundLight);
                }
                return _editSoundLightCommand;
            }
        }

        private async void EditSoundLight()
        {
            if (SelectedValue == null)
            {
                SnackbarQueue.Enqueue("请选择需要编辑的任务分类");
                return;
            }
            CreateSoundLightView createArea = new CreateSoundLightView(SelectedValue);
            await MaterialDesignThemes.Wpf.DialogHost.Show(createArea, "DialogHostWait");
        }


        private RelayCommand _deleteSoundLightCommand;

        public RelayCommand DeleteSoundLightCommand
        {
            get
            {
                if (_deleteSoundLightCommand == null)
                {
                    _deleteSoundLightCommand = new RelayCommand(DeleteSoundLight);
                }
                return _deleteSoundLightCommand;
            }
        }

        private void DeleteSoundLight()
        {
            if (SelectedValue == null)
            {
                SnackbarQueue.Enqueue("请选择需要删除的声光配置");
                return;
            }
            MessageBoxResult msgResult = MessageBoxEx.Show("确定此删除声光配置？", "警告", MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.No)
            {
                return;
            }
            OperateResult deleteResult = _wmsDataService.DeleteSoundLight(SelectedValue);
            SnackbarQueue.Enqueue(string.Format("操作结果：{0}", deleteResult.Message));
            Search();
        }

    }
}
