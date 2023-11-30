namespace CL.WCS.RFIDForSimulateDeviceImpPckg
{
	partial class SimulateBarcodeDevice
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.txbCurrentBarCodeList = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txbNewBarCodeList = new System.Windows.Forms.TextBox();
			this.btnUpdateBarcode = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "当前容器条码：";
			// 
			// txbCurrentBarCodeList
			// 
			this.txbCurrentBarCodeList.Location = new System.Drawing.Point(30, 56);
			this.txbCurrentBarCodeList.Multiline = true;
			this.txbCurrentBarCodeList.Name = "txbCurrentBarCodeList";
			this.txbCurrentBarCodeList.ReadOnly = true;
			this.txbCurrentBarCodeList.Size = new System.Drawing.Size(180, 350);
			this.txbCurrentBarCodeList.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(268, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "新条码：";
			// 
			// txbNewBarCodeList
			// 
			this.txbNewBarCodeList.Location = new System.Drawing.Point(289, 56);
			this.txbNewBarCodeList.Multiline = true;
			this.txbNewBarCodeList.Name = "txbNewBarCodeList";
			this.txbNewBarCodeList.Size = new System.Drawing.Size(180, 350);
			this.txbNewBarCodeList.TabIndex = 3;
			// 
			// btnUpdateBarcode
			// 
			this.btnUpdateBarcode.Location = new System.Drawing.Point(499, 373);
			this.btnUpdateBarcode.Name = "btnUpdateBarcode";
			this.btnUpdateBarcode.Size = new System.Drawing.Size(75, 23);
			this.btnUpdateBarcode.TabIndex = 4;
			this.btnUpdateBarcode.Text = "更新";
			this.btnUpdateBarcode.UseVisualStyleBackColor = true;
			this.btnUpdateBarcode.Click += new System.EventHandler(this.btnUpdateBarcode_Click);
			// 
			// SimulateBarcodeDevice
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 462);
			this.Controls.Add(this.btnUpdateBarcode);
			this.Controls.Add(this.txbNewBarCodeList);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txbCurrentBarCodeList);
			this.Controls.Add(this.label1);
			this.Name = "SimulateBarcodeDevice";
			this.Text = "RFID模拟设备";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txbCurrentBarCodeList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txbNewBarCodeList;
		private System.Windows.Forms.Button btnUpdateBarcode;

	}
}