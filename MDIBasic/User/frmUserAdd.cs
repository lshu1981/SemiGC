using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace LSSCADA
{
    public partial class frmUserAdd : Form
    {
        CUserInfo nUserInfo;
        bool bAdd = true;
        public frmUserAdd()
        {
            InitializeComponent();
        }

        public frmUserAdd(CUserInfo _user,bool _bAdd,int ID,string sName,string sRole)
        {
            InitializeComponent();
            nUserInfo = _user;
            ArrayList ListRole = nUserInfo.GetAllRole();
            if (ListRole != null)
            {
                foreach (string rowArray in ListRole)
                {
                    comboBox1.Items.Add(rowArray);
                }
            }
            bAdd = _bAdd;
            if (_bAdd)
            {
                comboBox1.SelectedIndex = 0;
                textUserID.Text = ID.ToString();
                this.Text = "添加用户";
            }
            else
            {
                textUserName.ReadOnly = true;
                textUserID.Text = ID.ToString();
                textUserName.Text = sName;
                comboBox1.Text = sRole;
                this.Text = "修改用户";
            }
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
                if (textUserName.Text == "")
                {
                    MessageBox.Show("用户名不能为空！", "错误");
                    return; 
                }
                if (textNew1.Text == "")
                {
                    MessageBox.Show("密码不能为空！", "错误");
                    return;
                }
                if (comboBox1.Text == "" || comboBox1.Text == null)
                {
                    MessageBox.Show("请选择用户角色类型！", "错误");
                    return;                    
                }
                string sRe = "";
                if (bAdd)
                {
                    if (nUserInfo.AddUser(textUserID.Text, textUserName.Text, comboBox1.Text, textNew1.Text, ref sRe))
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show(sRe, "错误");
                    }
                }
                else
                {
                    if (nUserInfo.EditUser(textUserName.Text, comboBox1.Text, textNew1.Text, ref sRe))
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show(sRe, "错误");
                    }
                }

              /*  if (nUserInfo.ChangePassword(textUserName.Text, textPassword.Text, textNew1.Text, ref sRe))
                {
                    this.Close();
                    return;
                }
                else
                {
                    MessageBox.Show(sRe, "错误");
                }*/
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
