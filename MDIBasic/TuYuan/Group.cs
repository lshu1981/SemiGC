using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Xml;
using System.Diagnostics;

namespace LSSCADA
{
    class SwitchPic
    {
        public string PicName = "";
        public string Condition = "";
        public KJIcon BaseKJIcon = null;
        public CExpression FExpression=new CExpression();            //条件表达式
        /// <summary>
        /// 获取参数
        /// </summary>
        public SwitchPic(string _PicName, string _Condition)
        {
            PicName = _PicName;
            Condition = _Condition;
            FExpression.ExpressType = LCExpressType.Expression;
            FExpression.Exipression = _Condition;
            FExpression.GetDeviceVar();
        }
    }

    class CGroup : CTuYuan
    {
        public RectangleF FRect;
        public float FWidth;
        public float FHight;
        public ArrayList ListSwitchPic = new ArrayList();
        public KJIcon BaseKJIcon = null ;
        public override SizeF Size
        {
            set
            {
                PointF StartCenterPoint = GetCenterPointF();
                FWidth = value.Width;
                FHight = value.Height;
                PointF EndCenterPoint = GetCenterPointF();
                PointF PingYinSiLiang1 = new PointF(0, 0), PingYinSiLiang2 = new PointF(0, 0);
                PingYinSiLiang1.X = EndCenterPoint.X - StartCenterPoint.X;
                PingYinSiLiang1.Y = EndCenterPoint.Y - StartCenterPoint.Y;
                //PointF PingYinSiLiang2 = RotateTransformPointF(PingYinSiLiang1,new  PointF(0,0),m_RotateAngle);
                PointF PingYinSiLiang = new PointF(0, 0);
                PingYinSiLiang.X = PingYinSiLiang2.X - PingYinSiLiang1.X;
                PingYinSiLiang.Y = PingYinSiLiang2.Y - PingYinSiLiang1.Y;
                FRect.Offset(PingYinSiLiang);
                foreach (CFocus Focus in FocusList)
                {
                    Focus.SelectRectWidth = Math.Abs(CFocus.FocusRectWidth / (FWidth / FRect.Width));
                    Focus.SelectRectHeight = Math.Abs(CFocus.FocusRectWidth / (FHight / FRect.Height));
                }
                foreach (CBase Ob in Children)
                {
                    Ob.SelectedDisWMargin = Math.Abs(SelectedDisWMargin / (FWidth / FRect.Width));
                    Ob.SelectedDisHMargin = Math.Abs(SelectedDisHMargin / (FHight / FRect.Height));
                }
                //if (OnChange != nullptr)
                //	OnChange(this,false);
            }
        }

