using System;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CL.WCS.SystemConfigPckg.Model;
using CL.WCS.SystemConfigPckg.ViewModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CL.WCS.SystemConfigPckg.View
{
    /// <summary>
    /// SqliteConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class SqliteConfigView : UserControl, IDbConfiguration
    {
        //Data Source=D:\WcsGDZhanJiang01.db;Pooling=true;FailIfMissing=false
        public SqliteConfigView(object config)
        {
            InitializeComponent();
            InitilzieConfigValue(config);
        }

        private void InitilzieConfigValue(object config)
        {
            string configConnectString = config as string;
            if (!string.IsNullOrEmpty(configConnectString))
            {
                string[] dbConfigItem = configConnectString.Split(';');
                foreach (string configItem in dbConfigItem)
                {
                    string[] keyValue = configItem.Split('=');
                    if (keyValue[0].Trim().Contains("Data Source"))
                    {
                        TbHost.Text = keyValue[1].Trim();
                    }
                    else if (keyValue[0].Trim().Contains("Pooling"))
                    {
                        CbIsPooling.IsChecked = bool.Parse(keyValue[1].Trim());
                    }
                    else if (keyValue[0].Trim().Contains("FailIfMissing"))
                    {
                        CbIsFailIfMissing.IsChecked = bool.Parse(keyValue[1].Trim());
                    }
                }
                //解析连接串
            }
        }
        private bool IsPooling { get; set; }
        private bool IsFailIfMissing { get; set; }
        private string DbFilePath { get; set; }

        public string GetConnectionString()
        {
            if (CbIsPooling.IsChecked.HasValue)
            {
                IsPooling = CbIsPooling.IsChecked.Value;
            }

            if (CbIsFailIfMissing.IsChecked.HasValue)
            {
                IsFailIfMissing = CbIsFailIfMissing.IsChecked.Value;
            }
            string connectString = string.Format("Data Source={0};Pooling={1};FailIfMissing={2}", DbFilePath, IsPooling, IsFailIfMissing);
            return connectString;
        }

        private void BtnCheck_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string dbConnectionString = GetConnectionString();
                using (SQLiteConnection conn = new SQLiteConnection(dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    MessageBoxEx.Show("连接成功");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("连接失败 : " + ex.Message);
            }

        }

        private void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            MultiBindingExpression mbe = BindingOperations.GetMultiBindingExpression(this.Parent, ContentProperty);
            if (mbe == null)
            {
                MessageBoxEx.Show("更新失败");
                return;
            }
            if (mbe.BindingExpressions.Count <= 0)
            {
                MessageBoxEx.Show("更新失败");
                return;
            }
            BindingExpression bing = mbe.BindingExpressions[0] as BindingExpression;
            if (bing == null)
            {
                MessageBoxEx.Show("更新失败");
                return;
            }
            ConfigItem<string> bingSource = (ConfigItem<string>)bing.ResolvedSource;
            bingSource.Value = GetConnectionString();
            MessageBoxEx.Show("更新成功");
        }
    }
}
