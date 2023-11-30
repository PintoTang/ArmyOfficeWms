using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.WPF.UserCtrl.Login
{
    public static class UserDataForTxt
    {
        public static string[] GetRmdUserInfo(string userTxtFullPath)
        {
            StreamReader sr = new StreamReader(userTxtFullPath, Encoding.Default); //设置读取文件变量
            string str1 = sr.ReadLine();    //读取文件
            sr.Close();
            if (str1 == "" || str1 == null) return null;
            return str1.Split(',');
        }

        public static void WriteUserInfo(string FilePath, string userName, string userPwd,bool isRmd)
        {
            string rmd = "0";
            if (isRmd)
            {
                rmd = "1";
            }
            else
            {
                rmd = "0";
                userName = string.Empty;
                userPwd = string.Empty;
            }
            string FileContent = userName + "," + userPwd + "," + rmd;
            StreamWriter fs = new StreamWriter(FilePath);
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
