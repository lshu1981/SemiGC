using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Drawing.Drawing2D;

namespace LSSCADA
{
    class CLine : CTuYuan
    {
        //public:
	    [Browsable(false)]
        public override CFillOptions FillOptions
	    {
		    get
		    {
			    return m_FillOptions;
		    }		
	    }

	    [Browsable(false), ReadOnly(true), Description("大小"), Category("Layout"), DisplayName("大小")]
        public override SizeF Size
	    {
		    set
		    {}				

		    get
		    {
			    return base.Size;
		    }		
	    }

        /*public override RectangleF GetRect()
        {
            base.GetRect();
            RectangleF rect = new RectangleF();
	
	        if (Points.Count >= 2)
	        {
		        PointF MinP = (PointF)Points[0];
		        PointF MaxP =(PointF)Points[1];
		        //if (FDrawing)
			    //    MaxP = PreviewPoint;
		        //else
			        MaxP = (PointF)Points[1];
		        PointF[] pts = new PointF[2];
		        pts[0] = MinP;
		        pts[1] = MaxP;
		        //Matrix  matrxValue = new Matrix();
		        //PointF CenterP = GetCenterPointF();
		        //matrxValue.Translate(-CenterP.X,-CenterP.Y,MatrixOrder.Append);
		        //matrxValue.Rotate(m_RotateAngle,MatrixOrder.Append);
		        //matrxValue.Translate(CenterP.X,CenterP.Y,MatrixOrder.Append);
		        //matrxValue.TransformPoints(pts);
		        MinP = pts[0];
		        MaxP = pts[0];
                foreach (PointF P in Points)
		        {
			        MinP.X = Math.Min(P.X,MinP.X);
			        MinP.Y = Math.Min(P.Y,MinP.Y);
			        MaxP.X = Math.Max(P.X,MaxP.X);
			        MaxP.Y = Math.Max(P.Y,MaxP.Y);

		        }
		        if ( FDrawing)
		        {
			        MinP.X = Math.Min(PreviewPoint.X,MinP.X);
			        MinP.Y = Math.Min(PreviewPoint.Y,MinP.Y);
			        MaxP.X = Math.Max(PreviewPoint.X,MaxP.X);
			        MaxP.Y = Math.Max(PreviewPoint.Y,MaxP.Y);
		        }
		        rect.Location = MinP;
		        rect.Size =new SizeF(MaxP.X - MinP.X ,MaxP.Y - MinP.Y );
		        //rect = RotateTransformRectangleF(rect,GetCenterPointF(),m_RotateAngle);
	        }

	        return rect;
        }*/
        //public virtual bool Selected(Region RegionValue) { }
        //public virtual bool Selected(PointF SelectPoint) { }
        //public virtual void OnMoveFocus(CFocus Focus, System.Drawing.SizeF Offset) { }
        public override void DrawPoints(Graphics g) 
        {
            base.DrawPoints(g);
                if (this.Parent != null)
                {
                    if (this.Parent.KJIconType == 1)
                        Name = this.Parent.IconName;
                    if (this.Parent.KJIconType == 2)
                    {
                        PointF fRPF0 = new PointF(((PointF)Points[0]).X * this.Parent.fRatio, ((PointF)Points[0]).Y * this.Parent.fRatio);
                        PointF fRPF1 = new PointF(((PointF)Points[1]).X * this.Parent.fRatio, ((PointF)Points[1]).Y * this.Parent.fRatio);
                        g.DrawLine(DrawPen, PointF.Add(fRPF0, this.Parent.m_RotatePosition), PointF.Add(fRPF1, this.Parent.m_RotatePosition));
                    }
                    else
                        g.DrawLine(DrawPen, PointF.Add((PointF)Points[0], this.Parent.m_RotatePosition), PointF.Add((PointF)Points[1], this.Parent.m_RotatePosition));
                }
                else
                    g.DrawLine(DrawPen, (PointF)Points[0], (PointF)Points[1]);
	        //if (FIsFocused || FIsSeleced)
		    //    DrawFocus(g);
        }
        //public virtual void SetLastPoint(PointF PValue) { }
        //public virtual void AddPoint(PointF PValue) { }
        //public virtual CBase Clone() { }
        //public virtual CBase CopyTo(CBase DesObject) { }
        //public virtual void SaveToXML(XmlElement Node) { }
        public override void LoadFromXML(XmlElement Node) 
        {
            base.LoadFromXML(Node);
            XmlElement LineNode = Node;
        }
	    public CLine():base()
	    {
		    m_ElementType = LCElementType.LINE;
	    }
        public CLine(String _Name, CBase _Parent, Object _Owner)
            : base(_Name, _Parent, _Owner)
	    {
		    m_ElementType = LCElementType.LINE;
	    }
	    ~CLine()
	    {}
    }
}
