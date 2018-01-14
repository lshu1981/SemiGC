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
    public partial class frmC密码输入 : Form
    {
        string Password = "";

        public frmC密码输入()
        {
            InitializeComponent();
        }

        public frmC密码输入(string str1)
        {
            InitializeComponent();
            Password = str1;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textPassword.Text == Password)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                MessageBox.Show("密码错误，请重新输入!");
                textPassword.Text = "";
            }
        }

        private void buttonCal_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
