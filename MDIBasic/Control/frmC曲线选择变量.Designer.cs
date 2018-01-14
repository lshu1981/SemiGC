namespace LSSCADA.Control
{
    partial class frmC曲线选择变量
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
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.butTeamDown = new System.Windows.Forms.Button();
            this.butTeamAdd = new System.Windows.Forms.Button();
            this.butTeamUp = new System.Windows.Forms.Button();
            this.butTeamEdit = new System.Windows.Forms.Button();
            this.butTeamDel = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.butApply = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.butOK = new System.Windows.Forms.Button();
            this.GroupAdd = new System.Windows.Forms.GroupBox();
            this.butAdd4 = new System.Windows.Forms.Button();
            this.butAdd1 = new System.Windows.Forms.Button();
            this.butAdd3 = new System.Windows.Forms.Button();
            this.butAdd2 = new System.Windows.Forms.Button();
            this.GroupDel = new System.Windows.Forms.GroupBox();
            this.butDel = new System.Windows.Forms.Button();
            this.butDown1 = new System.Windows.Forms.Button();
            this.butUp1 = new System.Windows.Forms.Button();
            this.dGV1 = new System.Windows.Forms.DataGridView();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.GroupAdd.SuspendLayout();
            this.GroupDel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1184, 742);
            this.splitContainer1.SplitterDistance = 461;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.treeView1);
            this.splitContainer3.Size = new System.Drawing.Size(461, 742);
            this.splitContainer3.SplitterDistance = 180;
            this.splitContainer3.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.butTeamDown);
            this.splitContainer4.Panel1.Controls.Add(this.butTeamAdd);
            this.splitContainer4.Panel1.Controls.Add(this.butTeamUp);
            this.splitContainer4.Panel1.Controls.Add(this.butTeamEdit);
            this.splitContainer4.Panel1.Controls.Add(this.butTeamDel);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.listBox1);
            this.splitContainer4.Size = new System.Drawing.Size(461, 180);
            this.splitContainer4.SplitterDistance = 112;
            this.splitContainer4.TabIndex = 0;
            // 
            // butTeamDown
            // 
            this.butTeamDown.Location = new System.Drawing.Point(12, 144);
            this.butTeamDown.Name = "butTeamDown";
            this.butTeamDown.Size = new System.Drawing.Size(85, 27);
            this.butTeamDown.TabIndex = 11;
            this.butTeamDown.Text = "∨";
            this.butTeamDown.UseVisualStyleBackColor = true;
            // 
            // butTeamAdd
            // 
            this.butTeamAdd.Location = new System.Drawing.Point(12, 12);
            this.butTeamAdd.Name = "butTeamAdd";
            this.butTeamAdd.Size = new System.Drawing.Size(85, 27);
            this.butTeamAdd.TabIndex = 6;
            this.butTeamAdd.Text = "添加曲线组";
            this.butTeamAdd.UseVisualStyleBackColor = true;
            this.butTeamAdd.Click += new System.EventHandler(this.butTeamAdd_Click);
            // 
            // butTeamUp
            // 
            this.butTeamUp.Location = new System.Drawing.Point(12, 111);
            this.butTeamUp.Name = "butTeamUp";
            this.butTeamUp.Size = new System.Drawing.Size(85, 27);
            this.butTeamUp.TabIndex = 10;
            this.butTeamUp.Text = "∧";
            this.butTeamUp.UseVisualStyleBackColor = true;
            // 
            // butTeamEdit
            // 
            this.butTeamEdit.Location = new System.Drawing.Point(12, 45);
            this.butTeamEdit.Name = "butTeamEdit";
            this.butTeamEdit.Size = new System.Drawing.Size(85, 27);
            this.butTeamEdit.TabIndex = 8;
            this.butTeamEdit.Text = "修改曲线组";
            this.butTeamEdit.UseVisualStyleBackColor = true;
            this.butTeamEdit.Click += new System.EventHandler(this.butTeamAdd_Click);
            // 
            // butTeamDel
            // 
            this.butTeamDel.Location = new System.Drawing.Point(12, 78);
            this.butTeamDel.Name = "butTeamDel";
            this.butTeamDel.Size = new System.Drawing.Size(85, 27);
            this.butTeamDel.TabIndex = 7;
            this.butTeamDel.Text = "删除曲线组";
            this.butTeamDel.UseVisualStyleBackColor = true;
            this.butTeamDel.Click += new System.EventHandler(this.butTeamAdd_Click);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(345, 180);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(461, 558);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
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
            this.splitContainer2.Panel1.Controls.Add(this.butApply);
            this.splitContainer2.Panel1.Controls.Add(this.butCancel);
            this.splitContainer2.Panel1.Controls.Add(this.butOK);
            this.splitContainer2.Panel1.Controls.Add(this.GroupAdd);
            this.splitContainer2.Panel1.Controls.Add(this.GroupDel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dGV1);
            this.splitContainer2.Size = new System.Drawing.Size(719, 742);
            this.splitContainer2.SplitterDistance = 74;
            this.splitContainer2.TabIndex = 0;
            // 
            // butApply
            // 
            this.butApply.Enabled = false;
            this.butApply.Location = new System.Drawing.Point(9, 261);
            this.butApply.Name = "butApply";
            this.butApply.Size = new System.Drawing.Size(55, 33);
            this.butApply.TabIndex = 7;
            this.butApply.Text = "应用";
            this.butApply.UseVisualStyleBackColor = true;
            this.butApply.Click += new System.EventHandler(this.butApply_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(9, 314);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(55, 33);
            this.butCancel.TabIndex = 7;
            this.butCancel.Text = "取消";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butOK
            // 
            this.butOK.Location = new System.Drawing.Point(10, 261);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(55, 33);
            this.butOK.TabIndex = 6;
            this.butOK.Text = "确定";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // GroupAdd
            // 
            this.GroupAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(140)))), ((int)(((byte)(160)))));
            this.GroupAdd.Controls.Add(this.butAdd4);
            this.GroupAdd.Controls.Add(this.butAdd1);
            this.GroupAdd.Controls.Add(this.butAdd3);
            this.GroupAdd.Controls.Add(this.butAdd2);
            this.GroupAdd.Location = new System.Drawing.Point(3, 0);
            this.GroupAdd.Name = "GroupAdd";
            this.GroupAdd.Size = new System.Drawing.Size(68, 245);
            this.GroupAdd.TabIndex = 3;
            this.GroupAdd.TabStop = false;
            // 
            // butAdd4
            // 
            this.butAdd4.Enabled = false;
            this.butAdd4.Location = new System.Drawing.Point(6, 191);
            this.butAdd4.Name = "butAdd4";
            this.butAdd4.Size = new System.Drawing.Size(55, 33);
            this.butAdd4.TabIndex = 2;
            this.butAdd4.Text = "添加至第4轴>>";
            this.butAdd4.UseVisualStyleBackColor = true;
            this.butAdd4.Click += new System.EventHandler(this.butAdd1_Click);
            // 
            // butAdd1
            // 
            this.butAdd1.Enabled = false;
            this.butAdd1.Location = new System.Drawing.Point(6, 32);
            this.butAdd1.Name = "butAdd1";
            this.butAdd1.Size = new System.Drawing.Size(55, 33);
            this.butAdd1.TabIndex = 2;
            this.butAdd1.Text = "添加至第1轴>>";
            this.butAdd1.UseVisualStyleBackColor = true;
            this.butAdd1.Click += new System.EventHandler(this.butAdd1_Click);
            // 
            // butAdd3
            // 
            this.butAdd3.Enabled = false;
            this.butAdd3.Location = new System.Drawing.Point(7, 137);
            this.butAdd3.Name = "butAdd3";
            this.butAdd3.Size = new System.Drawing.Size(55, 33);
            this.butAdd3.TabIndex = 2;
            this.butAdd3.Text = "添加至第3轴>>";
            this.butAdd3.UseVisualStyleBackColor = true;
            this.butAdd3.Click += new System.EventHandler(this.butAdd1_Click);
            // 
            // butAdd2
            // 
            this.butAdd2.Enabled = false;
            this.butAdd2.Location = new System.Drawing.Point(6, 82);
            this.butAdd2.Name = "butAdd2";
            this.butAdd2.Size = new System.Drawing.Size(55, 33);
            this.butAdd2.TabIndex = 2;
            this.butAdd2.Text = "添加至第2轴>>";
            this.butAdd2.UseVisualStyleBackColor = true;
            this.butAdd2.Click += new System.EventHandler(this.butAdd1_Click);
            // 
            // GroupDel
            // 
            this.GroupDel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(140)))), ((int)(((byte)(160)))));
            this.GroupDel.Controls.Add(this.butDel);
            this.GroupDel.Controls.Add(this.butDown1);
            this.GroupDel.Controls.Add(this.butUp1);
            this.GroupDel.Location = new System.Drawing.Point(3, 0);
            this.GroupDel.Name = "GroupDel";
            this.GroupDel.Size = new System.Drawing.Size(68, 245);
            this.GroupDel.TabIndex = 0;
            this.GroupDel.TabStop = false;
            this.GroupDel.Visible = false;
            // 
            // butDel
            // 
            this.butDel.Location = new System.Drawing.Point(6, 32);
            this.butDel.Name = "butDel";
            this.butDel.Size = new System.Drawing.Size(55, 33);
            this.butDel.TabIndex = 5;
            this.butDel.Text = "删除<<";
            this.butDel.UseVisualStyleBackColor = true;
            this.butDel.Click += new System.EventHandler(this.butDel_Click);
            // 
            // butDown1
            // 
            this.butDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butDown1.Enabled = false;
            this.butDown1.Location = new System.Drawing.Point(6, 137);
            this.butDown1.Name = "butDown1";
            this.butDown1.Size = new System.Drawing.Size(55, 33);
            this.butDown1.TabIndex = 4;
            this.butDown1.Text = "∨";
            this.butDown1.UseVisualStyleBackColor = true;
            this.butDown1.Click += new System.EventHandler(this.butDown1_Click);
            // 
            // butUp1
            // 
            this.butUp1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butUp1.Enabled = false;
            this.butUp1.Location = new System.Drawing.Point(6, 82);
            this.butUp1.Name = "butUp1";
            this.butUp1.Size = new System.Drawing.Size(55, 33);
            this.butUp1.TabIndex = 3;
            this.butUp1.Text = "∧";
            this.butUp1.UseVisualStyleBackColor = true;
            this.butUp1.Click += new System.EventHandler(this.butUp1_Click);
            // 
            // dGV1
            // 
            this.dGV1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGV1.Location = new System.Drawing.Point(0, 0);
            this.dGV1.Name = "dGV1";
            this.dGV1.RowTemplate.Height = 23;
            this.dGV1.Size = new System.Drawing.Size(641, 742);
            this.dGV1.TabIndex = 1;
            this.dGV1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dGV1_CellBeginEdit);
            this.dGV1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV1_CellClick);
            this.dGV1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV1_CellDoubleClick);
            this.dGV1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGV1_CellEndEdit);
            this.dGV1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dGV1_CellValidating);
            // 
            // frmC曲线选择变量
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 742);
            this.Controls.Add(this.splitContainer1);
            this.Name = "frmC曲线选择变量";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmC曲线选择变量";
            this.Load += new System.EventHandler(this.frmC曲线选择变量_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.GroupAdd.ResumeLayout(false);
            this.GroupDel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGV1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox GroupDel;
        private System.Windows.Forms.DataGridView dGV1;
        private System.Windows.Forms.Button butDown1;
        private System.Windows.Forms.Button butUp1;
        private System.Windows.Forms.Button butAdd1;
        private System.Windows.Forms.Button butAdd4;
        private System.Windows.Forms.Button butAdd3;
        private System.Windows.Forms.Button butAdd2;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button butDel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox GroupAdd;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Button butTeamEdit;
        private System.Windows.Forms.Button butTeamDel;
        private System.Windows.Forms.Button butTeamAdd;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button butTeamDown;
        private System.Windows.Forms.Button butTeamUp;
        private System.Windows.Forms.Button butApply;
    }
}