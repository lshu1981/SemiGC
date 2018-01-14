using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PublicDll;
using System.Collections;

namespace LSSCADA.Control
{
    public struct SDopant
    {
        public int ID;//掺杂物编号
        public string Name;//掺杂物名称
        public int MinVentTime;//最小Vent时间
    }
    public struct SAlarmSet
    {
        public int ID;// 数字量编号
        public string Name;//名称
        public string Action;//动作
    }
    
    public partial class frm设置 : Form
    {
        string sXMLPath;
        List<SDopant> ListDopant = new List<SDopant>();
        List<NumericUpDown> ListPecipeRunRules = new List<NumericUpDown>();
        CProtcolFINS SPLC;

        string[] sCol2 = new string[] { "编号", "名称", "种类", "PC(torr)", "改变速率(s)", "最小Vent时间(s)", "恒温槽", "设定温度(℃)", "实际温度(℃)", "初始重量(g)", "剩余重量(g)", "PMO(torr)" };
        bool[] bCol2 = new bool[] { true, true, true };
        string[] sCol3 = new string[] { "变量名称", "报文序号", "字偏移", "位偏移", "报文类型" };
        string[] sCol8 = new string[] { "欧陆表", "最小值", "最大值", " P ", " I ", " D " };
        bool[] bCol8 = new bool[] { true };
        string[] sCol5 = new string[] { "编号", "名称", "类型", "报警值"};
        bool[] bCol5 = new bool[] { true, true };

        string Password = "82304150";
        public frm设置()
        {
            InitializeComponent();
        }

        private void frm设置_Load(object sender, EventArgs e)
        {
            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");
            SPLC = (CProtcolFINS)Sta;
            //SPLC.SendReadIdle();

            LoadXML();
            FillGrid();
        }

        private void LoadXML()
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            sXMLPath = CProject.sPrjPath + "\\Project\\Setting.xml";
            myxmldoc.Load(sXMLPath);

            string xpath = "root/Dopant";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                SDopant nSet;
                nSet.Name = item.GetAttribute("Name");
                nSet.ID = Convert.ToInt32(item.GetAttribute("ID"));
                nSet.MinVentTime = Convert.ToInt32(item.GetAttribute("MinVentTime"));
                ListDopant.Add(nSet);
            }

            xpath = "root/PecipeRunRules";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            if(childNode!=null)
                FillGrid6(childNode);

            xpath = "root/Other";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            FillGrid7(childNode);

            xpath = "root/Maintenance";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            FillGrid4(childNode);

