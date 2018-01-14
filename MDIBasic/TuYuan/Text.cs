using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

namespace LSSCADA
{
    class CValueFormatTypeConverter : StringConverter
    {
//    public:
	    public override  bool GetStandardValuesSupported(ITypeDescriptorContext context) 
	    {
		    return true;
	    }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) 
	    {
		    return true;
	    }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) 
	    {
		    ArrayList TypeNames = new ArrayList();
		    TypeNames.Add("General");
		    TypeNames.Add("#00.0000");
		    TypeNames.Add("#00.000");
		    TypeNames.Add("#00.00");
		    TypeNames.Add("#00.0");
		    TypeNames.Add("#00");
		    TypeNames.Add("#0.00E+00");
		    return new TypeConverter.StandardValuesCollection(TypeNames);
	    }

	    CValueFormatTypeConverter()
	    {}
    }

    class CText : CTuYuan
    {
    //protected:
	    public String m_Text;						//文本内容
        public Font DrawFont;	//字体
        public SolidBrush FontBrush;//文本画刷
        //public SolidBrush
        public StringFormat TextFormat;			//文本格式
        public CExpression FExpression;            //显示方式
        public bool Updated;
        public float TextW;
        public float TextH;
        public StringAlignment m_HorizonAlign; //水平对齐
        public StringAlignment m_VerticalAlign;//垂直对齐
        public bool m_AutoFit;
        public bool m_AutoWrap;
        public bool m_Background;              //是否绘制背景, 绘制背景才能填充
        public bool IsBorder = false;                  //是否显示边框
        public Boolean bCloseWin = false;              //点击关闭窗口
        public string strIfName = "";               //快捷窗口
        public string strSta = "";                  //界面所在
        public Boolean IsExecProgram = false;       //是否执行程序
        public string Program = "";                 //程序
        public SizeF TextSize;
    //public:
	    [Browsable(true),Description("文本"), Category("Appearance"), DisplayName("文本")]
	    public String Text
	    {
		    get
		    {
			    return m_Text;
		    }

		    set
		    {
			    m_Text = value;			
			    GetTextSize();
		    }
	    }	
	    [Browsable(true),Description("显示方式"), Category("Appearance"), DisplayName("显示方式")]
	    //[TypeConverter(new CTileTypeConverter..get())]
	    public String ExpressType
	    {
		    get
		    {
			    switch (FExpression.ExpressType)
			    {
			    case LCExpressType.StaticText : return  "静态文本";
                case LCExpressType.RTVar: return "实时数据";
                case LCExpressType.Var: return "变量";
			    case LCExpressType.Expression : return  "表达式";
			    default: return "";
			    }
		    }
		    set
		    {
			    if (value == "静态文本")
				    FExpression.ExpressType = LCExpressType.StaticText;
                else if (value == "实时数据")
                    FExpression.ExpressType = LCExpressType.RTVar;
			    else if (value == "变量")
				    FExpression.ExpressType = LCExpressType.Var;
			    else if (value == "表达式")
				    FExpression.ExpressType = LCExpressType.Expression;
		    }
	    }

	    [Browsable(true),Description("浮点数数值格式"), Category("Appearance")]//, DisplayName("数值格式")]
	    //[TypeConverter(CValueFormatTypeConverter.typeid)]
	    String ValueFormat;
	
	    [Browsable(true),Description("显示内容"), Category("Appearance")]//, DisplayName("显示内容")]
	    //[EditorAttribute(ExprEditor.typeid, System.Drawing.Design.UITypeEditor.typeid)]
	    CExpression Expression
	    {
		    get
		    {
			    return FExpression;
		    }

		    set
		    {		
			    FExpression = value;
		    }
	    }	
	    [Browsable(true),Description("字体"), Category("Appearance"), DisplayName("字体")]
	    Font Font
	    {
		    get
		    {
			    return DrawFont;
		    }

		    set
		    {
			    DrawFont = (System.Drawing.Font)(value.Clone());
			    GetTextSize();			
		    }
	    }	
	    [Browsable(true),Description("字体颜色"), Category("Appearance"), DisplayName("字体颜色")]
	    Color FontColor
	    {
		    get
		    {
                return Color.Black;// FontBrush.Color;
		    }

		    set
		    {
			   // FontBrush.Color = value;
		    }
	    }
	    [Browsable(true),Description("是否绘制背景, 绘制背景才能填充"), Category("Appearance"), DisplayName("背景")]
	    bool Background
	    {
		    get
		    {
			    return m_Background;
		    }

		    set
		    {
			    m_Background = value;			
		    }
	    }

	    [Browsable(true),Description("是否将文本内容自动填满边框"), Category("Appearance"), DisplayName("自适应")]
	    bool AutoFit
	    {
		    get
		    {
			    return m_AutoFit;
		    }

		    set
		    {
			    m_AutoFit = value;

			    if (m_AutoFit)
			    {
				    TextFormat.FormatFlags = TextFormat.FormatFlags | StringFormatFlags.NoWrap;
				    TextFormat.FormatFlags = TextFormat.FormatFlags & (StringFormatFlags.LineLimit);
				    TextFormat.FormatFlags = TextFormat.FormatFlags | StringFormatFlags.NoClip;
				
				    TextFormat.Alignment = StringAlignment.Near;
				    TextFormat.LineAlignment = StringAlignment.Near;
			    }
			    else
			    {
				    if (m_AutoWrap)
					    TextFormat.FormatFlags = TextFormat.FormatFlags & (~StringFormatFlags.NoWrap);
				    else
					    TextFormat.FormatFlags = TextFormat.FormatFlags | StringFormatFlags.NoWrap;
				    TextFormat.FormatFlags = TextFormat.FormatFlags | StringFormatFlags.LineLimit;
				    TextFormat.FormatFlags = TextFormat.FormatFlags & (~StringFormatFlags.NoClip);

				    TextFormat.Alignment = HorizonAlign;
				    TextFormat.LineAlignment = VerticalAlign;				
			    }
		    }
	    }

	    [Browsable(true),Description("自动换行"), Category("Appearance"), DisplayName("自动换行")]
	    bool AutoWrap
	    {
		    get
		    {
			    return m_AutoWrap;
		    }

		    set
		    {
			    if (m_AutoFit)
				    return;

			    m_AutoWrap = value;	

			    if (m_AutoWrap)
				    TextFormat.FormatFlags = TextFormat.FormatFlags & (~StringFormatFlags.NoWrap);
			    else
				    TextFormat.FormatFlags = TextFormat.FormatFlags | StringFormatFlags.NoWrap;
		    }
	    }

	    [Browsable(true),Description("水平对齐"), Category("Appearance"), DisplayName("水平对齐")]
	    StringAlignment HorizonAlign
	    {
		    get
		    {
			    return m_HorizonAlign;
		    }

		    set
		    {
			    m_HorizonAlign = value;			
			    TextFormat.Alignment = m_HorizonAlign;
		    }
	    }

	    [Browsable(true),Description("垂直对齐"), Category("Appearance"), DisplayName("垂直对齐")]
	    StringAlignment VerticalAlign
	    {
		    get
		    {
			    return m_VerticalAlign;
		    }

		    set 
		    {
			    m_VerticalAlign = value;			
			    TextFormat.LineAlignment = m_VerticalAlign;
		    }
	    }
        public void GetTextSize()
        {
            TextSize = TextRenderer.MeasureText(m_Text, DrawFont);
	        for (int i = 0;i < m_Text.Length;i++)
	        {
		        if (m_Text[i] > 0xFF)
			        TextSize.Width += 1;
	        }
        }
        //public override void Transform(Matrix MatrixValue)
        //{
        //    base.Transform(MatrixValue);
        //}
        //public virtual void AutoFitTransform(Matrix MatrixValue) { }
        //public virtual void AddPoint(PointF PValue) { }
        //public virtual void SetLastPoint(PointF PValue) { }
        public override bool Selected(PointF SelectPoint)
        {
            //if (!FisLive)
		    //    return false;
	        //SelectPoint = TransformPointF(SelectPoint);
	        if (base.Selected(SelectPoint))
		        return true;
	        RectangleF Rect = GetPointsRect();
	        return Rect.Contains(SelectPoint);
        }
        public override void DrawPoints(Graphics g)
        {
            base.DrawPoints(g);

	        RectangleF rect = GetTextRect(); 
	        
            //SolidBrush BackgroundBrush = this.DrawBrush;
            //BackgroundBrush.Color = m_SolidBrush;
			    myGraphicsPath.AddRectangle(rect);
		        myGraphicsPath.Transform(myPathMatrix);

		    if (FillOptions.BrushType != LCBrushType.Blank && !Background)
	        {
                g.FillPath(m_SolidBrush, myGraphicsPath);
		        //myGraphicsPath.Reset();
	        }
            if (IsBorder)
            {
                g.DrawPath(DrawPen, myGraphicsPath);
                
            }
           // myGraphicsPath.Reset();
	       // Single FontYSize = g.DpiY * DrawFont.SizeInPoints / 72;
	        //Single FontXSize = g.DpiX * DrawFont.SizeInPoints / 72;

	        //myGraphicsPath.Transform(myPathMatrix);
	        //g.FillPath(FontBrush, myGraphicsPath);
            //g.DrawPath(TextPen, myGraphicsPath);
            DateTime now = DateTime.Now;
            string str1 = FExpression.execStr();
            /*if (FExpression.ExpressType == LCExpressType.Expression)
            {//性能测试
                DateTime startTime = DateTime.Now;
                for (int k = 0; k < 100000; k++)
                {
                    str1 = FExpression.exec(TextFormat);
                }
                DateTime endTime = DateTime.Now;

                Debug.WriteLine(FExpression.Exipression +":"+ ((TimeSpan)(endTime - startTime)).TotalMilliseconds);
            }
            else
            {
                str1 = FExpression.exec(TextFormat);
            }*/
            StringFormat format1 = new StringFormat(StringFormatFlags.LineLimit);
            
            g.DrawString(str1, DrawFont, FontBrush, rect, TextFormat);
            //g.DrawString
        }

        private RectangleF GetTextRect()
        {
            return new RectangleF(iOrgX1, iOrgY1, TextW, TextH);
        }
        public virtual RectangleF GetViewRect()
        {
            //if (Updated)//运行版刷新。
	        //{
		        float fPenWidth = DrawPen.Width;
		        RectangleF rect;
		        rect = GetPointsRect();
		        float W = TextSize.Width;
		        float H = TextSize.Height;
		
		        rect.Size =new SizeF(rect.Width*W/TextW,rect.Height*H/TextH);
		        //rect = RotateTransformRectangleF(rect,GetCenterPointF(),m_RotateAngle);
		        rect.Offset(-fPenWidth/2,-fPenWidth/2);
		        //rect.Size += new SizeFConverterSizeF(fPenWidth, fPenWidth);
		        return rect;
	        //}
	        //else
	        //{
		    //    return base.GetViewRect();
	        //}
        }
        //public virtual void SaveToXML(XmlElement Node) { }
        public override void LoadFromXML(XmlElement Node)
        {
            base.LoadFromXML(Node);
            try
            {
                Points.Add(new PointF(iOrgX1, iOrgY1));
                Points.Add(new PointF(iOrgX2, iOrgY2));

                XmlElement TextNode = (XmlElement)(Node.SelectSingleNode("Appearance"));
                //绘制背景
                Background = Convert.ToBoolean(TextNode.GetAttribute("Transparent"));

                TextW = Convert.ToSingle(TextNode.GetAttribute("Width"));
                TextH = Convert.ToSingle(TextNode.GetAttribute("Height"));

                String strFontName = TextNode.GetAttribute("FontName");
                float fSize = Convert.ToSingle(TextNode.GetAttribute("FontSize"));
                FontStyle fStyle = (FontStyle)Enum.Parse(typeof(FontStyle), TextNode.GetAttribute("FontStyle"));

                DrawFont = new System.Drawing.Font(strFontName, fSize, fStyle);
                String strTemp = TextNode.GetAttribute("FontColor");
                FontBrush.Color = ColorTranslator.FromWin32(Int32.Parse(strTemp));
                
                int  ExpType =int.Parse( TextNode.GetAttribute("ViewStyle"));
                FExpression.ExpressType = (LCExpressType) ExpType;
                FExpression.Exipression = TextNode.GetAttribute("ShowVar");
                FExpression.IsShowUnit =Convert.ToBoolean( TextNode.GetAttribute("IsShowUnit"));
                FExpression.GetDeviceVar();

                if (TextNode.HasAttribute("TextOut_horizontal"))
                    HorizonAlign = (StringAlignment)Enum.Parse(typeof(StringAlignment), TextNode.GetAttribute("TextOut_horizontal"));
                if (TextNode.HasAttribute("TextOut_Vertical"))
                    VerticalAlign = (StringAlignment)Enum.Parse(typeof(StringAlignment), TextNode.GetAttribute("TextOut_Vertical"));

                Text = TextNode.GetAttribute("Text");
                FExpression.sText = Text;
                FExpression.GetDeviceVar();

                IsBorder = Convert.ToBoolean(TextNode.GetAttribute("IsBorder"));
                DrawPen.Color = ColorTranslator.FromWin32(Int32.Parse(TextNode.GetAttribute("BorderColor")));
                DrawPen.Width = Int32.Parse(TextNode.GetAttribute("BorderWidth"));
                DrawPen.DashStyle = DashStyle.Solid;
                FillOptions.BrushType = LCBrushType.Solid;
                m_SolidBrush.Color = ColorTranslator.FromWin32(Int32.Parse(TextNode.GetAttribute("FillColor")));
                strIfName = TextNode.GetAttribute("strIfName");
                bCloseWin = Convert.ToBoolean(TextNode.GetAttribute("bCloseWin"));
                strSta = TextNode.GetAttribute("strSta");

                TextNode = (XmlElement)(Node.SelectSingleNode("Behavior"));
                IsExecProgram = Convert.ToBoolean(TextNode.GetAttribute("IsExecProgram"));
                Program = TextNode.GetAttribute("Program");
            }
            catch (Exception e)
            {
            }
        }
        //public virtual void DoFlash() { }

        //String CText.GetToolTip() { }
        //public virtual void UpDate() { }
        //public virtual CBase Clone() { }
        //public virtual CBase CopyTo(CBase DesObject) { }
	    public CText():base()
	    {
		    m_ElementType = LCElementType.TEXT;		
		    m_Background = false;		
		    DrawFont = new Font("宋体", 16,GraphicsUnit.World);
		    Text =  "文本";
		    FontBrush = new SolidBrush(System.Drawing.Color.DeepSkyBlue);
            //FontBrush = SystemBrushes.ControlText;
		    FExpression = new CExpression();
		    Updated = false;
            TextFormat = new StringFormat(StringFormat.GenericDefault);
		    HorizonAlign = StringAlignment.Near;
		    VerticalAlign = StringAlignment.Center;
		    AutoFit = true;
		    ValueFormat = "General";
	    }
	    public CText(String _Name, CBase _Parent, Object _Owner) : base(_Name, _Parent, _Owner)
	    {
		    m_ElementType = LCElementType.TEXT;		
		    m_Background = false;		
		    DrawFont = new System.Drawing.Font("宋体", 16,GraphicsUnit.World);
		    Text ="文本";
		    FontBrush = new SolidBrush(System.Drawing.Color.DeepSkyBlue);
            //FontBrush = 
		    FExpression = new CExpression();
		    Updated = false;
            TextFormat = new StringFormat(StringFormat.GenericDefault);
		    HorizonAlign = StringAlignment.Near;
		    VerticalAlign = StringAlignment.Center;
		    AutoFit = true;
		    AutoWrap = false;
		    ValueFormat = "General";
	    }
    }
}
