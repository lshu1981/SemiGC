namespace LSSCADA.Control
{
    partial class frmC生长室维护
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button002 = new System.Windows.Forms.Button();
            this.button014 = new System.Windows.Forms.Button();
            this.button013 = new System.Windows.Forms.Button();
            this.button015 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.num1033 = new System.Windows.Forms.NumericUpDown();
            this.num1032 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gBSet = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num1033)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num1032)).BeginInit();
            this.gBSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(140)))), ((int)(((byte)(160)))));
            this.groupBox1.Controls.Add(this.gBSet);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button002);
            this.groupBox1.Controls.Add(this.button014);
            this.groupBox1.Controls.Add(this.button013);
            this.groupBox1.Controls.Add(this.button015);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.Red;
            this.groupBox1.Location = new System.Drawing.Point(13, 372);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(570, 347);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "生长室维护";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.ForestGreen;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(164, 278);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 30);
            this.button1.TabIndex = 903;
            this.button1.Text = "下发参数设置";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.ForestGreen;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(395, 278);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(160, 30);
            this.button3.TabIndex = 902;
            this.button3.Text = "确定";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button002
            // 
            this.button002.BackColor = System.Drawing.Color.Coral;
            this.button002.ForeColor = System.Drawing.Color.White;
            this.button002.Location = new System.Drawing.Point(20, 153);
            this.button002.Name = "button002";
            this.button002.Size = new System.Drawing.Size(120, 30);
            this.button002.TabIndex = 901;
            this.button002.Text = "N2 Idle";
            this.button002.UseVisualStyleBackColor = false;
            this.button002.Click += new System.EventHandler(this.buttonSend1002);
            // 
            // button014
            // 
            this.button014.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button014.ForeColor = System.Drawing.Color.White;
            this.button014.Location = new System.Drawing.Point(20, 117);
            this.button014.Name = "button014";
            this.button014.Size = new System.Drawing.Size(120, 30);
            this.button014.TabIndex = 900;
            this.button014.Text = "回充N2";
            this.button014.UseVisualStyleBackColor = false;
            this.button014.Click += new System.EventHandler(this.buttonSend1002);
            // 
            // button013
            // 
            this.button013.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button013.ForeColor = System.Drawing.Color.White;
            this.button013.Location = new System.Drawing.Point(20, 81);
            this.button013.Name = "button013";
            this.button013.Size = new System.Drawing.Size(120, 30);
            this.button013.TabIndex = 899;
            this.button013.Text = "真空检漏";
            this.button013.UseVisualStyleBackColor = false;
            this.button013.Click += new System.EventHandler(this.buttonSend1002);
            // 
            // button015
            // 
            this.button015.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button015.ForeColor = System.Drawing.Color.White;
            this.button015.Location = new System.Drawing.Point(20, 45);
            this.button015.Name = "button015";
            this.button015.Size = new System.Drawing.Size(120, 30);
            this.button015.TabIndex = 898;
            this.button015.Text = "泵吹扫";
            this.button015.UseVisualStyleBackColor = false;
            this.button015.Click += new System.EventHandler(this.buttonSend1002);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkRed;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(20, 243);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 30);
            this.button2.TabIndex = 897;
            this.button2.Text = "停止";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBox2.Location = new System.Drawing.Point(282, 38);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(83, 29);
            this.textBox2.TabIndex = 895;
            this.textBox2.Text = "9";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(275, 15);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 16);
            this.label5.TabIndex = 894;
            this.label5.Text = "当前吹扫次数";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.textBox1.Location = new System.Drawing.Point(164, 45);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(391, 137);
            this.textBox1.TabIndex = 893;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(161, 26);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 16);
            this.label4.TabIndex = 892;
            this.label4.Text = "信息栏";
            // 
            // num1033
            // 
            this.num1033.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.num1033.Location = new System.Drawing.Point(147, 39);
            this.num1033.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num1033.Name = "num1033";
            this.num1033.Size = new System.Drawing.Size(81, 29);
            this.num1033.TabIndex = 889;
            this.num1033.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num1033.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.num1033.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.num1033.ValueChanged += new System.EventHandler(this.num1033_ValueChanged);
            // 
            // num1032
            // 
            this.num1032.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.num1032.Location = new System.Drawing.Point(12, 39);
            this.num1032.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num1032.Name = "num1032";
            this.num1032.Size = new System.Drawing.Size(81, 29);
            this.num1032.TabIndex = 30;
            this.num1032.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.num1032.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.num1032.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num1032.ValueChanged += new System.EventHandler(this.num1032_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(141, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "吹扫时间(s)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "吹扫次数";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // gBSet
            // 
            this.gBSet.Controls.Add(this.label2);
            this.gBSet.Controls.Add(this.label1);
            this.gBSet.Controls.Add(this.num1032);
            this.gBSet.Controls.Add(this.num1033);
            this.gBSet.Controls.Add(this.label5);
            this.gBSet.Controls.Add(this.textBox2);
            this.gBSet.Location = new System.Drawing.Point(164, 185);
            this.gBSet.Name = "gBSet";
            this.gBSet.Size = new System.Drawing.Size(391, 77);
            this.gBSet.TabIndex = 3;
            this.gBSet.TabStop = false;
            this.gBSet.Visible = false;
            // 
            // frmC生长室维护
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1280, 732);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmC生长室维护";
            this.Text = "frmC生长室";
            this.Resize += new System.EventHandler(this.frmC生长室维护_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num1033)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num1032)).EndInit();
            this.gBSet.ResumeLayout(false);
            this.gBSet.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button002;
        private System.Windows.Forms.Button button014;
        private System.Windows.Forms.Button button013;
        private System.Windows.Forms.Button button015;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num1033;
        private System.Windows.Forms.NumericUpDown num1032;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox gBSet;


    }
}