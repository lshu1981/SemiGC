using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Xml;
using PublicDll;

namespace SemiGC
{
    public partial class frmRecipe : Form
    {
        public CLayer LayerMod = new CLayer(); 
        public static List<CLayer> ListHisLayer = new List<CLayer>();
        public static int LayerSelIndex = -1;
        public static string sAppPath = "";
        public static List<CSubProgram> ListSubProgram = new List<CSubProgram>();
        public static int iCycle = -1;

        public CProgram pProgram = new CProgram();

        private bool bSave = false;
        private bool bOpen = false;
        private string sOld = "";

        private bool bOnline = false;
        public frmRecipe()
        {
            InitializeComponent();
            sAppPath = AppDomain.CurrentDomain.BaseDirectory;
            sAppPath = System.Windows.Forms.Application.StartupPath;
        }

        public frmRecipe(CProgram pShowPro,string _AppPath)
        {
            InitializeComponent();
            sAppPath = _AppPath;
            pProgram = pShowPro.Clone();
            bOnline = true;
            //menuStrip2.Visible = false;
            //SaveAstool.Visible = false;
            Newtool.Visible = false;
            Opentool.Visible = false;
            bOpen = true;
            bSave = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            GetListLayer();//读取Layer.xml，获取层库信息
            InitDVG(dGV1);

            UpdateDGV(dGV1);
        }
        /// <summary>读取Layer.xml，获取层库信息
        /// 读取Layer.xml，获取层库信息
        /// </summary>
        private void GetListLayer()
        {
            //层模板，用于初始化层的控制项
            LayerMod.InitFromXml(sAppPath);
  
            string filePath = sAppPath + @"\Project\Layer.xml";
            XmlDocument myxmldoc = new XmlDocument();
            myxmldoc.Load(filePath);

            ListHisLayer.Clear();
            string xpath = "root/LayerList/Layer";
            XmlNodeList mynodes = myxmldoc.SelectNodes(xpath);
            foreach (XmlElement node in mynodes)
            {
                CLayer cl = LayerMod.Clone();
                cl.LoadFromXML(node);
                ListHisLayer.Add(cl);
            }

            ListSubProgram.Clear();
            xpath = "root/SubProgramList/SubProgram";
            mynodes = myxmldoc.SelectNodes(xpath);
            foreach (XmlElement node in mynodes)
            {
                CSubProgram cl = new CSubProgram();
                cl.LoadFromXML(node);
                ListSubProgram.Add(cl);
            }
        }

