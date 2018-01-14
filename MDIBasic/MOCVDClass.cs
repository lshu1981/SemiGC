using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Drawing;

namespace LSSCADA
{
    public class CReminderList
    {
        public Dictionary<int, string> ManualActList = new Dictionary<int, string>();
        public Dictionary<int, string> Alarm2List = new Dictionary<int, string>();
        public Dictionary<int, Reminder> ReminderList = new Dictionary<int, Reminder>();
        public CReminderList() { }

        public void LoadXml()
        {
            ManualActList.Clear();
            Alarm2List.Clear();
            ReminderList.Clear();
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sXMLPath = CProject.sPrjPath + "\\Project\\Reminder.xml";
            myxmldoc.Load(sXMLPath);

            string xpath = "Root/ReminderList";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                Reminder nSet = new Reminder();
                nSet.Index = Convert.ToInt32(item.GetAttribute("Index"));
                nSet.Desc = item.GetAttribute("Desc");
                nSet.Type = Convert.ToInt32(item.GetAttribute("Type"));
                nSet.Add = item.GetAttribute("Add");
                nSet.Value = Convert.ToInt32(item.GetAttribute("Value"));
                ReminderList.Add(nSet.Index, nSet);
            }
            xpath = "Root/ManualActList";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                int iKey = Convert.ToInt32(item.GetAttribute("Key"));
                string sValue = item.GetAttribute("Value");
                ManualActList.Add(iKey, sValue);
            }
            xpath = "Root/Alarm2";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                int iKey = Convert.ToInt32(item.GetAttribute("Index"));
                string sValue = item.GetAttribute("Desc");
                Alarm2List.Add(iKey, sValue);
            }
        }
    }

    public class Reminder
    {
        public int Index = 0;
        public string Desc = "";
        public int Type = 0;//默认1：界面显示，无需确认  2
        public string Add = "";
        public int Value = 0;
        public string[] AddList
        {
            get
            {
                string[] str1 = Add.Split(';');
                return str1;
            }
        }
    }

    public class CBubblerList
    {
        public List<Bubbler> BubblerList = new List<Bubbler>();
        Single TempRange = 0;//恒温槽温度偏差

        public int selBubbler = -1;//源瓶维护界面所选源瓶编号

        public CBubblerList() { }
        public void LoadXml()
        {
            BubblerList.Clear();
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sXMLPath = CProject.sPrjPath + "\\Project\\Setting.xml";
            myxmldoc.Load(sXMLPath);

            string xpath = "root/Bubbler";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            TempRange = Convert.ToSingle(childNode.GetAttribute("TempRange"));
            foreach (XmlElement item in childNode.ChildNodes)
            {
                Bubbler nSet =new Bubbler();
                nSet.Name = item.GetAttribute("Name");
                nSet.Type = item.GetAttribute("Type");
                nSet.ID = Convert.ToInt32(item.GetAttribute("ID"));
                nSet.PC = Convert.ToInt32(item.GetAttribute("PC"));
                nSet.Ramp = Convert.ToSingle(item.GetAttribute("Ramp"));
                nSet.MinVentTime = Convert.ToInt32(item.GetAttribute("MinVentTime"));
                nSet.RE215 = item.GetAttribute("RE215");
                nSet.DesiredTemp = Convert.ToSingle(item.GetAttribute("DesiredTemp"));
                nSet.TempRange = TempRange;
                nSet.wins = item.GetAttribute("wins");

                nSet.Weight = Convert.ToSingle(item.GetAttribute("Weight"));
                nSet.UseWeight = Convert.ToSingle(item.GetAttribute("UseWeight"));
                nSet.UseWeightOld = nSet.UseWeight;
                nSet.MFC1 = item.GetAttribute("MFC1");
                nSet.P = item.GetAttribute("P");
                nSet.Through = item.GetAttribute("Through");

                nSet.MW = Convert.ToSingle(item.GetAttribute("MW"));
                nSet.sPMO =item.GetAttribute("PMO");

                nSet.nVarMFC = frmMain.staComm.GetVarByStaNameVarName("NJ301", nSet.MFC1);
                nSet.nVarP = frmMain.staComm.GetVarByStaNameVarName("NJ301", nSet.P);
                nSet.nVarThrough = frmMain.staComm.GetVarByStaNameVarName("NJ301", nSet.Through);
                BubblerList.Add(nSet);
            }
        }

        public void CalcUseWeight()
        {
            foreach (Bubbler nBub in BubblerList)
            {
                nBub.CalcUseWeight();
            }
        }

        public void SaveBubblerUseWeight()
        {
            try
            {
                bool bSave = false;
                foreach (Bubbler nBub in BubblerList)
                {
                    if (nBub.UseWeight != nBub.UseWeightOld)
                    {
                        bSave = true;
                        break;
                    }
                }
                if (!bSave)
                    return ;

                XmlDocument myxmldoc = new XmlDocument();
                string sPath = CProject.sPrjPath + "\\Project\\Setting.xml";
                myxmldoc.Load(sPath);

                string xpath = "root/Bubbler";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement node in childNode.ChildNodes)
                {
                    string sName = node.GetAttribute("Name");

                    foreach (Bubbler nBub in BubblerList)
                    {
                        if (sName == nBub.Name)
                        {
                            if (nBub.UseWeight != nBub.UseWeightOld)
                            {
                                node.SetAttribute("UseWeight", nBub.UseWeight.ToString());
                            }
                            break;
                        }
                    }
                }
                myxmldoc.Save(sPath);
                return ;
            }
            catch (Exception ex)
            {
                return ;
            }
        }
    }

    public class Bubbler
    {
        public int ID = 0;//	源瓶编号
        public string Name = "";//	源瓶名称
        public string Type = "";//	种类
        public int PC = 0;//	PC(torr)
        public Single Ramp = 0;//	改变速率(s)
        public int MinVentTime = 0;//	最小Vent时间
        public Single TempRange = 0;//恒温槽温度偏差
        public Single DesiredTemp = 0;//设定温度(℃)
        public Single BathTemp//	恒温槽温度(℃)
        {
            get
            {
                if (nVarT != null)
                    return (Single)nVarT.GetDoubleValue();
                else
                    return 0;
            }
        }
        public Color BathTempFill
        {
            get
            {
                if (Math.Abs(BathTemp - DesiredTemp) > Math.Abs(TempRange))
                    return Color.Red;
                else
                    return Color.Black;
            }
        }
        public string wins = "源瓶维护01.xml";
        public double Weight = 0;//源瓶重量
        public string sWeight
        {
            get
            {
                return Weight.ToString("f2");
            }
        }
        public double UseWeightOld = 0;//源瓶使用重量
        public double UseWeight = 0;//源瓶使用重量
        public string sUseWeight
        {
            get
            {
                return (Weight - UseWeight).ToString("f2");
            }
        }
        public string MFC1 = "";
        public string P = "";
        public string Through = "";
        public double MW = 0;
        public string sPMO = "";
        public double PMO
        {
            get
            {
                try
                {
                    return Convert.ToDouble(sPMO);
                }
                catch (Exception)
                {
                    return 1;
                }
            }
        }

        public string RE215//恒温槽读取温度
        {
            set
            {
                string[] str2 = value.Split('.');
                if (str2.Length ==1)
                {
                    nVarT = frmMain.staComm.GetVarByStaNameVarName(str2[0], sVarT);
                    return;
                }
                else if (str2.Length > 1)
                {
                    nVarT = frmMain.staComm.GetVarByStaNameVarName(str2[0], str2[1]);
                    sVarT = str2[1];
                }
                else
                {
                    nVarT = null;
                }
            }
            get
            {
                if (nVarT != null)
                    return nVarT.StaName;
                else
                    return "";
            }
        }
        public string sVarT;
        public CVar nVarT;//恒温槽读取温度

        public CVar nVarMFC;
        public CVar nVarP;
        public CVar nVarThrough;
        
        public Bubbler() { }

        public void CalcUseWeight()
        {
            try
            {
                if (nVarThrough.GetBoolValue())
                {
                    double n_MO = (nVarMFC.GetDoubleValue() * PMO) / (22414 * (nVarP.GetDoubleValue() - PMO));
                    UseWeight += n_MO * MW / 60;
                }
            }
            catch (Exception ex)
            {
 
            }
        }
    }

    public class CPublicVar
    {
        public string[] sListOLName = new string[] { "欧陆表Inner", "欧陆表Middle", "欧陆表Outer" };
        CVar[] nVarPV = new CVar[3];    //温度
        CVar[] nVarMode = new CVar[3];  //控温模式
        CVar[] nVarPVW = new CVar[3];   //温度写
        CVar[] nVarI = new CVar[3];     //电流
        CVar[] nVarIW = new CVar[3];    //欧陆表电流写变量

        CVar[] nVarU = new CVar[3];     //电压

        CVar[] nVarPLCI = new CVar[8];  //往PLC写电流变量
        CVar[,] nVarPID = new CVar[3, 3];

        CVar nVarDI187;
        public CProtcolModbusTCP[] nOLSta = new CProtcolModbusTCP[3]; 
        public CVar CB23;               //电机转速变量
        public string[] sOLPV
        {
            get
            {
                try
                {
                    string[] str1 = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        str1[i] = nVarPV[i].GetStringValue("f1");
                    }
                    return str1;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public int[] sOLPVW
        {
            get
            {
                int[] str1 = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    str1[i] = (int)nVarPVW[i].GetInt64Value();
                    if (str1[i] < 450)
                        str1[i] = 450;
                    if (str1[i] > 1250)
                        str1[i] = 1250;
                }
                return str1;
            }
        }
        public Double[] sOLIW
        {
            get
            {
                Double[] str1 = new Double[3];
                for (int i = 0; i < 3; i++)
                {
                    str1[i] = nVarIW[i].GetDoubleValue();
                }
                return str1;
            }
        }

        public Double[] U
        {
            get
            {
                Double[] str1 = new Double[3];
                for (int i = 0; i < 3; i++)
                {
                    str1[i] = nVarU[i].GetDoubleValue();
                }
                return str1;
            }
        }

        public Double[] I
        {
            get
            {
                Double[] str1 = new Double[3];
                for (int i = 0; i < 3; i++)
                {
                    str1[i] = nVarI[i].GetDoubleValue();
                }
                return str1;
            }
        }
        public Double[] W
        {
            get
            {
                Double[] str1 = new Double[3];
                for (int i = 0; i < 3; i++)
                {
                    str1[i] = U[i] * I[i];
                }
                return str1;
            }
        }

        public byte[] PLCI
        {
            get
            {
                byte[] str1 = new byte[16];
                for (int i = 0; i < 8; i++)
                {
                    int kk = (int)(nVarPLCI[i].GetDoubleValue() * 10);
                    str1[2 * i + 0] =(byte)( kk >> 8);
                    str1[2 * i + 1] = (byte)kk;
                }
                return str1;
            }
        }

        public int[] sOLMode
        {
            get
            {
                int[] str1 = new int[3];
                for (int i = 0; i < 3; i++)
                {
                        str1[i] =(int) nVarMode[i].GetInt64Value();
                }
                return str1;
            }
        }
        public bool bDI187
        {
            get
            {
                return !nVarDI187.GetBoolValue();
            }
        }

        public double dLayTolTime = 0;                  //当前层总时间
        public double dLayRunTime = 0;                  //当前层总时间
        public bool bSendToOL = false;                  //
        public bool[] bAutoLock = new bool[] { false, false, false };
        public double[] dLayAdd = new double[3] { 0, 0, 0, };//当前层每秒增量

        public int[] SetToOLValue = new int[] { 0, 0, 0, 0, 0 ,0};//当前层设置参数

        public int[] SetToOLValueNext = new int[] { 0, 0, 0, 0, 0, 0 };//下一层层设置参数

        public List<PID>[] ListPID = new List<PID>[3];
        public CPublicVar()
        {
            CB23 = frmMain.staComm.GetVarByStaNameVarName("NJ301", "B_23");
            nVarDI187 = frmMain.staComm.GetVarByStaNameVarName("NJ301", "DI187");
            nVarU[0] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "AI01");
            nVarU[1] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "AI02");
            nVarU[2] = frmMain.staComm.GetVarByStaNameVarName("NJ301", "AI06");
            for (int i = 0; i < 3; i++)
            {
                nVarPV[i] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "PV");
                nVarMode[i] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "Mode");
                nVarPVW[i] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "PV_W");
                nVarI[i] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "I");
                nVarIW[i] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "I_W");
                nVarPID[i, 0] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "PID_P");
                nVarPID[i, 1] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "PID_I");
                nVarPID[i, 2] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[i], "PID_D");
                CStation Sta = frmMain.staComm.GetStaByStaName(sListOLName[i]);
                nOLSta[i] = (CProtcolModbusTCP)Sta;
            }

            nVarPLCI[0] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[0], "I");
            nVarPLCI[1] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[1], "I_1");
            nVarPLCI[2] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[1], "I_3");
            nVarPLCI[3] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[1], "I_4");
            nVarPLCI[4] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[1], "I_6");
            nVarPLCI[5] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[2], "I_1");
            nVarPLCI[6] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[2], "I_3");
            nVarPLCI[7] = frmMain.staComm.GetVarByStaNameVarName(sListOLName[2], "I_4");

            LoadXml();
        }

        public void LoadXml()
        {
           ListPID[0] = new List<PID>();
           ListPID[1] = new List<PID>();
           ListPID[2] = new List<PID>();
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sXMLPath = CProject.sPrjPath + "\\Project\\Setting.xml";
            myxmldoc.Load(sXMLPath);

            string xpath = "root/PID";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                PID nSet = new PID();
                nSet.Min = Convert.ToInt32(item.GetAttribute("Min"));
                nSet.Max = Convert.ToInt32(item.GetAttribute("Max"));
                nSet.P = Convert.ToInt32(item.GetAttribute("P"));
                nSet.I = Convert.ToInt32(item.GetAttribute("I"));
                nSet.D = Convert.ToInt32(item.GetAttribute("D"));
                int i = Convert.ToInt32(item.GetAttribute("OLType"));
                ListPID[i].Add(nSet);
            }
        }

        public void SaveToXml()
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sXMLPath = CProject.sPrjPath + "\\Project\\Setting.xml";
            myxmldoc.Load(sXMLPath);

            string xpath = "root/PID";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            while (childNode.ChildNodes.Count > 0)
            {
                childNode.RemoveChild(childNode.FirstChild);
            }

            for (int i = 0; i < 3;i++ )
            {
                foreach (PID nPID in ListPID[i])
                {
                    XmlElement row = myxmldoc.CreateElement("row");
                    row.SetAttribute("OLType", i.ToString());
                    row.SetAttribute("Min", nPID.Min.ToString());
                    row.SetAttribute("Max", nPID.Max.ToString());
                    row.SetAttribute("P", nPID.P.ToString());
                    row.SetAttribute("I", nPID.I.ToString());
                    row.SetAttribute("D", nPID.D.ToString());
                    childNode.AppendChild(row);
                }
            }
            myxmldoc.Save(sXMLPath);
        }

        public void GotoNextLay(double dTol)//进入下一层温控程序
        {
            dLayTolTime = dTol;
            dLayRunTime = 0;
            SetToOLValue = SetToOLValueNext;
            bAutoLock = new bool[] { false, false, false };
            for (int i = 0; i < 3; i++)
            {
                if (SetToOLValue[0] == 1)//渐变
                {
                    if (SetToOLValue[1] == 1)//电流控制模式
                    {
                        dLayAdd[i] = (SetToOLValue[2 + i] - sOLIW[i]) / (dLayTolTime-1);
                    }
                    else//温度控制模式
                    {
                        dLayAdd[i] = (SetToOLValue[2 + i] -Convert.ToDouble( sOLPV[i])) / (dLayTolTime-1);
                    }
                }
                else//跳变
                {
                    dLayAdd[i] = 0;
                }
            }
        }

        public void ManualSetPV(int index,int PValue,int iSys)//手动设定温度
        {
            if (SetToOLValue[1] != 1 || iSys !=7)//温度模式
            {
                bAutoLock[index] = true;
                nOLSta[index].SendAODO(nVarPVW[index].ByteAddr, (int)(PValue / nVarPVW[index].RatioValue), 6);
            }
        }

        public void InitTo0()//
        {
            for (int i = 0; i < 3; i++)
            {
                bAutoLock[i] = true;
                nOLSta[i].SendAODO(nVarMode[i].ByteAddr, 1, 6);
                nOLSta[i].SendAODO(nVarIW[i].ByteAddr, 0, 6);
                nOLSta[i].SendAODO(nVarPVW[i].ByteAddr, 0, 6);
            }
        }

        public void SendPV2OL(double iTime)//下发温度设定到欧陆表
        {
            iTime++;
            if (iTime <= dLayRunTime)
                return;
            if (iTime <= 0 || iTime > dLayTolTime)
                return;
            dLayRunTime = iTime;
            for (int i = 0; i < 3; i++)
            {
                if (bAutoLock[i])
                    continue;
                if (SetToOLValue[1] != nVarMode[i].GetInt64Value())//检查温控模式
                    nOLSta[i].SendAODO(nVarMode[i].ByteAddr, SetToOLValue[1], 6);
                if (SetToOLValue[0] == 1)//渐变
                {
                    if (SetToOLValue[1] == 1)//电流控制模式
                    {
                        double iD = (SetToOLValue[i + 2] - dLayAdd[i] * (dLayTolTime - iTime)) / nVarIW[i].RatioValue;
                        nOLSta[i].SendAODO(nVarIW[i].ByteAddr, (int)Math.Round(iD), 6);
                    }
                    else//温度控制模式
                    {
                        double iD = (SetToOLValue[i + 2] - dLayAdd[i] * (dLayTolTime - iTime)) / nVarPVW[i].RatioValue;
                        nOLSta[i].SendAODO(nVarPVW[i].ByteAddr, (int)Math.Round(iD), 6);
                        CheckPID(i,(int)(SetToOLValue[i + 2] - dLayAdd[i] * (dLayTolTime - iTime)));
                    }
                }
                else//跳变
                {
                    if (SetToOLValue[1] == 1)//电流控制模式
                    {
                        if (SetToOLValue[2 + i] != sOLIW[i])//设定值与目标值不等
                            nOLSta[i].SendAODO(nVarIW[i].ByteAddr, (int)Math.Round((SetToOLValue[2 + i] / nVarIW[i].RatioValue)), 6);
                    }
                    else//温度控制模式
                    {
                        if (SetToOLValue[2 + i] != sOLPVW[i])//设定值与目标值不等
                            nOLSta[i].SendAODO(nVarPVW[i].ByteAddr, (int)Math.Round((SetToOLValue[2 + i] / nVarPVW[i].RatioValue)), 6);
                        CheckPID(i, sOLPVW[i]);
                    }
                }
            }
        }

        private void CheckPID(int k, int PV)
        {
            for (int i = 0; i < ListPID[k].Count; i++)
            {
                if (PV >= ListPID[k][i].Min && PV < ListPID[k][i].Max)
                {
                    if (GetPID(k, 0) != ListPID[k][i].P)
                        nOLSta[k].SendAODO(nVarPID[k, 0].ByteAddr, ListPID[k][i].P, 6);
                    if (GetPID(k, 1) != ListPID[k][i].I)
                        nOLSta[k].SendAODO(nVarPID[k, 1].ByteAddr, ListPID[k][i].I, 6);
                    if (GetPID(k, 2) != ListPID[k][i].D)
                        nOLSta[k].SendAODO(nVarPID[k, 2].ByteAddr, ListPID[k][i].D, 6);
                    return;
                }
            }
        }

        private int GetPID(int i, int j)
        {
            try
            {
                return (int)nVarPID[i, j].GetInt64Value();
            }
            catch 
            {
                return 0;
            }
        }
    }

    public class PID
    {
        public int Min = 0;
        public int Max = 0;
        public int P = 0;
        public int I = 0;
        public int D = 0;
    }
}
