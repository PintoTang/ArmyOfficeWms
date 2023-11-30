using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CL.WCS.BarcodeDeviceAbstractPckg;
using CL.Framework.CmdDataModelPckg;

namespace CL.WCS.RFIDForSimulateDeviceImpPckg
{
	public partial class SimulateBarcodeDevice : Form
	{
		List<string> currentBarcodeList = new List<string>();
		public SimulateBarcodeDevice(DeviceName deviceName)
		{
			InitializeComponent();
			this.Text = deviceName.FullName;
			this.Show();
		}

		private void btnUpdateBarcode_Click(object sender, EventArgs e)
		{
			string barcode = txbNewBarCodeList.Text.Trim().Replace("\r\n", "");
			lock (currentBarcodeList)
			{
				currentBarcodeList = barcode.Split(',').ToList();
				BindData();
			}
		}

		private void BindData()
		{
			string barcode = "";
			foreach (string item in currentBarcodeList)
			{
				barcode += item + "\r\n";
			}
			txbCurrentBarCodeList.Text = barcode;
		}


		public List<string> GetBarcode()
		{
			return currentBarcodeList;
		}
	}
}
