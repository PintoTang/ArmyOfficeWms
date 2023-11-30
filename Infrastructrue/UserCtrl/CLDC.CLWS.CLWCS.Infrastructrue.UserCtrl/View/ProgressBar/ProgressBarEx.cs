using System.Windows;

namespace CLDC.Infrastructrue.UserCtrl.View.ProgressBar
{
    public static class ProgressBarEx
    {
        private static FrmProgressBar _waitView;
        public static void ReportProcess(double step, string msg, string title = "")
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (_waitView == null)
                    {
                        _waitView = new FrmProgressBar("正在处理，请稍后.....");
                    }
                    if (!string.IsNullOrEmpty(title))
                    {
                        _waitView.SetTilte(title);
                    }
                    _waitView.SetCurrentRunValue(step, msg);
                    if (step>=100)
                    {
                        _waitView = null;
                    }
                });
        }
    }
}
