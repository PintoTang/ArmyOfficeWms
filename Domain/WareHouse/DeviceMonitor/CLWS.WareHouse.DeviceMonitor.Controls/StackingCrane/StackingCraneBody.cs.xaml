using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WHSE.Monitor.Framework.UserControls
{
    /// <summary>
    /// StackingCraneBody.xaml 的交互逻辑
    /// </summary>
    public partial class StackingCraneBody : UserControl
    {
        public StackingCraneBody()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty PreColumnProperty = DependencyProperty.Register(
            "PreColumn", typeof(int), typeof(StackingCraneBody), new PropertyMetadata(default(int)));
        /// <summary>
        /// 前一次的列
        /// </summary>
        public int PreColumn
        {
            get { return (int)GetValue(PreColumnProperty); }
            set { SetValue(PreColumnProperty, value); }
        }

        public static readonly DependencyProperty CurColumnProperty = DependencyProperty.Register(
          "CurColumn", typeof(int), typeof(StackingCraneBody), new FrameworkPropertyMetadata(default(int), CurColumnChangedCallback));
        /// <summary>
        /// 新的排
        /// </summary>
        public int CurColumn
        {
            get { return (int)GetValue(CurColumnProperty); }
            set { SetValue(CurColumnProperty, value); }
        }

        private static void CurColumnChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var stackingCrane = (StackingCraneBody)dependencyObject;
            int curColumn = (int)dependencyPropertyChangedEventArgs.NewValue;
            
           stackingCrane.TransformToNewPosition(curColumn);
        }
        public void ReLoad(int curColumn)
        {
            this.TransformToNewPosition(curColumn);
        }

        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register("ColumnWidth", typeof(double), typeof(StackingCraneBody), new PropertyMetadata(default(double)));

        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty ColumnStartOffsetProperty = DependencyProperty.Register("ColumnStartOffset", typeof(double), typeof(StackingCraneBody), new PropertyMetadata(default(double)));

        public double ColumnStartOffset
        {
            get { return (double)GetValue(ColumnStartOffsetProperty); }
            set { SetValue(ColumnStartOffsetProperty, value); }
        }


        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(double), typeof(StackingCraneBody), new PropertyMetadata(default(double)));

        public double Speed
        {
            get { return (double)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public static readonly DependencyProperty CurDirProperty = DependencyProperty.Register(
         "CurRunDirectionEm", typeof(StackingCraneBase.RunDirectionEm), typeof(StackingCraneBody), new PropertyMetadata(default(StackingCraneBase.RunDirectionEm), CurDirChangedCallback));
  
        public StackingCraneBase.RunDirectionEm CurRunDirectionEm
        {
            get { return (StackingCraneBase.RunDirectionEm)GetValue(CurDirProperty); }
            set { SetValue(CurDirProperty, value); }
        }
        private static void CurDirChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var stackingCrane = (StackingCraneBody)dependencyObject;
            StackingCraneBase.RunDirectionEm curRunDirEm = (StackingCraneBase.RunDirectionEm)dependencyPropertyChangedEventArgs.NewValue;

            int curColNum = (int)CurColumnProperty.DefaultMetadata.DefaultValue;
            stackingCrane.TransformToNewPosition(curColNum);
        }




        public static readonly DependencyProperty CurSumColNumProperty = DependencyProperty.Register(
         "CurSumColNum", typeof(int), typeof(StackingCraneBody),  new PropertyMetadata(new int()));
        /// <summary>
        //当前总列数
        /// </summary>
        public int CurSumColNum
        {
            get { return (int)GetValue(CurSumColNumProperty); }
            set { SetValue(CurSumColNumProperty, value); }
        }
        private void TransformToNewPosition(int curColumn)
        {
            if(curColumn>CurSumColNum)
            {
                curColumn = 0;
            }
            if (CurRunDirectionEm == StackingCraneBase.RunDirectionEm.LeftToRight)
            {
                TransformXLeftToRightNewPosition(curColumn);
            }
            else if (CurRunDirectionEm == StackingCraneBase.RunDirectionEm.RightToLeft)
            {
                TransformXRightToLeftNewPosition(curColumn);
            }
          
           
        }
        /// <summary>
        /// 1 从右到左移动
        /// </summary>
        /// <param name="curColumn"></param>
        private void TransformXRightToLeftNewPosition(int curColumn)
        {

            double columnWidth = ColumnWidth;
            double toX = 0.0;

            if (curColumn == 0)
            {
                toX = CurSumColNum * columnWidth + ColumnStartOffset;
            }
            else
            {
                toX = (CurSumColNum - curColumn) * columnWidth + columnWidth / 2;
                //if (curColumn != CurSumColNum)
                //{
                //    toX = (CurSumColNum - curColumn) * columnWidth;
                //}
                //else
                //{
                //    toX = (CurSumColNum - curColumn) * columnWidth + columnWidth / 2;
                //}
            }
            double fromX = 0.0;
            if (PreColumn == 0)
            {
                fromX = CurSumColNum * columnWidth + ColumnStartOffset;
            }
            else
            {
                fromX = columnWidth * (CurSumColNum - PreColumn) + columnWidth / 2;
                //if (PreColumn != CurSumColNum)
                //{
                //    fromX = columnWidth * (CurSumColNum - PreColumn);
                //}
                //else
                //{
                //    fromX = columnWidth * (CurSumColNum - PreColumn) + columnWidth / 2;
                //}

            }
            if (double.IsNaN(fromX))
            {
                fromX = CurSumColNum * columnWidth + ColumnStartOffset;
            }
            if (double.IsNaN(toX) || double.IsInfinity(toX))
            {
                toX = CurSumColNum * columnWidth + ColumnStartOffset;
            }
            TranslateTransform tt = new TranslateTransform();
            this.RenderTransform = tt;
            DoubleAnimation dav = new DoubleAnimation(fromX, toX, new Duration(TimeSpan.FromSeconds(Speed)));
            tt.BeginAnimation(TranslateTransform.XProperty, dav);
            PreColumn = curColumn;
        }


        /// <summary>
        /// 0 从左到右移动
        /// </summary>
        /// <param name="curColumn"></param>
        private void TransformXLeftToRightNewPosition(int curColumn)
        {

            double columnWidth = ColumnWidth;
            double toX = 0.0;
            if (curColumn == 0)
            {
                toX = 0.0;
            }
            else
            {
                toX = (curColumn - 1) * columnWidth + ColumnStartOffset;
            }
            double fromX = 0.0;
            if (PreColumn == 0)
            {
                fromX = 0.0;
            }
            else
            {
                fromX = columnWidth * (PreColumn - 1) + ColumnStartOffset;
            }
            if (double.IsNaN(fromX))
            {
                fromX = 0.0;
            }
            if (double.IsNaN(toX) || double.IsInfinity(toX))
            {
                toX = 0.0;
            }
            TranslateTransform tt = new TranslateTransform();
            this.RenderTransform = tt;
            DoubleAnimation dav = new DoubleAnimation(fromX, toX, new Duration(TimeSpan.FromSeconds(Speed)));
            tt.BeginAnimation(TranslateTransform.XProperty, dav);
            PreColumn = curColumn;
        }


    }
}
