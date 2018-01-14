using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace LSSCADA
{
    public class ImageEditor : UITypeEditor
    {
        public ImageEditor() { }

        // Indicates whether the UITypeEditor provides a form-based (modal) dialog,
        // drop down dialog, or no UI outside of the properties window.
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // Displays the UI for value selection.
        public override Object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, Object value)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Application.StartupPath + "\\Image\\";
            openFileDialog1.FileName = (String)value;
            openFileDialog1.Filter = "All files(*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg"
                                    + "|Bmp files (*.bmp)|*.bmp"
                                    + "|png files (*.png)|*.png"
                                    + "|jpg files (*.jpg)|*.jpg";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String TextureFile = System.IO.Path.GetFileName(openFileDialog1.FileName);
                if (openFileDialog1.FileName != Application.StartupPath + "\\Image\\" + TextureFile)
                    System.IO.File.Copy(openFileDialog1.FileName, Application.StartupPath + "\\Image\\" + TextureFile, true);
                value = TextureFile;
            }
            //delete openFileDialog1;
            return value;
        }
    }
    //元素类型
    public enum LCElementType
    {
        POINTER        , 
        TEXT, 
        LINE, 
        RECTANGLE, 
        ROUNDRECTANGLE, 
        ELLIPS, 
        POLYLINE, 
        POLYGON,
        ARC,  //图元类型
        Pipe,//管
        Ring,//圆环
        KJRuler,//标尺
        BUTTON,
        EDIT,															//控件类型
        FORM,																	//窗口
        GROUP,																	//成组
        CONPIONT,
        ICON            ,
        PRG            ,
        KuiXianPRCG            , 
        HISCURVECHART, 
        REALCURVECHART,
        CHARGECURVECHART, 
        IMAGECONTROL,
        TABLECONTROL	//控件类型
    }

    //绘图状态
    public enum LCDrawingState
    {
        None, New, MoveElement, MoveFocus, MultiSelect, MultiMove, EDITCONPIONT
    }

    //活动点方向类型
    public enum LCFocusType
    {
        None, WestNorth, North, EastNorth, East, EastSouth, South, WestSouth, West, SizeAll, Start, End
    }

    //活动点绘制类型
    public enum LCFocusDrawMode
    {
        SingleSelect, MultiSelect, Locked
    }

    public delegate void CTransform(Matrix MatrixValue);
    public delegate void CObjectChange(Object Sender, bool CanUndo);
    public delegate bool CQuryReName(Object Sender, String Name);
    public delegate void FocusEvent(Object Sender, bool bFocused);
    enum ConPointType { A, B }

    public class CBase : ContainerControl
    {
        public Object m_Owner;							//所在窗口
        public CBase m_Parent;							//父亲
        public String m_Name;								//名
        public String m_Path;								//路径
        public PointF m_Location;			//位置
        public LCElementType m_ElementType;				//元素类型	
        //CLayer m_Layer;							//所在图层
        public const float DISMARGIN = 2;			//选择范围	
        //IC2000* m_c2000;
        public bool FIsChanged;
        //public:	
        public ArrayList Children;						//子元素
        public ArrayList ReDians;
        public ArrayList Points;
        public ArrayList FocusList;
        public bool FisLive;
        public float m_fAngleAtPosition;
        public SizeF m_RotatePosition;                 //用于Group的原点变换
        public bool ReDianVisible;
        public float SelectedDisWMargin;
        public float SelectedDisHMargin;
        public float m_RotateAngle;						//旋转角度
        public Matrix Copy_Matrix;						//原有Matrix的拷贝，用于还原
        public bool FIsFocused;
        public bool FIsSeleced;
        public bool FDrawing;//处于绘制状态。
        public PointF PreviewPoint;//用于预览的一个点。
        public bool EditingReDian;

        //Group用
        public Int32 KJIconType = 0;    //图标类型，1成组图标，2图标
        public string IconName = "";    //图标名称
        public float fRatio = 0;        //缩放比例
        public int iIconDir = 0;        //旋转角度

        // 当前鼠标坐标
        public PointF m_MouseLocation;

        //Appearance属性
        public Int32 iElementOrder = 0;//前后顺序 数值越小，越先画
        public Int32 tbruStyle = 0;
        public Color tbruColor;
        public Color FillColor;
        public Color EmgColor ;
        public Int32 iShadeFillStyle = 0;
        //Misc
        public string GLayerName = "";
        //public string Name = "";
        //Layout
        public Single iOrgX1 = 0;
        public Single iOrgX2 = 0;
        public Single iOrgY1 = 0;
        public Single iOrgY2 = 0;

        [Browsable(false)]
        public virtual Brush DrawBrush							//画刷
        {
            get { return null; }
        }
        [Browsable(false)]
        public Object Owner
        {
            get { return m_Owner; }
            set
            {
                m_Owner = value;
                foreach (CBase Object in Children)
                {
                    Object.Owner = value;
                }
            }
        }
        [Browsable(false)]
        public CBase Parent
        {
            get { return m_Parent; }

            set
            {
                m_Parent = value;
                if (m_Parent != null)
                    m_Path = Parent.Path + "/" + m_Name;
                else
                    m_Path = m_Name;
            }
        }
        [Browsable(true), Description("名称"), Category("Design"), DisplayName("名称")]
        public String Name
        {
            get
            { return m_Name; }
            set
            {
                m_Name = value;
            }
        }
        [Browsable(true), Description("路径"), Category("Design"), DisplayName("路径")]
        public String Path
        {
            get { return m_Path; }
        }
        [Browsable(false), Description("类型"), Category("Design")]
        public String TypeName
        {
            get { return this.ElementType.ToString(); }
        }
        [Browsable(false)]
        public LCElementType ElementType
        {
            get { return this.m_ElementType; }
        }
        [Browsable(false)]
        public Pen DrawPen;								//画笔
        [Browsable(true), Description("颜色"), Category("Appearance"), DisplayName("颜色")]
        public Color Color
        {
            get { return DrawPen.Color; }
            set
            {
                DrawPen.Color = value;

                if (this.ChildCount > 0)
                    foreach (CBase Child in this.Children)
                        Child.Color = value;
            }
        }
        [Browsable(true), Description("范围"), Category("Appearance"), DisplayName("范围")]
        public RectangleF Rect
        {
            get { return GetRect(); }

            set
            {
                //这里要对元素进行拉伸变换。
                //m_Rect = value;
                //m_Location = m_Rect.Location;
                //OnRectChange();
            }
        }
        [Browsable(true), Description("位置"), Category("Layout"), DisplayName("位置")]
        public PointF Location
        {
            get { return GetRect().Location; }
            set { }
        }
        [Browsable(true), Description("左上角横坐标"), Category("Layout")]
        public float X
        {
            get { return GetRect().X; }
        }
        [Browsable(true), Description("左上角纵坐标"), Category("Layout")]
        public float Y
        {
            get { return GetRect().Top; }
        }
        [Browsable(true), Description("大小"), Category("Layout"), DisplayName("大小")]
        public virtual SizeF Size
        {
            get
            {
                SizeF szTemp = GetPointsRect().Size;
                return szTemp;
            }

            set
            {
                //if (this.IsLocked())
                //    return;

                if (Math.Abs(value.Width) > Int32.MaxValue || Math.Abs(value.Height) > Int32.MaxValue)
                    return;

                //拉伸变换。
                RectangleF Rectf = GetPointsRect();
                float W = value.Width / Rectf.Width;
                float H = value.Height / Rectf.Height;
                foreach (CFocus Focus in FocusList)
                {
                    if (Focus.FocusType == LCFocusType.EastSouth)
                    {
                        PointF ptemp = Focus.Point;
                        ptemp.X += -Rectf.Left;
                        ptemp.Y += -Rectf.Top;
                        ptemp.X *= W;
                        ptemp.Y *= H;
                        ptemp.X += Rectf.Left;
                        ptemp.Y += Rectf.Top;
                        ptemp.X -= Focus.Point.X;
                        ptemp.Y -= Focus.Point.Y;
                        Focus.Selected = true;
                        bool temp = FIsFocused;
                        FIsFocused = true;
                        //ptemp = RotateTransformPointF(ptemp,new PointF(0,0),m_RotateAngle);
                        //Move(ptemp);
                        Focus.Selected = false;
                        FIsFocused = temp;
                        return;
                    }
                }
            }
        }
        [Browsable(false), Description("子元素")]
        public CBase this[int index]
        {
            get
            {
                if (this.Children == null || index < 0 || index > this.Children.Count)
                    return null;
                return (CBase)this.Children[index];
            }
        }

        [Browsable(false), Description("子元素数量")]
        public int ChildCount
        {
            get
            {
                if (this.Children == null)
                    return 0;
                return this.Children.Count;
            }
        }
        [Browsable(false), Description("是否锁定")]
        public bool Locked;
        [Browsable(true), Description("是否可见"), Category("Behavior")]//, DisplayName ("可见")]
        public bool Visible;
        [Browsable(true), Description("是否使能"), Category("Behavior")]//, DisplayName("使能")]
        public bool Enabled;

        //元素知道自己是多选时，可以不触发，触发也没有关系，因为窗口是不会处理的。
        CTransform OnTransform = null;
       
        public Graphics fg;
        public RectangleF FPreviousRect;

        // 当前鼠标坐标
        ArrayList ObjectUndolst;
        public int UndoPointer;
        public virtual PointF GetReverseRotatePoint(PointF pt)
        {
            //获取点被反向旋转后的坐标	
            RectangleF Rect = GetRect();
            float CenterX = Rect.Location.X + Rect.Width / 2;
            float CenterY = Rect.Location.Y + Rect.Height / 2;
            pt = PointF.Subtract(pt, new SizeF(CenterX, CenterY));

            PointF[] TransformList = { pt };
            Matrix TransformMatrix = new Matrix();
            TransformMatrix.Rotate(-m_RotateAngle);
            TransformMatrix.TransformPoints(TransformList);
            pt = TransformList[0];
            pt = PointF.Add(pt, m_RotatePosition);
            return pt;
        }					//获取点被反向旋转后的坐标
        //virtual void OnMoveFocus( CFocus Focus,System.Drawing.SizeF Offset) = 0 ;
        public virtual RectangleF GetPointsRect()
        {
            RectangleF rect = new RectangleF();
            if (Points.Count > 0)
            {
                PointF MinP = (PointF)Points[0];
                PointF MaxP = (PointF)Points[0];
                foreach (PointF P in Points)
                {
                    MinP.X = Math.Min(P.X, MinP.X);
                    MinP.Y = Math.Min(P.Y, MinP.Y);
                    MaxP.X = Math.Max(P.X, MaxP.X);
                    MaxP.Y = Math.Max(P.Y, MaxP.Y);
                }
                rect.Location = MinP;
                rect.Size = new SizeF(MaxP.X - MinP.X, MaxP.Y - MinP.Y);
            }
            if (this.Parent != null)
            {
                rect.Location = PointF.Add(rect.Location, this.Parent.m_RotatePosition);
            }
            return rect;
        }
        public Matrix myPathMatrix;
        public GraphicsPath myGraphicsPath;
        public virtual void DrawPoints(Graphics g)
        {//显示热点
            myGraphicsPath.Reset();

            if (ReDianVisible && Parent == null)//只有顶层元素才显示热点。
            {
                foreach (CBase Redian in ReDians)
                {
                    Redian.Draw(g);
                }
            }
        }
        public virtual PointF GetCenterPointF()
        {
            PointF Point = new PointF();
            if (Points.Count > 0)
            {
                PointF MinP = (PointF)Points[0];
                PointF MaxP = (PointF)Points[0];
                foreach (PointF P in Points)
                {
                    MinP.X = Math.Min(P.X, MinP.X);
                    MinP.Y = Math.Min(P.Y, MinP.Y);
                    MaxP.X = Math.Max(P.X, MaxP.X);
                    MaxP.Y = Math.Max(P.Y, MaxP.Y);

                }
                Point.X = (MaxP.X + MinP.X) / 2;
                Point.Y = (MaxP.Y + MinP.Y) / 2;

            }

            return Point;
        }
        public virtual CBase Clone(CBase _Parent) 
        {
            CBase obj = (CBase)this.MemberwiseClone();
            obj.m_Parent = _Parent;
            return obj;
        }

        public virtual void AddChild(CBase Child) { }
        public virtual void AddChild(ArrayList Childs) { }
        public virtual void Draw(Graphics g)
        {
            if (IconName == "断路器1分闸")
                Name = IconName;
            if (!Visible)
            {
                CBase parentbase = this;
                while (parentbase.Parent != null)
                    parentbase = parentbase.Parent;
            }
            if (Children != null)
            {
                foreach (CBase Object in Children)
                {
                    Object.Draw(g);
                }
            }
            DrawPoints(g);
        }

        public virtual void Draw(Graphics g,Single iS)
        {
           // DrawPoints(g);
        }

        public virtual void DrawFocus(Graphics g)
        {
            foreach (CFocus Focus in FocusList)
            {
                Focus.FocusDrawMode = FIsFocused ? LCFocusDrawMode.SingleSelect : LCFocusDrawMode.MultiSelect;
                Focus.Draw(g, myPathMatrix);
            }
        }
        //Selected 和 Foucused都是true时为单选，只有Selected时是多选。//元件本身就通过这个规则知道多选还是单选，
        public virtual bool Selected(PointF SelectPoint)
        {
            if (!FisLive)
                return false;
            bool ret = false;
            foreach (CFocus focuse in FocusList)
            {
                focuse.Selected = focuse.Select(SelectPoint);
                if (focuse.Selected)
                    ret = true;
            }
            return ret;
        }
        
        public virtual void AddPoint(PointF PValue)
        {
            if (Points.Count == 0)
                Points.Add(new PointF(PValue.X, PValue.Y));
            else
                //{
                //    if (PValue != ((PointF)Points[Points.Count-1])) 
                Points.Add(new PointF(PValue.X, PValue.Y));
            //}
            PreviewPoint = PValue;
        }
        //public virtual void SetLastPoint(PointF PValue) { }
        public virtual RectangleF GetRect()
        {
            RectangleF rect = new RectangleF();
            if (Points.Count > 0)
            {
                PointF MinP = (PointF)Points[0];
                PointF MaxP = (PointF)Points[0];
                foreach (PointF P in Points)
                {
                    MinP.X = Math.Min(P.X, MinP.X);
                    MinP.Y = Math.Min(P.Y, MinP.Y);
                    MaxP.X = Math.Max(P.X, MaxP.X);
                    MaxP.Y = Math.Max(P.Y, MaxP.Y);

                }
                rect.Location = MinP;
                rect.Size = new SizeF(MaxP.X - MinP.X, MaxP.Y - MinP.Y);
                //rect = RotateTransformRectangleF(rect,GetCenterPointF(),m_RotateAngle);
            }

            return rect;
        }

        public void Move(PointF PValue)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                PointF P = (PointF)Points[i];
                P.X += PValue.X;
                P.Y += PValue.Y;
            }
            //焦点
            for (int i = 0; i < FocusList.Count; i++)
            {
                CFocus Focus = (CFocus)FocusList[i];
                Focus.Point.X += PValue.X;
                Focus.Point.Y += PValue.Y;
            }
        }

        public void MoveTo(PointF PValue)
        {
            //以下整个平移。
            RectangleF rect;
            rect = GetRect();
            PointF Offset = new PointF(PValue.X - rect.Location.X, PValue.Y - rect.Location.Y);
            Move(Offset);
        }

        public virtual void Transform(Matrix MatrixValue)
        {
            if (Parent != null)
                Parent.Transform(MatrixValue);
            else if (OnTransform != null)
            {
                OnTransform(MatrixValue);
            }
        }
        //public virtual CBase GetFlagObject() { }
        //public virtual ArrayList CreateShortCutObject() { }
        //public virtual void AtachShortCutObject() { }
        //public virtual String GetToolTip() { return null; }
        //public virtual String ToString() { }
        //public virtual CBase CopyTo(CBase DesObject) { }
        /*public virtual void SaveToXML(XmlElement Node)
        {
            if (ObjectUndolst.Count > 0)
	        {
		        CBase StartState = ((CBase)ObjectUndolst[UndoPointer]).Clone();
		        foreach (Object UndoObject in ObjectUndolst)
			        delete (CBase)UndoObject;
		        ObjectUndolst.Clear();
		        ObjectUndolst.Add(StartState);
		        UndoPointer = 0;
	        }
	        XmlAttribute newAttr = Node.OwnerDocument.CreateAttribute( "ElementType" );
	        newAttr.Value = m_ElementType.ToString();
	        Node.Attributes.Append(newAttr);

	        XmlElement CBaseNode = Node.OwnerDocument.CreateElement("CBase");
	        Node.AppendChild(CBaseNode);

	        CBaseNode.SetAttribute("Name",Name);	
	        CBaseNode.SetAttribute("LayerName",LayerName);
	        //CBaseNode.SetAttribute("Locked",Locked.ToString());
	        CBaseNode.SetAttribute("Visible",Visible.ToString());
	        CBaseNode.SetAttribute("RotateAngle", m_RotateAngle.ToString());
	        CBaseNode.SetAttribute("Enabled",Enabled.ToString());
	        CBaseNode.SetAttribute("Color",Color.ToArgb().ToString());

	        String strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", DrawPen.Width);
	        CBaseNode.SetAttribute("PenWidth", strTemp);

	        strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", (int)DrawPen.DashStyle);
	        CBaseNode.SetAttribute("DashStyle", strTemp);

	        XmlElement  PointNode;
	        foreach ( PointF  Point in Points)
	        {
		        PointNode =Node.OwnerDocument.CreateElement("PointF");
		        Node.AppendChild(PointNode);
		        PointNode.SetAttribute("X",Point.X.ToString());
		        PointNode.SetAttribute("Y",Point.Y.ToString());
	        }

	        XmlElement ChildrenNode ;
	        foreach ( CBase ChildrenObject in Children)
	        {
		        ChildrenNode = Node.OwnerDocument.CreateElement("Children");
		        Node.AppendChild(ChildrenNode);
		        ChildrenObject.SaveToXML(ChildrenNode);	
	        }

	        XmlElement ReDianNode;
	        foreach (CBase  Redian in ReDians)
	        {
		        ReDianNode = Node.OwnerDocument.CreateElement("Redian");
		        Node.AppendChild(ReDianNode);
		        Redian.SaveToXML(ReDianNode);
	        }

	        m_CartoonData.SaveToXML(Node);
	        XmlElement FOnClickNode ;
	        if (FOnClick.Program != "")
	        {
		        FOnClickNode = Node.OwnerDocument.CreateElement("FOnClick");
		        Node.AppendChild(FOnClickNode);
		        FOnClick.SaveToXML(FOnClickNode);
	        }
	
	        if (FOnDoubleClick.Program != "")
	        {
		        XmlElement FOnDoubleClickNode ;
		        FOnDoubleClickNode = Node.OwnerDocument.CreateElement("FOnDoubleClick");
		        Node.AppendChild(FOnDoubleClickNode);
		        FOnDoubleClick.SaveToXML(FOnDoubleClickNode);
	        }
	
	        if (FOnMouseMoving.Program != "")
	        {
		        XmlElement FOnMouseMovingNode ;
		        FOnMouseMovingNode = Node.OwnerDocument.CreateElement("FOnMouseMoving");
		        Node.AppendChild(FOnMouseMovingNode);
		        FOnMouseMoving.SaveToXML(FOnMouseMovingNode);
	        }
        }*/
        public virtual void LoadFromXML(XmlElement Node)
        {
            String strTemp = "";
            XmlElement CBaseNode = (XmlElement)(Node.SelectSingleNode("Misc"));
            Name = CBaseNode.GetAttribute("Name");

            CBaseNode = (XmlElement)(Node.SelectSingleNode("Appearance"));
            if (CBaseNode.HasAttribute("iElementOrder"))
            {
                strTemp = CBaseNode.GetAttribute("iElementOrder");
                iElementOrder = Int32.Parse(strTemp);
            }

            if (CBaseNode.HasAttribute("tpenColor"))
            {
                String argb = CBaseNode.GetAttribute("tpenColor");
                DrawPen.Color = ColorTranslator.FromWin32(Convert.ToInt32(argb));

                strTemp = CBaseNode.GetAttribute("tpenWidth");
                DrawPen.Width = Int32.Parse(strTemp);

                strTemp = CBaseNode.GetAttribute("tpenStyle");
                DrawPen.DashStyle = (DashStyle)Enum.Parse(typeof(DashStyle), strTemp);
            }

            if (CBaseNode.HasAttribute("tbruStyle"))
            {
                tbruStyle =Convert.ToInt32(CBaseNode.GetAttribute("tbruStyle"));
                tbruColor =ColorTranslator.FromWin32( Convert.ToInt32(CBaseNode.GetAttribute("tbruColor")));
                FillColor =ColorTranslator.FromWin32( Convert.ToInt32(CBaseNode.GetAttribute("FillColor")));
                EmgColor =ColorTranslator.FromWin32( Convert.ToInt32(CBaseNode.GetAttribute("EmgColor")));
                iShadeFillStyle = Convert.ToInt32(CBaseNode.GetAttribute("iShadeFillStyle"));
            }

            CBaseNode = (XmlElement)(Node.SelectSingleNode("Behavior"));

            if (CBaseNode.HasAttribute("bVirShow"))
                Visible = Convert.ToBoolean(CBaseNode.GetAttribute("bVirShow"));

            CBaseNode = (XmlElement)(Node.SelectSingleNode("Layout"));

            if (CBaseNode.HasAttribute("iOrgX1"))
            {
                strTemp = CBaseNode.GetAttribute("iOrgX1");
                iOrgX1 = float.Parse(strTemp);
                strTemp = CBaseNode.GetAttribute("iOrgY1");
                iOrgY1 = float.Parse(strTemp);
                strTemp = CBaseNode.GetAttribute("iOrgX2");
                iOrgX2 = float.Parse(strTemp);
                strTemp = CBaseNode.GetAttribute("iOrgY2");
                iOrgY2 = float.Parse(strTemp);
            }
            if (CBaseNode.HasAttribute("Left"))
            {
                strTemp = CBaseNode.GetAttribute("Left");
                iOrgX1 = float.Parse(strTemp);
                strTemp = CBaseNode.GetAttribute("Top");
                iOrgY1 = float.Parse(strTemp);
                strTemp = CBaseNode.GetAttribute("Width");
                iOrgX2 = float.Parse(strTemp);
                strTemp = CBaseNode.GetAttribute("Height");
                iOrgY2 = float.Parse(strTemp);
            }
            if (CBaseNode.HasAttribute("Rotate"))
                m_RotateAngle = Convert.ToSingle(CBaseNode.GetAttribute("Rotate"));
            if (CBaseNode.ChildNodes.Count == 0)
            {
                AddPoint(new PointF(iOrgX1, iOrgY1));
                AddPoint(new PointF(iOrgX2, iOrgY2));
            }
            else
            {
                XmlElement CPointNode = (XmlElement)(Node.SelectSingleNode("Layout/Points"));
                foreach (XmlElement node in CPointNode.ChildNodes)
                {
                    int x =int.Parse( node.GetAttribute("x"));
                    int y =int.Parse( node.GetAttribute("y"));
                    AddPoint(new PointF(x, y));
                }
                Points[0] = new PointF(iOrgX1, iOrgY1);
            }
            if (this.Parent == null)
                m_RotatePosition = new SizeF(iOrgX1, iOrgY1);
            else
                m_RotatePosition = this.Parent.m_RotatePosition;
            //Enabled = Convert.ToBoolean(CBaseNode.GetAttribute("Enabled"));
            /*XmlNodeList PointList = Node.SelectNodes("PointF");
            for (int i = 0 ; i< PointList.Count -1 ; i++)
            {
                XmlElement PointNode = (XmlElement)PointList[i];
                float X = Convert.ToSingle(PointNode.GetAttribute("X"));
                float Y = Convert.ToSingle(PointNode.GetAttribute("Y"));
                AddPoint(PointF(X,Y));
            }
            if (PointList.Count >= 1 )
            {
                XmlElement PointNode = (XmlElement)PointList[PointList.Count - 1];
                float X = Convert.ToSingle(PointNode.GetAttribute("X"));
                float Y = Convert.ToSingle(PointNode.GetAttribute("Y"));
                //SetLastPoint(PointF(X,Y));
            }
            else
                FDrawing = false;
	
            */
            /*XmlNodeList ChildList = Node.SelectNodes("Children");
            foreach (XmlElement  ChildNode in ChildList )
            {
                LCElementType temp = (LCElementType)Enum.Parse(typeof(LCElementType),ChildNode.GetAttribute("ElementType"));
                //CElementFactory.SetClassIndex(temp);
                //Owner;
                //CBase ChildObject = CElementFactory.CreateElement(this,Owner);
                //ChildObject.LoadFromXML(ChildNode);
                //Children.Add(ChildObject);
            }

	
            //m_CartoonData.LoadFromXML(Node);

            XmlElement FOnClickNode = (XmlElement)(Node.SelectSingleNode("FOnClick"));
            //if (FOnClickNode != null)
            //    FOnClick.LoadFromXML(FOnClickNode);

            XmlElement FOnDoubleClickNode = (XmlElement)(Node.SelectSingleNode("FOnDoubleClick"));
            //if (FOnDoubleClickNode != null)
            //   FOnDoubleClick.LoadFromXML(FOnDoubleClickNode);

            XmlElement FOnMouseMovingNode = (XmlElement)(Node.SelectSingleNode("FOnMouseMoving"));
            //if (FOnMouseMovingNode != null)
            //    FOnMouseMoving.LoadFromXML(FOnMouseMovingNode);
             * */
        }
        //public virtual String GetPropertyValue(String PropertyName) { }
        //public virtual int SetPropertyValue(String PropertyName, String Value) { }
        public CBase()
        {
            //isflash = false;
            Locked = false;
            Visible = true;
            Enabled = true;
            if (Parent != null)
                m_Path = Parent.Path + "/" + Name;

            //m_CartoonData = new CCartoonData;
            m_ElementType = LCElementType.POINTER;
            DrawPen = (Pen)(Pens.DeepSkyBlue.Clone());

            m_fAngleAtPosition = 0;

            //FOnClick = new CProgram();
            //FOnDoubleClick = new CProgram();
            //FOnMouseMoving = new CProgram();
            //if (Owner != null)
            //	m_Layer =  ((DrawChild )Owner).GetActiveLayer();
            //m_LayerName = "";
            Children = new ArrayList();
            ReDians = new ArrayList();
            FocusList = new ArrayList();
            Points = new ArrayList();
            fg = null;
            FDrawing = true;
            EditingReDian = false;
            myPathMatrix = new Matrix();
            myGraphicsPath = new GraphicsPath();
            SelectedDisWMargin = DISMARGIN;
            SelectedDisHMargin = DISMARGIN;
            //OnChangeName = null;
            //OnTransform= null;
            ObjectUndolst = new ArrayList();
            FisLive = false;
            UndoPointer = 0;
            //FTimer = null;
        }
        public CBase(String _Name, CBase _Parent, Object _Owner)
        {
            //isflash = false;
            Name = _Name;
            if (Name == null)
                Name = "";
            Parent = _Parent;
            m_Owner = _Owner;
            Locked = false;
            Visible = true;
            Enabled = true;

            if (Parent != null)
                m_Path = Parent.Path + "/" + Name;

            //m_CartoonData = new CCartoonData;
            m_ElementType = LCElementType.POINTER;
            DrawPen = (Pen)(Pens.DeepSkyBlue.Clone());

            m_fAngleAtPosition = 0;

            //FOnClick = new CProgram();
            ////FOnDoubleClick = new CProgram();
            //FOnMouseMoving = new CProgram();

            //m_Layer =  ((DrawChild )Owner).GetActiveLayer();
            //m_LayerName = "";
            Children = new ArrayList();
            ReDians = new ArrayList();
            FocusList = new ArrayList();
            Points = new ArrayList();
            fg = null;
            FDrawing = true;
            EditingReDian = false;
            myPathMatrix = new Matrix();
            myGraphicsPath = new GraphicsPath();
            SelectedDisWMargin = DISMARGIN;
            SelectedDisHMargin = DISMARGIN;
            //OnChangeName = null;
            //OnTransform = null;
            ObjectUndolst = new ArrayList();
            UndoPointer = 0;
            FisLive = false;
            //FTimer = null;
        }
        //public virtual void SetFocused(bool value) { }
        //这个元素是否有焦点.
        //public virtual bool GetFocused() { return FIsFocused; }
        //public virtual void Undo() { }
        //public virtual void Redo() { }
        //public virtual void Push() { }
        //public virtual RectangleF GetViewRect() { }
        //public virtual RectangleF GetPreviousRect() { }
        //从上次Push到现在是否有属性发生改变。
        //bool IsChanged() { }
        //   #pragma endregion

        //实现IBase_Run的接口
        //    #pragma region _IBase_Run
        //   public:
        //public Timer FTimer;//实现闪烁的定时器。
        //public virtual void ResetColorInfo() { }
        //public virtual void DoFlash(){}
        //public void OnFTimer(Object sender, EventArgs e){}
        //void SetTransform(CTransform OnTransform){}
        //public virtual void UpDate(){}//有属性被更新返回ture否则返回false;
        //和组态版一样。
        //void Draw(Graphics g){}
        //public void Show(){}
        //public void Hide(){}
        //public bool BVisible(){}
        //public void OnMouseDown(PointF MouseDownPoint){}
        //public void OnMouseMove(PointF MouseDownPoint){}
        //void OnMouseUp(PointF MouseDownPoint){}
        //public void OnFormDoubleClick(PointF MouseDownPoint){}
        //和组态版一样。
        //void LoadFromXML(XmlElement Node){}
        //#pragma  endregion
        //public:

        //为运行版做的但是要废掉。
        //public virtual void RotateAtPosition(PointF PPosition, double fAngle){}

        //virtual int DoFill(Graphics gdev, Drawing.Brush brush, PointF position, SizeF size){}
        //virtual int DoFlash(Graphics gdev, Drawing.Pen pen, Drawing.Brush brush){}
        //int SetupTimer(){}
        //int KillTimer(){}

        //当对象发生一个可以撤销的动作时就触发这个事件。
        //int UpdateWnd(Graphics g){}
        //动画的支持
        //public bool IsCartoon(){}	
    }

    class CFocus
    {
        //public:
        public const int FocusRectWidth = 5;				//活动点矩形大小
        public float SelectRectWidth;
        public float SelectRectHeight;
        public LCFocusType FocusType;
        public LCFocusDrawMode FocusDrawMode;
        public PointF Point;
        public bool Selected;
        public GraphicsPath GPath;
        public CFocus()
        {
            Selected = false;
            //GPath = new GraphicsPath;
            SelectRectWidth = FocusRectWidth;
            SelectRectHeight = FocusRectWidth;
        }
        public void CopyFrom(CFocus focus)
        {
            FocusType = focus.FocusType;
            FocusDrawMode = focus.FocusDrawMode;
            Point = focus.Point;
        }
        public void MoveTo(PointF PValue)
        {

        }
        public void Move(PointF PValue)
        {

        }
        public bool Select(PointF P)
        {
            RectangleF FocusRect = new RectangleF(0, 0, SelectRectWidth, SelectRectHeight);
            FocusRect.Location = new PointF(Point.X - SelectRectWidth / 2, Point.Y - SelectRectHeight / 2);
            return FocusRect.Contains(P);
        }
        public void Draw(Graphics g, Matrix MatrixValue)
        {
            GPath.Reset();
            RectangleF FocusRect = new RectangleF(0, 0, FocusRectWidth, FocusRectWidth);
            Brush FillBrush = Brushes.Yellow;
            if (FocusDrawMode == LCFocusDrawMode.MultiSelect)
                FillBrush = Brushes.White;
            else if (FocusDrawMode == LCFocusDrawMode.Locked)
                FillBrush = Brushes.Gray;
            float Offset = FocusRectWidth / 2;
            PointF[] Points = new PointF[12];
            Points[0].X = Point.X;
            Points[0].Y = Point.Y;
            MatrixValue.TransformPoints(Points);
            FocusRect.Location = new PointF(Points[0].X - Offset, Points[0].Y - Offset);
            //GPath.AddRectangle(FocusRect);
            //g.FillPath(FillBrush,GPath);
            //g.DrawPath(Pens.Black,GPath);
        }
    }
    class CTileTypeConverter : StringConverter
    {
        //public:
        /*public bool GetStandardValuesSupported(ITypeDescriptorContext context)
	    {
		    return true;
	    }
            public bool GetStandardValuesExclusive(ITypeDescriptorContext context) 
	    {
		    return true;
	    }*/

        //public TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context){}

        protected Guid instanceGUID;

        CTileTypeConverter()
        { }
    }

    class CElementFactory
    {
        public static LCElementType Type;
        public static void SetClassIndex(LCElementType index)//工具条选择。
        {
            Type = index;
        }
        public static CBase CreateElement()
        {
            switch (Type)
            {
                case LCElementType.TEXT:
                    return new CText();
                case LCElementType.LINE:
                    return new CLine();
                case LCElementType.RECTANGLE:
                    return new CRectangle();
                case LCElementType.ELLIPS:
                    return new CEllips();
                case LCElementType.IMAGECONTROL:
                    return new ImageControl();
                case LCElementType.Pipe:
                    return new CPipe();
                case LCElementType.Ring:
                    return new CRing();
                case LCElementType.KJRuler:
                    return new KJRuler();
                //case LCElementType.ROUNDRECTANGLE:
                //    return new CRoundRectangle();
                //case LCElementType.ARC:
                //    return new CArc();
                //case LCElementType.GROUP:
                //    return new CGroup();
                //case LCElementType.POLYLINE:
                //    return new CPolyLine();
                /*case LCElementType.CONPIONT:
                    return new CReDian();
                case LCElementType.KuiXianPRCG:
                    return new CKuiXianPRCG();
                case LCElementType.HISCURVECHART:
                    return new CCurveChart(LCElementType.HISCURVECHART);
                case LCElementType.REALCURVECHART:
                    return new CCurveChart(LCElementType.REALCURVECHART);
                case LCElementType.CHARGECURVECHART:
                    return new CCurveChart(LCElementType.CHARGECURVECHART);
                case LCElementType.TABLECONTROL:
                    return new CTable();*/
                default:
                    return null;
            }
        }
        public static CBase CreateElement(CBase Parent, Object Owner)
        {
            switch (Type)
            {
                case LCElementType.TEXT:
                    return new CText("", Parent, Owner);
                case LCElementType.LINE:
                    return new CLine("", Parent, Owner);
                case LCElementType.RECTANGLE:
                    return new CRectangle("", Parent, Owner);
                case LCElementType.ELLIPS:
                    return new CEllips("", Parent, Owner);
                case LCElementType.IMAGECONTROL:
                    return new ImageControl("", Parent, Owner);
                case LCElementType.Pipe:
                    return new CPipe();
                case LCElementType.Ring:
                    return new CRing();
                case LCElementType.KJRuler:
                    return new KJRuler();
                //case LCElementType.ROUNDRECTANGLE:
               //     return new CRoundRectangle("", Parent, Owner);
                //case LCElementType.ARC:
                //    return new CArc("", Parent, Owner);
                //case LCElementType.GROUP:
                //    return new CGroup("", Parent, Owner);
                //case LCElementType.POLYLINE:
                //    return new CPolyLine("", Parent, Owner);
                /*case LCElementType.POLYGON:
                    return new CPolygon("", Parent, Owner);
                case LCElementType.PRG:
                    return new CRectPRCG("",Parent,Owner);
                case LCElementType.KuiXianPRCG:
                    return new CKuiXianPRCG("",Parent,Owner);
                case LCElementType.HISCURVECHART:
                    return new CCurveChart(LCElementType.HISCURVECHART,"",Parent,Owner);
                case LCElementType.REALCURVECHART:
                    return new CCurveChart(LCElementType.REALCURVECHART,"",Parent,Owner);
                case LCElementType.CHARGECURVECHART:
                    return new CCurveChart(LCElementType.CHARGECURVECHART,"",Parent,Owner);
                case LCElementType.TABLECONTROL:
                    return new CTable("",Parent,Owner);
                //case LCElementType.PRG:
                //	return new CRectPRCG();*/
                default:
                    return null;
            }
        }
        public static CBase CreateElement(String Name, LCElementType Type, CBase Parent, Object Owner)
        {
            //根据类型生成相应的元素对象
            switch (Type)
            {
                case LCElementType.TEXT:
                    return new CText(Name, Parent, Owner);
                case LCElementType.LINE:
                    return new CLine(Name, Parent, Owner);
                case LCElementType.RECTANGLE:
                    return new CRectangle(Name, Parent, Owner);
                case LCElementType.ELLIPS:
                    return new CEllips(Name, Parent, Owner);
                case LCElementType.IMAGECONTROL:
                    return new ImageControl(Name, Parent, Owner);
                //case LCElementType.ROUNDRECTANGLE:
                //    return new CRoundRectangle(Name, Parent, Owner);
                //case LCElementType.ARC:
                //    return new CArc(Name, Parent, Owner);
                //case LCElementType.GROUP:
                //    return new CGroup(Name, Parent, Owner);
                //case LCElementType.POLYLINE:
                //    return new CPolyLine(Name, Parent, Owner);
                /*case LCElementType.POLYGON:
                    return new CPolygon(Name,Parent,Owner);
                case LCElementType.ARC:
                    return new CArc(Name,Parent,Owner);
                case LCElementType.HISCURVECHART:
                    return new CCurveChart(LCElementType.HISCURVECHART,Name,Parent,Owner);
                case LCElementType.REALCURVECHART:
                    return new CCurveChart(LCElementType.REALCURVECHART,Name,Parent,Owner);
                case LCElementType.CHARGECURVECHART:
                    return new CCurveChart(LCElementType.CHARGECURVECHART,Name,Parent,Owner);
                case LCElementType.TABLECONTROL:
                    return new CTable(Name,Parent,Owner);
                //case LCElementType.PRG:
                //	return new CRectPRCG();*/
                default:
                    return null;
            }
        }
        public static ArrayList CreateALLElements()
        {
            ArrayList Objects = new ArrayList();
            Objects.Add(new CText());
            Objects.Add(new CLine());
            //Objects.Add(new CRectangle());
            //Objects.Add(new CRoundRectangle());
            //Objects.Add(new CEllips());
            //Objects.Add(new CPolyLine());
            //Objects.Add(new CPolygon());
            //Objects.Add(new CArc());
            return Objects;
        }
    }
}

