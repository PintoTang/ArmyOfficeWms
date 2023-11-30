using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage
{
    public sealed class TransportManage : ITransportManage, IRestore
    {
        private readonly TransportMsgDataAbstract _transportDataHandler;
        private readonly IOrderManage _orderManageHandler;

        public TransportManage(TransportMsgDataAbstract transportDatabase, IOrderManage orderManage)
        {
            _transportDataHandler = transportDatabase;
            _orderManageHandler = orderManage;
        }

        private string Name = "搬运管理";

        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level = EnumLogLevel.Info, bool isNotifyUi = true)
        {
            LogHelper.WriteLog(this.Name, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }
        private readonly UniqueDataObservablePool<TransportMessage> _managedDataPool = new UniqueDataObservablePool<TransportMessage>();
        public UniqueDataObservablePool<TransportMessage> ManagedDataPool
        {
            get { return _managedDataPool; }
        }
        public OperateResult Restore()
        {
            List<TransportResultEnum> transportStatusLst = new List<TransportResultEnum> { TransportResultEnum.UnFinish };
            OperateResult<List<TransportMsgModel>> transportMsgLst = _transportDataHandler.GetTransportDataByStatus(transportStatusLst);
            if (!transportMsgLst.IsSuccess)
            {
                LogMessage(string.Format("断点恢复没有待完成的搬运信息！\r\n 原因：{0}", transportMsgLst.Message), EnumLogLevel.Warning, true);
                return OperateResult.CreateSuccessResult();
            }

            List<TransportMsgModel> restoreTransportLst = transportMsgLst.Content;
            foreach (TransportMsgModel transportMode in restoreTransportLst)
            {
                try
                {
                    string guid = transportMode.Guid;
                    TransportMessage transportMsg = new TransportMessage(guid);
                    transportMsg.CurAddr = new Addr(transportMode.CurAddr);
                    transportMsg.DestAddr = new Addr(transportMode.DestAddr);
                    transportMsg.OwnerId = transportMode.OwnerId.GetValueOrDefault();
                    transportMsg.StartAddr = new Addr(transportMode.StartAddr);
                    transportMsg.TransportStatus = transportMode.TransportStatus;
                    OperateResult<ExOrder> orderResult = _orderManageHandler.GetMemoryExOrderByOrderId(transportMode.ExOrderId.GetValueOrDefault());
                    if (!orderResult.IsSuccess)
                    {
                        continue;
                    }
                    transportMsg.TransportOrder = orderResult.Content;
                    transportMsg.DestDevice = DeviceManage.Instance.FindDeivceByDeviceId(transportMode.DestId.GetValueOrDefault());
                    transportMsg.StartDevice = DeviceManage.Instance.FindDeivceByDeviceId(transportMode.StartId.GetValueOrDefault());
                    transportMsg.TransportDevice = DeviceManage.Instance.FindDeivceByDeviceId(transportMode.TransportId.GetValueOrDefault());
                    transportMsg.TransportFinishType = transportMode.TransportFinishType;//纠正
                    transportMsg.TrayType = transportMode.TrayType.GetValueOrDefault();
                    _managedDataPool.AddPool(transportMsg);
                    LogMessage(string.Format("断点恢复获取到搬运信息：{0}", transportMsg.TransportOrder.OrderId), EnumLogLevel.Debug,
                        false);
                }
                catch (Exception ex)
                {
                    LogMessage(
                        string.Format("断点恢复的指令ID：{0} 发生异常: \r\n {1}", transportMode.ExOrderId,
                            OperateResult.ConvertException(ex)), EnumLogLevel.Error, true);
                }
            }
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult HandleRestoreData()
        {
            OperateResult restoreResult = Restore();
            if (!restoreResult.IsSuccess)
            {
                return restoreResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult RemoveTransport(TransportMessage transport)
        {
           return _managedDataPool.RemovePool(transport);
        }

        public UniqueDataObservablePool<TransportMessage> GetAllTransportMessageByExOrderId(int orderId)
        {
            UniqueDataObservablePool<TransportMessage> dataPool = new UniqueDataObservablePool<TransportMessage>();
            Expression<Func<TransportMsgModel, bool>> whereLambda=t=>t.ExOrderId==orderId;
            //string sql = string.Format("SELECT *FROM T_BO_TRANSPORTDATA WHERE EXORDERID='{0}'",orderId);
            OperateResult<List<TransportMsgModel>> transportMsgLst = _transportDataHandler.GetTransportData(1,10000, "ADDDATETIME ASC", whereLambda,out int totalCount);
            if (!transportMsgLst.IsSuccess)
            {
                return dataPool;
            }

            List<TransportMsgModel> restoreTransportLst = transportMsgLst.Content;
            foreach (TransportMsgModel transportMode in restoreTransportLst)
            {
                try
                {
                    string guid = transportMode.Guid;
                    TransportMessage transportMsg = new TransportMessage(guid);
                    transportMsg.CurAddr = new Addr(transportMode.CurAddr);
                    transportMsg.DestAddr = new Addr(transportMode.DestAddr);
                    transportMsg.OwnerId = transportMode.OwnerId.GetValueOrDefault();
                    transportMsg.StartAddr = new Addr(transportMode.StartAddr);
                    transportMsg.TransportStatus = transportMode.TransportStatus;
                    OperateResult<ExOrder> orderResult = _orderManageHandler.GetDataBaseExOrderByOrderId(transportMode.ExOrderId.GetValueOrDefault());
                    if (!orderResult.IsSuccess)
                    {
                        continue;
                    }
                    transportMsg.TransportOrder = orderResult.Content;
                    transportMsg.DestDevice = DeviceManage.Instance.FindDeivceByDeviceId(transportMode.DestId.GetValueOrDefault());
                    transportMsg.StartDevice = DeviceManage.Instance.FindDeivceByDeviceId(transportMode.StartId.GetValueOrDefault());
                    transportMsg.TransportDevice = DeviceManage.Instance.FindDeivceByDeviceId(transportMode.TransportId.GetValueOrDefault());
                    transportMsg.TransportFinishType = transportMode.TransportFinishType;//纠正
                    transportMsg.TrayType = transportMode.TrayType.GetValueOrDefault();
                    dataPool.AddPool(transportMsg);
                    LogMessage(string.Format("断点恢复获取到搬运信息：{0}", transportMsg.TransportOrder.OrderId), EnumLogLevel.Debug,
                        false);
                }
                catch (Exception ex)
                {
                    LogMessage(
                        string.Format("断点恢复的指令ID：{0} 发生异常: \r\n {1}", transportMode.ExOrderId,
                            OperateResult.ConvertException(ex)), EnumLogLevel.Error, true);
                }
            }
            return dataPool;
        }

        public UniqueDataObservablePool<TransportMessage> GetAllUnFinishedTransportByOwnerId(int ownerId)
        {
            UniqueDataObservablePool<TransportMessage> dataPool = new UniqueDataObservablePool<TransportMessage>();
            IEnumerable<TransportMessage> dataPoolContainer = _managedDataPool.FindAllData(t => t.OwnerId.Equals(ownerId));
            foreach (TransportMessage transport in dataPoolContainer)
            {
                dataPool.DataPool.Add(transport);
            }
            return dataPool;

        }

        public OperateResult UpdateTransport(TransportMessage transport)
        {
            OperateResult updateResult = _managedDataPool.UpdatePool(transport);
            if (!updateResult.IsSuccess)
            {
                string msg = string.Format("更新搬运信息：{0} 出错", transport.UniqueCode);
                LogMessage(msg);
                return updateResult;
            }
            _transportDataHandler.Save(transport.DatabaseMode);

            return OperateResult.CreateSuccessResult();
        }

        public void UpdateTransportAsync(TransportMessage transport)
        {
            OperateResult updateResult = _managedDataPool.UpdatePool(transport);
            if (!updateResult.IsSuccess)
            {
                string msg = string.Format("更新搬运信息：{0} 出错", transport.UniqueCode);
                LogMessage(msg);
            }
            _transportDataHandler.SaveAsync(transport.DatabaseMode);
        }

        public UniqueDataObservablePool<TransportMessage> GetAllUnFinishedTransportByDestId(int destId)
        {
            UniqueDataObservablePool<TransportMessage> dataPool = new UniqueDataObservablePool<TransportMessage>();
            IEnumerable<TransportMessage> dataPoolContainer = _managedDataPool.FindAllData(t => t.DestDevice.Id.Equals(destId));
            foreach (TransportMessage transport in dataPoolContainer)
            {
                dataPool.DataPool.Add(transport);
            }
            return dataPool;
        }
    }
}
