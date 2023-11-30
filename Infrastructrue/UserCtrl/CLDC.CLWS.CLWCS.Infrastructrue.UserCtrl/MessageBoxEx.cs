using System.Windows;

namespace CLDC.Infrastructrue.UserCtrl
{
   public class MessageBoxEx
   {
        static string strErrFrmMsg = "无效的窗口句柄";
        public static MessageBoxResult Show(string messageBoxText)
        {
            ShowMessage sm = new ShowMessage(messageBoxText);
            sm.Topmost = true;
            if (!messageBoxText.Contains(strErrFrmMsg))
            {
                sm.ShowDialog();
            }
            return sm.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            ShowMessage sm = new ShowMessage(messageBoxText, caption);
            sm.Topmost = true;
            if (!messageBoxText.Contains(strErrFrmMsg))
            {
                sm.ShowDialog();
            }
            return sm.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            ShowMessage sm = new ShowMessage(messageBoxText, caption, button);
            sm.Topmost = true;
            if (!messageBoxText.Contains(strErrFrmMsg))
            {
                sm.ShowDialog();
            }
            return sm.Result;
        }
    }
}
