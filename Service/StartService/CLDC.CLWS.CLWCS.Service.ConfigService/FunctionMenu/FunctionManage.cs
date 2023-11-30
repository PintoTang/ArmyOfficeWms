using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class FunctionManage
    {
      private static  List<FunctionMenuInfo> functionMenuInfoLst = new List<FunctionMenuInfo>();

        private static FunctionManage functionManage;
        /// <summary>
        /// 菜单单例
        /// </summary>
        public static FunctionManage InStance
        {
            get
            {
                if(functionManage==null)
                {
                    functionManage = new FunctionManage();
                }
                return functionManage;
            }
        }
        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="funcInfo"></param>
        public void Add(FunctionMenuInfo funcInfo)
        {
            functionMenuInfoLst.Add(funcInfo);
        }

        public  List<FunctionMenuInfo> GetAllData()
        {
            return functionMenuInfoLst;
        }

        public OperateResult InitConfig(string path)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                Context cc = new Context(new FunctionMenuParent());
                cc.LoadXml(path);

                string strNodeParms = @"Configuration/Functions";
                cc.SetXmlNode(strNodeParms);
                cc.Request();
                if (cc.CurNextNodeInfo == null) return OperateResult.CreateFailedResult("加载失败 FunctionMenu.xml", 1);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
                result.ErrorCode = 1;
            }
            return result;
        }

    }
}
