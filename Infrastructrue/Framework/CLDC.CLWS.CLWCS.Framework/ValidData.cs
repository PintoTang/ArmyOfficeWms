using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Framework
{
    /// <summary>
    /// 验证数据
    /// </summary>
    public static class ValidData
    {
        /// <summary>
        /// 验证登录用户名
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static bool RexCheckUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;
            // 昵称格式：限16个字符，支持中英文、数字、减号或下划线
            //string regStr = "^[\\u4e00-\\u9fa5_a-zA-Z0-9-]{4,16}$";
            string regStr = @"^(?![0-9]+$)(?![a-zA-Z]+$)(?!([^(0-9a-zA-Z)]|[\(\)])+$)([^(0-9a-zA-Z)]|[\(\)]|[a-zA-Z]|[0-9]){6,16}$";
            bool checkNumLenght= Regex.IsMatch(userName, regStr);
            bool checkSpChar= CheckSpecialCharacters(userName);
            return checkNumLenght && !checkSpChar;
        }
        /**
       * 正则表达式验证密码  最少8位
       * @param input
       * @return
       */
        public static bool RexCheckPassword(string userPwd)
        {
            if (string.IsNullOrEmpty(userPwd)) return false;
            // 8-20 位，字母、数字、字符
            //String reg = "^([A-Z]|[a-z]|[0-9]|[`-=[];,./~!@#$%^*()_+}{:?]){8,20}$";
            String regStr = @"^(?![0-9]+$)(?![a-zA-Z]+$)(?!([^(0-9a-zA-Z)]|[\(\)])+$)([^(0-9a-zA-Z)]|[\(\)]|[a-zA-Z]|[0-9]){8,20}$";
            bool checkChar= Regex.IsMatch(userPwd, regStr);
            //bool checkSpChar = CheckSpecialCharacters(userPwd);//密码支持特殊字符
            return checkChar;
        }

        /// <summary>
        /// 验证非法输入
        /// </summary>
        /// <param name="strData">输入的数据</param>
        /// <returns>True 表示 非法输入，False表示正常输入</returns>
        public static bool CheckSpecialCharacters(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return false;
            Regex regExp = new Regex("[ \\[ \\] \\^ \\-_*×――(^)$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]");
            return regExp.IsMatch(strData);
        }
      
        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="strData">输入数据</param>
        /// <param name="regStr">正则表达式</param>
        /// <returns></returns>
        public static bool CheckData(string strData, string regStr)
        {
            if (string.IsNullOrEmpty(strData)) return false;
            return Regex.IsMatch(strData, regStr);
        }

        /// <summary>
        /// 校验数据20个长度 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns>true 过长，false没有超过20个长度</returns>
        public static bool CheckDataLen_20(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return false;

            if (strData.Length > 20)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 校验查询 输入的数据特殊字符 和数据长度 20字符（默认）
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static bool CheckSearchParmsLenAndSpecialCharts(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return false;
            bool isDataLen_20 = CheckDataLen_20(strData);
            bool isDataSpecial = CheckSpecialCharacters(strData);
            return isDataLen_20 || isDataSpecial;
        }

    }
}
