using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcEnvironMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class UcEnvironMonitor : UserControl
    {
        private string s_OutputParms;


        public UcEnvironMonitor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int _timeSpac = 2;
            int timespac = Convert.ToInt32(1000 * _timeSpac);
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    GetDeviceData();
                    Thread.Sleep(1000);//2s刷新一下
                }
            });
        }

        private void GetDeviceData()
        {
            OperateResult<string> opResult = new OperateResult<string>();
            try
            {
                //请求设备信息及实时数据
                opResult = HttpHelper.CreateHttpResponse("http://localhost:9001/" + "app/GetDeviceData?groupId=", false, null, "2", 2);
                if (opResult.IsSuccess)
                {
                    s_OutputParms = opResult.Content;
                    EnvironMonitorReturn responseResult = (EnvironMonitorReturn)opResult.Content;
                    string strTemperature = string.Empty; string strHumidity = string.Empty;
                    string strTemper=string.Empty;string strHumi = string.Empty;
                    int infraredAlarm = 0; int infraredNoAlarm = 0;
                    int smokeAlarm = 0; int smokeNoAlarm = 0;
                    foreach (EnvironMonitorModel item in responseResult.data)
                    {
                        foreach (RealTimeData data in item.realTimeData)
                        {
                            if (data.dataName.Contains("温度"))
                            {
                                if (data.dataName.Contains("运行库温度"))
                                {
                                    strTemperature = (double.Parse(data.dataValue) + 0.05) + "℃|" + data.dataValue + "℃|";
                                }
                                strTemper += data.dataName.Remove(data.dataName.IndexOf("温度")) + "|";
                            }
                            else if (data.dataName.Contains("湿度"))
                            {
                                if (data.dataName.Contains("运行库湿度"))
                                {
                                    strHumidity = (double.Parse(data.dataValue) + 0.05) + "%|" + data.dataValue + "%|";
                                }
                                strHumi += data.dataName.Remove(data.dataName.IndexOf("湿度")) + "|";
                            }
                            else if (data.dataName.Contains("红外"))
                            {
                                if (data.isAlarm)
                                {
                                    infraredAlarm++;
                                }
                                else
                                {
                                    infraredNoAlarm++;
                                }
                            }
                            else if (data.dataName.Contains("烟感"))
                            {
                                if (data.isAlarm)
                                {
                                    smokeAlarm++;
                                }
                                else
                                {
                                    smokeNoAlarm++;
                                }
                            }
                        }
                    }
                    this.Dispatcher.BeginInvoke(new Action(() => tbTemperature.Text = "实时温度:"+ strTemperature.TrimEnd('|')));
                    this.Dispatcher.BeginInvoke(new Action(() => tbHumidity.Text = "实时湿度:" + strHumidity.TrimEnd('|')));
                    this.Dispatcher.BeginInvoke(new Action(() => tbTepmer.Text = "温度区域:" + strTemper.TrimEnd('|')));
                    this.Dispatcher.BeginInvoke(new Action(() => tbHumi.Text = "湿度区域:" + strHumi.TrimEnd('|')));
                    this.Dispatcher.BeginInvoke(new Action(() => tbInfrared.Text = "正常:" + infraredNoAlarm + "/" + (infraredAlarm + infraredNoAlarm) + ",报警:" + infraredAlarm + "/" + (infraredAlarm + infraredNoAlarm)));
                    this.Dispatcher.BeginInvoke(new Action(() => tbSmokeDetector.Text = "正常:" + smokeNoAlarm + "/" + (smokeAlarm + smokeNoAlarm) + ",报警:" + smokeAlarm + "/" + (smokeAlarm + smokeNoAlarm)));
                    this.Dispatcher.BeginInvoke(new Action(() => tbEntranceGuard.Text = "正常:2/2,报警:0/2"));
                }
                else
                {
                    s_OutputParms = opResult.Message;
                }
            }
            catch (Exception ex)
            {
                s_OutputParms = OperateResult.ConvertException(ex);
            }
        }


    }
}
