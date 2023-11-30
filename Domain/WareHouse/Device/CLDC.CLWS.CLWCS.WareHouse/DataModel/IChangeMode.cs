using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    /// <summary>
    /// 切换模式的接口
    /// </summary>
    public interface IChangeMode
    {
        /// <summary>
        /// 当前的运行模式
        /// </summary>
        DeviceModeEnum CurMode { get; set; }
        /// <summary>
        /// 询问是否可以切换模式
        /// </summary>
        /// <param name="destMode"></param>
        /// <returns></returns>
        OperateResult IsCanChangeMode(DeviceModeEnum destMode);
        /// <summary>
        /// 切换到指定模式
        /// </summary>
        /// <param name="destMode"></param>
        /// <returns></returns>
        OperateResult ChangeMode(DeviceModeEnum destMode);

        /// <summary>
        /// 验证是否是指定模式
        /// </summary>
        /// <param name="destMode"></param>
        /// <returns></returns>
        bool CheckMode(DeviceModeEnum destMode);
    }
}
