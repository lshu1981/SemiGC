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
    public partial class frmPubInput : Form
    {
        public string sOld = "";
        public string sWithout = "";
        bool bWithout = false;
        public frmPubInput()
        {
            InitializeComponent();
        }

        public frmPubInput(string str1)
        {
            InitializeComponent();
            sOld = str1;
            this.textBox1.Text = str1;
        }

        public frmPubInput(string str1,string str2)
        {
            InitializeComponent();
            sOld = str1;
            sWithout = str2;
            bWithout = true;
            this.textBox1.Text = str1;
        }

        private void frmPubInput_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sWithout.IndexOf("{" + textBox1.Text + "}") >= 0)
            {
                MessageBox.Show("该输入已经存在，请重新输入！", "错误");
            }
            else
            {
                sOld = textBox1.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
