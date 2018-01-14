using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace LSSCADA
{
    //管
    class CPipe : CTuYuan
    {
        Single Pipefocus = 0;
        string PipeP1 = "";//第一个点的角度列表 \
        string PipeP2 = "";//第二个点的角度列表
        int PipeWidth = 1;//管道宽
        public bool IsBorder = false;                  //是否显示边框
        int iDir = 1;//方向 0:y1=y2竖   1:x1=x2横
        PointF[] PTS;
        RectangleF rect;

        string ShowText;
        float TextX;
        float TextY;
        public SolidBrush FontBrush;//文本画刷
        public StringFormat TextFormat;			//文本格式
        public Font DrawFont;	//字体
        RectangleF TextRect;

        public float ValX1, ValY1, ValX2, ValY2;
        public Point ValPF;
        RectangleF ValRect;
        public StringFormat ValFormat;			//文本格式
        public Font ValFont;	//字体

        bool bShowText = false;     //显示文本      0
        bool bShowPie = true;       //显示流量计    1
        bool bShowRea = false;      //显示读取值    2
        public bool bShowSet = false;      //显示设定值    3
        public float SetWidth = 48;//设定框的长度
        public int BShowVal
        {
            set
            {
                bShowText = Convert.ToBoolean(value & 1);   //显示文本
                bShowPie = Convert.ToBoolean(value & 2);    //显示流量计
                bShowRea = Convert.ToBoolean(value & 4);    //显示读取值
                bShowSet = Convert.ToBoolean(value & 8);    //显示设定值
            }
        }

        CVar LiuVar = new CVar();

        public CPipe() : base() 
        {
            m_ElementType = LCElementType.Pipe;

            FontBrush = new SolidBrush(System.Drawing.Color.White);
            TextFormat = new StringFormat(StringFormat.GenericDefault);
            ValFormat = new StringFormat(StringFormat.GenericDefault);
            ValFormat.Alignment = StringAlignment.Far;
            ValFormat.LineAlignment = StringAlignment.Center;
            DrawFont = new Font("黑体", 12, GraphicsUnit.World);
            ValFont = new Font("宋体", 14, GraphicsUnit.World);
            m_RotateAngle = 0;
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

                iOrgX1 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgX1"));
                iOrgX2 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgX2"));
                iOrgY1 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgY1"));
                iOrgY2 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgY2"));
                
                Pipefocus = Convert.ToSingle(CBaseNode.GetAttribute("Pipefocus"));
                PipeP1 = CBaseNode.GetAttribute("PipeP1");
                PipeP2 = CBaseNode.GetAttribute("PipeP2");
                PipeWidth = Convert.ToInt32(CBaseNode.GetAttribute("PipeWidth"));

                if (CBaseNode.HasAttribute("Angle"))
                {
                    m_RotateAngle = Convert.ToSingle(CBaseNode.GetAttribute("Angle"));
                }

                if (CBaseNode.HasAttribute("tpenColor"))
                {
                    IsBorder = Convert.ToBoolean(CBaseNode.GetAttribute("IsBorder"));

                    DrawPen.Color = ColorTranslator.FromWin32(Convert.ToInt32( CBaseNode.GetAttribute("tpenColor")));
                    DrawPen.Width =Convert.ToInt32( CBaseNode.GetAttribute("tpenWidth"));
                    DrawPen.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle),  CBaseNode.GetAttribute("tpenStyle"));
                }

                if (CBaseNode.HasAttribute("tbruStyle"))
                {
                    tbruStyle = Convert.ToInt32(CBaseNode.GetAttribute("tbruStyle"));
                    tbruColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("tbruColor")));
                    FillColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("FillColor")));
                    //EmgColor = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("EmgColor")));
                    iShadeFillStyle = Convert.ToInt32(CBaseNode.GetAttribute("iShadeFillStyle"));
                }

                if (CBaseNode.HasAttribute("BShowText"))
                {
                    BShowVal = Convert.ToInt32(CBaseNode.GetAttribute("BShowText"));
                    ShowText = CBaseNode.GetAttribute("ShowText");
                    TextX = Convert.ToSingle(CBaseNode.GetAttribute("TextX"));
                    TextY = Convert.ToSingle(CBaseNode.GetAttribute("TextY"));
                    FontBrush.Color = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("TextColor")));
                    float fSize = Convert.ToSingle(CBaseNode.GetAttribute("TextSize"));
                    DrawFont = new Font("黑体", fSize, GraphicsUnit.World);

                    ValX1 = Convert.ToSingle(CBaseNode.GetAttribute("ValX1"));
                    ValY1 = Convert.ToSingle(CBaseNode.GetAttribute("ValY1"));
                    ValX2 = Convert.ToSingle(CBaseNode.GetAttribute("ValX2"));
                    ValY2 = Convert.ToSingle(CBaseNode.GetAttribute("ValY2"));
                    if (CBaseNode.HasAttribute("SetWidth"))
                        SetWidth = Convert.ToSingle(CBaseNode.GetAttribute("SetWidth"));
                    else
                        SetWidth = 48;

                    LiuVar = frmMain.staComm.GetVarByStaNameVarName("NJ301", Name);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            if (iOrgY1 == iOrgY2)
                iDir = 1;
            else
                iDir = 0;
            List<PointF> LP = new List<PointF>();
            List<PointF> LP1 = GetPipeP(PipeP1, 1, iDir);
            List<PointF> LP2 = GetPipeP(PipeP2, 2, iDir);
            for (int i = 0; i < LP1.Count; i++)
            {
                PointF p1 = new PointF(LP1[i].X * PipeWidth + iOrgX1, LP1[i].Y * PipeWidth + iOrgY1);
                LP.Add(p1);
            }

            for (int i = 0; i < LP2.Count; i++)
            {
                PointF p1 = new PointF(LP2[i].X * PipeWidth + iOrgX2, LP2[i].Y * PipeWidth + iOrgY2);
                LP.Add(p1);
            }
            
            if (LP.Count > 0)
            {
                PTS = new PointF[LP.Count+1];
                PointF MinP = LP[0];
                PointF MaxP = LP[0];

                for (int i = 0; i < LP.Count; i++)
                {
                    PTS[i] = LP[i];
                    MinP.X = Math.Min(LP[i].X, MinP.X);
                    MinP.Y = Math.Min(LP[i].Y, MinP.Y);
                    MaxP.X = Math.Max(LP[i].X, MaxP.X);
                    MaxP.Y = Math.Max(LP[i].Y, MaxP.Y);
                }
                PTS[LP.Count] = PTS[0];
                rect.Location = MinP;
                rect.Size = new SizeF(MaxP.X - MinP.X, MaxP.Y - MinP.Y);

                TextRect.Location = new PointF(MinP.X + TextX, MinP.Y + TextY);
                TextRect.Size = new SizeF(90, 30);

                ValPF = new Point((int)(MinP.X + ValX1), (int)(MinP.Y + ValY1));
                ValRect.Location = new PointF(MinP.X + ValX2, MinP.Y + ValY2);
                ValRect.Size = new SizeF(SetWidth, 20);
            }
        }

        private List<PointF> GetPipeP(string PipeP, int index, int iStyle)
        {
            if (iStyle == 2)
            {
                int iii = 0;
            }
            int Style = iStyle % 2;
            int[] iPipe = new int[] { 180,0,360};
            if (index == 1 && Style == 1)
            {
                switch (PipeP)
                {
                    //case "―": iPipe = new int[] { 180, 0, 360 }; break;
                    case "｜": iPipe = new int[] { 270, 0, 90 }; break;
                    case "／": iPipe = new int[] { 225, 0, 45 }; break;
                    case "＼": iPipe = new int[] { 315, 0,  135}; break;
                    //case "∧": iPipe = new int[] { 225, 0, 315 }; break;
                    //case "∨": iPipe = new int[] { 135, 0, 45 }; break;
                    case "＜": iPipe = new int[] { 315, 0, 45 }; break;
                    case "＞": iPipe = new int[] { 225, 0, 135 }; break;
                    default: iPipe = new int[] { 270, 0, 90 }; break;
                }
            }
            else if (index == 2 && Style == 1)
            {
                switch (PipeP)
                {
                    //case "―": iPipe = new int[] { 360, 0, 180 }; break;
                    case "｜": iPipe = new int[] { 90, 0, 270 }; break;
                    case "／": iPipe = new int[] { 45, 0, 225 }; break;
                    case "＼": iPipe = new int[] { 135, 0, 315 }; break;
                    //case "∧": iPipe = new int[] { 314, 0, 225 }; break;
                    //case "∨": iPipe = new int[] { 45, 0, 135 }; break;
                    case "＜": iPipe = new int[] { 45, 0, 135 }; break;
                    case "＞": iPipe = new int[] { 135, 0, 225 }; break;
                    default: iPipe = new int[] { 90, 0, 270 }; break;
                }
            }
            else if (index == 1 && Style == 0)
            {
                switch (PipeP)
                {
                    case "―": iPipe = new int[] { 180, 0, 360 }; break;
                    //case "｜": iPipe = new int[] { 270, 0, 90 }; break;
                    case "／": iPipe = new int[] { 225, 0, 45 }; break;
                    case "＼": iPipe = new int[] { 135, 0, 315 }; break;
                    case "∧": iPipe = new int[] { 225, 0, 315 }; break;
                    case "∨": iPipe = new int[] { 135, 0, 45 }; break;
                    //case "＜": iPipe = new int[] { 315, 0, 45 }; break;
                    //case "＞": iPipe = new int[] { 225, 0, 135 }; break;
                    default: iPipe = new int[] { 180, 0, 360 }; break;
                }
            }
            else if (index == 2 && Style == 0)
            {
                switch (PipeP)
                {
                    case "―": iPipe = new int[] { 360, 0, 180 }; break;
                    //case "｜": iPipe = new int[] { 90, 0, 270 }; break;
                    case "／": iPipe = new int[] { 45, 0, 225 }; break;
                    case "＼": iPipe = new int[] { 315, 0, 135 }; break;
                    case "∧": iPipe = new int[] { 315, 0, 225 }; break;
                    case "∨": iPipe = new int[] { 45, 0, 135 }; break;
                    //case "＜": iPipe = new int[] { 45, 0, 135 }; break;
                    //case "＞": iPipe = new int[] { 135, 0, 225 }; break;
                    default: iPipe = new int[] { 360, 0, 180 }; break;
                }
            }
            List<PointF> LP1 = new List<PointF>();
            for (int i = 0; i < iPipe.Length; i++)
            {
                Single[] iXY = new Single[] { 0, 0 };
                switch (Convert.ToInt32(iPipe[i]))
                {
                    case 0: iXY = new Single[] { 0, 0 }; break;
                    case 45: iXY = new Single[] { 1, -1 }; break;
                    case 90: iXY = new Single[] { 0, -1 }; break;
                    case 135: iXY = new Single[] { -1, -1 }; break;
                    case 180: iXY = new Single[] { -1, 0 }; break;
                    case 225: iXY = new Single[] { -1, 1 }; break;
                    case 270: iXY = new Single[] { 0, 1 }; break;
                    case 315: iXY = new Single[] { 1, 1 }; break;
                    case 360: iXY = new Single[] { 1, 0 }; break;
                }
                PointF p1 = new PointF(iXY[0], iXY[1] );
                LP1.Add(p1);
            }

            return LP1;
        }

        public override void DrawPoints(Graphics g)
        {
            try
            {
                base.DrawPoints(g);
                if (m_RotateAngle > 0)
                {
                    g.TranslateTransform(iOrgX1, iOrgY1);
                    g.RotateTransform(m_RotateAngle);
                    g.TranslateTransform(-iOrgX1,- iOrgY1);
                }
                GraphicsPath path = new GraphicsPath();
                path.AddLines(PTS);
                LinearGradientBrush brush = new LinearGradientBrush(rect, tbruColor, FillColor, (LinearGradientMode)iDir);
                if (Pipefocus < 1)
                    brush.SetBlendTriangularShape(Pipefocus);

                Matrix myMatrix = new Matrix();
                path.Transform(myMatrix);
                if(bShowPie)
                    g.FillPath(brush, path);
                if (IsBorder)
                {
                    g.DrawPath(DrawPen, path);
                }

                if (bShowText)
                    g.DrawString(ShowText, DrawFont, FontBrush, TextRect, TextFormat);
                if (bShowRea)
                {
                    g.FillRectangle(Brushes.DarkGray, ValRect);
                    g.DrawString(LiuVar.GetStringValue(1,5), ValFont, new SolidBrush(Color.Black), ValRect, ValFormat);
                }
                if (m_RotateAngle > 0)
                    g.ResetTransform();
                //g.ResetTransform();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void Rotate(Matrix MatrixValue)
        {
            if (m_RotateAngle < 1)
                return;
            PointF centerPoint = GetCenterPointF();

            MatrixValue.Translate(-centerPoint.X, -centerPoint.Y, MatrixOrder.Append);
            MatrixValue.Rotate(m_RotateAngle, MatrixOrder.Append);
            MatrixValue.Translate(centerPoint.X, centerPoint.Y, MatrixOrder.Append);
        }

        public override void Draw(Graphics g, Single iS)
        {
            base.Draw(g,iS);
            GraphicsPath path = new GraphicsPath();
            PointF[] PS = new PointF[PTS.Length];
            for (int i = 0; i < PTS.Length; i++)
            {
                PS[i].X = PTS[i].X * iS;
                PS[i].Y = PTS[i].Y * iS;
            }
            path.AddLines(PS);
            RectangleF rect1 = new RectangleF(rect.X * iS, rect.Y * iS, rect.Width * iS, rect.Height * iS);
            LinearGradientBrush brush = new LinearGradientBrush(rect1, tbruColor, FillColor, (LinearGradientMode)iDir);
            if (Pipefocus < 1)
                brush.SetBlendTriangularShape(Pipefocus);
            g.FillPath(brush, path);
            if (IsBorder)
            {
                g.DrawPath(DrawPen, path);
            }
        }

        public override bool Selected(PointF SelectPoint)
        {
            return rect.Contains(SelectPoint);
        }
    }
}
