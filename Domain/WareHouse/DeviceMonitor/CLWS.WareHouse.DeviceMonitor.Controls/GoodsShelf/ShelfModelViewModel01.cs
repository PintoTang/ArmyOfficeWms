using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace WHSE.Monitor.Framework.UserControls
{
    public class ShelfModelViewModel01:ViewModelBase
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
        private string _backColor;

        public string BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                RaisePropertyChanged();
            }
        }
    }
}
