using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcChart.xaml 的交互逻辑
    /// </summary>
    public partial class UcChart : UserControl
    {
        public UcChart()
        {
            InitializeComponent();
            PointLabel = chartPoint => string.Format("{0}({1:p})", chartPoint.Y, chartPoint.Participation);
            DataContext = this;
        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        private void pipChart_DataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;
            //clear selected slice
            foreach (PieSeries series in chart.Series)
            {
                series.PushOut = 0;
                var selectedSeries = (PieSeries)chartPoint.SeriesView;
                selectedSeries.PushOut = 8;
            }
        }
    }
}
