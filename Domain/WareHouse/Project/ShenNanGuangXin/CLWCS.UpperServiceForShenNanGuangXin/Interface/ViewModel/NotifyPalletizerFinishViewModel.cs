using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;

namespace CLWCS.UpperServiceForHeFei.Interface.ViewModel
{
    public class NotifyPalletizerFinishViewModel : IInvokeCmd
    {
       public NotifyPalletizerFinishCmdMode DataModel { get; set; }
       public NotifyPalletizerFinishViewModel(NotifyPalletizerFinishCmdMode dataModel)
       {
           DataModel = dataModel;
       }
       public string GetCmd()
       {
           return DataModel.ToString();
       }
    }
}
