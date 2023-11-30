using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.ThreadHandle;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Framework.Log.Helper;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model
{
    /// <summary>
    /// 针对获取Web服务插入的数据监控类
    /// </summary>
    public sealed class DbServiceMonitor : IStateControl, IRestore
    {
        private readonly string _wareHouseCode = SystemConfig.Instance.WhCode;
        public DbServiceMonitor(string selectSql, string updateSql, int monitorInterval,IDbServiceHandle serviceHandle)
        {
            IsAsync = false;
            this.dbHelper = DependencyHelper.GetService<IDbHelper>("WcsLocal");
            this.SelectSql = string.Format(selectSql, SystemConfig.Instance.WhCode);
            this.UpdateSql = updateSql;
            this.MonitorInterval = monitorInterval;
            this.DbServiceHandle = serviceHandle;
        }

        private ThreadHandleProcess _dataThreadHandler;
        public OperateResult Start()
        {
          CurRunState = RunStateMode.Run;
          _dataThreadHandler = new ThreadHandleProcess(DataMonitorThreadName, ThreadHandle);
            return _dataThreadHandler.Start();

        }

        private const string DataMonitorThreadName = "DataMonitor_ThreadHandle";

        /// <summary>
        /// 查询的SQL语句
        /// </summary>
        public string SelectSql { get; set; }

        /// <summary>
        /// 更新的SQL语句
        /// </summary>
        public string UpdateSql { get; set; }

        public int monitorInterval = 1000;

        private readonly IDbHelper dbHelper;
        public IDbServiceHandle DbServiceHandle;


        /// <summary>
        /// 日志显示的名字
        /// </summary>
        public string Name { get; set; }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 业务处理超时时间
        /// </summary>
        public int Seconds
        {
            get { return seconds; }
            set { seconds = value; }
        }

        /// <summary>
        /// 是否需要异步处理业务
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 获取数据的时间间隔
        /// </summary>
        public int MonitorInterval
        {
            get { return monitorInterval; }
            set { monitorInterval = value; }
        }

        private string key = "rowid";


        /// <summary>
        /// 业务处理
        /// </summary>
        /// <param name="rowId">唯一键</param>
        private void ProcessMointorData(string rowId)
        {
            DataTable dt = dbHelper.QueryDataTable(SelectSql.Insert(SelectSql.LastIndexOf("where", StringComparison.Ordinal) + 5, " "+key + "='" + rowId + "' and "));
            if (dt.Rows.Count < 1)
            {
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                string statue = row[3].ToString();
                if (statue.Equals("2"))
                {
                    continue;
                }
                DbServiceHandle.BusinessProcessData(row, IsAsync);
                BusinessFinishedCallback(row[key].ToString());
            }
        }

        /// <summary>
        /// 更改数据库的数据操作
        /// </summary>
        /// <param name="id"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void BusinessFinishedCallback(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(UpdateSql))
                {
                    return;
                }
                Hashtable parameters = new Hashtable();
                parameters.Add("key", id);
                dbHelper.ExecuteNonQuery(UpdateSql, parameters);
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("接口业务处理完成时，刷新WCS监控表状态异常：{0},rd_guid:{1}", OperateResult.ConvertException(ex), id), EnumLogLevel.Error, true);
            }
        }

        private void DataQueueHandle()
        {
            while (_dataQueueHandle.IsContinuous)
            {
                try
                {
                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        Thread.Sleep(5000);
                        continue;
                    }
                    if (changedQueue.Count == 0)
                    {
                        Thread.Sleep(200);
                        continue;
                    }
                    string id = changedQueue.Peek();
                    ProcessMointorData(id);
                    changedQueue.Dequeue();
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("业务处理异常，异常信息：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, true);
                }
                finally
                {
                    seconds = 0;
                    waitReceive.Set();
                }
            }
        }
        private void DataQueueMonitor()
        {
            while (_dataQueueMonitor.IsContinuous)
            {
                try
                {
                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        Thread.Sleep(5000);
                        continue;
                    }
                    Thread.Sleep(MonitorInterval);
                    DataTable dt = dbHelper.QueryDataTable(SelectSql);
                    if (dt.Rows.Count < 1)
                    {
                        continue;
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string id = dt.Rows[i][key].ToString();
                        if (string.IsNullOrEmpty(id))
                        {
                            continue;
                        }
                        AddQueueReceive(id);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("查询T_BO_RECEIVEDATA表未处理状态的值 异常信息: {0}  执行的SQL: {1} ", OperateResult.ConvertException(ex), SelectSql), EnumLogLevel.Error, true);
                }
            }
        }
        /// <summary>
        /// 先暂停接受数据线程，等待业务处理完成，继续接受数据
        /// </summary>
        private ManualResetEvent waitReceive = new ManualResetEvent(false);
        private QueueDataPool<string> changedQueue = new QueueDataPool<string>();
        private int seconds = 0;


        private void AddQueueReceive(string id)
        {
            if (changedQueue.Count > 20)
            {
                waitReceive.WaitOne();
            }
            changedQueue.Enqueue(id);
            seconds = -1;
            waitReceive.Reset();
        }

        


        private const string DataQueueMonitorThreadName = "_数据监控";

        private const string DataQueueHandleThreadName = "_数据处理";

        private ThreadHandleProcess _dataQueueMonitor;
        private ThreadHandleProcess _dataQueueHandle;
        private void ThreadHandle()
        {
            _dataQueueMonitor = ThreadHandleManage.CreateNewThreadHandle(Name + DataQueueMonitorThreadName, DataQueueMonitor);
            _dataQueueMonitor.Start();

            _dataQueueHandle = ThreadHandleManage.CreateNewThreadHandle(Name + DataQueueHandleThreadName, DataQueueHandle);
            _dataQueueHandle.Start();
        }


        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            LogHelper.WriteLog(Name, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }


        private RunStateMode curState = RunStateMode.Pause;
        private ControlStateMode curControlState = ControlStateMode.Auto;
        /// <summary>
        /// 当前运行状态
        /// </summary>
        public RunStateMode CurRunState
        {
            get
            {
                return curState;
            }
            set { curState = value; }
        }

        public ControlStateMode CurControlMode
        {
            get { return curControlState; }
            set { curControlState = value; }
        }

        public OperateResult Run()
        {
            curState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        public OperateResult Pause()
        {
            curState = RunStateMode.Pause;
            return OperateResult.CreateSuccessResult();
        }


        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        public OperateResult Reset()
        {
            curState = RunStateMode.Reset;
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Restore()
        {
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Stop()
        {
            curState = RunStateMode.Stop;
            return OperateResult.CreateSuccessResult();
        }
    
        /// <summary>
        /// 根据传入的条件查询
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataByParms( string S_HandleStatu, string S_MethodName, string S_InputDataParms,
                                         string S_ReceviveStartTime, string S_ReceviveEndTime, string S_HandleStartTime, string S_HandleEndTime)
        {
            string sqlstr = string.Format("select * from T_BO_RECEIVEDATA where  WH_CODE ='{0}' ", _wareHouseCode);
            if (!string.IsNullOrEmpty(S_HandleStatu) && !S_HandleStatu.Contains("全部"))
            {
                sqlstr += " and DHSTATUS_ID='" + int.Parse(S_HandleStatu) + "'";
            }

            if (!string.IsNullOrEmpty(S_MethodName))
            {
                sqlstr += " and RD_METHODNAME like'" + "%" + S_MethodName + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_InputDataParms))
            {
                sqlstr += " and RD_PARAMVALUE like'" + "%" + S_InputDataParms + "%" + "'";
            }

            if (!string.IsNullOrEmpty(S_ReceviveStartTime) && !string.IsNullOrEmpty(S_ReceviveEndTime))
            {
                DateTime startDate = Convert.ToDateTime(S_ReceviveStartTime);
                DateTime endDate = Convert.ToDateTime(S_ReceviveEndTime);
                //sqlstr += string.Format(" and RD_RECEIVEDATE between  to_date( '{0}', 'yyyy-mm-dd hh24:mi:ss') and  to_date('{1}','yyyy-mm-dd hh24:mi:ss ' )", startDate, endDate);//oracle
                sqlstr += string.Format(" and RD_RECEIVEDATE between   '{0}' and  '{1}'", startDate, endDate);
            }

            if (!string.IsNullOrEmpty(S_HandleStartTime) && !string.IsNullOrEmpty(S_HandleEndTime))
            {
                DateTime startDate = Convert.ToDateTime(S_HandleStartTime);
                DateTime endDate = Convert.ToDateTime(S_HandleEndTime);
                //sqlstr += string.Format(" and  RD_HANDLERDATE between  to_date( '{0}', 'yyyy-mm-dd hh24:mi:ss') and  to_date('{1}','yyyy-mm-dd hh24:mi:ss ' )", startDate, endDate);//oracle
                sqlstr += string.Format(" and  RD_HANDLERDATE between  '{0}' and '{1}'", startDate, endDate);
            }

            DataTable dt = dbHelper.QueryDataTable(sqlstr);

            if (dt.Rows.Count < 1)
            {
                return null;
            }
            return dt;
        }
        public OperateResult HandleRestoreData()
        {
           return OperateResult.CreateSuccessResult();
        }
    }
}
