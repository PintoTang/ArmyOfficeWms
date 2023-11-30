using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// DevicePropertyItem
    /// </summary>
	public class DevicePropertyItem
	{
        /// <summary>
        /// 设备名
        /// </summary>
		public string name
		{
			get;
			set;
		}

        /// <summary>
        /// 出库站台
        /// </summary>
		public string outSrcAddrPrefix
		{
			get;
			set;
		}
        /// <summary>
        /// 入库站台
        /// </summary>
		public string inSrcAddrPrefix
		{
			get;
			set;
		}
        /// <summary>
        /// 移库站台
        /// </summary>
		public string moveSrcAddrPrefix
		{
			get;
			set;
		}

        /// <summary>
        /// 滚筒线出口业务类
        /// </summary>
		public object exportBusiness
		{
			get;
			set;
		}
        /// <summary>
        /// 扫描枪
        /// </summary>
		public string scanner
		{
			get;
			set;
		}

        /// <summary>
        /// 扫描枪组
        /// </summary>
        public string scannerGroup
        {
            get;
            set;
        }
        /// <summary>
        /// 地址
        /// </summary>
		public string addr
		{
			get;
			set;
		}

        /// <summary>
        /// 关键站台
        /// </summary>
		public string positiveStationCode
		{
			get;
			set;
		}
        /// <summary>
        /// 读取单个值
        /// </summary>
		public string OPCSignalValueMode
		{
			get;
			set;
		}
        /// <summary>
        /// 协议类型
        /// </summary>
		public string protocolType
		{
			get;
			set;
		}

        /// <summary>
        /// 是否双深位
        /// </summary>
		public string isDoubleCell
		{
			get;
			set;
		}

		/// <summary>
		/// 策略类型
		/// </summary>
		public string StrategyType
		{
			get;
			set;
		}

		/// <summary>
		/// 下一步地址是否为目的地址
		/// </summary>
		public string nextIsFinish
		{
			get;
			set;
		}

		/// <summary>
		/// 服务端IP
		/// </summary>
		public string socketServiceIP
		{
			get;
			set;
		}

		/// <summary>
		/// 服务端端口
		/// </summary>
		public string socketServicePort
		{
			get;
			set;
		}

		/// <summary>
		/// 服务端最多可连数量
		/// </summary>
		public string numConnections
		{
			get;
			set;
		}

		/// <summary>
		/// 服务端最多缓存大小
		/// </summary>
		public string receiveBufferSize
		{
			get;
			set;
		}

		/// <summary>
		/// 空托盘满托数量
		/// </summary>
		public string trayFullCount
		{
			get;
			set;
		}

		/// <summary>
		/// 上位系统
		/// </summary>
		public string epistatic
		{
			get;
			set;
		}

		/// <summary>
		/// 空托盘类型
		/// </summary>
		public string trayType
		{
			get;
			set;
		}

		/// <summary>
		/// 空托盘分配对象
		/// </summary>
		public string distributeTrayObject
		{
			get;
			set;
		}

		/// <summary>
		/// 预留字段
		/// </summary>
		public string reverseField
		{
			get;
			set;
		}

		/// <summary>
		/// 客户端IP
		/// </summary>
		public string socketClientIP
		{
			get;
			set;
		}

		/// <summary>
		/// 客户端端口
		/// </summary>
		public string socketClientRemotePort
		{
			get;
			set;
		}

		/// <summary>
		/// 客户端本地端口
		/// </summary>
		public string socketClientLocalPort
		{
			get;
			set;
		}

		public string deviceControl
		{
			get;
			set;
		}

		public string functionType
		{
			get;
			set;
		}

		/// <summary>
		/// 转换成json格式
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static explicit operator DevicePropertyItem(string json)
		{
			return JsonConvert.DeserializeObject<DevicePropertyItem>(json);
		}
	}
    /// <summary>
    /// 
    /// </summary>
	public class ExportBusiness
	{
        /// <summary>
        /// 类型
        /// </summary>
		public string type
		{
			get;
			set;
		}

        /// <summary>
        /// 数量
        /// </summary>
		public string number
		{
			get;
			set;
		}

        /// <summary>
        /// 扫描枪
        /// </summary>
		public string scanner
		{
			get;
			set;
		}
        /// <summary>
        /// isReverse
        /// </summary>
		public string isReverse
		{
			get;
			set;
		}

		private string strategyType = "AllocationStrategyForStationGroup";
		public string StrategyType
		{
			get { return strategyType; }
			set { strategyType = value; }
		}
        /// <summary>
        /// 转换成json格式
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
		public static explicit operator ExportBusiness(string json)
		{
			return JsonConvert.DeserializeObject<ExportBusiness>(json);
		}
	}
}
