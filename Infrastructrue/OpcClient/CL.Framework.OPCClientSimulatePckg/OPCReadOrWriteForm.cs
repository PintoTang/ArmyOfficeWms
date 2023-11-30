using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using CL.Framework.OPCClientAbsPckg;

namespace CL.Framework.OPCClientSimulatePckg
{
	public partial class OPCReadOrWriteForm : Form, OPCReadOrWriteAbstract
	{
		private string callbackType = string.Empty;

		public delegate void SetDeviceLabelHandler(int address, string values, string readType);
		public delegate void SetDeviceLabelHandlerList(List<int> values);
		public static event SetDeviceLabelHandler ResultEvent;
		public static event SetDeviceLabelHandlerList ResultEventList;

		public delegate void SetDeviceLabelHandlerStringList(string flag, List<string> values);
		public static event SetDeviceLabelHandlerStringList ResultEventStringList;

		public OPCReadOrWriteForm(OPCClientSimulate opcClientSimulate)
		{
			InitializeComponent();

			ResultEvent += opcClientSimulate.OPCReadOrWrite_ResultEvent;
			ResultEventList += opcClientSimulate.OPCReadOrWriteList_ResultEvent;
			ResultEventStringList += opcClientSimulate.OPCReadOrWriteList_ResultEventStringList;

			//dataGridView1.AllowUserToAddRows = false;
			//dataGridView2.AllowUserToAddRows = false;
			//this.btnWriteList.Enabled = false;

			Thread thread = new Thread(OrderHandle);
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
		}

		private void OrderHandle()
		{
			Application.EnableVisualStyles();
			Application.Run(this);
		}

