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
using System.Xml;
using ZedGraph;

namespace LSSCADA.Control
{
    public partial class frmC曲线刻度设置 : Form
    {
        string[] sCol2 = new string[] { "序", "Y轴名称", "最小刻度", "最大刻度" ,"颜色"};
        bool[] bCol2 = new bool[] { true, false, false, false, true };
        public CLSChart ZedG;

        public frmC曲线刻度设置()
        {
            InitializeComponent();
        }

        public frmC曲线刻度设置(CLSChart ZedG1,bool bCur)
        {
            InitializeComponent();
            ZedG = ZedG1;
            if (bCur)
            {
                this.Size = new Size(501, 380);
                PaneFill.BackColor = ZedG.PaneFill;
                ChartFill.BackColor = ZedG.ChartFill;
                LegendFill.BackColor = ZedG.LegendFill;
                GridColor.BackColor = ZedG.GridColor;

                XAxisTitleShow.Checked = ZedG.XAxisTitleShow;
                XAxisTitle.Text = ZedG.XAxisTitle;
                XAxisFormat.Text = ZedG.XAxisFormat;
                CurveLineWith.Value = ZedG.CurveLineWith;
                groupBox2.Dock = DockStyle.Fill;
                groupBox1.Visible = false;
                button2.Visible = false;
                FillDGV2();
            }
            else
            {
                this.Size = new Size(400, 300);
                groupBox1.Dock = DockStyle.Fill;
                groupBox2.Visible = false;
                button1.Visible = false;
                CPublicDGV.InitializeDGV(dGV1, sCol2, bCol2, true);
                dGV1.AllowUserToAddRows = false;//禁止用户新建行
                dGV1.RowHeadersVisible = false; //去掉左侧的行标题
                for (int i = 0; i < dGV1.ColumnCount; i++)
                {
                    dGV1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                FillDGV1();
            }
        }

        private void FillDGV1()
        {
            for (int ij = 0; ij < dGV1.Columns.Count; ij++)
            {
                dGV1.Columns[ij].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dGV1.Rows.Clear();
            dGV1.RowCount = ZedG.SelTream.ListYAxisGroup.Count;
            int i = 0;
            foreach (CLSYAxisGroup YGroup in ZedG.SelTream.ListYAxisGroup)
            {

                dGV1.Rows[i].Cells[0].Value = i + 1;
                dGV1.Rows[i].Cells[1].Value = YGroup.Text;
                if (YGroup.ScaleMinAuto)
                    dGV1.Rows[i].Cells[2].Value = "Auto";
                else
                    dGV1.Rows[i].Cells[2].Value = YGroup.ScaleMin.ToString();
                if (YGroup.ScaleMaxAuto)
                    dGV1.Rows[i].Cells[3].Value = "Auto";
                else
                    dGV1.Rows[i].Cells[3].Value = YGroup.ScaleMax.ToString();
                dGV1.Rows[i].Cells[4].Style.BackColor = YGroup.FontColor;
                i++;
            }
        }
        private void FillDGV2()
        {
            for (int ij = 0; ij < dGV2.Columns.Count; ij++)
            {
                dGV2.Columns[ij].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dGV2.Rows.Clear();
            dGV2.RowCount = ZedG.ListCurF.Count;
            dGV2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            int i = 0;
            foreach (CCurveFont nCurF in ZedG.ListCurF.Values)
            {
                FillDGV2Row(i++,  nCurF);
            }
        }
        private void FillDGV2Row(int iRow, CCurveFont nCurF)
        {
            dGV2.Rows[iRow].Cells[0].Value = nCurF.Desc;
            dGV2.Rows[iRow].Cells[1].Value = nCurF.sFont;
            dGV2.Rows[iRow].Cells[1].Style.Font = nCurF.nFont;
            dGV2.Rows[iRow].Cells[1].Style.BackColor = nCurF.FillColor;
            dGV2.Rows[iRow].Cells[1].Style.ForeColor = nCurF.FontColor;
            dGV2.Rows[iRow].Cells[2].Value = nCurF.IsFillVisible;
            dGV2.Rows[iRow].Cells[3].Value = nCurF.sFillColor;
            dGV2.Rows[iRow].Cells[3].Style.BackColor = nCurF.FillColor;
            dGV2.Rows[iRow].Cells[3].Style.ForeColor = nCurF.FontColor;
            dGV2.Rows[iRow].Cells[4].Value = nCurF.IsScaled;
        }

        string sOld = "";
        private void dGV1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            sOld = dGV1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }

        private void dGV1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            int iCol = e.ColumnIndex;
            switch (e.ColumnIndex)
            {
                case 2:
                    try
                    {
                        double dV2 = Convert.ToDouble(dGV1.Rows[iRow].Cells[iCol].Value.ToString());
                        try
                        {
                            double dV3 = Convert.ToDouble(dGV1.Rows[iRow].Cells[iCol + 1].Value.ToString());
                            if (dV2 > dV3)
                            {
                                MessageBox.Show("最小刻度不能大于最大刻度", "错误");
                                dGV1.Rows[iRow].Cells[iCol].Value = sOld;
                            }
                        }
                        catch
                        { }
                    }
                    catch (Exception ex)
                    {
                        dGV1.Rows[iRow].Cells[iCol].Value = "Auto";
                    }
                    break;
                case 3:
                    try
                    {
                        double dV3 = Convert.ToDouble(dGV1.Rows[iRow].Cells[iCol].Value.ToString());
                        try
                        {
                            double dV2 = Convert.ToDouble(dGV1.Rows[iRow].Cells[iCol - 1].Value.ToString());
                            if (dV2 > dV3)
                            {
                                MessageBox.Show("最大刻度不能小于最小刻度", "错误");
                                dGV1.Rows[iRow].Cells[iCol].Value = sOld;
                            }
                        }
                        catch
                        { }
                    }
                    catch (Exception ex)
                    {
                        dGV1.Rows[iRow].Cells[iCol].Value = "Auto";
                    }
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)//刷新显示
        {
            FillDGV1();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < dGV1.RowCount; i++)
            {
                ZedG.SelTream.ListYAxisGroup[i].Text = dGV1.Rows[i].Cells[1].Value.ToString();
                try
                {
                    if (dGV1.Rows[i].Cells[2].Value.ToString() == "Auto")
                        ZedG.SelTream.ListYAxisGroup[i].ScaleMinAuto = true;
                    else
                    {
                        ZedG.SelTream.ListYAxisGroup[i].ScaleMinAuto = false;
                        ZedG.SelTream.ListYAxisGroup[i].ScaleMin = Convert.ToDouble(dGV1.Rows[i].Cells[2].Value.ToString());
                    }
                }
                catch
                {
                    ZedG.SelTream.ListYAxisGroup[i].ScaleMinAuto = true;
                }

                try
                {
                    if (dGV1.Rows[i].Cells[3].Value.ToString() == "Auto")
                        ZedG.SelTream.ListYAxisGroup[i].ScaleMaxAuto = true;
                    else
                    {
                        ZedG.SelTream.ListYAxisGroup[i].ScaleMaxAuto = false;
                        ZedG.SelTream.ListYAxisGroup[i].ScaleMax = Convert.ToDouble(dGV1.Rows[i].Cells[3].Value.ToString());
                    }
                }
                catch
                {
                    ZedG.SelTream.ListYAxisGroup[i].ScaleMaxAuto = true;
                }
                ZedG.SelTream.ListYAxisGroup[i].FontColor = dGV1.Rows[i].Cells[4].Style.BackColor;
            }
            SaveToXML();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox nPic = (PictureBox)sender;
            ColorDialog aaa = new ColorDialog();
            aaa.Color = nPic.BackColor;
            if (aaa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                nPic.BackColor = aaa.Color;
            }
        }

        //保存曲线色织
        private void button1_Click_1(object sender, EventArgs e)
        {
            ZedG.PaneFill = PaneFill.BackColor;
            ZedG.ChartFill = ChartFill.BackColor;
            ZedG.LegendFill = LegendFill.BackColor;
            ZedG.GridColor = GridColor.BackColor;

            ZedG.XAxisTitleShow = XAxisTitleShow.Checked;
            ZedG.XAxisTitle = XAxisTitle.Text;
            ZedG.XAxisFormat = XAxisFormat.Text;
            ZedG.CurveLineWith = (int)CurveLineWith.Value;
            for (int i = 0; i < dGV2.RowCount; i++)
            {
                string sDesc = dGV2.Rows[i].Cells[0].Value.ToString();
                foreach (CCurveFont nCurF in ZedG.ListCurF.Values)
                {
                    if (sDesc == nCurF.Desc)
                    {
                        nCurF.IsFillVisible =Convert.ToBoolean( dGV2.Rows[i].Cells[2].Value);
                        nCurF.IsScaled = Convert.ToBoolean(dGV2.Rows[i].Cells[4].Value);
                        break;
                    }
                }
            }
            SaveToXML();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void dGV2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != 1 && e.ColumnIndex != 3)
                    return;
                string sDesc = dGV2.Rows[e.RowIndex].Cells[0].Value.ToString();
                foreach (CCurveFont nCurF in ZedG.ListCurF.Values)
                {
                    if (sDesc == nCurF.Desc)
                    {
                        if (e.ColumnIndex == 1)
                        { dGV2Edit1(nCurF,e.RowIndex); }
                        else if (e.ColumnIndex == 3)
                        { dGV2Edit3(nCurF, e.RowIndex); }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "dGV2_CellDoubleClick");
            }
        }
        private void dGV2Edit1(CCurveFont nCurF,int iRow)//修改字体
        {
            FontDialog nfont = new FontDialog();
            nfont.Color = nCurF.FontColor;
            nfont.ShowColor = true;
            nfont.Font = nCurF.nFont;
            if (nfont.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                nCurF.nFont = nfont.Font;
                nCurF.FontColor = nfont.Color;
                dGV2.Rows[iRow].Cells[1].Value = nCurF.sFont;
                dGV2.Rows[iRow].Cells[1].Style.Font = nCurF.nFont;
                dGV2.Rows[iRow].Cells[1].Style.BackColor = nCurF.FillColor;
                dGV2.Rows[iRow].Cells[1].Style.ForeColor = nCurF.FontColor;
                dGV2.Rows[iRow].Cells[3].Style.ForeColor = nCurF.FontColor;
            }
        }
        private void dGV2Edit3(CCurveFont nCurF, int iRow)//修改颜色
        {
            ColorDialog nColor = new ColorDialog();
            nColor.Color = nCurF.FillColor;
            if (nColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                nCurF.FillColor = nColor.Color;
                dGV2.Rows[iRow].Cells[1].Style.BackColor = nCurF.FillColor;
                dGV2.Rows[iRow].Cells[3].Style.BackColor = nCurF.FillColor;
                dGV2.Rows[iRow].Cells[3].Value = nCurF.sFillColor;
            }
        }

