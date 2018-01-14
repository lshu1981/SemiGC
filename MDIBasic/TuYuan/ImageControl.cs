using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Xml;

namespace LSSCADA
{
    [Description("图片设置"), TypeConverter(typeof(CFillOptionsConvert))]
    public struct ImageInfo
    {
        public Object Clone()
        {
            ImageInfo temp = new ImageInfo();
            temp.m_iName = this.m_iName;
            temp.m_iType = this.m_iType;
            temp.m_iSwitchExp = this.m_iSwitchExp;
            return temp;
        }
        [Browsable(true), Description("图片名"), Category("Appearance"), DisplayName("图片名")]
        public String ImgName
        {
            get
            {
                return m_iName;
            }
            set
            {
                this.m_iName = value;
            }
        }
        [Browsable(true), Description("图片类型"), Category("Appearance"), DisplayName("图片类型")]
        public String ImgType
        {
            get
            {
                return m_iType;
            }
            set
            {
                this.m_iType = value;
            }
        }
        public String m_iName;
        public String m_iType;
        public String m_iSwitchExp;
        public override String ToString()
        {
            return m_iName;
        }
    }

    public struct ImageControlInfo
    {
        public Object Clone()
        {
            ImageControlInfo temp = new ImageControlInfo();
            temp.m_DefaultImg = (ImageInfo)(this.m_DefaultImg.Clone());
            temp.m_ImgList = (ArrayList)(this.m_ImgList.Clone());
            return temp;
        }
        public ImageInfo m_DefaultImg;
        public ArrayList m_ImgList;
    }

