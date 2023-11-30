using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLDC.Framework.Log;
using System.Threading;
using CL.Framework.OPCClientAbsPckg;
using CL.Framework.CmdDataModelPckg;

namespace CL.Framework.OPCClientSimulatePckg
{

	public class OPCClientSimulate : OPCClientAbstract
	{

        private class OpcSimulateClientFactory : IOpcClientFactory
        {
            public OPCClientAbstract Create()
            {
                OPCClientSimulateAbstract opcClientSimulateAbstract = new OPCClientSimulateFactory();
                return  new OPCClientSimulate(opcClientSimulateAbstract);
            }
        }


		private Semaphore batchReadSem;
		private Semaphore batchReadBoolSem = new Semaphore(0, 1);
		private List<ProcHandler> ProcHandlerLst = null;
		OPCReadOrWriteAbstract opcReadOrWriteAbstract = null;
		private Dictionary<string, string> PLCConnectionDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public delegate void OPCReadBoardMtltiSim(List<DeviceAddressInfo> deviceInfoList);

		public override event OPCWriteBoardSingle OPCWriteBoardSingleEvent;
		public override event OPCWriteBoardMulti OPCWriteBoardMultiEvent;
		public override event OPCReadBoardSingle OPCReadBoardSingleEvent;
		public override event OPCReadBoardMtlti OPCReadBoardMultiEvent;

		public OPCClientSimulate(OPCClientSimulateAbstract opcClientSimulateAbstract)
		{
			ProcHandlerLst = new List<ProcHandler>();
			batchReadSem = new Semaphore(0, 1);
			opcReadOrWriteAbstract = opcClientSimulateAbstract.Make(this);
		}

		public OPCClientSimulate(Semaphore batchReadSem, List<ProcHandler> ProcHandlerLst)
		{
			this.batchReadSem = batchReadSem;
			this.ProcHandlerLst = ProcHandlerLst;
		}
		ManualResetEvent manualReset = new ManualResetEvent(true);
		int seconds = 0;
		public override void PauseOpcService()
		{
			seconds = -1;
			manualReset.Reset();
		}

		public override void RecoveryOpcService()
		{
			seconds = 0;
			manualReset.Set();
		}

	    public override IOpcClientFactory GetFactory()
	    {
            return  null;
	    }

	    public override void Write(string deviceName, string itemName, int value)
		{
			Write(deviceName, itemName, value.ToString());
		}

		public override void Write(string deviceName, string itemName, float value)
		{
			Write(deviceName, itemName, value.ToString());
		}

		public override void Write(string deviceName, string itemName, string value)
		{
			manualReset.WaitOne(seconds, true);
			StringBuilder msg = new StringBuilder();
			string opcItemName = CreateItem(deviceName, itemName);
			msg.Append("接收到单独写指令(Write):\r\n");
			msg.AppendFormat("groupName[{0}]", deviceName);
			msg.AppendFormat(",itemName[{0}]", opcItemName);
			msg.AppendFormat(",value[{0}]", value);
			Log.getEventFile().Info(msg.ToString());
			if (itemName.Contains("*"))
				itemName = itemName.Remove(itemName.IndexOf("*"), 1);
			ShowOPCWriteValueSingleToBoard(deviceName, opcItemName, value);
		}

		public float btnFloatValue = 0.0f;
		public override float ReadFloat(string deviceName, string itemName)
		{
			string itemNameReal = string.Empty;
			if (itemName.Contains("@"))
			{
				itemNameReal = itemName.Split('@')[1];
			}
			else
			{
				itemNameReal = itemName;
			}
			manualReset.WaitOne(seconds, true);
			ProcHandler pro = new ProcHandler();
			pro.itemName = itemName;
			pro.deviceName = deviceName;
			pro.readSem = new Semaphore(0, 1);
			string itemConnection = GetOpcConnection(deviceName, itemName);
			lock (ProcHandlerLst)
			{
				ProcHandlerLst.Add(pro);
				opcReadOrWriteAbstract.OPC_PassValue(deviceName, itemConnection, itemNameReal, "FLOAT");
			}
			pro.readSem.WaitOne();
			SaveReadLog(deviceName, itemName, btnFloatValue.ToString());
			return btnFloatValue;
		}

