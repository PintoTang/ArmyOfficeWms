namespace CL.Framework.OPCClientSimulatePckg
{
	partial class OPCReadOrWriteForm
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTimmer = new System.Windows.Forms.TextBox();
            this.btnTimmer = new System.Windows.Forms.Button();
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.opcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.opcIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.readType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.opcValues = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.opcSend = new System.Windows.Forms.DataGridViewButtonColumn();
            this.opcNameLst = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.opcIndexLst = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.opcValueLst = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnWriteList = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.opcName,
            this.opcIndex,
            this.readType,
            this.opcValues,
            this.opcSend});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 17);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(505, 475);
            this.dataGridView1.TabIndex = 6;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.opcNameLst,
            this.opcIndexLst,
            this.opcValueLst});
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(3, 17);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(487, 446);
            this.dataGridView2.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(1008, 495);
            this.splitContainer1.SplitterDistance = 511;
            this.splitContainer1.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(215, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "MS";
            // 
            // txtTimmer
            // 
            this.txtTimmer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTimmer.Location = new System.Drawing.Point(141, 6);
            this.txtTimmer.Name = "txtTimmer";
            this.txtTimmer.Size = new System.Drawing.Size(53, 21);
            this.txtTimmer.TabIndex = 11;
            this.txtTimmer.Text = "200";
            // 
            // btnTimmer
            // 
            this.btnTimmer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTimmer.Location = new System.Drawing.Point(10, 4);
            this.btnTimmer.Name = "btnTimmer";
            this.btnTimmer.Size = new System.Drawing.Size(115, 23);
            this.btnTimmer.TabIndex = 10;
            this.btnTimmer.Text = "启动定时发送";
            this.btnTimmer.UseVisualStyleBackColor = true;
            this.btnTimmer.Click += new System.EventHandler(this.btnTimmer_Click);
            // 
            // dataGridViewButtonColumn1
            // 
            this.dataGridViewButtonColumn1.HeaderText = "操作";
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.Text = "发送";
            this.dataGridViewButtonColumn1.UseColumnTextForButtonValue = true;
            this.dataGridViewButtonColumn1.Width = 70;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 495);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "单个读取";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(493, 466);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "批量读取";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnWriteList);
            this.panel1.Controls.Add(this.txtTimmer);
            this.panel1.Controls.Add(this.btnTimmer);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 466);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 29);
            this.panel1.TabIndex = 14;
            // 
            // opcName
            // 
            this.opcName.HeaderText = "地址名";
            this.opcName.Name = "opcName";
            this.opcName.ReadOnly = true;
            this.opcName.Width = 210;
            // 
            // opcIndex
            // 
            this.opcIndex.HeaderText = "opcIndex";
            this.opcIndex.Name = "opcIndex";
            this.opcIndex.ReadOnly = true;
            this.opcIndex.Visible = false;
            // 
            // readType
            // 
            this.readType.HeaderText = "读取类型";
            this.readType.Name = "readType";
            this.readType.Width = 80;
            // 
            // opcValues
            // 
            this.opcValues.HeaderText = "返回值";
            this.opcValues.Name = "opcValues";
            // 
            // opcSend
            // 
            this.opcSend.HeaderText = "操作";
            this.opcSend.Name = "opcSend";
            this.opcSend.Text = "发送";
            this.opcSend.UseColumnTextForButtonValue = true;
            this.opcSend.Width = 70;
            // 
            // opcNameLst
            // 
            this.opcNameLst.HeaderText = "地址名";
            this.opcNameLst.Name = "opcNameLst";
            this.opcNameLst.ReadOnly = true;
            this.opcNameLst.Width = 300;
            // 
            // opcIndexLst
            // 
            this.opcIndexLst.HeaderText = "opcIndexLst";
            this.opcIndexLst.Name = "opcIndexLst";
            this.opcIndexLst.ReadOnly = true;
            this.opcIndexLst.Visible = false;
            // 
            // opcValueLst
            // 
            this.opcValueLst.HeaderText = "返回值";
            this.opcValueLst.Name = "opcValueLst";
            this.opcValueLst.Width = 110;
            // 
            // btnWriteList
            // 
            this.btnWriteList.Location = new System.Drawing.Point(408, 3);
            this.btnWriteList.Name = "btnWriteList";
            this.btnWriteList.Size = new System.Drawing.Size(75, 23);
            this.btnWriteList.TabIndex = 13;
            this.btnWriteList.Text = "批量发送";
            this.btnWriteList.UseVisualStyleBackColor = true;
            this.btnWriteList.Click += new System.EventHandler(this.btnWriteList_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(493, 466);
            this.panel2.TabIndex = 15;
            // 
            // OPCReadOrWriteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 495);
            this.Controls.Add(this.splitContainer1);
            this.Name = "OPCReadOrWriteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OPCReadOrWriteForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn1;
		private System.Windows.Forms.DataGridView dataGridView2;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button btnTimmer;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtTimmer;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn opcName;
        private System.Windows.Forms.DataGridViewTextBoxColumn opcIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn readType;
        private System.Windows.Forms.DataGridViewTextBoxColumn opcValues;
        private System.Windows.Forms.DataGridViewButtonColumn opcSend;
        private System.Windows.Forms.DataGridViewTextBoxColumn opcNameLst;
        private System.Windows.Forms.DataGridViewTextBoxColumn opcIndexLst;
        private System.Windows.Forms.DataGridViewTextBoxColumn opcValueLst;
        private System.Windows.Forms.Button btnWriteList;
        private System.Windows.Forms.Panel panel2;
    }
}