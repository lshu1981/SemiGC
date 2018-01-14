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

namespace LSSCADA.Control
{
    public class CLSChart
    {
        public string TitleText = "";

        public Dictionary<string, CCurveFont> ListCurF = new Dictionary<string, CCurveFont>();
        public CCurveFont TitleFont { get { return ListCurF["TitleFont"]; } }//图表标题字体
        public CCurveFont LegendFont { get { return ListCurF["LegendFont"]; } }//X坐标轴标题字体
        public CCurveFont AxisXTitle { get { return ListCurF["AxisXTitle"]; } }	//X坐标轴标题字体
        public CCurveFont AxisXScale { get { return ListCurF["AxisXScale"]; } }	//X坐标轴刻度字体
        public CCurveFont AxisYTitle { get { return ListCurF["AxisYTitle"]; } }	//Y坐标轴标题字体
        public CCurveFont AxisYScale { get { return ListCurF["AxisYScale"]; } } //Y坐标轴刻度字体

        public Color LegendFill = Color.White;        //图例背景色
        public Color GridColor = Color.White;               //网格颜色
        public Color PaneFill = Color.DimGray;              //边框背景色
        public Color ChartFill = Color.Black;     //绘图区背景色
        public int CurveLineWith = 1;                       //曲线宽度

        public bool bXMajorGridVis = true;
        public bool bYMajorGridVis = true;

        public string XAxisFormat = "HH:mm:ss";
        public string XAxisTitle = "采用时间";
        public bool XAxisTitleShow = true;                //显示X轴标题

        public int iSelTeamIndex = 0;

        public List<CLSTeamCurve> ListTream = new List<CLSTeamCurve>();
        public CLSTeamCurve SelTream = new CLSTeamCurve();
        public ZedGraphControl zedG = new ZedGraphControl();
        GraphPane myPane = new GraphPane();
        public CLSChart()
        {
            zedG.GraphPane = myPane;
        }

        public void DrawChart()
        {
            DrawPane();
            DrawYAxis();
        }
        public void DrawPane()
        {
            myPane = new GraphPane();
            zedG.GraphPane = myPane;
            //边框
            myPane.Rect = new RectangleF(0, 0, zedG.Width, zedG.Height);
            // myPane.LineType = LineType.Stack;
            myPane.Fill = new Fill(PaneFill);//边框底色

            //标题
            myPane.Title.FontSpec = TitleFont.FontSpec;
            myPane.Title.Text = TitleText;

            //图例
            myPane.Legend.Fill = new Fill(new SolidBrush(LegendFill));//图例填充
            myPane.Legend.FontSpec = LegendFont.FontSpec;
            myPane.Legend.Fill.IsScaled = false;
            myPane.Legend.Border.IsVisible = false;
            // Turn off the axis background fill

            myPane.Chart.Fill = new Fill(new SolidBrush(ChartFill));
            myPane.Chart.Border.Color = GridColor;
            // Hide the legend隐藏图例
            //myPane.Legend.IsVisible = false;
            // myPane.Legend.Fill = new Fill(Color.Black);
            // myPane.Legend.FontSpec.FontColor = CaptionColor;
            // myPane.Legend.IsShowLegendSymbols = true;
            // Set the colors to white show it shows up on a dark background
            //myPane.Title.FontSpec.Family = CaptionFont.FontFamily;

            //X轴坐标
            myPane.XAxis.Title.IsVisible = XAxisTitleShow;  //是否显示X轴标题
            myPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;
            myPane.XAxis.Title.FontSpec = AxisXTitle.FontSpec;
            myPane.XAxis.Color = GridColor;

            //X轴刻度
            myPane.XAxis.Scale.Format = XAxisFormat;
            myPane.XAxis.Scale.FontSpec = AxisXScale.FontSpec;

            //myPane.XAxis.Scale.MajorStepAuto = false;//刻度步长自动设置
            //myPane.XAxis.Scale.MajorStep = 120;     //大刻度步长

            // myPane.XAxis.Scale.MaxAuto
            //myPane.XAxis.Title.FontSpec.FontColor = AxisColor;
            //myPane.XAxis.Title.FontSpec.Fill.IsScaled = false;//X轴标题大小随曲线大小变化
            //myPane.XAxis.Title.FontSpec.Size = 12;      //X轴标题
            //myPane.XAxis.Title.FontSpec.Fill.IsScaled = AxisXTitle.Fill.IsScaled;
            //myPane.XAxis.Title.FontSpec.Size = AxisXTitle.Size;
            //myPane.XAxis.Scale.FontSpec.Fill.IsScaled = AxisXScale.Fill.IsScaled;
            //myPane.XAxis.Scale.FontSpec.Size = AxisXScale.Size;
            //myPane.XAxis.Scale.FontSpec.FontColor = AxisXScale.FontColor;
            //myPane.XAxis.Scale.FontSpec.FontColor = AxisColor;
            //myPane.XAxis.Scale.FontSpec.Fill.IsScaled = false;//X轴刻度大小随曲线大小变化
            //myPane.XAxis.Scale.FontSpec.Size = 10;      //X轴刻度大小

            myPane.XAxis.MajorGrid.IsVisible = bXMajorGridVis;//显示主网格
            myPane.XAxis.MajorGrid.Color = GridColor;//主网格颜色
            myPane.XAxis.MajorGrid.IsZeroLine = true;
            myPane.XAxis.MinSpace = 10f;
            myPane.XAxis.MinorTic.Color = GridColor;
            myPane.XAxis.MajorTic.Color = GridColor;
            //myPane.XAxis.Scale.Align = AlignP.Outside;
            //AxisYScale.Angle = 90;
            //myPane.YAxis.Color = AxisColor;
            myPane.YAxis.MajorGrid.Color = GridColor;//主网格颜色
            myPane.YAxis.MajorGrid.IsVisible = bYMajorGridVis;//显示主网格
            //myPane.XAxis.MajorGrid.DashOn = 5f;//主网格  线长 默认为1
            //myPane.XAxis.MajorGrid.DashOff = 5f;//主网格 线间距 默认为5
            // Show the grid lines
            zedG.PointDateFormat = "yyyy-MM-dd HH:mm:ss";
            zedG.PointValueFormat = "f2";
        }

