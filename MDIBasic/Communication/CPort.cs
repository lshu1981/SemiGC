using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.Threading;

namespace LSSCADA
{
    [Serializable]
    public class CPort
    {
        public List<CStation> ListStation = new List<CStation>();     //通道下所挂子站列表

        public Boolean bOpen = false;       //端口打开标识
        public CRecvBuff cRecvBuff = new CRecvBuff();//接收到的数据

        public int PresentSendStaIndex = -1;   //上次发送的子站序号
        public CMessage LastSendMsg = new CMessage();   //上次发送的报文

        Int64 ID = 0;
        public String PortName = "";         //端口名称，唯一标识
        public String PortDescript = "";     //端口描述
        public String PortAddress = "";     //端口地址，用于数据发布时用
        public String PortType = "";        //版卡型号
        public String PortConfig1 = "";	    //主端口设置
        public String PortConfig2 = "";	    //备用端口设置
        public String PortProtocol = "";    //端口协议
        public String PortProtocolConfig = "";//端口协议设置

        public int DelayTime = 0;           //发送后等待延时

        public int MsgFailRep = 2;          //同一报文重发次数
        public int DevFailNum=3;            //设备不同报文失败次数
        public int DevFailCyc=10;           //设备通讯故障后等待周期

        public bool BBroadcastTime = false; //是否广播对时
        public int TimeCyc = 0;             //对时周期

        public virtual bool LoadFromNode(XmlElement Node)
        {
            //加载设备属性
            PortName = Node.GetAttribute("PortName");
            PortDescript = Node.GetAttribute("PortDescript");
            PortAddress = Node.GetAttribute("PortAddress");
            PortType = Node.GetAttribute("PortType");
            PortConfig1 = Node.GetAttribute("PortConfig1");
            PortConfig2 = Node.GetAttribute("PortConfig2");
            PortProtocol = Node.GetAttribute("PortProtocol");
            PortProtocolConfig = Node.GetAttribute("PortProtocolConfig");

            DelayTime = Convert.ToInt32(Node.GetAttribute("DelayTime"));
            MsgFailRep = Convert.ToInt32(Node.GetAttribute("MsgFailRep"));
            DevFailNum = Convert.ToInt32(Node.GetAttribute("DevFailNum"));
            DevFailCyc = Convert.ToInt32(Node.GetAttribute("DevFailCyc"));
            BBroadcastTime = Convert.ToBoolean(Node.GetAttribute("BBroadcastTime"));
            TimeCyc = Convert.ToInt32(Node.GetAttribute("TimeCyc"));

            return true;
        }

        // 实现IPort接口
        public virtual bool InitImmMsgList() { return false; }//初始化全查询
        public virtual bool Open() { return true; }
        public virtual bool Close() { return true; }
        public virtual bool Read() { return false; }
        public virtual bool Write(SSend_Message sSend) { return false; }
        public virtual bool GetStatus() { return false; }
        public virtual bool GetDRNHandle() { return false; }
        public virtual bool SetParam() { return false; }
        public virtual bool GetDescriptionString() { return false; }

