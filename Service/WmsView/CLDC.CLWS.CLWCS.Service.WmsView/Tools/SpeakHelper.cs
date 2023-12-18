using System;
using System.Speech.Synthesis;
using System.Threading;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Tools
{
    /// <summary>
    /// 语音帮助类
    /// </summary>
    public class SpeakHelper
    {
        public static SpeechSynthesizer Synth;
        private static Thread _thread;
        private static string _contentThread;

        /// <summary>
        /// 文本转语音
        /// </summary>
        /// <param name="content">文本内容</param>
        public static void SpeekContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;
            _contentThread = content;
            _thread = new Thread(SpeakContentThread);
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        private static void SpeakContentThread()
        {
            try
            {
                if (Synth != null)
                {
                    Synth.Dispose();
                }
                Synth = new SpeechSynthesizer();
                Synth.Rate = -1;
                Synth.Volume = 80;
                Synth.SpeakAsync(_contentThread);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 停止语音
        /// </summary>
        public static void StopSpeak()
        {
            try
            {
                if (Synth != null)
                {
                    Synth.Dispose();//释放 SpeechSynthesizer 在会话期间使用的对象并释放资源
                }
                if (_thread != null)
                {
                    _thread.Abort();//终止线程
                }
            }
            catch (Exception)
            { }
        }
    }
}
