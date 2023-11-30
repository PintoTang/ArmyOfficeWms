using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WHSE.Monitor.Framework.UserControls
{
    public class ShelfModelViewModel:ViewModelBase
    {
        private int _columnNo;

        public int ColumnNo
        {
            get { return _columnNo; }
            set
            {
                _columnNo = value;
                RaisePropertyChanged();
            }
        }
    }
}
