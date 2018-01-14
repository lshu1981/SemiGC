using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Xml;
using System.Drawing.Drawing2D;

namespace LSSCADA
{
    //标尺
    class KJRuler : CTuYuan
    {
        string label = "";          //标题
        string unit = "";          //单位
        string Sta = "";          //单位
        string Var = "";          //单位
        public double sinValue = 0;        //值
        int Total = 0;              //总刻度

        public bool IsBorder = false;                  //是否显示边框

        public SolidBrush FontBrush;//文本画刷

        public StringFormat FormatCenter;			//文本格式
        public StringFormat FormatLeft;			//文本格式
        public StringFormat FormatRight;			//文本格式
        public Font DrawFont;	//字体

        public Font ValFont;	//字体
        public bool BShowVal = false;

        CVar LiuVar = new CVar();

        public KJRuler()
            : base() 
        {
            m_ElementType = LCElementType.Pipe;

            FontBrush = new SolidBrush(System.Drawing.Color.White);
            FormatCenter = new StringFormat(StringFormatFlags.NoClip);
            FormatCenter.Alignment = StringAlignment.Center;
            FormatCenter.LineAlignment = StringAlignment.Center;

            FormatLeft = new StringFormat(StringFormatFlags.NoClip);
            FormatLeft.Alignment = StringAlignment.Near;
            FormatLeft.LineAlignment = StringAlignment.Center;
            FormatRight = new StringFormat(StringFormatFlags.NoClip);
            FormatRight.Alignment = StringAlignment.Far;
            FormatRight.LineAlignment = StringAlignment.Center;

            DrawFont = new Font("宋体", 12, GraphicsUnit.World);
            ValFont = new Font("宋体", 14, GraphicsUnit.World);
        }

