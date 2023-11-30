using System;
using System.Collections.Generic;
using System.Threading;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.UserCtrl.View.ProgressBar;

namespace CLDC.CLWS.CLWCS.Service.ThreadHandle
{
    public class ThreadHandleManage
    {
    


        public static int ThreadPoolCount
        {
            get { return _threadPool.Count; }
        }

        private static List<ThreadHandleProcess> _threadPool = new List<ThreadHandleProcess>();

        public static List<ThreadHandleProcess> ThreadPool
        {
            get { return _threadPool; }
            set
            {
                lock (_threadPool)
                {
                    _threadPool = value;
                }
            }
        }

        public static ThreadHandleProcess CreateNewThreadHandle(string threadName, Action threadStart)
        {
            ThreadHandleProcess newThreadHandle = new ThreadHandleProcess(threadName, new ThreadStart(threadStart));
            AddThreadPool(newThreadHandle);
            return newThreadHandle;
        }

        private static void AddThreadPool(ThreadHandleProcess threadHandle)
        {
            lock (_threadPool)
            {
                _threadPool.Add(threadHandle);
            }
        }

        public static OperateResult RemoveThreadHandle(ThreadHandleProcess threadHandle)
        {
            OperateResult result = new OperateResult();
            lock (_threadPool)
            {
                try
                {
                    result.IsSuccess = _threadPool.Remove(threadHandle);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
                return result;
            }
        }

        public static OperateResult StopAllThreadHandle()
        {
            double step = 0;
            double interval = Math.Ceiling(100.0 / _threadPool.Count);
            foreach (ThreadHandleProcess threadHandle in _threadPool)
            {
                ProgressBarEx.ReportProcess(step, string.Format("正在关闭线程：{0} 状态为：{1}", threadHandle.Name, threadHandle.ThreadState),"正在关闭线程服务，请稍后......");

                step = step + interval;
                if (threadHandle.ThreadState == ThreadState.Running || threadHandle.ThreadState == ThreadState.WaitSleepJoin)
                {
                    threadHandle.Stop();
                }
                ProgressBarEx.ReportProcess(step, string.Format("完成关闭线程：{0} 状态为：{1}", threadHandle.Name, threadHandle.ThreadState));
            }
            return OperateResult.CreateSuccessResult();
        }

    }
}
