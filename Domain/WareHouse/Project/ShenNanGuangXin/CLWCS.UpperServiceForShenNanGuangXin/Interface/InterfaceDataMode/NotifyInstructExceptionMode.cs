using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyInstructExceptionMode
    {
        /// <summary>
        /// 异常指令编号
        /// </summary>
        public string INSTRUCTION_CODE { get; set; }
        /// <summary>
        /// 异常条码号
        /// </summary>
        public string PACKAGE_BARCODE { get; set; }
        /// <summary>
        /// 异常代码
        /// </summary>
        public string EXCEPTION_CODE { get; set; }
        /// <summary>
        /// 异常的当前地址
        /// </summary>
        public string CURRENT_ADDR { get; set; }

        public override string ToString()
        {
            return this.ToJson();
        }

    }
}
