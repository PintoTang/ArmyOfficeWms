using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyInstructFinishCmdMode
    {
        private NotifyInstructFinishData _data=new NotifyInstructFinishData();

        public NotifyInstructFinishData DATA
        {
            get { return _data; }
            set { _data = value; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class NotifyInstructFinishData
    {
        public int ID { get; set; }
        public string PACKAGE_BARCODE { get; set; }
        public string DST_ADDR { get; set; }
    }
}
