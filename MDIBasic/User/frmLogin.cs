using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace LSSCADA
{
    public partial class frmLogin : Form
    {
        CUserInfo nUserInfo;
        public Dictionary<int, string> ListUser = new Dictionary<int, string>();
        bool bLogin = true;
        public frmLogin()
        {
            InitializeComponent();
        }

        public frmLogin(CUserInfo _User,bool _bLogin)
        {
            InitializeComponent();
            nUserInfo = _User;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            ListUser = nUserInfo.GetAllUsers();
            if (ListUser != null)
            {
                foreach (KeyValuePair<int, string> kvp in ListUser)
                {
                    comboBox1.Items.Add(kvp.Value);
                }
                if(nUserInfo.OldUserName!=null)
                    comboBox1.Text = nUserInfo.OldUserName;
            }
            bLogin = _bLogin;
            if (!_bLogin)
                this.Text = "确认";
            this.ActiveControl = textPassword;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                string sRe = "";
                if (nUserInfo.Login(comboBox1.Text, textPassword.Text, ref sRe))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }      
    }
}
