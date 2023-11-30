using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WHSE.Monitor.Framework.UserControls
{
	public class UIHelper : Application
	{
		//刷新界面
		private static DispatcherOperationCallback
			 exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
		public static void DoEvents()
		{
			DispatcherFrame nestedFrame = new DispatcherFrame();
			DispatcherOperation exitOperation =
				Dispatcher.CurrentDispatcher.BeginInvoke(
				DispatcherPriority.Background,
				exitFrameCallback, nestedFrame);
			Dispatcher.PushFrame(nestedFrame);

			if (exitOperation.Status != DispatcherOperationStatus.Completed)
			{
				exitOperation.Abort();
			}
		}
		private static object ExitFrame(object state)
		{
			DispatcherFrame frame = state as DispatcherFrame;
			frame.Continue = false;
			return null;
		}
	}
}
