using System.IO;
using System.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common
{
    public class DeviceConfigManage
    {

        public string xmlFullPath = Directory.GetCurrentDirectory() + @"\Config\DeviceConfig.xml";

        public  XmlDocument xmlDoc { get; set; }
        private static DeviceConfigManage deviceConfigManage;
        public static DeviceConfigManage Instance
        {
            get
            {
                if (deviceConfigManage == null)
                {
                    deviceConfigManage = new DeviceConfigManage();
                }
                return deviceConfigManage;
            }
        }
        public DeviceConfigManage()
        {
            loadXml();
        }
        private void loadXml()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlReader xmlReader = XmlReader.Create(xmlFullPath, new XmlReaderSettings { IgnoreComments = true });
            xDoc.Load(xmlReader);
            xmlReader.Close();
            xmlDoc = xDoc;
        }
    }
}
