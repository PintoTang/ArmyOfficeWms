using CLDC.Framework.Log;
using CLDC.Infrastructrue.Xml;
using System;
using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    /// <summary>
    /// 出入库事由配置
    /// </summary>
    public class TaskTeamConfig
    {
        private static TaskTeamConfig taskTeamConfig;

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }
        public List<TaskTeam> TaskTeamList { get; set; }

        public event AppExitingHandlder ApplicationExitingEvent;

        /// <summary>
        /// 单实例系统配置管理类
        /// </summary>
        /// <returns></returns>
        public static TaskTeamConfig Instance
        {
            get
            {
                if (taskTeamConfig == null)
                    taskTeamConfig = new TaskTeamConfig();
                return taskTeamConfig;
            }
        }

        public void AppExit()
        {
            if (ApplicationExitingEvent != null)
            {
                ApplicationExitingEvent();
            }
        }

        public TaskTeamConfigModel CurTaskTeamConfig { get; set; }

        private TaskTeamConfig()
        {
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\TaskTeamConfig.xml";
                CurTaskTeamConfig = (TaskTeamConfigModel)XmlSerializerHelper.LoadFromXml(strFileName, typeof(TaskTeamConfigModel));
                TaskTeamList = CurTaskTeamConfig.TaskTeamList;
            }
            catch (Exception ex)
            {
                Log.getDebugFile().Info(ex.Message);
                throw ex;
            }
        }


    }
}