		private const int CP_NOCLOSE_BUTTON = 0x200;
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams myCp = base.CreateParams;
				myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
				return myCp;
			}
		}

		public void OPC_PassValue(string deviceName, string opcConnections, string itemName, string readType)
		{
			string strDeviceName = deviceName.Substring(deviceName.Length - 1, 1);

			if (this.dataGridView1.InvokeRequired)
			{
				this.dataGridView1.Invoke(new MethodInvoker(delegate { InsertDGV1(deviceName, opcConnections, itemName, readType); }));
			}
			else
			{
				InsertDGV1(deviceName, opcConnections, itemName, readType);
			}
		}

		public void OPC_PassValueList(string deviceName, List<DeviceAddressInfo> itemName)
		{
			string strDeviceName = deviceName.Substring(deviceName.Length - 1, 1);

			if (this.dataGridView2.InvokeRequired)
			{
				this.dataGridView2.Invoke(new MethodInvoker(delegate
				{
					InsertDGV2(deviceName, itemName);
					this.btnWriteList.Enabled = true;
				}));
			}
			else
			{
				InsertDGV2(deviceName, itemName);
				this.btnWriteList.Enabled = true;
			}
		}

		public void OPC_PassValueList(string type, string deviceName, List<DeviceAddressInfo> itemName)
		{
			lock (callbackType)
			{
				callbackType = type;
				OPC_PassValueList(deviceName, itemName);
			}
		}

		private void btnWriteList_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(callbackType))
			{
				lock (callbackType)
				{
					List<string> stringList = new List<string>();
					for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
					{
						stringList.Add(this.dataGridView2.Rows[i].Cells[2].Value.ToString());
					}

					ResultEventStringList(callbackType, stringList);
					callbackType = string.Empty;
					this.btnWriteList.Enabled = false;
					this.dataGridView2.Rows.Clear();
				}
				return;
			}

			List<int> list = new List<int>();
			string strOpcValue = "";
			int a = 0;
			for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
			{
				try
				{
					strOpcValue = this.dataGridView2.Rows[i].Cells[2].Value.ToString();
				}
				catch (Exception)
				{
					MessageBox.Show("返回值不能为空！");
					return;
				}
				if (int.TryParse(strOpcValue, out a) == false)
					return;
				list.Add(a);
			}
			ResultEventList(list);
			this.btnWriteList.Enabled = false;
			this.dataGridView2.Rows.Clear();
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 4)//button列
			{
				return;
			}

			string strOpcValue = "";
			if (ResultEvent != null)
			{
				string readType = this.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
				try
				{
					if (this.dataGridView1.Rows[e.RowIndex].Cells[3].Value == null &&readType=="String")
					{
						strOpcValue = "";
					}
					else
					{
						strOpcValue = this.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
					}
				}
				catch (Exception)
				{
					MessageBox.Show("返回值不能为空！");
					return;
				}

				ResultEvent(e.RowIndex, strOpcValue, readType);
				this.dataGridView1.Rows.RemoveAt(e.RowIndex);
			}
		}

		/// <summary>
		/// 向dataGridView1插入新行
		/// </summary>
		/// <param name="deviceName"></param>
		/// <param name="opcConnections"></param>
		/// <param name="itemName"></param>
		/// <param name="readType"></param>
		private void InsertDGV1(string deviceName, string opcConnections, string itemName, string readType)
		{
			DataGridViewRow row = new DataGridViewRow();

			DataGridViewTextBoxCell opcName = new DataGridViewTextBoxCell();
			opcName.Value = deviceName + ":" + itemName;
			row.Cells.Add(opcName);

			DataGridViewTextBoxCell opcIndex = new DataGridViewTextBoxCell();
			opcIndex.Value = deviceName + "#" + itemName;
			row.Cells.Add(opcIndex);

			//DataGridViewTextBoxCell opcConnection = new DataGridViewTextBoxCell();
			//opcConnection.Value = opcConnections;
			//row.Cells.Add(opcConnection);

			DataGridViewTextBoxCell type = new DataGridViewTextBoxCell();
			type.Value = readType;
			row.Cells.Add(type);

			DataGridViewTextBoxCell opcValue = new DataGridViewTextBoxCell();
			switch (readType)
			{
				case "INT":
					opcValue.Value = 0;
					break;
				case "Bool":
					opcValue.Value = "false";
					break;
				default:
					break;
			}
			row.Cells.Add(opcValue);

			this.dataGridView1.Rows.Insert(this.dataGridView1.Rows.Count, row);
		}
		/// <summary>
		/// 向dataGridView2中插入新行
		/// </summary>
		/// <param name="deviceName"></param>
		/// <param name="itemName"></param>
		private void InsertDGV2(string deviceName, List<DeviceAddressInfo> itemName)
		{
			foreach (var item in itemName)
			{
				DataGridViewRow row = new DataGridViewRow();

				DataGridViewTextBoxCell opcNameLst = new DataGridViewTextBoxCell();
				//opcNameLst.Value = item.deviceName.Split('@')[0] + ":" + item.Datablock;//old
				opcNameLst.Value = item.Datablock.RealDataBlockAddr;
				row.Cells.Add(opcNameLst);

				DataGridViewTextBoxCell opcIndexLst = new DataGridViewTextBoxCell();
				//opcIndexLst.Value = item.deviceName + "#" + item.Datablock;//old 
				opcIndexLst.Value = item.Datablock.RealDataBlockAddr;
				row.Cells.Add(opcIndexLst);

				//去掉 不用的连接
				//DataGridViewTextBoxCell opcConnectionLst = new DataGridViewTextBoxCell();
				//opcConnectionLst.Value = item.deviceName.Split('@')[1];
				//row.Cells.Add(opcConnectionLst);

				DataGridViewTextBoxCell opcValueLst = new DataGridViewTextBoxCell();
				opcValueLst.Value = string.IsNullOrEmpty(callbackType) ? "0" : "false";
				row.Cells.Add(opcValueLst);

				this.dataGridView2.Rows.Insert(this.dataGridView2.Rows.Count, row);
			}
		}
		int timmerMS = 200;
		Thread th = null;
		private void btnTimmer_Click(object sender, EventArgs e)
		{
			Button btn = sender as Button;
			if (btn.Tag == null)
			{
				int.TryParse(txtTimmer.Text, out timmerMS);
				btn.Tag = "已启动";
				btn.Text = "停止";
				th = new Thread(new ThreadStart(TimmerWriter));
				th.IsBackground = true;
				th.Start();
			}
			else
			{
				btn.Tag = null;
				btn.Text = "启动定时发送";
			}
		}

		private void TimmerWriter()
		{
			while (btnTimmer.Tag != null && "已启动" == btnTimmer.Tag.ToString())
			{
				try
				{
					List<int> list = new List<int>();
					string strOpcValue = "";
					int a = 0;
					for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
					{
						try
						{
							strOpcValue = this.dataGridView2.Rows[i].Cells[2].Value.ToString();
						}
						catch (Exception)
						{
							MessageBox.Show("返回值不能为空！");
							continue;
						}
						if (int.TryParse(strOpcValue, out a) == false)
							continue;
						list.Add(a);
					}
					//if (list.Count == 0)
					//	continue;

					ResultEventList(list);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message + "!" + ex.StackTrace);
				}
				finally
				{

					Thread.Sleep(timmerMS);
					if (this.dataGridView2.InvokeRequired)
					{
						DCallBackDataGridView d = new DCallBackDataGridView(Ctrl_Event_DCallBack);
						this.Invoke(d, this.dataGridView2);
					}
				}
			}
		}
		delegate void DCallBackDataGridView(DataGridView sender);
		void Ctrl_Event_DCallBack(DataGridView sender)
		{
			sender.Rows.Clear();
		}
	}
}
