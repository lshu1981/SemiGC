using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Globalization;

namespace LSSCADA
{
    class CBackgroundConvert { }
    //背景图
    [Description("背景图"), TypeConverter(typeof(CBackgroundConvert))]
    public class CBackground
    {
        public String m_ImageFile;

        [Browsable(true), Description("方式"), Category("Appearance")]//, DisplayName("方式")]
        public ImageLayout Layout;

        [Browsable(true), Description("图像文件"), Category("Appearance")]//, DisplayName("图像")]
        [EditorAttribute(typeof(ImageEditor), typeof(UITypeEditor))]
        public String ImageFile
        {
            get
            {
                return m_ImageFile;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    m_ImageFile = null;
                    BackImage = null;
                    return;
                }
                String _ImageFile = Application.StartupPath + "\\Image\\" + value;
                if (!File.Exists(_ImageFile))
                    _ImageFile = Application.StartupPath + "\\Image\\" + m_ImageFile;
                else
                {
                    m_ImageFile = value;
                    BackImage = (Bitmap)(Image.FromFile(_ImageFile, true));
                }
            }
        }

        [Browsable(false)]
        public Bitmap BackImage;
        //图片转array<Byte>
        public Byte[] GetBytesFromImg(Image img)
        {
            System.IO.MemoryStream tempStream = new System.IO.MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(tempStream, img);
            tempStream.Close();
            return tempStream.ToArray();
        }
        //array<Byte>转图片
        public Image GetImgFromBytes(Byte[] bytes)
        {
            System.IO.MemoryStream tempStream = new System.IO.MemoryStream(bytes, 0, bytes.Length);
            Image img = null;
            try
            {
                img = Image.FromStream(tempStream);
            }
            catch
            {
                BinaryFormatter bf = new BinaryFormatter();
                img = (Image)bf.Deserialize(tempStream);
            }
            tempStream.Close();
            return img;
        }
        public void SaveToXML(XmlElement Node)
        {
            if (!String.IsNullOrEmpty(m_ImageFile))
            {
                //将背景图片存入数据库
                String[] imgNames = m_ImageFile.Split('.');
                if (imgNames.Length == 2)
                {
                    String AddBkimg2Db = "insert into FileInfo "
                            + "values ('',@name,@type,@file)";
                    //IDBAccess DBAccess = DBAccessFactory.CreateInstance("");
                    String strImg = "select Name "
                                        + "from FileInfo "
                                        + "where Name = '" + imgNames[0] + "'"
                                        + " and Type = '" + imgNames[1] + "'";
                }
            }

            XmlElement CBackgroundNode = Node.OwnerDocument.CreateElement("CBackground");
            CBackgroundNode.SetAttribute("ImageFile", m_ImageFile);
            CBackgroundNode.SetAttribute("Layout", Layout.ToString());
            Node.AppendChild(CBackgroundNode);
        }

        public void LoadFromXML(XmlElement Node)
        {
            String strTemp = Node.GetAttribute("BackImagePath");
            String strpath = CProject.sPrjPath + "\\ImageDirectory\\" + strTemp;
            if (!File.Exists(strpath) && !String.IsNullOrEmpty(strTemp))
            {
                String[] imgNames = strTemp.Split('.');
            }
            if (File.Exists(strpath))
            {
                m_ImageFile = strTemp;
                BackImage = (Bitmap)(Image.FromFile(strpath, true));
            }

            strTemp = Node.GetAttribute("m_iBgImageType");
            Layout = (ImageLayout)Enum.Parse(typeof(ImageLayout), strTemp);
        }

        public CBackground Clone()
        {
            CBackground clone = new CBackground();
            clone.m_ImageFile = m_ImageFile != null ? (String)m_ImageFile.Clone() : null;
            clone.Layout = Layout;

            if (m_ImageFile != null && File.Exists(m_ImageFile))
            {
                BackImage = (Bitmap)(Image.FromFile(m_ImageFile, true));
            }

            return clone;
        }

