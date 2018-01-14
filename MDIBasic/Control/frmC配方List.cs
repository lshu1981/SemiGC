using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PublicDll;
using LSSCADA.Database;

namespace LSSCADA.Control
{
    public partial class frmC配方List : Form
    {
        int iMode = 0; //模式  0：选择模式，1管理模式
        string[] sCol2 = new string[] { "编号", "配方名称", "开始时间", "结束时间", "状态", "总层数", "GUID" };
        bool[] bCol2 = new bool[] { true, true, true, true };
        DataTable DTValue;
        public CSelRecipe nSelRecipe;

        public frmC配方List()
        {
            InitializeComponent();
        }
        public frmC配方List(int _iMode)
        {
            InitializeComponent();
            iMode = _iMode;
            InitTimeList();
        }

        public void InitTimeList()
        {
            try
            {
                listBox1.Items.Clear();

                string sSQL = "SELECT year(DT_Start) as Y,month(DT_Start) as M,count(*) as N FROM p_processinfo group by year(DT_Start) , month(DT_Start) order by DT_Start desc;";
                DataTable ListValue = LSDatabase.GetSOEData(sSQL);
                foreach (DataRow nRow in ListValue.Rows)
                {
                    DateTime DT = new DateTime(Convert.ToInt32(nRow["Y"]), Convert.ToInt32(nRow["M"]), 1, 0, 0, 0);
                    listBox1.Items.Add(DT.ToString("yyyy年MM月") + "(" + nRow["N"] + ")");
                }
                listBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitTimeList:" + ex.Message);
            }
        }

        private void FillDGV(DateTime DTStart ,DateTime DTEnd)
        {
            string sSQL = "SELECT Name as 名称,DT_Start as 开始时间,DT_End as 结束时间 ,UserName as 操作员,Status,StepNumber as 总层数,rowguid as 唯一标示 FROM p_processinfo ";
            sSQL += "where DT_Start between '" + DTStart.ToString("yyyy-MM-dd hh:mm:ss");
            sSQL += "' and '" + DTEnd.ToString("yyyy-MM-dd HH:mm:ss") + "'order by DT_Start desc;";
            DTValue = LSDatabase.GetSOEData(sSQL);
            dGV1.DataSource = DTValue;
            dGV1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void butSelOK_Click(object sender, EventArgs e)
        {
            string sMsg = "";
            try
            {
                nSelRecipe = new CSelRecipe();
                sMsg = "配方开始时间：";
                nSelRecipe.DT_S = Convert.ToDateTime(dGV1.SelectedRows[0].Cells["开始时间"].Value.ToString());
                sMsg = "配方结束时间：";
                nSelRecipe.DT_E = Convert.ToDateTime(dGV1.SelectedRows[0].Cells["结束时间"].Value.ToString());
                sMsg = "配方名称："; 
                nSelRecipe.Name = dGV1.SelectedRows[0].Cells["名称"].Value.ToString();
                sMsg = "配方总层数：";
                nSelRecipe.iLayNum = Convert.ToInt32(dGV1.SelectedRows[0].Cells["总层数"].Value.ToString());
                sMsg = "配方唯一标示：";
                nSelRecipe.sGUID = dGV1.SelectedRows[0].Cells["唯一标示"].Value.ToString();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(sMsg + ex.Message, "错误");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str1 = listBox1.Text;
            int iY =Convert.ToInt32( str1.Substring(0, 4));
            int iM = Convert.ToInt32(str1.Substring(5, 2));
            DateTime DTS = new DateTime(iY, iM, 1, 0, 0, 0);
            FillDGV(DTS,DTS.AddMonths(1));
        }

        private void butSelCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }

    public class CSelRecipe
    {
        public DateTime DT_S;
        public DateTime DT_E;
        public string Name;
        public string sGUID;
        public int iLayNum;
    }
}
