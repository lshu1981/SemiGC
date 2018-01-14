using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSSCADA.Control
{
    public partial class frmC机械手控制 : Form
    {
        public int iMode = 0;
        
        string[] sMode = new string[] { "无","捡起","放置","传送"};
        string[] sComm = new string[] { "NULL", "PICK ", "PLACE ", "XFER " };
        string sAction = "PICK ";

        public string sSend;
        string sSend1, sSend2;
        public string sSendComm 
        {
            get { return this.sSend; }
        }

        public frmC机械手控制()
        {
            InitializeComponent();
        }

        public frmC机械手控制(int i)
        {
            InitializeComponent();
            iMode = i;
            this.Text = "机械手" + sMode[iMode] + "控制";
            sAction = sComm[iMode];
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            button1.Text = sMode[iMode];
            switch (iMode)
            {
                case 1:
                    groupBox1.Visible = true;
                    groupBox2.Visible = false;
                    break;
                case 2:
                    groupBox1.Visible = true;
                    groupBox2.Visible = false;
                    break;
                case 3:
                    groupBox1.Visible = false;
                    groupBox2.Visible = true;
                    break;
            }
        }

        private void frm机械手控制_Load(object sender, EventArgs e)
        {
            groupBox1.Location = new Point(16, 1);
        }

        private void button7_Click(object sender, EventArgs e)//取消
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)//取消
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)//捡起，放置
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button5_Click(object sender, EventArgs e)//传送
        {
            this.DialogResult = DialogResult.OK;
        }

        private void GW01_CheckedChanged(object sender, EventArgs e)//传送工位1选择
        {
            RadioButton nCon = (RadioButton)sender;
            int[] ab = new int[4]{1,1,1,1};
            sSend1 = nCon.Name.Substring(5, 1);
            switch (nCon.Name)
            {
                case "GW0101": ab = new int[] { 0, 1, 1, 0 }; GW0201.Checked = false; GW0204.Checked = false; break;
                case "GW0102": ab = new int[] { 1, 0, 1, 1 }; GW0202.Checked = false; break;
                case "GW0103": ab = new int[] { 1, 1, 0, 1 }; GW0203.Checked = false; break;
                case "GW0104": ab = new int[] { 0, 1, 1, 0 }; GW0204.Checked = false; GW0201.Checked = false; break;
            }
            GW0201.Enabled = Convert.ToBoolean(ab[0]);
            GW0202.Enabled = Convert.ToBoolean(ab[1]);
            GW0203.Enabled = Convert.ToBoolean(ab[2]);
            GW0204.Enabled = Convert.ToBoolean(ab[3]);
            if (GW0201.Checked || GW0202.Checked || GW0203.Checked || GW0204.Checked)
            {
                sSend = sAction + sSend1.ToString() + " " + sSend2.ToString();
                textBox2.Text = sSendComm;
                button5.Enabled = true;
            }
            else
            {
                sSend = "";
                textBox2.Text = sSendComm;
                button5.Enabled = false;
            }
        }

        private void GW02_CheckedChanged(object sender, EventArgs e)//传送工位2选择
        {
            button5.Enabled = true;
            RadioButton nCon = (RadioButton)sender;
            sSend2 = nCon.Name.Substring(5, 1);
            sSend = sAction + sSend1.ToString()+" " + sSend2.ToString();
            textBox2.Text = sSendComm;
        }

        private void GW_CheckedChanged(object sender, EventArgs e)//单步操作工位选择
        {
            RadioButton nCon = (RadioButton)sender;
            button1.Enabled = true;
            string str1 = "";

            switch (nCon.Name)
            {
                case "GW01": str1 = "生长室"; break;
                case "GW02": str1 = "进样室上"; break;
                case "GW03": str1 = "进样室下"; break;
                case "GW04": str1 = "层流室"; break;
            }
            sSend = sAction + nCon.Name.Substring(3, 1) +"\r\n";
            button1.Text = str1 + sMode[iMode];
            textBox1.Text = sSendComm;
        }
    }
}
