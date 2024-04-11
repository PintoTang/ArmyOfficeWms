using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.TaskHandleCenter;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class AreaListViewModel : ViewModelBase
    {
        private string _curArea;
        private int? _curStatus;
        private string _curTaskType;
        private WmsDataService _wmsDataService;
        public ObservableCollection<Area> AreaList { get; set; }


        /// <summary>
        /// 当前选中的任务分类信息
        /// </summary>
        public Area SelectedValue { get; set; }


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

        public AreaListViewModel()
        {
            AreaList = new ObservableCollection<Area>();
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
            AreaList.Clear();
            try
            {
                var where = CombineSearchSql();
                OperateResult<List<Area>> accountListResult = _wmsDataService.GetAreaPageList(where);
                if (!accountListResult.IsSuccess)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                    return;
                }
                if (accountListResult.Content != null && accountListResult.Content.Count > 0)
                {
                    accountListResult.Content.ForEach(ite =>
                    {
                        AreaList.Add(ite);
                    });
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private Expression<Func<Area, bool>> CombineSearchSql()
        {
            Expression<Func<Area, bool>> whereLambda = t => t.IsDeleted == false;
            if (!string.IsNullOrEmpty(CurArea))
            {
                whereLambda = whereLambda.AndAlso(t => t.AreaCode == CurArea);
            }
            if (CurStatus.HasValue)
            {
                whereLambda = whereLambda.AndAlso(t => t.Status == CurStatus.Value);
            }
            return whereLambda;
        }


        private RelayCommand _createAreaCommand;
        public RelayCommand CreateAreaCommand
        {
            get
            {
                if (_createAreaCommand==null)
                {
                    _createAreaCommand = new RelayCommand(CreateArea);                    
                }
                return _createAreaCommand;
            }
        }

        private async void CreateArea()
        {
            CreateAreaView createArea = new CreateAreaView();
            await MaterialDesignThemes.Wpf.DialogHost.Show(createArea, "DialogHostWait");
        }


        private RelayCommand _editAreaCommand;
        public RelayCommand EditAreaCommand
        {
            get
            {
                if (_editAreaCommand == null)
                {
                    _editAreaCommand = new RelayCommand(EditArea);
                }
                return _editAreaCommand;
            }
        }

        private async void EditArea()
        {
            if (SelectedValue == null)
            {
                SnackbarQueue.Enqueue("请选择需要编辑的任务分类");
                return;
            }
            CreateAreaView createArea = new CreateAreaView(SelectedValue);
            await MaterialDesignThemes.Wpf.DialogHost.Show(createArea, "DialogHostWait");
        }


        private RelayCommand _deleteAreaCommand;

        public RelayCommand DeleteAreaCommand
        {
            get
            {
                if (_deleteAreaCommand == null)
                {
                    _deleteAreaCommand = new RelayCommand(DeleteArea);
                }
                return _deleteAreaCommand;
            }
        }

        private void DeleteArea()
        {
            if (SelectedValue == null)
            {
                SnackbarQueue.Enqueue("请选择需要删除的任务分类");
                return;
            }
            MessageBoxResult msgResult = MessageBoxEx.Show(string.Format("确定删除任务分类：{0}", SelectedValue.AreaName), "警告", MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.No)
            {
                return;
            }
            OperateResult deleteResult = _wmsDataService.DeleteArea(SelectedValue);
            SnackbarQueue.Enqueue(string.Format("操作结果：{0}", deleteResult.Message));
            Search();
        }

    }
}