        /// <summary>打开Excel文件,从Sheet1表中读取配方实例
        /// 打开Excel文件,从Sheet1表中读取配方实例
        /// </summary>
        /// <param name="filePath">Excel文件全路径</param>
        /// <param name="filePath1">程序目录</param>
        /// <param name="pPro">返回的配方实例</param>
        public static void OpenExcel(string filePath,string filePath1,ref CProgram pPro)
        {
            pPro = new CProgram();
            Cursor.Current = Cursors.WaitCursor;
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source = " + filePath + ";Extended Properties ='Excel 8.0;HDR=YES;IMEX=1'";
            OleDbConnection conn = new OleDbConnection(strConn);
            try
            {
                conn.Open();
                string strExcel = "";
                OleDbDataAdapter myCommand = null;
                DataSet ds = null;
                strExcel = "select * from [Sheet1$]";
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                ds = new DataSet();

                myCommand.Fill(ds, "table1");
                pPro.fileInfo = new FileInfo(filePath);
                pPro.ListLayer = new List<CLayer>();
                CLayer newLay = new CLayer();
                
                newLay.InitFromXml(filePath1);
                for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
                {
                    CLayer nLay = newLay.Clone();
                    nLay.Clear();
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        nLay.ListCell[j].StrValue = ds.Tables[0].Rows[j][i].ToString();
                    }
                    if(nLay.ListCell[2].lngValue!=null && nLay.ListCell[2].lngValue[0]>0)
                        pPro.ListLayer.Add(nLay);
                }
                pPro.LoadRulesFromXML(filePath1);
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("frmMain.OpenExcel:" + ex.Message);
                conn.Close();
            }
            Cursor.Current = Cursors.Default; 
        }
        private void UpdateDGV(DataGridView dgv)//重新填充dataGridView表格
        {
            //dgv.DataSource = ds.Tables[0];
            Savetool.Enabled = false;
            SaveAstool.Enabled = true;
            bSave = false;

            string sTitle = "配方编辑(";
            if (bOnline)
                sTitle = "配方在线编辑(";
            while (dgv.Columns.Count > 1)
            {
                dgv.Columns.RemoveAt(1);
            }
            if (pProgram == null)
                return;
            if (pProgram.fileInfo != null)
                this.Text = sTitle + pProgram.fileInfo.FullName + ")";
            else
                this.Text = "配方编辑(*)";
            dgv.ColumnCount = pProgram.ListLayer.Count + 1;
            for (int i = 1; i <= pProgram.ListLayer.Count; i++)
            {
                dgv.Columns[i].Name = i.ToString();
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < LayerMod.ListCell.Count; i++)
            {
                for (int j = 1; j <= pProgram.ListLayer.Count; j++)
                {
                    dgv.Rows[i].Cells[j].Value = pProgram.ListLayer[j - 1].ListCell[i].StrValue;
                }
            }
            dgv.AutoResizeColumns();
        }
        private void InitDVG(DataGridView dgv)//初始化表格第一列
        {
            while (dgv.Columns.Count > 0)
            {
                dgv.Columns.RemoveAt(0);
            }
            dgv.ColumnCount = 1;
            dgv.RowCount = LayerMod.ListCell.Count;
            dgv.ColumnHeadersVisible = true;

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new System.Drawing.Font("Verdana", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            // Resize the height of the column headers. 
            dgv.AutoResizeColumnHeadersHeight();
            // Set the column header names.
            dgv.Columns[0].Name = "管理层";
            dgv.Columns[0].ReadOnly = true;
            dgv.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;

            for (int i = 0; i < LayerMod.ListCell.Count; i++)
            {
                string str1 = CStrPublicFun.Get2StrTo1(LayerMod.ListCell[i].Name, LayerMod.ListCell[i].Linker, 40, ' ');
                dgv.Rows[i].Cells[0].Value = str1.Trim();
                //dgv.Rows[i].Cells[1].Value = LayerMod.ListCell[i].Linker;
            }
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            for (int i = 0; i < LayerMod.ListIndex.Count - 1; i++)
            {
                LayerMod.ListCell[LayerMod.ListIndex[i]].BackColor = Color.Gold;
                dgv.Rows[LayerMod.ListIndex[i]].DefaultCellStyle.BackColor = Color.Gold;
                dgv.Rows[LayerMod.ListIndex[i]].ReadOnly = true;
                dgv.Rows[LayerMod.ListIndex[i]].DefaultCellStyle.Font = new System.Drawing.Font(dgv.Font.FontFamily, dgv.Font.Size + 2, FontStyle.Bold);
                for (int j = LayerMod.ListIndex[i] + 1; j < LayerMod.ListIndex[i + 1]; j++)
                {
                    LayerMod.ListCell[j].BackColor = CExcel.BlackColor[i];
                    dgv.Rows[j].DefaultCellStyle.BackColor = CExcel.BlackColor[i];
                }
            }
            dgv.Columns[0].Frozen = true;
            dgv.AutoResizeColumns();
        }
        
        private CProgram GetProgram(DataGridView dgv,ref bool bErr)//从表格获取配方类实例
        {
            CProgram nPro = new CProgram();
            bErr = false;
            try
            {
                for (int i = 1; i < dgv.Columns.Count; i++)
                {
                    string aa ="";
                    if(dgv.Rows[1].Cells[i].Value !=null)
                        aa = dgv.Rows[1].Cells[i].Value.ToString();
                    if (string.IsNullOrEmpty(aa))
                    {
                        string sShow = "第" + i.ToString() + "列层名称为空\r\n是否继续？";
                        if (MessageBox.Show(sShow, "空", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            return null;
                        }
                    }
                    CLayer nLay = LayerMod.Clone();
                    nLay.Clear();
                    nLay.Name = aa;

                    string message = "";
                    bool baa = true;

                    for (int j = 0; j < dgv.Rows.Count; j++)
                    {
                        string sNew = "";
                        if (dGV1.Rows[j].Cells[i].Value != null)
                            sNew = dGV1.Rows[j].Cells[i].Value.ToString();
                        else
                            sNew = "";
                        CLayerCell nCell = LayerMod.ListCell[j];

                        string str1 = "";
                        if (!nCell.CheckExcel(sNew, ref str1))
                        {
                            message += str1 + "\r\n";
                            baa = false;
                            bErr = true;
                        }
                        nLay.ListCell[j].StrValue = sNew;
                    }
                    if (!baa)
                    {
                        MessageBox.Show("第" + i.ToString() + "层\r\n" + message, "错误");
                    }
                    nPro.ListLayer.Add(nLay);
                }
                return nPro;
            }
            catch (Exception ex)
            {
                MessageBox.Show("frmMain.GetProgram:\r\n" + ex.Message);
                return null;
            }
        }
        //打开
        private void toolOpen_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Excel文件";
                ofd.FileName = "";
                ofd.Multiselect = false;
                //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                ofd.Filter = "Excel files (*.xls,*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*";
                ofd.ValidateNames = true;
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                string strName = string.Empty;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    strName = ofd.FileName;
                    string filePath = sAppPath + @"\Project\Layer.xml";
                    OpenExcel(strName, sAppPath, ref pProgram);
                    UpdateDGV(this.dGV1);
                    bOpen = true;
                    UpdateShow();
                }
                else
                {
                    return;
                }
                if (strName == "")
                {
                    MessageBox.Show("没有选择Excel文件！无法进行数据导入");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("frmMain.toolOpen_Click:\r\n"+ex.Message);
            }
        }
        //保存
        private void toolSave_Click(object sender, EventArgs e)
        {
            if (bOnline)
            {
                bOnlineSave();
                return;
            }
            SaveToExcel(sender,e);
        }