        public void DrawYAxis()
        {
            myPane.CurveList.Clear();
            if (SelTream.ListYAxisGroup.Count < 1)
            {
                zedG.AxisChange();
                zedG.Refresh();
                return;
            }
            myPane.XAxis.Title.Text = XAxisTitle;
            for (int i = 0; i < SelTream.ListYAxisGroup.Count; i++)
            {
                CLSYAxisGroup cCur = SelTream.ListYAxisGroup[i];
                if (i == 0)
                {
                    // Make the Y axis scale red
                    SetYAxis(myPane.YAxis, AxisYTitle.FontSpec, AxisYScale.FontSpec, cCur);
                }
                else if (i == 1)
                {
                    SetYAxis(myPane.Y2Axis, AxisYTitle.FontSpec, AxisYScale.FontSpec, cCur);
                }
                else if (i % 2 == 0)
                {
                    YAxis yAxis3 = new YAxis(cCur.Text);
                    SetYAxis(yAxis3, AxisYTitle.FontSpec, AxisYScale.FontSpec, cCur);
                    myPane.YAxisList.Add(yAxis3);
                }
                else if (i % 2 == 1)
                {
                    Y2Axis yAxis3 = new Y2Axis(cCur.Text);
                    SetYAxis(yAxis3, AxisYTitle.FontSpec, AxisYScale.FontSpec, cCur);
                    myPane.Y2AxisList.Add(yAxis3);
                }
                else
                {
                    return;
                }

                int i1 = i % 2;
                int i2 = i / 2;

                foreach (CLSCurve nCur in cCur.ListCur)
                {
                    LineItem myCurve = myPane.AddCurve(nCur.Text, nCur.ListPT, nCur.LineColor, SymbolType.None);
                    myCurve.Symbol.Size = 10;
                    myCurve.Line.Width = CurveLineWith;
                    //myCurve.Line.Style = DashStyle.DashDotDot;
                    // Fill the symbols with white
                    //myCurve.Symbol.Fill = new Fill(Color.White);
                    myCurve.YAxisIndex = i2;
                    if (i1 == 1)
                    {
                        // Associate this curve with the Y2 axis
                        myCurve.IsY2Axis = true;
                    }
                }
            }

            zedG.AxisChange();
            zedG.Refresh();
        }
        private void SetYAxis(YAxis yAxis3, FontSpec YTitle, FontSpec YScale, CLSYAxisGroup cCur)
        {
            try
            {
                yAxis3.Title.Text = cCur.Text;
                yAxis3.Title.FontSpec = YTitle.Clone();
                yAxis3.Scale.FontSpec = YScale.Clone();
                yAxis3.Scale.FontSpec.Angle = 90;

                yAxis3.Scale.FontSpec.FontColor = cCur.FontColor;//刻度字体颜色
                yAxis3.Title.FontSpec.FontColor = cCur.FontColor;//标题字体颜色
                yAxis3.MajorTic.Color = GridColor;                  //主网格刻度尺颜色
                yAxis3.Color = GridColor;
                yAxis3.MinorTic.Color = GridColor;     //次网格刻度尺颜色
                yAxis3.MajorGrid.IsVisible = true;      //是否显示主网格刻度线
                yAxis3.MajorGrid.Color = GridColor;      //主网格刻度线颜色
                // turn off the opposite tics so the Y2 tics don't show up on the Y axis
                yAxis3.MajorTic.IsInside = true;// false; 
                yAxis3.MinorTic.IsInside = false;
                yAxis3.MajorTic.IsOpposite = false;
                yAxis3.MinorTic.IsOpposite = false;
                // Align the Y2 axis labels so they are flush to the axis
                yAxis3.Scale.Align = AlignP.Inside;
                yAxis3.IsVisible = true;
                yAxis3.Scale.Min = cCur.ScaleMin;
                yAxis3.Scale.MinAuto = cCur.ScaleMinAuto;
                yAxis3.Scale.Max = cCur.ScaleMax;
                yAxis3.Scale.MaxAuto = cCur.ScaleMaxAuto;
            }
            catch (Exception)
            { }
        }
        private void SetYAxis(Y2Axis yAxis3, FontSpec YTitle, FontSpec YScale, CLSYAxisGroup cCur)
        {
            try
            {
                yAxis3.Title.Text = cCur.Text;
                yAxis3.Title.FontSpec = YTitle.Clone();
                yAxis3.Scale.FontSpec = YScale.Clone();
                yAxis3.Scale.FontSpec.Angle = 270;

                yAxis3.Scale.FontSpec.FontColor = cCur.FontColor;//刻度字体颜色
                yAxis3.Title.FontSpec.FontColor = cCur.FontColor;//标题字体颜色
                yAxis3.MajorTic.Color = GridColor;                  //主网格刻度尺颜色
                yAxis3.Color = GridColor;
                yAxis3.MinorTic.Color = GridColor;     //次网格刻度尺颜色
                yAxis3.MajorGrid.IsVisible = true;      //是否显示主网格刻度线
                yAxis3.MajorGrid.Color = GridColor;      //主网格刻度线颜色
                // turn off the opposite tics so the Y2 tics don't show up on the Y axis
                yAxis3.MajorTic.IsInside = true;// false; 
                yAxis3.MinorTic.IsInside = false;
                yAxis3.MajorTic.IsOpposite = false;
                yAxis3.MinorTic.IsOpposite = false;
                // Align the Y2 axis labels so they are flush to the axis
                yAxis3.Scale.Align = AlignP.Inside;
                yAxis3.IsVisible = true;
                yAxis3.Scale.Min = cCur.ScaleMin;
                yAxis3.Scale.MinAuto = cCur.ScaleMinAuto;
                yAxis3.Scale.Max = cCur.ScaleMax;
                yAxis3.Scale.MaxAuto = cCur.ScaleMaxAuto;
            }
            catch (Exception)
            { }
        }

