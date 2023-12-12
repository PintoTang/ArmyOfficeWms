using CLDC.Framework.Log;
using CLDC.Infrastructrue.Xml;
using System;
using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    public delegate void AppExitingHandlder();
    /// <summary>
    /// 出入库事由配置
    /// </summary>
    public class ReasonConfig
    {
        private static ReasonConfig reasonConfig;

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }
        public List<Reason> ReasonList { get; set; }

        public event AppExitingHandlder ApplicationExitingEvent;

        /// <summary>
        /// 单实例系统配置管理类
        /// </summary>
        /// <returns></returns>
        public static ReasonConfig Instance
        {
            get
            {
                if (reasonConfig == null)
                    reasonConfig = new ReasonConfig();
                return reasonConfig;
            }
        }

        public void AppExit()
        {
            if (ApplicationExitingEvent != null)
            {
                ApplicationExitingEvent();
            }
        }

        public ReasonConfigModel CurReasonConfig { get; set; }

        private ReasonConfig()
        {
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\ReasonConfig.xml";
                CurReasonConfig = (ReasonConfigModel)XmlSerializerHelper.LoadFromXml(strFileName, typeof(ReasonConfigModel));
                ReasonList = CurReasonConfig.ReasonList;
            }
            catch (Exception ex)
            {
                Log.getDebugFile().Info(ex.Message);
                throw ex;
            }
        }


    }
}
