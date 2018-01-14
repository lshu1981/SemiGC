using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSSCADA
{
    public partial class frmUserPassword : Form
    {
        CUserInfo nUserInfo;
        public frmUserPassword()
        {
            InitializeComponent();
        }

        public frmUserPassword(CUserInfo _User)
        {
            InitializeComponent();
            nUserInfo = _User;
            textUserName.Text = nUserInfo.UserName;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (textNew1.Text != textNew2.Text)
                {
                    MessageBox.Show("两次输入的密码不一致！", "错误");
                    return;
                }
                string sRe = "";
                if (nUserInfo.ChangePassword(textUserName.Text, textPassword.Text, textNew1.Text, ref sRe))
                {
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show(sRe, "错误");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void buttonCal_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
