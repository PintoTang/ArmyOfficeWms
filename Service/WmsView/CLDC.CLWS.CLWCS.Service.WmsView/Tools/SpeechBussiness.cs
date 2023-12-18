using CLDC.Infrastructrue.UserCtrl.Model;
using System;
using System.Speech.Synthesis;
using System.Threading;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Tools
{
    /// <summary>
    /// 语音帮助类
    /// </summary>
    public class SpeechBussiness : ISpeech
    {
        Thread thread = null;
        SpeechSynthesizer speakAsync = null;
        bool isAbort = false;
        bool lastCompleted = true;
        public SpeechBussiness()
        {
            speakAsync = new SpeechSynthesizer();
            speakAsync.Rate = 0;

            speakAsync.SpeakCompleted += speakAsync_SpeakCompleted;
            thread = new Thread(new ThreadStart(ThreadSpeakAsync));
            thread.Start();
        }

        private void ThreadSpeakAsync()
        {
            while (true)
            {
                if (!isAbort)
                {
                    try
                    {
                        if (lastCompleted)
                        {
                            if (SpeechQueue.Instance.Count > 0)
                            {
                                string msg = SpeechQueue.Instance.Dequeue();
                                lastCompleted = false;
                                if (!string.IsNullOrEmpty(msg))
                                {
                                    speakAsync.SpeakAsync(msg);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SnackbarQueue.MessageQueue.Enqueue("语音播报异常：" + ex.Message);
                    }
                    Thread.Sleep(50);
                }
            }
        }

        void speakAsync_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            lastCompleted = e.Prompt.IsCompleted;
        }

        public void Speak(string msg)
        {
            SpeechSynthesizer speak = new SpeechSynthesizer();
            speak.Speak(msg);
            speak.Dispose();
        }

        public void SpeakAsync(string msg)
        {
            SpeechQueue.Instance.Enqueue(msg);
        }


        public void ThreadAbort()
        {
            isAbort = true;
            speakAsync.Dispose();
            thread.Abort();
        }
    }

}
