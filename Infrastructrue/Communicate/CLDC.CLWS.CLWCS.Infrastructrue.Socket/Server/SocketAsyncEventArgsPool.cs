using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Server
{
    internal sealed class SocketAsyncEventArgsPool : IDisposable
    {
        /// <summary>
        /// 用来存放空闲的连接的，使用时pop出来，使用完后push进去。 
        /// </summary>
        internal Stack<SocketAsyncEventArgsWithId> SocketClientPool;
        /// <summary>
        /// busypool是一个字典类型的，用来存放正在使用的连接的，key是用户标识，设计的目的是为了统计在线用户数目和查找相应用户的连接，
        /// 当然这是很重要的，为什么设计成字典类型的，是因为我们查找时遍历字典的关键字就行了而不用遍历每一项的UID，这样效率会有所提高。 
        /// </summary>
        internal IDictionary<string, SocketAsyncEventArgsWithId> BusySocketClientDic;

        /// <summary>
        /// 这是一个存放用户标识的数组，起一个辅助的功能
        /// </summary>
        private readonly string[] _keys;
        /// <summary>
        /// 返回连接池中可用的连接数。 
        /// </summary>
        internal Int32 Count
        {
            get
            {
                lock (this.SocketClientPool)
                {
                    return this.SocketClientPool.Count;
                }
            }
        }
        /// <summary>
        /// 返回在线用户的标识列表
        /// </summary>
        internal string[] OnlineUid
        {
            get
            {
                lock (this.BusySocketClientDic)
                {
                    BusySocketClientDic.Keys.CopyTo(_keys, 0);
                }
                return _keys;
            }
        }

        internal SocketAsyncEventArgsPool(Int32 capacity)
        {
            _keys = new string[capacity];
            this.SocketClientPool = new Stack<SocketAsyncEventArgsWithId>(capacity);
            this.BusySocketClientDic = new Dictionary<string, SocketAsyncEventArgsWithId>(capacity);
        }
        /// <summary>
        /// 用于获取一个可用连接给用户
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal SocketAsyncEventArgsWithId Pop(string uid)
        {
            if (String.IsNullOrEmpty(uid))
                return null;
            SocketAsyncEventArgsWithId si = null;
            lock (this.SocketClientPool)
            {
                si = this.SocketClientPool.Pop();
            }
            si.Uid = uid;
            si.State = true;    //mark the state of Pool is not the initial step
            BusySocketClientDic.Add(uid, si);
            return si;
        }

        /// <summary>
        /// 把一个使用完的连接放回连接池
        /// </summary>
        /// <param name="item"></param>
        internal void Push(SocketAsyncEventArgsWithId item)
        {
            if (item == null)
                throw new ArgumentNullException("SocketAsyncEventArgsWithId对象为空");
            if (item.State == true)
            {
                if (BusySocketClientDic.Keys.Count != 0)
                {
                    if (BusySocketClientDic.Keys.Contains(item.Uid))
                        BusySocketClientDic.Remove(item.Uid);
                    else
                        throw new ArgumentException("SocketAsyncEventWithId不在忙碌队列中");
                }
                else
                    throw new ArgumentException("忙碌队列为空");
            }
            item.Uid = "-1";
            item.State = false;
            lock (this.SocketClientPool)
            {
                this.SocketClientPool.Push(item);
            }
        }
        /// <summary>
        /// 查找在线用户连接，返回这个连接。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal SocketAsyncEventArgsWithId FindByUid(string uid)
        {
            if (String.IsNullOrEmpty(uid))
                return null;
            SocketAsyncEventArgsWithId si = null;
            String[] onLineUIDS = this.OnlineUid;
            foreach (string key in onLineUIDS)
            {
                if (key == uid)
                {
                    si = BusySocketClientDic[uid];
                    break;
                }
            }
            return si;
        }
        /// <summary>
        /// 判断某个用户的连接是否在线
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal bool BusyPoolContains(string uid)
        {
            lock (this.BusySocketClientDic)
            {
                return BusySocketClientDic.Keys.Contains(uid);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            SocketClientPool.Clear();
            BusySocketClientDic.Clear();
            SocketClientPool = null;
            BusySocketClientDic = null;
        }

        #endregion
    }
    /// <summary>
    /// 该类是一个用户的连接的最小单元，也就是说对一个用户来说有两个SocketAsyncEventArgs对象，这两个对象是一样的，但是有一个用来发送消息，一个接收消息，这样做的目的是为了实现双工通讯，提高用户体验。
    /// </summary>
    internal sealed class SocketAsyncEventArgsWithId : IDisposable
    {
        /// <summary>
        /// 默认的用户标识是"-1”
        /// </summary>
        private string _uid = "-1";
        /// <summary>
        /// 状态是false表示不可用
        /// </summary>
        private bool _state = false;
        private MySocketAsyncEventArgs _receivesaea;
        private MySocketAsyncEventArgs _sendsaea;

        internal string Uid
        {
            get { return _uid; }
            set
            {
                _uid = value;
                ReceiveSAEA.Uid = value;
                SendSAEA.Uid = value;
            }
        }
        /// <summary>
        /// 当前是否连接在线状态
        /// </summary>
        internal bool State
        {
            get { return _state; }
            set { this._state = value; }
        }
        internal MySocketAsyncEventArgs ReceiveSAEA
        {
            set { _receivesaea = value; }
            get { return _receivesaea; }
        }
        internal MySocketAsyncEventArgs SendSAEA
        {
            set { _sendsaea = value; }
            get { return _sendsaea; }
        }

        //constructor
        internal SocketAsyncEventArgsWithId()
        {
            ReceiveSAEA = new MySocketAsyncEventArgs("Receive");
            SendSAEA = new MySocketAsyncEventArgs("Send");

        }

        #region IDisposable Members

        public void Dispose()
        {
            ReceiveSAEA.Dispose();
            SendSAEA.Dispose();
        }

        #endregion
    }
    internal sealed class MySocketAsyncEventArgs : SocketAsyncEventArgs
    {
        /// <summary>
        /// 用户标识符，用来标识这个连接是那个用户的。
        /// </summary>
        internal string Uid;
        private readonly string _property;
        //internal RequestHandler ReqHandler;

        /// <summary>
        /// MySocketAsyncEventArgs类只带有一个参数的构造函数，说明类在实例化时就被说明是用来完成接收还是发送任务的。
        /// </summary>
        /// <param name="property">标识该连接是用来发送信息还是监听接收信息的。</param>
        internal MySocketAsyncEventArgs(string property)
        {
            this._property = property;
            if (_property == "Receive")
            {
                //ReqHandler = new RequestHandler();
            }
        }

    }
}