        public override void DrawPoints(Graphics g)
        {
            base.DrawPoints(g);
            if (KJIconType == 2)//图标
            {
                if (ListSwitchPic.Count > 0) //有切换
                {
                    foreach (SwitchPic nSP in ListSwitchPic)
                    {
                        string sResult = nSP.FExpression.execStr();
                        //Debug.WriteLine(this.Name+ sResult);
                        if (string.Compare(sResult, "true", true) == 0)
                        {
                            foreach (CBase obj in nSP.BaseKJIcon.ListTuYuan)
                            {
                                obj.Parent = this;
                                obj.Draw(g);
                                obj.Parent = null;
                                
                            }
                            return;
                        }
                    }
                }
                if (BaseKJIcon == null)
                    return;
                foreach (CBase obj in BaseKJIcon.ListTuYuan)
                {
                    obj.Parent = this;
                    obj.Draw(g);
                    obj.Parent = null;
                }
            }
        }
        //virtual bool Selected(Region RegionValue) override;
        //virtual bool Selected(PointF SelectPoint) override;
        public override PointF GetCenterPointF()
        {
            return new PointF(FRect.Left + FWidth / 2, FRect.Top + FHight / 2);
        }
        public override RectangleF GetPointsRect()
        {
            RectangleF Rectf = new RectangleF(); ;
            Rectf.Location = FRect.Location;
            Rectf.Size = new SizeF(FWidth, FHight);
            return Rectf;
        }
        public override RectangleF GetRect()
        {
            RectangleF Rectf = new RectangleF();
            Rectf.Location = FRect.Location;
            Rectf.Size = new SizeF(FWidth, FHight);
            //Rectf = RotateTransformRectangleF(Rectf,GetCenterPointF(),m_RotateAngle);	
            return Rectf;
        }
        //virtual void Move(PointF PValue)override;
        //virtual void MoveTo(PointF PValue) override;
        public override void AddPoint(PointF PValue)
        {
            Points.Clear();
            base.AddPoint(PValue);
            FRect.Location = PValue;
            if (FRect.Width > FRect.Height)
            {
                FWidth = 64;
                FHight = 64 * (FRect.Height / FRect.Width);
            }
            else if (FRect.Width < FRect.Height)
            {
                FHight = 64;
                FWidth = 64 * (FRect.Width / FRect.Height);
            }
            else
            {
                FWidth = 64;
                FHight = 64;
            }
            foreach (CFocus Focus in FocusList)
            {
                Focus.SelectRectWidth = Math.Abs(CFocus.FocusRectWidth / (FWidth / FRect.Width));
                Focus.SelectRectHeight = Math.Abs(CFocus.FocusRectWidth / (FHight / FRect.Height));
            }
            foreach (CBase Ob in Children)
            {
                Ob.SelectedDisWMargin = Math.Abs(SelectedDisWMargin / (FWidth / FRect.Width));
                Ob.SelectedDisHMargin = Math.Abs(SelectedDisHMargin / (FHight / FRect.Height));
            }
            FDrawing = false;
        }
        public override void Transform(Matrix MatrixValue)
        {
            PointF Pointf = GetCenterPointF();
            MatrixValue.Scale(FWidth / FRect.Width, FHight / FRect.Height, MatrixOrder.Append);
            MatrixValue.Translate(FRect.Left, FRect.Top, MatrixOrder.Append);
            base.Transform(MatrixValue);
        }
        public PointF TransformPointF(PointF Point)
        {
            //Point = RotateTransformPointF(Point,GetCenterPointF(),-m_RotateAngle);
            Point.X -= FRect.Left;
            Point.Y -= FRect.Top;
            if (FWidth != 0)
                Point.X = Point.X / (FWidth / FRect.Width);
            if (FHight != 0)
                Point.Y = Point.Y / (FHight / FRect.Height);
            return Point;
        }
        public override void AddChild(CBase Child)
        {
            if (this.Children.IndexOf(Child) < 0)
            {
                this.Children.Add(Child);
                Child.Parent = this;
                Child.Locked = false;
                //Child.LayerName = this.LayerName;
            }

            if (this.Children.Count == 0)
            {
                return;
            }
            RectangleF ChildRect = Child.GetRect();
            if (Children.Count == 1)
                FRect = ChildRect;
            else
                FRect = RectangleF.Union(FRect, ChildRect);
            DrawPen.Width = Math.Max(Child.DrawPen.Width, DrawPen.Width);
            FWidth = FRect.Width;
            FHight = FRect.Height;
            FDrawing = false;
        }
        public override void AddChild(ArrayList Childs)
        {
        }
        //virtual void RemoveChild(CBase Child) override;
        //virtual ArrayList ChaifengChild() override;
        //virtual void RemoveAllChild() override;
        //virtual CBase CreateReDianAt(float x,float y,ConPointType Type) override;
        //virtual void MoveReDian(float x,float y)override;

