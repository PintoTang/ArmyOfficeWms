using CL.Framework.CmdDataModelPckg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WHSE.Monitor.Client.UserControls;
using WHSE.Monitor.Framework.UserControls.Running.Abs;

namespace WHSE.Monitor.Framework.UserControls
{
    /// <summary>
    /// StackingCrane.xaml 的交互逻辑
    /// </summary>
    public partial class StackingCraneBase : DeviceSimulation, ILogisticsDeviceInfo
    {
        public StackingCraneOperationAbs stackingCraneOperation;
        public StackingCraneBase()
        {
            InitializeComponent();
        }

        protected const string _headName = "StackingCrane#";
        protected string _userControlName;
        /// <summary>
        /// 控件名
        /// </summary>
        public virtual string UserControlName
        {
            get { return _userControlName; }
            set
            {
                _userControlName = _headName + value;

            }
        }

        private double _speed = 1.0;




        private double _railWidth;
        /// <summary>
        /// 轨道宽度
        /// </summary>
        public double RailWidth
        {
            get { return _railWidth; }
            set
            {
                _railWidth = value;
            }
        }

        private double _colunmWidth = 10;

        private double _startOffset = 40;





        public virtual void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (stackingCraneOperation != null)
            {
                stackingCraneOperation.ShowDetailForm();

            }

        }

        public virtual void btn_Click(object sender, RoutedEventArgs e)
        {

        }

        public virtual DoubleAnimationBase getPath(DeviceName from, DeviceName to)
        {
            throw new NotImplementedException();
        }


        private bool _isEndDevice;

        public bool IsEndDevice
        {
            get { return _isEndDevice; }
            set
            {
                _isEndDevice = value;
            }
        }

        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public double ColunmWidth
        {
            get { return _colunmWidth; }
            set { _colunmWidth = value; }
        }

        public double StartOffset
        {
            get { return _startOffset; }
            set { _startOffset = value; }
        }

        /// <summary>
        /// 堆垛机行走方向
        /// </summary>
        public enum RunDirectionEm
        {
            /// <summary>
            /// 从左到右
            /// </summary>
            LeftToRight = 0,
            /// <summary>
            /// 从右到左
            /// </summary>
            RightToLeft = 1,
            /// <summary>
            /// 从上到下
            /// </summary>
            UpToDown = 2,
            /// <summary>
            /// 从下到上
            /// </summary>
            DownToUp = 3
        }


        public static readonly DependencyProperty CurDirectionProperty = DependencyProperty.Register(
      "DiretionEm", typeof(RunDirectionEm), typeof(StackingCraneBase), new PropertyMetadata(default(RunDirectionEm)));

        public RunDirectionEm DiretionEm
        {
            get { return (RunDirectionEm)GetValue(CurDirectionProperty); }
            set { SetValue(CurDirectionProperty, value); }
        }


        private int _sumColNum;
        /// <summary>
        /// 当前堆垛机 对应的总列数
        /// </summary>
        public int SumColNum
        {
            get { return _sumColNum; }
            set { _sumColNum = value; }
        }


        private void StackingCraneBase_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                StackingCraneUseCtrl.ColumnWidth = this.ColunmWidth;
                StackingCraneUseCtrl.ColumnStartOffset = this.StartOffset;
                StackingCraneUseCtrl.Speed = this.Speed;
                StackingCraneUseCtrl.CurSumColNum = SumColNum;
                StackingCraneUseCtrl.CurRunDirectionEm = DiretionEm;

               StackingCraneUseCtrl.ReLoad(StackingCraneUseCtrl.CurColumn);
            }, DispatcherPriority.Background);
        }
    }
    public enum UserControlLocation
    {
        /// <summary>
        /// 原点位置
        /// </summary>
        StartingPosition,
        /// <summary>
        /// 反原点位置
        /// </summary>
        EndPosition

    }

}
