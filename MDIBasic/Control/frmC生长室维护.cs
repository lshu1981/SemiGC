using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace LSSCADA.Control
{
    public partial class frmC生长室维护 : Form
    {
        public CForm cForm = new CForm();
        public string sFilePath = "";
        public List<CBase> ListTuYuan = new List<CBase>();
        List<CNumUpDown> ListNumericUpDown = new List<CNumUpDown>();
        CProtcolFINS SPLC;

        public frmC生长室维护()
        {
            InitializeComponent();
        }

        public frmC生长室维护(Form _Owner, int iTop)
        {
            InitializeComponent();
            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");
            SPLC = (CProtcolFINS)Sta;

            this.MdiParent = _Owner;
            LoadWins();
            LoadFromPLC();
            button1.Visible = false;
            this.Top = (int)cForm.m_Location.Y + iTop;

            this.DoubleBuffered = true;
        }

        private void LoadFromPLC()
        {
            num1032.Value = Math.Min(num1032.Maximum, SPLC.System[31]);//生长室吹扫次数
            num1033.Value = Math.Min(num1033.Maximum, SPLC.System[32]);//生长室吹扫时间s
        }

        public void LoadWins()//读取Wins文件
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            cForm.sPath = CProject.sPrjPath + "\\Project\\Wins\\生长室维护.xml";
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
        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.ScaleTransform(0.5f, 0.5f);//扩大2倍
            e.Graphics.TranslateTransform(frmMain.iLeftD, 0);
            e.Graphics.ScaleTransform(frmMain.iWinFoucs, frmMain.iWinFoucs);//扩大2倍
            DrawForms(e.Graphics);
            base.OnPaint(e);
            e.Graphics.ResetTransform();

            //e.Graphics.ResetTransform();
        }
        public void nCon_ValueChanged(object sender, EventArgs e)//微调框数值改变
        {
            int i = 0;
            NumericUpDown nCon = (NumericUpDown)sender;
            string aaa = nCon.Name;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SPLC == null)
                return;
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
            if (SPLC.System[0] != 10 && SPLC.System[0] != 9)
            {
                textBox1.Text = "";
                button2.Enabled = false;
                button2.BackColor = Color.LightGray;

                button002.Enabled = true;
                button002.BackColor = Color.Coral;
                button013.Enabled = button002.Enabled;
                button013.BackColor = button002.BackColor;
                button014.Enabled = button002.Enabled;
                button014.BackColor = button002.BackColor;
                button015.Enabled = button002.Enabled;
                button015.BackColor = button002.BackColor;
                return;
            }
            button2.Enabled = true;
            button2.BackColor = Color.Red;

            button002.Enabled = false;
            button002.BackColor = Color.LightGray;
            button013.Enabled = button002.Enabled;
            button013.BackColor = button002.BackColor;
            button014.Enabled = button002.Enabled;
            button014.BackColor = button002.BackColor;
            button015.Enabled = button002.Enabled;
            button015.BackColor = button002.BackColor;
            int iReturn = SPLC.System[19];
            textBox2.Text = SPLC.System[33].ToString();

            if (iReturn > 0)
            {
                string sDesc = iReturn.ToString() + ":";
                if (frmMain.staComm.nReminder.ReminderList.ContainsKey(iReturn))
                {
                    Reminder nRem = frmMain.staComm.nReminder.ReminderList[iReturn];
                    sDesc += nRem.Desc;

                    if (nRem.Index % 10 == 1)
                    {
                        button3.Visible = false;
                    }
                    else
                    {
                        button3.Visible = true;

                    }
                }
                textBox1.Text = sDesc;
            }
            else
            {
                textBox1.Text = "";
                button3.Visible = false;
            }
            this.Invalidate();
        }

        private void buttonSend1002(object sender, EventArgs e)
        {
            Button nCon = (Button)sender;
            gBSet.Visible = false;
            switch (nCon.Name)
            {
                case "button013":
                    if (bMsgBoxShowYN("是否执行 生长室真空检漏 程序？", "真空检漏"))
                    {
                        SPLC.SendAODO("1002", 13, "SY");//13：生长室真空检漏
                    }
                    break;
                case "button014":
                    if (bMsgBoxShowYN("是否执行 生长室充气 程序？", "充气"))
                    {
                        SPLC.SendAODO("1002", 14, "SY");//14：生长室充气
                    }
                    break;
                case "button015":
                    if (bMsgBoxShowYN("是否执行 生长室泵吹扫 程序？", "泵吹扫"))
                    {
                        SPLC.SendAODO("1002", 15, "SY");//15：生长室泵吹扫
                        gBSet.Visible = true;
                    }
                    break;
                case "button002":
                    if (bMsgBoxShowYN("是否执行 N2 Idle 程序？", "N2 Idl"))
                    {
                        SPLC.SendAODO("1002", 2, "SY");//2:N2 Idle
                    }
                    break;
            }
        }

        private bool bMsgBoxShowYN(string strText, string strCaption)
        {
            if (MessageBox.Show(strText, strCaption,
             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)//确认
        {
            SPLC.SendAODO("1020", 0, "SY");//System[19]	1020	R	Idle返回码
            button3.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)//停止
        {
            SPLC.SendAODO("1002", 0, "SY");//停止
            SPLC.SendAODO("1020", 0, "SY");//System[19]	1020	R	Idle返回码
            gBSet.Visible = false;
        }

        private void num1032_ValueChanged(object sender, EventArgs e)
        {
            button1.Visible = true;
        }

        private void num1033_ValueChanged(object sender, EventArgs e)
        {
            button1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)//下发设定参数
        {
            try
            {
                SPLC.SendAODO("1032", (int)num1032.Value, "SY");//1032	W	生长室吹扫次数设定
                SPLC.SendAODO("1033", (int)num1033.Value, "SY");//1033	W	生长室吹扫时间设定
                SPLC.SendAODO("1020", 0, "SY");//System[19]	1020	R	Idle返回码
                button1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("下发生长室吹扫参数失败:" + ex.Message);
            }
        }

        private void frmC生长室维护_Resize(object sender, EventArgs e)
        {
            Size aa = this.MdiParent.Size;
            this.Size = new System.Drawing.Size(aa.Width - 20, aa.Height - frmMain.iToolHeight);
            NumUDScale(frmMain.iWinFoucs);
        }

        private void NumUDScale(Single iFoucs)//微调框缩放
        {
            foreach (CNumUpDown nNum in ListNumericUpDown)
            {
                nNum.nNumUD.Location = new Point((int)(nNum.x * iFoucs + frmMain.iLeftD), (int)(nNum.y * iFoucs));
                nNum.nNumUD.Size = new Size((int)(nNum.w * iFoucs), (int)(iFoucs * nNum.h));
                nNum.nNumUD.Font = new Font("宋体", nNum.Size * iFoucs, GraphicsUnit.World);
            }
        }
    }
}