            xpath = "root/System";

            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            if (childNode.HasAttribute("Password"))
                Password = childNode.GetAttribute("Password");
        }

        private void FillGrid()
        {
            sCol8 = new string[] { "欧陆表", "最小值", "最大值", "   P   ", "   I   ", "   D   " };
            CPublicDGV.InitializeDGV(dGV2, sCol2, bCol2, true);
            CPublicDGV.InitializeDGV(dGV5, sCol5, bCol5, true);
            CPublicDGV.InitializeDGV(dGV8, sCol8, bCol8, true);
            FillGrid1();
            FillGrid2();
            FillGrid3();
            FillGrid5();
            FillGrid8();
            UpdatePLCValue();
            NotSortable(dGV1);
            NotSortable(dGV2);
            NotSortable(dGV3);
            NotSortable(dGV5);
            NotSortable(dGV8);

        }
        private void NotSortable(DataGridView dgv)
        {
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void FillGrid1()//模拟量
        {
            ArrayList obj = new ArrayList();
            string[] str1;
            foreach (CVar nVar in SPLC.StaDevice.ListDevVar)
            {
                if (nVar.ByteAddr >= SPLC.intStart && nVar.ByteAddr < SPLC.intEnd)
                {
                    str1 = new string[10];
                    int k = 1;
                    str1[k++] = nVar.Name;
                    str1[k++] = nVar.Description;
                    str1[k++] = nVar.StrTag[0];
                    str1[k++] = nVar.Unit;
                    str1[k++] = nVar.IntTag[9].ToString();
                    obj.Add(str1);
                }
            }
            foreach (string[] rowArray in obj)
            {
                dGV1.Rows.Add(rowArray);
            }
        }
        private void FillGrid2()//源瓶
        {
            dGV2.RowCount =frmMain.staComm.nBubbler.BubblerList.Count;
            for (int i = 0; i < frmMain.staComm.nBubbler.BubblerList.Count; i++)
            {
                Bubbler nbub = frmMain.staComm.nBubbler.BubblerList[i];
                dGV2.Rows[i].Cells[0].Value = nbub.ID;	//	源瓶编号
                dGV2.Rows[i].Cells[1].Value = nbub.Name;	//	源瓶名称
                dGV2.Rows[i].Cells[2].Value = nbub.Type;	//	种类
                dGV2.Rows[i].Cells[3].Value = nbub.PC;	//	PC(torr)
                dGV2.Rows[i].Cells[4].Value = nbub.Ramp;	//	改变速率(s)
                dGV2.Rows[i].Cells[5].Value = nbub.MinVentTime;	//	最小Vent时间
                dGV2.Rows[i].Cells[6].Value = nbub.RE215;	        //	恒温槽
                dGV2.Rows[i].Cells[7].Value = nbub.DesiredTemp;	//	设定温度(℃)
                dGV2.Rows[i].Cells[8].Value = nbub.BathTemp;	//	恒温槽温度(℃)
                dGV2.Rows[i].Cells[9].Value = nbub.sWeight;	//	源瓶重量
                dGV2.Rows[i].Cells[10].Value = nbub.sUseWeight;	//	源瓶重量
                dGV2.Rows[i].Cells[11].Value = nbub.sPMO;	//	PMO
            }
        }
        private void FillGrid3()//掺杂物
        {
            dGV3.RowCount = ListDopant.Count;
            for (int i = 0; i < ListDopant.Count; i++)
            {
                dGV3.Rows[i].Cells[0].Value = ListDopant[i].ID;
                dGV3.Rows[i].Cells[1].Value = ListDopant[i].Name;
                dGV3.Rows[i].Cells[2].Value = ListDopant[i].MinVentTime;
            }
        }
        private void FillGrid4(XmlElement childNode)//读取维护
        {
            try
            {
                num9001.Value = Convert.ToInt32(childNode.GetAttribute("num9001"));//加热丝内圈电阻
                num9002.Value = Convert.ToInt32(childNode.GetAttribute("num9002"));//加热丝中圈电阻
                num9003.Value = Convert.ToInt32(childNode.GetAttribute("num9003"));//加热丝外圈电阻
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillGrid5()//报警
        {
            dGV5.Rows.Clear();
            ArrayList obj = new ArrayList();
            string[] str1; 
            foreach (CVar nVar in SPLC.StaDevice.ListDevVar)
            {
                if (nVar.DAType == EDAType.DA_YC )
                    continue;
                if (!check8DO.Checked)
                {
                    if (nVar.Writeable == 1)
                        continue;
                }
                str1 = new string[4];
                int k = 0;
                str1[k++] = nVar.Name;
                str1[k++] = nVar.Description;
                str1[k++] = nVar.DIAlarmType;
                str1[k++] = nVar.DIAlarmValue;
              obj.Add(str1);
            }
            foreach (string[] rowArray in obj)
            {
                dGV5.Rows.Add(rowArray);
            }
            button5.Enabled = false;
        }
        private void FillGrid6(XmlElement childNode)//读取配方运行规则
        {
            int i = 0;
            int iH = 35;
            int iW = 400;
            int iLen = 300;
            int iNum = 8;
            foreach (XmlElement item in childNode.ChildNodes)
            {
                Label nLab = new Label();
                nLab.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                nLab.ForeColor = System.Drawing.Color.Black;
                nLab.Location = new System.Drawing.Point(8 + (int)(i / iNum) * iW, 33 + (int)(i % iNum) * iH);
                nLab.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
                nLab.Name = item.GetAttribute("Key");
                nLab.Size = new System.Drawing.Size(iLen, 22);
                nLab.Text = item.GetAttribute("Name");
                nLab.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                this.groupBox5.Controls.Add(nLab);

                NumericUpDown nNum = new NumericUpDown();
                nNum.Location = new System.Drawing.Point(iLen + 15 + (int)(i / iNum) * iW, 29 + (int)(i % iNum) * iH);
                nNum.Maximum = new decimal(new int[] {10000, 0, 0, 0 });
                nNum.Name = "PRRN" + item.GetAttribute("Key");
                nNum.Size = new System.Drawing.Size(81, 29);
                nNum.Tag = item.GetAttribute("Key");
                nNum.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
                nNum.Value = new decimal(new int[] { Convert.ToInt32(item.GetAttribute("Value")), 0, 0, 0 });
                this.groupBox5.Controls.Add(nNum);
                ListPecipeRunRules.Add(nNum);
                i++;
            }
        }
        private void FillGrid7(XmlElement childNode)//读取其他配置
        {
            try
            {
                OtherNum1.Value = Convert.ToInt32(childNode.GetAttribute("OtherNum1"));//Idle切换压力
                OtherNum2.Value = Convert.ToInt32(childNode.GetAttribute("OtherNum2"));//门阀开启压力
                OtherNum3.Value = Convert.ToInt32(childNode.GetAttribute("OtherNum3"));//喉阀
                
                OtherCBox1.Text = childNode.GetAttribute("OtherCBox1");//压力单位
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillGrid8()//PID设置
        {
            try
            {
                ArrayList obj = new ArrayList();
                string[] str1;
               
                dGV8.Rows.Clear();
                for (int i = 0; i < 3; i++)
                {
                    foreach (PID nPID in frmMain.nMainVar.ListPID[i])
                    {
                        str1 = new string[sCol8.Length];
                        int k = 0;
                        str1[k++] = (i + 1).ToString() + "." + (frmMain.nMainVar.ListPID[i].IndexOf(nPID) + 1).ToString() + "." + frmMain.nMainVar.sListOLName[i];
                        str1[k++] = nPID.Min.ToString();
                        str1[k++] = nPID.Max.ToString();
                        str1[k++] = nPID.P.ToString();
                        str1[k++] = nPID.I.ToString();
                        str1[k++] = nPID.D.ToString();
                        obj.Add(str1);

                    }
                }
                foreach (string[] rowArray in obj)
                {
                    dGV8.Rows.Add(rowArray);
                }
            }
            catch
            {}
        }

        private void button1_Click(object sender, EventArgs e)//保存模拟量
        {
            if (MessageBox.Show("是否保存模拟量设置的修改？", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }

            int i=0, j=0;
            try
            {
                for (i = 0; i < dGV1.RowCount; i++)
                {
                    for (j = 5; j <= 8; j++)
                    {
                        double ff = Convert.ToSingle(dGV1.Rows[i].Cells[6].Value);
                    }
                }
            }
            catch (Exception ee)
            {
                string str1 = "第" + i.ToString() + "行第" + j.ToString() + "列：";
                if (i >= 0 && i < dGV1.RowCount)
                    str1 = dGV1.Rows[i].Cells[1].Value.ToString();
                MessageBox.Show(str1 + ee.Message, "格式错误");
                return;
            }
            try
            {
                foreach (CVar nVar in SPLC.StaDevice.ListDevVar)
                {
                    if (nVar.ByteAddr >= SPLC.intStart && nVar.ByteAddr < SPLC.intEnd)
                    {
                        for (i = 0; i < dGV1.RowCount; i++)
                        {
                            if (nVar.Name == Convert.ToString(dGV1.Rows[i].Cells[1].Value))
                            {
                                nVar.IntTag[9] = Convert.ToSingle(dGV1.Rows[i].Cells[5].Value);
                                nVar.RatioValue = nVar.IntTag[9] / nVar.IntTag[8];
                                if (nVar.Name == "A_52" || nVar.Name == "A_53" || nVar.Name == "A_54" || nVar.Name == "A_55")
                                {
                                    nVar.IntTag[5] =(int)( Convert.ToSingle(dGV1.Rows[i].Cells[6].Value) * 10);
                                    nVar.IntTag[6] =(int)( Convert.ToSingle(dGV1.Rows[i].Cells[7].Value) * 10);
                                    nVar.IntTag[7] =(int)( Convert.ToSingle(dGV1.Rows[i].Cells[8].Value) * 10);
                                }
                                else
                                {
                                    nVar.IntTag[5] = Convert.ToInt32(dGV1.Rows[i].Cells[6].Value);
                                    nVar.IntTag[6] = Convert.ToInt32(dGV1.Rows[i].Cells[7].Value);
                                    nVar.IntTag[7] = Convert.ToInt32(dGV1.Rows[i].Cells[8].Value);
                                }
                                break;
                            }
                        }
                    }
                }               
                SPLC.SendWriteIdle();
                SPLC.SendReadIdle();

                int B_23 = 1500;
                XmlDocument myxmldoc = new XmlDocument();
                string sPath = CProject.sPrjPath + "\\Project\\IO\\PortInf_Table.xml";
                myxmldoc.Load(sPath);

                string xpath = "IO/VariableInf_Table/row";
                XmlNodeList childNode = myxmldoc.SelectNodes(xpath);
                foreach (XmlElement node in childNode)
                {
                    string sName = node.GetAttribute("Name");
                    foreach (CVar nVar in SPLC.StaDevice.ListDevVar)
                    {
                        if (nVar.ByteAddr >= SPLC.intStart && nVar.ByteAddr < SPLC.intEnd)
                        {
                            if (sName == nVar.Name)
                            {
                                if (sName == "B_23")
                                    B_23 =(int) nVar.IntTag[9];
                                node.SetAttribute("IntTag9", nVar.IntTag[9].ToString());
                                node.SetAttribute("RatioValue", nVar.RatioValue.ToString("f9"));
                                break;
                            }
                        }
                    }
                }
                myxmldoc.Save(sPath);

                myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                xpath = "root/PecipeRunRules";
                XmlElement cldNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in cldNode.ChildNodes)
                {
                    string sKey = item.GetAttribute("Key");
                    if (sKey == "MMaxValue")
                    {
                        foreach (NumericUpDown nNum in ListPecipeRunRules)
                        {
                            if (nNum.Tag.ToString() == sKey)
                            {
                                nNum.Value = B_23;
                                item.SetAttribute("Value", nNum.Value.ToString());
                                break;
                            }
                        }
                        break;
                    }
                }

                myxmldoc.Save(sXMLPath);
                MessageBox.Show("模拟量配置修改保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                i++;
                MessageBox.Show("修改模拟量:R" + i.ToString() + ":" + ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)//保存源瓶
        {
            if (MessageBox.Show("是否保存源瓶设置的修改？", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }
            int i = 0;
            try
            {
                for (i = 0; i < dGV2.RowCount; i++)
                {
                    if (dGV2.Rows[i].Cells[1].Value != null)
                    {
                        int iID = Convert.ToInt32(dGV2.Rows[i].Cells[3].Value);
                        Single sin = Convert.ToSingle(dGV2.Rows[i].Cells[4].Value); //nbub.Ramp;	//	改变速率(s)
                        iID = Convert.ToInt32(dGV2.Rows[i].Cells[5].Value); //nbub.MinVentTime;	//	最小Vent时间
                        sin = Convert.ToSingle(dGV2.Rows[i].Cells[7].Value); //nbub.DesiredTemp;	//	设定温度(℃)
                        double dd = Convert.ToDouble(dGV2.Rows[i].Cells[9].Value);  //nbub.sWeight;	//	源瓶重量
                        dd = Convert.ToDouble(dGV2.Rows[i].Cells[11].Value); //nbub.PMO;	//	PMO*/
                    }
                }
            }
            catch (Exception ex)
            {
                i++;
                MessageBox.Show("修改源瓶:R" + i.ToString() + ":" + ex.Message);
                return;
            }

            try
            {
                foreach (Bubbler nbub in frmMain.staComm.nBubbler.BubblerList)
                {
                    for (i = 0; i < dGV2.RowCount; i++)
                    {
                        if (dGV2.Rows[i].Cells[1].Value == null)
                            continue;
                        string sName = dGV2.Rows[i].Cells[1].Value.ToString();
                        if (sName == nbub.Name)
                        {
                            nbub.PC = Convert.ToInt32(dGV2.Rows[i].Cells[3].Value);
                            nbub.Ramp = Convert.ToSingle(dGV2.Rows[i].Cells[4].Value); 	//	改变速率(s)
                            nbub.MinVentTime = Convert.ToInt32(dGV2.Rows[i].Cells[5].Value); //	最小Vent时间
                            nbub.DesiredTemp = Convert.ToSingle(dGV2.Rows[i].Cells[7].Value); //	设定温度(℃)
                            nbub.Weight = Convert.ToDouble(dGV2.Rows[i].Cells[9].Value);  //	源瓶重量
                            nbub.sPMO = dGV2.Rows[i].Cells[11].Value.ToString(); //	sPMO
                        }
                    }
                }

                XmlDocument myxmldoc = new XmlDocument();
                string sbXMLPath = CProject.sPrjPath + "\\Project\\Setting.xml";
                myxmldoc.Load(sbXMLPath);

                string xpath = "root/Bubbler/row";
                XmlNodeList childNode = myxmldoc.SelectNodes(xpath);
                foreach (XmlElement node in childNode)
                {
                    string sName = node.GetAttribute("Name");
                    foreach (Bubbler nbub in frmMain.staComm.nBubbler.BubblerList)
                    {
                        if (sName == nbub.Name)
                        {
                            node.SetAttribute("PC", nbub.PC.ToString());
                            node.SetAttribute("Ramp", nbub.Ramp.ToString());
                            node.SetAttribute("MinVentTime", nbub.MinVentTime.ToString());
                            node.SetAttribute("DesiredTemp", nbub.DesiredTemp.ToString());
                            node.SetAttribute("RE215", nbub.RE215.ToString() +"."+ nbub.sVarT);
                            node.SetAttribute("Weight", nbub.sWeight);
                            node.SetAttribute("PMO", nbub.sPMO);
                        }
                    }
                }

                /*dGV2.Rows[i].Cells[4].Value = nbub.Ramp;	//	改变速率(s)
                dGV2.Rows[i].Cells[5].Value = nbub.MinVentTime;	//	最小Vent时间
                dGV2.Rows[i].Cells[6].Value = nbub.RE215;	        //	恒温槽
                dGV2.Rows[i].Cells[7].Value = nbub.DesiredTemp;	//	设定温度(℃)
                dGV2.Rows[i].Cells[8].Value = nbub.BathTemp;	//	恒温槽温度(℃)
                dGV2.Rows[i].Cells[9].Value = nbub.sWeight;	//	源瓶重量
                dGV2.Rows[i].Cells[10].Value = nbub.sUseWeight;	//	源瓶重量
                dGV2.Rows[i].Cells[11].Value = nbub.PMO;	//	PMO*/
                myxmldoc.Save(sbXMLPath);
                MessageBox.Show("源瓶配置修改保存成功","成功",MessageBoxButtons.OK,MessageBoxIcon.Asterisk );
            }
            catch (Exception ex)
            {
                i++;
                MessageBox.Show("修改源瓶:R" + i.ToString() + ":" + ex.Message);
            }
        }
        private void button3_Click(object sender, EventArgs e)//保存掺杂物
        {
            int i = 0;
            if (MessageBox.Show("是否保存掺杂物设置的修改？", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/Dopant/row";
                XmlNodeList childNode = myxmldoc.SelectNodes(xpath);
                foreach (XmlElement node in childNode)
                {
                    string sName = node.GetAttribute("Name");
                    for (i = 0; i < dGV3.RowCount; i++)
                    {
                        if (sName == Convert.ToString(dGV3.Rows[i].Cells[1].Value))
                        {
                            int iID = Convert.ToInt32(dGV3.Rows[i].Cells[2].Value);
                            node.SetAttribute("MinVentTime", iID.ToString());
                        }
                    }
                }
                myxmldoc.Save(sXMLPath);
                MessageBox.Show("掺杂物配置修改保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                i++;
                MessageBox.Show("修改掺杂物:R" + i.ToString() + ":" + ex.Message);
            }
        }
        private void button4_Click(object sender, EventArgs e)//保存维护
        {
            if (MessageBox.Show("是否保存维护设置的修改？", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }

            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/Maintenance";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);

                childNode.SetAttribute("num9001", num9001.Value.ToString());
                childNode.SetAttribute("num9002", num9002.Value.ToString());
                childNode.SetAttribute("num9003", num9003.Value.ToString());
                            
                myxmldoc.Save(sXMLPath);

                if ((int)num1022.Value != SPLC.System[21]) SPLC.SendAODO("1022", (int)num1022.Value, "SY");//1022	W	源瓶吹扫次数设定
                if ((int)num1023.Value != SPLC.System[22]) SPLC.SendAODO("1023", (int)num1023.Value, "SY");//1023	W	源瓶吹扫时间设定
                if ((int)num1032.Value != SPLC.System[31]) SPLC.SendAODO("1032", (int)num1032.Value, "SY");//1032	R	生长室次扫次数
                if ((int)num1033.Value != SPLC.System[32]) SPLC.SendAODO("1033", (int)num1033.Value, "SY");//1033	R	生长室次扫时间

                MessageBox.Show("维护配置修改保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改维护:"+ ex.Message);
            }
        }
        private void button5_Click(object sender, EventArgs e)//保存警报
        {
            int i = 0;
            if (MessageBox.Show("是否保存警报设置的修改？", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            try
            {
                foreach (CVar nVar in SPLC.StaDevice.ListDevVar)
                {
                    if (nVar.DAType == EDAType.DA_YX)
                    {
                        for (i = 0; i < dGV5.RowCount; i++)
                        {
                            if (nVar.Name == Convert.ToString(dGV5.Rows[i].Cells[0].Value))
                            {
                                nVar.Description = Convert.ToString(dGV5.Rows[i].Cells[1].Value);
                                string sType = "",sValue = "";
                                if (dGV5.Rows[i].Cells[2].Value != null)
                                {
                                    sType = dGV5.Rows[i].Cells[2].Value.ToString();
                                    sValue = dGV5.Rows[i].Cells[3].Value.ToString();
                                }
                                nVar.SetDIAlarm(sType, sValue);
                            }
                        }
                    }
                }
                frmMain.staComm.SaveAlarmToXML();
                MessageBox.Show("警报配置修改保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                button5.Enabled = false;
            }
            catch (Exception ex)
            {
                i++;
                MessageBox.Show("修改警报:R" + i.ToString() + ":" + ex.Message);
            }
        }
        private void button6_Click(object sender, EventArgs e)//保存运行规则
        {
            if (MessageBox.Show("是否保存配方运行规则的修改？", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }

            try
            {
                int B_23 = 1500;
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/PecipeRunRules";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    string sKey = item.GetAttribute("Key");
                    foreach (NumericUpDown nNum in ListPecipeRunRules)
                    {
                        if (nNum.Tag.ToString() == sKey)
                        {
                            item.SetAttribute("Value", nNum.Value.ToString());
                            if (sKey == "MMaxValue")
                            {
                                B_23 = (int)nNum.Value;
                            }
                        }
                    }
                }

                myxmldoc.Save(sXMLPath);

                //保存最大转速
                myxmldoc = new XmlDocument();
                string sPath = CProject.sPrjPath + "\\Project\\IO\\PortInf_Table.xml";
                myxmldoc.Load(sPath);

                xpath = "IO/VariableInf_Table/row";
                XmlNodeList cldNode = myxmldoc.SelectNodes(xpath);
                foreach (XmlElement node in cldNode)
                {
                    string sName = node.GetAttribute("Name");

                    if (sName == "B_23")
                    {
                        node.SetAttribute("IntTag9", B_23.ToString());
                        break;
                    }
                }
                foreach (CVar nVar in SPLC.StaDevice.ListDevVar)
                {

                    if (nVar.Name == "B_23")
                    {
                        nVar.IntTag[9] = B_23;
                        break;
                    }
                }
                myxmldoc.Save(sPath);
                MessageBox.Show("配方运行规则修改保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改配方运行规则:" + ex.Message);
            }
        }
        private void button7_Click(object sender, EventArgs e)//保存其他设置
        {

            if (MessageBox.Show("是否保存其他设置的修改？", "保存", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }

            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/Other";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);

                childNode.SetAttribute("OtherNum1", OtherNum1.Value.ToString());
                childNode.SetAttribute("OtherNum2", OtherNum2.Value.ToString());
                childNode.SetAttribute("OtherNum3", OtherNum3.Value.ToString());

                childNode.SetAttribute("OtherCBox1", OtherCBox1.Text);

                myxmldoc.Save(sXMLPath);

                if ((int)num1026.Value != SPLC.System[25]) SPLC.SendAODO("1026", (int)num1026.Value, "SY");//1026	R	进样室抽气压力
                if ((int)num1027.Value != SPLC.System[26]) SPLC.SendAODO("1027", (int)num1027.Value, "SY");//1027	R	进样室吹扫回冲压力设定
                if ((int)num1028.Value != SPLC.System[27]) SPLC.SendAODO("1028", (int)num1028.Value, "SY");//1028	R	进样室吹扫次数设定
                if ((int)num1030.Value != SPLC.System[29]) SPLC.SendAODO("1030", (int)num1030.Value, "SY");//1030	R	进样室Idle设定压力
                if ((int)num1031.Value != SPLC.System[30]) SPLC.SendAODO("1031", (int)num1031.Value, "SY");//1031	R	大气压强
                if (CBox1013.SelectedIndex != SPLC.System[12]) SPLC.SendAODO("1013", CBox1013.SelectedIndex, "SY");//1013	R	程序完成后运行Idle	0:H2 Idle  1:N2 Idle
                if (CBox1014.SelectedIndex != SPLC.System[13]) SPLC.SendAODO("1014", CBox1014.SelectedIndex, "SY");//1014	R	程序退出后后运行Idle	0:H2 Idle  1:N2 Idle
                if ((int)num1036.Value != SPLC.System[35]) SPLC.SendAODO("1036", (int)num1036.Value, "SY");//1036		Hot Idle伺服速度设定值

                MessageBox.Show("其他设置修改保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改其他设置:" + ex.Message);
            }
        }
        private void button8_Click(object sender, EventArgs e)//保存PID设置
        {
            frmC密码输入 nFrm = new frmC密码输入(Password);
            nFrm.ShowDialog();
            if (nFrm.DialogResult != DialogResult.OK)
            {
                return;
            }

            try
            {
                int[,] xy = new int[dGV8.RowCount, 7];
                for (int i = 0; i < dGV8.RowCount; i++)
                {
                    string str1 = dGV8.Rows[i].Cells[0].Value.ToString();
                    string[] str2 = str1.Substring(0, 4).Split('.');
                    xy[i, 0] = Convert.ToInt32(str2[0]) - 1;
                    xy[i, 1] = Convert.ToInt32(str2[1]) - 1;
                    for (int j = 1; j < dGV8.ColumnCount; j++)
                    {
                        xy[i, j + 1] = Convert.ToInt32(dGV8.Rows[i].Cells[j].Value.ToString());
                    }
                }
                for (int i = 0; i < dGV8.RowCount; i++)
                {
                    frmMain.nMainVar.ListPID[xy[i, 0]][xy[i, 1]].Min = xy[i, 2];
                    frmMain.nMainVar.ListPID[xy[i, 0]][xy[i, 1]].Max = xy[i, 3];
                    frmMain.nMainVar.ListPID[xy[i, 0]][xy[i, 1]].P = xy[i, 4];
                    frmMain.nMainVar.ListPID[xy[i, 0]][xy[i, 1]].I = xy[i, 5];
                    frmMain.nMainVar.ListPID[xy[i, 0]][xy[i, 1]].D = xy[i, 6];
                }
                FillGrid8();
                frmMain.nMainVar.SaveToXml();
                MessageBox.Show("保存PID设置成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dGV1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (Convert.ToBoolean(dGV1.Rows[e.RowIndex].Cells[0].Value))
                    dGV1.Rows[e.RowIndex].ReadOnly = true;
                else
                    dGV1.Rows[e.RowIndex].ReadOnly = false;
                dGV1.Rows[e.RowIndex].Cells[0].ReadOnly = false;
            }
        }
        private void dGV5_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            button5.Enabled = true;
            if (e.ColumnIndex == 2)
            {
                try
                {
                    if (dGV5.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(0, 1) == "1")
                        dGV5.Rows[e.RowIndex].Cells[2].Value = "1报警";
                    else if (dGV5.Rows[e.RowIndex].Cells[2].Value.ToString().Substring(0, 1) == "2")
                        dGV5.Rows[e.RowIndex].Cells[2].Value = "2警告";
                    else
                    {
                        dGV5.Rows[e.RowIndex].Cells[2].Value = "";
                        dGV5.Rows[e.RowIndex].Cells[3].Value = "";
                        dGV5.Rows[e.RowIndex].Cells[3].ReadOnly = true;
                        return;
                    }
                    dGV5.Rows[e.RowIndex].Cells[3].ReadOnly = false;
                    if (dGV5.Rows[e.RowIndex].Cells[3].Value == null || dGV5.Rows[e.RowIndex].Cells[3].Value.ToString() == "")
                        dGV5.Rows[e.RowIndex].Cells[3].Value = 1;
                }
                catch
                {
                    dGV5.Rows[e.RowIndex].Cells[2].Value = "";
                    dGV5.Rows[e.RowIndex].Cells[3].Value = "";
                    dGV5.Rows[e.RowIndex].Cells[3].ReadOnly = true;
                }
            }
        }

        private void butUpdateIdle_Click(object sender, EventArgs e)//刷新模拟量表
        {
            SPLC.SendReadIdle();
            UpdatePLCValue();
        }

        private void UpdatePLCValue()
        {
            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");

            for (int i = 0; i < dGV1.RowCount; i++)
            {
                string sName = dGV1.Rows[i].Cells[1].Value.ToString();
                CVar nVar = frmMain.staComm.GetVarByStaNameVarName("NJ301", sName);
                if (nVar == null)
                    continue;

                int k = 6;
                if (nVar.Name == "A_52" || nVar.Name == "A_53" || nVar.Name == "A_54" || nVar.Name == "A_55")
                {
                    dGV1.Rows[i].Cells[k++].Value = (((float)nVar.PLCValue[3] / 10)).ToString("f1");
                    dGV1.Rows[i].Cells[k++].Value = (((float)nVar.PLCValue[4] / 10)).ToString("f1");
                    dGV1.Rows[i].Cells[k++].Value = (((float)nVar.PLCValue[5] / 10)).ToString("f1");
                }
                else
                {
                    dGV1.Rows[i].Cells[k++].Value = nVar.PLCValue[3].ToString();
                    dGV1.Rows[i].Cells[k++].Value = nVar.PLCValue[4].ToString();
                    dGV1.Rows[i].Cells[k++].Value = nVar.PLCValue[5].ToString();
                }
            }

            num1022.Value =Math.Min(num1022.Maximum,  SPLC.System[21]);//源瓶吹扫次数
            num1023.Value =Math.Min(num1023.Maximum,  SPLC.System[22]);//源瓶吹扫时间s
            num1032.Value =Math.Min(num1032.Maximum,  SPLC.System[31]);//生长室吹扫次数
            num1033.Value =Math.Min(num1033.Maximum,  SPLC.System[32]);//生长室吹扫时间s

            num1026.Value =Math.Min(num1026.Maximum,  SPLC.System[25]);//进样室抽气压力(torr)
            num1027.Value =Math.Min(num1027.Maximum,  SPLC.System[26]);//进样室充气压力(sccm)
            num1028.Value =Math.Min(num1028.Maximum,  SPLC.System[27]);//进样室吹扫次数设定
            num1030.Value =Math.Min(num1030.Maximum,  SPLC.System[29]);//进样室Idle设定压力
            num1031.Value =Math.Min(num1031.Maximum,  SPLC.System[30]);//大气压值
            num1036.Value = Math.Min(num1036.Maximum, SPLC.System[35]);//Hot Idle伺服速度设定值

            CBox1013.SelectedIndex = Math.Min(SPLC.System[12], 1);//程序完成后运行
            CBox1014.SelectedIndex = Math.Min(SPLC.System[13], 1);//程序退出后运行
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (Bubbler nbub in frmMain.staComm.nBubbler.BubblerList)
                {
                    for (int i = 0; i < dGV2.RowCount; i++)
                    {
                        if( dGV2.Rows[i].Cells[1].Value== null)
                            continue;
                        string sName = dGV2.Rows[i].Cells[1].Value.ToString();
                        if (sName == nbub.Name)
                        {
                            dGV2.Rows[i].Cells[8].Value = nbub.BathTemp;	//	恒温槽温度(℃)
                            dGV2.Rows[i].Cells[8].Style.ForeColor = nbub.BathTempFill;	//	恒温槽温度(℃)
                            dGV2.Rows[i].Cells[10].Value = nbub.sUseWeight;	//	源瓶重量
                        }
                    }
                }
            }
            catch (Exception ee)
            { }
        }

        private void button5Update_Click(object sender, EventArgs e)
        {
            FillGrid5();
        }

        private void dGV2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 6)
                return;
            string sName =  dGV2.Rows[e.RowIndex].Cells[1].Value .ToString();
            Bubbler nbub = null;
            for (int i = 0; i < frmMain.staComm.nBubbler.BubblerList.Count; i++)
            {
                if (frmMain.staComm.nBubbler.BubblerList[i].Name == sName)
                {
                    nbub = frmMain.staComm.nBubbler.BubblerList[i];
                    break;
                }
            }
            if (nbub == null)
                return;
            try
            {
                int index = Convert.ToInt32(dGV2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                foreach (CStation nSta in frmMain.staComm.ListStation)
                {
                    if (nSta.Driver == "恒温槽")
                    {
                        if (nSta.Address64 == index)
                        {
                            nbub.RE215 = nSta.Name;
                            dGV2.Rows[e.RowIndex].Cells[6].Value = nbub.RE215;
                            return;
                        }
                    }
                }
                dGV2.Rows[e.RowIndex].Cells[6].Value = nbub.RE215;
            }
            catch (Exception ex)
            {
                dGV2.Rows[e.RowIndex].Cells[6].Value = nbub.RE215;
            }
        }

    }
}
