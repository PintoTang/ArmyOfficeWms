using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 堆垛机列表
    /// </summary>
    public static class StackingCraneCache
    {
         static BindingList<StackingCraneItem> cacheStackingCraneInfoList = new BindingList<StackingCraneItem>();
        /// <summary>
         /// 堆垛机列表
        /// </summary>
         public static BindingList<StackingCraneItem> CacheStackingCraneInfoList
         {
             set { cacheStackingCraneInfoList = value; }
             get { return cacheStackingCraneInfoList; }
         }


    }
}
