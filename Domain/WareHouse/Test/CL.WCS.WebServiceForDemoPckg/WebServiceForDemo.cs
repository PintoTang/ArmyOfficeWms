using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.WebServiceAbstractPckg;
using CLDC.Framework.WCS.ConfigManager;
using CL.WCS.NotifyOffShelveFinishAbstractPckg;
using CL.WCS.NotifyOrderFinishAbstractPckg;
using CL.WCS.NotifyRouteStateAbstractPckg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataBaseOperationAbstractPckg;
using CL.Framework.CmdHandlerAbstractPckg;

namespace CL.WCS.WebServiceForDemoPckg
{
	public class WebServiceForDemo : WebServiceAbstract, NotifyRouteStateAbstract, NotifyOrderFinishAbstract, NotifyOffShelveFinishAbstract
	{
		public bool RouteStateUpdate(string[] routeIds, string routeStatus)
		{
			return true;
		}
		private BaseDataAbstract baseData;
		//记录该托盘号所对应货物分到的巷道号，便于分配仓位
		private Dictionary<string, string> trayIdAndTunnelIdDictionary = new Dictionary<string, string>();

		//记录该托盘号所对应货物分到的仓位号
		private Dictionary<string, string> trayIdAndCellCodeDictionary = new Dictionary<string, string>();

		public WebServiceForDemo(BaseDataAbstract _baseData)
		{
			this.baseData = _baseData;
		}
		public Item RequireAllocateTunnelByMaterial(Item item)
		{
			if (string.IsNullOrEmpty(item.WarehouseId) || string.IsNullOrEmpty(item.HighFlag) ||
					string.IsNullOrEmpty(item.TrayId) || string.IsNullOrEmpty(item.StationCode))
			{
				ConfigManageHelper.WriteLog("DemoException", "WebServiceForDemo收到RequireAllocateTunnelByMaterial方法传参异常：" +
					string.Format("WARE_ID='{0}',STA_ID='{1}',SUB_TRAY_ID='{2}',TRAY_ID='{3}',HIGH_FLAG='{4}'",
					string.IsNullOrEmpty(item.WarehouseId) ? "null" : item.WarehouseId,
					string.IsNullOrEmpty(item.StationCode) ? "null" : item.StationCode,
					string.IsNullOrEmpty(item.SubTrayId) ? "null" : item.SubTrayId,
					string.IsNullOrEmpty(item.TrayId) ? "null" : item.TrayId,
					string.IsNullOrEmpty(item.HighFlag) ? "null" : item.HighFlag));
				throw new Exception();
			}
			Item resultItem = new Item();
			resultItem.WarehouseId = item.WarehouseId;
			resultItem.TrayId = item.TrayId;
			resultItem.IsAllowIn = true;
			resultItem.LedMessage = "无返回内容";
			resultItem.TaskId = "Demo01";
			switch (item.WarehouseId)
			{
				case "E07":
					if (item.StationCode == "Entrance:1_1_1_S" || item.StationCode =="InAndOutStation:2_1_1_S"
						|| item.StationCode == "PickStation:6_2" || item.StationCode == "PickStation:7_2")
					{
						resultItem.TunnelId = "Tunnel:1_S_E07";
						resultItem.Area = "S";
					}
					else
					{
						string[] areaAry = { "Tunnel:2_S", "Tunnel:3_S", "Tunnel:4_S",
										 "Tunnel:1_N", "Tunnel:2_N", "Tunnel:3_N", "Tunnel:4_N" ,"Tunnel:5_N"};
						Random tunnelRandom = new Random();
						resultItem.TunnelId = areaAry[tunnelRandom.Next(0, 8)] + "_E07";
						resultItem.Area = resultItem.TunnelId.Split('_')[1];
					}
					break;

				case "E06":
					string[] tunnelRandom2 = { "Tunnel:1_M", "Tunnel:2_M", "Tunnel:3_M", "Tunnel:4_M" };
					Random r2 = new Random();
					resultItem.TunnelId = tunnelRandom2[r2.Next(0, 4)] + "_E06";
					resultItem.Area = "M";
					break;

				default:
					break;
			}
			if (!trayIdAndTunnelIdDictionary.ContainsKey(resultItem.TrayId))
			{
				trayIdAndTunnelIdDictionary.Add(resultItem.TrayId, resultItem.TunnelId);
			}
			else
			{
				ConfigManageHelper.WriteLog("DemoException", string.Format("WebServiceForDemo的RequireAllocateTunnelByMaterial接口收到调用，但是该托盘号：{0}已经请求过巷道分配了", resultItem.TrayId));
				trayIdAndTunnelIdDictionary[resultItem.TrayId] = resultItem.TunnelId;
			}
			ConfigManageHelper.WriteLog("DemoLog", string.Format("WebServiceForDemo的RequireAllocateTunnelByMaterial接口被调用，返回的内容为：IsAllowIn = {0}，LedMessage = {1}，TaskId = {2}，TunnelId = {3}",
				resultItem.IsAllowIn, resultItem.LedMessage, resultItem.TaskId, resultItem.TunnelId));
			return resultItem;
		}

