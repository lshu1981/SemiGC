using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Drawing2D;
using ZedGraph;
using System.Diagnostics;
using LSSCADA.Database;
using CABC;

namespace LSSCADA.Control
{

    public partial class frmC数据监视 : Form
    {
        CLSChart ZedG1 = new CLSChart();
        /// <summary>
        /// 曲线模式 0实时曲线 1配方曲线 2历史曲线
        /// </summary>
        int ChartMode = 2;
        int iRealSec = 0;//实时曲线长度
        int iRecipeSec = 0;//配方延时长度
        DateTime DT_S, DT_E; //历史曲线的时间节点
        CSelRecipe nSelRecipe;//配方曲线
        public frmC数据监视()
        {
            InitializeComponent();
        }

        public frmC数据监视(Form _Owner)
        {
            InitializeComponent();

            this.MdiParent = _Owner;
            Size aa = this.MdiParent.Size;
            this.Size = new System.Drawing.Size(aa.Width - 20, aa.Height - frmMain.iToolHeight);
            LoadWins();

            this.tabPage4.Controls.Add(ZedG1.zedG);
            ZedG1.zedG.Dock = DockStyle.Fill;
            ZedG1.zedG.MouseMoveEvent += new ZedGraphControl.ZedMouseEventHandler(zedG_MouseMoveEvent);
            ZedG1.zedG.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(zedG_ContextMenuBuilder);

            dTP1.Value = new DateTime(2018, 1, 5, 0, 0, 0);
            dTP2.Value = new DateTime(2011, 1, 1, 0, 0, 0);

            listBox1.Items.Clear();
            foreach (CLSTeamCurve nTeam in ZedG1.ListTream)
            {
                listBox1.Items.Add(nTeam.Name);
            }
            if (ZedG1.iSelTeamIndex < listBox1.Items.Count)
                listBox1.SelectedIndex = ZedG1.iSelTeamIndex;
            else
                listBox1.SelectedIndex = 0;
            this.DoubleBuffered = true;
        }