        private void SaveToExcel(object sender, EventArgs e)
        {
            if (bSave)
            {
                if (MessageBox.Show("是否保存当前配方？", "保存",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    if (pProgram.fileInfo == null)
                    {
                        toolSaveAs_Click(sender, e);
                    }
                    else
                    {
                        bool bErr = false;
                        CProgram nPro = GetProgram(dGV1, ref bErr);
                        if (bErr)
                        {
                            if (MessageBox.Show("是否取消保存？", "保存",
     MessageBoxButtons.YesNo, MessageBoxIcon.Question,
     MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                return;
                            }
                        }
                        if (nPro != null)
                        {

                            pProgram.ListLayer = nPro.ListLayer;
                            Cursor.Current = Cursors.WaitCursor;
                            if (pProgram.ExportTOExcel(pProgram.fileInfo.FullName))
                                MessageBox.Show("数据已经成功导出到：" + pProgram.fileInfo.FullName, "导出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Cursor.Current = Cursors.Default;
                        }
                        else
                        {
                            return;
                        }
                    }
                    bSave = false;
                    Savetool.Enabled = bSave;
                }
            }
        }

        private void toolSaveAs_Click(object sender, EventArgs e)
        {
            string fileName = "配方" + DateTime.Now.ToString("yyyyMMddhhmmss");
            if (pProgram != null)
            {
                fileName = pProgram.Name + DateTime.Now.ToString("yyyyMMddhhmmss");
            }
            bool bErr = false;
            CProgram nPro = GetProgram(dGV1, ref bErr);
            if (bErr)
            {
                if (MessageBox.Show("是否取消保存？", "保存",
MessageBoxButtons.YesNo, MessageBoxIcon.Question,
MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    return;
                }
            }
            if (nPro != null)
            {
                pProgram = nPro;
                SaveExcel(fileName);
                bSave = false;
            }
        }
        private void SaveExcel(string fileName)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Excel 97-2003 files (*.xls)|*.xls|Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = fileName;
            //saveFileDialog1.DefaultExt = "xlsx";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                if (pProgram.ExportTOExcel(pProgram.fileInfo.FullName))
                    MessageBox.Show("数据已经成功导出到：" + pProgram.fileInfo.FullName, "导出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cursor.Current = Cursors.Default;
            }
            bSave = false;
            Savetool.Enabled = bSave;
        }

        #region DataGridView编辑检查有效性
        /// <summary>
        /// 单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string sNew = "";
            if (dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                sNew = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            else
                sNew = "";
            string message = "";
            CLayerCell nCell = LayerMod.ListCell[e.RowIndex];
            bool aa = nCell.CheckExcel(sNew, ref message);
            if (!aa)
            {
                MessageBox.Show(message, "错误");
                dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = sOld;
                return;
            }
            switch(nCell.CellType)
            {
                case  ECellType.GC_datetime:
                    dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = message;
                    if (bDefault)
                    {
                        if (message == nCell.StrValue)
                        {
                            dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Gray;
                        }
                        else
                        {
                            dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = nCell.BackColor;
                        }
                    }
                    break;
                case ECellType.GC_DO:
                case ECellType.GC_H2N2Switch:
                case ECellType.GC_IR2:
                case ECellType.GC_YPIVR1:
                case ECellType.GC_YPIVR2:
                    dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = sNew.ToUpper();
                    if (bDefault)
                    {
                        if (sNew.ToUpper() == nCell.StrValue)
                        {
                            dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Gray;
                        }
                        else
                        {
                            dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = nCell.BackColor;
                        }
                    }
                    break;
                default:
                    if (bDefault)
                    {
                        if (sNew == nCell.StrValue)
                        {
                            dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Gray;
                        }
                        else
                        {
                            dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = nCell.BackColor;
                        }
                    }
                    break;
            }
            if (!bSave)
            {
                if (sNew != sOld)
                {
                    bSave = true;
                    Savetool.Enabled = bSave;
                }
            }
            
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                sOld = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            else
                sOld = "";
        }

        #endregion

        private void toolNew_Click(object sender, EventArgs e)
        {
            if (bSave)
            {
                if (MessageBox.Show("当前配方被修改了，是否保存当前配方？", "保存",
             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                }
            }
            pProgram = new CProgram();
            UpdateDGV(dGV1);
            bOpen = true;
            bSave = false;
            UpdateShow();
        }

        private void toolLaySave_Click(object sender, EventArgs e)//保存所选层
        {
            try
            {
                List<int> iRe = CPublicDGV.GetSelCol(dGV1);
                for (int i = 0; i < iRe.Count; i++)
                {
                    SaveDGVToLayer(dGV1, iRe[i],false);
                }
                MessageBox.Show("保存完成", "保存");
                GetListLayer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void SaveDGVToLayer(DataGridView dgv, int iCol,bool bRe)//保存层
        {
            try
            {
                if (iCol == 0)
                    return;
                string aa = dgv.Rows[1].Cells[iCol].Value.ToString();
                if (aa == string.Empty || aa == "")
                {
                    string sShow = "第" + iCol.ToString() + "列层名称为空\r\n无法保存！";
                    return;
                }
                CLayer nLay = LayerMod.Clone();
                nLay.Clear();
                nLay.Name = aa;
                foreach (CLayer node in ListHisLayer)
                {
                    if (node.Name == aa)
                    {
                        if (!bRe)//直接覆盖
                        {
                            if (MessageBox.Show("层" + aa + "已经存在，是否覆盖？\r\n选择否，将不保存该层，请修改名称后重试", "保存",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.No)
                            {
                                return;
                            }
                        }
                    }
                }
                for (int j = 1; j < dgv.Rows.Count; j++)
                {
                    string message = "";
                    bool baa = nLay.ListCell[j].CheckExcel(dgv.Rows[j].Cells[iCol].Value.ToString(), ref message);
                    if (!baa)
                    {
                        MessageBox.Show(message, "错误");
                        return;
                    }
                    nLay.ListCell[j].StrValue = dgv.Rows[j].Cells[iCol].Value.ToString();
                }
                nLay.SaveToXML();
            }
            catch (Exception ex)
            {
            }
        }

        private void toolInsertLay_Click(object sender, EventArgs e)//插入层
        {
            List<int> iRe = CPublicDGV.GetSelCol(dGV1);
            if (iRe.Count == 1)
            {
                if(CheckCyc(iRe[0]) == 1 || CheckCyc(iRe[0]) == 2)
                {
                    MessageBox.Show("不能在循环中插入层", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                frmLayerManager fLM = new frmLayerManager(false);
                fLM.ShowDialog(this);
                if (LayerSelIndex >= 0 && LayerSelIndex < ListHisLayer.Count)
                {
                    CLayer SelLay = ListHisLayer[LayerSelIndex].Clone();
                    InsertDGV(dGV1, iRe[0] + 1, SelLay, 0);
                    bSave = true;
                }
                else
                {
                    MessageBox.Show("选择的层不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void toolAddLay_Click(object sender, EventArgs e)//新增层 只能在右边增加
        {
            try
            {
                List<int> iRe = CPublicDGV.GetSelCol(dGV1);
                if (iRe.Count == 1)
                {
                    if (CheckCyc(iRe[0]) == 1 || CheckCyc(iRe[0]) == 2)
                    {
                        MessageBox.Show("不能在循环中插入层", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    CLayer nLay = new CLayer();
                    if (iRe[0] == 0)
                    {
                        nLay = LayerMod.Clone();
                        nLay.ListCell[1].StrValue = "NewLay";
                        nLay.Name = nLay.ListCell[1].StrValue;
                    }
                    else
                    {
                        nLay = null;
                    }
                    InsertDGV(dGV1, iRe[0]+1, nLay, -1);
                    bSave = true;
                }
            }
            catch (Exception ex)
            { }
        }

        private void toolDelLay_Click(object sender, EventArgs e)//删除层
        {
            List<int> iRe = CPublicDGV.GetSelCol(dGV1);
            if (iRe!=null)
            {
                if (MessageBox.Show("是否要删除选中的层？", "删除",
             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    int iCol = dGV1.SelectedCells[0].ColumnIndex;
                    for (int i = iRe.Count - 1; i >= 0; i--)
                    {
                        if (CheckCyc(iRe[i]) != 0)
                        {
                            MessageBox.Show("不能删除带循环的层，请先删除循环后重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (iRe[i] >= 1)
                        {
                            dGV1.Columns.RemoveAt(iRe[i]);
                            bSave = true;
                        }
                        else
                        {
                            MessageBox.Show("控制项不能删除！", "删除", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } 
                    }
                }
            }
        }

        private void toolSubInsert_Click(object sender, EventArgs e)//插入子程序
        {
            try
            {
                List<int> iRe = CPublicDGV.GetSelCol(dGV1);
                if (iRe.Count == 1)
                {
                    if (CheckCyc(iRe[0]) != 0)
                    {
                        MessageBox.Show("不能在循环中插入层", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    frmSubManager newSub = new frmSubManager(false);
                    newSub.ShowDialog();
                    if (newSub.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        if (newSub.sSubSelName !=null && newSub.sSubSelName !="")
                        {
                            foreach (CSubProgram SelSub in ListSubProgram)
                            {
                                if (SelSub.Name == newSub.sSubSelName)
                                {
                                    int iCol = dGV1.SelectedCells[0].ColumnIndex;
                                    for (int i = SelSub.LayerList.Length - 1; i >= 0; i--)
                                    {
                                        foreach (CLayer nLay in ListHisLayer)
                                        {
                                            if (nLay.Name == SelSub.LayerList[i])
                                            {
                                                InsertDGV(dGV1, iCol + 1, nLay, 0);
                                                bSave = true;
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("选择的子程序不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void toolSubSave_Click(object sender, EventArgs e)//保存子程序
        {
            try
            {
                int iCol1 = dGV1.SelectedCells[0].ColumnIndex;
                int iCol2 = dGV1.SelectedCells[dGV1.SelectedCells.Count - 1].ColumnIndex;
                if (iCol1 != iCol2)
                {

                    string str1 = "";
                    for (int i = Math.Min(iCol1, iCol2); i <= Math.Max(iCol1, iCol2); i++)
                    {
                        str1 += dGV1.Rows[1].Cells[i].Value + ",";
                        SaveDGVToLayer(dGV1, i,true);
                    }
                    str1 = str1.TrimEnd(',');

                    foreach (CSubProgram nSub in ListSubProgram)
                    {
                        string str2 = nSub.sLayerList.TrimEnd(',');
                        if (str2 == str1)
                        {
                            MessageBox.Show("以下组合的子程序已经存在，请重新选择：\r\n" + str1, "错误");
                            return;
                        }
                    }

                    frmSubAdd newSub = new frmSubAdd(str1);
                    newSub.ShowDialog();
                    GetListLayer();
                }
                else
                {
                    MessageBox.Show("两列或两列以上的层才能组合为一个子程序", "错误");
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void toolCycAdd_Click(object sender, EventArgs e)//添加循环
        {
            try
            {
                List<int> iRe = CPublicDGV.GetSelCol(dGV1);

                int iCol1 = iRe[0];
                int iCol2 = iRe[iRe.Count-1];
                if (iCol1 != iCol2)
                {

                    string str1 = "";
                    for (int i = Math.Min(iCol1, iCol2); i <= Math.Max(iCol1, iCol2); i++)
                    {
                        if (dGV1.Rows[0].Cells[i].Value != null)
                        {
                            str1 = dGV1.Rows[0].Cells[i].Value.ToString();
                            if (str1.Length > 0)
                            {
                                MessageBox.Show(i.ToString() + "列已经存在一个循环中\r\n禁止两个循环交叉\r\n请重新选择", "错误");
                                return;
                            }
                        }
                    }
                    iCycle = -1;
                    frmCycleAdd newSub = new frmCycleAdd();
                    newSub.ShowDialog();
                    if (iCycle > 1)
                    {
                        for (int i = Math.Min(iCol1, iCol2); i <= Math.Max(iCol1, iCol2); i++)
                        {
                            dGV1.Rows[0].Cells[i].Value = iCycle.ToString("D2") + ";" + (i - Math.Min(iCol1, iCol2) + 1).ToString("D2");
                        }
                        bSave = true;
                    }
                }
                else
                {
                    MessageBox.Show("两列或两列以上的层才能组成循环", "错误");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void toolCycDel_Click(object sender, EventArgs e)//删除循环
        {
            try
            {
                List<int> iRe = CPublicDGV.GetSelCol(dGV1);

                int iCol1 = iRe[0];
                int iCol2 = iRe[iRe.Count - 1];
                foreach (int i in iRe)
                {
                    DelCyc(i);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DelCyc(int iCol1)
        {
            try
            {
                string str1 = "";
                if (dGV1.Rows[0].Cells[iCol1].Value == null)
                    return;
                str1 = dGV1.Rows[0].Cells[iCol1].Value.ToString();
                if (str1.Length == 0)
                    return;

                string[] str2 = str1.Split(';');
                if (str2.Length < 2)
                    return;
                int iCyc = int.Parse(str2[0]);
                int iIndex = int.Parse(str2[1]);
                int iStart = iCol1;
                int iEnd = iCol1;

                for (int i = 1; i < dGV1.ColumnCount + iIndex-iCol1; i++)
                {
                    
                    string sCyc = str2[0] + ";" + i.ToString("D2");
                    if (dGV1.Rows[0].Cells[iCol1 - iIndex+i].Value.ToString() == sCyc)
                    {
                        iStart = Math.Min(iStart, iCol1 - iIndex + i);
                        iEnd = Math.Max(iEnd, iCol1 - iIndex + i);
                    }
                }

                    str1 = "所选循环起始列为:" + iStart.ToString();
                    str1 += "\r\n所选循环结束列为:" + iEnd.ToString();
                    str1 += "\r\n所选循环循环次数:" + iCyc.ToString();
                    str1 += "\r\n是否删除所选循环？";
                    if (MessageBox.Show(str1, "删除",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        for (int i = iStart; i <= iEnd; i++)
                        {
                            dGV1.Rows[0].Cells[i].Value = "";
                        }
                        bSave = true;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DelCyc");
            }
        }

        /// <summary>
        /// 检查循环
        /// </summary>
        /// <param name="iCol">列序号</param>
        /// <returns>0在循环外 1在循环中 2在循环头 3循环尾</returns>
        private int CheckCyc(int iCol1)//检查循环
        {
            string str1 = "";
            if (dGV1.Rows[0].Cells[iCol1].Value == null)
                return 0;
            str1 = dGV1.Rows[0].Cells[iCol1].Value.ToString();
            if (str1.Length == 0)
                return 0;

            string[] str2 = str1.Split(';');
            if (str2.Length < 2)
                return 0;

            int iCyc = int.Parse(str2[0]);
            int iIndex = int.Parse(str2[1]);
            int iStart = iCol1;
            int iEnd = iCol1;

            for (int i = 1; i < dGV1.ColumnCount + iIndex - iCol1; i++)
            {

                string sCyc = str2[0] + ";" + i.ToString("D2");
                if (dGV1.Rows[0].Cells[iCol1 - iIndex + i].Value.ToString() == sCyc)
                {
                    iStart = Math.Min(iStart, iCol1 - iIndex + i);
                    iEnd = Math.Max(iEnd, iCol1 - iIndex + i);
                }
            }
            if (iEnd == iCol1)
                return 3;
            else if (iCol1 == iStart)
                return 2;
            else
                return 1;
        }

        /// <summary>
        /// InsertDGV 表格插入层
        /// </summary>
        /// <param name="dgv">表格控件名称</param>
        /// <param name="iCol">需要插入的列数，在该列右边插入</param>
        /// <param name="nLay">需要插入的数据</param>
        /// <param name="iClone">插入模式</param>
        private void InsertDGV(DataGridView dgv,int iCol, CLayer nLay,int iClone)
        {
            //dgv.DataSource = ds.Tables[0];
            Savetool.Enabled = true;
            bSave = true;

            DataGridViewColumn nCol = new DataGridViewTextBoxColumn();

            dgv.Columns.Insert(iCol, nCol);

            for (int i = 1; i < dgv.ColumnCount; i++)
            {
                dgv.Columns[i].Name = i.ToString();
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            if (nLay != null)
            {
                for (int i = 0; i < LayerMod.ListCell.Count; i++)
                {
                    dgv.Rows[i].Cells[iCol].Value = nLay.ListCell[i].StrValue;
                }
            }
            else
            {
                for (int i = 0; i < LayerMod.ListCell.Count; i++)
                {
                    dgv.Rows[i].Cells[iCol].Value = dgv.Rows[i].Cells[iCol+iClone].Value;
                }
                string sNum = CStrPublicFun.GetNumberFromStr(dgv.Rows[1].Cells[iCol + iClone].Value.ToString(),0,1);
                string sNew = "";
                if (sNum != null)
                {
                    int iNum = Convert.ToInt32(sNum) + 1;
                    string sOld = dgv.Rows[1].Cells[iCol + iClone].Value.ToString();
                    if (iNum.ToString().Length < sNum.Length)
                        sNew = sOld.Substring(0, sOld.Length - sNum.Length) + iNum.ToString("D" + sNum.Length.ToString());
                    else
                        sNew = sOld.Substring(0, sOld.Length - sNum.Length) + iNum.ToString();
                }
                else
                    sNew = dgv.Rows[1].Cells[iCol + iClone].Value.ToString() + "01";
                dgv.Rows[1].Cells[iCol].Value = sNew;
            }
            dgv.AutoResizeColumns();
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Point p = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y);
                dvgMenu.Show(p);
            }
        }

        private void dvgMenuCopy_Click(object sender, EventArgs e)
        {
            dgvCopy(dGV1);
        }

        private void dvgMenuPaste_Click(object sender, EventArgs e)
        {
            dgvPaste(dGV1);
        }

        private void dvgMenuDel_Click(object sender, EventArgs e)
        {
            dgvDel(dGV1);
        }

        //可在dgv中任意位置粘贴任意行列的数据        
        /// <summary>
        /// DataGridView复制
        /// </summary>
        /// <param name="dgv">DataGridView实例</param>
        private void dgvCopy(DataGridView dgv)
        {
            if (dgv.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                try
                {
                    Clipboard.SetDataObject(dgv.GetClipboardContent());
                }
                catch (Exception MyEx)
                {
                    MessageBox.Show(MyEx.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        /// <summary>
        /// DataGridView剪切
        /// </summary>
        /// <param name="dgv">DataGridView实例</param>
        private void dgvCut(DataGridView dgv)
        {
            dgvCopy(dgv);
            try
            {
                int k = dgv.SelectedRows.Count;
                if (k == dgv.Rows.Count)
                    k--;
                for (int i = k; i >= 1; i--)
                {
                    dgv.Rows.RemoveAt(dgv.SelectedRows[i - 1].Index);
                }
            }
            catch (Exception MyEx)
            {
                MessageBox.Show(MyEx.Message);
            }

        }
        /// <summary>
        /// DataGridView删除
        /// </summary>
        /// <param name="dgv">DataGridView实例</param>
        private void dgvDel(DataGridView dgv)
        {
            try
            {
                int k = dgv.SelectedCells.Count;

                if (MessageBox.Show("确实要删除这" + Convert.ToString(k) + "项吗？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                { }
                else
                {
                    //DataGridViewCell Cell1 = dgv.SelectedCells[0];
                    //DataGridViewCell Cell2 = dgv.SelectedCells[k-1];
                    for (int i = 0; i < k; i++)
                    {
                        dgv.SelectedCells[i].Value = "";
                    }
                }
            }
            catch (Exception MyEx)
            {
                MessageBox.Show(MyEx.Message);
            }

        }
        /// <summary>
        /// DataGridView粘贴
        /// </summary>
        /// <param name="dt">DataGridView数据源</param>
        /// <param name="dgv">DataGridView实例</param>
        public void dgvPaste(DataGridView dgv)
        {
            try
            {
                int cRowIndex = dgv.CurrentCell.RowIndex;
                int cColIndex = dgv.CurrentCell.ColumnIndex;
                //最后一行为新行
                int rowCount = dgv.Rows.Count - 1;
                int colCount = dgv.ColumnCount;
                //获取剪贴板内容
                string pasteText = Clipboard.GetText();
                //判断是否有字符存在
                if (string.IsNullOrEmpty(pasteText))
                    return;
                //以换行符分割的数组
                string[] lines = pasteText.Trim().Split('\n');
                int txtLength = lines.Length;
                DataGridViewRow row;
                //判断是修改还是添加,如果dgv中行数减当前行号大于要粘贴的行数，直接修改
                if (rowCount - cRowIndex > txtLength)
                {
                    for (int j = cRowIndex; j < cRowIndex + txtLength; j++)
                    {
                        //以制表符分割的数组
                        string[] vals = lines[j - cRowIndex].Split('\t');
                        //判断要粘贴的列数与dgv中列数减当前列号的大小，取最小值
                        int minColLength = vals.Length > colCount - cColIndex ? colCount - cColIndex : vals.Length;
                        row = dgv.Rows[j];
                        for (int i = 0; i < minColLength; i++)
                        {
                            row.Cells[i + cColIndex].Value = vals[i];
                        }
                    }
                }
                //否则先修改后添加
                else
                {
                    MessageBox.Show("粘贴区域小于复制区域");
                }
            }
            catch (Exception MyEx)
            {
                MessageBox.Show(MyEx.Message);
            }
        }

        private void toolAbout_Click(object sender, EventArgs e)
        {
            AboutBox1 nAbout = new AboutBox1();
            nAbout.Show();
        }

        private void MenuLControl_Click(object sender, EventArgs e)
        {
            frmControlManager fCon = new frmControlManager(LayerMod);
            fCon.ShowDialog();
            LayerMod.InitFromXml(sAppPath);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (LayerMod.ListCell[e.RowIndex].CellType == ECellType.GC_head)
                {
                    LayerMod.ListCell[e.RowIndex].bVisible = !LayerMod.ListCell[e.RowIndex].bVisible;
                    for (int i = LayerMod.ListCell[e.RowIndex].iHeadFirst; i <= LayerMod.ListCell[e.RowIndex].iHeadLast; i++)
                    {
                        dGV1.Rows[i].Visible = LayerMod.ListCell[e.RowIndex].bVisible;
                    }
                }
            }
        }

        private void toolClose_Click(object sender, EventArgs e)
        {
            if (bOnline)
            {
                if (bOnlineSave())
                {
                    this.Close();
                }
            }
            else
            {
                SaveToExcel(sender, e);
                this.Close();
            }
        }

        private bool bOnlineSave()
        {
            if (bSave)
            {
                if (MessageBox.Show("当前配方被修改了，是否保存当前配方？", "保存",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    bool bErr = false;
                    CProgram nPro = GetProgram(dGV1, ref bErr);
                    nPro.TimeSum();
                    if (nPro.ListFlow.Count <= pProgram.LayRunIndex)
                    {
                        MessageBox.Show("层数少于当前运行层，不能保存修改！", "错误");
                        return false;
                    }
                    if (bErr)
                    {
                        if (MessageBox.Show("是否取消保存？", "保存",
 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
 MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    pProgram = nPro;
                    bSave = false;
                    return true;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        private void MenuLBank_Click(object sender, EventArgs e)
        {
            GetListLayer();
            frmLayerManager fLM = new frmLayerManager(true);
            fLM.ShowDialog(this);
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            UpdateShow();
        }

        private void UpdateShow()
        {
            int[] iRC = CPublicDGV.GetSelState(dGV1);
            bool bInsertLay = true;
            bool bAddLay = true;
            bool bDelLay = true;
            bool bCyc = true;
            bool bSub = true;
            if (bOpen)
            {
                if (iRC[0] == iRC[1])
                {
                    bCyc = false;
                    bSub = false;
                }
                else
                {
                    bInsertLay = false;
                    bAddLay = false;
                }

                if (iRC[0] == 0)
                {
                    bDelLay = false;
                    bCyc = false;
                    bSub = false;
                }
            }
            else
            {
                bInsertLay = false;
                bAddLay = false;
                bDelLay = false;
                bCyc = false;
                bSub = false;
            }
            Checktool.Enabled = bOpen;
            SaveAstool.Enabled = bOpen;
            Savetool.Enabled = bOpen && bSave;

            LayInsertTool.Enabled = bInsertLay;
            LayInsertMenu.Enabled = bInsertLay;
            LayInsertdvg.Enabled = bInsertLay;
            SubInsertTool.Enabled = bInsertLay;
            SubInsertMenu.Enabled = bInsertLay;
            SubInsertdvg.Enabled = bInsertLay;

            LayAddTool.Enabled = bAddLay;
            LayAddMenu.Enabled = bAddLay;
            LayAdddvg.Enabled = bAddLay;

            LayDelTool.Enabled = bDelLay;
            LayDelMenu.Enabled = bDelLay;
            LayDeldvg.Enabled = bDelLay;
            LaySaveTool.Enabled = bDelLay;
            LaySavedvg.Enabled = bDelLay;
            LaySaveDefault.Enabled = bDelLay;

            SubSaveTool.Enabled = bSub;
            SubSaveMenu.Enabled = bSub;
            SubSavedvg.Enabled = bSub;

            CycAdddvg.Enabled = bCyc;
            CycTool.Enabled = bCyc;
            CycDeldvg.Enabled = bCyc;

            if (iRC[2] > 0)
            {
                dvgMenuCopy.Visible = true;
                dvgMenuPaste.Visible = true;
                dvgMenuDel.Visible = true;

            }
            else
            {
                dvgMenuCopy.Visible = false;
                dvgMenuPaste.Visible = false;
                dvgMenuDel.Visible = false;
            }
        }

        #region 右键菜单

        private void MenuSManager_Click(object sender, EventArgs e)
        {
            GetListLayer();
            frmSubManager newSub = new frmSubManager(true);
            newSub.ShowDialog();
        }
        #endregion

        private void toolStripCheck_Click(object sender, EventArgs e)//检查配方
        {
            bool bErr = false;
            CProgram nPro = GetProgram(dGV1,ref bErr);
            if (!bErr)
            {
                nPro.LoadRulesFromXML(sAppPath);
                bool bRe = nPro.ChcekRunRules();
                if (!nPro.CheckBubbler())
                    bRe = false;

                if (bRe)
                {
                    MessageBox.Show("检查完毕，没有错误！");
                }
            }
        }

        private void LaySaveDefault_Click(object sender, EventArgs e)//保存缺省层
        {
            try
            {
                List<int> iRe = CPublicDGV.GetSelCol(dGV1);
                if (iRe[0] > 0)
                {
                    if (MessageBox.Show("是否将第" + iRe[0].ToString() + "层保存为缺省层？", "保存",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        SaveDefaultLayer(dGV1, iRe[0]);
                        MessageBox.Show("保存完成", "保存");
                        //层模板，用于初始化层的控制项
                        LayerMod.InitFromXml(sAppPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void SaveDefaultLayer(DataGridView dgv, int iCol)//保存缺省层
        {
            try
            {
                if (iCol == 0)
                    return;
                CLayer nLay = LayerMod.Clone();
                nLay.Clear();
                for (int j = 1; j < dgv.Rows.Count; j++)
                {
                    string message = "";
                    bool baa = nLay.ListCell[j].CheckExcel(dgv.Rows[j].Cells[iCol].Value.ToString(), ref message);
                    if (!baa)
                    {
                        MessageBox.Show(message, "错误");
                        return;
                    }
                    nLay.ListCell[j].StrValue = dgv.Rows[j].Cells[iCol].Value.ToString();
                }
                nLay.SaveToDefault();
            }
            catch (Exception ex)
            {
            }
        }

        bool bDefault = false;
        private void toolDefault_Click(object sender, EventArgs e)
        {
            bDefault = !bDefault;
            toolDefault.Checked = bDefault;
            for (int i = 1; i < dGV1.ColumnCount; i++)
            {
                for (int j = 2; j < dGV1.RowCount; j++)
                {
                    if (bDefault)
                    {
                        if (dGV1.Rows[j].Cells[i].Value != null)
                        {
                            string sV = dGV1.Rows[j].Cells[i].Value.ToString();
                            if (sV == LayerMod.ListCell[j].StrValue)
                            {
                                dGV1.Rows[j].Cells[i].Style.BackColor = Color.Gray;
                            }
                            else
                            {
                                dGV1.Rows[j].Cells[i].Style.BackColor = LayerMod.ListCell[j].BackColor;
                            }
                        }
                    }
                    else
                    {
                        dGV1.Rows[j].Cells[i].Style.BackColor = LayerMod.ListCell[j].BackColor;
                    }
                }
            }
        }
    }
}
