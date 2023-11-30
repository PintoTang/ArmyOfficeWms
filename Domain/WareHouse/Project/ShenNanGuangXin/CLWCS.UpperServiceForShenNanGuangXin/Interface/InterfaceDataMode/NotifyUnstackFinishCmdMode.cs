using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyUnstackFinishCmdMode
    {
        private UnStackFinishCmdData _data=new UnStackFinishCmdData();

        public UnStackFinishCmdData DATA
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class UnStackFinishCmdData
    {

        public string ADDR { get; set; }

        public StackFinishActionEnum ACTION { get; set; }
    }

    public enum StackFinishActionEnum
    {
        /// <summary>
        /// 抬起
        /// </summary>
        [Description("抬起")]
        UP=1,
        /// <summary>
        /// 放下
        /// </summary>
        [Description("放下")]
        DOWN=2
    }
}
