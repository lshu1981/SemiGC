using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PublicDll;
using System.Xml;

namespace SemiGC
{
    public partial class frmLayerManager : Form
    {
        public bool bManager;
        public frmLayerManager()
        {
            InitializeComponent();
        }

        public frmLayerManager(bool bMan)
        {
            InitializeComponent();
            bManager = bMan;
            groupInsert.Visible =! bMan;
            groupManager.Visible = bMan;
        }
        private void frmLayerManager_Load(object sender, EventArgs e)
        {
            string[] sHead = new string[] { "序号", "层名称", "时间", "渐变", "压力", "转速", "控温类型" };
            bool[] bReadOnly = new bool[] { false, false };
            CPublicDGV.InitializeDGV(dataGridView1, sHead, bReadOnly, true);
            FillDataGridView();
        }

        private void FillDataGridView()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.RowCount = frmRecipe.ListHisLayer.Count;
            for (int i = 0; i < frmRecipe.ListHisLayer.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
                for (int j = 1; j <= 5; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = frmRecipe.ListHisLayer[i].ListCell[j].StrValue;
                }
                dataGridView1.Rows[i].Cells[6].Value = frmRecipe.ListHisLayer[i].ListCell[97].StrValue;
            }
            dataGridView1.AutoResizeColumns();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int i = dataGridView1.SelectedCells[0].RowIndex;
                frmRecipe.LayerSelIndex = -1;
                try
                {
                    frmRecipe.LayerSelIndex = int.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString())-1;
                    this.Close();
                }
                catch (Exception ex)
                {
                    frmRecipe.LayerSelIndex = -1;
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("请选选一个层", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void LayDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要删除选中的层？", "删除",
             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                List<int>iRe = CPublicDGV.GetSelRow(dataGridView1);
                for (int i = iRe.Count - 1; i >= 0; i--)
                {
                    LayDelFromXML(dataGridView1.Rows[iRe[i]].Cells[1].Value.ToString());
                    dataGridView1.Rows.RemoveAt(iRe[i]);
                }
            }
        }

        private void LayDelFromXML(string sName)
        {
            string filePath =frmRecipe.sAppPath + @"\Project\Layer.xml";
            XmlDocument myxmldoc = new XmlDocument();
            myxmldoc.Load(filePath);

            string xpath = "root/LayerList";
            XmlElement myNode =(XmlElement) myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement node in myNode.ChildNodes)
            {
                if (node.GetAttribute("ID002") == sName)
                    myNode.RemoveChild(node);
            }
            myxmldoc.Save(filePath);
        }
    }
}