        public CBackground()
        {
            ImageFile = "defaultbackground.png";
        }
    }
    /*
 class CBackgroundConvert :  ExpandableObjectConverter
{
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
	{
		if (destinationType.Equals(Type.GetType("DMS.DMS_Graphics.CBackground")))
			return true;
		return ExpandableObjectConverter.CanConvertTo(context, destinationType);
	}

	virtual Object ConvertTo(ITypeDescriptorContext context, 
									CultureInfo culture, 
									Object value, 
									Type destinationType) override
	{
		if (destinationType.Equals(Type.GetType("System.String"))
				&& value.GetType().Equals(Type.GetType("DMS.DMS_Graphics.CBackground")))
		{
			CBackground Background = dynamic_cast <CBackground > (value);
			String Desstr;
			switch (Background.Layout)
			{
			case ImageLayout.None : Desstr = "无"; break;
			case ImageLayout.Center : Desstr = "居中"; break;
			case ImageLayout.Tile : Desstr = "平铺"; break;
			case ImageLayout.Stretch : Desstr = "拉伸"; break;
			case ImageLayout.Zoom : Desstr = "放大"; break;
			default : Desstr = "未知"; break;
			}
			return Desstr;
		}
		return ExpandableObjectConverter.ConvertTo(context, culture, value, destinationType);
	}
};*/
    //窗口
    public class CForm : CBase
    {
        //frmChild formChild1;
        public int iTop = 0;
        public string sPath;
        public CBackground m_Background;
        public Bitmap TileBackMap;
        Size m_AreaSize;
        public double scale;
        public Size OrgScrollSize;
        public PointF Position;
        public Color BackColor;
        [Browsable(true), Description("刷新周期(ms)"), Category("Behavior")]//, DisplayName("刷新周期")]
        int RefreshPeriod;

        [Browsable(true), Description("窗口宽度"), Category("Layout")]//, DisplayName("窗口宽度")]
        float Width;

        [Browsable(true), Description("窗口高度"), Category("Layout")]//, DisplayName("窗口高度")]
        float Height;

        [Browsable(true), Description("边框风格"), Category("Layout")]//, DisplayName("边框风格")]
        FormBorderStyle BorderStyle;

        [Browsable(true), Description("是否置顶"), Category("Layout")]//, DisplayName("是否置顶")]
        bool TopMost;

        [Browsable(true), Description("可缩放"), Category("Behavior")]//, DisplayName("可缩放")]
        bool Scaleable;

        [Browsable(true), Description("可拖动"), Category("Behavior")]//, DisplayName("可拖动")]
        bool Dragable;

        [Browsable(true), Description("是否在运行系统中单击右键出现右键菜单"), Category("Behavior")]//, DisplayName("右键菜单使能")]
        bool PopMenuEnabled;

        [Browsable(true), ReadOnly(true), Description("背景图设置"), Category("Appearance")]//, DisplayName("背景图")]
        CBackground Background
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

        [Browsable(false), Description("网格可见"), Category("Appearance")]//, DisplayName("网格可见")]
        public static bool GridVisible;

        [Browsable(false), Description("网格大小"), Category("Appearance")]//, DisplayName("网格大小")]
        public static int GridSize;

        public override void DrawPoints(Graphics g)
        {
            base.DrawPoints(g);
        }
        //public override void SaveToXML(XmlElement Node)
        //{ }
        public override void LoadFromXML(XmlElement Node)
        {
            base.LoadFromXML(Node);
            XmlElement CFormNode = (XmlElement)(Node.SelectSingleNode("Misc"));
            m_Background.LoadFromXML(CFormNode);
            RefreshPeriod = Convert.ToInt32(CFormNode.GetAttribute("RefreshInterval"));
            //FOnLoad.LoadFromXML(CFormNode);

            CFormNode = (XmlElement)(Node.SelectSingleNode("Appearance"));
            String strTemp = CFormNode.GetAttribute("Color");
            BackColor = ColorTranslator.FromWin32(Convert.ToInt32(strTemp));
            this.Color = BackColor;

            CFormNode = (XmlElement)(Node.SelectSingleNode("Layout"));
            if (CFormNode.HasAttribute("Left"))
                m_Location.X = Convert.ToSingle(CFormNode.GetAttribute("Left"));
            if (CFormNode.HasAttribute("Top"))
                m_Location.Y = Convert.ToSingle(CFormNode.GetAttribute("Top"));
            Width = Convert.ToSingle(CFormNode.GetAttribute("Width"));
            Height = Convert.ToSingle(CFormNode.GetAttribute("Height"));
            int W = Convert.ToInt32(CFormNode.GetAttribute("HorzRange"));
            int H = Convert.ToInt32(CFormNode.GetAttribute("VertRange"));
            AreaSize = new Size(W, H);

            CFormNode = (XmlElement)(Node.SelectSingleNode("Behavior"));
            strTemp = CFormNode.GetAttribute("FormType");	//窗口类型0子窗口，1，对话框，2无边框
            BorderStyle = (FormBorderStyle)Enum.Parse(typeof(FormBorderStyle), (2 - Convert.ToInt32(strTemp)).ToString());
            //BorderStyle = (FormBorderStyle)Enum.Parse(typeof(FormBorderStyle),strTemp);
            //strTemp = CFormNode.GetAttribute("TopMost");
            //TopMost = Convert.ToBoolean(CFormNode.GetAttribute("TopMost"));//
            Scaleable = Convert.ToBoolean(CFormNode.GetAttribute("IsCanZoom"));//缩放
            Dragable = Convert.ToBoolean(CFormNode.GetAttribute("IsCanDrag"));//拖动
            //if (CFormNode.HasAttribute("PopMenuEnabled"))
            //    PopMenuEnabled = Convert.ToBoolean(CFormNode.GetAttribute("PopMenuEnabled"));
            //GridVisible = Convert.ToBoolean(CFormNode.GetAttribute("GridVisible"));
            // GridSize = Convert.ToInt32( CFormNode.GetAttribute("GridSize"));


        }

        //*public override bool Selected(Region RegionValue)
        //{
        //    return false;
        //}

        //public override void SetLastPoint(PointF PValue)
        //{
        //}

        //public override bool GetDrawing()
        //{
        //    return false;
        //}

        //public override bool GetSelected()
        //{
        //    return false;
        //}

        //[Browsable(false)]
        //public String LayerName;

        //[Browsable(false)]
        //public bool Enabled;

        public override bool Selected(PointF SelectPoint)
        {
            return false;
        }

        [Browsable(true), Description("绘图区域大小"), Category("Layout")]//, DisplayName("区域大小")]
        public Size AreaSize
        {
            get { return m_AreaSize; }
            set
            {
                m_AreaSize = value;
            }
        }

        [Browsable(false)]
        public SizeF Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }


        public PointF Location
        {
            get
            {
                return m_Location;
            }
            set
            {
                m_Location = value;
            }
        }

        public float X
        {
            get
            {
                return m_Location.X;
            }
            set
            {
                m_Location.X = value;
            }
        }

        public float Y
        {
            get
            {
                return m_Location.Y;
            }

            set
            {
                m_Location.Y = value;
            }
        }
        public static void InitializeTransform(Graphics g, float scale, System.Drawing.Point ScrollPosition)
        {
            return;
            g.ResetTransform();
            g.TranslateTransform(ScrollPosition.X, ScrollPosition.Y);
            g.ScaleTransform(scale, scale);
        }
        public void Draw(object OBj)//, Bitmap b, RectangleF InvalidateRect)
        {
            if (OBj == null)
                return;

            Form OwnerForm = (Form)OBj;
            //绘制背景图
            //OwnerForm.MdiParent.ClientSize = AreaSize;
            //Size ss = OwnerForm.MdiParent.ClientSize;

            RectangleF DrawRect = new RectangleF(0, 0, AreaSize.Width, AreaSize.Height);
            //DrawRect = OwnerForm.GraphicToForm(DrawRect);
            OwnerForm.FormBorderStyle = BorderStyle;
            OwnerForm.StartPosition = FormStartPosition.Manual;
            OwnerForm.Left = (int)m_Location.X;
            OwnerForm.Top = (int)m_Location.Y + iTop;
            OwnerForm.Size = AreaSize;
            OwnerForm.BackColor = this.Color;
            OwnerForm.BackgroundImage = Background.BackImage;
            OwnerForm.BackgroundImageLayout = Background.Layout;
            //绘制边界线	
            //DrawG.DrawRectangle(Pens.Gray, System.Drawing.Rectangle(0, 0, this.Rect.Width, this.Rect.Height));
        }

        public CForm()
        {
            GridVisible = false;
            GridSize = 8;

            m_ElementType = LCElementType.FORM;

            m_Location = new PointF(0, 0);
            RefreshPeriod = 1000;
            m_Background = new CBackground();
            //this.Width = ((Form)Owner).Width;
            //this.Height = ((Form)Owner).Height;
            this.BorderStyle = FormBorderStyle.Sizable;
            this.Dragable = true;
            this.PopMenuEnabled = true;
            this.TopMost = false;
            this.Scaleable = true;
            DrawPen.Color = System.Drawing.Color.Black;
            //this.ScrollVisible = false;
            scale = 1;
            Screen[] screens = Screen.AllScreens;
            System.Drawing.Rectangle g = screens[0].Bounds;
            OrgScrollSize.Width = g.Width;
            OrgScrollSize.Height = g.Height;
        }

        public CForm(String _Name, Object _Owner)
            : base(_Name, null, _Owner)
        {
            m_ElementType = LCElementType.FORM;

            m_Location = new PointF(0, 0);
            RefreshPeriod = 1000;
            m_Background = new CBackground();
            this.Width = ((Form)Owner).Width;
            this.Height = ((Form)Owner).Height;
            this.BorderStyle = FormBorderStyle.Sizable;
            this.Dragable = true;
            this.PopMenuEnabled = true;
            this.TopMost = false;
            this.Scaleable = true;
            DrawPen.Color = System.Drawing.Color.Black;
            //this.ScrollVisible = false;
            scale = 1;
            Screen[] screens = Screen.AllScreens;
            System.Drawing.Rectangle g = screens[0].Bounds;
            OrgScrollSize.Width = g.Width;
            OrgScrollSize.Height = g.Height;
        }
    }
}
