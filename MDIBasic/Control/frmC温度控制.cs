using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PublicDll;
using System.Xml;
using System.Collections;

namespace LSSCADA.Control
{
    public partial class frmC温度控制 : Form
    {
        string[] sListStaName = new string[] { "欧陆表Inner", "欧陆表Middle", "欧陆表Outer" };
        string[] sListVarName = new string[] { "PV", "I", "Mode", "PV_W", "I_W", "PID_P", "PID_I", "PID_D", "电流变化率", "电流上限" };
        string[] sListDF = new string[] { "f2", "f2", "D1", "f2", "f2", "D1", "D1", "D1", "f2", "f2" };

        string[] sCol2 = new string[] { "名称", "字地址", "值", "描述", "备注" };
        bool[] bCol2 = new bool[] { true, true, true, true, true, true };
        CProtcolFINS SPLC;

        public frmC温度控制()
        {
            InitializeComponent();
        }
        public frmC温度控制(Form _Owner, int iTop)
        {
            InitializeComponent();

            this.MdiParent = _Owner;
            this.DoubleBuffered = true;

            this.Top = iTop;
            CStation Sta = frmMain.staComm.GetStaByStaName("NJ301");
            SPLC = (CProtcolFINS)Sta;
            FillDGV(dGV1);

            FillGrid2();
        }

        private void FillDGV(DataGridView dGV)
        {
            CStation Sta = frmMain.staComm.GetStaByStaName(sListStaName[0]);
            CProtcolModbusTCP STCP = (CProtcolModbusTCP)Sta;
            dGV.RowCount = sListVarName.Length+3;
            for (int i = 0; i < sListVarName.Length; i++)
            {
                dGV.Rows[i].Cells[0].Value = sListVarName[i];
                dGV.Rows[i].Cells[1].Value = frmMain.staComm.GetVarByStaNameVarName(sListStaName[0], sListVarName[i]).ByteAddr;
                dGV.Rows[i].Cells[2].Value = frmMain.staComm.GetVarByStaNameVarName(sListStaName[0], sListVarName[i]).GetStringValue(sListDF[i]);
                dGV.Rows[i].Cells[3].Value = frmMain.staComm.GetVarByStaNameVarName(sListStaName[1], sListVarName[i]).GetStringValue(sListDF[i]);
                dGV.Rows[i].Cells[4].Value = frmMain.staComm.GetVarByStaNameVarName(sListStaName[2], sListVarName[i]).GetStringValue(sListDF[i]);
            }
            dGV.Rows[sListVarName.Length + 1].Cells[1].ReadOnly = false;
            dGV.Rows[sListVarName.Length + 2].Cells[1].ReadOnly = false;

            dGV.Rows[sListVarName.Length + 1].Cells[0].Value = "手写";
            dGV.Rows[sListVarName.Length + 2].Cells[0].Value = "手写";

            dGV.Rows[0].DefaultCellStyle.BackColor = Color.LightGray;
            dGV.Rows[1].DefaultCellStyle.BackColor = Color.LightGray;

            dGV.Rows[sListVarName.Length + 1].DefaultCellStyle.BackColor = Color.Lime;
            dGV.Rows[sListVarName.Length + 2].DefaultCellStyle.BackColor = Color.Lime;
        }

        private void FillGrid2()
        {
            CPublicDGV.InitializeDGV(dGV2, sCol2, bCol2, true);
            dGV2.Rows.Clear();

            ArrayList obj = new ArrayList();
            string[] str1;

            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sXMLPath = CProject.sPrjPath + "\\Project\\Setting.xml";
            myxmldoc.Load(sXMLPath);

            string xpath = "root/System";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                str1 = new string[dGV2.ColumnCount];
                str1[0] = item.GetAttribute("名称");
                str1[1] = item.GetAttribute("字地址");
                str1[3] = item.GetAttribute("描述");
                str1[4] = item.GetAttribute("备注");
                obj.Add(str1);
            }
            foreach (string[] rowArray in obj)
            {
                dGV2.Rows.Add(rowArray);
            }
        }

