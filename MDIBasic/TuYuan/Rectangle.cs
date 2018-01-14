using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Drawing.Drawing2D;

namespace LSSCADA
{
    class CRectangle : CTuYuan
    {
        public SizeF RectSize;
        public override void DrawPoints(Graphics g)
        {
            /*if (m_CartoonData.FillChanged)
            //{
            myGraphicsPath.Reset();
            myGraphicsPath.AddRectangle(Rectangle.Round(CartoonRect));
            myGraphicsPath.Transform(myPathMatrix);
            System.Drawing.Color tempColor = CartoonBrush.Color;
            //if (Color != m_CartoonData.m_OrgclrLine)
            //	CartoonBrush.Color = m_CartoonData.m_clrLine;
            g.FillPath(CartoonBrush, myGraphicsPath);
            CartoonBrush.Color = tempColor;
            //}*/
            base.DrawPoints(g);
            myGraphicsPath.AddRectangle(Rectangle.Round(GetPointsRect()));
            //myGraphicsPath.Transform(myPathMatrix);
            if (FillOptions.BrushType != LCBrushType.Blank)//|| !FillOptions.NoFrame)
                g.FillPath(DrawBrush, myGraphicsPath);
            g.DrawPath(DrawPen, myGraphicsPath);
            /*if (FIsFocused || FIsSeleced)
            {
                DrawFocus(g);
            }*/
        }

        public  void DrawRect(Graphics g)
        {
            
            //base.DrawPoints(g);
            PointF[] points = {
                                  new PointF(m_Location.X,m_Location.Y),
                                  new PointF(m_Location.X+RectSize.Width,m_Location.Y),
                                  new PointF(m_Location.X+RectSize.Width,m_Location.Y+RectSize.Height),
                                  new PointF(m_Location.X,m_Location.Y+RectSize.Height),
                              };
            myGraphicsPath = new GraphicsPath();
            myGraphicsPath.AddLines(points);
            //myGraphicsPath.Transform(myPathMatrix);
            Pen pen = new Pen(Color.FromArgb(150, Color.Gray), 1);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            g.DrawPath(pen, myGraphicsPath);

            RectangleF RF = new RectangleF(m_Location, RectSize);
           // LinearGradientBrush p = new LinearGradientBrush(RF, Color.White, Color.Black, 0);
            //p.CenterColor = Color.White;
           
            Color[] color = { Color.Black};
            //p.SurroundColors = color;
           // g.FillPath(p, myGraphicsPath);
            /*if (FIsFocused || FIsSeleced)
            {
                DrawFocus(g);
            }*/
        }
        //virtual override void SetLastPoint(PointF PValue) ;
        //virtual override void AddPoint(PointF PValue) ;
        //virtual override void SaveToXML(XmlElement Node) ;
        public override void LoadFromXML(XmlElement Node)
        {
            base.LoadFromXML(Node);
            XmlElement CRectangleNode = (XmlElement)(Node.SelectSingleNode("CRectangle"));
        }
        //public:
        public CRectangle()
            : base()
        {
            m_ElementType = LCElementType.RECTANGLE;
        }
        public CRectangle(String _Name, CBase _Parent, Object _Owner)
            : base(_Name, _Parent, _Owner)
        {
            m_ElementType = LCElementType.RECTANGLE;
        }
    }
}
