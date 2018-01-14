namespace LSSCADA.Control
{
    partial class frmC数据监视
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
            this.butShowHis = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.butUpdateDGV = new System.Windows.Forms.Button();
            this.HisdGV1 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dTP1 = new System.Windows.Forms.DateTimePicker();
            this.dTP2 = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nMY = new System.Windows.Forms.NumericUpDown();
            this.nMM = new System.Windows.Forms.NumericUpDown();
            this.nMD = new System.Windows.Forms.NumericUpDown();
            this.nMH = new System.Windows.Forms.NumericUpDown();
            this.nMMin = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nMY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMMin)).BeginInit();
            this.SuspendLayout();
            // 
            // butShowHis
            // 
            this.butShowHis.ForeColor = System.Drawing.Color.Black;
            this.butShowHis.Location = new System.Drawing.Point(57, 138);
            this.butShowHis.Name = "butShowHis";
            this.butShowHis.Size = new System.Drawing.Size(92, 29);
            this.butShowHis.TabIndex = 11;
            this.butShowHis.Text = "刷新曲线";
            this.butShowHis.UseVisualStyleBackColor = true;
            this.butShowHis.Click += new System.EventHandler(this.butShowHis_Click);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(251, 549);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(140)))), ((int)(((byte)(160)))));
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.Black;
            this.splitContainer2.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer2.Size = new System.Drawing.Size(1280, 732);
            this.splitContainer2.SplitterDistance = 251;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel3);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.dTP1);
            this.splitContainer1.Panel1.Controls.Add(this.dTP2);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.butShowHis);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(251, 732);
            this.splitContainer1.SplitterDistance = 179;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1025, 732);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Black;
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1017, 706);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "曲线模式";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.Black;
            this.tabPage5.Controls.Add(this.splitContainer3);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1068, 706);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "表格模式";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.butUpdateDGV);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.HisdGV1);
            this.splitContainer3.Size = new System.Drawing.Size(1062, 700);
            this.splitContainer3.SplitterDistance = 37;
            this.splitContainer3.TabIndex = 3;
            // 
            // butUpdateDGV
            // 
            this.butUpdateDGV.ForeColor = System.Drawing.Color.Black;
            this.butUpdateDGV.Location = new System.Drawing.Point(55, 3);
            this.butUpdateDGV.Name = "butUpdateDGV";
            this.butUpdateDGV.Size = new System.Drawing.Size(92, 29);
            this.butUpdateDGV.TabIndex = 12;
            this.butUpdateDGV.Text = "刷新表格";
            this.butUpdateDGV.UseVisualStyleBackColor = true;
            this.butUpdateDGV.Click += new System.EventHandler(this.butUpdateDGV_Click);
            // 
            // HisdGV1
            // 
            this.HisdGV1.AllowUserToAddRows = false;
            this.HisdGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HisdGV1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HisdGV1.Location = new System.Drawing.Point(0, 0);
            this.HisdGV1.Name = "HisdGV1";
            this.HisdGV1.ReadOnly = true;
            this.HisdGV1.RowTemplate.Height = 23;
            this.HisdGV1.Size = new System.Drawing.Size(1062, 659);
            this.HisdGV1.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dTP1
            // 
            this.dTP1.CalendarFont = new System.Drawing.Font("宋体", 12F);
            this.dTP1.CustomFormat = "yyyy-MM-dd";
            this.dTP1.Font = new System.Drawing.Font("宋体", 12F);
            this.dTP1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dTP1.Location = new System.Drawing.Point(7, 24);
            this.dTP1.Name = "dTP1";
            this.dTP1.Size = new System.Drawing.Size(118, 26);
            this.dTP1.TabIndex = 13;
            // 
            // dTP2
            // 
            this.dTP2.CustomFormat = "HH:mm:ss";
            this.dTP2.Font = new System.Drawing.Font("宋体", 12F);
            this.dTP2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dTP2.Location = new System.Drawing.Point(138, 24);
            this.dTP2.Name = "dTP2";
            this.dTP2.ShowUpDown = true;
            this.dTP2.Size = new System.Drawing.Size(106, 26);
            this.dTP2.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "起始时间：";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.99984F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.00024F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.99984F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.00024F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.99984F));
            this.tableLayoutPanel3.Controls.Add(this.nMMin, 4, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.nMH, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.nMY, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.nMD, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.nMM, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label7, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.label8, 4, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(7, 74);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(239, 52);
            this.tableLayoutPanel3.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(1, 1);
            this.label2.Margin = new System.Windows.Forms.Padding(1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 18);
            this.label2.TabIndex = 7;
            this.label2.Text = "年";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(48, 1);
            this.label4.Margin = new System.Windows.Forms.Padding(1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "月";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(95, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 18);
            this.label1.TabIndex = 8;
            this.label1.Text = "日";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(189, 1);
            this.label8.Margin = new System.Windows.Forms.Padding(1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 18);
            this.label8.TabIndex = 11;
            this.label8.Text = "分";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(142, 1);
            this.label7.Margin = new System.Windows.Forms.Padding(1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 18);
            this.label7.TabIndex = 10;
            this.label7.Text = "时";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "时间长度：";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nMY
            // 
            this.nMY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nMY.Font = new System.Drawing.Font("宋体", 12F);
            this.nMY.Location = new System.Drawing.Point(3, 23);
            this.nMY.Name = "nMY";
            this.nMY.Size = new System.Drawing.Size(41, 26);
            this.nMY.TabIndex = 17;
            // 
            // nMM
            // 
            this.nMM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nMM.Font = new System.Drawing.Font("宋体", 12F);
            this.nMM.Location = new System.Drawing.Point(50, 23);
            this.nMM.Name = "nMM";
            this.nMM.Size = new System.Drawing.Size(41, 26);
            this.nMM.TabIndex = 17;
            // 
            // nMD
            // 
            this.nMD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nMD.Font = new System.Drawing.Font("宋体", 12F);
            this.nMD.Location = new System.Drawing.Point(97, 23);
            this.nMD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nMD.Name = "nMD";
            this.nMD.Size = new System.Drawing.Size(41, 26);
            this.nMD.TabIndex = 17;
            this.nMD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nMH
            // 
            this.nMH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nMH.Font = new System.Drawing.Font("宋体", 12F);
            this.nMH.Location = new System.Drawing.Point(144, 23);
            this.nMH.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nMH.Name = "nMH";
            this.nMH.Size = new System.Drawing.Size(41, 26);
            this.nMH.TabIndex = 17;
            // 
            // nMMin
            // 
            this.nMMin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nMMin.Font = new System.Drawing.Font("宋体", 12F);
            this.nMMin.Location = new System.Drawing.Point(191, 23);
            this.nMMin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nMMin.Name = "nMMin";
            this.nMMin.Size = new System.Drawing.Size(45, 26);
            this.nMMin.TabIndex = 17;
            // 
            // frmC数据监视
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1280, 732);
            this.Controls.Add(this.splitContainer2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmC数据监视";
            this.Text = "frmC生长室";
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HisdGV1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nMY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nMMin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button butShowHis;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView HisdGV1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button butUpdateDGV;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.NumericUpDown nMMin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nMH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nMY;
        private System.Windows.Forms.NumericUpDown nMD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nMM;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dTP1;
        private System.Windows.Forms.DateTimePicker dTP2;
        private System.Windows.Forms.Label label5;
    }
}