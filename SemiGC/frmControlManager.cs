using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PublicDll;
using System.Collections;

namespace SemiGC
{
    public partial class frmControlManager : Form
    {
        private bool bSave = false;
        private string sOld = "";
        public CLayer LayerMod = new CLayer();

        string[] sCol2 = new string[] { "序号", "名称","关联变量", "项类型", "数据类型", "缺省值", "变比", "值范围" };
        bool[] bCol2 = new bool[] { true, true, true, true, true };

        public frmControlManager()
        {
            InitializeComponent();
        }

        public frmControlManager(CLayer _LayerMod)
        {
            InitializeComponent();
            LayerMod = _LayerMod;
        }

        private void frmControlManager_Load(object sender, EventArgs e)
        {
            FillCol(dGV1);
            FillRow(dGV1);
        }

        private void FillCol(DataGridView dgv)
        {
            CPublicDGV.InitializeDGV(dGV1, sCol2, bCol2, true);

            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            for (int i = 0; i < dGV1.ColumnCount; i++)
            {
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void FillRow(DataGridView dgv)
        {
            dgv.Rows.Clear();

            ArrayList obj = new ArrayList();
            string[] str1;
            for (int i = 0; i < LayerMod.ListCell.Count; i++)
            {
                str1 = new string[dGV1.ColumnCount];
                int k = 0;
                str1[k++] = i.ToString();
                str1[k++] = LayerMod.ListCell[i].Name;
                str1[k++] = LayerMod.ListCell[i].Linker;//关联变量
                str1[k++] = LayerMod.ListCell[i].CellType.ToString();//项类型
                str1[k++] = LayerMod.ListCell[i].ValueType;//数据类型
                str1[k++] = LayerMod.ListCell[i].StrValue;//缺省值
                str1[k++] = LayerMod.ListCell[i].RatioValue;
                str1[k++] = LayerMod.ListCell[i].sRange;
                obj.Add(str1);

            }
            foreach (string[] rowArray in obj)
            {
                dGV1.Rows.Add(rowArray);
            }

            for (int i = 0; i < LayerMod.ListCell.Count; i++)
            {
                if (LayerMod.ListCell[i].CellType == ECellType.GC_head)
                {
                    dgv.Rows[i].DefaultCellStyle.Font = new Font(dgv.Font.FontFamily, dgv.Font.Size + 2, FontStyle.Bold);
                    dgv.Rows[i].ReadOnly = true;
                }
                dgv.Rows[i].DefaultCellStyle.BackColor = LayerMod.ListCell[i].BackColor;
                SetCellStyle(dgv, i, LayerMod.ListCell[i].ValueType);
            }
            //dgv.AutoResizeColumns();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string sNew = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                int iCol = e.ColumnIndex;
                int iRow = e.RowIndex;
                CLayerCell nCell = LayerMod.ListCell[iRow];
                switch (iCol)
                {
                    //    0       1         2         3         4          5         6       7
                    //{ "序号", "名称","关联变量", "项类型", "数据类型", "缺省值", "变比", "值范围" };
                    case 5://缺省值
                        string message = "";
                        bool aa = nCell.CheckExcel(sNew, ref message);
                        if (!aa)
                        {
                            MessageBox.Show(message, "错误");
                            dGV1.Rows[iRow].Cells[iCol].Value = sOld;
                            return;
                        }
                        break;
                    case 6://变比
                        try
                        {
                            double f1 = Convert.ToDouble(dGV1.Rows[iRow].Cells[iCol].Value.ToString());
                        }
                        catch (Exception ex1)
                        {
                            MessageBox.Show(ex1.Message + "\r\n请重新输入", "格式错误");
                            dGV1.Rows[iRow].Cells[iCol].Value = sOld;
                            return;
                        }
                        break;
                    case 7://值范围
                        try
                        {
                            string  str1 = dGV1.Rows[iRow].Cells[iCol].Value.ToString();
                            str1 = str1.Replace('，',',');
                            str1 = str1.Replace('；', ',');
                            str1 = str1.Replace('：', ',');
                            str1 = str1.Replace(';', ',');
                            str1 = str1.Replace(':', ',');
                            string[] str2 = str1.Split(',');
                            if (str2.Length < 2)
                            {
                                MessageBox.Show("值范围不能少于2个，不同值之间请用','分开", "格式错误");
                                dGV1.Rows[iRow].Cells[iCol].Value = sOld;
                                return;
                            }
                        }
                        catch (Exception ex1)
                        {
                            MessageBox.Show(ex1.Message + "\r\n请重新输入", "格式错误");
                            dGV1.Rows[iRow].Cells[iCol].Value = sOld;
                            return;
                        }
                        break;
                    default:
                        break;
                }
                if (!bSave)
                {
                    if (sNew != sOld)
                    {
                        bSave = true;
                        toolSave.Enabled = bSave;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "格式错误");
            }
            //ECellType 
        }
       
        private void SetCellStyle(DataGridView dgv, int iRow, string ValType)
        {
            try
            {
                //    0       1         2         3         4          5         6       7
                //{ "序号", "名称","关联变量", "项类型", "数据类型", "缺省值", "变比", "值范围" };
                switch (ValType.ToUpper())
                {
                    case "STRING":
                        for (int i = 5; i <= 7; i++)
                        {
                            dgv.Rows[iRow].Cells[i].Style.ForeColor = Color.Gray;
                            dgv.Rows[iRow].Cells[i].ReadOnly = true;
                            dgv.Rows[iRow].Cells[i].Value = "";
                        }
                        break;
                    case "DATE":
                        for (int i = 6; i <= 7; i++)
                        {
                            dgv.Rows[iRow].Cells[i].Style.ForeColor = Color.Gray;
                            dgv.Rows[iRow].Cells[i].ReadOnly = true;
                            dgv.Rows[iRow].Cells[i].Value = "";
                        }
                        break;
                    case "INT":
                    case "FLOAT":
                         dgv.Rows[iRow].Cells[7].Style.ForeColor = Color.Gray;
                        dgv.Rows[iRow].Cells[7].ReadOnly = true;
                        dgv.Rows[iRow].Cells[7].Value = "";
                        break;
                    case "BOOL":
                        dgv.Rows[iRow].Cells[6].Style.ForeColor = Color.Gray;
                        dgv.Rows[iRow].Cells[6].ReadOnly = true;
                        dgv.Rows[iRow].Cells[6].Value = "";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            { }
        }
        /// <summary>保存
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolSave_Click(object sender, EventArgs e)
        {
            CLayer nLay = GetControlInfo();
            
            if(nLay != null)
            {
                LayerMod = nLay.Clone();
                LayerMod.SaveToDefault();
                bSave = false;
                toolSave.Enabled = bSave;
            }
        }

        private CLayer GetControlInfo()
        {
            CLayer nLay = new CLayer();
            nLay = LayerMod.Clone();
            nLay.Clear();
            try
            {
                for (int i = 0; i < nLay.ListCell.Count; i++)
                {
                    switch (nLay.ListCell[i].ValueType.ToUpper())
                    {
                        case "STRING":
                            continue;
                            break;
                        case "DATE":
                            string message = "";
                            string sNew = dGV1.Rows[i].Cells[5].Value.ToString();
                            bool aa = nLay.ListCell[i].CheckExcel(sNew, ref message);
                            if (!aa)
                            {
                                MessageBox.Show(message, "错误");
                                return null;
                            }
                            nLay.ListCell[i].StrValue = message;
                            break;
                        case "INT":
                        case "FLOAT":
                           message = "";
                            sNew = dGV1.Rows[i].Cells[5].Value.ToString();
                            aa = nLay.ListCell[i].CheckExcel(sNew, ref message);
                            if (!aa)
                            {
                                MessageBox.Show(message, "错误");
                                return null;
                            }
                            nLay.ListCell[i].StrValue = sNew;
                            double f1 = Convert.ToDouble(dGV1.Rows[i].Cells[6].Value.ToString());
                            nLay.ListCell[i].RatioValue = dGV1.Rows[i].Cells[6].Value.ToString();
                            break;
                        case "BOOL":
                            string str1 = dGV1.Rows[i].Cells[7].Value.ToString();
                            str1 = str1.Replace('，', ',');
                            str1 = str1.Replace('；', ',');
                            str1 = str1.Replace('：', ',');
                            str1 = str1.Replace(';', ',');
                            str1 = str1.Replace(':', ',');
                            string[] str2 = str1.Split(',');
                            if (str2.Length < 2)
                            {
                                MessageBox.Show("值范围不能少于2个，不同值之间请用','分开", "格式错误");
                                return null;
                            }
                            nLay.ListCell[i].sRange = str1;

                            message = "";
                            sNew = dGV1.Rows[i].Cells[5].Value.ToString();
                            aa = nLay.ListCell[i].CheckExcel(sNew, ref message);
                            if (!aa)
                            {
                                MessageBox.Show(message, "错误");
                                return null;
                            }
                            nLay.ListCell[i].StrValue = sNew;

                            break;
                        default:
                            break;
                    }
                }
                return nLay;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "保存失败", MessageBoxButtons.OK);
                return null;
            }
        }
        //退出
        private void toolQuit_Click(object sender, EventArgs e)
        {
            if (bSave)
            {
                if (MessageBox.Show("控制项参数进行了修改\r\n退出前是否保存当前修改？", "保存",
             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    toolSave_Click( sender,  e);
                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }

        }
        //关于
        private void toolAbout_Click(object sender, EventArgs e)
        {
            string sAbout = "控制项管理操作方式：";
            sAbout += "\r\n数据类型分为：时间、字符串、整数、浮点数";
            sAbout += "\r\n时间类型的值，初始值格式必须为XX:XX:XX";
            sAbout += "\r\n字符串类型：可以设置范围限制，范围中的值以','分开";
            sAbout += "\r\n整数和浮点数：可以设置最大值和最小值";
            sAbout += "\r\n";

            MessageBox.Show(sAbout);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                sOld = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            else
                sOld = "";
        }
    }
}