		public string btnStringValue = "";
		public override string ReadString(string deviceName, string itemName)
		{
			string itemNameReal = string.Empty;
			if (itemName.Contains("@"))
			{
				itemNameReal = itemName.Split('@')[1];
			}
			else
			{
				itemNameReal = itemName;
			}
			manualReset.WaitOne(seconds, true);
			ProcHandler pro = new ProcHandler();
			pro.itemName = itemName;
			pro.deviceName = deviceName;
			pro.readSem = new Semaphore(0, 1);
			lock (ProcHandlerLst)
			{
				ProcHandlerLst.Add(pro);
				opcReadOrWriteAbstract.OPC_PassValue(deviceName, GetOpcConnection(deviceName, itemName), itemNameReal, "String");
			}
			pro.readSem.WaitOne();
			SaveReadLog(deviceName, itemName, btnStringValue);
			return btnStringValue;
		}

		public int btnvalue = -1;
		public override int Read(string deviceName, string itemName)
		{
			string itemNameReal = string.Empty;
			if (itemName.Contains("@"))
			{
				itemNameReal = itemName.Split('@')[1];
			}
			else
			{
				itemNameReal = itemName;
			}
			manualReset.WaitOne(seconds, true);
			ProcHandler pro = new ProcHandler();
			pro.itemName = itemName;
			pro.deviceName = deviceName;
			pro.readSem = new Semaphore(0, 1);
			string itemConnection = GetOpcConnection(deviceName, itemName);
			lock (ProcHandlerLst)
			{
				ProcHandlerLst.Add(pro);
				opcReadOrWriteAbstract.OPC_PassValue(deviceName, itemConnection, itemNameReal, "INT");
			}
			pro.readSem.WaitOne();
			SaveReadLog(deviceName, itemName, btnvalue.ToString());
			ShowOPCReadValueSingleToBoard(deviceName, itemNameReal, "1");
			return btnvalue;
		}

		private void SaveReadLog(string deviceName, string itemName, string result)
		{
			StringBuilder msg = new StringBuilder();
			string opcItemName = CreateItem(deviceName, itemName);
			msg.Append("接收到单独读指令(Read):\r\n");
			msg.AppendFormat("groupName[{0}]", deviceName);
			msg.AppendFormat(",itemName[{0}]", opcItemName);
			msg.AppendFormat("。执行完成的返回值为：{0}", result);
			Log.getEventFile().Info(msg.ToString());
		}

		public void OPCReadOrWrite_ResultEvent(int index, string values, string readType)
		{
			if ("INT" == readType)
			{
				int temp = 0;
				if (int.TryParse(values, out temp) == false)
				{
					btnvalue = -0x7FFFFFFF;
				}
				else
				{
					btnvalue = temp;
				}
			}
			else if ("String" == readType)
			{
				btnStringValue = values;
			}
			else if ("Bool" == readType)
			{
				bool temp = false;
				if (!bool.TryParse(values, out temp))
					return;
				btnBoolValue = temp;
			}
			else if ("FLOAT" == readType)
			{
				float floatTemp = 0;
				if (float.TryParse(values, out floatTemp) == false)
				{
					btnFloatValue = 0.0f;
				}
				else
				{
					btnFloatValue = floatTemp;
				}
			}
			else
			{
				return;
			}

			lock (ProcHandlerLst)
			{
				ProcHandlerLst[index].readSem.Release();
				ProcHandlerLst.RemoveAt(index);
			}
		}

		public void OPCReadOrWriteList_ResultEvent(List<int> values)
		{
			btnvalue3 = values;
			batchReadSem.Release();
		}