        public String GetPortProtocolName(EPortProtocol _PortProtocol)
        {
            switch (_PortProtocol)
            {
                case EPortProtocol.NoProtocol: return "CommonIO";
                case EPortProtocol.ProfibusDP: return "Profibus DP";
                case EPortProtocol.Modbus: return "Modbus";
                case EPortProtocol.INTBUS: return "INTBUS";
                case EPortProtocol.HilonA_TDHL: return "HilonA_TDHL";
                case EPortProtocol.DeviceNET: return "DeviceNET";
                case EPortProtocol.Modbus_CAN_TDHL: return "Modbus_CAN_TDHL";
                case EPortProtocol.Modbus_NARI_LC: return "Modbus_NARI_LC";
                case EPortProtocol.LRR_RNB3000: return "LRR_RNB3000";
                case EPortProtocol.SDP_UPS_RS232_BJHDZX: return "SDP_UPS_RS232_BJHDZX";
                case EPortProtocol.CDT91: return "CDT91";
                case EPortProtocol.LFP: return "LFP";
                case EPortProtocol.DL_T645_1997: return "DL/T645_1997";
                case EPortProtocol.IEC103_NARI: return "IEC103_NARI";
                case EPortProtocol.WPD200: return "WPD200";
                case EPortProtocol.WP: return "WP";
                case EPortProtocol.ModbusTCP: return "Modbus TCP";
                case EPortProtocol.IEC101: return "IEC101";
                case EPortProtocol.IEC102: return "IEC102";
                case EPortProtocol.IEC103_HilonA_TF_SHFZ: return "IEC103_HilonA_TF_SHFZ";
                case EPortProtocol.IEC103_SIEMENS_Siprotec: return "IEC103_SIEMENS_Siprotec";
                case EPortProtocol.CAN20BEx_TDHL: return "CAN20BEx_TDHL";
                case EPortProtocol.CAN20BEx_MPW: return "CAN20BEx_MPW";
                case EPortProtocol.CAN20A_HilonA_TDHL: return "CAN20A_HilonA_TDHL";
                case EPortProtocol.Nonstandard_Series: return "非标准串口通讯协议";
                case EPortProtocol.OPC: return "OPC";
                case EPortProtocol.EtherNet_IP: return "EtherNet/IP";
                case EPortProtocol.DTU_TDHL: return "DTU_TDHL";
                case EPortProtocol.DTU_HD: return "DTU_HD";
                case EPortProtocol.IEC103_SF: return "IEC103_SF";
                case EPortProtocol.IEC103_NANZI: return "IEC103_NANZI";
                case EPortProtocol.ATN101: return "ATN101";
                case EPortProtocol.CDT_TCP: return "CDT_TCP";
                default: return "CommonIO";
            }
        }
        public EPortProtocol GetPortProtocol(String _PortProtocolName)
        {
            if (_PortProtocolName == "CommonIO") return EPortProtocol.NoProtocol;
            else if (_PortProtocolName == "Profibus DP") return EPortProtocol.ProfibusDP;
            else if (_PortProtocolName == "Modbus") return EPortProtocol.Modbus;
            else if (_PortProtocolName == "INTBUS") return EPortProtocol.INTBUS;
            else if (_PortProtocolName == "HilonA_TDHL") return EPortProtocol.HilonA_TDHL;
            else if (_PortProtocolName == "DeviceNET") return EPortProtocol.DeviceNET;
            else if (_PortProtocolName == "Modbus_CAN_TDHL") return EPortProtocol.Modbus_CAN_TDHL;
            else if (_PortProtocolName == "Modbus_NARI_LC") return EPortProtocol.Modbus_NARI_LC;
            else if (_PortProtocolName == "LRR_RNB3000") return EPortProtocol.LRR_RNB3000;
            else if (_PortProtocolName == "SDP_UPS_RS232_BJHDZX") return EPortProtocol.SDP_UPS_RS232_BJHDZX;
            else if (_PortProtocolName == "CDT91") return EPortProtocol.CDT91;
            else if (_PortProtocolName == "LFP") return EPortProtocol.LFP;
            else if (_PortProtocolName == "DL/T645_1997") return EPortProtocol.DL_T645_1997;
            else if (_PortProtocolName == "IEC103_NARI") return EPortProtocol.IEC103_NARI;
            else if (_PortProtocolName == "WPD200") return EPortProtocol.WPD200;
            else if (_PortProtocolName == "WP") return EPortProtocol.WP;
            else if (_PortProtocolName == "Modbus TCP") return EPortProtocol.ModbusTCP;
            else if (_PortProtocolName == "IEC101") return EPortProtocol.IEC101;
            else if (_PortProtocolName == "IEC102") return EPortProtocol.IEC102;
            else if (_PortProtocolName == "IEC103_HilonA_TF_SHFZ") return EPortProtocol.IEC103_HilonA_TF_SHFZ;
            else if (_PortProtocolName == "IEC103_SIEMENS_Siprotec") return EPortProtocol.IEC103_SIEMENS_Siprotec;
            else if (_PortProtocolName == "CAN20BEx_TDHL") return EPortProtocol.CAN20BEx_TDHL;
            else if (_PortProtocolName == "CAN20BEx_MPW") return EPortProtocol.CAN20BEx_MPW;
            else if (_PortProtocolName == "CAN20A_HilonA_TDHL") return EPortProtocol.CAN20A_HilonA_TDHL;
            else if (_PortProtocolName == "非标准串口通讯协议") return EPortProtocol.Nonstandard_Series;
            else if (_PortProtocolName == "OPC") return EPortProtocol.OPC;
            else if (_PortProtocolName == "EtherNet/IP") return EPortProtocol.EtherNet_IP;
            else if (_PortProtocolName == "DTU_TDHL") return EPortProtocol.DTU_TDHL;
            else if (_PortProtocolName == "DTU_HD") return EPortProtocol.DTU_HD;
            else if (_PortProtocolName == "IEC103_SF") return EPortProtocol.IEC103_SF;
            else if (_PortProtocolName == "IEC103_NANZI") return EPortProtocol.IEC103_NANZI;
            else if (_PortProtocolName == "ATN101") return EPortProtocol.ATN101;
            else if (_PortProtocolName == "CDT_TCP") return EPortProtocol.CDT_TCP;
            return EPortProtocol.NoProtocol;
        }

        [Browsable(true), Description("协议名称"), Category("Data"), DisplayName("协议类型")]
        public String sProtocolName
        {
            get
            {
                return PortProtocol;
            }
        }
    }
}
