using System.ComponentModel;
using System.Windows.Controls;
using CL.WCS.SystemConfigPckg.View;

namespace CL.WCS.SystemConfigPckg.Model
{
    public enum DatabaseTypeEnum
    {
        [Description("MySql")]
        MySql = 0,
        [Description("SqlServer")]
        SqlServer = 1,
        [Description("Sqlite")]
        Sqlite = 2,
        [Description("Oracle")]
        Oracle = 3,
        [Description("PostgreSQL")]
        PostgreSQL = 4,
        [Description("Dm")]
        Dm = 5,
        [Description("Kdbndp")]
        Kdbndp = 6
    }

    public static class DatabaseTypeEnumEx
    {
        public static UserControl GetConfigView(this DatabaseTypeEnum dbTypeEnum, object configValue)
        {
            if (dbTypeEnum == DatabaseTypeEnum.SqlServer)
            {
                return new SqlServerConfigView(configValue);
            }
            if (dbTypeEnum == DatabaseTypeEnum.Oracle)
            {
                return new OracleConfigView(configValue);
            }
            if (dbTypeEnum == DatabaseTypeEnum.Sqlite)
            {
                return new SqliteConfigView(configValue);
            }
            else
            {
                return new UserControl();
            }
        }
    }
}