		public override void Write(string deviceName, Dictionary<string, int> Inv)
		{
			manualReset.WaitOne(seconds, true);
			StringBuilder msg = new StringBuilder();
			msg.Append("接收到批量写指令(Write):\r\n");
			msg.AppendFormat("groupName[{0}]", deviceName);
			foreach (var list in Inv)
			{
				string opcItemName = CreateItem(deviceName, list.Key);
				msg.AppendFormat(",key[{0}]", opcItemName);
				msg.AppendFormat(",value[{0}]\r\n", list.Value);
			}
			Log.getEventFile().Info(msg.ToString());
			Dictionary<string, object> tmp = new Dictionary<string, object>();
			foreach (var item in Inv)
			{
				string opcItemName = CreateItem(deviceName, item.Key.ToString());
				tmp.Add(opcItemName, item.Value.ToString());
			}
			ShowOPCWriteValueMultiToBoard(deviceName, tmp);
		}

		public List<int> btnvalue3 = null;
		public override List<int> Read(List<DeviceAddressInfo> deviceInfoList)
		{
			List<DeviceAddressInfo> deviceInforListReal = new List<DeviceAddressInfo>();
			manualReset.WaitOne(seconds, true);
			lock (deviceInfoList)
			{
				string deviceName = "deviceName";
				List<string> deviceInfoListClone = CloneDeviceInfoList(deviceInfoList);
				foreach (var deviceInfo in deviceInfoList)
				{
					deviceInfo.deviceName = deviceInfo.deviceName + "@" + GetOpcConnection(deviceInfo.deviceName, deviceInfo.Datablock.RealDataBlockAddr);
					DeviceAddressInfo deviceAddressInfroReal = new DeviceAddressInfo();
					if (deviceInfo.Datablock.RealDataBlockAddr.Contains("@"))
					{
						string itemNameReal = deviceInfo.Datablock.RealDataBlockAddr.Split('@')[1];
						deviceAddressInfroReal.deviceName = deviceInfo.deviceName;
						deviceAddressInfroReal.Datablock.RealDataBlockAddr = itemNameReal;
						deviceInforListReal.Add(deviceAddressInfroReal);
					}
					else
					{
						deviceInforListReal.Add(deviceInfo);
					}
				}
				opcReadOrWriteAbstract.OPC_PassValueList(deviceName, deviceInforListReal);
				batchReadSem.WaitOne();
				StringBuilder msg = new StringBuilder();
				msg.Append("接收到批量读指令(Read):\r\n");
				for (int i = 0; i < deviceInfoList.Count; i++)
				{
					DeviceAddressInfo device = deviceInfoList[i] as DeviceAddressInfo;
					string opcItemName = CreateItem(device.deviceName.Split('@')[0], device.Datablock.RealDataBlockAddr);
					msg.AppendFormat("groupName[{0}]", device.deviceName.Split('@')[0]);
					msg.AppendFormat(",itemName[{0}]", opcItemName);
					if (btnvalue3 != null)
					{
						if (btnvalue3.Count > i)
						{
							msg.AppendFormat(",执行完成的返回值为:{0};\r\n", btnvalue3[i].ToString());
						}
					}
					else
					{
						msg.AppendFormat(",无返回值;");
					}
				}
				Log.getEventFile().Info(msg.ToString());

				ShowOPCReadValueMultiToBoard(deviceInfoListClone, btnvalue3);
				return btnvalue3;
			}
		}

		/// <summary>
		/// 将写入OPC的设备名称和地址信息,格式List<DeviceAddressInfo>转换为List<string>
		/// </summary>
		/// <param name="deviceInfoList"></param>
		/// <returns></returns>
		private List<string> CloneDeviceInfoList(List<DeviceAddressInfo> deviceInfoList)
		{
			List<string> DeviceAddressInfoListClone = new List<string>();
			foreach (var deviceInfoItem in deviceInfoList)
			{
				string itemNameReal = string.Empty;
				if (deviceInfoItem.Datablock.RealDataBlockAddr.Contains("@"))
				{
					itemNameReal = deviceInfoItem.Datablock.RealDataBlockAddr.Split('@')[1];
				}
				else
				{
					itemNameReal = deviceInfoItem.Datablock.RealDataBlockAddr;
				}
				string item = deviceInfoItem.deviceName + "|" + GetOpcConnection(deviceInfoItem.deviceName, deviceInfoItem.Datablock.RealDataBlockAddr) + itemNameReal;
				DeviceAddressInfoListClone.Add(item);
			}
			return DeviceAddressInfoListClone;
		}