        public void LoadFromXML(XmlElement Node)
        {
            try
            {
                //iElementOrder = Convert.ToInt32(CBaseNode.GetAttribute("iElementOrder"));
                //GLayerName = CBaseNode.GetAttribute("GLayerName");
                XmlElement CBaseNode = (XmlElement)(Node.SelectSingleNode("Misc"));
                //Name = CBaseNode.GetAttribute("Name");

                CBaseNode = (XmlElement)(Node.SelectSingleNode("Appearance"));
                TitleText = CBaseNode.GetAttribute("Caption");

                if (CBaseNode.HasAttribute("iSelTeamIndex"))
                {
                    iSelTeamIndex = Convert.ToInt32(CBaseNode.GetAttribute("iSelTeamIndex"));
                }
                if (CBaseNode.HasAttribute("XAxisTitleShow"))
                {
                    XAxisTitle = CBaseNode.GetAttribute("XAxisTitle");
                    XAxisFormat = CBaseNode.GetAttribute("XAxisFormat");
                    XAxisTitleShow = Convert.ToBoolean(CBaseNode.GetAttribute("XAxisTitleShow"));
                }
                if (CBaseNode.HasAttribute("LegendFill"))
                {
                    LegendFill = ColorTranslator.FromHtml(CBaseNode.GetAttribute("LegendFill"));
                    GridColor = ColorTranslator.FromHtml(CBaseNode.GetAttribute("GridColor"));
                    PaneFill = ColorTranslator.FromHtml(CBaseNode.GetAttribute("PaneFill"));
                    ChartFill = ColorTranslator.FromHtml(CBaseNode.GetAttribute("ChartFill"));
                    CurveLineWith = Convert.ToInt32(CBaseNode.GetAttribute("CurveLineWith"));
                }

                ListCurF.Clear();
                foreach (XmlElement node in CBaseNode.ChildNodes)
                {
                    LoadFontFromXML(node);
                }

                CBaseNode = (XmlElement)(Node.SelectSingleNode("Behavior"));

                bXMajorGridVis = Convert.ToBoolean(CBaseNode.GetAttribute("bXMajorGridVis"));
                bYMajorGridVis = Convert.ToBoolean(CBaseNode.GetAttribute("bYMajorGridVis"));

                foreach (XmlElement node in CBaseNode.ChildNodes)
                {
                    CLSTeamCurve nTream = new CLSTeamCurve();
                    nTream.LoadFromXML(node);
                    ListTream.Add(nTream);
                }
                if (ListTream.Count > 0)
                {
                    SelTream = ListTream[0];
                    //DrawChart();
                }
            }
            catch (Exception e)
            {

            }
        }

