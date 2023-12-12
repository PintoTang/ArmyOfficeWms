using CLDC.Framework.Log;
using CLDC.Infrastructrue.Xml;
using System;
using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    public delegate void ApplicationExitingHandlder();
    /// <summary>
    /// 任务分类配置
    /// </summary>
    public class TaskTypeConfig
    {
        private static TaskTypeConfig taskTypeConfig;

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }
        public List<TaskButton> TaskButtonList { get; set; }

        public event ApplicationExitingHandlder ApplicationExitingEvent;

        /// <summary>
        /// 单实例系统配置管理类
        /// </summary>
        /// <returns></returns>
        public static TaskTypeConfig Instance
        {
            get
            {
                if (taskTypeConfig == null)
                    taskTypeConfig = new TaskTypeConfig();
                return taskTypeConfig;
            }
        }

        public void AppExit()
        {
            if (ApplicationExitingEvent != null)
            {
                ApplicationExitingEvent();
            }
        }

        public TaskTypeConfigModel CurTaskTypeConfig { get; set; }

        private TaskTypeConfig()
        {
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\TaskTypeConfig.xml";
                CurTaskTypeConfig = (TaskTypeConfigModel)XmlSerializerHelper.LoadFromXml(strFileName, typeof(TaskTypeConfigModel));
                TaskButtonList = CurTaskTypeConfig.TaskButtonList;
            }
            catch (Exception ex)
            {
                Log.getDebugFile().Info(ex.Message);
                throw ex;
            }
        }


    }
}
