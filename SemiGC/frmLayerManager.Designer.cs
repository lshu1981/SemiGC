namespace SemiGC
{
    partial class frmLayerManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLayerManager));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupManager = new System.Windows.Forms.GroupBox();
            this.LayDel = new System.Windows.Forms.Button();
            this.groupInsert = new System.Windows.Forms.GroupBox();
            this.butCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.butOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupManager.SuspendLayout();
            this.groupInsert.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupManager);
            this.splitContainer1.Panel2.Controls.Add(this.groupInsert);
            this.splitContainer1.Size = new System.Drawing.Size(769, 420);
            this.splitContainer1.SplitterDistance = 267;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(769, 267);
            this.dataGridView1.TabIndex = 0;
            // 
            // groupManager
            // 
            this.groupManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupManager.Controls.Add(this.LayDel);
            this.groupManager.Location = new System.Drawing.Point(3, 69);
            this.groupManager.Name = "groupManager";
            this.groupManager.Size = new System.Drawing.Size(763, 63);
            this.groupManager.TabIndex = 6;
            this.groupManager.TabStop = false;
            this.groupManager.Text = "层管理";
            // 
            // LayDel
            // 
            this.LayDel.Location = new System.Drawing.Point(67, 25);
            this.LayDel.Name = "LayDel";
            this.LayDel.Size = new System.Drawing.Size(76, 26);
            this.LayDel.TabIndex = 2;
            this.LayDel.Text = "删除层";
            this.LayDel.UseVisualStyleBackColor = true;
            this.LayDel.Click += new System.EventHandler(this.LayDel_Click);
            // 
            // groupInsert
            // 
            this.groupInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupInsert.Controls.Add(this.butCancel);
            this.groupInsert.Controls.Add(this.label2);
            this.groupInsert.Controls.Add(this.butOK);
            this.groupInsert.Controls.Add(this.label1);
            this.groupInsert.Location = new System.Drawing.Point(3, 3);
            this.groupInsert.Name = "groupInsert";
            this.groupInsert.Size = new System.Drawing.Size(763, 66);
            this.groupInsert.TabIndex = 5;
            this.groupInsert.TabStop = false;
            this.groupInsert.Text = "插入";
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(466, 15);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(76, 26);
            this.butCancel.TabIndex = 4;
            this.butCancel.Text = "取消";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 47);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(248, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "也可以从本地磁盘中导入新的层：";
            this.label2.Visible = false;
            // 
            // butOK
            // 
            this.butOK.Location = new System.Drawing.Point(352, 15);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(76, 26);
            this.butOK.TabIndex = 2;
            this.butOK.Text = "确定";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "从上面表格中选择一层插入当前配方程序中";
            // 
            // frmLayerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 420);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmLayerManager";
            this.Text = "层管理";
            this.Load += new System.EventHandler(this.frmLayerManager_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupManager.ResumeLayout(false);
            this.groupInsert.ResumeLayout(false);
            this.groupInsert.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupInsert;
        private System.Windows.Forms.GroupBox groupManager;
        private System.Windows.Forms.Button LayDel;
    }
}