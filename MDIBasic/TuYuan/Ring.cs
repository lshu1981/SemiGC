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
    //圆环
    class CRing: CTuYuan
    {
        PointF centerPoint;
        RectangleF rect;

        int iLineWidth;
        int RingR;
        Color FillColor0;
        Color FillColor1;
        Color LineColor;
        public bool bValue;

        //文本显示
        bool bShowText = false;
        public string ShowText;
        float TextX;
        float TextY;
        public SolidBrush FontBrush;//文本画刷
        public StringFormat TextFormat;			//文本格式
        public Font DrawFont;	//字体
        RectangleF TextRect;

        public CRing()
            : base() 
        {
            m_ElementType = LCElementType.Ring;
            iLineWidth = 4;
            RingR = 12;
            bValue = false;
            FillColor0 = Color.Lime;
            FillColor1 = Color.Red;
            LineColor = Color.Gray;
            FontBrush = new SolidBrush(System.Drawing.Color.White);
            TextFormat = new StringFormat(StringFormat.GenericDefault);
            DrawFont = new Font("黑体", 11, GraphicsUnit.World);
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
                StaName = CBaseNode.GetAttribute("StaName");
                VarName = CBaseNode.GetAttribute("VarName");

                iOrgX1 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgX1"));
                iOrgX2 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgX2"));
                iOrgY1 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgY1"));
                iOrgY2 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgY2"));

                if (CBaseNode.HasAttribute("BShowText"))
                {
                    bShowText = Convert.ToInt32(CBaseNode.GetAttribute("BShowText"))>0;
                    ShowText = CBaseNode.GetAttribute("ShowText");
                    TextX = Convert.ToSingle(CBaseNode.GetAttribute("TextX"));
                    TextY = Convert.ToSingle(CBaseNode.GetAttribute("TextY"));
                    FontBrush.Color = ColorTranslator.FromWin32(Convert.ToInt32(CBaseNode.GetAttribute("TextColor")));
                    float fSize = Convert.ToSingle(CBaseNode.GetAttribute("TextSize"));
                    DrawFont = new Font("黑体", fSize, GraphicsUnit.World);
                }
                iLineWidth = Convert.ToInt32(CBaseNode.GetAttribute("LineWidth"));
                RingR = Convert.ToInt32(CBaseNode.GetAttribute("RingR"));

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            InitRing();
        }
        public void InitRing()
        {
            centerPoint = new PointF(iOrgX1, iOrgY1);
            rect.Location = new PointF(iOrgX1 - RingR, iOrgY1 - RingR);
            rect.Size = new SizeF(RingR * 2, RingR * 2);
            TextRect.Location = new PointF(iOrgX1 - RingR + TextX, iOrgY1 - RingR + TextY);
            TextRect.Size = new SizeF(80,30);
        }
        public override void DrawPoints(Graphics g)
        {
            //base.DrawPoints(g);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(rect);
            SolidBrush brush = new SolidBrush(FillColor1);

            if (bShowText)
            {
                string sVar = "AV" + ShowText.PadLeft(3, '0');
                CVar nVar = frmMain.staComm.GetVarByStaNameVarName("NJ301", sVar);

                if (nVar != null)
                    bValue = nVar.GetBoolValue();
            }
            if (bValue)
             brush.Color = FillColor0;
            g.FillPath(brush, path);
            g.DrawEllipse(new Pen(LineColor, iLineWidth), rect);
            if(bShowText)
                g.DrawString(ShowText, DrawFont, FontBrush, TextRect, TextFormat);
        }

        public override void Draw(Graphics g, Single iS)
        {
            base.Draw(g,iS);

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(rect);
            SolidBrush brush = new SolidBrush(FillColor0);

            if (bValue)
                brush.Color = FillColor1;
            g.FillPath(brush, path);
            g.DrawEllipse(new Pen(LineColor, iLineWidth), rect);
        }

        public override bool Selected(PointF SelectPoint)
        {
            //if (!FisLive)
            //    return false;
            //SelectPoint = TransformPointF(SelectPoint);
            //if (base.Selected(SelectPoint))
            //    return true;

            return rect.Contains(SelectPoint);
        }
    }
}
