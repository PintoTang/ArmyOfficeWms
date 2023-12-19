using CLDC.Framework.Log;
using CLDC.Infrastructrue.Xml;
using System;
using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    /// <summary>
    /// 任务分类配置
    /// </summary>
    public class SoundLightConfig
    {
        private static SoundLightConfig _soundLightConfig;

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }
        public List<Command> CommandList { get; set; }

        /// <summary>
        /// 单实例系统配置管理类
        /// </summary>
        /// <returns></returns>
        public static SoundLightConfig Instance
        {
            get
            {
                if (_soundLightConfig == null)
                    _soundLightConfig = new SoundLightConfig();
                return _soundLightConfig;
            }
        }

        public SoundLightConfigModel SoundLightConfigModel { get; set; }

        private SoundLightConfig()
        {
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\SoundLightConfig.xml";
                SoundLightConfigModel = (SoundLightConfigModel)XmlSerializerHelper.LoadFromXml(strFileName, typeof(SoundLightConfigModel));
                CommandList = SoundLightConfigModel.CommandList;
            }
            catch (Exception ex)
            {
                Log.getDebugFile().Info(ex.Message);
                throw ex;
            }
        }


    }
}