        public override void LoadFromXML(XmlElement CBaseNode)
        {
            try
            {
                //base.LoadFromXML(CBaseNode);
                // XmlElement TextNode = (XmlElement)(Node.SelectSingleNode("Layout"));
                iElementOrder = Convert.ToInt32(CBaseNode.GetAttribute("iElementOrder"));
                GLayerName = CBaseNode.GetAttribute("GLayerName");
                Name = CBaseNode.GetAttribute("Name");
                label = CBaseNode.GetAttribute("label");
                unit = CBaseNode.GetAttribute("unit");

                iOrgX1 = Convert.ToSingle(CBaseNode.GetAttribute("Left"));
                iOrgY1 = Convert.ToSingle(CBaseNode.GetAttribute("Top"));
                iOrgX2 = Convert.ToSingle(CBaseNode.GetAttribute("Width"));
                iOrgY2 = Convert.ToSingle(CBaseNode.GetAttribute("Height"));

                float fSize = Convert.ToSingle(CBaseNode.GetAttribute("FontSize"));
                DrawFont = new Font("宋体", fSize, GraphicsUnit.World);
                Total = Convert.ToInt32(CBaseNode.GetAttribute("Total"));
                KeDuMax = Convert.ToSingle(CBaseNode.GetAttribute("KeDuMax"));
                KeDuMin = Convert.ToSingle(CBaseNode.GetAttribute("KeDuMin"));

                KeDuLineSize = Convert.ToInt32(CBaseNode.GetAttribute("KeDuLineSize"));

                TopRemain = Convert.ToSingle(CBaseNode.GetAttribute("TopRemain"));
                BottemRemain = Convert.ToSingle(CBaseNode.GetAttribute("BottemRemain"));
                fLeftPer = Convert.ToSingle(CBaseNode.GetAttribute("fLeftPer"));
                if (fLeftPer < 0)fLeftPer = 0;
                if (fLeftPer > 0.6) fLeftPer = 0.6f;

                fMidPer = Convert.ToSingle(CBaseNode.GetAttribute("fMidPer"));
                if (fMidPer < 0.2) fLeftPer = 0.2f;
                if ((fMidPer + fLeftPer) > 1) fMidPer = 1f - fLeftPer;

                SmallTotal = Convert.ToInt32(CBaseNode.GetAttribute("SmallTotal"));
                fUpLimitAlarmV = Convert.ToSingle(CBaseNode.GetAttribute("fUpLimitAlarmV"));
                fDownLimitAlarmV = Convert.ToSingle(CBaseNode.GetAttribute("fDownLimitAlarmV"));
                fValue = Convert.ToSingle(CBaseNode.GetAttribute("fValue"));

                RulerBackColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("RulerBackColor")));
                FrameColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("FrameColor")));
                FingerColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("FingerColor")));
                KeduFontColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("KeduFontColor")));
                FillColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("FillColor")));
                AlarmColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("AlarmColor")));

                DrawStyle = Convert.ToInt32(CBaseNode.GetAttribute("DrawStyle"));

                Sta = CBaseNode.GetAttribute("Sta");
                Var = CBaseNode.GetAttribute("Var");

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public override void DrawPoints(Graphics g)
        {
            try
            {
               // base.DrawPoints(g);
                g.TranslateTransform(iOrgX1, iOrgY1);

                GraphicsPath myPath = new GraphicsPath();

                myPath.AddRectangle(new RectangleF(0, 0, iOrgX2, iOrgY2));
                g.FillPath(new SolidBrush(FrameColor), myPath);
                // g.DrawPath(new Pen(Color.Black, 1f), myPath);
                g.DrawPath(new Pen(KeduFontColor, 1f), myPath);

                Single iW1 = iOrgX2 * fLeftPer;
                Single iW2 = iOrgX2 * fMidPer+iW1;
                Single iH = Math.Abs(iOrgY2 - TopRemain - BottemRemain);
                myPath = new GraphicsPath();
                myPath.AddRectangle(new RectangleF(iW1, TopRemain, iW2 - iW1,iH));
                g.FillPath(new SolidBrush(RulerBackColor), myPath);

                Single iHD = iH / Total;
                for (int i = 0; i <= Total;i++ )
                {
                    Single iTop = TopRemain + (float)i * iHD;
                    g.DrawLine(new Pen(KeduFontColor, 1f), new PointF(iW1, iTop), new PointF(iW2, iTop));
                    for (int j = 1; j < SmallTotal; j++)
                    {
                        if (i == Total)
                            break;
                        Single iSmallTop = iTop + iHD / (Single)SmallTotal * j;
                        g.DrawLine(new Pen(KeduFontColor, 1f), new PointF(iW1 + (iW2 - iW1) / 4, iSmallTop), new PointF(iW2 - (iW2 - iW1) / 4, iSmallTop));
                    }
                    Single iValue = KeDuMax + i * (KeDuMin - KeDuMax) / Total;
                    g.DrawString(iValue.ToString(), DrawFont, new SolidBrush(KeduFontColor), new RectangleF(0, iTop - 10, iOrgX2, 20), FormatLeft);
                }
                
                g.DrawString(unit, DrawFont, new SolidBrush(KeduFontColor), new RectangleF(0, TopRemain - 10, iOrgX2, 20), FormatRight);

                if (label != "")
                {
                    g.DrawString(label, DrawFont, new SolidBrush(KeduFontColor), new RectangleF(0, 0, iOrgX2, TopRemain), FormatCenter);
                }
                g.DrawString(sinValue.ToString("0.000"), DrawFont, new SolidBrush(KeduFontColor), new RectangleF(0, Math.Abs(iOrgY2 - BottemRemain), iOrgX2, BottemRemain), FormatCenter);

                myPath = new GraphicsPath();
                float sinT = (KeDuMax - (float)sinValue) / (KeDuMax - KeDuMin) * iH+TopRemain;
                PointF[] PFS = new PointF[] { new PointF(iW2-(iW2 - iW1) / 4, sinT), new PointF(iW2 + 10, sinT - 5), new PointF(iW2 + 10, sinT + 5) };
                myPath.AddLines(PFS);
                g.FillPath(new SolidBrush(FingerColor), myPath);
               
          //      g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, points);
                g.ResetTransform();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public float KeDuMax { get; set; }  //刻度最大值
        public float KeDuMin { get; set; }  //刻度最小值
        public int KeDuLineSize { get; set; }//刻度线大小

        public float TopRemain { get; set; }  //上边距
        public float BottemRemain { get; set; }  //下边距
        public float fLeftPer { get; set; }  //左宽度等分比
        public float fMidPer { get; set; }  //中间宽度等分比
        public int SmallTotal { get; set; }  //辅助刻度格数
        public float fUpLimitAlarmV { get; set; }  //高限报警
        public float fDownLimitAlarmV { get; set; }  //低限报警
        public float fValue { get; set; }  //当前值


        public System.Drawing.Color RulerBackColor { get; set; }
        public System.Drawing.Color FrameColor { get; set; }
        public System.Drawing.Color FingerColor { get; set; }
        public System.Drawing.Color KeduFontColor { get; set; }
        public System.Drawing.Color AlarmColor { get; set; }

        public int DrawStyle { get; set; }
    }
}
