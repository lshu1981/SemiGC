namespace LSSCADA.Control
{
    partial class frmAlarmList
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkUpdate = new System.Windows.Forms.CheckBox();
            this.ButFirmAll = new System.Windows.Forms.Button();
            this.ButFirmOne = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dGV1 = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dGV2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dGV3 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dTP_E = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dTP_S = new System.Windows.Forms.DateTimePicker();
            this.butQuery = new System.Windows.Forms.Button();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.HisdGV1 = new System.Windows.Forms.DataGridView();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.HisdGV2 = new System.Windows.Forms.DataGridView();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.HisdGV3 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV3)).BeginInit();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV1)).BeginInit();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV2)).BeginInit();
            this.tabPage8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV3)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkUpdate);
            this.splitContainer1.Panel1.Controls.Add(this.ButFirmAll);
            this.splitContainer1.Panel1.Controls.Add(this.ButFirmOne);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1202, 481);
            this.splitContainer1.SplitterDistance = 40;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // checkUpdate
            // 
            this.checkUpdate.AutoSize = true;
            this.checkUpdate.Location = new System.Drawing.Point(187, 11);
            this.checkUpdate.Name = "checkUpdate";
            this.checkUpdate.Size = new System.Drawing.Size(91, 20);
            this.checkUpdate.TabIndex = 2;
            this.checkUpdate.Text = "暂停刷新";
            this.checkUpdate.UseVisualStyleBackColor = true;
            // 
            // ButFirmAll
            // 
            this.ButFirmAll.Location = new System.Drawing.Point(74, 6);
            this.ButFirmAll.Name = "ButFirmAll";
            this.ButFirmAll.Size = new System.Drawing.Size(89, 29);
            this.ButFirmAll.TabIndex = 1;
            this.ButFirmAll.Text = "确认全部";
            this.ButFirmAll.UseVisualStyleBackColor = true;
            this.ButFirmAll.Click += new System.EventHandler(this.ButFirmAll_Click);
            // 
            // ButFirmOne
            // 
            this.ButFirmOne.Location = new System.Drawing.Point(12, 6);
            this.ButFirmOne.Name = "ButFirmOne";
            this.ButFirmOne.Size = new System.Drawing.Size(56, 29);
            this.ButFirmOne.TabIndex = 0;
            this.ButFirmOne.Text = "确认";
            this.ButFirmOne.UseVisualStyleBackColor = true;
            this.ButFirmOne.Click += new System.EventHandler(this.ButFirmOne_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1198, 408);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dGV1);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1190, 378);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "【报警】";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dGV1
            // 
            this.dGV1.AllowUserToAddRows = false;
            this.dGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGV1.Location = new System.Drawing.Point(3, 3);
            this.dGV1.Name = "dGV1";
            this.dGV1.ReadOnly = true;
            this.dGV1.RowTemplate.Height = 23;
            this.dGV1.Size = new System.Drawing.Size(1184, 372);
            this.dGV1.TabIndex = 1;
            this.dGV1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV1_CellDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dGV2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1190, 382);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "【警告】";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dGV2
            // 
            this.dGV2.AllowUserToAddRows = false;
            this.dGV2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGV2.Location = new System.Drawing.Point(3, 3);
            this.dGV2.Name = "dGV2";
            this.dGV2.ReadOnly = true;
            this.dGV2.RowTemplate.Height = 23;
            this.dGV2.Size = new System.Drawing.Size(1198, 426);
            this.dGV2.TabIndex = 2;
            this.dGV2.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV2_CellDoubleClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dGV3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1190, 382);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "【事件】";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dGV3
            // 
            this.dGV3.AllowUserToAddRows = false;
            this.dGV3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGV3.Location = new System.Drawing.Point(3, 3);
            this.dGV3.Name = "dGV3";
            this.dGV3.ReadOnly = true;
            this.dGV3.RowTemplate.Height = 23;
            this.dGV3.Size = new System.Drawing.Size(1198, 426);
            this.dGV3.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1216, 517);
            this.tabControl2.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer1);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1208, 487);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "实时日志";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer2);
            this.tabPage5.Location = new System.Drawing.Point(4, 26);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1208, 487);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "历史日志查询";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dTP_E);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.dTP_S);
            this.splitContainer2.Panel1.Controls.Add(this.butQuery);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl3);
            this.splitContainer2.Size = new System.Drawing.Size(1202, 481);
            this.splitContainer2.SplitterDistance = 40;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 1;
            // 
            // dTP_E
            // 
            this.dTP_E.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dTP_E.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dTP_E.Location = new System.Drawing.Point(256, 6);
            this.dTP_E.Name = "dTP_E";
            this.dTP_E.Size = new System.Drawing.Size(203, 26);
            this.dTP_E.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(224, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 12;
            this.label3.Text = "至";
            // 
            // dTP_S
            // 
            this.dTP_S.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dTP_S.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dTP_S.Location = new System.Drawing.Point(11, 6);
            this.dTP_S.Name = "dTP_S";
            this.dTP_S.Size = new System.Drawing.Size(203, 26);
            this.dTP_S.TabIndex = 11;
            // 
            // butQuery
            // 
            this.butQuery.Location = new System.Drawing.Point(479, 3);
            this.butQuery.Name = "butQuery";
            this.butQuery.Size = new System.Drawing.Size(56, 29);
            this.butQuery.TabIndex = 0;
            this.butQuery.Text = "查询";
            this.butQuery.UseVisualStyleBackColor = true;
            this.butQuery.Click += new System.EventHandler(this.butQuery_Click);
            // 
            // tabControl3
            // 
            this.tabControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl3.Controls.Add(this.tabPage6);
            this.tabControl3.Controls.Add(this.tabPage7);
            this.tabControl3.Controls.Add(this.tabPage8);
            this.tabControl3.Location = new System.Drawing.Point(4, 4);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(1198, 407);
            this.tabControl3.TabIndex = 2;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.HisdGV1);
            this.tabPage6.Location = new System.Drawing.Point(4, 26);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(1190, 377);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.Text = "【报警】";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // HisdGV1
            // 
            this.HisdGV1.AllowUserToAddRows = false;
            this.HisdGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HisdGV1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HisdGV1.Location = new System.Drawing.Point(3, 3);
            this.HisdGV1.Name = "HisdGV1";
            this.HisdGV1.ReadOnly = true;
            this.HisdGV1.RowTemplate.Height = 23;
            this.HisdGV1.Size = new System.Drawing.Size(1184, 375);
            this.HisdGV1.TabIndex = 1;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.HisdGV2);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(1190, 381);
            this.tabPage7.TabIndex = 1;
            this.tabPage7.Text = "【警告】";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // HisdGV2
            // 
            this.HisdGV2.AllowUserToAddRows = false;
            this.HisdGV2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HisdGV2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HisdGV2.Location = new System.Drawing.Point(3, 3);
            this.HisdGV2.Name = "HisdGV2";
            this.HisdGV2.ReadOnly = true;
            this.HisdGV2.RowTemplate.Height = 23;
            this.HisdGV2.Size = new System.Drawing.Size(1184, 375);
            this.HisdGV2.TabIndex = 2;
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.HisdGV3);
            this.tabPage8.Location = new System.Drawing.Point(4, 22);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(1190, 381);
            this.tabPage8.TabIndex = 2;
            this.tabPage8.Text = "【事件】";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // HisdGV3
            // 
            this.HisdGV3.AllowUserToAddRows = false;
            this.HisdGV3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HisdGV3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HisdGV3.Location = new System.Drawing.Point(3, 3);
            this.HisdGV3.Name = "HisdGV3";
            this.HisdGV3.ReadOnly = true;
            this.HisdGV3.RowTemplate.Height = 23;
            this.HisdGV3.Size = new System.Drawing.Size(1184, 375);
            this.HisdGV3.TabIndex = 2;
            // 
            // frmAlarmList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 517);
            this.Controls.Add(this.tabControl2);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmAlarmList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "日志";
            this.Load += new System.EventHandler(this.frmAlarmList_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGV1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGV2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGV3)).EndInit();
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV1)).EndInit();
            this.tabPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV2)).EndInit();
            this.tabPage8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button ButFirmOne;
        private System.Windows.Forms.DataGridView dGV1;
        private System.Windows.Forms.CheckBox checkUpdate;
        private System.Windows.Forms.Button ButFirmAll;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dGV2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dGV3;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button butQuery;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.DataGridView HisdGV1;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.DataGridView HisdGV2;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.DataGridView HisdGV3;
        private System.Windows.Forms.DateTimePicker dTP_E;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dTP_S;
    }
}