        void zedG_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                if ((string)item.Tag == "show_val")// “恢复默认大小”菜单项
                {
                    menuStrip.Items.Remove(item);//移除菜单项

                    item.Visible = false; //不显示

                    break;
                }
            }
            ToolStripSeparator Line1 = new ToolStripSeparator();
            ToolStripMenuItem Y轴刻度范围 = new ToolStripMenuItem("Y轴刻度范围");
            ToolStripMenuItem 编辑曲线组 = new ToolStripMenuItem("编辑曲线组");
            ToolStripMenuItem 曲线个性定制 = new ToolStripMenuItem("曲线参数设置");
            Y轴刻度范围.Click += new EventHandler(Y轴刻度范围_Click);
            编辑曲线组.Click += new EventHandler(编辑曲线组_Click);
            曲线个性定制.Click += new EventHandler(曲线个性定制_Click);
            //menuStrip.Items.Clear();
            menuStrip.Items.Insert(0, Line1);
            menuStrip.Items.Insert(0, Y轴刻度范围);
            menuStrip.Items.Insert(0, 编辑曲线组);
            menuStrip.Items.Insert(0, 曲线个性定制);
            ToolStripMenuItem newitem = new ToolStripMenuItem("显示节点数据");
           // newitem.CheckState = CheckState.Unchecked;
            newitem.Checked = bShowToolTip;
            newitem.Click += new EventHandler(显示节点数据_Click);
            menuStrip.Items.Add(newitem);
        }

        void 曲线个性定制_Click(object sender, EventArgs e)
        {
            frmC曲线刻度设置 nSet = new frmC曲线刻度设置(ZedG1, true);
            nSet.ShowDialog();
            if (nSet.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ZedG1.DrawChart();
            }
        }

        void 编辑曲线组_Click(object sender, EventArgs e)
        {
            frmC曲线选择变量 nSet = new frmC曲线选择变量(ZedG1, "butEdit");
            nSet.ShowDialog();
            if (nSet.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ZedG1.SelTream.ListYAxisGroup = nSet.ListCurGroup;
                ZedG1.DrawChart();
            }
        }

        void Y轴刻度范围_Click(object sender, EventArgs e)
        {
            frmC曲线刻度设置 nSet = new frmC曲线刻度设置(ZedG1, false);
            nSet.ShowDialog();
            if (nSet.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                ZedG1.DrawChart();
            }
        }

        void 显示节点数据_Click(object sender, EventArgs e)
        {
            bShowToolTip = !bShowToolTip;
            ToolStripMenuItem newitem = (ToolStripMenuItem)sender;
            newitem.Checked = bShowToolTip;
            if(bShowToolTip)
            newitem.CheckState = CheckState.Checked;
            else
                newitem.CheckState = CheckState.Unchecked;
        }

        #region MouseMove Events
        PointF mouseOldPt = new PointF(0,0);
        bool bShowToolTip = false;
        bool zedG_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            PointF mousePt = new PointF(e.X, e.Y);
            if (mouseOldPt == mousePt)
                return false;

            mouseOldPt = mousePt;
            if (bShowToolTip)
            {
                HandlePointValues(mousePt, sender);
            }
            else
            {
                toolTip1.Active = false;
            }
            return false;
        }
        /// <summary>
        /// Make a string label that corresponds to a user scale value.
        /// </summary>
        /// <param name="axis">The axis from which to obtain the scale value.  This determines
        /// if it's a date value, linear, log, etc.</param>
        /// <param name="val">The value to be made into a label</param>
        /// <param name="iPt">The ordinal position of the value</param>
        /// <param name="isOverrideOrdinal">true to override the ordinal settings of the axis,
        /// and prefer the actual value instead.</param>
        /// <returns>The string label.</returns>
        protected string MakeValueLabel(Axis axis, double val, int iPt, bool isOverrideOrdinal)
        {
            if (axis != null)
            {
                if (axis.Scale.IsDate || axis.Scale.Type == AxisType.DateAsOrdinal)
                {
                    return XDate.ToString(val, "yyyy-MM-dd HH:mm:ss");
                }
                else if (axis.Scale.IsAnyOrdinal && axis.Scale.Type != AxisType.LinearAsOrdinal
                                && !isOverrideOrdinal)
                {
                    return iPt.ToString("f2");
                }
                else
                    return val.ToString("f2");
            }
            else
                return "";
        }
        private bool HandlePointValues(PointF mousePt, ZedGraphControl sender)
        {
            int iPt;
            GraphPane pane;
            object nearestObj;

            using (Graphics g = this.CreateGraphics())
            {

                if (sender.MasterPane.FindNearestPaneObject(mousePt,
                    g, out pane, out nearestObj, out iPt))
                {
                    if (nearestObj is CurveItem && iPt >= 0)
                    {
                        CurveItem curve = (CurveItem)nearestObj;
                        // Provide Callback for User to customize the tooltips
                        PointPair pt = curve.Points[iPt];

                        if (pt.Tag is string)
                            this.toolTip1.SetToolTip(this, (string)pt.Tag);
                        else
                        {
                            double xVal, yVal, lowVal;
                            ValueHandler valueHandler = new ValueHandler(pane, false);
                            if ((curve is BarItem || curve is ErrorBarItem || curve is HiLowBarItem)
                                    && pane.BarSettings.Base != BarBase.X)
                                valueHandler.GetValues(curve, iPt, out yVal, out lowVal, out xVal);
                            else
                                valueHandler.GetValues(curve, iPt, out xVal, out lowVal, out yVal);

                            string xStr = MakeValueLabel(curve.GetXAxis(pane), xVal, iPt, curve.IsOverrideOrdinal);
                            string yStr = MakeValueLabel(curve.GetYAxis(pane), yVal, iPt, curve.IsOverrideOrdinal);

                            this.toolTip1.SetToolTip(sender, xStr + "\n" + yStr);
                            this.toolTip1.ForeColor = curve.Color;
                            this.toolTip1.BackColor = Color.Black;
                            toolTip1.Active = true;
                            //this.pointToolTip.SetToolTip( this,
                            //  curve.Points[iPt].ToString( this.pointValueFormat ) );
                        }
                    }
                    else
                        toolTip1.Active = false;

                }
                else
                    toolTip1.Active = false;

                //g.Dispose();
            }
            return true;
        }
        #endregion
        public void LoadWins()//读取窗口文件
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sWinsPath = System.Environment.CurrentDirectory + "\\Project\\曲线显示.xml";
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
                        ZedG1.LoadFromXML(TYNode);
                    }
                }
            }//foreach (XmlElement item in childNode.ChildNodes)
        }

        public void SaveSelTeamToWins()//读取窗口文件
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sWinsPath = System.Environment.CurrentDirectory + "\\Project\\曲线显示.xml";
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
                        ZedG1.SaveSelTeamToWins(TYNode);
                        break;
                    }
                }
            }//foreach (XmlElement item in childNode.ChildNodes)
            myxmldoc.Save(sWinsPath);
        }

        int iOldSecond = -1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ChartMode != 0)
                    return;

                if (iOldSecond != frmMain.staComm.iOldSec)
                {
                    iOldSecond = frmMain.staComm.iOldSec;
                    if (ZedG1.SelTream != null)
                    {
                        DateTime DT = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        ZedG1.SelTream.UpdateReal(new XDate(DT));
                        ZedG1.zedG.AxisChange();
                        ZedG1.zedG.Refresh();
                        //HisdGV1.DataSource = GetChartDT();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("数据监视.timer1_Tick" + ex.Message);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Enabled = false;
            string str1 = listBox1.Text;
            foreach (CLSTeamCurve nTeam in ZedG1.ListTream)
            {
                if (nTeam.Name == str1)
                {
                    ZedG1.SelTream = nTeam;
                    if (ZedG1.iSelTeamIndex != listBox1.SelectedIndex)
                    {
                        ZedG1.iSelTeamIndex = listBox1.SelectedIndex;
                        SaveSelTeamToWins();
                    }
                    if (ChartMode == 0)
                    {
                        DrawChart();
                        //HisdGV1.DataSource = GetChartDT();
                    }
                    listBox1.Enabled = true;
                    return;
                }
            }
            listBox1.Enabled = true;
        }

        private void butShowHis_Click(object sender, EventArgs e)//历史曲线
        {
            ChartMode = 2;
            iRealSec = -1;
            GetDTSel();
            DrawChart();
        }
        //读取时间选项
        private void GetDTSel()
        {
            DT_S = new DateTime(dTP1.Value.Year, dTP1.Value.Month, dTP1.Value.Day, dTP2.Value.Hour, dTP2.Value.Minute, dTP2.Value.Second);
            DT_E = DT_S.AddYears((int)nMY.Value);
            DT_E = DT_E.AddMonths((int)nMM.Value);
            DT_E = DT_E.AddDays((int)nMD.Value);
            DT_E = DT_E.AddHours((int)nMH.Value);
            DT_E = DT_E.AddMinutes((int)nMMin.Value);
        }

        private void DrawChart()
        {
            Cursor.Current = Cursors.WaitCursor;
            switch (ChartMode)
            {
                case 0://实时曲线
                    ZedG1.XAxisFormat = "HH:mm:ss";
                    ZedG1.TitleText = "实时曲线：" +ZedG1.SelTream.Name + "(" + (iRealSec/60).ToString() + "分钟)";
                    ZedG1.SelTream.UpdateReal(DateTime.Now, DateTime.Now, iRealSec);
                    iOldSecond = DateTime.Now.Second;
                    break;
                case 1://配方曲线
                    ZedG1.XAxisFormat = "yy-MM-dd\r\nHH:mm:ss";
                    ZedG1.TitleText = nSelRecipe.Name+"：" + ZedG1.SelTream.Name + "\r\n(" + nSelRecipe.DT_S.ToString("yyyy-MM-dd HH:mm:ss") + "—" + nSelRecipe.DT_E.ToString("yyyy-MM-dd HH:mm:ss") + ")";
                    ZedG1.SelTream.UpdateReal(nSelRecipe.DT_S.AddSeconds(-1 * iRecipeSec), nSelRecipe.DT_E.AddSeconds( iRecipeSec), iRealSec);
                    break;
                case 2://历史曲线
                    ZedG1.XAxisFormat = "yy-MM-dd\r\nHH:mm:ss";
                    ZedG1.TitleText = "历史曲线:" + ZedG1.SelTream.Name + "\r\n(" + DT_S.ToString("yyyy-MM-dd HH:mm:ss") + "—" + DT_E.ToString("yyyy-MM-dd HH:mm:ss") + ")";
                    ZedG1.SelTream.UpdateReal(DT_S, DT_E, iRealSec);
                    break;
                default:
                    return;
            }
            ZedG1.DrawChart();
            /*double dNow = new XDate(DateTime.Now);
            double dS = new XDate(DateTime.Now.AddSeconds(-1 * iRealSec));
            ZedG1.zedG.GraphPane.XAxis.Scale.MinAuto = false;
            ZedG1.zedG.GraphPane.XAxis.Scale.Min = dS;
            ZedG1.zedG.GraphPane.XAxis.Scale.MaxAuto = false;
            ZedG1.zedG.GraphPane.XAxis.Scale.Max = dNow;*/
            HisdGV1.DataSource = GetChartDT();
            HisdGV1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            Cursor.Current = Cursors.Default;
        }

        private DataTable GetChartDT()
        {
            DataTable nDT = new DataTable("ChartDT");
            int iColNum = 0;
            int iRowNum = 0;
            foreach (CLSYAxisGroup nGroup in ZedG1.SelTream.ListYAxisGroup)
            {
                foreach (CLSCurve nCur in nGroup.ListCur)
                {
                    iRowNum = Math.Max(iRowNum, nCur.ListPT.Count);
                    DataColumn nDTCol1 = new DataColumn(nCur.Text + ":时间", typeof(string));
                    DataColumn nDTCol2 = new DataColumn(nCur.Text + ":值", typeof(string));
                    nDT.Columns.Add(nDTCol1);
                    nDT.Columns.Add(nDTCol2);
                    iColNum++;
                    iColNum++;
                }
            }
            for (int i = 0; i < iRowNum; i++)
            {
                nDT.Rows.Add(nDT.NewRow());
            }
            int k = 0;
            foreach (CLSYAxisGroup nGroup in ZedG1.SelTream.ListYAxisGroup)
            {
                foreach (CLSCurve nCur in nGroup.ListCur)
                {
                    for (int j = 0; j < nCur.ListPT.Count; j++) 
                    {
                        nDT.Rows[j][k] = XDate.ToString(nCur.ListPT[j].X, "yyyy-MM-dd HH:mm:ss");
                        nDT.Rows[j][k + 1] = nCur.ListPT[j].Y.ToString("f2");
                    }
                    k++;
                    k++;
                }
            }

            return nDT;
        }

        private void butShowRepice_Click(object sender, EventArgs e)
        {
            if(nSelRecipe !=null)
                DrawChart();
        }

        private void butUpdateDGV_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            butUpdateDGV.Enabled = false;
            HisdGV1.DataSource = GetChartDT();
            butUpdateDGV.Enabled = true;
            Cursor.Current = Cursors.Default;
        }
    }

}
