using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace SemiGC
{
    public partial class frmSubManager : Form
    {
        public string sSubSelName = "";
        bool bEdit = false;
        public frmSubManager()
        {
            InitializeComponent();
        }

        public frmSubManager(bool _bEdit)
        {
            InitializeComponent();
            bEdit = _bEdit;
            butDel.Visible = bEdit;
            if (bEdit)
            {
                butOK.Text = "保存";
                butOK.Enabled = false;
                butCancel.Text = "关闭";
            }
            else
            {
                butOK.Text = "确定";
                butCancel.Text = "取消";
            }
        }

        private void frmSubManager_Load(object sender, EventArgs e)
        {
            InitializeDataGridView();
            FillDataGridView();
        }

        private void InitializeDataGridView()
        {
            // Create an unbound DataGridView by declaring a column count.
            dataGridView1.ColumnCount = 4;
            dataGridView1.ColumnHeadersVisible = true;

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            // Resize the height of the column headers. 
            dataGridView1.AutoResizeColumnHeadersHeight();

            // Set the column header names.
            dataGridView1.Columns[0].Name = "序号";
            dataGridView1.Columns[1].Name = "名称";
            dataGridView1.Columns[2].Name = "描述";
            dataGridView1.Columns[3].Name = "层列表";
        }

        private void FillDataGridView()
        {
            dataGridView1.Rows.Clear();
            if (frmRecipe.ListSubProgram.Count == 0)
                return;
            dataGridView1.RowCount = frmRecipe.ListSubProgram.Count;

            for (int i = 0; i < frmRecipe.ListSubProgram.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();

                dataGridView1.Rows[i].Cells[1].Value = frmRecipe.ListSubProgram[i].Name;
                dataGridView1.Rows[i].Cells[2].Value = frmRecipe.ListSubProgram[i].Desc;
                dataGridView1.Rows[i].Cells[3].Value = frmRecipe.ListSubProgram[i].sLayerList;
            }
            dataGridView1.AutoResizeColumns();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (bEdit)//保存
            { SaveToXML(); }
            else
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    try
                    {
                        sSubSelName = dataGridView1.SelectedRows[0].Cells["名称"].Value.ToString();
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("请选选一个层", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void butDel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                    butOK.Enabled = true;
                }
            }
        }

        private void SaveToXML()
        {
            if (MessageBox.Show("是否保存子程序列表？", "保存",
             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                return;
            }
            frmRecipe.ListSubProgram.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                CSubProgram newSub = new CSubProgram();
                newSub.Name = row.Cells["名称"].Value.ToString();
                newSub.Desc = row.Cells["描述"].Value.ToString();
                newSub.sLayerList = row.Cells["层列表"].Value.ToString();
                frmRecipe.ListSubProgram.Add(newSub);
                
            }

            string filePath = frmRecipe.sAppPath + @"\Project\Layer.xml";
            XmlDocument myxmldoc = new XmlDocument();
            myxmldoc.Load(filePath);

            string xpath = "root/SubProgramList";
            XmlElement ListNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            while (ListNode.ChildNodes.Count > 0)
            {
                ListNode.RemoveChild(ListNode.FirstChild);
            }

            foreach (CSubProgram newSub in frmRecipe.ListSubProgram)
            {
                XmlElement nLayNode = myxmldoc.CreateElement("SubProgram"); // 创建根节点album
                nLayNode.SetAttribute("Name", newSub.Name);
                nLayNode.SetAttribute("Desc", newSub.Desc);
                nLayNode.SetAttribute("sLayerList", newSub.sLayerList);
                ListNode.AppendChild(nLayNode);
            }
            myxmldoc.Save(filePath);
            MessageBox.Show("保存成功", "成功");
        }
    }
}
