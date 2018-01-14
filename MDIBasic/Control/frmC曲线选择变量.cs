using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace LSSCADA.Control
{
    public partial class frmC曲线选择变量 : Form
    {
        Color[] ListColor = new Color[] { 
        Color.Red,
        Color.Lime,
        Color.Yellow,
        Color.Blue,
        Color.Orange,
        Color.Fuchsia,
        Color.Cyan,
        Color.White,
        Color.Green,
        Color.DarkRed,
        Color.DeepPink,
        Color.LightGray,
        Color.PaleGreen,
        Color.OrangeRed,
        Color.DarkViolet,
        Color.Olive,
        Color.DeepSkyBlue,
        Color.DarkCyan
        };
        string[] sCurveGroupName = new string[] { "测量值1", "测量值2", "测量值3", "测量值4", "测量值5", "测量值6", "测量值7" };
        public List<CLSCurve> ListCur = new List<CLSCurve>();
        public List<CLSYAxisGroup> ListCurGroup = new List<CLSYAxisGroup>();
        public CLSChart ZedG = new CLSChart();
        int iSelIndex = 0;
        string sEdit = "";
        public frmC曲线选择变量()
        {
            InitializeComponent();
            FillVarTreeList();
            InitializeDGV(dGV1);
        }
        public frmC曲线选择变量(CLSChart ZedG1, string ButName)
        {
            InitializeComponent();
            FillVarTreeList();
            InitializeDGV(dGV1);
            ZedG = ZedG1;
            ListCur.Clear();
            if (ButName == "butEdit")
            {
                this.Text = "曲线组编辑";
                splitContainer3.Panel1Collapsed = false;
                butApply.Visible = true;
                butOK.Visible = false;
                FillCurTreeList();
            }
            else
            {
                this.Text = "自由选择曲线";
                splitContainer3.Panel1Collapsed = true;
                butApply.Visible = false;
                butOK.Visible = true;
                ShowSelTeam();
            }
        }

        private void ShowSelTeam()
        {
            ListCur.Clear();
            sCurveGroupName = new string[] { "测量值1", "测量值2", "测量值3", "测量值4", "测量值5", "测量值6", "测量值7" };
            foreach (CLSYAxisGroup nCG in ZedG.SelTream.ListYAxisGroup)
            {
                sCurveGroupName[ZedG.SelTream.ListYAxisGroup.IndexOf(nCG)] = nCG.Text;
                foreach (CLSCurve nCu in nCG.ListCur)
                {
                    ListCur.Add(nCu);
                }
            }
            if (ListCur.Count > 0)
            {
                iSelIndex = ListCur[ListCur.Count - 1].iYAxis + 1;

            }
            else
            {
                iSelIndex = 0;
            }
            RefreshDGW(0);
        }

        private void FillCurTreeList()
        {
            listBox1.Items.Clear();
            foreach (CLSTeamCurve nTeam in ZedG.ListTream)
            {
                listBox1.Items.Add(nTeam.Name);
            }
            listBox1.SelectionMode = SelectionMode.One;
            listBox1.SelectedIndex =ZedG.ListTream.IndexOf( ZedG.SelTream);
        }

        private void FillVarTreeList()
        {
            treeView1.Nodes.Add("Prj.", frmMain.staPrj.Name);
            //treeView1.CheckBoxes = true;
            foreach (CPort nPort in frmMain.staComm.ListPort)
            {
                //treeView1.Nodes[0].Nodes.Add(nPort.PortName,nPort.PortDescript);
                TreeNode nNode = new TreeNode(nPort.PortName);
                nNode.Name = "Prt." + frmMain.staComm.ListPort.IndexOf(nPort);

                foreach (CStation nSta in nPort.ListStation)
                {
                    TreeNode nnN = nNode.Nodes.Add("Sta." + frmMain.staComm.ListStation.IndexOf(nSta).ToString(), nSta.Name);
                    foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                    {
                        if (nVar.SaveToDB <= 0 || nVar.VarType == EVarTYPE.Var_BOOL || nVar.VarType == EVarTYPE.Var_STRING || nVar.VarType == EVarTYPE.Var_BLOB)
                            continue;
                        nnN.Nodes.Add("Var." + nSta.StaDevice.ListDevVar.IndexOf(nVar).ToString(), nVar.Name + ":" + nVar.Description);
                    }
                }
                treeView1.Nodes[0].Nodes.Add(nNode);
            }

            treeView1.ExpandAll();
        }

        private void InitializeDGV(DataGridView DGV)
        {
            // Create an unbound DataGridView by declaring a column count.
            DGV.ColumnCount = 4;
            DGV.ColumnHeadersVisible = true;

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            // Resize the height of the column headers. 
            DGV.AutoResizeColumnHeadersHeight();
            // Resize all the row heights to fit the contents of all non-header cells.
            DGV.AutoResizeRows(
                DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);

            // Set the column header names.
            DGV.Columns[0].Name = "序号";
            DGV.Columns[1].Name = "列名";
            DGV.Columns[2].Name = "线颜色";
            DGV.Columns[3].Name = "变量";
            DGV.Columns[0].Width = 50;
            DGV.Columns[1].Width = 300;
            DGV.Columns[2].Width = 80;
            DGV.Columns[3].Width = 150;
            DGV.Columns[0].ReadOnly = true;
            DGV.Columns[2].ReadOnly = true;
            DGV.Columns[3].ReadOnly = true;
        }

        private void RefreshButton(int iSel)
        {
            bool bVis = iSel < 1;
            bool bEnb = iSel < 2;
            GroupAdd.Visible = iSel < 1;
            GroupDel.Visible = iSel >= 1;

            butAdd1.Enabled = (iSelIndex >= 0);
            butAdd2.Enabled = (iSelIndex >= 1);
            butAdd3.Enabled = (iSelIndex >= 2);
            butAdd4.Enabled =(iSelIndex >= 3);
            butDel.Enabled = (iSel == 1);
            butUp1.Enabled = (iSel == 1);
            butDown1.Enabled = (iSel == 1);
        }

        private void RefreshDGW(int iIndex)
        {
            if(iIndex<0 || iIndex>ListCur.Count)
                return;
            
            int k = 0;
            if (iIndex == 0)
                    k = -1;
                else
                    k = ListCur[iIndex - 1].iYAxis;
            int n = iIndex + k + 1;
            while (dGV1.RowCount > n+1)
            {
                dGV1.Rows.RemoveAt(n);
            }
            string[] str1 = new string[] { "第1轴", "测量值1", "" };
            int iRow = 0;
            
            for (int i = iIndex; i < ListCur.Count; i++)
            {
                CLSCurve cCur = ListCur[i];
                
                if (i >= ListColor.Length)
                {
                    cCur.LineColor = Color.FromArgb(0xFF, Color.FromArgb(i * 7000));
                }
                else
                {
                    cCur.LineColor = ListColor[i];
                }
                if (cCur.iYAxis != k)
                {
                    k = cCur.iYAxis;
                    str1 = new string[] { "第" + (k + 1).ToString() + "轴",sCurveGroupName[k], "" };
                    iRow = dGV1.Rows.Add(str1);
                    dGV1.Rows[iRow].DefaultCellStyle.BackColor = Color.LightGray;
                    dGV1.Rows[iRow].DefaultCellStyle.Font = new System.Drawing.Font(dGV1.Font.FontFamily, dGV1.Font.Size + 2, FontStyle.Bold);
                    dGV1.Rows[iRow].DefaultCellStyle.ForeColor = Color.Red;
                    dGV1.Rows[iRow].Cells[2].Style.BackColor = cCur.LineColor;
                }
                str1 = new string[] { (i + 1).ToString(), cCur.Text, cCur.LineColor.ToArgb().ToString("X8"), cCur.StaVarName };
                iRow = dGV1.Rows.Add(str1);
                dGV1.Rows[iRow].Cells[2].Style.BackColor = cCur.LineColor;
            }
        }

        private void butAdd1_Click(object sender, EventArgs e)
        {
            butApply.Enabled = true;
            Button nBut = (Button)sender;
            int i = Convert.ToInt32(nBut.Name.Substring(6, 1)) - 1;

            TreeNode node = treeView1.SelectedNode;
            string sType = node.Name.Substring(0, 3);
            if (sType == "Var")
            {
                int iSta = Convert.ToInt32(node.Parent.Name.Substring(4));
                int iVar = Convert.ToInt32(node.Name.Substring(4));
                CStation sta2 = frmMain.staComm.ListStation[iSta];
                CVar nVar = sta2.StaDevice.ListDevVar[iVar];
                if (nVar != null)
                {
                    foreach (CLSCurve cCur in ListCur)
                    {
                        if (nVar == cCur.cVar)
                        {
                            MessageBox.Show("该变量已添加！", "错误");
                            return;
                        }
                    }
                    CLSCurve nCur = new CLSCurve(nVar);
                    nCur.iYAxis = i;
                    if (iSelIndex <= i)
                        iSelIndex = i+1;
                    for (int j = 0; j < ListCur.Count; j++)
                    {
                        if (i < ListCur[j].iYAxis)
                        {
                            ListCur.Insert(j, nCur);
                            RefreshDGW(ListCur.IndexOf(nCur));
                            return;
                        }
                    }
                    ListCur.Add(nCur);
                    RefreshDGW(ListCur.IndexOf(nCur));
                }
            }
        }

        private void butUp1_Click(object sender, EventArgs e)
        {
            try
            {
                string str1 = dGV1.Rows[dGV1.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                if (str1 != "")
                {
                    int i = Convert.ToInt32(str1);
                    if (i >= 2 || i <= ListCur.Count)
                    {
                        if (ListCur[i - 2].iYAxis != ListCur[i - 1].iYAxis)
                        {
                            if (i < ListCur.Count)
                            {
                                if (ListCur[i].iYAxis != ListCur[i - 1].iYAxis)
                                {
                                    for (int j = i; j < ListCur.Count; j++)
                                    {
                                        ListCur[j].iYAxis--;
                                    }
                                }
                            }
                            ListCur[i - 1].iYAxis = ListCur[i - 2].iYAxis;
                            butApply.Enabled = true;
                            RefreshDGW(i - 1);
                        }
                        else
                        {
                            CLSCurve lcc = ListCur[i - 1];
                            ListCur[i - 1] = ListCur[i - 2];
                            ListCur[i - 2] = lcc;
                            butApply.Enabled = true;
                            RefreshDGW(i - 2);
                        }   
                    }
                }
            }
            catch
            {
            }
        }

        private void butDown1_Click(object sender, EventArgs e)
        {
            try
            {
                string str1 = dGV1.Rows[dGV1.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                if (str1 != "")
                {
                    int i = Convert.ToInt32(str1);
                    if (i >= 1 || i < ListCur.Count)
                    {
                        if (ListCur[i].iYAxis != ListCur[i - 1].iYAxis)
                        {
                            if (i == 1 || ListCur[i-2].iYAxis != ListCur[i - 1].iYAxis)
                            {
                                {
                                    for (int j = i; j < ListCur.Count; j++)
                                    {
                                        ListCur[j].iYAxis--;
                                    }
                                }
                            }

                            ListCur[i - 1].iYAxis = ListCur[i].iYAxis;
                            butApply.Enabled = true;
                            RefreshDGW(i - 1);
                        }
                        else
                        {
                            CLSCurve lcc = ListCur[i - 1];
                            ListCur[i - 1] = ListCur[i ];
                            ListCur[i ] = lcc;
                            butApply.Enabled = true;
                            RefreshDGW(i - 1);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                string sType = node.Name.Substring(0, 3);
                if (sType == "Var")
                {
                    string sName = node.Name.Substring(4);
                }
                RefreshButton(0);
            }
            catch (Exception ee)
            {
                MessageBox.Show("treeView1_AfterSelect:" + ee.Message);
            }

        }

        private void dGV1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    if (dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        ColorDialog aaa = new ColorDialog();
                        aaa.Color = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor;
                        aaa.ShowDialog();
                        Color aaaa = aaa.Color;
                        dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = aaaa;
                        dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = aaaa.ToArgb().ToString("X6");
                    }
                }
            }
            catch (Exception ee)
            { 
            }
        }

        private void butDel_Click(object sender, EventArgs e)
        {
            try
            {
                string str1 = dGV1.Rows[dGV1.SelectedCells[0].RowIndex].Cells[0].Value.ToString();
                if (str1 != "")
                {
                    int i = Convert.ToInt32(str1);
                    if (i >= 1 || i <= ListCur.Count)
                    {
                        ListCur.RemoveAt(i - 1);
                        butApply.Enabled = true;
                        RefreshDGW(i - 1);
                    }
                }
            }
            catch
            { 
            }
        }

        private void dGV1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView nDGV = (DataGridView)sender;
                if (dGV1.Rows[e.RowIndex].DefaultCellStyle.BackColor == Color.LightSlateGray)
                {
                    RefreshButton(2);
                }
                else
                    RefreshButton(1);
            }
            catch (Exception ee)
            { }
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            ListCurGroup.Clear();
            for (int i = 0; i < dGV1.RowCount; i++)
            {
                if (dGV1.Rows[i].Cells[0].Value == null)
                    continue;
                string str0 = dGV1.Rows[i].Cells[0].Value.ToString();
                if (str0.Substring(0, 1) == "第")
                {
                    int j = Convert.ToInt32(str0.Substring(1, 1));
                    CLSYAxisGroup cCurG = new CLSYAxisGroup(dGV1.Rows[i].Cells[1].Value.ToString());
                    cCurG.FontColor = dGV1.Rows[i].Cells[2].Style.BackColor;
                    ListCurGroup.Add(cCurG);
                }
                else
                {
                    int j = Convert.ToInt32(str0);
                    ListCur[j - 1].Text = dGV1.Rows[i].Cells[1].Value.ToString();
                    ListCur[j - 1].LineColor = dGV1.Rows[i].Cells[2].Style.BackColor;
                    ListCurGroup[ListCurGroup.Count - 1].ListCur.Add(ListCur[j - 1]);
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void dGV1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                butApply.Enabled = true;
                if (e.ColumnIndex == 1)
                {
                    if (dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null || dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                    {
                        dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = sEdit; ;
                    }
                    else
                    {
                        string str1 = dGV1.Rows[e.RowIndex].Cells[0].Value.ToString();
                        if (str1.Substring(0, 1) == "第")
                        {
                            int i = Convert.ToInt32(str1.Substring(1, 1));
                            sCurveGroupName[i - 1] = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                        }
                        else
                        {
                            int i = Convert.ToInt32(str1);
                            ListCur[i - 1].Text = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ee)
            {
            }
        }

        private void dGV1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
           
        }

        private void dGV1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                sEdit = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            catch (Exception ex)
            {
                sEdit = "";
            }
        }

        private void butTeamAdd_Click(object sender, EventArgs e)
        {
            butApply.Enabled = true;
            Button nBut = (Button)sender;
            switch (nBut.Name)
            {
                case "butTeamAdd":
                    string str2 = "";
                    foreach (CLSTeamCurve nTeam in ZedG.ListTream)
                    {
                        str2 += "{" + nTeam.Name + "}";
                    }
                    frmPubInput nIn = new frmPubInput("新建曲线组", str2);
                    nIn.ShowDialog();
                    if (nIn.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        CLSTeamCurve nTeam = new CLSTeamCurve();
                        nTeam.Name = nIn.sOld;
                        ZedG.ListTream.Add(nTeam);
                        FillCurTreeList();
                    }
                    break;
                case "butTeamEdit":
                    str2 = "";
                    foreach (CLSTeamCurve nTeam in ZedG.ListTream)
                    {
                        str2 += "{" + nTeam.Name + "}";
                    }
                    nIn = new frmPubInput(ZedG.SelTream.Name, str2);
                    nIn.ShowDialog();
                    if (nIn.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        ZedG.SelTream.Name = nIn.sOld;
                        FillCurTreeList();
                    }
                    break;
                case "butTeamDel":
                    if (MessageBox.Show("是否删除曲线组："+ ZedG.SelTream.Name +"？", "删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.No)//弹出确定对话框
                    {
                        return;
                    }
                    for (int i = 0; i < ZedG.ListTream.Count;i++ )
                    {
                        if (ZedG.ListTream[i].Name == ZedG.SelTream.Name)
                        {
                            ZedG.ListTream.RemoveAt(i);
                            if (ZedG.ListTream.Count > 0)
                            {
                                ZedG.SelTream = ZedG.ListTream[Math.Max(i - 1, 0)];
                            }
                            else
                            {
                                ZedG.SelTream = null;
                            }
                            FillCurTreeList();
                        }
                    }
                    break;
            }
        }

        private void frmC曲线选择变量_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str1 = listBox1.SelectedItem.ToString();
            foreach (CLSTeamCurve nTeam in ZedG.ListTream)
            {
                if (str1 == nTeam.Name)
                {
                    ZedG.SelTream = nTeam;
                    ShowSelTeam();
                    return;
                }
            }
        }

        private void SaveToXML()//保存到XML
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sWinsPath = CProject.sPrjPath + "\\Project\\曲线显示.xml";
            myxmldoc.Load(sWinsPath);

            //取图元
            string xpath = "FormWindow/Misc";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                string sNodeName = item.Name;
                string str1 = sNodeName.Substring(0, 2);
                if (sNodeName == "KJTideCurves")
                {
                    foreach (XmlElement TYNode in item.ChildNodes)
                    {
                        ZedG.SaveToXML(TYNode, myxmldoc);
                    }
                }
            }
            myxmldoc.Save(sWinsPath);
        }

        private void butApply_Click(object sender, EventArgs e)//应用
        {
            try
            {
                butApply.Enabled = false;
                ZedG.SelTream.ListYAxisGroup.Clear();
                for (int i = 0; i < dGV1.RowCount; i++)
                {
                    if (dGV1.Rows[i].Cells[0].Value == null)
                        continue;
                    string str0 = dGV1.Rows[i].Cells[0].Value.ToString();
                    if (str0.Substring(0, 1) == "第")
                    {
                        int j = Convert.ToInt32(str0.Substring(1, 1));
                        CLSYAxisGroup cCurG = new CLSYAxisGroup(dGV1.Rows[i].Cells[1].Value.ToString());
                        cCurG.FontColor = dGV1.Rows[i].Cells[2].Style.BackColor;
                        ZedG.SelTream.ListYAxisGroup.Add(cCurG);
                    }
                    else
                    {
                        int j = Convert.ToInt32(str0);
                        ListCur[j - 1].Text = dGV1.Rows[i].Cells[1].Value.ToString();
                        ListCur[j - 1].LineColor = dGV1.Rows[i].Cells[2].Style.BackColor;
                        ZedG.SelTream.ListYAxisGroup[ZedG.SelTream.ListYAxisGroup.Count - 1].ListCur.Add(ListCur[j - 1]);
                    }
                }
                SaveToXML();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
