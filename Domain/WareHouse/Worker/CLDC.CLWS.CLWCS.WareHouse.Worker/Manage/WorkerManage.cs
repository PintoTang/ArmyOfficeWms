using System;
using System.Collections.Generic;
using System.Reflection;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.ConfigService;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.UserCtrl.View.ProgressBar;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Manage
{
    public class WorkerManage : IManage<WorkerBaseAbstract>
    {
        private string _logName = "初始化管理";

        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static WorkerManage Instance
        {
            get
            {
                if (_workerManage == null)
                    _workerManage = new WorkerManage();
                return _workerManage;
            }
        }

        private static WorkerManage _workerManage;

        private DataManageablePool<WorkerBaseAbstract> managedDataPool = new DataManageablePool<WorkerBaseAbstract>();
        public DataManageablePool<WorkerBaseAbstract> ManagedDataPool { get { return managedDataPool; } }


      

        /// <summary>
        /// 初始化所有设备
        /// </summary>
        /// <returns></returns>
        public OperateResult InitWorkers()
        {
            OperateResult result = new OperateResult();
            try
            {
                string strCurPath = System.IO.Directory.GetCurrentDirectory();
                string strXmlPath = strCurPath + @"\Config" + @"\CoordinationConfig.xml";

                Context cc = new Context(new Coordination());
                cc.LoadXml(strXmlPath);

                string strNodeParms = @"Configuration/Coordinations";
                cc.SetXmlNode(strNodeParms);
                cc.Request();

                if (cc.CurNextNodeInfo == null) return OperateResult.CreateFailedResult("加载CoordinationConfig.xml失败", 1);

                List<Coords> coordsLst = (List<Coords>)cc.CurNextNodeInfo;

                double step = 0.0;
                int count = coordsLst.Count;

                double interval = Math.Ceiling(100.0 / count);

                foreach (var coord in coordsLst)
                {
                    LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化设备{1} {2} 命名空间：{3} 类名：{4}", DateTime.Now, coord.Id.Trim(),coord.Name,coord.NameSpace, coord.Class), EnumLogLevel.Debug);
                    ProgressBarEx.ReportProcess(step, string.Format("开始初始化组件：{0}", coord.Name), "正在初始化组件，请稍后......");
                    step = step + interval;
                    WorkerBaseAbstract orderWorker = (WorkerBaseAbstract)Assembly.Load(coord.NameSpace)
                    .CreateInstance(coord.NameSpace + "." + coord.Class);
                    WorkerBusinessAbstract orderWorkerBusiness = (WorkerBusinessAbstract)Assembly
                        .Load(coord.CoordBusinessHandle.NameSpace)
                        .CreateInstance(coord.CoordBusinessHandle.NameSpace + "." + coord.CoordBusinessHandle.Class);

                    DeviceName deviceName = new DeviceName(coord.DeviceName);
                    if (orderWorker == null)
                    {
                        return OperateResult.CreateFailedResult(string.Format("工作者 命名空间：{0} 类名：{1} 反射出错", coord.NameSpace, coord.Class), 1);
                    }
                    if (orderWorkerBusiness == null)
                    {
                        return OperateResult.CreateFailedResult(string.Format("工作业务 命名空间：{0} 类名：{1} 初始化出错", coord.CoordBusinessHandle.NameSpace, coord.CoordBusinessHandle.Class), 1);
                    }
                    int workerId = int.Parse(coord.Id.Trim());//传入值
                    string name = coord.Name;
                    int workSize = coord.WorkSize;
                    WorkerTypeEnum workerType = (WorkerTypeEnum)Enum.Parse(typeof(WorkerTypeEnum), coord.Type);

                    OperateResult initResult = orderWorker.Initailize(workerId, name, workerType, workSize, deviceName, orderWorkerBusiness, coord.Class, coord.NameSpace);
                    if (!initResult.IsSuccess)
                    {
                        return OperateResult.CreateFailedResult(string.Format("工作者：{0} 初始化出错，原因：\r\n{1}", orderWorker.Name, initResult.Message), 1);
                    }
                    OperateResult addResult = Add(orderWorker);
                    if (!addResult.IsSuccess)
                    {
                        string msg = string.Format("添加工作者：{0} 失败！失败原因：\r\n{1}", orderWorker.Name, addResult.Message);
                        return OperateResult.CreateFailedResult(msg, 1);
                    }
                    ProgressBarEx.ReportProcess(step, string.Format("结束初始化组件：{0}", coord.Name));

                    LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化设备{1} {2} 命名空间：{3} 类名：{4}", DateTime.Now, coord.Id.Trim(), coord.Name, coord.NameSpace, coord.Class), EnumLogLevel.Debug);
                }
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
                result.ErrorCode = 1;
            }
            return result;
        }

        public OperateResult LoadConfig(string path)
        {
            return OperateResult.CreateSuccessResult();
        }
        public OperateResult Add(WorkerBaseAbstract data)
        {
            return managedDataPool.AddPool(data);
        }

        public OperateResult Delete(int id)
        {
            return managedDataPool.RemovePool(id);
        }

        public WorkerBaseAbstract FindDeivceByDeviceId(int id)
        {
            OperateResult<WorkerBaseAbstract> findResult = managedDataPool.FindData(id);
            return findResult.Content;
        }

        public WorkerBaseAbstract FindDeviceByDeviceName(string name)
        {
            OperateResult<WorkerBaseAbstract> findResult = managedDataPool.FindData(name);
            return findResult.Content;
        }

        public List<WorkerBaseAbstract> GetAllData()
        {
            return ManagedDataPool.DataPool;
        }

        public OperateResult Update(WorkerBaseAbstract data)
        {
            return ManagedDataPool.UpdatePool(data);
        }
    }
}
