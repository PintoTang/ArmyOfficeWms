using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;

namespace CLWCS.UpperServiceForHeFei.Interface.ViewModel
{
    public class NotifyUnstackFinishCmdViewModel : IInvokeCmd
    {
        public NotifyUnstackFinishCmdViewModel(NotifyUnstackFinishCmdMode dataModel)
        {
            DataModel = dataModel;
        }
        public NotifyUnstackFinishCmdMode DataModel{get; set; }
        public string GetCmd()
        {
            return DataModel.ToString();
        }

        private Dictionary<StackFinishActionEnum, string> _dicActionTypeList = new Dictionary<StackFinishActionEnum, string>();
         /// <summary>
         /// 是否需要请求地址集合
         /// </summary>
        public Dictionary<StackFinishActionEnum, string> DicActionTypeList
         {
             get
             {
                 if (_dicActionTypeList.Count == 0)
                 {
                     foreach (var value in Enum.GetValues(typeof(StackFinishActionEnum)))
                     {
                         StackFinishActionEnum em = (StackFinishActionEnum)value;
                         _dicActionTypeList.Add(em, em.GetDescription());
                     }
                 }
                 return _dicActionTypeList;
             }
             set
             {
                 _dicActionTypeList = value;
             }
         }

        

    }
}
