using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.Service.License.DataModel
{
    public class SerialNumberDataModel : ViewModelBase
    {
        private int _forecastDays;
        private DateTime _expiryDate;

        /// <summary>
        /// 机器识别码
        /// </summary>
        public string IdentificationCode { get; set; }

        /// <summary>
        /// 提前预告天数
        /// </summary>
        public int ForecastDays
        {
            get { return _forecastDays; }
            set
            {
                _forecastDays = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            set
            {
                _expiryDate = value; 
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 最近一次校验注册码的日期
        /// </summary>
        public DateTime FrontDate { get; set; }

    }
}
