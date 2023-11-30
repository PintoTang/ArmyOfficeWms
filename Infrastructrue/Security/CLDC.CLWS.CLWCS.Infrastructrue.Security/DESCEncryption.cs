using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.Infrastructrue.Security
{
    /// <summary>
    /// DESC 加解密
    /// </summary>
    public class DESCEncryption
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">加密文本</param>
        /// <param name="skey">密钥</param>
        /// <returns></returns>
        public static string Encrypt(string text, string skey)
        {
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(text);

            des.Key = Encoding.ASCII.GetBytes(MD5Encryption.Encrypt(skey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(MD5Encryption.Encrypt(skey).Substring(0, 8));

            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            var ret = new StringBuilder();
            foreach (var b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            des.Dispose();
            ms.Dispose();
            cs.Dispose();
            return ret.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="hash">加密后字符串</param>
        /// <param name="skey">密钥</param>
        /// <returns></returns>
        public static string Decrypt(string hash, string skey)
        {
            var des = new DESCryptoServiceProvider();
            int len;
            len = hash.Length / 2;
            var inputByteArray = new byte[len];
            int x, i;

            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(hash.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }

            des.Key = Encoding.ASCII.GetBytes(MD5Encryption.Encrypt(skey).Substring(0, 8));
            des.IV = Encoding.ASCII.GetBytes(MD5Encryption.Encrypt(skey).Substring(0, 8));

            var ms = new System.IO.MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            des.Dispose();
            ms.Dispose();
            cs.Dispose();
            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}
