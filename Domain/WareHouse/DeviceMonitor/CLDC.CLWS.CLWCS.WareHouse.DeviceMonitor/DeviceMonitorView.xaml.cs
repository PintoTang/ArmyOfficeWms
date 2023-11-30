using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Domain;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.CLWS.CLWCS.Framework;
using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.WareHouse.DeviceMonitor
{
    /// <summary>
    /// DeviceMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceMonitorView : UserControl, IMainUseCtrl
    {
        private Dictionary<string, UserControl> _deviceMonitorDic = new Dictionary<string, UserControl>();
        public DeviceMonitorView()
        {
            InitializeComponent();
            //InitializeViewMode();
            //InitializeMonitorDetail(useControl);
        }


        public void SetMonitorViewBoxSize(double height,double width)
        {
            MonitorViewBox.Height = height;
            MonitorViewBox.Width = width;
            MonitorMainCanvas.Height = height;
            MonitorMainCanvas.Width = width;

            _oldTempWidth = width;
            _oldTempHeight = height;

        }

        private double _defaultMonitorViewBoxWidth;
        private double _defaultMonitorViewBoxHeight;

        private double _oldTempWidth { get; set; }
        private double _oldTempHeight { get; set; }
        private double _oldTempPointX { get; set; }
        private double _oldTempPointY { get; set; }

        public void AddMonitorDetail(string key,UserControl userControl)
        {
            if (_deviceMonitorDic.ContainsKey(key))
            {
                return;
            }
            _deviceMonitorDic[key] = userControl;
        }

        public void InitializeMonitorDetail(string key)
        {
            if (!_deviceMonitorDic.ContainsKey(key))
            {
                return;
            }
            MonitorDetail.Content = _deviceMonitorDic[key];
        }


        private bool mouseDown;
        private Point mouseXY;
        private void MonitorViewBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MonitorViewBox.Focus();
            var img = sender as Viewbox;
            if (img == null)
            {
                return;
            }
            img.CaptureMouse();
            if (isZoom)
            {
                img.Cursor = Cursors.SizeAll;
            }
            else
            {
                img.Cursor = Cursors.ScrollAll;
            }
            mouseDown = true;
            mouseXY = e.GetPosition(img);

        }
        private void MonitorViewBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Viewbox;
            if (img == null)
            {
                return;
            }
            img.Cursor = Cursors.Arrow;
            img.ReleaseMouseCapture();
            mouseDown = false;
        }

        private void MonitorViewBox_MouseMove(object sender, MouseEventArgs e)
        {
            var img = sender as Viewbox;
            if (img == null)
            {
                return;
            }
            if (mouseDown)
            {
                Domousemove(img, e);
            }
        }
        private void Domousemove(Viewbox img, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            //var position = e.GetPosition(img);
            //var scrollH = ScrollViewer.HorizontalOffset;
            //var scrollV = ScrollViewer.VerticalOffset;
            //this.ScrollViewer.ScrollToHorizontalOffset(-(position.X-scrollH));
            //this.ScrollViewer.ScrollToVerticalOffset(-(position.Y-scrollV));

            var group = FindResource("MonitorViewTransform") as TransformGroup;
            if (group == null)
            {
                return;
            }
            var transform = group.Children[1] as TranslateTransform;
            if (transform == null)
            {
                return;
            }
            var position = e.GetPosition(img);
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        }

        private void MonitorContent_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {

            var zoomAera = sender as Viewbox;
            if (zoomAera == null)
            {
                return;
            }
            var group = FindResource("MonitorViewTransform") as TransformGroup;
            if (group == null)
            {
                return;
            }
            var delta = e.Delta * 0.001;
            DowheelZoom(delta);
        }
        private void DowheelZoom(double delta)
        {
            if (isZoom)
            {
                double multiple = delta;
                ScaleMonitorView(multiple);
            }
        }

        private void ScaleMonitorView(double multiple)
        {
            double actualWidthValue = MonitorViewBox.ActualWidth;
            double actualHeightValue = MonitorViewBox.ActualHeight;
            double reduceWidthValue = actualWidthValue * (1 + multiple);
            double reduceHeightValue = actualHeightValue * (1 + multiple);
            if (reduceWidthValue <= 0.0 || reduceHeightValue <= 0.0)
            {
                return;
            }
            MonitorViewBox.Width = reduceWidthValue;
            MonitorViewBox.Height = reduceHeightValue;
        }

        private bool isZoom = false;
        private bool isFull = false;
        private void MonitorViewGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.LeftCtrl))
            {
                isZoom = true;
            }
        }

        private void MonitorViewGrid_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.LeftCtrl))
            {
                isZoom = false;
            }
            if (e.Key.Equals(Key.F11))
            {
                ScreenControl();
            }
        }

        private void ScreenControl()
        {
            if (isFull)
            {
                isFull = false;
                if (DelegateContainer.MinMonitoScreen != null)
                {
                    DelegateContainer.MinMonitoScreen();
                    MessageZone.Visibility = Visibility.Visible;
                    ToolZone.Visibility = Visibility.Visible;
                    StatisticsZone.Visibility = Visibility.Visible;
                    TitleZone.Visibility = Visibility.Visible;
                }
            }
            else
            {
                isFull = true;
                if (DelegateContainer.MaxMonitorScreen != null)
                {
                    DelegateContainer.MaxMonitorScreen();
                    MessageZone.Visibility = Visibility.Collapsed;
                    ToolZone.Visibility = Visibility.Collapsed;
                    StatisticsZone.Visibility = Visibility.Collapsed;
                    TitleZone.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Reduce_OnClick(object sender, RoutedEventArgs e)
        {
            double multiple = -0.1;
            ScaleMonitorView(multiple);
        }

        private void Enlarge_OnClick(object sender, RoutedEventArgs e)
        {
            double multiple = 0.1;
            ScaleMonitorView(multiple);
        }

        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_OnClick(null, null);
            }
        }


        private void BtnFullScreen_OnClick(object sender, RoutedEventArgs e)
        {
            ScreenControl();
        }

        private string _useCtrlId = "模拟监控";

        public string UseCtrlId
        {
            get
            {
                return _useCtrlId;
            }
            set { _useCtrlId = value; }
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

        private void SetScaleTransform(FrameworkElement element, ScaleTransform transform)
        {
            var sourceTransform = element.RenderTransform as TransformGroup;
            if (sourceTransform == null)
            {
                element.RenderTransform = transform;
            }
            else
            {
                for (int i = 0; i < sourceTransform.Children.Count; i++)
                {
                    if (sourceTransform.Children[i] is ScaleTransform)
                    {
                        sourceTransform.Children[i] = transform;
                        break;
                    }
                }
            }

        }


        private void ScaleElement(FrameworkElement element)
        {
            double height = element.ActualHeight;
            double width = element.ActualWidth;
            double centerX = width / 2;
            double centerY = height / 2;

            ScaleTransform scaleTransform = new ScaleTransform
            {
                CenterX = centerX,
                CenterY = centerY
            };

            SetScaleTransform(element, scaleTransform);

            DoubleAnimation enlargeAnimation = new DoubleAnimation(1, 1.2, TimeSpan.FromMilliseconds(200));
            enlargeAnimation.Completed += delegate
            {
                SetScaleTransform(element, scaleTransform);

                DoubleAnimation reduceAnimation = new DoubleAnimation(1.2, 1, TimeSpan.FromMilliseconds(200));
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, reduceAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, reduceAnimation);

            };
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, enlargeAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, enlargeAnimation);

        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                return;
            }
            if (ValidData.CheckSpecialCharacters(SearchBox.Text))
            {
                SnackbarQueue.Enqueue(string.Format("禁止非法输入：{0} ", SearchBox.Text));
                return;
            }
            string template = "Device";
            if (!(MonitorDetail.Content is FrameworkElement)) return;
            
            FrameworkElement element = MonitorDetail.Content as FrameworkElement;
            object uiElement = element.FindName(template + SearchBox.Text);
            if (uiElement == null)
            {
                SnackbarQueue.Enqueue(string.Format("查找不到编号为：{0} 的结果", SearchBox.Text));
                return;
            }
            if (!(uiElement is FrameworkElement)) return;
           
            FrameworkElement selectElement = uiElement as FrameworkElement;
            ScaleElement(selectElement);

        }

        private void DeviceMonitorView_OnLoaded(object sender, RoutedEventArgs e)
        {
            bool hasContent = ((ContentControl) MonitorDetail.Content).HasContent;
            if (!hasContent)
            {
                return;
            }
           
            var monitorCanvas = (FrameworkElement)((ContentControl)MonitorDetail.Content).Content;
            //object monitorView = ((ContentControl)MonitorDetail.Content).FindName("MonitorMainGrid");
            //Canvas monitorCanvas = monitorView as Canvas;
            //if (monitorCanvas == null)
            //{ return; }

            //记录X、Y初始值
            var group = FindResource("MonitorViewTransform") as TransformGroup;
            if (group != null)
            {
                var transform = group.Children[1] as TranslateTransform;
                if (transform != null)
                {
                    _oldTempPointX = transform.X;
                    _oldTempPointY = transform.Y;
                }
            }

            double logAh = MessageZone.ActualHeight;

            double mdAh = monitorCanvas.ActualHeight;
            double mvbAh = GdMonitorView.ActualHeight+logAh;
            if (mdAh == 0.0) return;
            double multipleH =(mvbAh / mdAh) - 1;

            double mdAw = monitorCanvas.ActualWidth;
            double mvbAw = GdMonitorView.ActualWidth;
            if (mdAw == 0.0) return;
            double multipleW = (mvbAw / mdAw) - 1;

            double multiple = 0.0;
            if (multipleH > multipleW)
            {
                multiple = multipleW;
            }
            else
            {
                multiple = multipleH;
            }

            ScaleMonitorView(multiple);

            _defaultMonitorViewBoxWidth = MonitorViewBox.Width;
            _defaultMonitorViewBoxHeight = MonitorViewBox.Height;
        }
        private string _preKey = "";
        private void Floor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null || cb.SelectedValue==null)
            {
                return;
            }
            string value = cb.SelectedValue.ToString();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            if (value.Contains("System.Windows.Controls.ComboBoxItem:"))
            {
                value = value.Split(':')[1].Trim();
            }
            if (_preKey != value)
            {
                _preKey = value;
                InitializeMonitorDetail(value);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null || cb.SelectedValue==null)
            {
                return;
            }
            string value = cb.SelectedValue.ToString();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            if (value.Contains("System.Windows.Controls.ComboBoxItem:"))
            {
                value = value.Split(':')[1].Trim();
            }

            double multiple = 100;
            string temp = value.Replace("%", "");
            bool isRight = double.TryParse(value.Replace("%", ""), out multiple);
            if (!isRight)
            {
                return;
            }

            double reduceWidthValue = _defaultMonitorViewBoxWidth * (multiple / 100);
            double reduceHeightValue = _defaultMonitorViewBoxHeight * (multiple / 100);
            if (reduceWidthValue <= 0.0 || reduceHeightValue <= 0.0)
            {
                return;
            }
            MonitorViewBox.Width = reduceWidthValue;
            MonitorViewBox.Height = reduceHeightValue;
        }

        /// <summary>
        /// 一键恢复 处理界面被拖动不见
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Init_OnClick(object sender, RoutedEventArgs e)
        {
            double multiple = 100;

            var group = FindResource("MonitorViewTransform") as TransformGroup;
            if (group != null)
            {
                var transform = group.Children[1] as TranslateTransform;
                if (transform != null)
                {
                    transform.X = _oldTempPointX;
                    transform.Y = _oldTempPointY;
                }
            }
            if (_oldTempWidth > 0.0 && _oldTempHeight > 0.0)
            {
                double reduceWidthValue = _oldTempWidth*(multiple/100);
                double reduceHeightValue = _oldTempHeight*(multiple/100);
                if (reduceWidthValue <= 0.0 || reduceHeightValue <= 0.0) return;

                MonitorViewBox.Width = reduceWidthValue;
                MonitorViewBox.Height = reduceHeightValue;
            }
        }
    }
}
