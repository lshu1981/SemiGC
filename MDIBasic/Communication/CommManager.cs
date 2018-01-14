using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Diagnostics;
using System.IO;
using PublicDll;
using System.Xml.Serialization;
using LSSCADA.Database;

namespace LSSCADA
{
    public class CCommManager
    {
        public LSDatabase nDatabase;
        public CAlarm staAlarm;
        public static bool Initialized;
        public List<CPort> ListPort = new List<CPort>(); //All Port
        public List<CDevice> ListDevice = new List<CDevice>(); //All Device
        public List<CStation> ListStation = new List<CStation>();
        public List<CVar> ListSaveToDB = new List<CVar>();
        public string sIOPath = "";

        public CReminderList nReminder = new CReminderList();
        public CBubblerList nBubbler = new CBubblerList();
        public int iOldSec = -1;

        private System.Timers.Timer CommTimer;// = new Timer(10000);

        public bool InitAllObjFromXML()
        {
            sIOPath = CProject.sPrjPath + "\\Project\\IO\\PortInf_Table.xml";
            string sPath = CProject.sPrjPath + "\\Project\\Reminder.xml";

            if (!GetPortsFromXML())//获取端口列表ListPort
                return false;
            //DebugTest(0);
            if (!GetStationsFromXML())//获取子站列表ListStation
                return false;
            //DebugTest(2);
            if (!GetDevicesFromXML())//获取设备类别ListDevice，同时获取设备所有属性：message,picture,varpic,change
                return false;
            //DebugTest(3);
            GetAlarmFromXML();//获取报警配置

            ListSaveToDB.Clear();
            foreach (CStation nSta in ListStation)
            {
                foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                {
                    nVar.StaName = nSta.Name;
                    if (nVar.SaveToDB > 0)
                    {
                        ListSaveToDB.Add(nVar);
                    }
                }
            }

            foreach (CStation nSta in ListStation)
            {
                foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                {
                    if (nVar.DAType == EDAType.DA_YS)
                    {
                        if (nVar.ExpressR != null)
                            nVar.ExpressR.GetDeviceVar(nSta.Name);
                        if (nVar.ExpressW != null)
                            nVar.ExpressW.GetDeviceVar(nSta.Name);
                    }
                }
            }
            nReminder.LoadXml();
            nBubbler.LoadXml();

            return true;
        }

        //初始化通讯信息
        public bool InitTongXun()
        {
            foreach (CPort nPort in ListPort)
            {
                Debug.WriteLine(nPort.PortName);
                nPort.Open();
            }
            //CreateHisDatabase();//创建历史数据库
            CommTimer = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为1000毫秒； 
            CommTimer.Elapsed += new System.Timers.ElapsedEventHandler(CommTimerCall);//到达时间的时候执行事件； 
            CommTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
            CommTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            return false;
        }

        //从PortInf_Table.xml中获取端口信息，存入ListPort
        public bool GetPortsFromXML()//获取端口信息
        {
            ListPort.Clear();
            XmlDocument MyXmlDoc = new XmlDocument();

            MyXmlDoc.Load(sIOPath);

            string xpath = "IO/PortInf_Table";
            XmlElement childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                string sPortType = item.GetAttribute("PortType");
                string sPortProtocol = item.GetAttribute("PortProtocol");
                CPort nPort = (CPort)CDAModule.CreatePort(sPortType, sPortProtocol);
                if (nPort == null)
                    continue;
                nPort.LoadFromNode(item);
                ListPort.Add(nPort);
            }
            return true;
        }

