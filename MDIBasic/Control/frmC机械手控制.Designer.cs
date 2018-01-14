namespace LSSCADA.Control
{
    partial class frmC机械手控制
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.GW01 = new System.Windows.Forms.RadioButton();
            this.GW02 = new System.Windows.Forms.RadioButton();
            this.GW03 = new System.Windows.Forms.RadioButton();
            this.GW04 = new System.Windows.Forms.RadioButton();
            this.button6 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.GW0201 = new System.Windows.Forms.RadioButton();
            this.GW0202 = new System.Windows.Forms.RadioButton();
            this.GW0203 = new System.Windows.Forms.RadioButton();
            this.GW0204 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.GW0101 = new System.Windows.Forms.RadioButton();
            this.GW0102 = new System.Windows.Forms.RadioButton();
            this.GW0103 = new System.Windows.Forms.RadioButton();
            this.GW0104 = new System.Windows.Forms.RadioButton();
            this.button7 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(16, 190);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(467, 180);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "托盘操作";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(275, 26);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(133, 26);
            this.textBox1.TabIndex = 26;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.GW01);
            this.groupBox5.Controls.Add(this.GW02);
            this.groupBox5.Controls.Add(this.GW03);
            this.groupBox5.Controls.Add(this.GW04);
            this.groupBox5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox5.Location = new System.Drawing.Point(68, 27);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(175, 140);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "请选择工位";
            // 
            // GW01
            // 
            this.GW01.AutoSize = true;
            this.GW01.Location = new System.Drawing.Point(8, 27);
            this.GW01.Margin = new System.Windows.Forms.Padding(4);
            this.GW01.Name = "GW01";
            this.GW01.Size = new System.Drawing.Size(74, 20);
            this.GW01.TabIndex = 13;
            this.GW01.TabStop = true;
            this.GW01.Text = "生长室";
            this.GW01.UseVisualStyleBackColor = true;
            this.GW01.CheckedChanged += new System.EventHandler(this.GW_CheckedChanged);
            // 
            // GW02
            // 
            this.GW02.AutoSize = true;
            this.GW02.Location = new System.Drawing.Point(8, 55);
            this.GW02.Margin = new System.Windows.Forms.Padding(4);
            this.GW02.Name = "GW02";
            this.GW02.Size = new System.Drawing.Size(90, 20);
            this.GW02.TabIndex = 14;
            this.GW02.TabStop = true;
            this.GW02.Text = "进样室上";
            this.GW02.UseVisualStyleBackColor = true;
            this.GW02.CheckedChanged += new System.EventHandler(this.GW_CheckedChanged);
            // 
            // GW03
            // 
            this.GW03.AutoSize = true;
            this.GW03.Location = new System.Drawing.Point(8, 83);
            this.GW03.Margin = new System.Windows.Forms.Padding(4);
            this.GW03.Name = "GW03";
            this.GW03.Size = new System.Drawing.Size(90, 20);
            this.GW03.TabIndex = 15;
            this.GW03.TabStop = true;
            this.GW03.Text = "进样室下";
            this.GW03.UseVisualStyleBackColor = true;
            this.GW03.CheckedChanged += new System.EventHandler(this.GW_CheckedChanged);
            // 
            // GW04
            // 
            this.GW04.AutoSize = true;
            this.GW04.Location = new System.Drawing.Point(8, 111);
            this.GW04.Margin = new System.Windows.Forms.Padding(4);
            this.GW04.Name = "GW04";
            this.GW04.Size = new System.Drawing.Size(74, 20);
            this.GW04.TabIndex = 16;
            this.GW04.TabStop = true;
            this.GW04.Text = "层流室";
            this.GW04.UseVisualStyleBackColor = true;
            this.GW04.CheckedChanged += new System.EventHandler(this.GW_CheckedChanged);
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.SystemColors.Control;
            this.button6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.Location = new System.Drawing.Point(275, 121);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(133, 37);
            this.button6.TabIndex = 14;
            this.button6.Text = "取消";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(275, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 37);
            this.button1.TabIndex = 9;
            this.button1.Text = "进样室上捡起";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.button7);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(16, 1);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(467, 180);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "托盘传送操作";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(357, 26);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(86, 26);
            this.textBox2.TabIndex = 27;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.GW0201);
            this.groupBox4.Controls.Add(this.GW0202);
            this.groupBox4.Controls.Add(this.GW0203);
            this.groupBox4.Controls.Add(this.GW0204);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(170, 27);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(164, 141);
            this.groupBox4.TabIndex = 25;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "放置工位";
            // 
            // GW0201
            // 
            this.GW0201.AutoSize = true;
            this.GW0201.Enabled = false;
            this.GW0201.Location = new System.Drawing.Point(8, 27);
            this.GW0201.Margin = new System.Windows.Forms.Padding(4);
            this.GW0201.Name = "GW0201";
            this.GW0201.Size = new System.Drawing.Size(74, 20);
            this.GW0201.TabIndex = 19;
            this.GW0201.TabStop = true;
            this.GW0201.Text = "生长室";
            this.GW0201.UseVisualStyleBackColor = true;
            this.GW0201.CheckedChanged += new System.EventHandler(this.GW02_CheckedChanged);
            // 
            // GW0202
            // 
            this.GW0202.AutoSize = true;
            this.GW0202.Enabled = false;
            this.GW0202.Location = new System.Drawing.Point(8, 55);
            this.GW0202.Margin = new System.Windows.Forms.Padding(4);
            this.GW0202.Name = "GW0202";
            this.GW0202.Size = new System.Drawing.Size(90, 20);
            this.GW0202.TabIndex = 20;
            this.GW0202.TabStop = true;
            this.GW0202.Text = "进样室上";
            this.GW0202.UseVisualStyleBackColor = true;
            this.GW0202.CheckedChanged += new System.EventHandler(this.GW02_CheckedChanged);
            // 
            // GW0203
            // 
            this.GW0203.AutoSize = true;
            this.GW0203.Enabled = false;
            this.GW0203.Location = new System.Drawing.Point(8, 83);
            this.GW0203.Margin = new System.Windows.Forms.Padding(4);
            this.GW0203.Name = "GW0203";
            this.GW0203.Size = new System.Drawing.Size(90, 20);
            this.GW0203.TabIndex = 21;
            this.GW0203.TabStop = true;
            this.GW0203.Text = "进样室下";
            this.GW0203.UseVisualStyleBackColor = true;
            this.GW0203.CheckedChanged += new System.EventHandler(this.GW02_CheckedChanged);
            // 
            // GW0204
            // 
            this.GW0204.AutoSize = true;
            this.GW0204.Enabled = false;
            this.GW0204.Location = new System.Drawing.Point(8, 111);
            this.GW0204.Margin = new System.Windows.Forms.Padding(4);
            this.GW0204.Name = "GW0204";
            this.GW0204.Size = new System.Drawing.Size(74, 20);
            this.GW0204.TabIndex = 22;
            this.GW0204.TabStop = true;
            this.GW0204.Text = "层流室";
            this.GW0204.UseVisualStyleBackColor = true;
            this.GW0204.CheckedChanged += new System.EventHandler(this.GW02_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.GW0101);
            this.groupBox3.Controls.Add(this.GW0102);
            this.groupBox3.Controls.Add(this.GW0103);
            this.groupBox3.Controls.Add(this.GW0104);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(23, 27);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(122, 141);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "捡起工位";
            // 
            // GW0101
            // 
            this.GW0101.AutoSize = true;
            this.GW0101.Location = new System.Drawing.Point(8, 27);
            this.GW0101.Margin = new System.Windows.Forms.Padding(4);
            this.GW0101.Name = "GW0101";
            this.GW0101.Size = new System.Drawing.Size(74, 20);
            this.GW0101.TabIndex = 13;
            this.GW0101.TabStop = true;
            this.GW0101.Text = "生长室";
            this.GW0101.UseVisualStyleBackColor = true;
            this.GW0101.CheckedChanged += new System.EventHandler(this.GW01_CheckedChanged);
            // 
            // GW0102
            // 
            this.GW0102.AutoSize = true;
            this.GW0102.Location = new System.Drawing.Point(8, 55);
            this.GW0102.Margin = new System.Windows.Forms.Padding(4);
            this.GW0102.Name = "GW0102";
            this.GW0102.Size = new System.Drawing.Size(90, 20);
            this.GW0102.TabIndex = 14;
            this.GW0102.TabStop = true;
            this.GW0102.Text = "进样室上";
            this.GW0102.UseVisualStyleBackColor = true;
            this.GW0102.CheckedChanged += new System.EventHandler(this.GW01_CheckedChanged);
            // 
            // GW0103
            // 
            this.GW0103.AutoSize = true;
            this.GW0103.Location = new System.Drawing.Point(8, 83);
            this.GW0103.Margin = new System.Windows.Forms.Padding(4);
            this.GW0103.Name = "GW0103";
            this.GW0103.Size = new System.Drawing.Size(90, 20);
            this.GW0103.TabIndex = 15;
            this.GW0103.TabStop = true;
            this.GW0103.Text = "进样室下";
            this.GW0103.UseVisualStyleBackColor = true;
            this.GW0103.CheckedChanged += new System.EventHandler(this.GW01_CheckedChanged);
            // 
            // GW0104
            // 
            this.GW0104.AutoSize = true;
            this.GW0104.Location = new System.Drawing.Point(8, 111);
            this.GW0104.Margin = new System.Windows.Forms.Padding(4);
            this.GW0104.Name = "GW0104";
            this.GW0104.Size = new System.Drawing.Size(74, 20);
            this.GW0104.TabIndex = 16;
            this.GW0104.TabStop = true;
            this.GW0104.Text = "层流室";
            this.GW0104.UseVisualStyleBackColor = true;
            this.GW0104.CheckedChanged += new System.EventHandler(this.GW01_CheckedChanged);
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.SystemColors.Control;
            this.button7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.Location = new System.Drawing.Point(357, 121);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(86, 37);
            this.button7.TabIndex = 23;
            this.button7.Text = "取消";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.SystemColors.Control;
            this.button5.Enabled = false;
            this.button5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.Location = new System.Drawing.Point(357, 63);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(86, 39);
            this.button5.TabIndex = 12;
            this.button5.Text = "开始传送";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // frm机械手控制
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 188);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frm机械手控制";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "机械手控制";
            this.Load += new System.EventHandler(this.frm机械手控制_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton GW0102;
        private System.Windows.Forms.RadioButton GW0101;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.RadioButton GW0204;
        private System.Windows.Forms.RadioButton GW0203;
        private System.Windows.Forms.RadioButton GW0202;
        private System.Windows.Forms.RadioButton GW0201;
        private System.Windows.Forms.RadioButton GW0104;
        private System.Windows.Forms.RadioButton GW0103;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton GW01;
        private System.Windows.Forms.RadioButton GW02;
        private System.Windows.Forms.RadioButton GW03;
        private System.Windows.Forms.RadioButton GW04;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}