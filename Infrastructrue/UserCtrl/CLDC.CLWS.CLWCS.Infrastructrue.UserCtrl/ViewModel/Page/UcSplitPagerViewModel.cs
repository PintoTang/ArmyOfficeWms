using System;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using GalaSoft.MvvmLight;

namespace CLDC.Infrastructrue.UserCtrl.ViewModel.Page
{
    /// <summary>
    /// 分页控件ViewModel
    /// </summary>
    public class UcSplitPagerViewModel : ViewModelBase
    {
        public event Action InvalidPage;
        private long _totalPageSize;
        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPageSize
        {
            get
            {
                return (long)Math.Ceiling((double)TotalCount / PageSize);
            }
            set
            {
                _totalPageSize = value;
                RaisePropertyChanged();
            }
        }

        private long _pageSize = 30;
        /// <summary>
        /// 每页数据
        /// </summary>
        public long PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RaisePropertyChanged();
            }

        }
        private long _pageIndex = 1;
        /// <summary>
        /// 当前显示的页 索引
        /// </summary>
        public long PageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (_pageIndex != value)
                {
                    _pageIndex = value;
                    RaisePropertyChanged("PageIndex");
                }
            }
        }
        private long _currentPage=1;
        /// <summary>
        /// 当前页数
        /// </summary>
        public long CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }

        }
        private long _totalCount = 0;
        /// <summary>
        /// 总条数
        /// </summary>
        public long TotalCount
        {
            get
            {
                return _totalCount;
            }
            set
            {
                _totalCount = value;
                RaisePropertyChanged();
                RaisePropertyChanged("TotalPageSize");
            }
        }

        private string inputPage;
        /// <summary>
        /// 文本框输入的页码
        /// </summary>
        public string InputPage
        {
            get { return inputPage; }
            set
            {
                long gopage;
                if (long.TryParse(value, out gopage))
                {
                    inputPage = value;
                    RaisePropertyChanged("InputPage");
                }
                else
                {
                    OnInvalidPage();
                }
            }
        }

        #region 事件
        private RelayCommand firstPageCommand;
        public RelayCommand Firstpagecommand
        {
            get
            {
                if (firstPageCommand == null)
                {
                    firstPageCommand = new RelayCommand(SwitchToFirstPage);
                }
                return firstPageCommand;
            }
        }
        private RelayCommand previousPageCommand;
        public RelayCommand PreviousCommand
        {
            get
            {
                if (previousPageCommand == null)
                {
                    previousPageCommand = new RelayCommand(SwitchToPreviousPage,CanPreviousPage);
                }
                return previousPageCommand;
            }
        }

        private bool CanPreviousPage()
        {
            return PageIndex > 1;
        }

        private RelayCommand _nextPageCommond;
        public RelayCommand NextPageCommand
        {
            get
            {
                if (_nextPageCommond == null)
                {
                    _nextPageCommond = new RelayCommand(SwitchToNextPage,CanNextPage);
                }
                return _nextPageCommond;
            }
        }

        private bool CanNextPage()
        {
            return PageIndex < TotalPageSize;
        }

        private RelayCommand lastPageCommand;
        public RelayCommand Lastpagecommand
        {
            get
            {
                if (lastPageCommand == null)
                {
                    lastPageCommand = new RelayCommand(SwitchToLastPage);
                }
                return lastPageCommand;
            }
        }

        private RelayCommand _goCommand;
        public RelayCommand GoCommand  //跳转到对应的页码
        {
            get
            {
                if (_goCommand == null)
                {
                    _goCommand = new RelayCommand(GoInputPage);
                }
                return _goCommand;
            }
        }
        public void OnInvalidPage()
        {
            if (InvalidPage != null)
            {
                InvalidPage();
            }
        }
        #endregion

        /// <summary>
        /// 首页
        /// </summary>
        private void SwitchToFirstPage()
        {
            CurrentPage = 1;
            ChangContent(CurrentPage);
        }
        /// <summary>
        /// 上一页
        /// </summary>
        private void SwitchToPreviousPage()
        {
            --CurrentPage;
            ChangContent(CurrentPage);
        }
        /// <summary>
        /// 下一页
        /// </summary>
        private void SwitchToNextPage()
        {
            ++CurrentPage;
            ChangContent(CurrentPage);
        }

        /// <summary>
        /// 尾页
        /// </summary>
        private void SwitchToLastPage()
        {
            CurrentPage = TotalPageSize;
            ChangContent(TotalPageSize);
        }
        /// <summary>
        /// 分页改变触发事件
        /// </summary>
        public event Action PageChange;

        private void ChangContent(long index)
        {
            PageIndex = index;
            //事件触发
            if (PageChange != null)
                PageChange();
        }

        private void GoInputPage()
        {
            long gopage;
            if (long.TryParse(InputPage, out gopage))
            {
                if (gopage >= 1 && gopage <= TotalPageSize)
                {
                    CurrentPage = gopage;
                    ChangContent(gopage);
                }
                else
                {
                    OnInvalidPage();
                }
            }
        }
    }
}