		/// <summary>
		/// 获取组名对应的值
		/// </summary>
		/// <param name="groupName">组名</param>
		/// <returns>对应键的值</returns>
		public string GetOpcConnection(string groupName, string itemName)
		{
			try
			{
				if (itemName.Contains("@"))
				{
					Addr realOpcAddr = new Addr(itemName.Split('@')[0]);
					string key = PLCConnectionDictionary.Keys.FirstOrDefault(p => p.Contains(":") && new Addr(p).IsContain(realOpcAddr));
					if (!string.IsNullOrEmpty(key))
					{
						return PLCConnectionDictionary[key];
					}
				}
				if (PLCConnectionDictionary.Keys.Contains(groupName, StringComparer.OrdinalIgnoreCase))
				{
					return PLCConnectionDictionary[groupName];
				}
				if (PLCConnectionDictionary.Keys.Contains(groupName.Split('#')[0], StringComparer.OrdinalIgnoreCase))
				{
					return PLCConnectionDictionary[groupName.Split('#')[0]];
				}
				if (PLCConnectionDictionary.Keys.Contains(groupName.Split(':')[0], StringComparer.OrdinalIgnoreCase))
				{
					return PLCConnectionDictionary[groupName.Split(':')[0]];
				}
				return "";
			}
			catch
			{
				return "";
			}
		}

		/// <summary>
		/// 判断传入的协议名称是否已加前缀处理
		/// </summary>
		/// <param name="itemName">协议名称</param>
		/// <returns>是否已拼接处理</returns>
		private bool IsOpcConnection(string itemName)
		{
			bool IsConnection = false;
			try
			{
				IsConnection = itemName.Contains("S7:[S7 connection_");
			}
			catch
			{
				return false;
			}
			return IsConnection;
		}

		/// <summary>
		///将协议名称加上前缀
		/// </summary>
		/// <param name="deviceName">设备名称</param>
		/// <param name="itemName">协议名称</param>
		/// <returns></returns>
		public string CreateItem(string deviceName, string itemName)
		{
			string itemNameReal = string.Empty;
			if (IsOpcConnection(itemName) == false)
			{
				string opcConnection = GetOpcConnection(deviceName, itemName);
				if (itemName.Contains("@"))
				{
					itemNameReal = itemName.Split('@')[1];
				}
				else
				{
					itemNameReal = itemName;
				}
				itemNameReal = opcConnection + OpcConnectionReplace(itemNameReal);
			}
			else
			{
				itemNameReal = OpcConnectionReplace(itemName);
			}
			return itemNameReal;
		}

		/// <summary>
		/// 替换协议名称DBD转INT处理
		/// </summary>
		/// <param name="itemName">协议名称</param>
		/// <returns>是否已拼接处理</returns>
		private string OpcConnectionReplace(string itemName)
		{
			if (itemName.StartsWith("Q") || itemName.StartsWith("M"))
			{
				return itemName;
			}
			string itemPrefix = itemName.Substring(itemName.IndexOf(',', 1) + 1, 3);
			switch (itemPrefix)
			{
				case "DBW":
					return itemName.Replace(itemPrefix, "INT");
				case "INT":
					return itemName;
				case "DBD":
					return itemName.Replace(itemPrefix, "DINT");
				case "DIN":
					return itemName;
				case "DBX":
					return itemName.Replace(itemPrefix, "STRING");
				case "STR":
					return itemName;
				case "REA":
					return itemName;
				default:
					if (itemPrefix.StartsWith("X"))
						return itemName;
					throw new Exception("请检查传入协议地址" + itemName + "是否符合协议规范！");
			}
		}

