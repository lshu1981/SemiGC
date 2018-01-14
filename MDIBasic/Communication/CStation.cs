using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Data;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace LSSCADA
{
    public enum EStaType { Sta_OPC, Sta_CommonIO, Sta_Modbus_TCP, Sta_DTU, Sta_EtherNetIP, Sta_General }
    class CPriorityConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) { return null; }

        public CPriorityConverter()
        {
        }
    }

    [System.Security.Permissions.PermissionSetAttribute
    (System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute
    (System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]

    public class CExtendVar : CVar
    {
        public int IsSaveAsInitial = 0;//是否退出时保存

        public override bool LoadFromNode(XmlElement Node)
        {
            base.LoadFromNode(Node);
            Name = Node.GetAttribute("ParamInName");
            Description = Node.GetAttribute("ParamOutName");

            VarType = (EVarTYPE)Convert.ToInt32(Node.GetAttribute("ValType"));

            ExpressW.szExpress = Node.GetAttribute("Req_Formula");
            ExpressR.szExpress = Node.GetAttribute("Res_Formula");
            Default = Node.GetAttribute("DefaultValue");
            Length = Convert.ToInt32(Node.GetAttribute("ValLength"));
            IsSaveAsInitial = Convert.ToInt32(Node.GetAttribute("IsSaveAsInitial"));

            SaveToDB = Convert.ToInt32(Node.GetAttribute("IsSaveToDB"));
            return true;
        }
    }

    public class CStation
    {
        public CAlarm staAlarm;
        public string CommStateS
        {
            get
            {
                switch (CommStateE)
                {
                    case  ECommSatate.Unknown:
                        return "状态未知";
                    case ECommSatate.Failure:
                        return "通信故障";
                    case ECommSatate.Normal:
                        return "通信正常";
                    default:
                        return "其他";
                }
            }
        }
        public Color CommStateC
        {
            get
            {
                switch (CommStateE)
                {
                    case ECommSatate.Unknown:
                        return Color.White;
                    case ECommSatate.Failure:
                        return Color.Red;
                    case ECommSatate.Normal:
                        return Color.Lime;
                    default:
                        return Color.LightGray;
                }
            }
        }
        public ECommSatate CommStateE = ECommSatate.Unknown;
        public UInt64 ID = 0;
        public Int64 Address64 = 0; //子站地址、缺省值0、CP卡子站的地址为（3~125） 其它卡串口子站的地址为（0~255）
        public int Category = 0;

        public string Name = "";            //子站名称
        public string Address = "";         //子站地址
        public string Setting = "";         //子站配置 TCP用于IP地址和端口
        public string Description = "";     //子站描述
        public string Driver = "";          //
        public string PortName = "";
        public int RunState = 0;             //运行状态 0:卸载  1:运行
        public string RunStateS
        {
            get
            {
                if (RunState == 0)
                    return "卸载";
                else
                    return "运行";
            }
        }

        public int CommPriority;
        public UInt64 iSendByte;//子站发送字节数
        public UInt64 iRecvByte;//子站接收字节数
        public UInt64 iSendPackage;//子站发送包
        public UInt64 iRecvPackage;//子站接收包

        protected int iProtocolType = 0;			//协议类型  
        public EStaType staType = EStaType.Sta_Modbus_TCP;
        public CDevice StaDevice = new CDevice();		//设备
        public String PortID = "";			//所属端口			
        public int iPortIndex = -1;            //所属端口序号 
        public int iStaIndex = -1;//子站在端口中的序号

        public List<CVar> LstExtendVar = new List<CVar>(); //全局内存变量

        public int present_MsgFailRep = 0;                //报文重发次数
        public int present_DevFailNum = 0;             //报文失败次数
        public int present_DevFailCyc = 0;         //通讯等待周期

        public List<CMessage> ListMsgCyc = new List<CMessage>();   //循环报文队列
        public int present_Msg_Send = 0;            //当前发送的报文索引

        public List<Ccondition> ListCond = new List<Ccondition>();//条件报警

        public Modbus报文查看 frmShow;
        public bool bDebug = false;

        //public	CSOEDMSObjEdit SOEDMSObjEdit;		//请求编码转换公式
        public virtual bool LoadFromNode(XmlElement Node)
        {
            Name = Node.GetAttribute("Name");
            Address = Node.GetAttribute("Address");
            Description = Node.GetAttribute("Description");
            Setting = Node.GetAttribute("Setting");
            Driver = Node.GetAttribute("Driver");
            PortName = Node.GetAttribute("PortName");

            RunState = Convert.ToInt32(Node.GetAttribute("RunState"));
            return true;
        }

        public bool LoadAlarmFromNode(XmlElement Node)//读取报警配置
        {
            ListCond.Clear();
            string xpath = "Alarms/Conditions";
            XmlElement PrjNode = (XmlElement)Node.SelectSingleNode(xpath);
            foreach (XmlElement StaNode in PrjNode.ChildNodes)
            {
                Ccondition nCond = new Ccondition();
                nCond.LoadFromNode(StaNode);
                ListCond.Add(nCond);
            }
            xpath = "Alarms/Vars";
            PrjNode = (XmlElement)Node.SelectSingleNode(xpath);
            if (PrjNode != null)
            {
                foreach (XmlElement StaNode in PrjNode.ChildNodes)
                {
                    foreach (CVar nVar in StaDevice.ListDevVar)
                    {
                        if (nVar.Name == StaNode.GetAttribute("Name"))
                        {
                            nVar.nVarAlarm = new CVarAlarm();
                            nVar.nVarAlarm.staName = Name;
                            nVar.nVarAlarm.staAlarm = staAlarm;
                            nVar.nVarAlarm.Name = StaNode.GetAttribute("Name");
                            nVar.nVarAlarm.LoadFromNode(StaNode);
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 保存报警配置
        /// </summary>
        /// <param name="nNode"></param>
        /// <param name="MyXmlDoc"></param>
        public void SaveAlarmToNode(XmlElement nNode, XmlDocument MyXmlDoc)
        {
            XmlElement Alarms = MyXmlDoc.CreateElement("Alarms");
            XmlElement Conditions = MyXmlDoc.CreateElement("Conditions");
            Conditions.SetAttribute("count", ListCond.Count.ToString());
            foreach (Ccondition nCond in ListCond)
            {
                XmlElement condition = MyXmlDoc.CreateElement("condition");
                XmlElement Expression = MyXmlDoc.CreateElement("Expression");
                Expression.InnerText = nCond.Expression;
                XmlElement Description = MyXmlDoc.CreateElement("Description");
                Description.InnerText = nCond.Description;
                XmlElement Response = MyXmlDoc.CreateElement("Response");
                Response.InnerText = nCond.Response;
                XmlElement Priority = MyXmlDoc.CreateElement("Priority");
                Priority.InnerText = nCond.Priority.ToString();
                XmlElement SubConditions = MyXmlDoc.CreateElement("SubConditions");
                SubConditions.SetAttribute("count", nCond.ListSubCond.Count.ToString());
                foreach (Ccondition nCond1 in nCond.ListSubCond)
                {
                    XmlElement condition1 = MyXmlDoc.CreateElement("condition");
                    XmlElement Expression1 = MyXmlDoc.CreateElement("Expression");
                    Expression1.InnerText = nCond1.Expression;
                    XmlElement Description1 = MyXmlDoc.CreateElement("Description");
                    Description1.InnerText = nCond1.Description;
                    XmlElement Response1 = MyXmlDoc.CreateElement("Response");
                    Response1.InnerText = nCond1.Response;
                    condition1.AppendChild(Expression1);
                    condition1.AppendChild(Description1);
                    condition1.AppendChild(Response1);
                    SubConditions.AppendChild(condition1);
                }
                condition.AppendChild(Expression);
                condition.AppendChild(Description);
                condition.AppendChild(Response);
                condition.AppendChild(Priority);
                condition.AppendChild(SubConditions);
                Conditions.AppendChild(condition);
            }
            XmlElement FaultRecords = MyXmlDoc.CreateElement("FaultRecords");
            FaultRecords.SetAttribute("count", "0");
            Alarms.AppendChild(Conditions);
            Alarms.AppendChild(FaultRecords);
            XmlElement Vars = MyXmlDoc.CreateElement("Vars");
            Alarms.AppendChild(Vars);
            nNode.AppendChild(Alarms);
            nNode.SetAttribute("Name",Name);

            foreach (CVar nVar in StaDevice.ListDevVar)
            {
                if (nVar.nVarAlarm == null)
                    continue;
                if (nVar.nVarAlarm.GetNull()>0)
                {
                    XmlElement nVarNode = MyXmlDoc.CreateElement("Var");
                    nVarNode.SetAttribute("Name",nVar.Name);
                    nVar.nVarAlarm.SaveAlarmToNode(nVarNode, MyXmlDoc);
                    Vars.AppendChild(nVarNode);
                }
            }
        }
        //读取全局内存变量
        public bool GetExtendVar()
        {
            LstExtendVar.Clear();
            foreach (CVar nVar in StaDevice.ListDevVar)
            {
                if (nVar.DAType == EDAType.DA_YS)
                {
                    LstExtendVar.Add(nVar);
                }
            }
            return true;
            XmlDocument MyXmlDoc = new XmlDocument();
            String strpath = CProject.sPrjPath + "\\YssDatabase\\StationInf_Table.xml";
            MyXmlDoc.Load(strpath);

            string xpath = "StationInf_Table";
            XmlElement childNode = (XmlElement)MyXmlDoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                CExtendVar node = new CExtendVar();
                node.LoadFromNode(item);
                LstExtendVar.Add(node);
            }
            return true;
        }

        public virtual void MsgSplit() //组装所有读报文
        {
            
        }

        public virtual bool Open() //
        {
            return true;
        }
        public virtual bool Close() //组装所有读报文
        {
            return true;
        }
        public CMessage GetNextLoopMsg()
        {
            try
            {
                CMessage temSendMsg = new CMessage();
                if (ListMsgCyc.Count != 0)
                {
                    temSendMsg = (CMessage)ListMsgCyc[present_Msg_Send];
                    present_Msg_Send++;
                    if (present_Msg_Send >= ListMsgCyc.Count)
                        present_Msg_Send = 0;
                    return temSendMsg;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("CStation.GetYaoXinMessage" + e.Message);
            }
            return null;
        }
        public virtual bool PortDataRecv(CMessage sSend, CRecvBuff cRecv)
        {
            return true;
        }
        public virtual CMessage GetSendMsgImm()//获取立即发送的报文
        {
            return new CMessage();
        }
        public virtual CMessage GetSendMsgCyc()//获取循环发送的报文
        {
            return new CMessage();
        }
    }
}
