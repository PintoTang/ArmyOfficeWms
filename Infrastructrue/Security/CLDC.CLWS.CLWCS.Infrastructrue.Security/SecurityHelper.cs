using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.Infrastructrue.Security
{
    public class SecurityHelper
    {
        ///// <summary> 
        ///// 加密数据 
        ///// </summary> 
        ///// <param name="encryptContent">加密字符串</param> 
        ///// <param name="skey">生成秘钥，默认为MATICSOFT</param> 
        ///// <returns>返回加密字符串</returns> 
        public static string Encrypt(string encryptContent, string sKey = "Clou")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.Default.GetBytes(encryptContent);
            des.Key = Encoding.ASCII.GetBytes(Md5Key(sKey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(Md5Key(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length); //写
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);//十六进制字符串,后面的2为精度.
            }
            return ret.ToString();
        }

        /// <summary>
        /// 解密数据 
        /// </summary>
        /// <param name="decryptContent"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string decryptContent, string sKey = "Clou")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = decryptContent.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(decryptContent.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = Encoding.ASCII.GetBytes(Md5Key(sKey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(Md5Key(sKey).Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// 对秘钥进行处理
        /// </summary>
        /// <param name="str">你的秘钥</param>
        /// <returns></returns>
        public static string Md5Key(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0'); //转成16进制，如果小于2位用0补充
            return ret;
        }


    }
}
