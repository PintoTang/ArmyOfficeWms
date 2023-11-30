using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.Service.ThreadHandle
{

	/// <summary>
	/// 单线程处理业务的接口
	/// </summary>
    public  class ThreadHandleProcess
	{
        /// <summary>
        /// 释放持续进行
        /// </summary>
        public bool IsContinuous { get; set; }

	    public Action<string> ThreadStopAction;

	    private void StopThread(string msg)
	    {
            if (ThreadStopAction!=null)
            {
                ThreadStopAction(msg);
            }
	    }

        public string Name
        {
            get { return _handleThread.Name; }
            set { _handleThread.Name = value; }
        }

        public bool IsAlive
        {
            get { return _handleThread.IsAlive; }
        }
        public ThreadState ThreadState
        {
            get { return _handleThread.ThreadState; }
        }

        public string Tid { get; set; }


	    public ThreadHandleProcess(string threadName,ThreadStart threadStart)
	    {
	        this._threadHandle = threadStart;
            InitThreadHandle();
            this.Name = threadName;
	        this.IsContinuous = true;
	    }

	    public OperateResult Stop()
	    {
            OperateResult result=new OperateResult();
	        try
	        {
	            IsContinuous = false;
	            StopThread("系统退出");
	            _handleThread.Join();
	            result.IsSuccess = true;
	        }
	        catch (Exception ex)
	        {
	            result.IsSuccess = false;
	            result.Message = OperateResult.ConvertException(ex);    
	        }
	        return result;
	    }

	    /// <summary>
		/// 启动线程
		/// </summary>
		/// <returns></returns>

		public  OperateResult Start()
		{
			return StartThreadHandle();
		}

		private OperateResult StartThreadHandle()
		{
			if (_handleThread == null)
			{
				return OperateResult.CreateFailedResult();
			}
			if (_handleThread.ThreadState == ThreadState.Unstarted)
			{
				_handleThread.Start();
				return OperateResult.CreateSuccessResult();
			}
			return OperateResult.CreateFailedResult();
		}


		private Thread _handleThread;

		/// <summary>
		/// 初始化线程处理
		/// </summary>
		public  void InitThreadHandle()
		{
			if (_handleThread == null)
			{
				_handleThread = new Thread(_threadHandle) { IsBackground = false };
				return;
			}
			if (_handleThread.ThreadState == ThreadState.Running || _handleThread.ThreadState == ThreadState.WaitSleepJoin)
			{
				_handleThread.Abort();
				while (_handleThread.ThreadState != ThreadState.Stopped)
				{
					Thread.Sleep(2000);
				}
			}
			_handleThread = new Thread(_threadHandle) { IsBackground = false };
		}

	    /// <summary>
	    /// 线程处理
	    /// </summary>
        private readonly ThreadStart _threadHandle;

	}
}
