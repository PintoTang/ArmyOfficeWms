using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.Infrastructrue.Xml;

namespace CL.WCS.ConfigManagerPckg
{
    public class ConfigHelper
    {
        private const string CoordinationConfigPath = "Config/CoordinationConfig.xml";
        private const string DeviceConfigPath = "Config/DeviceConfig.xml";

        private static XmlOperator _coordinationConfig;
        private static XmlOperator _deviceConfig;
        public static XmlOperator GetCoordinationConfig
        {
            get
            {
                if (_coordinationConfig==null)
                {
                    _coordinationConfig = new XmlOperator(CoordinationConfigPath);
                }
                return _coordinationConfig;
            }
        }

        public static XmlOperator GetDeviceConfig
        {
            get
            {
                if (_deviceConfig == null)
                {
                    _deviceConfig = new XmlOperator(DeviceConfigPath);
                }
                return _deviceConfig;
            }
        }

        public static void UpdateDeviceConfig()
        {
            _deviceConfig=new XmlOperator(DeviceConfigPath);
        }

        public static void UpdateCoordinationConfig()
        {
            _coordinationConfig=new XmlOperator(CoordinationConfigPath);
        }

    }
}