		public Item RequireAllocateCell(string WarehouseId, string TrayId, string TaskId)
		{
			if (string.IsNullOrEmpty(WarehouseId) || string.IsNullOrEmpty(TaskId) ||
				   string.IsNullOrEmpty(TrayId))
			{
				ConfigManageHelper.WriteLog("DemoException", "WebServiceForDemo收到RequireAllocateCell方法传参异常，" +
					string.Format("WARE_ID='{0}',TRAY_ID='{1}',MIS_ID='{2}'",
					string.IsNullOrEmpty(WarehouseId) ? "null" : WarehouseId,
					string.IsNullOrEmpty(TrayId) ? "null" : TrayId,
					string.IsNullOrEmpty(TaskId) ? "null" : TaskId));
				throw new Exception();
			}
			Item resultItem = new Item();
			resultItem.WarehouseId = WarehouseId;
			resultItem.TrayId = TrayId;
			resultItem.TaskId = TaskId;
			if (trayIdAndTunnelIdDictionary != null && trayIdAndTunnelIdDictionary.ContainsKey(TrayId))
			{
				resultItem.TunnelId = trayIdAndTunnelIdDictionary[TrayId];
				resultItem.Area = resultItem.TunnelId.Split('_')[1];
			}
			Random rangeRandom = new Random();
			Random rowRandom = new Random();
			Random columnRandom = new Random();
			int range = 0;
			int row = 0;
			int column = 0;
			switch (WarehouseId)
			{
				case "E07":
					range = GetRangByTunnelId(resultItem.TunnelId, rangeRandom);
					row = rowRandom.Next(1, 9);
					if (resultItem.Area == "S")
					{
						column = columnRandom.Next(1, 50);
					}
					else
					{
						column = columnRandom.Next(1, 40);
					}
					break;

				case "E06":
					range = GetRangByTunnelId(resultItem.TunnelId, rangeRandom);
					row = rowRandom.Next(1, 7);
					column = columnRandom.Next(1, 53);
					break;

				default:
					break;
			}
			resultItem.Cell = resultItem.WarehouseId + "1" + resultItem.Area + string.Format("{0:D2}{1:D2}{2:D2}", range, row, column) + "0";
			if (!trayIdAndCellCodeDictionary.ContainsKey(resultItem.TrayId))
			{
				trayIdAndCellCodeDictionary.Add(resultItem.TrayId,resultItem.Cell);
			}
			else
			{
				ConfigManageHelper.WriteLog("DemoException", string.Format("WebServiceForDemo的RequireAllocateCell接口收到调用，但是该托盘号：{0}已经请求过仓位分配了", resultItem.TrayId));
				trayIdAndCellCodeDictionary[resultItem.TrayId] = resultItem.Cell;
			}
			ConfigManageHelper.WriteLog("DemoLog", string.Format("WebServiceForDemo的RequireAllocateCell接口被调用，返回的内容为：cell = {0}", resultItem.Cell));
			return resultItem;
		}

		public bool InboundFinsh(Item item)
		{
			if (string.IsNullOrEmpty(item.WarehouseId) || string.IsNullOrEmpty(item.TrayId) ||
					string.IsNullOrEmpty(item.TaskId))
			{
				ConfigManageHelper.WriteLog("DemoException", "WebServiceForDemo收到InboundFinsh方法传参异常，" +
					string.Format("WARE_ID='{0}',TRAY_ID='{1}',TASK_ID='{2}'",
					string.IsNullOrEmpty(item.WarehouseId) ? "null" : item.WarehouseId,
					string.IsNullOrEmpty(item.TrayId) ? "null" : item.TrayId,
					string.IsNullOrEmpty(item.TaskId) ? "null" : item.TaskId));
				return false;
			}
			item.WarehouseId = item.WarehouseId;
			if (trayIdAndCellCodeDictionary != null && trayIdAndCellCodeDictionary.ContainsKey(item.TrayId))
			{
				item.Cell = trayIdAndCellCodeDictionary[item.TrayId];
				item.Area = item.Cell.Substring(4, 1);
			}
			bool isSuccess = baseData.InsertInboundInfo(item.TrayId, item.Cell, item.WarehouseId, item.Area, item.TaskId);
			if(!isSuccess)
			{
				ConfigManageHelper.WriteLog("DemoException", "向本地数据库插入入库信息失败!");
				return isSuccess;
			}
			ConfigManageHelper.WriteLog("DemoLog", "WebServiceForDemo的InboundFinsh接口收到调用，并已经向本地数据库插入信息！");
			return isSuccess;
		}

		public TrayItem RequireAllocateTunnelByTray(TrayItem item)
		{
			return new TrayItem();
		}

		public Item ReAllocateTunnelByMaterial(Item item)
		{
			return new Item();
		}

		public bool RequireTrayForPalletizer(string warehouseId, string stationId, TrayType trayType)
		{
			return true;
		}

		public StackingCraneItem InboundReAllocateCell(StackingCraneItem item)
		{
			return new StackingCraneItem();
		}

		public bool OffShelveFinish(string warehouseId, string trayId, string taskNo)
		{
			return true;
		}

