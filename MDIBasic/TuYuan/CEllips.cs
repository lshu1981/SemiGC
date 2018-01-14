using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace LSSCADA
{
    class CEllips : CTuYuan
    {
        //public virtual CBaseClone()override;
        //public virtual void SetLastPoint(PointF PValue) override;
        //public virtual void AddPoint(PointF PValue) override;
        public override void DrawPoints(Graphics g)
        {
            base.DrawPoints(g);

            /*float fLeft, fTop, fRight, fBottom;	

            if ( GetDrawing() )
            {	
                fLeft = Math.Min(((PointF)Points[0]).X, PreviewPoint.X);
                fTop = Math.Min(((PointF)Points[0]).Y, PreviewPoint.Y);
                fRight = Math.Max(((PointF)Points[0]).X, PreviewPoint.X);
                fBottom = Math.Max(((PointF)Points[0]).Y, PreviewPoint.Y);
            } else {
                fLeft = Math.Min(((PointF)Points[0]).X, ((PointF)Points[1]).X);
                fTop = Math.Min(((PointF)Points[0]).Y, ((PointF)Points[1]).Y);
                fRight = Math.Max(((PointF)Points[0]).X, ((PointF)Points[1]).X);
                fBottom = Math.Max(((PointF)Points[0]).Y, ((PointF)Points[1]).Y);
            }
	
            RectangleF rcClient = RectangleF.FromLTRB(fLeft, fTop, fRight, fBottom);
            */
            myGraphicsPath.AddEllipse(GetPointsRect());
            myGraphicsPath.Transform(myPathMatrix);
            
            RectangleF PathBounds = myGraphicsPath.GetBounds();
            if (PathBounds.Height < 0.1 || PathBounds.Width < 0.1)
                return;

            g.FillPath(DrawBrush, myGraphicsPath);

            if (FillOptions.BrushType == LCBrushType.Blank || !FillOptions.NoFrame)
            {
                g.DrawPath(DrawPen, myGraphicsPath);
            }
            if (FIsFocused || FIsSeleced)
                DrawFocus(g);
        }
        //public virtual bool Selected(PointF SelectPoint) override;
        //public virtual void SaveToXML(XmlElement Node) override;
        public override void LoadFromXML(XmlElement Node)
        {
            base.LoadFromXML(Node);
            XmlElement CEllipsNode = (XmlElement)(Node.SelectSingleNode("CEllips"));
        }
        //public virtual void UpDate()override;

        public CEllips()
        {
            m_ElementType = LCElementType.ELLIPS;
        }
        public CEllips(String _Name, CBase _Parent, Object _Owner)
        {
            m_ElementType = LCElementType.ELLIPS;
        }
    }
}
