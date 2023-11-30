namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    /// <summary>
    /// 调用外部服务的返回接口
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        bool IsSuccess { get; }
        /// <summary>
        /// 实体转换成Json
        /// </summary>
        /// <returns></returns>
        string ToJsonMsg();
        /// <summary>
        /// Json转换实体
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        object ToObject(string json);
        /// <summary>
        /// 创建成功的返回
        /// </summary>
        /// <returns></returns>
        string ToSuccessMsg();

        /// <summary>
        /// 创建失败返回
        /// </summary>
        /// <returns></returns>
        string ToFailMsg();
    }
}
