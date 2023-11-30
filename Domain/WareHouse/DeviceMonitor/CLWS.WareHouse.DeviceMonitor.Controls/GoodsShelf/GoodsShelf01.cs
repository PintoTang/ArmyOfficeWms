using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WHSE.Monitor.Framework.UserControls
{
    public class GoodsShelfModelViewModel01:ViewModelBase
    {

        /// <summary>
        /// 排号
        /// </summary>
        private string _shelfRow;
        public string ShelfRow
        {
            get { return _shelfRow; }
            set 
            { 
                _shelfRow = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 深度
        /// </summary>
        private string _shelfDeep;
        public string ShelfDeep
        {
            get { return _shelfDeep; }
            set 
            { 
                _shelfDeep = value;
                RaisePropertyChanged();
            }
        }
       
    }
}
