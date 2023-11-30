using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WHSE.Monitor.Framework.UserControls
{
    /// <summary>
    /// 区域
    /// </summary>
    public class AreaBean
    {

        private string _areaAreaName;
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName
        {
            get { return _areaAreaName; }
            set { _areaAreaName = value; }
        }


        private UserControl _areaUserControl;
        /// <summary>
        /// 区域UserControl
        /// </summary>
        public UserControl AreaUserControl
        {
            get { return _areaUserControl; }
            set { _areaUserControl = value; }
        }

        private List<UserControl> _children = new List<UserControl>();
        /// <summary>
        /// UserControl列表
        /// </summary>
        public List<UserControl> Children
        {
            get { return _children; }
            set { _children = value; }
        }

    }
}
