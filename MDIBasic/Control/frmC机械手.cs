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
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace LSSCADA.Control
{
    public delegate void InvokeInsertText(string sCon);
    public delegate void InvokeUpdateTree();
    public partial class frmC机械手 : Form
    {
        List<CBase> ListTuYuan = new List<CBase>();	//所有元素集合
        public CForm cForm = new CForm();           //窗口实例

        bool bOpenChamber = true;
        bool bOpenSample = true;

        KJRuler CRuler = new KJRuler();             //高度尺
        Machining CMach = new Machining();          //机械手
        CProtcolTran CTran = new CProtcolTran();    //机械手通信
        CProtcolFINS SPLC;
        CVar DI074, DI075, DI076, DI110, DI111;


        public Thread TPortThread = null;           //TCP通信线程

        public frmC机械手()
        {
            InitializeComponent();
        }

        public frmC机械手(Form _Owner,int iTop)
        {
            InitializeComponent();

            this.MdiParent = _Owner;
            cForm.iTop = iTop;
            LoadWins();
            this.Top =(int) cForm.m_Location.Y + cForm.iTop;

            CMach.MachByStation(40);
            this.DoubleBuffered = true;

            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");
            SPLC = (CProtcolFINS)Sta;
            DI074 = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI074");
            DI075 = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI075");
            DI076 = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI076");
            DI110 = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI110");
            DI111 = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI111");

            //启动端口数据收发线程
            CTran.Setting = CMach.Setting;
            CTran.fm = this;
            CTran.Open();
            TPortThread = new Thread(new ThreadStart(CTPortThread));
            TPortThread.Name = "TPortThread";
            TPortThread.IsBackground = true;
            TPortThread.Start();
            splitContainer2.Visible = false;
        }

        public void LoadWins()//读取窗口文件
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sWinsPath = CProject.sPrjPath + "\\Project\\Wins\\传送控制.xml";
            myxmldoc.Load(sWinsPath);

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
                            case "KJRuler":
                                CElementFactory.SetClassIndex(LCElementType.KJRuler);
                                break;
                            default:
                                continue;
                            //break;
                        }

                        CBase NewOb = CElementFactory.CreateElement(null, this);
                        if (NewOb == null)
                            continue;
                        NewOb.LoadFromXML(TYNode);
                        if (TYNode.Name == "KJRuler")
                            CRuler = (KJRuler)NewOb;
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
                else if (sNodeName == "Machinings")
                {
                    foreach (XmlElement TYNode in item.ChildNodes)
                    {
                        CMach.LoadFromXML(TYNode);
                    }
                }
            }//foreach (XmlElement item in childNode.ChildNodes)
        }

        public void DrawForms(Graphics g)//画图元控件
        {
            for (int i = 0; i < ListTuYuan.Count; i++)
            {
                CBase COb = ListTuYuan[i];
                //if (COb.ElementType == LCElementType.Pipe || COb.ElementType == LCElementType.Ring || COb.ElementType == LCElementType.TEXT || COb.ElementType == LCElementType.ELLIPS)
                {
                    COb.Draw(g);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.ScaleTransform(0.5f, 0.5f);//扩大2倍
            base.OnPaint(e);
            CMach.UpdateRTZ(CTran.RTZ);
            CRuler.sinValue = CTran.RTZ[2];
            textBoxPRT.Text = CTran.RTZ[0].ToString("0.000");
            textBoxPLT.Text = CTran.RTZ[1].ToString("0.000");
            textBoxPTT.Text = CTran.RTZ[2].ToString("0.000");


            //e.Graphics.TranslateTransform(frmMain.iLeftD, frmMain.iTopD+100);
            DrawForms(e.Graphics);
            //e.Graphics.ResetTransform();
            GetTuoPanWZ();
            /*string str12 = "";
            for (int k = 0; k <CMach.TuoPanNum; k++)
            {
                str12 += CMach.TuoPan [k].ToString() + ", ";
            }
            label4.Text +=  " ::: "  + str12;
            Debug.WriteLine(label4.Text);
            */
            CMach.DrawBI1(e.Graphics);
            CMach.eCommState = CTran.CommStateE;
            CMach.ActionStep = CTran.ActionStep;
            
                label1.BackColor = CTran.CommStateC;
                label1.Text = "机械手通信：" + CTran.CommStateS;
            
            DrawRing(e.Graphics);
            SetControlState();
            //e.Graphics.ResetTransform();
        }

        private void CTPortThread()//通信线程
        {
            while (true)
            {
                try
                {
                    if (!CTran.client.Connected)
                    {
                        CTran.bOpen = false;
                        CTran.CommStateE = ECommSatate.Unknown;
                        CTran.bOpen = CTran.ConnectServer();
                        if (!CTran.bOpen)
                        {
                            Thread.Sleep(100);            
                            continue;
                        }
                    }
                    if (CTran.netstream.CanRead)
                    {
                        byte[] buf = new byte[1024];
                        int iNum = 0;

                        StringBuilder sRecv = new StringBuilder();

                        // Incoming message may be larger than the buffer size.
                        do
                        {
                            iNum = CTran.netstream.Read(buf, 0, buf.Length);
                            sRecv.AppendFormat("{0}", Encoding.ASCII.GetString(buf, 0, iNum));
                        }
                        while (CTran.netstream.DataAvailable);

                        if (iNum > 0)
                        {
                            CTran.CommStateE = ECommSatate.Normal;
                            string str1 = sRecv.ToString();
                            //SetText(str1);
                            str1 = Regex.Replace(str1, "\n", "");
                            InsertText(str1);
                            string[] sValue = str1.Split('\r');
                            for (int i = 0; i < sValue.Length; i++)
                            {
                                if(sValue[i].Length>2)
                                    CTran . PortDataRecv(CTran.LastSendMsg, sValue[i]);
                            }
                            CTran.GetSendMsg();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("frmC机械手.CTPortThread" + e.Message);
                }
                Thread.Sleep(100);
            }
            //Debug.WriteLine("quit" );
        }

        public void InsertText(string sCon)
        {
            if (richTextBox1.InvokeRequired)  //控件是否跨线程？如果是，则执行括号里代码
            {
                InvokeInsertText setListCallback = new InvokeInsertText(InsertText);   //实例化委托对象
                richTextBox1.Invoke(setListCallback, sCon);   //重新调用SetListBox函数
            }
            else
            {
                if (sCon.IndexOf("RQ POS") >= 0 || sCon.IndexOf("POS ABS") >= 0)
                {
                    return;
                }

                if (PauseShowMsg.Checked)
                {
                    return;
                }
                if (richTextBox1.Lines.Length > 1000)
                {
                    richTextBox1.Select(0, richTextBox1.Lines[0].Length + 1);
                    richTextBox1.Clear();
                }
                sCon = DateTime.Now.ToLongTimeString() + " " + sCon;
                richTextBox1.AppendText (sCon);
            }
        }
        
        private void SetControlState()//设定控件状态
        {
            Color[] ConColor = new Color[20];
            for (int i = 0; i < ConColor.Length; i++)
            {
                ConColor[i] = System.Drawing.SystemColors.Control;
            }
            bool[] ConBool = new bool[20];
            for (int i = 0; i < ConBool.Length; i++)
            {
                ConBool[i] =true;
            }

            if (CTran.ActionStep > 0)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                return;
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
            }
            if (CMach. iStation % 10 == 0)//R在Home位置
            {
               ConColor[4] = Color.Red;//收回
               ConBool[4] = false;
            }
            else if (CMach.iStation % 10 == 1)
            {
                ConColor[3] = Color.Red;//伸出
                ConBool[3] = false;
            }

            int iST = CMach.iStation / 10;

            if (iST == 0)
            {
                ConBool[3] = false;
            }
            else
            {
                ConColor[5 + iST] = Color.Red;
                ConBool[5 + iST] = false;
            }

            ExtraMove01.BackColor = ConColor[1];//上
            ExtraMove02.BackColor = ConColor[2];//下
            ExtraMove03.BackColor = ConColor[3];//伸出
            ExtraMove04.BackColor = ConColor[4];//收回
            ExtraMove05.BackColor = ConColor[5];//安全
            ExtraMove06.BackColor = ConColor[6];//生长室
            ExtraMove07.BackColor = ConColor[7];//进样室
            ExtraMove08.BackColor = ConColor[8];//进样室
            ExtraMove09.BackColor = ConColor[9];//层流室
            ExtraMove10.BackColor = ConColor[10];//捡起
            ExtraMove11.BackColor = ConColor[11];//放置
            ExtraMove12.BackColor = ConColor[12];//传送
            
            ExtraMove01.Enabled = ConBool[1];//上
            ExtraMove02.Enabled = ConBool[2];//下
            ExtraMove03.Enabled = ConBool[3];//伸出
            ExtraMove04.Enabled = ConBool[4];//收回
            ExtraMove05.Enabled = ConBool[5];//安全
            ExtraMove06.Enabled = ConBool[6];//生长室
            ExtraMove07.Enabled = ConBool[7];//进样室
            ExtraMove08.Enabled = ConBool[8];//进样室
            ExtraMove09.Enabled = ConBool[9];//层流室
            ExtraMove10.Enabled = ConBool[10];//捡起
            ExtraMove11.Enabled = ConBool[11];//放置
            ExtraMove12.Enabled = ConBool[12];//传送
        }

        public void GetTuoPanWZ()//获取托盘位置
        {
            GetTuoPanNewWZ1();
            CMach.ROld = CTran.R;
            //Debug.WriteLine("iStation:" + iStation.ToString() + ";TuoPan:" + TuoPan[0].ToString());
        }

        public void GetTuoPanNewWZ1()//获取托盘位置
        {
            int[] iRe = GetDIValue();

            if (CTran.ActionStep <= 0 || CTran.ActionStep >= 3)//机械手静止状态时，直接检测位置和数量
            {
                CMach.iOutIn = 0;
                if (iRe.Length >= 0 && iRe.Length <= 2)
                {
                    CMach.TuoPanNum = iRe.Length;
                    for (int i = 0; i < iRe.Length; i++)
                    {
                        CMach.TuoPan[i] = iRe[i];
                    }
                }
                else
                {
                    CMach.TuoPanNum = 0;
                    CMach.TuoPan[0] = 10;
                }
                return;
            }

            string sActionCon = CTran.ActionCon[CTran.ActionStep];//获取机械手的动作信息
            bool bNull = false;//检测信息不全标识
            if (sActionCon == null)//没有动作信息
            {
                bNull = true;
            }
            string[] str1 = sActionCon.Split(' ');
            if (str1.Length < 2)//动作信息不全
            {
                bNull = true;
            }
            if (iRe.Length < CMach.TuoPanNum)//位置信息检测不全
            {
                bNull = true;
            }

            if (bNull)//检测信息不全标识，有检测信息的按检测信息，没有检测的位置信息按随托盘定
            {
                for (int i = 0; i < Math.Min(iRe.Length, CMach.TuoPanNum); i++)
                {
                    CMach.TuoPan[i] = iRe[i];
                }
                if (iRe.Length < CMach.TuoPanNum)
                {
                    for (int i = iRe.Length; i < CMach.TuoPanNum; i++)
                    {
                        CMach.TuoPan[i] = 10;
                    }
                }
                return;
            }

            for (int i = 0; i < CMach.TuoPanNum; i++)
            {
                int iReStr1 = -1;
                try
                {
                    if (str1.Length == 2)
                        iReStr1 = Convert.ToInt32(str1[1].Trim());
                    else if (str1.Length > 2)
                        iReStr1 = Convert.ToInt32(str1[2].Trim());
                }
                catch
                {
                }
                if (iRe[i] == iReStr1)
                {
                    switch (str1[0].Trim())
                    {
                        case "PICK"://捡起动作
                            if (CMach.ROld > CTran.R)//收回状态
                            {
                                CMach.iOutIn = 1;
                                if (CMach.TuoPan[i] < 10)//改变状态
                                {
                                    CMach.TuoPan[i] = CMach.TuoPan[i] * 10 + 1;
                                }
                            }
                            break;
                        case "PLACE"://放置动作
                            if (CMach.iOutIn == 2)
                                CMach.TuoPan[i] = iRe[i];
                            else
                            {
                                if (CMach.ROld > CTran.R)//收回状态,按实际检测状态， 伸出时不改变状态
                                {
                                    CMach.iOutIn = 2;
                                    CMach.TuoPan[i] = iRe[i];
                                }
                                else
                                {
                                    CMach.TuoPan[i] = iRe[i] * 10 + 1;
                                }
                            }
                            break;
                        default:
                            CMach.TuoPan[i] = iRe[i];
                            break;
                    }
                }
                else
                {
                    CMach.TuoPan[i] = iRe[i];
                }
            }
           
        }

        private int[] GetDIValue()
        {
            List<int> iRe = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                if (CMach.bDI[i])
                {
                    iRe.Add(i + 1);
                }
            }
            //return iRe.ToArray();
            for (int i = 4; i < 7; i++)
            {
                if (CMach.bDI[i])
                {
                    iRe.Add((i - 3) * 10 + 1);
                }
            }
            return iRe.ToArray();
        }

        private void DrawRing(Graphics g)//画阀门和进样室上下位
        {
            RectangleF rect1 = new RectangleF(CMach.PFCenter.X - 147, CMach.PFCenter.Y - 70, 14, 140);
            RectangleF rect2 = new RectangleF(CMach.PFCenter.X + 117, CMach.PFCenter.Y - 70, 14, 140);

            if (DI074.GetBoolValue())
            {
                g.FillRectangle(Brushes.Lime, rect1);
            }
            else if (DI075.GetBoolValue())
            {
                g.FillRectangle(Brushes.Red, rect1);
            }
            else
            {
                g.FillRectangle(Brushes.Gray, rect1);
            }

            if (DI110.GetBoolValue())
            {
                g.FillRectangle(Brushes.Lime, rect2);
            }
            else if (DI111.GetBoolValue())
            {
                g.FillRectangle(Brushes.Red, rect2);
            }
            else
            {
                g.FillRectangle(Brushes.Gray, rect2);
            }

            Single iL = 300;
            Single iH = 40;
            Single iW = 30;
            RectangleF rect3 = new RectangleF(CMach.PFCenter.X - iH, CMach.PFCenter.Y - iL - iW, iH / 2, iW * 2);
            RectangleF rect4 = new RectangleF(CMach.PFCenter.X + iH / 2, CMach.PFCenter.Y - iL - iW, iH / 2, iW * 2);
            bool B2 = false;
            bool B3 = false;
            SolidBrush FontBrush = new SolidBrush(System.Drawing.Color.DeepSkyBlue);
            FontBrush.Color = Color.White;
            StringFormat format1 = new StringFormat(StringFormatFlags.LineLimit);
            format1.Alignment = StringAlignment.Center;
            format1.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < CMach.TuoPanNum; i++)
            {
                if (CMach.TuoPan[i] == 2)//进样室高位
                {
                    B2 = true;
                }
                else if (CMach.TuoPan[i] == 3)//进样室高位
                {
                    B3 = true;
                }
            }
            if (B2)//进样室高位
            {
                g.FillRectangle(Brushes.Red, rect3);
                g.DrawString("上", new Font("宋体", 16, GraphicsUnit.World), FontBrush, rect3, format1);
            }
            else
            {
                g.FillRectangle(Brushes.Gray, rect3);
                g.DrawString("上", new Font("宋体", 16, GraphicsUnit.World), FontBrush, rect3, format1);
            }
            if (B3)//进样室高位
            {
                g.FillRectangle(Brushes.Red, rect4);
                g.DrawString("下", new Font("宋体", 16, GraphicsUnit.World), FontBrush, rect4, format1);
            }
            else
            {
                g.FillRectangle(Brushes.Gray, rect4);
                g.DrawString("下", new Font("宋体", 16, GraphicsUnit.World), FontBrush, rect4, format1);
            }
   
        }

        private void timer1_Tick(object sender, EventArgs e)//定时器
        {
            CTran.CommTimerCall();
            if(bRefresh())
                this.Invalidate();
            if (DI076.GetBoolValue())
            {
                button141.Enabled = true;
                button142.Enabled = false;
            }
            else
            {
                button141.Enabled = false;
                if (SPLC.System[0] == 7)
                    button142.Enabled = false;//配方运行是水冷套下不能点击
                else
                    button142.Enabled = true;
            }

            if (SPLC.System[0] == 18)
                button018.Enabled = false;
            else
                button018.Enabled = true;

            if (SPLC.System[0] == 7)
            {
                button074.Enabled = false;
            }
            else
                button074.Enabled = true;

            if (DI074.GetBoolValue())
            {
                button074.BackColor = Color.LimeGreen;
                button074.Text = "生长室门阀关闭";
            }
            else if (DI075.GetBoolValue())
            {
                button074.BackColor = Color.Red;
                button074.Text = "生长室门阀开启";
            }
            else
            {
                button074.BackColor = Color.Gray;
                button074.Text = "生长室门阀";
            }

            if (DI110.GetBoolValue())
            {
                button110.BackColor = Color.LimeGreen;
                button110.Text = "进样室门阀关闭";
            }
            else if (DI111.GetBoolValue())
            {
                button110.BackColor = Color.Red;
                button110.Text = "进样室门阀开启";
            }
            else
            {
                button110.BackColor = Color.Gray;
                button110.Text = "进样室门阀";
            }
            this.Invalidate();
            //Rectangle rc = new Rectangle(new Point(158, 13), new Size(203, 93));
            //this.Invalidate(rc,true);
        }
        private bool bRefresh()//是否需要刷新界面
        {
            if (CMach.eCommState != CTran.CommStateE)
            {
                return true;
            }
            if (CMach.ActionStep != CTran.ActionStep)
            {
                return true;
            }
            if (CMach.RTZ[0] == CTran.RTZ[0] && CMach.RTZ[1] == CTran.RTZ[1] && CMach.RTZ[2] == CTran.RTZ[2])
            {
                return false;
            }
            return true;
        }
        private void frmChildTran_MouseMove(object sender, MouseEventArgs e)
        {
            Point MouseP = new Point(e.X, e.Y);
            // label1.Location = MouseP;
            // label1.Text =  "(" + MouseP.X.ToString() + "," + MouseP.Y.ToString() + ")";

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
        CForm FormObject = new CForm();
        Point FormToGraphic(Point Point)
        {
            Point.X -= (int)FormObject.Position.X;
            Point.Y -= (int)FormObject.Position.Y;
            Point.X /= (int)FormObject.scale;
            Point.Y /= (int)FormObject.scale;
            return Point;
        }

        private void ExtramanGoto(object sender, EventArgs e)//机械手工位移动
        {
             Button nCon = (Button)sender;
            string str1 = "";
            int i = CMach.iStation / 10;
            int j = CMach.iStation % 10;
            switch (nCon.Name)
            {
                case "ExtraMove01":
                    if(j==0)
                        str1 = "GOTO N " + i.ToString() + " Z UP"; 
                    else
                        str1 = "GOTO N " + i.ToString() + " R EX Z UP"; 
                    break;
                case "ExtraMove02":
                    if (j == 0)
                        str1 = "GOTO N " + i.ToString();
                    else
                        str1 = "GOTO N " + i.ToString() + " R EX"; 
                    break;
                case "ExtraMove03":
                    str1 = "GOTO N " + i.ToString() + " R EX"; 
                    break;
                case "ExtraMove04": str1 = "RETRACT"; break;
                case "ExtraMove05": str1 = "HOME ALL"; break;
                case "ExtraMove06": str1 = "GOTO N 1"; break;
                case "ExtraMove07": str1 = "GOTO N 2"; break;
                case "ExtraMove08": str1 = "GOTO N 3"; break;
                case "ExtraMove09": str1 = "GOTO N 4"; break;
            }
            //Mach.RTZ2Angle();
            if (str1.Length > 0)
            {
                CTran.SendAODO(str1 + "\r\n");
            }
        }

        private void ExtramanRun(object sender, EventArgs e)//机械手捡起、放置、传送
        {
            Button nCon = (Button)sender;
            int iMode = 0;
            switch (nCon.Name)
            {
                case "ExtraMove10": iMode = 1; break;
                case "ExtraMove11": iMode = 2; break;
                case "ExtraMove12": iMode = 3; break;
            }
            frmC机械手控制 nFrm = new frmC机械手控制(iMode);
            nFrm.ShowDialog(this);
            if (nFrm.DialogResult == DialogResult.OK)
            {
                CTran.SendAODO(nFrm.sSendComm);
            }
        }

        private void button21_Click(object sender, EventArgs e)//开启关闭生长室门阀
        {
            bOpenChamber = !bOpenChamber; this.Invalidate();
        }
        private void button20_Click(object sender, EventArgs e)//开启关闭进样室门阀
        {
            bOpenSample = !bOpenSample; this.Invalidate();
        }

        private void buttonSend1002(object sender, EventArgs e)//下发程序控制
        {
            Button nCon = (Button)sender;
            switch (nCon.Name)
            {
                case "button074":
                    if (DI074.GetBoolValue())
                    {
                        if (bMsgBoxShowYN("是否进行生长室门阀关闭程序？", "关闭"))
                        {
                            SPLC.SendAODO("1002", 12, "SY");//12：生长室门阀关闭
                        }
                    }
                    else if (DI075.GetBoolValue())
                    {
                        if (SPLC.System[0] == 7)
                            return;
                        if (bMsgBoxShowYN("是否进行生长室门阀开启程序？", "开启"))
                        {
                            SPLC.SendAODO("1002", 11, "SY");//11:生长室门阀开启
                        }
                    }
                    break;
                case "button110"://进样室门阀
                    if (DI110.GetBoolValue())
                    {
                        if (bMsgBoxShowYN("是否进行进样室门阀关闭程序？", "关闭"))
                        {
                            if(SPLC.System[0] == 7)
                                SPLC.SendAODO("1046", 17, "SY");//17：进样室门阀关闭
                            else
                                SPLC.SendAODO("1002", 17, "SY");//17：进样室门阀关闭
                        }
                    }
                    else if (DI111.GetBoolValue())
                    {
                        if (bMsgBoxShowYN("是否进行进样室门阀开启程序？", "开启"))
                        {
                            if (SPLC.System[0] == 7)
                                SPLC.SendAODO("1046", 16, "SY");//16：进样室门阀开启
                            else
                                SPLC.SendAODO("1002", 16, "SY");//16：进样室门阀开启
                        }
                    }
                    break;
                case "button018":
                    if (bMsgBoxShowYN("是否进行进样室吹扫程序？", "吹扫"))
                    {
                        SPLC.SendAODO("1002", 18, "SY");//18：进样室吹扫
                    }
                    break;
                case "button147":
                    if (bMsgBoxShowYN("是否打开进样室抽气门阀？", "抽气"))
                    {
                        SPLC.SendAODO("AV147", 0, "DO");
                    }
                    break;
                case "button145":
                    if (bMsgBoxShowYN("是否打开进样室充气门阀？", "充气"))
                    {
                        SPLC.SendAODO("AV145", 0, "DO");
                    }
                    break;
                case "button141":
                    if (bMsgBoxShowYN("是否关闭水冷套？", "关闭"))
                    {
                        SPLC.SendAODO("AV141", 1, "DO2");
                        SPLC.SendAODO("AV142", 0, "DO2");
                    }
                    break;
                case "button142":
                    if (bMsgBoxShowYN("是否开启水冷套？", "开启"))
                    {
                        SPLC.SendAODO("AV141", 0, "DO2");
                        SPLC.SendAODO("AV142", 1, "DO2");
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

        private void button19_Click(object sender, EventArgs e)
        {
            frmC密码输入 nFrm = new frmC密码输入(CMach.Password);
            CTran.SendHandAODO("RQ STN 1 ARM A ALL\r\n");
            CTran.SendHandAODO("RQ STN 2 ARM A ALL\r\n");
            CTran.SendHandAODO("RQ STN 3 ARM A ALL\r\n");
            CTran.SendHandAODO("RQ STN 4 ARM A ALL\r\n");
            nFrm.ShowDialog(this);
            if (nFrm.DialogResult == DialogResult.OK)
            {
                frmC机械手设置 nFrm1 = new frmC机械手设置(CTran);
                nFrm1.ShowDialog(this);
            }
        }

        private void ExtraReset_Click(object sender, EventArgs e)
        {
            CTran.ListImmSendMsg.Clear();
            CTran.SendAODO("HALT\r\n");
            this.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                splitContainer2.Visible = false;
                checkBox1.Text = ">>";
            }
            else
            {
                splitContainer2.Visible = true;
                checkBox1.Text = "<<";
            }
        }

        private void frmC机械手_MouseDown(object sender, MouseEventArgs e)
        {
            int i = 0;
            PointF MouseP = new PointF(e.X, e.Y); ;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                for (i = ListTuYuan.Count - 1; i > -1; i--)
                {
                    CBase obj = (CBase)ListTuYuan[i];
                    if (obj.ElementType == LCElementType.Ring)
                    {
                        if (obj.Selected(MouseP))
                        {
                            CRing nRing = (CRing)obj;

                            string sVar = "AV" + nRing.ShowText.PadLeft(3, '0');
                            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");
                            CProtcolFINS SPLC = (CProtcolFINS)Sta;
                            SPLC.SendAODO(sVar, 0, "DO");
                            break;
                        }
                    }
                }
            }
        }

        private void frmC机械手_Resize(object sender, EventArgs e)
        {
            Size aa = this.MdiParent.Size;
            this.Size = new System.Drawing.Size(aa.Width - 20, aa.Height - frmMain.iToolHeight);
        }

        private void frmC机械手_FormClosing(object sender, FormClosingEventArgs e)
        {
            TPortThread.Abort();
        }

        private void PauseShowMsg_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    //机械手
    class Machining : CTuYuan
    {
        public PointF PFCenter = new PointF(925, 490);//机械手中心点位置

        public Single[] BAngle = new Single[3] { 60, 300, 180 };//大臂，小臂，手掌的角度顺时针方向
        public int iR1 = 34;//大臂直径1
        public int iR2 = 17;//大臂直径2
        public int iL = 76;//臂长
        public int iDR = 51; //底座直径
        Color iColor1 = Color.Green;
        Color iColor2 = Color.Fuchsia;

        public ECommSatate eCommState;

        public string[] sRTZ = new string[5];
        public double[] RTZ = new double[3] { -282, 0, 1 };
        public double[][] ListRTZ = new double[5][];    //工位参数
        public int iStation = 0;//工位,0:初始 
        //10:生长室收   11:生长室伸   12:生长室方向
        //20:进样室上收 21:进样室上伸 22:进样室上方向
        //30:进样室下收 31:进样室下伸 32:进样室下方向
        //40:层流室收   41:层流室伸   42:层流室方向
        //0:其他非安全方向
        public string Setting = "";
        public int ActionStep;

        public string Password = "";
        public double iRD = 0;
        public double iTD = 0;
        public double iZD = 0;

        CVar[] nDI = new CVar[7];
        public bool[] bDI
        {
            get
            {
                bool[] bb = new bool[7];
                for (int i = 0; i < 7; i++)
                {
                    bb[i] = nDI[i].GetBoolValue();
                }
                return bb;
            }
        }
        PointF[] TuoPanPT = new PointF[4];

        public int[] TuoPan = new int[] { 0, 0 };//托盘位置 1生长室 2进样室上 3进样室下 4层流室 11层流室方向 21 进样室方向 31生长室方向 
        public int TuoPanNum = 1;
        public double ROld = 0;
        public int iOutIn = 0;
        public Machining()
            : base()
        {
            ListRTZ[0] = new double[] { -282.107, -0.319, 1.122 };
            ListRTZ[1] = new double[] { 477.529, 179.494, 15.755 };
            ListRTZ[2] = new double[] { 208.089, 89.778, 23.226 };
            ListRTZ[3] = new double[] { 208.147, 89.778, -6.9 };
            ListRTZ[4] = new double[] { 322.765, -0.318, 13.77 };

            nDI[0] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI102");//生长室
            nDI[1] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI103");//进样室上
            nDI[2] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI104");//进样室下
            nDI[3] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI109");//层流室
            nDI[4] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI107");//层流室门阀
            nDI[5] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI105");//进样室门阀
            nDI[6] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI101");//生长室门阀

            TuoPanPT[0] = new PointF(667.7635f, 487.2388f);
            TuoPanPT[1] = new PointF(925.92f, 314.57f);
            TuoPanPT[2] = new PointF(925.92f, 314.5f);
            TuoPanPT[3] = new PointF(1135.34f, 491.51f);
        }

        public override void LoadFromXML(XmlElement CBaseNode)
        {
            try
            {
                GLayerName = CBaseNode.GetAttribute("GLayerName");
                Name = CBaseNode.GetAttribute("Name");
                Setting = CBaseNode.GetAttribute("Setting");
                Password = CBaseNode.GetAttribute("Password");

                iOrgX1 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgX1"));
                iOrgY1 = Convert.ToSingle(CBaseNode.GetAttribute("iOrgY1"));
                iRD = Convert.ToDouble(CBaseNode.GetAttribute("iRD"));
                iTD = Convert.ToDouble(CBaseNode.GetAttribute("iTD"));
                iZD = Convert.ToDouble(CBaseNode.GetAttribute("iZD"));

                sRTZ[0] = CBaseNode.GetAttribute("RTZ0");
                sRTZ[1] = CBaseNode.GetAttribute("RTZ1");
                sRTZ[2] = CBaseNode.GetAttribute("RTZ2");
                sRTZ[3] = CBaseNode.GetAttribute("RTZ3");
                sRTZ[4] = CBaseNode.GetAttribute("RTZ4");
                for (int i = 0; i < 5; i++)
                {
                    string[] vRTZ = sRTZ[i].Split(',');
                    for (int j = 0; j < Math.Min(vRTZ.Length, 3); j++)
                    {
                        ListRTZ[i][j] = Convert.ToDouble(vRTZ[j]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Machining.LoadFromXML" + e.Message);
            }
        }

        public void MachByStation(int iIndex)//按工位移动
        {
            iStation = iIndex;
            switch (iIndex)
            {
                case 11: RTZ = new double[3]; ListRTZ[1].CopyTo(RTZ, 0); break;
                case 21: RTZ = new double[3]; ListRTZ[2].CopyTo(RTZ, 0); break;
                case 31: RTZ = new double[3]; ListRTZ[3].CopyTo(RTZ, 0); break;
                case 41: RTZ = new double[3]; ListRTZ[4].CopyTo(RTZ, 0); break;
                case 10: RTZ = new double[3]; ListRTZ[1].CopyTo(RTZ, 0); RTZ[0] = ListRTZ[0][0]; break;
                case 20: RTZ = new double[3]; ListRTZ[2].CopyTo(RTZ, 0); RTZ[0] = ListRTZ[0][0]; break;
                case 30: RTZ = new double[3]; ListRTZ[3].CopyTo(RTZ, 0); RTZ[0] = ListRTZ[0][0]; break;
                case 40: RTZ = new double[3]; ListRTZ[4].CopyTo(RTZ, 0); RTZ[0] = ListRTZ[0][0]; break;
                default: break;
            }
            RTZ2Angle();
        }

        public void UpdateRTZ(double[] drtz)
        {
            RTZ = new double[3];
            drtz.CopyTo(RTZ, 0);
            RTZ2Angle();
        }

        public void RTZ2Angle()
        {
            int iD = 90;
            double R = RTZ[0];
            double T = RTZ[1];
            double Z = RTZ[2];
            double iAcos = Math.Abs(R) / (250 * 2);
            double T1 = Math.Acos(iAcos) * 180 / Math.PI;
            if (R < 0)
            {
                BAngle[0] = (Single)(180 - T1 - T) + iD;
                BAngle[1] = (Single)(180 + T1 - T) + iD;
                BAngle[2] = (Single)(-T) + iD;
            }
            else
            {
                BAngle[0] = (Single)(T1 - T) + iD;
                BAngle[1] = (Single)(360 - T1 - T) + iD;
                BAngle[2] = (Single)(-T) + iD;
            }
            for (int i = 1; i < 5; i++)
            {
                if (IsAngelBetweenSlew((Single)T, (Single)(ListRTZ[i][1] + 1), (Single)(ListRTZ[i][1] - 1)))//角度符合
                {
                    if (i == 2 || i == 3)
                    {
                        if (Z < (ListRTZ[2][2] + ListRTZ[3][2]) / 2)
                        {
                            i = 3;
                        }
                        else
                        {
                            i = 2;
                        }
                    }
                    if (IsNearby(R, ListRTZ[i][0], 1))//R位置符合伸出值
                    {
                        iStation = i * 10 + 1;
                    }
                    else if (IsNearby(R, ListRTZ[0][0], 1))//R位置符合初始值
                    {
                        iStation = i * 10;
                    }
                    else
                    {
                        iStation = i * 10 + 2;
                    }
                    break;
                }
                else
                {
                    if (IsNearby(R, ListRTZ[0][0], 1))//R位置符合初始值
                    {
                        iStation = 0;
                    }
                    else
                    {
                        iStation = 2;
                    }
                }
            }
            for (int i = 0; i < 3; i++)
            {
                while (BAngle[i] < 0)
                {
                    BAngle[i] += 360;
                }
                while (BAngle[i] >= 360)
                {
                    BAngle[i] -= 360;
                }
            }
        }

        bool IsNearby(double RValue, double slew, double iD)
        {
            if (RValue >= slew - Math.Abs(iD) && RValue <= slew + Math.Abs(iD))
            {
                return true;
            }
            return false;
        }
        bool IsAngelBetweenSlew(float angle, float slewr, float slewl)
        {
            float angler = angle;
            float anglel = angle;
            while (angler < slewl)
            {
                angler += 360;
            }
            while (anglel > slewr)
            {
                anglel -= 360;
            }
            if (IsAngelBetweenSlew360(angler, slewr, slewl) || IsAngelBetweenSlew360(anglel, slewr, slewl))
            {
                return true;
            }
            return false;
        }
        bool IsAngelBetweenSlew360(float angle, float slewr, float slewl)
        {
            if (angle >= slewl && angle <= slewr)
            {
                return true;//在区域中
            }
            return false;
        }

        public void DrawBI1(Graphics g)//画底座和大臂
        {
            PFCenter = new PointF(925, 490);//机械手中心点位置
            g.TranslateTransform(PFCenter.X, PFCenter.Y);
            g.RotateTransform(BAngle[0] - 90);
            GraphicsPath myPath = new GraphicsPath();

            myPath.AddEllipse(-iDR / 2, -iDR / 2, iDR, iDR);
            g.FillPath(Brushes.Red, myPath);
            // g.DrawPath(new Pen(Color.Black, 1f), myPath);

            myPath = new GraphicsPath();

            myPath.AddArc(-iR1 / 2, -iR1 / 2, iR1, iR1, 90, 180);
            myPath.AddLine(new PointF(0, -iR1 / 2), new PointF(iL, -iR2 / 2));
            myPath.AddArc(iL - iR2 / 2, -iR2 / 2, iR2, iR2, -90, 180);
            myPath.AddLine(new PointF(iL, iR2 / 2), new PointF(0, iR1 / 2));

            SolidBrush brush = new SolidBrush(iColor1);
            g.FillPath(brush, myPath);

            g.DrawPath(new Pen(Color.Black, 1f), myPath);

            PointF[] points = { new Point(iL, 0) };
            g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, points);
            g.ResetTransform();
            DrawBI2(g, points[0]);
        }
        private void DrawBI2(Graphics g, PointF iPF)//画小臂
        {
            GraphicsPath myPath = new GraphicsPath();
            int imR1 = iR2;//大臂直径1
            int imR2 = iR2;//大臂直径2

            g.TranslateTransform(iPF.X, iPF.Y);
            g.RotateTransform(BAngle[1] - 90);

            myPath = new GraphicsPath();

            myPath.AddArc(-imR1 / 2, -imR1 / 2, imR1, imR1, 90, 180);
            myPath.AddLine(new PointF(0, -imR1 / 2), new PointF(iL, -imR2 / 2));
            myPath.AddArc(iL - imR2 / 2, -imR2 / 2, imR2, imR2, -90, 180);
            myPath.AddLine(new PointF(iL, imR2 / 2), new PointF(0, imR1 / 2));

            SolidBrush brush = new SolidBrush(iColor2);
            g.FillPath(brush, myPath);
            g.DrawPath(new Pen(Color.Black, 1f), myPath);

            PointF[] points = { new Point(iL, 0) };
            g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, points);
            g.ResetTransform();
            DrawBI3(g, points[0], Color.Red, 52, imR2);
        }
        private void DrawBI3(Graphics g, PointF iPF, Color iColor, int iL, int iR)//画手掌
        {
            GraphicsPath myPath = new GraphicsPath();
            int iR1 = iR;//大臂直径1
            int iR2 = 60;//大臂直径2
            int iL1 = 46;
            int iL2 = 6;
            g.TranslateTransform(iPF.X, iPF.Y);
            g.RotateTransform(BAngle[2] - 90);

            myPath = new GraphicsPath();
            myPath.AddArc(-iR1 / 2, -iR1 / 2, iR1, iR1, 90, 180);
            myPath.AddLine(new PointF(0, -iR1 / 2), new PointF(iL1 - iR1 / 2, -iR1 / 2));
            //myPath.AddLine(new PointF(iL1 - iR1 / 2, -iR1 / 2), new PointF(iL1, -iR1));
            //myPath.AddArc(iL1 - iR1-iR1, -iR1 - iR1 / 2, iR1*2, iR1, 90, -90);
            myPath.AddArc(iL1, -iR2, 2 * (iR2 + iL2), 2 * (iR2 - iR1), 180, 90);
            myPath.AddArc(iL1 + iL2, -iR2, iR2 * 2, iR2 * 2, 270, -180);
            myPath.AddArc(iL1, iR2 - 2 * (iR2 - iR1), 2 * (iR2 + iL2), 2 * (iR2 - iR1), 90, 90);
            // myPath.AddArc(iL1 - iR1, iR1 / 2, iR1, iR1, 0,- 90);
            myPath.AddLine(new PointF(iL1 - iR1 / 2, iR1 / 2), new PointF(0, iR1 / 2));

            SolidBrush brush = new SolidBrush(iColor);
            g.FillPath(brush, myPath);
            g.DrawPath(new Pen(Color.Black, 1f), myPath);

            PointF[] points = { new Point(iL1 + iL2 + iR2, 0) };
            g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, points);
            g.ResetTransform();
            for (int i = TuoPanNum-1; i >=0; i--)
            {
                DrawTuoPan(g, points[0], Color.RoyalBlue, iR2 / 4, TuoPan[i]);
            }

        }
        private void DrawTuoPan(Graphics g, PointF iPF, Color iColor, int iR,int iTupPan)//画托盘
        {
            int iR2 = 60;//大臂直径2
            if (iTupPan >= 10 || iTupPan <= 0)
            {
                g.TranslateTransform(iPF.X, iPF.Y);
            }
            else
            {
                g.TranslateTransform(TuoPanPT[iTupPan - 1].X, TuoPanPT[iTupPan - 1].Y);
            }
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse(2 - iR2, 2 - iR2, iR2 * 2 - 4, iR2 * 2 - 4);
            SolidBrush brush = new SolidBrush(Color.DodgerBlue);
            g.FillPath(brush, myPath);
            g.DrawPath(new Pen(Color.Black, 1f), myPath);

            for (int i = 0; i < 6; i++)
            {
                myPath = new GraphicsPath();
                myPath.AddEllipse(iR / 2, iR / 2, iR, iR);
                g.RotateTransform(60);
                brush = new SolidBrush(iColor);
                //g.FillPath(brush, myPath);
                g.DrawPath(new Pen(Color.DarkSlateGray, 1f), myPath);
            }

            for (int i = 0; i < 12; i++)
            {
                myPath = new GraphicsPath();
                myPath.AddEllipse(iR * 2, iR / 2, iR, iR);
                g.RotateTransform(30);
                brush = new SolidBrush(iColor);
                //g.FillPath(brush, myPath);
                g.DrawPath(new Pen(Color.DarkSlateGray, 1f), myPath);
            }
            g.ResetTransform();
        }
    }
}
