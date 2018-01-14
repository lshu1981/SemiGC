using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace LSSCADA
{
    //填充类型
    enum LCBrushType
    {
        Solid, Blank, Hatch, LinearGradient, Texture
    }
    public delegate void FillOptionChangeEventHandler();
    class LinearGradientStyle
    {
    //public:
	    public Color StartingColor;
	    public Color EndingColor;
	    public float Angle;
	    public bool IsAngleScaleable;
        public LinearGradientMode Mode;
        public int iMode;

	    public void SaveToXML(XmlElement Node)
	    {
		    XmlElement StypeNode = Node.OwnerDocument.CreateElement("LinearGradientStyle");

		    String strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", StartingColor.ToArgb());
		    StypeNode.SetAttribute("StartingColor", strTemp);

		    strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", EndingColor.ToArgb());
		    StypeNode.SetAttribute("EndingColor", strTemp);
		
		    strTemp = String.Format(CultureInfo.CurrentCulture, "{0:F}", Angle);
		    StypeNode.SetAttribute("Angle", strTemp);

		    strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", IsAngleScaleable);
		    StypeNode.SetAttribute("IsAngleScaleable", strTemp);

		    strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", (int)Mode);
		    StypeNode.SetAttribute("Mode", strTemp);

		    Node.AppendChild(StypeNode);
	    }

	    public void LoadFromXML(XmlElement Node)
	    {

            //return;
            XmlElement StypeNode = (XmlElement)Node.SelectSingleNode("Appearance");

            String strTemp = StypeNode.GetAttribute("tpenColor");
		    int nValue = Int32.Parse(strTemp);
            StartingColor = ColorTranslator.FromWin32(nValue);

            strTemp = StypeNode.GetAttribute("tbruColor");
		    nValue = Int32.Parse(strTemp);
            EndingColor = ColorTranslator.FromWin32(nValue);
				
		    //strTemp = StypeNode.GetAttribute("Angle");
		    //Angle = float.Parse(strTemp);		
		
		    //strTemp = StypeNode.GetAttribute("IsAngleScaleable");
		    //IsAngleScaleable =(Int32.Parse(strTemp)!=0);
		
		    //strTemp = StypeNode.GetAttribute("Mode");
		    //Mode = (LinearGradientMode)Int32.Parse(strTemp);
	    }

	    public void CopyTo(LinearGradientStyle Dest)
	    {
		    Dest.StartingColor = this.StartingColor;
		    Dest.EndingColor = this.EndingColor;
		    Dest.Angle = this.Angle;
		    Dest.IsAngleScaleable = this.IsAngleScaleable;
		    Dest.Mode = this.Mode;		
	    }

	    public LinearGradientStyle()
	    {
		    StartingColor = System.Drawing.Color.Red;
		    EndingColor = System.Drawing.Color.Blue;
		    Angle = 0;
		    IsAngleScaleable = false;
		    Mode = LinearGradientMode.Horizontal;
	    }
    }
    [System.Security.Permissions.PermissionSetAttribute
    (System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute
    (System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]
    class LinearGradientEditor:System.Drawing.Design.UITypeEditor
    {
	    LinearGradientEditor(){}

	    // Indicates whether the UITypeEditor provides a form-based (modal) dialog,
	    // drop down dialog, or no UI outside of the properties window.
	    public override UITypeEditorEditStyle GetEditStyle( System.ComponentModel.ITypeDescriptorContext context )
	    {
		    return System.Drawing.Design.UITypeEditorEditStyle.Modal;
	    }

	    // Displays the UI for value selection.
	    public override Object EditValue( System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, Object value )
	    {
		    MessageBox.Show("LinearGradientEditor!", "waring", MessageBoxButtons.OK, MessageBoxIcon.Information);
		    //LinearGradientStyle;
		    return value;
	    }
    }
    //[Description("填充设置"), TypeConverter(CFillOptionsConvert.StandardValuesCollection[0])]
    class CFillOptions
    {
    //private:
	    LCBrushType m_BrushType = LCBrushType.Blank;                    //画刷类型
	    Color m_BackColor;							//背景色
	    Color m_ForeColor;							//前景色
	    HatchStyle m_HatchStyle;					//网格填充类型
	    LinearGradientStyle m_LinearGradient;		//渐变填充类型
	    String TextureFile;						//图案文件
	    bool m_NoFrame;								//是否无边框
    //public:
	    [Browsable(true), Description("画刷类型"), Category("Appearance"), DisplayName("画刷")]
	    public LCBrushType BrushType
	    {
		    get{return m_BrushType;}

		    set{
			    m_BrushType = value;			
			    if (OnFillOptionChange != null)
				    OnFillOptionChange();
		    }
	    }

	    [Browsable(true), Description("背景色"), Category("Appearance"), DisplayName("背景色")]
	    public Color BackColor
	    {
		    get{return m_BackColor;}

		    set
		    {
			    m_BackColor = value;
                if (OnFillOptionChange != null)
				    OnFillOptionChange();
		    }
	    }

	    [Browsable(true), Description("前景色"), Category("Appearance"), DisplayName("前景色")]
	    public Color ForeColor
	    {
		    get{return m_ForeColor;}

		    set
		    {
			    m_ForeColor = value;
                if (OnFillOptionChange != null)
				    OnFillOptionChange();
		    }
	    }

	    [Browsable(true), Description("有无边框"), Category("Appearance"), DisplayName("无边框")]
	    public bool NoFrame
	    {
		    get{return m_NoFrame;}

		    set 
		    {
			    m_NoFrame = value;
                if (OnFillOptionChange != null)
				    OnFillOptionChange();
		    }
	    }

	    [Browsable(true), Description("网格类型"), Category("Appearance"), DisplayName("网格")]
	    public HatchStyle Hatch
	    {		
		    get{return m_HatchStyle;}

		    set
		    {
			    m_HatchStyle = value;
                if (OnFillOptionChange != null)
				    OnFillOptionChange();
		    }
	    }

	    [Browsable(true), Description("渐变填充类型"), Category("Appearance"), DisplayName("渐变填充")]
	    //[EditorAttribute(LinearGradientEditor.typeid,System.Drawing.Design.UITypeEditor.typeid)]
	    public LinearGradientStyle LinearGradient
	    {
		    get{return m_LinearGradient;}

		    set 
		    {
			    value.CopyTo(m_LinearGradient);
                if (OnFillOptionChange != null)
				    OnFillOptionChange();
		    }
	    }

	    [Browsable(true), Description("填充图案"), Category("Appearance"), DisplayName("图案")]
	    //[EditorAttribute(ImageEditor.typeid,System.Drawing.Design.UITypeEditor.typeid)]
	    public String Texture
	    {
		    get{return TextureFile;}

		    set
		    {
			    String _ImageFile = Application.StartupPath + "\\Image\\" + value;
			    if (File.Exists(_ImageFile))
			    {
				    TextureFile = value;
                    if (OnFillOptionChange != null)
					    OnFillOptionChange();
			    }
		    }
	    }

    //public:

	    public void SaveToXML(XmlElement Node)
	    {		
		    XmlElement OptNode = Node.OwnerDocument.CreateElement("CFillOptions");

		    String strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", (int)m_BrushType);
		    OptNode.SetAttribute("BrushType", strTemp);

		    strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", m_BackColor.ToArgb());
		    OptNode.SetAttribute("BackColor", strTemp);

		    strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", m_ForeColor.ToArgb());
		    OptNode.SetAttribute("ForeColor", strTemp);

		    strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", (int)m_HatchStyle);
		    OptNode.SetAttribute("HatchStyle", strTemp);

		    //m_LinearGradient.SaveToXML(OptNode);

		    //OptNode.SetAttribute("TextureFile", TextureFile);

		    //strTemp = String.Format(CultureInfo.CurrentCulture, "{0}", m_NoFrame);
		    //OptNode.SetAttribute("NoFrame", strTemp);

		    //Node.AppendChild(OptNode);
	    }

	    public void LoadFromXML(XmlElement Node)
	    {
            XmlElement OptNode = (XmlElement)Node.SelectSingleNode("Appearance");

            if (OptNode.HasAttribute("iShadeFillStyle"))
            {
                String strTemp = OptNode.GetAttribute("iShadeFillStyle");
                Int32 nValue = Int32.Parse(strTemp);

                if (nValue > -1)//ShadeFillStyle>-1时为渐变填充，此时tbruStyle无效
                {
                    m_BrushType = LCBrushType.LinearGradient;
                    m_LinearGradient.iMode = nValue;
                    m_LinearGradient.Mode = (LinearGradientMode)(nValue <= 3 ? nValue : 3);
                }
                else
                {
                    strTemp = OptNode.GetAttribute("tbruStyle");
                    nValue = Int32.Parse(strTemp);
                    if (nValue > 1)
                    {
                        m_BrushType = LCBrushType.Hatch;
                        m_HatchStyle = (HatchStyle)(Int32.Parse(strTemp)-2);
                    }
                    else
                        m_BrushType = (LCBrushType)Int32.Parse(strTemp);
                }


                strTemp = OptNode.GetAttribute("tbruColor");
                nValue = Int32.Parse(strTemp);
                m_BackColor = ColorTranslator.FromWin32(nValue);

                strTemp = OptNode.GetAttribute("FillColor");
                nValue = Int32.Parse(strTemp);
                m_ForeColor = ColorTranslator.FromWin32(nValue);

                m_LinearGradient.LoadFromXML(Node);
            }
		    //TextureFile = OptNode.GetAttribute("TextureFile");

		    //strTemp = OptNode.GetAttribute("NoFrame");
		    //m_NoFrame = (Int32.Parse(strTemp)!=0);		
	    }

	    public FillOptionChangeEventHandler OnFillOptionChange;
	
	    public CFillOptions Clone()
	    {
		    CFillOptions FillOptions = new CFillOptions();
		    FillOptions.BackColor = this.BackColor;
		    FillOptions.BrushType = this.BrushType;		
		    FillOptions.ForeColor = this.ForeColor;
		    FillOptions.Hatch = this.Hatch;
		    FillOptions.LinearGradient = this.LinearGradient;
		    FillOptions.TextureFile = this.TextureFile;
		    FillOptions.NoFrame = this.NoFrame;
		    return FillOptions;
	    }

	    public void CopyTo(CFillOptions Dest)
	    {
		    Dest.BackColor = this.BackColor;
		    Dest.BrushType = this.BrushType;		
		    Dest.ForeColor = this.ForeColor;
		    Dest.Hatch = this.Hatch;
		    Dest.LinearGradient = this.LinearGradient;
		    Dest.TextureFile = this.TextureFile;
		    Dest.NoFrame = this.NoFrame;
	    }

	    public CFillOptions()
	    {
		    m_BrushType = LCBrushType.Blank;
		    m_BackColor = Color.White;
		    m_ForeColor = Color.Black;
		    m_HatchStyle = HatchStyle.Horizontal;
		    TextureFile = "defaulttexture.png";
		    m_LinearGradient = new LinearGradientStyle();
		    NoFrame = false;
	    }
    }

    class CFillOptionsConvert : ExpandableObjectConverter
    {
    //public:
	    public  override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
	    {
            ExpandableObjectConverter sEx = new ExpandableObjectConverter();
		    if (destinationType.Equals(Type.GetType("DMS.DMS_Graphics.CFillOptions")))
			    return true;
            return sEx.CanConvertTo(context, destinationType);
	    }

        public override Object ConvertTo(ITypeDescriptorContext context, 
									    CultureInfo culture, 
									    Object value, 
									    Type destinationType)
	    {
		    if (destinationType.Equals(Type.GetType("System.String"))
				    && value.GetType().Equals(Type.GetType("DMS.DMS_Graphics.CFillOptions")))
		    {
			    CFillOptions FillOptions = (CFillOptions)  (value);
			    String Desstr = "填充类型:";
			    switch (FillOptions.BrushType)
			    {
			    case LCBrushType.Blank : Desstr += "空心"; break;
			    case LCBrushType.Solid : Desstr += "实填充"; break;
			    case LCBrushType.Hatch : Desstr += "网格填充"; break;
			    case LCBrushType.LinearGradient : Desstr += "渐变填充"; break;
			    case LCBrushType.Texture : Desstr += "图案填充"; break;
			    default : Desstr += "未知"; break;
			    }
			    //Desstr += ";填充色:" + FillOptions.BackColor.ToString();
			    return Desstr;
		    }
            ExpandableObjectConverter sEx = new ExpandableObjectConverter();
		    return sEx.ConvertTo(context, culture, value, destinationType);
	    }
    }
    

    //图元基类
    class CTuYuan : CBase
    {
    //protected:
        public bool m_isAddWidth;									//判断图元是否已经加宽
        public RectangleF Copy_m_Rect;			//m_Rect的拷贝	
        public CFillOptions m_FillOptions;					    //填充设置
        public SolidBrush m_SolidBrush;							//实填充画刷
        public HatchBrush m_HatchBrush;							//网格填充画刷
        public LinearGradientBrush m_LinearGradientBrush;			//渐变填充画刷
        public PathGradientBrush m_PathGradientBrush;           
        public TextureBrush m_TextureBrush;						//图案填充画刷
        public double m_TextureBrushW;
        public double m_TextureBrushH;
    //protected:	
        public RectangleF CartoonRect;
        public SolidBrush CartoonBrush;
	    public Color CartoonFillColor;
	    public void Rotate(Matrix MatrixValue){}					//开始旋转
        public void ResetRotate(Graphics g){}				//旋转恢复		

        public string StaName;                          //关联子站名称
        public string VarName;                          //关联变量名称
	    void OnFillOptionChange(){}						//改变填充设置

	    [Browsable(false)]
	    public bool AddWidth
	    {
		    get
		    {
			    return m_isAddWidth;
		    }
		    set
		    {
			    if (value == m_isAddWidth)
				    return ;
			    if (value)
				    this.LineWidth += 5;
			    else
				    this.LineWidth -= 5;
			    m_isAddWidth = value;
		    }
	    }
	    [Browsable(false)]
	    public override Brush DrawBrush							//画刷
	    {
		    get
		    {			
			    //base.DrawBrush.get();

			    switch (m_FillOptions.BrushType)
			    {
			    case LCBrushType.Blank : return m_SolidBrush;
			    case LCBrushType.Solid : 
                    {
                        m_SolidBrush.Color = m_FillOptions.BackColor;
                        return m_SolidBrush;
                    }
                case LCBrushType.Hatch:
                    {
                        m_HatchBrush = new HatchBrush(FillOptions.Hatch, FillOptions.BackColor, FillOptions.ForeColor);
                        return m_HatchBrush;
                    }
			    case LCBrushType.LinearGradient : 
				    {
					    RectangleF FillRect = this.GetPointsRect();
					    if (FillRect.IsEmpty)
						    FillRect.Size =new SizeF(1, 1);
					    //if (m_LinearGradientBrush != null)
						//    delete m_LinearGradientBrush;

                        int iAngle = 0;
                        Single ifocus = 0;
                        bool iES = false;
                        if (FillOptions.LinearGradient.iMode < 20)
                        {
                            switch (FillOptions.LinearGradient.iMode)
                            {
                                case 0: iAngle = 180; ifocus = 0; break;
                                case 1: iAngle = 0; ifocus = 0; break;
                                case 2: iAngle = 270; ifocus = 0; break;
                                case 3: iAngle = 90; ifocus = 0; break;
                                case 4: iAngle = 180; ifocus = 0.5f; iES = true; break;
                                case 5: iAngle = 0; ifocus = 0.5f; break;
                                case 6: iAngle = 270; ifocus = 0.5f; iES = true; break;
                                case 7: iAngle = 90; ifocus = 0.5f; break;
                                case 8: iAngle = 225; ifocus = 0; break;
                                case 9: iAngle = 315; ifocus = 0; break;
                                case 10: iAngle = 45; ifocus = 0; break;
                                case 11: iAngle = 135; ifocus = 0; break;
                                case 12: iAngle = 135; ifocus = 0.5f; break;
                                case 13: iAngle = 315; ifocus = 0.5f; iES = true; break;
                                case 14: iAngle = 225; ifocus = 0; break;
                                case 15: iAngle = 45; ifocus = 0; break;
                                case 16: iAngle = 315; ifocus = 0; break;
                                case 17: iAngle = 135; ifocus = 0; break;
                                case 18: iAngle = 45; ifocus = 0.5f; break;
                                case 19: iAngle = 225; ifocus = 0.5f; iES = true; break;
                                default: iAngle = 0; ifocus = 0; break;
                            }
                            if (iES)
                                m_LinearGradientBrush = new LinearGradientBrush(FillRect
                                    , FillOptions.LinearGradient.EndingColor
                                , FillOptions.LinearGradient.StartingColor
                                , iAngle
                                , true);
                            else
                                m_LinearGradientBrush = new LinearGradientBrush(FillRect
                                , FillOptions.LinearGradient.StartingColor
                                , FillOptions.LinearGradient.EndingColor
                                , iAngle
                                , true);
                            if (ifocus > 0)
                                m_LinearGradientBrush.SetBlendTriangularShape(ifocus);

                            return m_LinearGradientBrush;
                        }
                        else
                        {
                            GraphicsPath path = new GraphicsPath();
                            path.AddRectangle(FillRect);
                            m_PathGradientBrush = new PathGradientBrush(path);
                            //m_PathGradientBrush.CenterColor = FillOptions.LinearGradient.StartingColor;
                            Color[] colors;
                            if (FillOptions.LinearGradient.iMode % 2 == 1)
                                colors = new Color[] { FillOptions.LinearGradient.StartingColor, FillOptions.LinearGradient.EndingColor };
                            else
                                colors = new Color[] { FillOptions.LinearGradient.EndingColor, FillOptions.LinearGradient.StartingColor };
                            m_PathGradientBrush.SurroundColors = colors;
                            float[] positions = {0.0f,1.0f};
                            //定义ColorBlend对象
                            ColorBlend clrBlend = new ColorBlend(2);
                            clrBlend.Colors = colors;
                            clrBlend.Positions = positions;
                            m_PathGradientBrush.InterpolationColors = clrBlend;
                            //m_PathGradientBrush.SetBlendTriangularShape(0.5f);
                            // m_PathGradientBrush.WrapMode = (WrapMode)(FillOptions.LinearGradient.iMode - 20);
                            return m_PathGradientBrush;
                        }
                        
					    
				    }
			    case LCBrushType.Texture :
			    { 
				    RectangleF FillRect = this.GetPointsRect();
				    double w = m_TextureBrushW;
				    double h = m_TextureBrushH;
                    Matrix MatrixValue = new Matrix();
				    MatrixValue.Reset();
				    MatrixValue.Scale((float)(FillRect.Width/w),(float)(FillRect.Height/h),MatrixOrder.Append);
				    MatrixValue.Translate(FillRect.Left,FillRect.Top,MatrixOrder.Append);
				    //Transform(MatrixValue);
				    m_TextureBrush.ResetTransform();
				    m_TextureBrush.MultiplyTransform(MatrixValue);
				    return m_TextureBrush;
			    }
			    default : return m_SolidBrush;
			    }
		    }
	    }

	    [Browsable(true),Description("旋转角度"), Category("Layout"), DisplayName("旋转角度")]
	    public float RotateAngle
	    {
		    get
		    {
			    return m_RotateAngle;
		    }

		    set
		    {
			    //if (this.IsLocked())
				//    return;

			    if (value < 0)
				    m_RotateAngle = 0;
			    else if (value > 360)
				    m_RotateAngle = 360;
			    else
				    m_RotateAngle = value;
		    }
	    }

	    [Browsable(true),Description("线宽"), Category("Appearance"), DisplayName("线宽")]
	    public float LineWidth
	    {
		    get
		    {
			    return DrawPen.Width;
		    }

		    set
		    {
			    if (value < 1)
				    value = 1;
			    else if (value > 20)
				    value = 20;
			    DrawPen.Width = value;

			    if (this.ChildCount > 0)
				    foreach(CTuYuan Child in this.Children)
					    Child.LineWidth = value;
		    }
	    }

	    [Browsable(true),Description("线型"), Category("Appearance"), DisplayName("线型")]
	    public DashStyle LineStyle
	    {
		    get
		    {
			    return DrawPen.DashStyle;
		    }

		    set 
		    {
			    DrawPen.DashStyle = value;

			    if (this.ChildCount > 0)
				    foreach(CTuYuan Child in this.Children)
					    Child.LineStyle = value;
		    }
	    }

	    [Browsable(true), ReadOnly(true), Description("填充设置"), Category("Appearance"), DisplayName("填充")]
        public virtual CFillOptions FillOptions
	    {
		    get
		    {
			    return m_FillOptions;
		    }

		    set
		    {
			    m_FillOptions = value;	

			    if ( this.ChildCount > 0 )
			    {
				    foreach( CTuYuan Child in this.Children )
				    {
					    m_FillOptions.CopyTo(Child.FillOptions);
					    Child.FillOptions.OnFillOptionChange();
				    }
			    }
		    }
	    }
	    //private: array<PointF> GetAllPoints();
	    public virtual void OnMoveFocus( CFocus Focus,System.Drawing.SizeF Offset){}
	    //设置元素处于焦点状态.
	    //public virtual void SetFocused(bool value)
	    //{
		//    base.SetFocused(value);
	    //}
	    public override bool Selected(PointF SelectPoint) 
	    {
	        FIsSeleced = GetPointsRect().Contains(SelectPoint);
		    return FIsSeleced;
	    }
        /*public override bool Selected(Region RegionValue)
	    {
		    Matrix RotateMatrix = new Matrix();
		    PointF P = GetCenterPointF();
		    RotateMatrix.Translate(-P.X,-P.Y,MatrixOrder.Append);
		    RotateMatrix.Rotate(-m_RotateAngle,MatrixOrder.Append);
		    RotateMatrix.Translate(P.X,P.Y,MatrixOrder.Append);
		    RegionValue.Transform(RotateMatrix);
		    RectangleF PRect = GetPointsRect();
		    if (PRect.Height == 0)
			    PRect.Size =new SizeF(PRect.Width,1);
		    if (PRect.Width == 0)
			    PRect.Size =new SizeF(1,PRect.Height);
		    Region tempRegion = RegionValue.Clone();
		    bool IsVisible = tempRegion.IsVisible(PRect);
		    if (IsVisible)
		    {
			    RotateMatrix.Reset();
			    RectangleF[] childrect =  tempRegion.GetRegionScans(RotateMatrix);
			    bool find = false;
			    for (int i = 0 ; i < childrect.Length ; i++)
				    if (!PRect.Contains(childrect[i]))
					    find = true;
			    IsVisible = find;
		    }
		    if (m_Parent != null)
			    return IsVisible;
		    FIsSeleced = IsVisible;
		    return  FIsSeleced;
	    }*/
	    //public virtual void SetLastPoint(PointF PValue) 
	    //{
		//    base.SetLastPoint(PValue);
	    //}
	    //public override void AddPoint(PointF PValue)
	    //{
		//    base.AddPoint(PValue);
	    //}
	    //public virtual bool GetDrawing()
	    //{
		//    return base.GetDrawing();
	    //}
	    /*
	    virtual bool GetSelected() override
		    {
			    return CBase.GetSelected();
		    }*/
	    public override void DrawPoints(Graphics g)
	    {
		    base.DrawPoints(g);
	    }

        public override void Draw(Graphics g, Single iS)
        {
            base.Draw(g, iS);
        }
	    //public virtual void Move(PointF PValue){}
	    //public virtual void MoveTo(PointF PValue){}
	    //public virtual CBase CopyTo(CBaseDesObject){}
	    //public virtual void SaveToXML(XmlElement Node){}
	    public override void LoadFromXML(XmlElement Node)
        {
            base.LoadFromXML(Node);
            
            m_FillOptions.LoadFromXML(Node);
	        OnFillOptionChange();
        }
        //public virtual int ComputeFocusPoint() { }
	    //public virtual void UpDate(){}
	    public CTuYuan():base()
	    {
		    m_RotateAngle = 0;
		    Copy_Matrix = new Matrix();

		    m_FillOptions = new CFillOptions();
           // m_FillOptions.OnFillOptionChange = new FillOptionChangeEventHandler(this,OnFillOptionChange);

		    m_SolidBrush = (SolidBrush ) (Brushes.Transparent.Clone());
		    CartoonBrush = (SolidBrush ) (Brushes.Transparent.Clone());
		    m_HatchBrush = new HatchBrush(FillOptions.Hatch, FillOptions.ForeColor, FillOptions.BackColor);
	    }
	    public CTuYuan(String _Name, CBase _Parent, Object _Owner) : base(_Name, _Parent, _Owner)//,m_isAddWidth(false)
	    {
		    m_RotateAngle = 0;
		    Copy_Matrix = new Matrix();

		    m_FillOptions = new CFillOptions();
		    //m_FillOptions.OnFillOptionChange = new FillOptionChangeEventHandler(this, CTuYuan.OnFillOptionChange);

		    m_SolidBrush = (SolidBrush) (Brushes.Transparent.Clone());
		    CartoonBrush = (SolidBrush) (Brushes.Transparent.Clone());
		    m_HatchBrush = new HatchBrush(FillOptions.Hatch, FillOptions.ForeColor, FillOptions.BackColor);
	    }	
    }
}