		public bool OutBoundKeyNode(string warehouseId, string trayId, string taskNo)
		{
			return true;
		}

		public string OutboundFinish(Item item)
		{
			if (string.IsNullOrEmpty(item.WarehouseId) || string.IsNullOrEmpty(item.TrayId)
				|| string.IsNullOrEmpty(item.TaskId) || string.IsNullOrEmpty(item.StationCode))
			{
				ConfigManageHelper.WriteLog("DemoException", "WebServiceForDemo收到OutboundFinish方法传参异常，" +
					string.Format("WARE_ID='{0}',TRAY_ID='{1}',MIS_ID='{2}',STA_ID='{3}'",
					string.IsNullOrEmpty(item.WarehouseId) ? "null" : item.WarehouseId,
					string.IsNullOrEmpty(item.TrayId) ? "null" : item.TrayId,
					string.IsNullOrEmpty(item.TaskId) ? "null" : item.TaskId,
					string.IsNullOrEmpty(item.StationCode) ? "null" : item.StationCode));
				return "OutboundFinish收到传入参数有误，不做处理！";
			}
			if (trayIdAndCellCodeDictionary != null && trayIdAndCellCodeDictionary.ContainsKey(item.TrayId))
			{
				trayIdAndCellCodeDictionary.Remove(item.TrayId);
			}
			if (trayIdAndTunnelIdDictionary != null && trayIdAndTunnelIdDictionary.ContainsKey(item.TrayId))
			{
				trayIdAndTunnelIdDictionary.Remove(item.TrayId);
			}

			bool isSuccess = baseData.DeleteInboundInfoByTrayId(item.TrayId);
			if (!isSuccess)
			{
				ConfigManageHelper.WriteLog("DemoException", string.Format("从本地数据库删除入库信息失败!，托盘号:{0}",item.TrayId));
				return "出库时，从本地数据库删除入库信息失败!";
			}
			ConfigManageHelper.WriteLog("DemoLog", "WebServiceForDemo的OutboundFinish接口收到调用，并已经从本地数据库删除入库信息！");
			return "出库成功！";	
		}

		public InventoryItem RequireAllocateTunnelByPickingState(InventoryItem inventoryItem)
		{
			return new InventoryItem();
		}

		public Item ReAllocateOutboundState(Item item)
		{
			return new Item();
		}

		public bool PickingEmpty(string warehouseId, string trayId, string taskNo)
		{
			return true;
		}

		public StackingCraneItem OutboundReAllocateCell(StackingCraneItem stackingCraneItem)
		{
			return new StackingCraneItem();
		}

		public bool InboundJoinPointFinish(string warehouseId, string trayId, string taskNo, string transmitStationId)
		{
			return true;
		}

		/// <summary>
		/// 搬送过程中，WCS告之WMS衔接站台号，WMS反馈WCS
		/// </summary>
		/// <param name="warehouseId">仓库ID</param>
		/// <param name="trayId">母托盘ID</param>
		/// <param name="taskNo">任务号</param>
		/// <param name="transmitStationId">衔接站台号</param>
		/// <returns></returns>
		public bool OutboundJoinPointFinish(string warehouseId, string trayId, string taskNo, string transmitStationId)
		{
			bool ret = false;










			return ret;
		}

		public Item IncomingDataValidation(string warehouseId, string stationId, string subBarcode)
		{
			Item item = new Item();
			return item;
		}

		private int GetRangByTunnelId(string tunnelId, Random rangeRandom)
		{
			int range = 0;
			List<int> rangeList = new List<int>();
			List<string> cellList = TunnelCellConfig.Instance.GetCellsByTunnel(tunnelId);
			foreach(string cell in cellList)
			{  
				int rangeValue = Convert.ToInt32(cell.Substring(2,1));
				rangeList.Add(rangeValue);
			}
			if (rangeList == null || rangeList.Count < 1)
			{
				ConfigManageHelper.WriteLog("DemoException", string.Format("根据巷道号{0}返回的排号为空！", tunnelId));
				return range;
			}
			if (rangeList.Count == 1)
			{
				range = rangeList[0];
			}
			else
			{
				range = rangeList[rangeRandom.Next(0, rangeList.Count)];
			}
			return range;
		}


		Item WebServiceAbstract.OutboundJoinPointFinish(string warehouseId, string trayId, string taskNo, string transmitStationId)
		{
			throw new NotImplementedException();
		}


		public Item InboundJoinPointFinish(string warehouseId, string trayId, string transmitStationId)
		{
			throw new NotImplementedException();
		}
		public CmdReturnInfo NotifyRouteState(string[] routeIds, string routeStatus)
		{
			throw new NotImplementedException();
		}
		public CmdReturnInfo NotifyOffShelveFinish(string warehouseId, string trayId, string taskNo)
		{
			throw new NotImplementedException();
		}
		public CmdReturnInfo NotifyOrderFinish(string packageBarcode, string warehouseId, string taskNo, Addr dstAddr, OrderTypeEnum orderType)
		{
			throw new NotImplementedException();
		}
	}
}
