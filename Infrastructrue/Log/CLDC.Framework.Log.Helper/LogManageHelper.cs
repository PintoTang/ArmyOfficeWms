using System;
using System.Collections.Generic;

namespace CLDC.Framework.Log.Helper
{
    /// <summary>
    /// 日志打开
    /// </summary>
    public class LogManageHelper
    {

        private static LogManageHelper logManageHelper;
        public static LogManageHelper Instance
        {
            get
            {
                if (logManageHelper == null)
                {
                    logManageHelper = new LogManageHelper();
                }
                return logManageHelper;
            }
        }


        public void OpenLogFile(string LogFileName)
        {
            string path = GetApplictionPath();
            if (string.IsNullOrEmpty(path)) return;
            if (!path.Contains("Log\\"))
                path += "Log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (System.IO.Directory.Exists(path))
            {
                string[] files = System.IO.Directory.GetFiles(path);
                List<String> filesLst = new List<String>(files);

                string fileName = filesLst.Find(x => x.Contains("Msg_" + LogFileName + ".log"));
                if (!string.IsNullOrEmpty(fileName))
                    System.Diagnostics.Process.Start(fileName);
                else
                {
                    //MessageBox.Show("未找到log文件！  " + fileName);
                }
            }
        }
        /// <summary>
        /// 取得当前程序集运行时路径(最后带有\号)
        /// 取得程序集路径(对于web程序取得虚拟目录绝对路径)
        /// </summary>
        /// <returns>运行时路径(最后带有\号)</returns>
        private  string GetApplictionPath()
        {
            Type t = this.GetType();

            //取得完整路径
            string path = t.Assembly.CodeBase;

            //路径例子: file:///E:/DEV/ShopFloor/SourceCode/ShopFloorWeb/bin/Common.DLL
            //针对以上路径进行处理，替换 file:///  为空
            path = path.Replace("file:///", "");

            //去除/bin/以后的内容,假如是web程序，这样处理就得到web根路径了,
            //假如是调试期间，也能取到开发根目录，实际部署不需要修改。
            int x = path.LastIndexOf("/bin/");

            //x == -1 表示不是常见web程序
            if (x == -1)
            {
                x = path.LastIndexOf("/");
            }

            //去除不需要的部分
            path = path.Substring(0, x + 1);

            //反转 / 符号为 \
            path = path.Replace("/", "\\");

            return path;
        }
    }
}
