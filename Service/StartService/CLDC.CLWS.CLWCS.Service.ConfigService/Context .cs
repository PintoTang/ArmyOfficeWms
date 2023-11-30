
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.Framework.Log.IO;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    /// <summary>
    /// Context 对象
    /// </summary>
    public class Context
    {
        private StateAbstract mState;//引用，用于执行各子状态的行为
        /// <summary>
        /// xml对象
        /// </summary>
        public  Xml xml = null;
        /// <summary>
        /// xml节点路径
        /// </summary>
        public string strNodePath = "";
        public Context(StateAbstract state)
        {
            this.mState = state;
        }
    
        /// <summary>
        /// 加载XML文件
        /// </summary>
        /// <param name="strFullPath">xml全路径</param>
        public void LoadXml(string strFullPath)
        {
            xml = new Xml(strFullPath, false);
        }
 
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="state">State对象</param>
        public void SetState(StateAbstract state)
        {
            this.mState = state;
        }
        /// <summary>
        /// 设置Xml Node 节点信息 
        /// </summary>
        /// <param name="nodePath">node路径</param>
        public void SetXmlNode(string nodePath)
        {
            strNodePath = nodePath;
        }

       /// <summary>
       /// 触发请求调用
       /// </summary>
        public void Request()
        {
            //通过引用，来执行当前状态行为
            mState.handle(this);
        }

        /// <summary>
        /// 从开始处查找并根据节点名称移除节点路径
        /// </summary>
        /// <param name="strRemoveNodeName">移除的节点路径名</param>
        public void ReMoveStartNodePath(string strRemoveNodeName)
        {
            int len = strNodePath.Length;
            int indexStart = strNodePath.IndexOf(strRemoveNodeName);

            string newNodeName = strNodePath.Substring(0,  indexStart);
            SetXmlNode(newNodeName);
        }
        /// <summary>
        /// 当前对象的下一个对象信息
        /// </summary>
        public object CurNextNodeInfo { get; set; }

        /// <summary>
        /// 包名称
        /// </summary>
        public string GlobalNameSpace { get; set; }
        /// <summary>
        /// 当前包命名空间
        /// </summary>
        public string CurNameSpace { get; set; }
    }
}
