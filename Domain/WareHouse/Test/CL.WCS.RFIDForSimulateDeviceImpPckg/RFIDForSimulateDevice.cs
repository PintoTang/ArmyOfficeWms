using CL.Framework.CmdDataModelPckg;
using CL.WCS.BarcodeDeviceAbstractPckg;
using CL.WCS.OPCDataBlockManagerPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CL.WCS.RFIDForSimulateDeviceImpPckg
{
	/// <summary>
	/// RFID仿真界面用于内侧流程
	/// </summary>
	public class RFIDForSimulateDevice : BarcodeDeviceBase
	{
		private SimulateBarcodeDevice simulateBarcodeDevice;

		/// <summary>
		/// RFID仿真界面用于内侧流程
		/// (外部调用SendCommand获取条码，软件控制读取条码)
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="opcOrderIdDataBlock">PLC就绪地址</param>
		/// <param name="simulateBarcodeDevice">条码仿真界面</param>
		public RFIDForSimulateDevice(
			DeviceName deviceName,
			SimulateBarcodeDevice simulateBarcodeDevice
			)
			: base(deviceName)
		{
			this.simulateBarcodeDevice = simulateBarcodeDevice;
		}

		/// <summary>
		/// RFID仿真界面用于内侧流程
		/// </summary>
		/// <param name="deviceName">设备名</param>
		/// <param name="opcOrderIdDataBlock">PLC就绪地址</param>
		/// <param name="opcMonitorAbstract">opcMonitor</param>
		/// <param name="simulateBarcodeDevice">条码仿真界面</param>
		public RFIDForSimulateDevice(
			DeviceName deviceName,
			OPCMonitorAbstract opcMonitorAbstract,
			SimulateBarcodeDevice simulateBarcodeDevice
			)
			: base(deviceName)
		{
			this.simulateBarcodeDevice = simulateBarcodeDevice;
			OPCDataBlock opcDataBlock = new OPCDataBlock(deviceName.FullName);
			opcMonitorAbstract.RegisterValueChange(deviceName.FullName, opcDataBlock.GetRealDataBlockAddr(DataBlockNameEnum.OPCOrderIdDataBlock), OPCOrderIdValueChange);
		}

		private void OPCOrderIdValueChange(int value, string opcAddr, string name)
		{
			if (0 == value)
			{
				return;
			}
			OnReceiveBarcode(simulateBarcodeDevice.GetBarcode());
		}

		public override void SendCommand(params object[] para)
		{
			OnReceiveBarcode(simulateBarcodeDevice.GetBarcode());
		}
	}
}
