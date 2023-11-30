using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.Infrastructrue.UserCtrl;

namespace CL.WCS.SystemConfigPckg.View
{
    /// <summary>
    /// SqlServerConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class SqlServerConfigView : UserControl, IDbConfiguration
    {
        public SqlServerConfigView(object config)
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
                    else if (keyValue[0].Trim().Contains("Initial Catalog"))
                    {
                        TbDbName.Text = keyValue[1].Trim();
                    }
                    else if (keyValue[0].Trim().Contains("User Id"))
                    {
                        TbUserName.Text = keyValue[1].Trim();
                    }
                    else if (keyValue[0].Trim().Contains("Password"))
                    {
                        PbContent.Password = keyValue[1].Trim();
                    }
                }
                //解析连接串
            }
        }
        private string Password { get; set; }
        private string UserName { get; set; }
        private string Host { get; set; }
        private string DbName { get; set; }
        public string GetConnectionString()
        {
            Password = PbContent.Password;
            UserName = TbUserName.Text;
            Host = TbHost.Text;
            DbName = TbDbName.Text;
            string connectString = string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3}", Host,
                DbName, UserName, Password);
            return connectString;
        }

        private void BtnCheck_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string dbConnectionString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(dbConnectionString))
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
