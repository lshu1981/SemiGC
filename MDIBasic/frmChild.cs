using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace LSSCADA
{
    public partial class frmChild : Form
    {
        public CForm cForm = new CForm();
        public string sFilePath = "";
        public GraphicsPath gPath1 = new GraphicsPath();
        public List<CBase> ListTuYuan = new List<CBase>();				//所有元素集合
        List<CNumUpDown> ListNumericUpDown = new List<CNumUpDown>();
        List<CRing> ListRing = new List<CRing>();

        public frmChild()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public frmChild(string sName, Form _Owner, int iTop)
        {
            InitializeComponent();
            cForm.Name = sName;
            cForm.sPath = CProject.sPrjPath + "\\Project\\Wins\\" + sName + ".xml";
            this.MdiParent = _Owner;
            LoadWins();
            cForm.iTop = iTop;
            cForm.Draw(this);
            this.DoubleBuffered = true;
        }

        public void LoadWins()//读取Wins文件
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            myxmldoc.Load(cForm.sPath);

            string xpath = "FormWindow";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            cForm.LoadFromXML(childNode);

            //取图元
            xpath = "FormWindow/Misc";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            int i = 0;
            foreach (XmlElement item in childNode.ChildNodes)
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
                            case "TYZLine":
                                CElementFactory.SetClassIndex(LCElementType.POLYLINE);
                                break;
                            case "TYArc":
                                CElementFactory.SetClassIndex(LCElementType.ARC);
                                break;
                            case "KJHotImage":
                                CElementFactory.SetClassIndex(LCElementType.IMAGECONTROL);
                                break;
                            case "KJIcon":
                                CElementFactory.SetClassIndex(LCElementType.GROUP);
                                break;
                            case "TYRing":
                                CElementFactory.SetClassIndex(LCElementType.Ring);
                                break;
                            case "TYPipe":
                                CElementFactory.SetClassIndex(LCElementType.Pipe);
                                break;
                            default:
                                continue;
                            //break;
                        }

                        CBase NewOb = CElementFactory.CreateElement(null, this);
                        if (NewOb == null)
                            continue;
                        NewOb.LoadFromXML(TYNode);

                        if (NewOb.ElementType == LCElementType.Pipe)
                        {
                            CPipe CNum = (CPipe)NewOb;
                            if (CNum.bShowSet)
                            {
                                NumericUpDown nNumUD = new NumericUpDown();
                                CNumUpDown cNumUD = new CNumUpDown(CNum.ValPF, CNum.SetWidth, 20, 13);
                                nNumUD.Font = new Font("宋体", 13, GraphicsUnit.World);
                                nNumUD.Location = CNum.ValPF;
                                nNumUD.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
                                nNumUD.Name = CNum.Name;
                                nNumUD.Size = new Size((int)CNum.SetWidth, 20);

                                nNumUD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left))));

                                nNumUD.UpDownAlign = LeftRightAlignment.Left;
                                nNumUD.TextAlign = HorizontalAlignment.Right;
                                CVar nVar = frmMain.staComm.GetVarByStaNameVarName("NJ301", nNumUD.Name);
                                if (nVar != null)
                                {
                                    nNumUD.Value = new decimal(new int[] { (int)nVar.PLCValue[2], 0, 0, 0 });
                                }
                                else
                                {
                                    nNumUD.Value = new decimal(new int[] { 0, 0, 0, 0 });
                                }
                                nNumUD.ValueChanged += new EventHandler(nCon_ValueChanged);
                                cNumUD.nNumUD = nNumUD;
                                ListNumericUpDown.Add(cNumUD);
                                this.Controls.Add(nNumUD);
                            }
                        }

                        for (i = ListTuYuan.Count - 1; i > -1; i--)
                        {
                            CBase Ob = (CBase)ListTuYuan[i];

                            Int32 iEO = Ob.iElementOrder;
                            if (NewOb.iElementOrder >= iEO)
                            {
                                ListTuYuan.Insert(i + 1, NewOb);
                                break;
                            }
                        }
                        if (i == -1)
                            ListTuYuan.Insert(0, NewOb);
                    }
                }//if ((sNodeName.Substring(0, 2) == "TY
            }//foreach (XmlElement item in childNode.ChildNodes)

            //对应图元
            foreach (CBase nTY in ListTuYuan)
            {
                if (nTY.ElementType == LCElementType.GROUP && nTY.KJIconType == 2)
                {
                    foreach (KJIcon nIcon in KJIconList.ListKJIcon)
                    {
                        if (nIcon.IconName == nTY.IconName)
                        {
                            ((CGroup)nTY).BaseKJIcon = nIcon;
                        }
                    }
                    foreach (SwitchPic nSP in ((CGroup)nTY).ListSwitchPic)
                    {
                        foreach (KJIcon nIcon in KJIconList.ListKJIcon)
                        {
                            if (nIcon.IconName == nSP.PicName)
                            {
                                ((SwitchPic)nSP).BaseKJIcon = nIcon;
                            }
                        }
                    }
                }
            }

        }
        public void DrawForms(Graphics dc)//画元件
        {
            DateTime startTime = DateTime.Now;
            for (int i = 0; i < ListTuYuan.Count; i++)
            {
                Object COb = ListTuYuan[i];
                ((CBase)COb).Draw(dc);
            }
            DateTime endTime = DateTime.Now;

            //Debug.WriteLine(":" + ((TimeSpan)(endTime - startTime)).TotalMilliseconds);
        }

        private void frmChild_MouseMove(object sender, MouseEventArgs e)//鼠标移动
        {
            PointF MouseP = new PointF((e.X - frmMain.iLeftD) / frmMain.iWinFoucs, (e.Y - frmMain.iTopD) / frmMain.iWinFoucs);

            for (int i = ListTuYuan.Count - 1; i > -1; i--)
            {
                CBase obj = (CBase)ListTuYuan[i];

                if (obj.ElementType == LCElementType.Ring)
                {
                    if (obj.Selected(MouseP))
                    {
                        this.Cursor = Cursors.Hand;
                        return;
                    }
                }
            }
            this.Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                e.Graphics.TranslateTransform(frmMain.iLeftD, frmMain.iTopD);
                e.Graphics.ScaleTransform(frmMain.iWinFoucs, frmMain.iWinFoucs);//扩大2倍
                DrawForms(e.Graphics);
                base.OnPaint(e);
                e.Graphics.ResetTransform();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("frmChild.OnPaint" + ex.Message);
            }
        }

        private void NumUDScale(Single iFoucs)//微调框缩放
        {
            foreach (CNumUpDown nNum in ListNumericUpDown)
            {
                nNum.nNumUD.Location = new Point((int)(nNum.x * iFoucs + frmMain.iLeftD), (int)(nNum.y * iFoucs + frmMain.iTopD));
                nNum.nNumUD.Size = new Size((int)(nNum.w * iFoucs), (int)(iFoucs * nNum.h));
                nNum.nNumUD.Font = new Font("宋体", nNum.Size * iFoucs, GraphicsUnit.World);
            }
        }

        private void frmChild_MouseDown(object sender, MouseEventArgs e)//鼠标点击
        {
            int i = 0;
            PointF MouseP = new PointF((e.X - frmMain.iLeftD) / frmMain.iWinFoucs, (e.Y - frmMain.iTopD) / frmMain.iWinFoucs);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                for (i = ListTuYuan.Count - 1; i > -1; i--)
                {
                    CBase obj = (CBase)ListTuYuan[i];
                    if (obj.ElementType == LCElementType.Ring)
                    {
                        if (obj.Selected(MouseP))
                        {
                            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");
                            CProtcolFINS SPLC = (CProtcolFINS)Sta;
                            if (SPLC.System[0] == 7)
                                return;

                            CRing nRing = (CRing)obj;

                            string sVar = "AV" + nRing.ShowText.PadLeft(3, '0');
                            SPLC.SendAODO(sVar, 0, "DO");
                            break;
                        }
                    }
                }
            }
        }

        public void nCon_ValueChanged(object sender, EventArgs e)//微调框数值改变
        {
            NumericUpDown nCon = (NumericUpDown)sender;
            string sVar = nCon.Name;
            int iValue = (int)nCon.Value;
            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");
            CProtcolFINS SPLC = (CProtcolFINS)Sta;
            SPLC.SendAODO(sVar, iValue, "AO");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (object obj in this.Controls)
                {
                    if (obj.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        NumericUpDown nNumUD = (NumericUpDown)obj;
                        CVar nVar = frmMain.staComm.GetVarByStaNameVarName("NJ301", nNumUD.Name);

                        if (nVar != null)
                        {
                            nNumUD.ValueChanged -= new EventHandler(nCon_ValueChanged);
                            nNumUD.Value = new decimal(new int[] { Math.Min((int)nNumUD.Maximum, (int)nVar.PLCValue[2]), 0, 0, 0 });
                            nNumUD.ValueChanged += new EventHandler(nCon_ValueChanged);
                        }
                    }
                }
                this.Invalidate();
                //Rectangle rc = new Rectangle(new Point(158, 13), new Size(203, 93));
                //this.Invalidate(rc,true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("frmChild.timer1_Tick" + ex.Message);
            }
        }

        private void frmChild_Resize(object sender, EventArgs e)
        {
            Size aa = this.MdiParent.Size;
            this.Size = new System.Drawing.Size(aa.Width - 20, aa.Height - frmMain.iToolHeight);
            NumUDScale(frmMain.iWinFoucs);
            label1.Text = "";// frmMain.iWinFoucs.ToString();
        }

    }
}
