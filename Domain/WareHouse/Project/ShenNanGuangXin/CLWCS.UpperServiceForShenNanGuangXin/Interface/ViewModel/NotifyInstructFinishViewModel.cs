using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using GalaSoft.MvvmLight;

namespace CLWCS.UpperServiceForHeFei.Interface.ViewModel
{
    public class NotifyInstructFinishViewModel : ViewModelBase, IInvokeCmd
    {
        public NotifyInstructFinishCmdMode DataModel { get; set; }
        public NotifyInstructFinishViewModel(NotifyInstructFinishCmdMode dataModel)
        {
            DataModel = dataModel;
        }
        public string GetCmd()
        {
            return DataModel.ToString();
        }
    }
}
