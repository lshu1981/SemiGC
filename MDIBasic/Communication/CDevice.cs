using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LSSCADA
{
    public enum EMsgPriority
    {
        NONE_LIST = -1,
        LO_LIST = 0,
        ME_LIST = 1,
        HI_LIST = 2,
    }

    public class CMessage
    {
        public int iStaIndex = -1;  //子站在端口中的序号
        public int iPortIndex = -1;            //所属端口序号 
        
        public String Driver = "";              //设备名称
        public int Message_No = -1;             //报文编号
        public String Description = "";         //报文描述
        public EMsgType MsgType = EMsgType.Msg_Call;//报文类型 1循环 2定时 3召唤
        public String Function = "";            //功能码
        public String Starting = "";            //起始地址
        public String Number = "";              //数量
        public int Delay_Time = 0;              //
        public int QuLen = 0;                   //请求报文长度
        public int ReLen = 0;                   //返回报文长度
        public EMsgPriority Priority = EMsgPriority.NONE_LIST;
        public SSend_Message sSendMsg = new SSend_Message();
        public Dictionary<string, CVar> ListMsgVar = new Dictionary<string, CVar>();
        //public List<CVar> ListMsgVar = new List<CVar>();
        public CMessage Clone()
        {
            CMessage obj = (CMessage)this.MemberwiseClone();
            obj.sSendMsg = sSendMsg.Clone();
            obj.ListMsgVar = new Dictionary<string, CVar>();
            foreach (KeyValuePair<string, CVar> kvp in ListMsgVar)
            {
                obj.ListMsgVar.Add(kvp.Key, kvp.Value.Clone()); 
            }
            return obj;
        }
        public bool LoadFromNode(XmlElement Node)
        {
            Description = Node.GetAttribute("Description");
            Driver = Node.GetAttribute("Driver");
            Message_No = Convert.ToInt32(Node.GetAttribute("Message_No"));
            MsgType = (EMsgType)Convert.ToInt32(Node.GetAttribute("Type"));

            Function = Node.GetAttribute("Function");
            Starting = Node.GetAttribute("Starting");
            Number = Node.GetAttribute("Number");

            Delay_Time = Convert.ToInt32(Node.GetAttribute("Delay_Time"));
            QuLen = Convert.ToInt32(Node.GetAttribute("QuLen"));
            ReLen = Convert.ToInt32(Node.GetAttribute("ReLen"));
            Priority = (EMsgPriority)Convert.ToInt32(Node.GetAttribute("Priority"));
            return true;
        }
    }

    public class CDevice
    {
        public UInt64 ID = 0;               //ID   工程中才有
        public String Driver = "";			//设备型号 ，设备的唯一标识
        public String Name = "";             //设备名称，相当于描述
        public String PortProtocol = "";			//设备类型即设备协议（区别端口协议）EPortProtocol中取值
        public String Specification = "";	//设备规格
        public String Vendor = "";			//生产厂商
        public String Tel_No = "";			//联系电话

        public int Request_Mes_Len = 0;		//设备请求报文长度
        public int Request_Cyc_Pos = 0;		//请求报文的循环起始位
        public int Request_Cyc_Len = 0;		//请求报文的循环长度
        public int Respond_Mes_Len = 0;		//设备响应报文长度
        public int Respond_Cyc_Pos = 0;		//响应报文的循环起始位
        public int Respond_Cyc_Len = 0;		//响应报文的循环长度

        public List<CVar> ListDevVar = new  List<CVar>();

        public List<CMessage> ListMsgLoop = new List<CMessage>();   //循环报文队列
        public List<CMessage> ListMsgTime = new List<CMessage>();   //定时报文队列
        public List<CMessage> ListMsgCall = new List<CMessage>();   //主叫报文队列
        public List<CMessage> ListMsgOther = new List<CMessage>();   //主叫报文队列

        public bool LoadFromNode(XmlElement Node)
        {
            //加载设备属性
            ID = UInt64.Parse(Node.GetAttribute("ID"));
            Driver = Node.GetAttribute("Driver");
            Name = Node.GetAttribute("Name");
            PortProtocol = Node.GetAttribute("PortProtocol");
            Specification = Node.GetAttribute("Specification");
            Vendor = Node.GetAttribute("Vendor");
            Tel_No = Node.GetAttribute("Tel_No");
            return true;
        }

        public CDevice Clone()
        {
            CDevice obj = (CDevice)this.MemberwiseClone();
            obj.ListDevVar = new List<CVar>();	

            obj.ListMsgLoop = new List<CMessage>();
            obj.ListMsgTime = new List<CMessage>();
            obj.ListMsgCall = new List<CMessage>();
            obj.ListMsgOther = new List<CMessage>();
            foreach (CVar nVar in ListDevVar) { obj.ListDevVar.Add(nVar.Clone()); }

            foreach (CMessage nMsg in ListMsgLoop) { obj.ListMsgLoop.Add(nMsg.Clone()); }
            foreach (CMessage nMsg in ListMsgTime) { obj.ListMsgTime.Add(nMsg.Clone()); }
            foreach (CMessage nMsg in ListMsgCall) { obj.ListMsgCall.Add(nMsg.Clone()); }
            foreach (CMessage nMsg in ListMsgOther) { obj.ListMsgOther.Add(nMsg.Clone()); }
            return obj;
        }

        public bool Load()
        {
            return true;
        }/*
        public bool IsInDevice(String _varName)
        {
            foreach (CDeviceVar deviceVar in DeviceVarList_3)
            {
                if (deviceVar.NodeName == _varName)
                    return true;
            }
            foreach (CDeviceVar deviceVar in DeviceVarList_0)
            {
                if (deviceVar.NodeName == _varName)
                    return true;
            }
            return false;
        }
        */
        //public    bool SaveToDB(int _iSign, DbCommand command) // 0 插入数据库 1 从数据库中删除
        public void Clear()
        {
            ListDevVar.Clear();
            ListMsgLoop.Clear();
            ListMsgTime.Clear();
            ListMsgOther.Clear();
            ListMsgCall.Clear();
        }

        public override String ToString()
        {
            return Driver;
        }
    }
}
