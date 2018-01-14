namespace LSSCADA.Control
{
    partial class frmC曲线刻度设置
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dGV1 = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.XAxisFormat = new System.Windows.Forms.TextBox();
            this.GridColor = new System.Windows.Forms.PictureBox();
            this.CurveLineWith = new System.Windows.Forms.NumericUpDown();
            this.LegendFill = new System.Windows.Forms.PictureBox();
            this.ChartFill = new System.Windows.Forms.PictureBox();
            this.PaneFill = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.XAxisTitleShow = new System.Windows.Forms.CheckBox();
            this.XAxisTitle = new System.Windows.Forms.TextBox();
            this.dGV2 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurveLineWith)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LegendFill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartFill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaneFill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGV2)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button3);
            this.splitContainer1.Panel2.Controls.Add(this.button2);
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Size = new System.Drawing.Size(495, 318);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dGV1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(408, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(87, 243);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Y轴刻度设置";
            // 
            // dGV1
            // 
            this.dGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGV1.Location = new System.Drawing.Point(3, 22);
            this.dGV1.Margin = new System.Windows.Forms.Padding(4);
            this.dGV1.Name = "dGV1";
            this.dGV1.RowTemplate.Height = 23;
            this.dGV1.Size = new System.Drawing.Size(81, 218);
            this.dGV1.TabIndex = 2;
            this.dGV1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dGV1_CellBeginEdit);
            this.dGV1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV1_CellDoubleClick);
            this.dGV1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV1_CellEndEdit);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.splitContainer2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(361, 243);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "曲线设置";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(3, 22);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dGV2);
            this.splitContainer2.Size = new System.Drawing.Size(355, 218);
            this.splitContainer2.SplitterDistance = 108;
            this.splitContainer2.TabIndex = 42;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.XAxisFormat, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.GridColor, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.CurveLineWith, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.LegendFill, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ChartFill, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.PaneFill, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label8, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label9, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.XAxisTitleShow, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.XAxisTitle, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(355, 108);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // XAxisFormat
            // 
            this.XAxisFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XAxisFormat.Location = new System.Drawing.Point(259, 84);
            this.XAxisFormat.Name = "XAxisFormat";
            this.XAxisFormat.Size = new System.Drawing.Size(93, 26);
            this.XAxisFormat.TabIndex = 43;
            this.XAxisFormat.Text = "HH:mm:ss";
            // 
            // GridColor
            // 
            this.GridColor.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.GridColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.GridColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridColor.Location = new System.Drawing.Point(259, 30);
            this.GridColor.Name = "GridColor";
            this.GridColor.Size = new System.Drawing.Size(93, 21);
            this.GridColor.TabIndex = 12;
            this.GridColor.TabStop = false;
            this.GridColor.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // CurveLineWith
            // 
            this.CurveLineWith.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CurveLineWith.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CurveLineWith.Location = new System.Drawing.Point(98, 81);
            this.CurveLineWith.Margin = new System.Windows.Forms.Padding(0);
            this.CurveLineWith.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.CurveLineWith.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CurveLineWith.Name = "CurveLineWith";
            this.CurveLineWith.Size = new System.Drawing.Size(60, 26);
            this.CurveLineWith.TabIndex = 41;
            this.CurveLineWith.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.CurveLineWith.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // LegendFill
            // 
            this.LegendFill.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.LegendFill.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LegendFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LegendFill.Location = new System.Drawing.Point(101, 30);
            this.LegendFill.Name = "LegendFill";
            this.LegendFill.Size = new System.Drawing.Size(54, 21);
            this.LegendFill.TabIndex = 11;
            this.LegendFill.TabStop = false;
            this.LegendFill.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // ChartFill
            // 
            this.ChartFill.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ChartFill.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ChartFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChartFill.Location = new System.Drawing.Point(259, 3);
            this.ChartFill.Name = "ChartFill";
            this.ChartFill.Size = new System.Drawing.Size(93, 21);
            this.ChartFill.TabIndex = 10;
            this.ChartFill.TabStop = false;
            this.ChartFill.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // PaneFill
            // 
            this.PaneFill.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.PaneFill.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PaneFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PaneFill.Location = new System.Drawing.Point(101, 3);
            this.PaneFill.Name = "PaneFill";
            this.PaneFill.Size = new System.Drawing.Size(54, 21);
            this.PaneFill.TabIndex = 9;
            this.PaneFill.TabStop = false;
            this.PaneFill.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 27);
            this.label1.TabIndex = 8;
            this.label1.Text = "图表区背景色";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(161, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 27);
            this.label3.TabIndex = 1;
            this.label3.Text = "绘图区背景色";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 27);
            this.label4.TabIndex = 2;
            this.label4.Text = "图例区背景色";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(161, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 27);
            this.label5.TabIndex = 3;
            this.label5.Text = "网格颜色";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 27);
            this.label6.TabIndex = 4;
            this.label6.Text = "显示X轴标题";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 27);
            this.label7.TabIndex = 5;
            this.label7.Text = "曲线线宽";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(161, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 27);
            this.label8.TabIndex = 6;
            this.label8.Text = "X轴标题";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(161, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 27);
            this.label9.TabIndex = 7;
            this.label9.Text = "X轴刻度格式";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // XAxisTitleShow
            // 
            this.XAxisTitleShow.AutoSize = true;
            this.XAxisTitleShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XAxisTitleShow.Location = new System.Drawing.Point(101, 57);
            this.XAxisTitleShow.Name = "XAxisTitleShow";
            this.XAxisTitleShow.Size = new System.Drawing.Size(54, 21);
            this.XAxisTitleShow.TabIndex = 13;
            this.XAxisTitleShow.UseVisualStyleBackColor = true;
            // 
            // XAxisTitle
            // 
            this.XAxisTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XAxisTitle.Location = new System.Drawing.Point(259, 57);
            this.XAxisTitle.Name = "XAxisTitle";
            this.XAxisTitle.Size = new System.Drawing.Size(93, 26);
            this.XAxisTitle.TabIndex = 42;
            this.XAxisTitle.Text = "采样时间";
            // 
            // dGV2
            // 
            this.dGV2.AllowUserToAddRows = false;
            this.dGV2.AllowUserToDeleteRows = false;
            this.dGV2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.dGV2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGV2.Location = new System.Drawing.Point(0, 0);
            this.dGV2.Margin = new System.Windows.Forms.Padding(4);
            this.dGV2.Name = "dGV2";
            this.dGV2.RowTemplate.Height = 23;
            this.dGV2.Size = new System.Drawing.Size(355, 106);
            this.dGV2.TabIndex = 3;
            this.dGV2.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV2_CellDoubleClick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 5;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column2.HeaderText = "字体";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 80;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Column3.HeaderText = "是否填充";
            this.Column3.Name = "Column3";
            this.Column3.Width = 78;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Column4.HeaderText = "填充颜色";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 78;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Column5.HeaderText = "刻度自适应";
            this.Column5.Name = "Column5";
            this.Column5.Width = 94;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(391, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 28);
            this.button3.TabIndex = 2;
            this.button3.Text = "取消";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(251, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 28);
            this.button2.TabIndex = 1;
            this.button2.Text = "确定";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(251, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 28);
            this.button1.TabIndex = 3;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // frmC曲线刻度设置
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 318);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmC曲线刻度设置";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "曲线参数";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGV1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurveLineWith)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LegendFill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChartFill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaneFill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGV2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dGV1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown CurveLineWith;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox XAxisFormat;
        private System.Windows.Forms.PictureBox GridColor;
        private System.Windows.Forms.PictureBox LegendFill;
        private System.Windows.Forms.PictureBox ChartFill;
        private System.Windows.Forms.PictureBox PaneFill;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox XAxisTitleShow;
        private System.Windows.Forms.TextBox XAxisTitle;
        private System.Windows.Forms.DataGridView dGV2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column5;
    }
}