        public void LoadFontFromXML(XmlElement CBaseNode)
        {
            if (CBaseNode != null)
            {
                FontConverter fc = new FontConverter();
                CCurveFont spec = new CCurveFont();
                spec.nFont = new Font("宋体", 11);
                string sName = CBaseNode.GetAttribute("Name");
                try { spec.nFont = (Font)fc.ConvertFromString(CBaseNode.GetAttribute("Font")); }
                catch (Exception) { }
                spec.Name = sName;
                spec.Desc = CBaseNode.GetAttribute("Desc");
                spec.FontColor = ColorTranslator.FromHtml(CBaseNode.GetAttribute("FontColor"));
                spec.FillColor = ColorTranslator.FromHtml(CBaseNode.GetAttribute("FillColor"));
                spec.IsScaled = Convert.ToBoolean(CBaseNode.GetAttribute("IsScaled"));
                spec.IsFillVisible = Convert.ToBoolean(CBaseNode.GetAttribute("IsFillVisible"));
                ListCurF.Add(sName, spec);
            }
        }

        public void SaveSelTeamToWins(XmlElement Node)
        {
            try
            {
                XmlElement CBaseNode = (XmlElement)(Node.SelectSingleNode("Appearance"));
                CBaseNode.SetAttribute("iSelTeamIndex", iSelTeamIndex.ToString());
            }
            catch (Exception e)
            {

            }
        }

        public void SaveToXML(XmlElement Node, XmlDocument MyXmlDoc)
        {
            try
            {
                XmlElement CBaseNode = (XmlElement)(Node.SelectSingleNode("Appearance"));
                CBaseNode.SetAttribute("LegendFill", ColorTranslator.ToHtml(LegendFill));
                CBaseNode.SetAttribute("GridColor", ColorTranslator.ToHtml(GridColor));
                CBaseNode.SetAttribute("PaneFill", ColorTranslator.ToHtml(PaneFill));
                CBaseNode.SetAttribute("ChartFill", ColorTranslator.ToHtml(ChartFill));
                CBaseNode.SetAttribute("CurveLineWith", CurveLineWith.ToString());
                CBaseNode.SetAttribute("XAxisTitleShow", XAxisTitleShow.ToString());
                CBaseNode.SetAttribute("XAxisFormat", XAxisFormat);
                CBaseNode.SetAttribute("XAxisTitle", XAxisTitle);
                while (CBaseNode.ChildNodes.Count > 0)
                {
                    CBaseNode.RemoveChild(CBaseNode.FirstChild);
                }
                foreach (CCurveFont nCurF in ListCurF.Values)
                {
                    XmlElement xmlCurve = MyXmlDoc.CreateElement("CurveFont");
                    xmlCurve.SetAttribute("Name", nCurF.Name);
                    xmlCurve.SetAttribute("Desc", nCurF.Desc);
                    xmlCurve.SetAttribute("Font", nCurF.sFont);
                    xmlCurve.SetAttribute("FontColor", nCurF.sFontColor);
                    xmlCurve.SetAttribute("IsFillVisible", nCurF.IsFillVisible.ToString());
                    xmlCurve.SetAttribute("FillColor", nCurF.sFillColor);
                    xmlCurve.SetAttribute("IsScaled", nCurF.IsScaled.ToString());
                    CBaseNode.AppendChild(xmlCurve);
                }

                CBaseNode = (XmlElement)(Node.SelectSingleNode("Behavior"));

                while (CBaseNode.ChildNodes.Count > 0)
                {
                    CBaseNode.RemoveChild(CBaseNode.FirstChild);
                }

                foreach (CLSTeamCurve nTeam in ListTream)
                {
                    XmlElement TeamCurve = MyXmlDoc.CreateElement("TeamCurve");
                    nTeam.SaveToXML(TeamCurve, MyXmlDoc);
                    CBaseNode.AppendChild(TeamCurve);
                }
            }
            catch (Exception e)
            {

            }
        }
    }

}
