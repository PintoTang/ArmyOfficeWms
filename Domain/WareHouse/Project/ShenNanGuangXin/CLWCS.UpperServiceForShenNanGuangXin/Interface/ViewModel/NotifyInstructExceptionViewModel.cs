using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using GalaSoft.MvvmLight;

namespace CLWCS.UpperServiceForHeFei.Interface.ViewModel
{
  public  class NotifyInstructExceptionViewModel:ViewModelBase, IInvokeCmd
    {
      public NotifyInstructExceptionMode DataModel { get; set; }
      public NotifyInstructExceptionViewModel(NotifyInstructExceptionMode dataModel)
        {
            DataModel = dataModel;
        }
        public string GetCmd()
        {
            return DataModel.ToString();
        }
    }
}
