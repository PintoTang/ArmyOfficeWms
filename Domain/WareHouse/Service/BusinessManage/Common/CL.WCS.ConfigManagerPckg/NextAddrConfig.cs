using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.Xml;

namespace CL.WCS.ConfigManagerPckg
{
    /// <summary>
    /// 下步地址配置
    /// </summary>
    public class NextAddrConfig
    {

        public static OperateResult<Dictionary<Addr, List<string>>> GetNextAddrDicByWorkerName(string workerName)
        {
            Dictionary<Addr, List<string>> nextAddrDic = new Dictionary<Addr, List<string>>();
            OperateResult<Dictionary<Addr, List<string>>> result = OperateResult.CreateFailedResult(nextAddrDic, "无数据");
            try
            {
                string fileName = "Config/NextAddrConfig.xml";
                string path = "Tab/DeviceType/Compute";
                XmlOperator doc = new XmlOperator(fileName);
                XmlNodeList nodeList = doc.GetXmlNode(path);
                if (nodeList == null)
                {
                    result.IsSuccess = true;
                    result.Message = "不存在下步地址配置信息";
                    return result;
                }
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["DeviceName"].Value == workerName)
                    {
                        string[] destAddrAry = node.Attributes["DestAddr"].Value.Trim().Split('|');
                        foreach (string strDestAddr in destAddrAry)
                        {
                            List<string> nextAddrLst = new List<string>();
                            string[] nextAddrs = node.Attributes["NextAddr"].Value.Trim().Split('|');
                            foreach (string nextAddr in nextAddrs)
                            {
                                nextAddrLst.Add(nextAddr);
                            }
                            nextAddrDic.Add(new Addr(strDestAddr), nextAddrLst);
                        }
                    }
                }
                result.Content = nextAddrDic;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.StackTrace + ex.Message;
            }
            return result;
        }

    }
}
