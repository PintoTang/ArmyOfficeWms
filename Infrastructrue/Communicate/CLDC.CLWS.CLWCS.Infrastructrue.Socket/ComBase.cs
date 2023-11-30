using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets
{
    public abstract class ComBase
    {
        /// <summary>
        /// 收取数据数组
        /// </summary>
        protected byte[] ReciveBytes;

        /// <summary>
        /// 收取数据事件
        /// </summary>
        public event ReceiveSocketData RvDataEvent;
        /// <summary>
        /// 发送数据事件
        /// </summary>
        public event SendData SendDataEvent;
        /// <summary>
        /// 收取数据委托
        /// </summary>
        public delegate void ReceiveSocketData(byte[] P_Buffer);
        /// <summary>
        /// 发送数据委托
        /// </summary>
        public delegate void SendData(byte[] P_Buffer);
        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract int Send(byte[] bysts);

        /// <summary>
        /// 发送数据
        /// </summary>
        public abstract bool Close();
        /// <summary>
        /// 收取数据
        /// </summary>
        public abstract int Receive(byte[] bytes);
        /// <summary>
        /// 收取数据事件方法
        /// </summary>
        protected void ReceiveData_Event(byte[] P_Buffer)
        {
            if (RvDataEvent != null)
            {
                RvDataEvent(P_Buffer);
            }
        }
        /// <summary>
        /// 发送数据事件方法
        /// </summary>
        protected void SendData_Event(byte[] P_Buffer)
        {
            if (SendDataEvent != null)
            {
                SendDataEvent(P_Buffer);
            }
        }
    }
}
