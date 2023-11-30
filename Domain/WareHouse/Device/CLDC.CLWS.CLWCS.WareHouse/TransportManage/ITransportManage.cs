using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage
{
    public interface  ITransportManage
    {
        UniqueDataObservablePool<TransportMessage> ManagedDataPool { get; }
        /// <summary>
        /// 通过拥有这获取搬运信息
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        UniqueDataObservablePool<TransportMessage> GetAllUnFinishedTransportByOwnerId(int ownerId);
        /// <summary>
        /// 更新搬运信息
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        OperateResult UpdateTransport(TransportMessage transport);

        /// <summary>
        /// 异步更新搬运信息
        /// </summary>
        /// <param name="transport"></param>
        void UpdateTransportAsync(TransportMessage transport);

        /// <summary>
        /// 通过目标ID获取搬运信息
        /// </summary>
        /// <param name="destId"></param>
        /// <returns></returns>
        UniqueDataObservablePool<TransportMessage> GetAllUnFinishedTransportByDestId(int destId);

        /// <summary>
        /// 处理恢复设备的状态和数据
        /// </summary>
        /// <returns></returns>
        OperateResult HandleRestoreData();

        /// <summary>
        /// 移除内存
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        OperateResult RemoveTransport(TransportMessage transport);

        UniqueDataObservablePool<TransportMessage> GetAllTransportMessageByExOrderId(int orderId);


    }
}