        //virtual CBase Clone() override;
        //virtual CBase CopyTo(CBaseDesObject)override;
        //virtual void SaveToXML(XmlElement Node) override;
        public override void LoadFromXML(XmlElement Node)
        {
            base.LoadFromXML(Node);
            XmlElement CGroupNode = (XmlElement)(Node.SelectSingleNode("Misc"));
            IconName = CGroupNode.GetAttribute("IconName");
            Name = CGroupNode.GetAttribute("Name");
            if (IconName == "GrpIcon36")
                Name = "成组图标3";
            if (CGroupNode.HasAttribute("iType"))//成组图标
            {
                KJIconType = 1;
                foreach (XmlElement item in CGroupNode.ChildNodes)
                {
                    string sNodeName = item.Name;
                    string str1 = sNodeName.Substring(0, 2);
                    if ((sNodeName.Substring(0, 2) == "TY" || sNodeName.Substring(0, 2) == "KJ") && item.ChildNodes.Count > 0)
                    {
                        foreach (XmlElement TYNode in item.ChildNodes)
                        {
                            switch (TYNode.Name)
                            {
                                case "TYLine":
                                    CElementFactory.SetClassIndex(LCElementType.LINE);
                                    break;
                                case "TYText":
                                    CElementFactory.SetClassIndex(LCElementType.TEXT);
                                    break;
                                case "TYRect":
                                    CElementFactory.SetClassIndex(LCElementType.RECTANGLE);
                                    break;
                                case "TYEllipse":
                                    CElementFactory.SetClassIndex(LCElementType.ELLIPS);
                                    break;
                                case "TYRndRect":
                                    CElementFactory.SetClassIndex(LCElementType.ROUNDRECTANGLE);
                                    break;
                                case "TYArc":
                                    CElementFactory.SetClassIndex(LCElementType.ARC);
                                    break;
                                case "KJIcon":
                                    CElementFactory.SetClassIndex(LCElementType.GROUP);
                                    break;
                                default:
                                    continue;
                                    break;
                            }

                            CBase NewOb = CElementFactory.CreateElement(this, this);
                            if (NewOb == null)
                                return;
                            NewOb.LoadFromXML(TYNode);
                            AddChild(NewOb);
                        }
                    }
                }
            }
            else//固定图标
            {
                KJIconType = 2;

                KJIconList.AddKJIcon(IconName);
                XmlElement LayoutNode = (XmlElement)(Node.SelectSingleNode("Layout"));
                fRatio = float.Parse(LayoutNode.GetAttribute("fRatio"));
                if(LayoutNode.HasAttribute("iIconDir"))
                {
                    iIconDir = int.Parse(LayoutNode.GetAttribute("iIconDir"));
                    LayoutNode = (XmlElement)(Node.SelectSingleNode("Behavior/SwitchPics"));
                    foreach (XmlElement item in LayoutNode.ChildNodes)
                    {
                        string PicName = item.GetAttribute("PicName");
                        string Condition = item.GetAttribute("Condition");
                        SwitchPic nSw = new SwitchPic(PicName, Condition);
                        ListSwitchPic.Add(nSw);
                        KJIconList.AddKJIcon(nSw.PicName);
                    }
                }
            }
            return;
        }
        public CGroup()
            : base()
        {
            m_ElementType = LCElementType.GROUP;

            Children = new ArrayList();
            //SaveToIconLibItem = new ToolStripMenuItem();
            //SaveToIconLibItem.Image = (cli.safe_cast<System.Drawing.Image  >(resources.GetObject(L"tsmiGroup.Image")));
            //SaveToIconLibItem.Name = L"SaveToIconLibItem";
            //SaveToIconLibItem.Size = System.Drawing.Size(152, 22);
            //SaveToIconLibItem.Text = L"存入图形库";
            //SaveToIconLibItem.Click += new System.EventHandler(this, &CGroup.OnSaveToIconLib);
            SelectedDisWMargin = CBase.DISMARGIN;
            SelectedDisHMargin = CBase.DISMARGIN;
        }
        public  CGroup(String _Name, CBase _Parent, Object _Owner)
            : base(_Name, _Parent, _Owner)
        {
            m_ElementType = LCElementType.GROUP;

            Children = new ArrayList();
            //SaveToIconLibItem = new ToolStripMenuItem();
            //SaveToIconLibItem.Image = (cli.safe_cast<System.Drawing.Image  >(resources.GetObject(L"tsmiGroup.Image")));
            //SaveToIconLibItem.Name = L"SaveToIconLibItem";
            //SaveToIconLibItem.Size = System.Drawing.Size(152, 22);
            //SaveToIconLibItem.Text = L"存入图形库";
            //SaveToIconLibItem.Click += new System.EventHandler(this, &CGroup.OnSaveToIconLib);
            SelectedDisWMargin = CBase.DISMARGIN;
            SelectedDisHMargin = CBase.DISMARGIN;
        }
    }
}
