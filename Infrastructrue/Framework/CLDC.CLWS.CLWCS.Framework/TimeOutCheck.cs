using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Framework
{
    /// <summary>
    /// 超时检查
    /// </summary>
    public class TimeOutCheck<T>
    {
        public TimeOutCheck(int timeOut)
        {
            StartTime = DateTime.Now;
            _IsSuccessful = false;
            DelayTime = timeOut;
        }

        public bool SetIsSucessful(bool isSuccess)
        {
            if (isSuccess)
            {
                TimeOutCallBackAction = null;
            }
            _IsSuccessful = isSuccess;
            return true;
        }

        /// <summary>
        /// 操作的开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 操作是否成功
        /// </summary>
        private bool _IsSuccessful;
        /// <summary>
        /// 延时的时间，单位毫秒
        /// </summary>
        public int DelayTime { get; set; }
        /// <summary>
        /// 用于超时执行的方法
        /// </summary>
        private Action<T> TimeOutCallBackAction { get; set; }

        /// <summary>
        /// 回调的参数
        /// </summary>
        public T CallBackParameter { get; set; }


        /// <summary>
        /// 线程池检查时间超时
        /// </summary>
        /// <param name="obj"></param>
        public void ThreadPoolCheckTimeOut(object obj)
        {
            if (obj is TimeOutCheck<T>)
            {
                TimeOutCheck<T> timeOut = obj as TimeOutCheck<T>;
                while (!timeOut._IsSuccessful)
                {
                    Thread.Sleep(100);
                    if ((DateTime.Now - timeOut.StartTime).TotalMilliseconds > timeOut.DelayTime)
                    {
                        // 连接超时或是验证超时
                        if (!timeOut._IsSuccessful)
                        {
                            if (timeOut.TimeOutCallBackAction != null)
                            {
                                timeOut.TimeOutCallBackAction.Invoke(CallBackParameter);
                            }
                        }
                        break;
                    }
                }

            }
        }
        public void Check(Action<T> timeOutCallback)
        {
            this.TimeOutCallBackAction = timeOutCallback;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolCheckTimeOut), this);
        }
    }
}