        private void dGV1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 2 &&  e.RowIndex >1)
            {
                DataGridView dGV = (DataGridView)sender;
                int iSta = e.ColumnIndex - 2;

                CStation Sta = frmMain.staComm.GetStaByStaName(sListStaName[iSta]);
                CProtcolModbusTCP STCP = (CProtcolModbusTCP)Sta;
                if (STCP == null)
                    return;
                try
                {
                    int iVar = e.RowIndex;
                    if (iVar < sListVarName.Length)
                    {
                        CVar nVar = frmMain.staComm.GetVarByStaNameVarName(sListStaName[iSta], sListVarName[iVar]);
                        float iValue = Convert.ToSingle(dGV1.Rows[iVar].Cells[2 + iSta].Value);
                        int  fSend =(int) Math.Round( iValue / nVar.RatioValue);
                        STCP.SendAODO(nVar.ByteAddr, fSend, 6);
                    }
                    else if (iVar == sListVarName.Length + 1 || iVar == sListVarName.Length + 2)
                    {
                        int iValue = Convert.ToInt32(dGV1.Rows[iVar].Cells[2+iSta].Value);
                        int iAdd = Convert.ToInt32(dGV.Rows[iVar].Cells[1].Value);
                        if(iAdd>0)
                            STCP.SendAODO(iAdd, iValue, 6);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                ReshDGV(dGV1);

                for (int i = 0; i <Math.Min( dGV2.RowCount,SPLC.System.Length); i++)
                {
                    dGV2.Rows[i].Cells[2].Value = SPLC.System[i];
                }
            }
            catch (Exception ee) { }
        }

        private void ReshDGV(DataGridView dGV)
        {
            for (int i = 0; i < sListVarName.Length; i++)
            {
                CVar nVar = frmMain.staComm.GetVarByStaNameVarName(sListStaName[0], sListVarName[i]);
                dGV.Rows[i].Cells[2].Value = nVar.GetStringValue(sListDF[i]);
                nVar = frmMain.staComm.GetVarByStaNameVarName(sListStaName[1], sListVarName[i]);
                dGV.Rows[i].Cells[3].Value = nVar.GetStringValue(sListDF[i]);
                nVar = frmMain.staComm.GetVarByStaNameVarName(sListStaName[2], sListVarName[i]);
                dGV.Rows[i].Cells[4].Value = nVar.GetStringValue(sListDF[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CVar nVar1 = frmMain.staComm.GetVarByStaNameVarName(sListStaName[0], "I_W");
            CVar nVar2 = frmMain.staComm.GetVarByStaNameVarName(sListStaName[1], "I_W");
            CVar nVar3 = frmMain.staComm.GetVarByStaNameVarName(sListStaName[2], "I_W");
            CStation Sta1 = frmMain.staComm.GetStaByStaName(sListStaName[0]);
            CProtcolModbusTCP STCP1 = (CProtcolModbusTCP)Sta1;
            CStation Sta2 = frmMain.staComm.GetStaByStaName(sListStaName[1]);
            CProtcolModbusTCP STCP2 = (CProtcolModbusTCP)Sta2;
            CStation Sta3 = frmMain.staComm.GetStaByStaName(sListStaName[2]);
            CProtcolModbusTCP STCP3 = (CProtcolModbusTCP)Sta3;

            double iValue1 = nVar1.GetDoubleValue() + Convert.ToDouble(textBox1.Text);
            int fSend1 = (int)Math.Round(iValue1 / nVar1.RatioValue);
            STCP1.SendAODO(nVar1.ByteAddr, (int)fSend1, 6);

            double iValue2 = nVar2.GetDoubleValue() + Convert.ToDouble(textBox2.Text);
            int fSend2 = (int)Math.Round(iValue2 / nVar2.RatioValue);
            STCP2.SendAODO(nVar3.ByteAddr, (int)fSend2, 6);

            double iValue3 = nVar3.GetDoubleValue() + Convert.ToDouble(textBox3.Text);
            int fSend3 = (int)Math.Round(iValue3 / nVar3.RatioValue);
            STCP3.SendAODO(nVar3.ByteAddr, (int)fSend3, 6);

        }

        private void frmC温度控制_Resize(object sender, EventArgs e)
        {
            Size aa = this.MdiParent.Size;
            this.Size = new System.Drawing.Size(aa.Width - 20, aa.Height - frmMain.iToolHeight);
        }

        #region 流量计控制
        /*int iDNNum = 0;
        List<CFlowmeter> ListDN = new List<CFlowmeter>();
        private void buttonPLC_Click(object sender, EventArgs e)
        {
            Button bSend = (Button)sender;
            switch (bSend.Text)
            {
                case "保存":
                    if (CheckIn())
                    {
                        SaveIn();
                        FillGrid2();
                        groupBox3.Enabled = true;
                        buttonPLC1.Enabled = false;
                    }
                    break;
                case "开始":
                    break;
                case "保持":
                    break;
                case "恢复":
                    break;
                case "停止":
                    break;
            }
        }
        private bool  CheckIn()
        {
            int i = 0, j = 0;
            try
            {
                for ( i = 0; i < dGV2.RowCount; i++)
                {
                    j = 0;
                    if (dGV2.Rows[i].Cells[j].Value != null)
                    {
                        string sName = dGV2.Rows[i].Cells[j].Value.ToString();
                        if (sName.Length > 0)
                        {
                            CVar nVar = frmMain.staComm.GetVarByStaNameVarName("NJ301", sName);
                            if (nVar == null)
                            {
                                MessageBox.Show("变量" + sName + "不存在", "错误");
                                return false;
                            }
                            j++;
                            int iMin =Convert.ToInt32( dGV2.Rows[i].Cells[j].Value.ToString());
                            j++;
                            int iMax = Convert.ToInt32(dGV2.Rows[i].Cells[j].Value.ToString());
                            j++;
                            int iSec = Convert.ToInt32(dGV2.Rows[i].Cells[j].Value.ToString());
                            if (iMin < 0 || iMin > nVar.IntTag[9])
                            {
                                MessageBox.Show("变量" + sName + "最小值超过容量", "错误");
                                return false;
                            }
                            if (iMax < 0 || iMax > nVar.IntTag[9])
                            {
                                MessageBox.Show("变量" + sName + "最大值超过容量", "错误");
                                return false;
                            }
                            if (iSec < 0 || iSec > 43200)
                            {
                                MessageBox.Show("时长超出范围", "错误");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("第" + i.ToString() + "行第" + j.ToString() + "列：" + ex.Message, "错误");
                return false;
            }
        }

        private bool SaveIn()
        {
            int i = 0, j = 0;
            try
            {
                ListDN.Clear();
                for (i = 0; i < dGV2.RowCount; i++)
                {
                    j = 0;
                    if (dGV2.Rows[i].Cells[j].Value != null)
                    {
                        string sName = dGV2.Rows[i].Cells[j].Value.ToString();
                        if (sName.Length > 0)
                        {
                            CVar nVar = frmMain.staComm.GetVarByStaNameVarName("NJ301", sName);
                            if (nVar == null)
                            {
                                MessageBox.Show("变量" + sName + "不存在", "错误");
                                return false;
                            }
                            j++;
                            int iMin = Convert.ToInt32(dGV2.Rows[i].Cells[j].Value.ToString());
                            j++;
                            int iMax = Convert.ToInt32(dGV2.Rows[i].Cells[j].Value.ToString());
                            j++;
                            int iSec = Convert.ToInt32(dGV2.Rows[i].Cells[j].Value.ToString());

                            CFlowmeter nFm = new CFlowmeter();
                            nFm.nVar = nVar;
                            nFm.ValueMin = iMin;
                            nFm.ValueMax = iMax;
                            nFm.iSec = iSec;
                            ListDN.Add(nFm);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("第" + i.ToString() + "行第" + j.ToString() + "列：" + ex.Message, "错误");
                return false;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void FillGrid2()
        {
            return;
            dGV2.Rows.Clear();
            dGV2.RowCount = ListDN.Count;
            int i = 0;
            foreach (CFlowmeter nFm in ListDN)
            {
                dGV2.Rows[i].Cells[0].Value = nFm.nVar.Name;
                dGV2.Rows[i].Cells[1].Value = nFm.ValueMin.ToString();
                dGV2.Rows[i].Cells[2].Value = nFm.ValueMax.ToString();
                dGV2.Rows[i].Cells[3].Value = nFm.iSec.ToString();
                dGV2.Rows[i].Cells[4].Value = nFm.nVar.PLCValue[2];
                dGV2.Rows[i].Cells[5].Value = nFm.nVar.GetStringValue(1, 5);
                i++;
            }
        }

        private void dGV2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            groupBox3.Enabled = false;
            buttonPLC1.Enabled = true;
        }*/
        #endregion
    }

    public class CFlowmeter
    {
        public CVar nVar;
        public int ValueMin = 0;
        public int ValueMax = 0;
        public int iSec = 0;
    }
}
