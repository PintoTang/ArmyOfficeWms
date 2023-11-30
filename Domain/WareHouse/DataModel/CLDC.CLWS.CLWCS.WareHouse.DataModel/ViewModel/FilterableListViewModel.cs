using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel.ViewModel
{
    public class FilterableListViewModel<T> : ViewModelBase where T : IFilterable
    {
        private DataObservablePool<T> _dataModel;

        public DataObservablePool<T> DataModel
        {
            get { return _dataModel; }
            set
            {
                _dataModel = value;
                RaisePropertyChanged();
            }
        }

        public FilterableListViewModel(DataObservablePool<T> dataModel)
        {
            DataModel = dataModel;
        }
    }
}