        //从StationInf_Table.xml中获取子站信息，再从sPrjName中获取对应的端口名称
        public bool GetStationsFromXML()//获取子站信息
        {
            XmlDocument MyXmlDoc = new XmlDocument();
            MyXmlDoc.Load(sIOPath);

            string xpath = "IO/StationInf_Table";
            XmlElement childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            foreach (XmlElement node in childNode.ChildNodes)
            {
                string sPortName = node.GetAttribute("PortName");
                foreach (CPort nPort in ListPort)
                {
                    if (sPortName == nPort.PortName)
                    {
                        CStation nSta = (CStation)CDAModule.CreateStation(nPort.PortType, nPort.PortProtocol);
                        if (nSta == null)
                            break;
                        nSta.LoadFromNode(node);
                        nSta.staAlarm = staAlarm;
                        ListStation.Add(nSta);
                        nPort.ListStation.Add(nSta);
                        nSta.iPortIndex = ListPort.IndexOf(nPort);
                        nSta.iStaIndex = nPort.ListStation.IndexOf(nSta);
                        break;
                    }
                }
            }
            return true;
        }

        public bool GetDevicesFromXML()//获取所有设备信息
        {
            ListDevice.Clear();
            XmlDocument MyXmlDoc = new XmlDocument();
            MyXmlDoc.Load(sIOPath);

            string xpath = "IO/DeviceInf_Table";
            XmlElement childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            foreach (XmlElement node in childNode.ChildNodes)
            {
                CDevice nDev = new CDevice();
                nDev.LoadFromNode(node);
                ListDevice.Add(nDev);
            }
            GetDevicesALLInfo();

            //对应子站设备
            foreach (CStation nSta in ListStation)
            {
                foreach (CDevice nDev in ListDevice)
                {
                    if (nSta.Driver == nDev.Driver)
                    {
                        nSta.StaDevice = (CDevice)nDev.Clone();
                        nSta.MsgSplit();
                        nSta.GetExtendVar();
                    }
                }
            }

            return true;
        }

        //从工程中加载所有设备所有属性信息，包括变量，报文，图案等
        public bool GetDevicesALLInfo()
        {
            //先加载设备信息
            foreach (CDevice cnode in ListDevice)
            {
                cnode.ListDevVar.Clear();
            }

            //加载设备变量(CDeviceVar,categogy = 0)
            XmlDocument MyXmlDoc = new XmlDocument();
            MyXmlDoc.Load(sIOPath);

            string xpath = "IO/VariableInf_Table";
            XmlElement childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                foreach (CDevice cnode in ListDevice)
                {
                    if (cnode.Driver == item.GetAttribute("Driver"))
                    {
                        CVar node = new CVar();
                        node.LoadFromNode(item);
                        cnode.ListDevVar.Add(node);
                    }
                }
            }

            //CMessage 请求读报文
            MyXmlDoc.Load(sIOPath);

