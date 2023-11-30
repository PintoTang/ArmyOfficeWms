using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    /// <summary>
    /// 滚筒线 设备信息对象类
    /// </summary>
    public class DeviceInfoConfig
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType;
        /// <summary>
        /// 设备包 命名空间
        /// </summary>
        public string DeviceNameSpace;
        /// <summary>
        /// 设备组 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 设备集合
        /// </summary>
        public List<Devices> DeviceLst;
    }
    /// <summary>
    /// 设备对象类
    /// </summary>
    public class Devices
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId;
        /// <summary>
        /// 设备节点名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 设备类
        /// </summary>
        public string Class;
        /// <summary>
        /// 设备包 命名空间
        /// </summary>
        public string NameSpace;
        /// <summary>
        /// 设备类名 
        /// </summary>
        public string DeviceName;
        /// <summary>
        /// 当前地址
        /// </summary>
        public string CurAddress;
        /// <summary>
        /// 
        /// </summary>
        public int  WorkSize;

        /// <summary>
        /// 是否需要显示Ui
        /// </summary>
        public bool IsShowUi;

        /// <summary>
        /// 设备业务对象类
        /// </summary>
        public DeviceBusinessHandle BusinessHandle;

        /// <summary>
        /// 设备控制Handle
        /// </summary>
        public DeviceControlHandle DeviceControlHandle;
       
    }
    /// <summary>
    /// 设备控制Handle
    /// </summary>
    public class DeviceControlHandle
    {
        public string Name;
        public string Class;
        public string NameSpace;
        public string Type;

         /// <summary>
        /// 设备协议对象类
        /// </summary>
        public DeviceProtocolTranslation ProtocolTranslation;
        /// <summary>
        /// 设备通讯协议类
        /// </summary>
        public DeviceCommunication Communication;
    }

    /// <summary>
    /// 设备业务类
    /// </summary>
    public class DeviceBusinessHandle
    {
        public string Name;
        public string Class;
        public string NameSpace;
        public string Type;

        public DeviceBugConfig DeviceConfig;
    }
    /// <summary>
    /// 设备配置类
    /// </summary>
    public class DeviceBugConfig
    {
        /// <summary>
        /// 配置类型
        /// </summary>
        public string ConfigType;
        /// <summary>
        ///// 是否检测就绪
        ///// </summary>
        //public bool IsCheckReady;
        ///// <summary>
        ///// 是否转换下一个地址
        ///// </summary>
        //public bool IsNeedTransportToNextAddr;
    }
    /// <summary>
    /// 设备协议对象类
    /// </summary>
    public class DeviceProtocolTranslation
    {
        //public string Name;
        public string Class;
        public string NameSpace;
        public string Type;
        public DeviceProtocolConfig DeviceProtocolConfig;
        
    }
    public class DeviceProtocolConfig
    {
        public string ConfigType;
    }
  
   
   
    /// <summary>
    /// 设备通讯类
    /// </summary>
    public class DeviceCommunication
    {
        public string Name;
        public string Class;
        public string NameSpace;
        public string Type;
        public DeviceCommConfig DeviceCommConfig;
       
    }
    /// <summary>
    /// 设备类型 (Config)类
    /// </summary>
    public class DeviceCommConfig
    {
        /// <summary>
        /// 配置类型
        /// </summary>
        public string ConfigType;
        /// <summary>
        ///  设备连接 通讯协议地址 类
        /// </summary>
        public DeviceCommConfigConn DeviceCommConfigConn;
        public DeviceConfigDataBlockItems DeviceConfigDataBlockItems;
    }
    /// <summary>
    /// 设备连接 通讯协议地址 类
    /// </summary>
    public class DeviceCommConfigConn
    {
        /// <summary>
        /// OPC conn
        /// </summary>
        public string Connection;
    }
    /// <summary>
    /// 设备 配置数据DataBlockItems 类
    /// </summary>
    public class DeviceConfigDataBlockItems
    {
        public string Template;
        public DeviceDataBlockItems DeviceDataBlockItemsLst;
    }
    /// <summary>
    /// 设备 配置数据DataBlockItems 类
    /// </summary>
    public class DeviceDataBlockItems
    {
        //public string Name;
        public List<DataBlock> DataBlockLst;
    }
    /// <summary>
    /// 协议数据
    /// </summary>
    public class DataBlock
    {
        public string Name;
        public string DataBlockName;
        public string realDataBlockAddr;

    }
}