        private void dGV1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 4)
                return;
            ColorDialog aaa = new ColorDialog();
            aaa.Color = dGV1.Rows[e.RowIndex].Cells[4].Style.BackColor;
            if (aaa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dGV1.Rows[e.RowIndex].Cells[4].Style.BackColor = aaa.Color;
            }
        }
    }

    public class CCurveFont
    {
        public Font nFont = new Font("宋体",11);
        public string Desc = "";
        public string Name = "";
        public Color FontColor = Color.White;
        public Color FillColor = Color.Black;
        public bool IsFillVisible = false;
        public bool IsScaled = false;

        public CCurveFont(){ }

        public CCurveFont(string  sFamily,int iSize,Color cor)
        {
            nFont = new Font(sFamily, iSize);
            FontColor= cor;
        }
        public string sFont
        {
            get
            {
                FontConverter fc = new FontConverter();
                return fc.ConvertToString(nFont);
            }
        }
        public string sFontColor
        {
            get
            {
                return ColorTranslator.ToHtml(FontColor);
            }
        }
        public string sFillColor
        {
            get
            {
                return ColorTranslator.ToHtml(FillColor);
            }
        }
        public FontSpec FontSpec
        {
            get
            {
                FontSpec spec = new FontSpec(nFont.Name, nFont.Size, FillColor, nFont.Bold, nFont.Italic, nFont.Underline);
                spec.FontColor = FontColor;
                spec.Fill.Color = FillColor;
                spec.Fill.IsScaled = IsScaled;
                spec.Border.IsVisible = false;
                if (!IsFillVisible)
                    spec.Fill.IsVisible = false;
                else
                    spec.Fill = new Fill(new SolidBrush(FillColor));
                return spec;
            }
        }
    }
}
