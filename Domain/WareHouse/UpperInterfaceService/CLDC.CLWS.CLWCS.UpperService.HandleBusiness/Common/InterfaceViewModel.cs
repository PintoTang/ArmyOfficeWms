using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.UpperService.HandleBusiness.Common
{
    public class InterfaceViewModel<T> : ViewModelBase, IInvokeCmd
    {
        public T DataModel { get; set; }
        public InterfaceViewModel(T dataModel)
        {
            DataModel = dataModel;
        }
        public string GetCmd()
        {
           return DataModel.ToString();
        }
    }
}
