using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.Interface
{
	public interface INotifyAttributeChange
	{
		/// <summary>
		/// 通知属性变化
		/// </summary>
		/// <param name="attributeName">属性名</param>		
		void NotifyAttributeChange(string attributeName,object newValue);
	}
}