            xpath = "IO/MessageInf_Table";
            childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                foreach (CDevice nDev in ListDevice)
                {
                    if (nDev.Driver == item.GetAttribute("Driver"))
                    {
                        CMessage nMsg = new CMessage();
                        nMsg.LoadFromNode(item);
                        switch (nMsg.MsgType)
                        {
                            case EMsgType.Msg_Loop:
                                nDev.ListMsgLoop.Add(nMsg);
                                break;
                            case EMsgType.Msg_Time:
                                nDev.ListMsgTime.Add(nMsg);
                                break;
                            case EMsgType.Msg_Call:
                                nDev.ListMsgCall.Add(nMsg);
                                break;
                            default:
                                nDev.ListMsgOther.Add(nMsg);
                                break;
                        }
                    }
                }
            }
            return true;
        }

        public bool GetAlarmFromXML()//获取报警配置
        {
            XmlDocument MyXmlDoc = new XmlDocument();
            MyXmlDoc.Load(CProject.sPrjPath + @"\Project\IO\Alarm.xml");

            string xpath = "System/Project";

            XmlElement childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            if (childNode == null)
                return false;
            foreach (XmlElement StaNode in childNode.ChildNodes)
            {
                if (StaNode.Name != "Station")
                    continue;
                foreach (CStation nSta in ListStation)
                {
                    if (nSta.Name == StaNode.GetAttribute("Name"))
                    {
                        nSta.LoadAlarmFromNode(StaNode);
                        break;
                    }
                }
            }

            return true;
        }

        //初始化通讯信息
        public bool Close()
        {
            foreach (CPort nPort in ListPort)
            {
                nPort.Close();
            }
            return false;
        }

        public bool SaveAlarmToXML()//保存报警配置
        {
            XmlDocument MyXmlDoc = new XmlDocument();
            string sPath = CProject.sPrjPath + @"\Project\IO\Alarm.xml";
            MyXmlDoc.Load(sPath);

            string xpath = "System/Project";

            XmlElement childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            childNode.RemoveAll();

            XmlElement Alarms0 = MyXmlDoc.CreateElement("Alarms");
            XmlElement Conditions0 = MyXmlDoc.CreateElement("Conditions");
            XmlElement PDRs = MyXmlDoc.CreateElement("PDRs");
            Conditions0.SetAttribute("count", "0");
            PDRs.SetAttribute("count", "0");
            Alarms0.AppendChild(Conditions0);
            Alarms0.AppendChild(PDRs);
            childNode.AppendChild(Alarms0);
            childNode.SetAttribute("Name", frmMain.staPrj.Name);

            foreach (CStation nSta in ListStation)
            {
                int k = 0;
                if (nSta.ListCond.Count > 0)
                    k++;
                else
                {
                    foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                    {
                        if (nVar.nVarAlarm == null)
                            continue;
                        if (nVar.nVarAlarm.GetNull()>0)
                        {
                            k++;
                            break;
                        }
                    }
                }
                if (k == 0)
                    continue;
                XmlElement nNode = MyXmlDoc.CreateElement("Station");
                nSta.SaveAlarmToNode(nNode, MyXmlDoc);
                childNode.AppendChild(nNode);
            }

            MyXmlDoc.Save(sPath);
            return true;
        }

        //定时器
        public void CommTimerCall(object source, System.Timers.ElapsedEventArgs e)
        {
            int iSec = DateTime.Now.Second;
            DateTime DTNow = DateTime.Now;
            if (iOldSec != iSec)
            {
                iOldSec = iSec;
                foreach (CStation nSta in ListStation)
                {
                    foreach (CVar nVar in nSta.LstExtendVar)
                    {
                        if (nVar.ExpressR != null)
                        {
                            nVar.SetExtendVarValue();
                        }
                    }
                    foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                    {
                        if (nVar.SaveToDB > 0)
                        {
                            SVarValue newValue = new SVarValue(nVar.VarNewValue.Value_d);
                            newValue.Date_Time = DTNow;
                            nVar.ListValue.Add(newValue);
                            while (nVar.ListValue.Count > CCONST.VarNumMax)
                            {
                                nVar.ListValue.RemoveAt(0);
                            }
                        }
                    }
                }
                nDatabase.CommTimerCall();
            }
        }

        //通过子站名获取子站网络状态
        public ECommSatate GetNetState(string szSta)
        {
            foreach (CStation nSta in ListStation)
            {
                if (szSta == nSta.Name)
                {
                    return nSta.CommStateE;
                }
            }
            return ECommSatate.Unknown;
        }
        //通过子站名和变量名获取变量引用
        public CVar GetVarByStaNameVarName(string StaName, string VarName)
        {
            foreach (CStation nSta in ListStation)
            {
                if (nSta.Name == StaName)
                {
                    foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                    {
                        if (nVar.Name == VarName)
                        {
                            return nVar;
                        }
                    }
                    break;
                }
            }
            return null;
        }
        //通过子站名获取子站引用
        public CStation GetStaByStaName(string StaName)
        {
            foreach (CStation nSta in ListStation)
            {
                if (nSta.Name == StaName)
                {
                    return nSta;
                }
            }
            return null;
        }

        //通过子站名和变量名获取变量值
        public string GetVarValueByStaNameVarName(string StaName, string VarName)
        {
            foreach (CStation nSta in ListStation)
            {
                if (nSta.Name == StaName)
                {
                    foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                    {
                        if (nVar.Name == VarName)
                        {
                            return nVar.GetStringValue(1);// +string.Format(":{0},{1},{2}", nSta.present_MsgFailRep, nSta.present_DevFailNum, nSta.present_DevFailCyc); ; ;
                        }
                    }
                    break;
                }
            }
            return "";
        }
    }
}

