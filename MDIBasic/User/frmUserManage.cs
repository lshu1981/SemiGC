using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PublicDll;
using System.Collections;

namespace LSSCADA
{
    public partial class frmUserManage : Form
    {
        string[] sCol2 = new string[] { "工号", "用户名称", "用户角色" };
        bool[] bCol2 = new bool[] { true, true, true, true };
        CUserInfo nUserInfo;
        string sSelName = "";
        int sSelID = 10001;
        string sSelRole = "";
        public frmUserManage()
        {
            InitializeComponent();
        }

        public frmUserManage(CUserInfo _User)
        {
            InitializeComponent();
            nUserInfo = _User;
            CPublicDGV.InitializeDGV(dGV1, sCol2, bCol2, true);
            dGV1.MultiSelect = false;
            FillGrid();
        }

        private void FillGrid()
        {
            dGV1.Rows.Clear();
            ArrayList obj = nUserInfo.GetAllUsersArray();
            if (obj != null)
            {
                foreach (string[] rowArray in obj)
                {
                    dGV1.Rows.Add(rowArray);
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            int ID = 10000;
            try
            {
                for (int i = 0; i < dGV1.RowCount; i++)
                {
                    if (dGV1.Rows[i].Cells[0].Value != null)
                    {
                        ID = Math.Max(ID, Convert.ToInt32(dGV1.Rows[i].Cells[0].Value.ToString()));
                    }
                }
            }
            catch (Exception ex)
            { }

            frmUserAdd fAdd = new frmUserAdd(nUserInfo, true, ID + 1,"","");
            fAdd.ShowDialog();
            if (fAdd.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                FillGrid();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            frmUserAdd fAdd = new frmUserAdd(nUserInfo,false, sSelID,sSelName,sSelRole);
            fAdd.ShowDialog();
            if (fAdd.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                FillGrid();
            }
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            try
            {
                if (sSelName.Length <= 0)
                    return;
                if (sSelName == "Administrator")
                {
                    MessageBox.Show("不能删除系统管理员账号", "错误");
                    return;
                }
                if (MessageBox.Show("是否删除用户["+ sSelName+ "]？", "删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    string sRe = "";
                    if (nUserInfo.DelUser(sSelName, ref sRe))
                    {
                        FillGrid();
                    }
                    else
                    {   
                        MessageBox.Show(sRe, "删除");
                    }
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dGV1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                bool bSel = false;

                if (dGV1.Rows[dGV1.SelectedCells[0].RowIndex].Cells[1].Value != null)
                {
                    sSelID =Convert.ToInt32( dGV1.Rows[dGV1.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
                    sSelName = dGV1.Rows[dGV1.SelectedCells[0].RowIndex].Cells[1].Value.ToString();
                    sSelRole = dGV1.Rows[dGV1.SelectedCells[0].RowIndex].Cells[2].Value.ToString();
                    this.Text = sSelName;
                    if (sSelName.Length > 0)
                    {
                        bSel = true;
                    }
                    if (sSelName == "Administrator")
                        bSel = false;
                }
                else
                {
                    sSelName = "";
                }

                buttonDel.Enabled = bSel;
                buttonEdit.Enabled = bSel;
            }
            catch (Exception ex)
            { }
        }
    }
}
