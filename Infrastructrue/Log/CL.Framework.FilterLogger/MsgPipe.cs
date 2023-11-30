using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CL.Framework.FilterLogger
{
    /// <summary>
    /// Log message and remove duplicate message
    /// </summary>
    public class MsgPipe
    {
		private Action<object, EnumLogLevel, LoggerType, string> _parentWriteLogAction;

        /// <summary>
        /// 初始化MsgPipe类并设置日志的重复刷新时间，如果App.config中配置了logReserveTime其的话，miniDuarationTime值会被忽略掉
        /// </summary>
        /// <param name="miniDuarationTime"></param>
        /// <param name="parentWriteLogAction">打印委托</param>
		public MsgPipe(int miniDuarationTime, Action<object, EnumLogLevel, LoggerType, string> parentWriteLogAction)
        {
            try
            {
                var xmlFilePath = @"Config\App.config";
                if (File.Exists(xmlFilePath))
                {
                    var allAppText = File.ReadAllText(xmlFilePath, Encoding.UTF8);
                    var match = Regex.Match(allAppText, @"key=""\s*logReserveTime\s*""\s+value=""\s*(-?\d+)\s*""",
                        RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var value = match.Groups[1].Value;
                        Console.WriteLine("app.config中的时间间隔为："+value);
                        MiniDuarationTime = int.Parse(value);
                    }
                }
                else
                {
                    Console.WriteLine("Config\\App.config 文件不存在");
                }
            }
            catch (Exception ex)
            {
                //ignore
            }
            if (!MiniDuarationTime.HasValue)
            {
                MiniDuarationTime = miniDuarationTime;
            }
            _parentWriteLogAction = parentWriteLogAction;
        }


        static MsgPipe(){}

		private void DiyLog(object obj, EnumLogLevel level, LoggerType loggerType, string deviceName = null)
        {
            if (_parentWriteLogAction != null)
            {
                _parentWriteLogAction(obj, level, loggerType, deviceName);
            }
        }

        /// <summary>
        /// 相同日志最小的持续时间，如这个值是15秒，18:00:00打印了一个日志，一直打印到18：00：15，当到18：00：16的时候，就重新打印该日志
        /// 如果为-1的话，就一直约束重复日志打印
        /// </summary>
        public  int? MiniDuarationTime;

        private readonly Dictionary<string, List<MsgModal>> _msgDic = new Dictionary<string, List<MsgModal>>();

		public void LogMsg(EnumLogLevel level, string deviceName, LoggerType loggerType, string msg)
        {
		    try
		    {
                if(loggerType == LoggerType.GetEventFile)
                {
                    if (!SystemConfig.Instance.CurSystemConfig.IsRecordEventLog.Value)
                    {
                        return;
                    }
                }
                var logKey = deviceName + loggerType + level;
                var now = DateTime.Now;
                var msgModal = new MsgModal(now, msg, now.Ticks, 0);
                if (_msgDic.ContainsKey(logKey))
                {
                    var list = _msgDic[logKey];
                    if (list.Count > 0)
                    {
                        var modal = list.Last();
                        if (modal==null)
                        {
                            return;
                        }
                        if (modal.ToString() == msg)
                        {
                            msgModal = modal;
                            msgModal.RepeatTime++;
                        }
                        else
                        {
                            if (list.Count > 100)
                            {
                                list.Clear();
                            }
                            if (modal.RepeatTime > 0)
                            {
                                DiyLog(modal + "\n此消息已重复打印" + modal.RepeatTime + "次", level, loggerType, deviceName);
                            }
                            list.Add(msgModal);
                        }
                    }
                    else
                    {
                        list.Add(msgModal);
                    }
                }
                else
                {
                    _msgDic.Add(logKey, new List<MsgModal>() { msgModal });
                }
                if (msgModal.RepeatTime > 0 && MiniDuarationTime > 0)
                {
                    var d = (now.Ticks - msgModal.Id) / Math.Pow(10, 7);
                    if (MiniDuarationTime != null && d > MiniDuarationTime.Value)
                    {
                        if (!msgModal.Msg.EndsWith("次"))
                        {
                            msgModal.Msg += "\r\n此消息重复打印" + msgModal.RepeatTime + "次";
                        }
                        msgModal.HappenTime = now;
                        msgModal.Id = now.Ticks;
                        msgModal.RepeatTime = 0;
                    }
                }
                if (msgModal.RepeatTime == 0)
                {
                    DiyLog(msgModal, level, loggerType, deviceName);
                }
		    }
		    catch (Exception)
		    {
		        
		      
		    }
           
        }
    }
}
