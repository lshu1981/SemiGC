using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace LSSCADA
{
    public class CDAModule
    {
        //根据板块类型和协议类型创建通道
        public static CPort CreatePort(string sPortType, string sPortProtocol)
        {
            CPort newPort = null;
            switch (sPortType)
            {
                case "串口":
                    switch (sPortProtocol)
                    {
                        case "Modbus_RTU":
                            newPort = new CPortSerial();
                            break;
                        case "恒温槽RE215":
                            newPort = new CPortSerial();
                            break;
                        case "真空硅901P":
                            newPort = new CPortSerial();
                            break;
                    }
                    break;
                case "TCP":
                    newPort = new CPortTCPClient();
                    break;
                default: break;
            }
            return newPort;
        }
        //根据板块类型和协议类型创建子站
        public static CStation CreateStation(string sPortType, string sPortProtocol)
        {
            CStation newSta = null;
            switch (sPortType)
            {
                case "串口":
                    if (sPortProtocol == "Modbus_RTU")
                    {
                        newSta = new CProtcolModbusRTU();
                    }
                    else if (sPortProtocol == "恒温槽RE215")
                    {
                        newSta = new CProtcolRE215();
                    }
                    else if (sPortProtocol == "真空硅901P")
                    {
                        newSta = new CProtcol901P();
                    }
                    else
                    {
                        newSta = null;
                    }
                    break;
                case "TCP":
                    if (sPortProtocol == "Modbus_TCP")
                    {
                        newSta = new CProtcolModbusTCP();
                    }
                    else if (sPortProtocol == "FINS_TCP")
                    {
                        newSta = new CProtcolFINS();
                    }
                    else
                    {
                        return null;
                    }
                    break;
                default: break;
            }
            return newSta;
        }


        public static String GetPortProtocolName(EPortProtocol _PortProtocol)
        {
            switch (_PortProtocol)
            {
                case EPortProtocol.NoProtocol:
                    return "CommonIO";
                case EPortProtocol.ProfibusDP:
                    return "Profibus DP";
                case EPortProtocol.Modbus:
                    return "Modbus";
                case EPortProtocol.INTBUS:
                    return "INTBUS";
                case EPortProtocol.HilonA_TDHL:
                    return "HilonA_TDHL";
                case EPortProtocol.DeviceNET:
                    return "DeviceNET";
                case EPortProtocol.Modbus_CAN_TDHL:
                    return "Modbus_CAN_TDHL";
                case EPortProtocol.Modbus_NARI_LC:
                    return "Modbus_NARI_LC";
                case EPortProtocol.LRR_RNB3000:
                    return "LRR_RNB3000";
                case EPortProtocol.SDP_UPS_RS232_BJHDZX:
                    return "SDP_UPS_RS232_BJHDZX";
                case EPortProtocol.CDT91:
                    return "CDT91";
                case EPortProtocol.LFP:
                    return "LFP";
                case EPortProtocol.DL_T645_1997:
                    return "DL/T645_1997";
                case EPortProtocol.IEC103_NARI:
                    return "IEC103_NARI";
                case EPortProtocol.WPD200:
                    return "WPD200";
                case EPortProtocol.WP:
                    return "WP";
                case EPortProtocol.ModbusTCP:
                    return "Modbus TCP";
                case EPortProtocol.IEC101:
                    return "IEC101";
                case EPortProtocol.IEC102:
                    return "IEC102";
                case EPortProtocol.IEC103_HilonA_TF_SHFZ:
                    return "IEC103_HilonA_TF_SHFZ";
                case EPortProtocol.IEC103_SIEMENS_Siprotec:
                    return "IEC103_SIEMENS_Siprotec";
                case EPortProtocol.CAN20BEx_TDHL:
                    return "CAN20BEx_TDHL";
                case EPortProtocol.CAN20BEx_MPW:
                    return "CAN20BEx_MPW";
                case EPortProtocol.CAN20A_HilonA_TDHL:
                    return "CAN20A_HilonA_TDHL";
                case EPortProtocol.Nonstandard_Series:
                    return "非标准串口通讯协议";
                case EPortProtocol.OPC:
                    return "OPC";
                case EPortProtocol.EtherNet_IP:
                    return "EtherNet/IP";
                case EPortProtocol.DTU_TDHL:
                    return "DTU_TDHL";
                case EPortProtocol.DTU_HD:
                    return "DTU_HD";
                case EPortProtocol.IEC103_SF:
                    return "IEC103_SF";
                case EPortProtocol.IEC103_NANZI:
                    return "IEC103_NANZI";
                case EPortProtocol.ATN101:
                    return "ATN101";
                case EPortProtocol.CDT_TCP:
                    return "CDT_TCP";
                default:
                    return "CommonIO";
            }
        }
    }
    public class CRecvBuff
    {
        public Byte[] bRecvBuff = new Byte[CCONST.MaxBuffLen];   //接受报文缓冲区
        public int iRecvLen = 0;                                    //接受报文长度
        public int station_address = -1;                            //接受报文所在子站
        public int mess_index = -1;             //次收到的报文在设备报文信息数组中的下标。
        //据此，可以快速找到应该匹配的报文。
        public int AddBuff(Byte[] bAddBuff)
        {
            if (iRecvLen + bAddBuff.Length <= CCONST.MaxBuffLen)
            {
                bAddBuff.CopyTo(bRecvBuff, iRecvLen);
                iRecvLen += bAddBuff.Length;
            }
            else
            {
                Array.Copy(bAddBuff, 0, bRecvBuff, iRecvLen, CCONST.MaxBuffLen - iRecvLen);
                iRecvLen += CCONST.MaxBuffLen - iRecvLen;
            }
            return iRecvLen;
        }
        public int AddBuff(Byte[] bAddBuff, int iStart, int iAddLen)
        {
            if (iAddLen + iRecvLen <= CCONST.MaxBuffLen)
            {
                Array.Copy(bAddBuff, iStart, bRecvBuff, iRecvLen, iAddLen);
                iRecvLen += iAddLen;
            }
            else
            {
                Array.Copy(bAddBuff, iStart, bRecvBuff, iRecvLen, CCONST.MaxBuffLen - iRecvLen);
                iRecvLen += CCONST.MaxBuffLen - iRecvLen;
            }
            return iRecvLen;
        }
        public int DelBuff(int iDelLen)
        {
            if (iDelLen < iRecvLen)
            {
                Byte[] bTemBuff = new Byte[CCONST.MaxBuffLen];
                bRecvBuff.CopyTo(bTemBuff, 0);
                Array.Clear(bRecvBuff, 0, bRecvBuff.Length);
                iRecvLen -= iDelLen;
                Array.Copy(bTemBuff, iDelLen, bRecvBuff, 0, iRecvLen);
            }
            else
            {
                Array.Clear(bRecvBuff, 0, bRecvBuff.Length);
                iRecvLen = 0;
            }
            return iRecvLen;
        }
    }

    //设备协议
    public enum EPortProtocol
    {
        OPC = -2,
        NoProtocol = -1,
        ProfibusDP = 0,
        Modbus = 1,
        INTBUS = 2,
        HilonA_TDHL = 3,
        DeviceNET = 4,
        Modbus_CAN_TDHL = 5,
        Modbus_NARI_LC = 6,
        LRR_RNB3000 = 7,
        SDP_UPS_RS232_BJHDZX = 8,
        CDT91 = 9,
        LFP = 10,
        DL_T645_1997 = 11,
        IEC103_NARI = 12,
        WPD200 = 13,
        WP = 14,
        ModbusTCP = 15,
        IEC101 = 16,
        IEC102 = 17,
        IEC103_HilonA_TF_SHFZ = 18,
        IEC103_SIEMENS_Siprotec = 19,
        CAN20BEx_TDHL = 20,
        CAN20BEx_MPW = 21,
        CAN20A_HilonA_TDHL = 22,
        EtherNet_IP = 23,
        Nonstandard_Series = 24,
        DTU_TDHL = 25,
        DTU_HD = 26,
        IEC103_SF = 27,
        IEC103_NANZI = 28,
        ATN101 = 29,
        CDT_TCP = 30
    };

    //通信状态，子站用三个，端口只有0和非0
    public enum ECommSatate//
    {
        Unknown = 0,    //子站：状态未知 端口：Open失败
        Failure = 1,    //子站：通信故障 端口：Open成功
        Normal = 2,       //子站：通信正常 端口：未定义
        Busy = 3       //子站：其他状态 端口：未定义
    }

    public class SSend_Message
    {
        public Byte[] DataBuffer;  //发送报文缓冲区，由循环报文和非循环报文组合而成
        public int Length;                 //发送报文长度=循环报文长度+非循环报文长度

        public SSend_Message Clone()
        {
            SSend_Message obj = (SSend_Message)this.MemberwiseClone();
            if (DataBuffer != null)
            {
                obj.DataBuffer = new byte[DataBuffer.Length];
                for (int i = 0; i < DataBuffer.Length; i++)
                {
                    obj.DataBuffer[i] = DataBuffer[i];
                }
            }
            else
            {
                obj.DataBuffer = new byte[1];
            }
            return obj;
        }
    }
}