		/// <summary>
		/// 将传给opc的单个写入的信息值，显示到OPC观测界面
		/// </summary>		
		private void ShowOPCWriteValueSingleToBoard(string deviceName, string opcWriteItemName, string opcWriteItemValue)
		{
			try
			{
				if (OPCWriteBoardSingleEvent != null)
				{
					OPCWriteBoardSingleEvent(deviceName, opcWriteItemName, opcWriteItemValue.ToString());
				}
			}
			catch (Exception ex)
			{
				CLDC.Framework.Log.Log.getExceptionFile().Info("OPCWrite观测界面单个写异常：" + ex.Message);
			}
		}

		/// <summary>
		/// 将传给opc的多个写入的信息值，显示到OPC观测界面
		/// </summary>		
		private void ShowOPCWriteValueMultiToBoard(string deviceName, Dictionary<string, object> opcWriteItemValueDic)
		{
			try
			{
				if (OPCWriteBoardMultiEvent != null)
				{
					OPCWriteBoardMultiEvent(deviceName, opcWriteItemValueDic);
				}
			}
			catch (Exception ex)
			{
				CLDC.Framework.Log.Log.getExceptionFile().Info("OPCWrite观测界面多个写异常：" + ex.Message);
			}
		}

		/// <summary>
		/// 将传给opc的单个读取的信息值，显示到观测界面
		/// </summary>		
		private void ShowOPCReadValueSingleToBoard(string deviceName, string opcReadAddr, string opcValue)
		{
			try
			{
				if (OPCReadBoardSingleEvent != null)
				{
					OPCReadBoardSingleEvent(deviceName, opcReadAddr, opcValue);
				}
			}
			catch (Exception ex)
			{
				CLDC.Framework.Log.Log.getExceptionFile().Info("OPC读写观测界面异常：单个读异常，原因：" + ex.Message);
			}
		}

		/// <summary>
		/// 将传给opc的多个读取的信息值，显示到观测界面
		/// </summary>		
		private void ShowOPCReadValueMultiToBoard(List<string> opcReadAddrList, List<int> opcReturnValue)
		{
			try
			{
				if (OPCReadBoardMultiEvent != null)
				{
					OPCReadBoardMultiEvent(opcReadAddrList, opcReturnValue);
				}
			}
			catch (Exception ex)
			{
				Log.getExceptionFile().Info("OPC读写观测界面异常：多个读异常，原因：" + ex.Message);
			}
		}

		public bool btnBoolValue = false;
		public override bool ReadBool(string deviceName, string itemName)
		{
			manualReset.WaitOne(seconds, true);
			ProcHandler pro = new ProcHandler();
			pro.itemName = itemName;
			pro.deviceName = deviceName;
			pro.readSem = new Semaphore(0, 1);
			lock (ProcHandlerLst)
			{
				ProcHandlerLst.Add(pro);
				opcReadOrWriteAbstract.OPC_PassValue(deviceName, GetOpcConnection(deviceName, itemName), itemName, "Bool");
			}
			pro.readSem.WaitOne();
			SaveReadLog(deviceName, itemName, btnBoolValue.ToString());
			return btnBoolValue;
		}

		public override void Write(string deviceName, string itemName, bool value)
		{
			Write(deviceName, itemName, value.ToString());
		}

