using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class UcStationLogInfoModel : NotifyObject
    {
        private string _dateTime;
        private string _content;
        private EnumLogLevel level;

        /// <summary>
        /// 日志的错误等级
        /// </summary>
        public EnumLogLevel Level
        {
            get { return level; }
            set
            {
                if (level != value)
                {
                    level = value;
                    RaisePropertyChanged("Level");
                }
            }
        }

        /// <summary>
        /// 时间
        /// </summary>
        public string DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    RaisePropertyChanged("DateTime");
                }
            }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }
    }
}
