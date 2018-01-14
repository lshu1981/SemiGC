using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace LSSCADA
{
    public class CProtcolModbusTCP : CProtcolTCP
    {
        public ushort iSendNum = 0;

        public CProtcolModbusTCP()
            : base()
        {
        }

        public override bool LoadFromNode(XmlElement Node)
        {
            base.LoadFromNode(Node);
            return true;
        }

        public override bool Open() //组装所有读报文
        {
            base.Open();
            InitLoopMsg();

            CommTimer = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为1000毫秒； 
            CommTimer.Elapsed += new System.Timers.ElapsedEventHandler(CommTimerCall);//到达时间的时候执行事件； 
            CommTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
            CommTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            client = new TcpClient();

            //启动端口数据收发线程
            TPortThread = new Thread(new ThreadStart(CTPortThread));
            TPortThread.Name = "TPortThread";
            TPortThread.IsBackground = true;
            TPortThread.Start();
            //Write("test12345");
            return true;
        }

        public override bool ConnectServer() //重新连接
        {
            try
            {
                if (!base.ConnectServer())
                    return false;
                SaveDebugMsg("正常");
                CommStateE = ECommSatate.Normal;
                GetSendMsg();

                return true;
            }
            catch (Exception e)
            {
                CommStateE = ECommSatate.Unknown;
                Debug.WriteLine("ModbusTCP.ConnectServer2:" + e.ToString());
                return false;
            }
        }

        public override bool Close() //组装所有读报文
        {
            //TPortThread.Abort();
            Stop();
            return false;
        }

        public void Stop()
        {
            if (netstream != null) { netstream.Close(); }
            if (client != null) { client.Close(); }
        }

        private void CTPortThread()
        {
            while (true)
            {
                try
                {
                    if (ListStrMsg.Count > ListStrMsgMax)
                    {
                        //SaveDebugMsg("正常");
                        ListStrMsg.RemoveRange(0, ListStrMsgMax / 5);
                    }
                    if (!client.Connected)
                    {
                        bOpen = false;
                        CommStateE = ECommSatate.Unknown;
                        bOpen = ConnectServer();
                        if (!bOpen)
                        {
                            continue;
                        }
                    }
                    if (netstream.CanRead)
                    {
                        byte[] buf = new byte[1024];
                        int iNum = 0;
                        int iLength = 0;
                        do
                        {
                            iNum = netstream.Read(buf, iLength, buf.Length);
                            iLength += iNum;
                        }
                        while (netstream.DataAvailable);

                        if (iLength > 0)
                        {
                            if (bDebug)
                            {
                                string sShow1 = "";
                                for (int i = 0; i < iLength; i++)
                                {
                                    sShow1 += buf[i].ToString("X2") + " ";
                                }
                                frmShow.InsertText(sShow1, "ModbusTCP:Recv", iLength);
                                Thread.Sleep(50);
                            }

                            string sShow = "";
                            for (int i = 0; i < Math.Min(iLength, 100); i++)
                            {
                                sShow += buf[i].ToString("X2") + " ";
                            }
                            sShow = DateTime.Now.ToLongTimeString() + "ModbusTCPRecv:" + sShow + " (" + iLength.ToString() + ")";
                           // Debug.WriteLine(sShow);
                            ListStrMsg.Add(sShow);
                            if (PortDataRecv(LastSendMsg, buf, iLength))
                                GetSendMsg();
                        }
                    }
                }
                catch (Exception e)
                {
                    ListStrMsg.Add("CProtcolModbusTCP.CTPortThread:" + e.Message);
                    Debug.WriteLine("CProtcolModbusTCP.CTPortThread:" + e.Message);
                }
                Thread.Sleep(10);
            }
        }

        //定时器
        public void CommTimerCall(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!bOpen)
            {
                return;
            }
            if (CommStateE == ECommSatate.Unknown)
            {
                return;
            }
            try
            {
                DelayTime += 100;
                if (DelayTime >=Math.Max( LastSendMsg.Delay_Time , 400 ))//超时
                {
                    SaveDebugMsg("延时");
                    DelayTime = 0;
                    present_MsgFailRep++;//子站重发次数+1
                    if (present_MsgFailRep > 2)//超过最大重发次数
                    {
                        present_MsgFailRep = 0;//子站重发次数置0
                        present_DevFailNum++;//子站失败次数置+1
                        if (present_DevFailNum > 3)//超过最大失败次数
                        {
                            if (CommStateE != ECommSatate.Failure)
                            {
                                SaveDebugMsg("故障");
                                CommStateE = ECommSatate.Failure;//子站置通信故障
                                CAlarmMsgEventArgs ee = new CAlarmMsgEventArgs();
                                ee.Date_Time = DateTime.Now;
                                ee.Recorder = Description + "通信故障";
                                ee.priority = EAlarmPriority.PRIORITY_1;
                                ee.eAlarmType = EAlarmType.StationState;
                                ee.StaName = Name;
                                staAlarm.OnAlarmEvent(ee);
                            }
                            present_DevFailNum = 0;
                            GetSendMsg();//失败次数没达到，继续发送下个子站报文
                        }
                        else
                        {
                            GetSendMsg();//失败次数没达到，继续发送下个子站报文
                        }
                    }
                    else
                    {
                        Write();
                    }
                }
            }
            catch (Exception e1)
            {
                ListStrMsg.Add("CProtcolModbusTCP.CommTimerCall:" + Name + e1.Message);
                Debug.WriteLine("CProtcolModbusTCP.CommTimerCall:" + Name + e1.Message);
            }
            //Debug.WriteLine("Send");
        }

        public virtual bool Write()
        {
            if (CommStateE != ECommSatate.Unknown)
            {//写串口数据
                try
                {
                    if (LastSendMsg == null)
                        return false;
                    //n_SerialPort.WriteLine(sSendData);
                    
                    netstream.Write(LastSendMsg.sSendMsg.DataBuffer, 0, LastSendMsg.sSendMsg.Length);

                    if (bDebug)
                    {
                        string sShow1 = "";
                        for (int i = 0; i < LastSendMsg.sSendMsg.Length; i++)
                        {
                            sShow1 += LastSendMsg.sSendMsg.DataBuffer[i].ToString("X2") + " ";
                        }
                        frmShow.InsertText(sShow1, "ModbusTCP;Send", LastSendMsg.sSendMsg.Length);
                    }
                    string sShow = "";
                    for (int i = 0; i < LastSendMsg.sSendMsg.Length; i++)
                    {
                        sShow += LastSendMsg.sSendMsg.DataBuffer[i].ToString("X2") + " ";
                    }
                    sShow = DateTime.Now.ToLongTimeString() + "ModbusTCPSend:" + sShow + " (" + LastSendMsg.sSendMsg.Length.ToString() + ")";
                    //Debug.WriteLine(sShow);
                    ListStrMsg.Add(sShow);
                }
                catch (Exception e1)
                {
                    ListStrMsg.Add("CProtcolModbusTCP.Write:" + Name + e1.Message);
                    return false;
                }
            }
            return true;
        }

        private bool GetSendMsg()
        {
            try
            {
                CMessage sSend = new CMessage();

                iSendNum++;

                lock (this)
                {
                    if (ListImmSendMsg.Count > 0)
                    {
                        LastSendMsg = (CMessage)ListImmSendMsg[0];
                        LastSendMsg.sSendMsg.DataBuffer[0] = (Byte)(iSendNum >> 8);
                        LastSendMsg.sSendMsg.DataBuffer[1] = (Byte)iSendNum;
                        Write();
                        ListImmSendMsg.RemoveAt(0);
                        return true;
                    }
                }

                sSend = GetNextLoopMsg();
                if (sSend.sSendMsg.Length > 0)
                {
                    LastSendMsg = sSend;
                    LastSendMsg.sSendMsg.DataBuffer[0] = (Byte)(iSendNum >> 8);
                    LastSendMsg.sSendMsg.DataBuffer[1] = (Byte)iSendNum;
                    Write();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("CProtcolModbusTCP.GetSendMsg" + e.Message);
            }
            return true;
        }

        public void InitLoopMsg()
        {
            ListMsgCyc.Clear();
            foreach (CMessage nMsg in StaDevice.ListMsgLoop)
            {
                nMsg.iPortIndex = this.iPortIndex;
                nMsg.iStaIndex = this.iStaIndex;
                PacketLoopMsg(nMsg);
                ListMsgCyc.Add(nMsg);
            }
        }

        public void PacketLoopMsg(CMessage cMsg)//组装循环报文
        {
            SSend_Message temSend = new SSend_Message();
            Byte iFun = Convert.ToByte(cMsg.Function);
            int iStart = Convert.ToInt32(cMsg.Starting);
            int iNum = Convert.ToInt32(cMsg.Number);
            switch (iFun)
            {
                case 1:
                case 2:
                    temSend.Length = 12;
                    temSend.DataBuffer = new byte[temSend.Length];

                    temSend.DataBuffer[5] = 6;
                    temSend.DataBuffer[6] = Convert.ToByte(Address);
                    temSend.DataBuffer[7] = iFun;

                    temSend.DataBuffer[8] = (byte)(iStart >> 8);
                    temSend.DataBuffer[9] = (byte)iStart;
                    temSend.DataBuffer[10] = (byte)(iNum >> 8);
                    temSend.DataBuffer[11] = (byte)iNum;
                    break;
                case 3:
                case 4:
                    temSend.Length = 12;
                    temSend.DataBuffer = new byte[temSend.Length];

                    temSend.DataBuffer[5] = 6;
                    temSend.DataBuffer[6] = Convert.ToByte(Address);
                    temSend.DataBuffer[7] = iFun;

                    temSend.DataBuffer[8] = (byte)(iStart >> 8);
                    temSend.DataBuffer[9] = (byte)iStart;
                    temSend.DataBuffer[10] = (byte)(iNum >> 8);
                    temSend.DataBuffer[11] = (byte)iNum;
                    break;
                case 5:
                case 6:
                    break;
                default:
                    break;
            }

            cMsg.sSendMsg = temSend;
            cMsg.ListMsgVar.Clear();

            foreach (CVar nVar in StaDevice.ListDevVar)
            {
                if (nVar.ByteAddr >= iStart && nVar.ByteAddr < iStart + iNum)
                    cMsg.ListMsgVar.Add(nVar.Name, nVar);
            }
        }

        //根据变量名称写
        public void SendAODO(string sVar, int iValue, int iFun)
        {
            //(012)49 A4 00 00 00 06 05 05 00 6E FF 00 
            //(012)49 E5 00 00 00 06 05 06 00 11 00 00 
            CMessage nMsg = new CMessage();
            SSend_Message temSend = new SSend_Message();

            foreach (CVar nVar in StaDevice.ListDevVar)
            {
                if (nVar.Name == sVar)
                {
                    int iAddr = nVar.ByteAddr;
                    switch (iFun)
                    {
                        case 5:
                        case 6:
                            temSend.Length = 12;
                            temSend.DataBuffer = new byte[temSend.Length];

                            temSend.DataBuffer[5] = 6;
                            temSend.DataBuffer[6] = Convert.ToByte(Address);
                            temSend.DataBuffer[7] = (Byte)iFun;

                            temSend.DataBuffer[8] = (byte)(iAddr >> 8);
                            temSend.DataBuffer[9] = (byte)iAddr;
                            temSend.DataBuffer[10] = (byte)(iValue >> 8);
                            temSend.DataBuffer[11] = (byte)iValue;
                            nMsg.sSendMsg = temSend;
                            ListImmSendMsg.Add(nMsg);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //根据变量名称写
        public void SendAODO(int iAddr, int iValue, int iFun)
        {
            //(012)49 A4 00 00 00 06 05 05 00 6E FF 00 
            //(012)49 E5 00 00 00 06 05 06 00 11 00 00 
            CMessage nMsg = new CMessage();
            SSend_Message temSend = new SSend_Message();
            switch (iFun)
            {
                case 5:
                case 6:
                    temSend.Length = 12;
                    temSend.DataBuffer = new byte[temSend.Length];

                    temSend.DataBuffer[5] = 6;
                    temSend.DataBuffer[6] = Convert.ToByte(Address);
                    temSend.DataBuffer[7] = (Byte)iFun;

                    temSend.DataBuffer[8] = (byte)(iAddr >> 8);
                    temSend.DataBuffer[9] = (byte)iAddr;
                    temSend.DataBuffer[10] = (byte)(iValue >> 8);
                    temSend.DataBuffer[11] = (byte)iValue;
                    nMsg.sSendMsg = temSend;
                    ListImmSendMsg.Add(nMsg);
                    break;
                default:
                    break;
            }
        }

        public bool PortDataRecv(CMessage sSend, byte[] cRecv, int iRecvLen)
        {
            //     00 01 02 03 04 05 06 07 08 09 10 11
            //(012)00 07 00 00 00 06 03 03 00 41 00 40 
            //(137)00 07 00 00 00 83 03 03 80 00 55 00 72 00 77 .......

            //(012)01 2F 00 00 00 06 06 02 00 00 00 99 
            //(029)01 2F 00 00 00 17 06 02 14 EC 9B CA 85 8E 35 EA 5D D7 5F 27 C3 A5 08 8E 3C F1 39 25 00 
            //(012)49 A4 00 00 00 06 05 05 00 6E FF 00 
            //(012)49 A4 00 00 00 06 05 05 00 6E FF 00 
            //(012)49 E5 00 00 00 06 05 06 00 11 00 00 
            //(012)49 E5 00 00 00 06 05 06 00 11 00 00 

            try
            {
                if (iRecvLen < 10)
                {
                    ListStrMsg.Add("PortDataRecv:false:iRecvLen < 10");
                    return false;
                }
                //Debug.WriteLine(iSendNum.ToString());
                for (int i = 0; i < iRecvLen - 10; i++)
                {
                    if (cRecv[i] * 256 + cRecv[i + 1] == iSendNum)
                    {
                        int iLen = cRecv[i + 5];
                        if (iRecvLen - i < iLen + 6)
                        {
                            ListStrMsg.Add("PortDataRecv:false:iRecvLen - i < iLen + 6");
                            return false;
                        }

                        if (CommStateE == ECommSatate.Failure)
                        {
                            SaveDebugMsg("正常");
                            CommStateE = ECommSatate.Normal;//子站置通信恢复
                            CAlarmMsgEventArgs ee = new CAlarmMsgEventArgs();
                            ee.Date_Time = DateTime.Now;
                            ee.Recorder = Description + "通信恢复";
                            ee.priority = EAlarmPriority.PRIORITY_1;
                            ee.eAlarmType = EAlarmType.StationState;
                            ee.StaName = Name;
                            staAlarm.OnAlarmEvent(ee);
                        }
                        else if (CommStateE != ECommSatate.Normal)
                        {
                            SaveDebugMsg("正常");
                            CommStateE = ECommSatate.Normal;//子站置通信恢复
                        }
                        DelayTime = 0;
                        present_MsgFailRep = 0;
                        present_DevFailNum = 0;

                        byte[] Datebuff = new byte[iLen];
                        Array.Copy(cRecv, i + 6, Datebuff, 0, iLen);

                        GetVarValue(sSend, Datebuff);
                        return true;
                    }
                }
                ListStrMsg.Add("PortDataRecv:false:cRecv[i] * 256 + cRecv[i + 1] == iSendNum");
                return false;
            }
            catch (Exception ex)
            {
                ListStrMsg.Add("CProtcolModbusTCP.PortDataRecv:" + ex.Message);
                Debug.WriteLine("CProtcolModbusTCP.PortDataRecv:" + ex.Message);
                string sShow = "";
                for (int i = 0; i < iRecvLen; i++)
                {
                    sShow += cRecv[i].ToString("X2") + " ";
                }
                ListStrMsg.Add(DateTime.Now.ToLongTimeString() + "Err:" + iRecvLen.ToString() + ":" + sShow);
                Debug.WriteLine(DateTime.Now.ToLongTimeString() + "Err:" + iRecvLen.ToString() + ":" + sShow);
                return false;
            }
        }

        private void CheckRunState()
        {

        }

        private void GetVarValue(CMessage nMsg, byte[] Datebuff)
        {
            lock (this)
            {
                foreach (CVar nVar in nMsg.ListMsgVar.Values)
                {
                    try
                    {
                        int iIndex =nVar.ByteAddr - Convert.ToInt32(nMsg.Starting);
                        int iBit = Convert.ToInt32(nVar.BitAddr);
                        int iLen = nVar.Length;
                        Int64 Value = 0, Value1 = 0;

                        switch (Convert.ToInt32(nMsg.Function))
                        {
                            case 1:
                            case 2:
                                //     00 01 02 03 04 05 06 07 08 09 10 11
                                //(006)06 02 00 00 00 99 
                                //(023)06 02 14 EC 9B CA 85 8E 35 EA 5D D7 5F 27 C3 A5 08 8E 3C F1 39 25 00 
                                int nByte = (iIndex >> 3) + 3;
                                Value = (Datebuff[nByte] >> (iIndex % 8)) & 1;
                                nVar.SetValue(Value);
                                break;
                            case 3:
                            case 4:
                                //     00 01 02 03 04 05 06 07 08 09 10 11
                                //(006)03 03 00 41 00 40 
                                //(131)03 03 80 00 55 00 72 00 77 ......
                                nByte = iIndex * 2 + 3;
                                if (iLen <= 16)
                                {
                                    if (nVar.bHighLow == 1)
                                        Value1 = (Datebuff[nByte] << 8) + Datebuff[nByte + 1];
                                    else
                                        Value1 = Datebuff[nByte] + (Datebuff[nByte + 1] << 8);
                                    Value = (Value1 >> iBit) & ((1 << iLen) - 1);
                                }
                                else if (iLen <= 32)
                                {
                                    if (nVar.bHighLow == 1)
                                        Value1 = (Datebuff[nByte] << 24) + (Datebuff[nByte + 1] << 16) + (Datebuff[nByte + 2] << 8) + Datebuff[nByte + 3];
                                    else
                                        Value1 = Datebuff[nByte] + (Datebuff[nByte + 1] << 8) + (Datebuff[nByte + 2] << 16) + (Datebuff[nByte + 3] << 24);
                                    if (iLen < 32)
                                        Value = (Value1 >> iBit) & ((1 << iLen) - 1);
                                    else
                                        Value = Value1;
                                }
                                else
                                    Value = 0;
                                nVar.SetValue(Value);
                                break;
                            case 5:
                            case 6:
                                //(006)05 05 00 6E FF 00 
                                //(006)05 05 00 6E FF 00
                                break;
                            case 7:
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ee)
                    {
                        nVar.SetValue(0);
                    }
                }
            }
        }
    }
}
