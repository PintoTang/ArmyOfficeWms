using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;


namespace WHSE.Monitor.Client.BizService.Abs
{
	public interface IPackageMoveOutput
	{
		event PackageMoveInfoOutputDelegate PackageMoveInfoOutputEvent;
	}
}
