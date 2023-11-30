using MaterialDesignThemes.Wpf;

namespace CLDC.Infrastructrue.UserCtrl.Model
{
    public class SnackbarQueue
    {
        private readonly static object messageLock=new object();
        private static SnackbarMessageQueue _messageQueue;
        public static SnackbarMessageQueue MessageQueue
        {
            get
            {
                lock (messageLock)
                {
                    if (_messageQueue == null)
                    {
                        _messageQueue = new SnackbarMessageQueue();
                    }
                }
                return _messageQueue;
            }
        }

        public static void Enqueue(string msg)
        {
            MessageQueue.Enqueue(msg);
        }

    }
}
