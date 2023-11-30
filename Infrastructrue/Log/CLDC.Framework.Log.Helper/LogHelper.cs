using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CL.Framework.FilterLogger;
using CL.WCS.SystemConfigPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using log4net;

namespace CLDC.Framework.Log.Helper
{
	

	/// <summary>
	/// 配置管理类
	/// </summary>
	public sealed class LogHelper
	{
        private static readonly MsgPipe MyLogger = new MsgPipe(-1, DiyLog);
		private static void DiyLog(object obj, EnumLogLevel level, LoggerType loggerType, string deviceName)
	    {
            ILog logger;
            switch (loggerType)
            {
                case LoggerType.GetDebugFile:
                    logger =Log.getDebugFile();
                    break;
                case LoggerType.GetErrorFile:
                    logger = Log.getErrorFile();
                    break;
                case LoggerType.GetEventFile:
                    if (!SystemConfig.Instance.CurSystemConfig.IsRecordEventLog.Value)
                    {
                        return;
                    }
                    logger = Log.getEventFile();
                    break;
                case LoggerType.GetExceptionFile:
                    logger = Log.getExceptionFile();
                    break;
                case LoggerType.GetMessageFile:
                    logger = Log.getMessageFile(deviceName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("loggerType", loggerType, null);
            }
            switch (level)
            {
				case EnumLogLevel.Error:
                    logger.Error(obj);
                    break;
				case EnumLogLevel.Warning:
                    logger.Warn(obj);
                    break;
				case EnumLogLevel.Info:
                    logger.Info(obj);
                    break;
				case EnumLogLevel.Debug:
                    logger.Debug(obj);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level", level, null);
            }
	    }

		static LogHelper()
		{

		}

		

        private static readonly Dictionary<string,object[]>  UiMessagDictionary=new Dictionary<string, object[]>();


	    /// <summary>
        /// 根据节点编号，将日志写入到设备的文件夹,如果有消息等级，会改变相应的窗口的颜色
	    /// </summary>
	    /// <param name="stationName">工站名称</param>
	    /// <param name="logMsg">消息内容</param>
	    /// <param name="logLevel">消息等级</param>
        public static void WriteLog(string stationName, string logMsg, EnumLogLevel? logLevel=EnumLogLevel.Info)
	    {
            if (logLevel != null && SystemConfig.Instance.CurSystemConfig.LogRecordLevel.Value>=logLevel)
	        {
                string msg = "[" + logLevel.ToString() + "]" + logMsg;
                switch (logLevel)
                {
                    case EnumLogLevel.Error:
                        MyLogger.LogMsg(EnumLogLevel.Error, stationName, LoggerType.GetMessageFile, msg);
                        break;
                    case EnumLogLevel.Warning:
                        MyLogger.LogMsg(EnumLogLevel.Warning, stationName, LoggerType.GetMessageFile, msg);
                        break;
                    case EnumLogLevel.Info:
                        MyLogger.LogMsg(EnumLogLevel.Info, stationName, LoggerType.GetMessageFile, msg);
                        break;
                    case EnumLogLevel.Debug:
                        MyLogger.LogMsg(EnumLogLevel.Debug, stationName, LoggerType.GetMessageFile, msg);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("logLevel", logLevel, null);
                }
	        }
	    }

		/// <summary>
		/// 打印异常信息
		/// </summary>
		/// <param name="stationEName"></param>
		/// <param name="ex"></param>
		public static void WriteLog(string stationEName, Exception ex)
		{
			WriteLog(stationEName, ex.Message,EnumLogLevel.Error);
			MyLogger.LogMsg(EnumLogLevel.Info, stationEName, LoggerType.GetExceptionFile, ex.Message);
		}
	}
    
}
