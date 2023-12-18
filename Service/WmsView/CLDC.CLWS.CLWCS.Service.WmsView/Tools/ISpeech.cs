namespace CLDC.CLWS.CLWCS.Service.WmsView.Tools
{
    public interface ISpeech
    {
        /// <summary>
        /// 语音播报  同步
        /// </summary>
        /// <param name="msg"></param>
        void Speak(string msg);

        /// <summary>
        /// 语音播报 异步
        /// </summary>
        /// <param name="msg"></param>
        void SpeakAsync(string msg);

        /// <summary>
        /// 停止线程
        /// </summary>
        void ThreadAbort();
    }
}