    public class ImgControlEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public Object EditValue(ITypeDescriptorContext context, Object value)
        {
            OpenFileDialog openImgFileDialog = new OpenFileDialog();
            openImgFileDialog.Filter = "所有图片文件(*.bmp;*.png;*.jpg;*.ico;*.gif)|*.bmp;*.png;*.jpg;*.ico;*.gif"
                + "|BMP文件(*.bmp)|*.bmp"
                + "|JPG文件(*.jpg)|*.jpg"
                + "|PNG文件(*.png)|*.png"
                + "|ICO文件(*.ico)|*.ico"
                + "|GIF文件(*.gif)|*.gif";
            openImgFileDialog.AddExtension = true;
            openImgFileDialog.CheckFileExists = true;
            openImgFileDialog.CheckPathExists = true;
            openImgFileDialog.Title = "选择图片";
            return openImgFileDialog;
        }
    }
    public class SwitchImage : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public  Object EditValue(ITypeDescriptorContext context, Object value)
        {
            return null;
        }
    }

    public class ImageControl : CBase
    {

        [Browsable(true), Description("旋转角度"), Category("Layout"), DisplayName("旋转角度")]
        public float RotateAngle
        {
            get
            {
                return m_RotateAngle;
            }

            set
            {
                //if (this.IsLocked())
                //	return;

                if (value < 0)
                    m_RotateAngle = 0;
                else if (value > 360)
                    m_RotateAngle = 360;
                else
                    m_RotateAngle = value;
            }
        }

        [Browsable(true), Description("选择图像"), Category("Design"), DisplayName("选择图像")]
        [EditorAttribute(typeof(ImgControlEditor), typeof(UITypeEditor))]
        public Object ImageChoose
        {
            get
            {
                return this.m_ImageInfo.m_DefaultImg.m_iName;
            }

            set
            {
                OpenFileDialog openImgFileDialog = (OpenFileDialog)value;
                if (System.Windows.Forms.DialogResult.OK == openImgFileDialog.ShowDialog())
                {
                    String[] strlist = openImgFileDialog.SafeFileName.Split('.');

                    int i = 0;
                    for (i = 0; i < this.m_ImageInfo.m_ImgList.Count; i++)
                    {
                        if (strlist[0].Equals(((ImageInfo)this.m_ImageInfo.m_ImgList[i]).m_iName))
                        {
                            break;
                        }
                    }
                    if (i < this.m_ImageInfo.m_ImgList.Count)
                    {
                        MessageBox.Show("图像条件切换中已存在同名的图像，请重新选择！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //set(value);
                        return;
                    }
                    Stream stream = openImgFileDialog.OpenFile();
                    this.m_imge = Image.FromStream(stream);

                    this.m_ImageInfo.m_DefaultImg.m_iName = strlist[0];
                    this.m_ImageInfo.m_DefaultImg.m_iType = strlist[1];
                    this.currentlyAnimating = false;
                }
            }
        }

        [Browsable(true), Description("切换"), Category("Behavior"), DisplayName("切换")]
        [EditorAttribute(typeof(SwitchImage), typeof(UITypeEditor))]
        public Object SwitchImage
        {
            get
            {
                if (m_ImageInfo.m_ImgList.Count == 0)
                    return "(无)";
                else
                    return "(有)";
            }

            set
            {
                SwitchImage = value;
            }
        }
        [Browsable(false)]
        public Image IMAGE
        {
            set
            {
                this.m_imge = value;
            }
        }
        public ImageControl()
        {
            this.m_ElementType = LCElementType.IMAGECONTROL;
            m_RotateAngle = 0;
            this.m_Height = 50;
            this.m_Width = 50;
            m_ImageInfo = new ImageControlInfo();
            m_ImageInfo.m_DefaultImg = new ImageInfo();
            m_ImageInfo.m_DefaultImg.m_iName = "defImg";
            m_ImageInfo.m_DefaultImg.m_iType = "png";
            m_ImageInfo.m_ImgList = new ArrayList();
            m_gifActive = false;
            currentlyAnimating = false;
            giftime = new Timer();
            giftime.Stop();
            //giftime.Tick += new EventHandler(this,ImageControl.GifTimerProc);
            giftime.Interval = 80;
            LastUpdateIndex = -1;
        }
        public ImageControl(String Name, CBase Parent, Object Owner)
        {
            this.m_ElementType = LCElementType.IMAGECONTROL;
            m_RotateAngle = 0;
            this.m_Height = 50;
            this.m_Width = 50;
            m_ImageInfo = new ImageControlInfo();
            m_ImageInfo.m_DefaultImg = new ImageInfo();
            m_ImageInfo.m_DefaultImg.m_iName = "defImg";
            m_ImageInfo.m_DefaultImg.m_iType = "png";
            m_ImageInfo.m_ImgList = new ArrayList();
            m_gifActive = false;
            currentlyAnimating = false;
            giftime = new Timer();
            giftime.Stop();
            //giftime.Tick += new EventHandler(this,&ImageControl.GifTimerProc);
            giftime.Interval = 80;
            LastUpdateIndex = -1;
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
        public override void Transform(Matrix MatrixValue)
        {
            Rotate(MatrixValue);
            base.Transform(MatrixValue);
        }
        //virtual bool Selected(Region SelectPoint) override;	
        //virtual bool Selected(PointF SelectPoint) override;				
        //public override void OnMoveFocus( CFocus Focus,System.Drawing.SizeF Offset) ;
        //public override void SaveToXML(XmlElement Node) ;
        public override void LoadFromXML(XmlElement Node)
        {
            base.LoadFromXML(Node);
            XmlElement imgNode = (XmlElement)(Node.SelectSingleNode("Behavior/SwitchPics"));
            if (imgNode == null)
                return;
            this.m_ImageInfo.m_DefaultImg.m_iName = imgNode.Attributes["PicPath"].Value.ToString();
            foreach(XmlNode cnode in imgNode.ChildNodes)
			{
				ImageInfo iif = new ImageInfo();
                iif.m_iName = cnode.Attributes["PicName"].Value.ToString();
                iif.m_iSwitchExp = cnode.Attributes["Condition"].Value.ToString();
				this.m_ImageInfo.m_ImgList.Add(iif);
			}
        }
        //public override void SetLastPoint(PointF PValue) ;
        //public override void AddPoint(PointF PValue) ;
        //public override CBase Clone() ;
        //public override CBase CopyTo(CBase DesObject) ;
        public override void DrawPoints(Graphics g)
        {
            base.DrawPoints(g);
            if (m_imge == null)
            {
                String ImgFile = CProject.sPrjPath + "\\Project\\ImageDirectory\\" + this.m_ImageInfo.m_DefaultImg.ImgName;
                if (File.Exists(ImgFile))
                    m_imge = Image.FromFile(ImgFile, true);
                ///else
                //m_imge = GetImgFromDatabase(this.m_ImageInfo.m_DefaultImg.ImgName, this.m_ImageInfo.m_DefaultImg.ImgType);

                if (m_imge == null)
                    m_imge = Image.FromFile(CProject.sPrjPath + "\\Image\\defaulttexture.png", true);
            }
            
            RectangleF rect =new RectangleF(new PointF( iOrgX1,iOrgY1),new SizeF(iOrgX2,iOrgY2));
            g.DrawImage(m_imge, rect);
           // if (FIsFocused || FIsSeleced)
            //    DrawFocus(g);
        }
        //public override void UpDate() ;			
        //public void GifTimerProc(Object,EventArgs);
        //public Byte[] GetBytesFromImg(Image img);
        //public Image GetImgFromBytes(Byte[] bytes);
        //public Image GetImgFromDatabase(String Name, String Type);
        //public void OnPaintGif(System.Object ,System.EventArgs );

        public float m_Width;
        public float m_Height;
        public Image m_imge;
        public ImageControlInfo m_ImageInfo;
        public Timer giftime;
        public bool m_gifActive;
        public bool currentlyAnimating;
        public int LastUpdateIndex;
    }
}
