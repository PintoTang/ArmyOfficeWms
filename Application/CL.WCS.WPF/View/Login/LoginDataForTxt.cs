using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.WPF.UserCtrl.Login
{
    public static class LoginDataForTxt
    {
        public static string[] GetLgoinInfo(string userTxtFullPath)
        {
            StreamReader sr = new StreamReader(userTxtFullPath, Encoding.Default); //设置读取文件变量
            string str1 = sr.ReadLine();    //读取文件
            sr.Close();
            if (str1 == "" || str1 == null) return null;
            return str1.Split(',');
        }

        public static void WriteLoginErrInfo(string filePath,int errCount,int sleepTimes,string curDateTimes)
        {

            string FileContent = errCount.ToString() + "," + sleepTimes.ToString() + "," + curDateTimes;
            StreamWriter fs = new StreamWriter(filePath);
            //获得字节
            byte[] datas = System.Text.Encoding.Default.GetBytes(FileContent);
            //开始
            fs.Write(FileContent);
            //清空缓冲区和关闭流;
            fs.Flush();
            fs.Close();
        }
    }
}
