using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcDefaultView.xaml 的交互逻辑
    /// </summary>
    public partial class UcDefaultView : UserControl
    {
        public UcDefaultView()
        {
            InitializeComponent();
            DataContext = new OrderListViewModel();
            Uri uri = new Uri(Environment.CurrentDirectory + "/Images/warehouse.png", UriKind.Absolute);
            BitmapImage bitmap = new BitmapImage(uri);
            ImgWarehouse.Source = bitmap;
        }

        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool mouseDown;
        /// <summary>
        /// 移动图片前，按下鼠标左键时，鼠标相对于ImageContainer的点
        /// </summary>
        Point moveStart;
        /// <summary>
        /// 旋转图片前，按下鼠标右键时，鼠标相对于ImageContainer的点
        /// </summary>
        Point rotateStart;

        // 鼠标左键按下
        private void ImgMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImageContainer.CaptureMouse();
            mouseDown = true;
            moveStart = e.GetPosition(ImageContainer);
        }

        // 鼠标左键释放
        private void ImgMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
            ImgWarehouse.ReleaseMouseCapture();
        }

        /// <summary>
        /// 移动图片
        /// </summary>
        /// <param name="moveEndPoint">移动图片的终点（相对于ImageContainer）</param>
        private void DoMove(Point moveEndPoint)
        {
            // 考虑到旋转的影响，因此将两个点转换到ImgWarehouse坐标系，计算x、y的增量
            Point start = ImageContainer.TranslatePoint(moveStart, ImgWarehouse);
            Point end = ImageContainer.TranslatePoint(moveEndPoint, ImgWarehouse);

            // 判断一下，如果scale很大的时候，移动会很迟缓。此时应该将移动放大
            if (scaler.ScaleX > 7)
            {
                transer.X += (end.X - start.X) * 4;
                transer.Y += (end.Y - start.Y) * 4;
            }
            else if (scaler.ScaleX > 5)
            {
                transer.X += (end.X - start.X) * 3;
                transer.Y += (end.Y - start.Y) * 3;
            }
            else if (scaler.ScaleX > 3)
            {
                transer.X += (end.X - start.X) * 2;
                transer.Y += (end.Y - start.Y) * 2;
            }
            else if (scaler.ScaleX < 0.5)
            {
                transer.X += (end.X - start.X) * 0.5;
                transer.Y += (end.Y - start.Y) * 0.5;
            }
            else
            {
                transer.X += (end.X - start.X);
                transer.Y += (end.Y - start.Y);
            }
            moveStart = moveEndPoint;
            // W+w > 2*move_x > -((2*scale-1)*w + W)  水平平移限制条件
            // H+h > 2*move_y > -((2*scale-1)*h + H)  垂直平移限制条件

            if (transer.X * 2 > ImgWarehouse.ActualWidth + ImageContainer.ActualWidth - 20)
                transer.X = (ImgWarehouse.ActualWidth + ImageContainer.ActualWidth - 20) / 2;

            if (-transer.X * 2 > (2 * scaler.ScaleX - 1) * ImgWarehouse.ActualWidth + ImageContainer.ActualWidth - 20)
                transer.X = -((scaler.ScaleX - 0.5) * ImgWarehouse.ActualWidth + ImageContainer.ActualWidth / 2 - 10);

            if (transer.Y * 2 > ImgWarehouse.ActualHeight + ImageContainer.ActualHeight - 20)
                transer.Y = (ImgWarehouse.ActualHeight + ImageContainer.ActualHeight - 20) / 2;

            if (-transer.Y * 2 > (2 * scaler.ScaleY - 1) * ImgWarehouse.ActualHeight + ImageContainer.ActualHeight - 20)
                transer.Y = -((scaler.ScaleY - 0.5) * ImgWarehouse.ActualHeight + ImageContainer.ActualHeight / 2 - 10);
        }

        //  鼠标右键按下
        private void ImgMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImageContainer.CaptureMouse();
            mouseDown = true;
            rotateStart = e.GetPosition(ImageContainer);
            // 需要注意的是：RotateTransfrom的CenterX和CenterY，始终是相对于原始坐标系（未经过变换的坐标系）的。
            // 因此，设置CenterX和CenterY之后，需要点拉回到坐标原点
            Point toImage = ImageContainer.TranslatePoint(rotateStart, ImgWarehouse); Point center = group.Transform(toImage);                      // 将中心点转换到ImgWarehouse现状的坐标系下            　　　　　　　　transer.X = (center.X - toImage.X * scaler.ScaleX);            　　　　　　　　transer.Y = (center.Y - toImage.Y * scaler.ScaleY);            　　　　　　　　rotater.CenterX = center.X;            　　　　　　　　rotater.CenterY = center.Y;
        }

        // 鼠标移动：有可能是移动图片，也有可能是旋转图片
        private void ImgMouseMove(object sender, MouseEventArgs e)
        {
            var mouseEnd = e.GetPosition(ImageContainer);           // 鼠标移动时，获取鼠标相对图片容器的点
            if (mouseDown)
            {
                if (e.LeftButton == MouseButtonState.Pressed)           // 按下鼠标左键，移动图片
                {
                    DoMove(mouseEnd);
                }
            }
        }

        // 鼠标滚轮滚动
        private void ImgMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var point = e.GetPosition(ImgWarehouse);
            var delta = e.Delta * 0.002;
            DoScale(point, delta);
        }

        /// <summary>
        /// 缩放图片。最小为0.1倍，最大为30倍
        /// </summary>
        /// <param name="point">相对于图片的点，以此点为中心缩放</param>
        /// <param name="delta">缩放的倍数增量</param>
        private void DoScale(Point point, double delta)
        {
            // 限制最大、最小缩放倍数
            if (scaler.ScaleX + delta < 0.1 || scaler.ScaleX + delta > 30) return;

            scaler.ScaleX += delta;
            scaler.ScaleY += delta;

            transer.X -= point.X * delta;
            transer.Y -= point.Y * delta;
        }

        private void OrderDetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
