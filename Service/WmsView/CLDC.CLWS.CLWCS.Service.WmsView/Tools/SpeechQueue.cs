using System.Collections.Concurrent;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Tools
{
    public class SpeechQueue
    {
        private object synObj = new object();
        ConcurrentQueue<string> QueueSpeechMsg = new ConcurrentQueue<string>();
        private static SpeechQueue instance;

        private SpeechQueue()
        {

        }

        public static SpeechQueue Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpeechQueue();
                }
                return instance;
            }
        }

        public int Count
        {
            get
            {
                return QueueSpeechMsg.Count;
            }
        }

        public string Dequeue()
        {
            string result = string.Empty;
            QueueSpeechMsg.TryDequeue(out result);
            return result;
        }

        public void Enqueue(string msg)
        {
            lock (synObj)
            {
                QueueSpeechMsg.Enqueue(msg);
            }
        }
    }
}