		public List<bool> btnvalue4 = null;
		public override List<bool> ReadBoolList(List<DeviceAddressInfo> deviceInfoList)
		{
			List<DeviceAddressInfo> deviceInforListReal = new List<DeviceAddressInfo>();
			manualReset.WaitOne(seconds, true);
			lock (deviceInfoList)
			{
				string deviceName = "deviceName";
				List<string> deviceInfoListClone = CloneDeviceInfoList(deviceInfoList);
				foreach (var deviceInfo in deviceInfoList)
				{
					deviceInfo.deviceName = deviceInfo.deviceName + "@" + GetOpcConnection(deviceInfo.deviceName, deviceInfo.Datablock.RealDataBlockAddr);
					DeviceAddressInfo deviceAddressInfroReal = null;
					if (deviceInfo.Datablock.RealDataBlockAddr.Contains("@"))
					{
						string itemNameReal = deviceInfo.Datablock.RealDataBlockAddr.Split('@')[1];
						deviceAddressInfroReal.deviceName = deviceInfo.deviceName;
						deviceAddressInfroReal.Datablock.RealDataBlockAddr = itemNameReal;
						deviceInforListReal.Add(deviceInfo);
					}
					else
					{
						deviceInforListReal.Add(deviceInfo);
					}
				}
				opcReadOrWriteAbstract.OPC_PassValueList("ResultEventStringList", deviceName, deviceInforListReal);
				batchReadBoolSem.WaitOne();
				StringBuilder msg = new StringBuilder();
				msg.Append("接收到批量读指令(Read):\r\n");
				for (int i = 0; i < deviceInfoList.Count; i++)
				{
					DeviceAddressInfo device = deviceInfoList[i] as DeviceAddressInfo;
					string opcItemName = CreateItem(device.deviceName.Split('@')[0], device.Datablock.RealDataBlockAddr);
					msg.AppendFormat("groupName[{0}]", device.deviceName.Split('@')[0]);
					msg.AppendFormat(",itemName[{0}]", opcItemName);
					if (btnvalue4 != null && btnvalue4.Count > i)
					{
						msg.AppendFormat(",执行完成的返回值为:{0};\r\n", btnvalue4[i].ToString());
					}
					else
					{
						msg.AppendFormat(",无返回值;");
					}
				}
				Log.getEventFile().Info(msg.ToString());
				return btnvalue4;
			}
		}

		public void OPCReadOrWriteList_ResultEventStringList(string flag, List<string> values)
		{
			ReadBoolhandle(values);
		}

		private void ReadBoolhandle(List<string> values)
		{
			List<bool> list = new List<bool>();
			bool a = false;
			foreach (string item in values)
			{
				if (bool.TryParse(item, out a) == false)
					return;
				list.Add(a);
			}
			btnvalue4 = list;
			batchReadBoolSem.Release();
		}

		public override void Write(string deviceName, Dictionary<string, object> itemValueDic)
		{
			manualReset.WaitOne(seconds, true);
			StringBuilder msg = new StringBuilder();
			msg.Append("接收到批量写指令(Write):\r\n");
			msg.AppendFormat("groupName[{0}]", deviceName);
			foreach (var list in itemValueDic)
			{
				string opcItemName = CreateItem(deviceName, list.Key);
				msg.AppendFormat(",key[{0}]", opcItemName);
				msg.AppendFormat(",value[{0}]\r\n", list.Value);
			}
			Log.getEventFile().Info(msg.ToString());
			Dictionary<string, object> tmp = new Dictionary<string, object>();
			foreach (var item in itemValueDic)
			{
				string opcItemName = CreateItem(deviceName, item.Key.ToString());
				tmp.Add(opcItemName, item.Value.ToString());
			}
			ShowOPCWriteValueMultiToBoard(deviceName, tmp);
		}

        public override void Write(string deviceName, string itemName, object value)
        {
            manualReset.WaitOne(seconds, true);
            StringBuilder msg = new StringBuilder();
            string opcItemName = CreateItem(deviceName, itemName);
            msg.Append("接收到单独写指令(Write):\r\n");
            msg.AppendFormat("groupName[{0}]", deviceName);
            msg.AppendFormat(",itemName[{0}]", opcItemName);
            msg.AppendFormat(",value[{0}]", value);
            Log.getEventFile().Info(msg.ToString());
            if (itemName.Contains("*"))
                itemName = itemName.Remove(itemName.IndexOf("*"), 1);
            ShowOPCWriteValueSingleToBoard(deviceName, opcItemName, value.ToString());
        }
    }
	public class ProcHandler
	{
		public string deviceName;
		public string itemName;
		public Semaphore readSem;
	}

}