using System.ServiceModel;

namespace CLWCS.WareHouse.WebserviceForHeFei
{
    /// <summary>
    /// 提供科陆SNDL的服务接口
    /// </summary>
    [ServiceContract(Namespace = "http://www.szclou.com")]
    public interface IWebserviceForHeFei
    {
        /// <summary>
        /// 下发指令接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string SendInstruct(string cmd);

        /// <summary>
        /// 下发拆垛任务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string SendUnstackCmd(string cmd);

      

        /// <summary>
        /// 下发设备模式
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string SendDeviceMode(string cmd);

          /// <summary>
        /// 下发称重请求
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string SendWeightRequest(string cmd);

        /// <summary>
        /// 刷卡开始、  刷卡超时结束
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string NotifyScannerCard(string cmd);

        /// <summary>
        /// 请求开门，请求关门
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string OpenOrCloseDoorRequest(string cmd);


        /// <summary>
        /// 通知报警 开始和结束
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string NotifyOrClearWarning(string cmd); 


         /// <summary>
        /// 强制结束 WMS  拆垛 码垛
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string ForceCloseTask(string cmd);

        /// <summary>
        /// 非法开门 报警与消除
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string NotifyTrespassOpenDoor(string cmd);

        /// <summary>
        /// 出入库 需要重新开始做任务 开始扫描接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string ReSendTask(string cmd);

        /// <summary>
        /// WMS通知开始任务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string NotifyStartTask(string cmd);


        /// <summary>
        /// WMS下发库存整理任务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string NotifyInventoryArrangeTask(string cmd);

        /// <summary>
        /// 请求开关门
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string ApplyCrossDoorRequest(string cmd);

        /// <summary>
        /// AGV搬运完成通知
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string NotifyAgvCarryFinished(string cmd);

        [OperationContract]
        string RobotHasTrayRequest();

        /// <summary>
        /// 下发机器人
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [OperationContract]
        string NotifyPlcRobotIn(string cmd);

        [OperationContract]
        string NotifyPriorityChange(string cmd);
    }
}
