using System;
using System.Windows;
using System.Windows.Input;

namespace CLDC.Infrastructrue.UserCtrl.View.ProgressBar
{
    /// <summary>
    /// WaitView.xaml 的交互逻辑
    /// </summary>
    public partial class FrmProgressBar : Window
    {
        public FrmProgressBar(string progressBarTitle)
        {
            InitializeComponent();
            SetTilte(progressBarTitle);
            ProgressBarFinishType = FinishType.Finish;
        }

        public FrmProgressBar(string progressBarTitle, int waitTimeOut, int maxTimeOut, FinishType progressBarType = FinishType.WaitTime)
        {
            InitializeComponent();
            SetTilte(progressBarTitle);
            TimeOut = waitTimeOut;
            ProgressBarFinishType = progressBarType;
            MaxTimeOut = maxTimeOut;
        }

        public void SetTilte(string title)
        {
            this.LbTitle.Content = title;
        }

        public void SetProcessValue(double value)
        {
            this.ProBar.Value = value;

        }

        public FrmProgressBar(string progressBarTitle, int waitTimeOut, FinishType progressBarType = FinishType.WaitTime)
        {
            InitializeComponent();
            SetTilte(progressBarTitle);
            TimeOut = waitTimeOut;
            ProgressBarFinishType = progressBarType;
        }

        public void SetCurrentRunValue(double stepNum, string stepMsg)
        {
            this.Dispatcher.Invoke(new Action<double, string>(SetCurrentValue), new object[] { stepNum, stepMsg });
        }
        private void SetCurrentValue(double stepNum, string stepMsg)
        {
            if (stepNum <= 0)
            {
                IsFinish = false;
                this.Show();
            }
            this.LbReportMessage.Content = stepMsg;
            this.ProBar.Value = stepNum;
            if (Math.Ceiling(stepNum) >= 100)
            {
                this.Close();
                IsFinish = true;
            }
        }

        private void ThreadPoolWait(object obj)
        {
            DateTime start = DateTime.Now;
            while (!IsFinish)
            {
                double timeSpan = (DateTime.Now - start).TotalMilliseconds;
                if (ProgressBarFinishType.Equals(FinishType.WaitTime))
                {
                    if (timeSpan > TimeOut)
                    {
                        break;
                    }
                }
                else
                {
                    if (timeSpan > MaxTimeOut)
                    {
                        MessageBoxResult result = MessageBoxResult.Yes;
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            result = MessageBoxEx.Show("等待超时，是否继续等待", "提示", MessageBoxButton.YesNo);
                        }));
                        if (result == MessageBoxResult.Yes)
                        {
                            start = DateTime.Now;
                            continue;
                        }
                        break;
                    }
                }
                if (IsFinish)
                {
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private int _maxTimeOut = 1000 * 60 * 500;
        private bool _isFinish = false;

        public int TimeOut { get; set; }

        public int MaxTimeOut
        {
            get { return _maxTimeOut; }
            set { _maxTimeOut = value; }
        }

        public FinishType ProgressBarFinishType { get; set; }


        public bool IsFinish
        {
            get { return _isFinish; }
            set { _isFinish = value; }
        }

        private void WaitView_OnLoaded(object sender, RoutedEventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(ThreadPoolWait, null);
        }
    }
    public enum FinishType
    {
        WaitTime,
        Finish
    }
}
