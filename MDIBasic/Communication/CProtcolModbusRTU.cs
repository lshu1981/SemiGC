using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace LSSCADA
{
    class CPortModbusRTU : CPort
    {
        SerialPort n_SerialPort = null;

        public ArrayList ListImmSendMsg = new ArrayList();//优先发送的报文队列
        AutoResetEvent[] PortEvents;
        private System.Timers.Timer CommTimer;// = new Timer(10000);
        public Thread TPortThread = null;             //通信线程
        public CPortModbusRTU()
            : base()
        {
            PortEvents = new AutoResetEvent[]
　　　　　　{
                new　AutoResetEvent(false),//[0]串口接收数据
　　　　　　    new　AutoResetEvent(false),//[1]串口发送数据
　　　　　　    new　AutoResetEvent(false)//[2]退出线程
            };
        }

        //打开串口
        public override bool Open()
        {
            base.Open();

            //if (PortName != "Com08")
            //    return false;
            try
            {
                n_SerialPort = new SerialPort();
                string[] sPortSet = PortConfig1.Split(',');
                n_SerialPort.PortName = sPortSet[0];
                n_SerialPort.BaudRate = Convert.ToInt32(sPortSet[1]);
                n_SerialPort.DataBits = Convert.ToInt16(sPortSet[3]);
                switch (sPortSet[2])
                {
                    case "N":
                        n_SerialPort.Parity = Parity.None;
                        break;
                    case "O":
                        n_SerialPort.Parity = Parity.Odd;
                        break;
                    case "E":
                        n_SerialPort.Parity = Parity.Even;
                        break;
                    default:
                        n_SerialPort.Parity = Parity.None;
                        break;
                }
                switch (sPortSet[4])
                {
                    case "0":
                        n_SerialPort.StopBits = StopBits.None;
                        break;
                    case "1":
                        n_SerialPort.StopBits = StopBits.One;
                        break;
                    case "2":
                        n_SerialPort.StopBits = StopBits.Two;
                        break;
                    case "1.5":
                        n_SerialPort.StopBits = StopBits.OnePointFive;
                        break;
                    default:
                        n_SerialPort.StopBits = StopBits.None;
                        break;
                }
                n_SerialPort.DataReceived += new SerialDataReceivedEventHandler(n_SerialPort_DataReceived);//设置串口接收中断函数
                n_SerialPort.Open();
                ListImmSendMsg.Clear();
                bOpen = true;
                CommTimer = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为1000毫秒； 
                CommTimer.Elapsed += new System.Timers.ElapsedEventHandler(CommTimerCall);//到达时间的时候执行事件； 
                CommTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
                CommTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

                //启动端口数据收发线程
                TPortThread = new Thread(new ThreadStart(CTPortThread));
                TPortThread.Name = "TPortThread";
                TPortThread.IsBackground = true;
	            TPortThread.Start();
                PresentSendStaIndex = 0;
                GetSendMsg();
                //Write("test12345");
                
            }
            catch (Exception)
            {
                bOpen = false;
                return false;
            }
            return bOpen;
        }

        public override bool Close()
        {
            //TPortThread.Abort();
            if (n_SerialPort != null)
            {
                if (n_SerialPort.IsOpen)
                    n_SerialPort.Close();
            }
            return true;
        }
        //定时器
        public void CommTimerCall(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!bOpen)
            {
                return;
            }
            try
            {
                DelayTime += 100;
                //Debug.WriteLine(iDelayTime);
                if (LastSendMsg.QuLen <= 0)
                    return;
                if (DelayTime >= LastSendMsg.Delay_Time)//超时
                {
                   int i = LastSendMsg.iStaIndex;
                    CStation nSta = ((CStation)ListStation[i]);
                    nSta.present_MsgFailRep++;//子站重发次数+1

                    //Debug.WriteLine(nSta.Name + " 1FailRep:" + nSta.present_FailRep.ToString() + ":" + LastSendMsg.Delay_Time.ToString());
                    if (nSta.present_MsgFailRep > MsgFailRep)//超过最大重发次数
                    {
                       // Debug.WriteLine(nSta.Name + " 2FailRep:" + nSta.present_FailRep.ToString());
                        nSta.present_MsgFailRep = 0;//子站重发次数置0
                        nSta.present_DevFailNum++;//子站失败次数置+1
                       // Debug.WriteLine(nSta.Name + " 3FailNum:" + nSta.present_MsgFailNum.ToString());
                        if (nSta.present_DevFailNum > DevFailNum)//超过最大失败次数
                        {
                          //  Debug.WriteLine(nSta.Name + " 4FailNum:" + nSta.present_MsgFailNum.ToString());
                            nSta.CommStateE = ECommSatate.Failure;//子站置通信故障
                            GetSendMsg();//失败次数没达到，继续发送下个子站报文
                        }
                        else
                        {
                            GetSendMsg();//失败次数没达到，继续发送下个子站报文
                        }
                    }
                    else
                    {
                        PortEvents[1].Set();//重发次数不到，重发上次报文
                    }
                }
            }
            catch (Exception e1)
            {
                Debug.WriteLine("CommTimerCall" + e1.Message);
                
            }
            //Debug.WriteLine("Send");
        }

        private bool GetSendMsg()
        { 
            try
            {
                CMessage sSend = new CMessage();
                lock (this)
                {
                    if (ListImmSendMsg.Count > 0)
                    {
                        sSend = (CMessage)ListImmSendMsg[0];
                        LastSendMsg = sSend.Clone();
                        //Write(sSend.sSendMsg);
                        ListImmSendMsg.RemoveAt(0);
                        PortEvents[1].Set();
                        return true;
                    }
                }
                //return true;
                CStation nSta = (CStation)ListStation[PresentSendStaIndex];
                if (nSta.Driver == "真空硅")
                { int iii = 0; }
                if (nSta == null)
                {
                    PresentSendStaIndex++;
                    PresentSendStaIndex = PresentSendStaIndex % ListStation.Count;
                    return false;
                }
                if (nSta.CommStateE == ECommSatate.Failure)//当前发送子站为通信不正常时
                {
                    nSta.present_DevFailCyc++;//当前发送子站等待周期+1
                    //Debug.WriteLine(nSta.Name + " 4:" + nSta.present_MsgFailWaitCyc.ToString());
                    if (nSta.present_DevFailCyc <= DevFailCyc)//不超过最大等待周期时继续下一个子站
                    {
                        //Debug.WriteLine(nSta.Name + " 5:" + PresentSendStaIndex.ToString());
                        PresentSendStaIndex++;
                        PresentSendStaIndex = PresentSendStaIndex % ListStation.Count;
                        //Debug.WriteLine(nSta.Name + " 6:" + PresentSendStaIndex.ToString());
                        GetSendMsg();
                        return false;
                    }
                    else//否则开始重新发送该子站，重新计算超时
                    {
                        nSta.present_MsgFailRep = 0;
                        nSta.present_DevFailNum = 0;
                        nSta.present_DevFailCyc = 0;
                    }
                }
                sSend = nSta.GetNextLoopMsg();
                if (sSend.sSendMsg.Length > 0)
                {
                    LastSendMsg = sSend;//.Clone();
                    PortEvents[1].Set();
                }

                PresentSendStaIndex++;
                PresentSendStaIndex = PresentSendStaIndex % ListStation.Count;
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetSendMsg" + e.Message);
            }
            return true;
        }
        //串口接收到数据中断
        void n_SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            PortEvents[0].Set();
        }
        //串口收发线程
        private void CTPortThread()
        {
            while (true)
            {
                int index = WaitHandle.WaitAny(PortEvents);
                switch (index)
                {
                    case 0://[0]串口接收数据
                        Thread.Sleep(10);
                        try
                        {
                            int n = n_SerialPort.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
                            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
                            //received_count += n;//增加接收计数
                            n_SerialPort.Read(buf, 0, n);//读取缓冲数据
                            if (n > 0)
                            {
                                lock (this)
                                {
                                    cRecvBuff.AddBuff(buf);
                                    int i = LastSendMsg.iStaIndex;
                                    CStation nSta = ((CStation)ListStation[i]);
                                    //Debug.WriteLine(nSta.Name + " 5:" + PresentSendStaIndex.ToString());
                                    nSta.PortDataRecv(LastSendMsg, cRecvBuff);
                                    //Debug.WriteLine(nSta.Name + " 6:" + PresentSendStaIndex.ToString());
                                   GetSendMsg();
                                    
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("串口接收" + e.Message);
                        }
                        //iTemp = 0;
                        break;
                    case 1://[1]串口发送数据
                        try
                        {
                            int j = LastSendMsg.iStaIndex;
                            CStation jSta = ((CStation)ListStation[j]);
                            //if (jSta.StaDevice.Information1 == 0)
                            //    Thread.Sleep(jSta.StaDevice.Information0);
                            if (jSta.Address == "251")
                            { int iii = 0; }
                            n_SerialPort.Write(LastSendMsg.sSendMsg.DataBuffer, 0, LastSendMsg.sSendMsg.Length);
                           // if (jSta.StaDevice.Information1 == 1)
                           //     Thread.Sleep(jSta.StaDevice.Information0);
                            DelayTime = 0;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("串口发送" + e.Message);
                        }
                        break;
                    case 2://[2]停止线程
                        break;
                    default:
                        break;
                }
                Thread.Sleep(10);
            }
            //Debug.WriteLine("quit" );
        }

        public virtual bool Write(SSend_Message sSend) 
        {
            if (bOpen)
            {//写串口数据
                try
                {
                    //n_SerialPort.WriteLine(sSendData);
                    n_SerialPort.Write(sSend.DataBuffer, 0, sSend.Length);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }

    class CProtcolModbusRTU : CStation
    {
        public CProtcolModbusRTU()
            : base()
        { }

        public override bool LoadFromNode(XmlElement Node)
        {
            base.LoadFromNode(Node);
            return true;
        }

        public override void MsgSplit()
        {
            Address64 = Convert.ToInt64(Address);
            foreach (CMessage nMsg in StaDevice.ListMsgLoop)
            {
                nMsg.iPortIndex = this.iPortIndex;
                nMsg.iStaIndex = this.iStaIndex;

                PacketLoopMsg(nMsg);

                ListMsgCyc.Add(nMsg);
            }

            foreach (CMessage nMsg in StaDevice.ListMsgLoop)
            {
                if (nMsg.Priority == EMsgPriority.HI_LIST)
                {
                    ListMsgCyc.Add(nMsg);
                    ListMsgCyc.Add(nMsg);
                    ListMsgCyc.Add(nMsg);
                }
                else if (nMsg.Priority == EMsgPriority.ME_LIST)
                {
                    ListMsgCyc.Add(nMsg);
                }
            }

        }
        //组装报文
        public void PacketLoopMsg(CMessage cMsg)
        {
            SSend_Message temSend = new SSend_Message();

            temSend.Length = 8;
            temSend.DataBuffer = new byte[temSend.Length];
            temSend.DataBuffer[0] = Convert.ToByte(Address);

            temSend.DataBuffer[1] = Convert.ToByte(cMsg.Function);
            int iStart = Convert.ToInt32(cMsg.Starting);
            temSend.DataBuffer[3] = (byte)(iStart % 256);
            temSend.DataBuffer[2] = (byte)(iStart >> 16);

            int iNum = Convert.ToInt32(cMsg.Number);
            temSend.DataBuffer[5] = (byte)(iNum % 256);
            temSend.DataBuffer[4] = (byte)(iNum >> 16);

            byte[] bCRC = new byte[2];
            bCRC = CRC.CRC16Chk(temSend.DataBuffer, temSend.Length - 2);
            temSend.DataBuffer[temSend.Length - 2] = bCRC[1];
            temSend.DataBuffer[temSend.Length - 1] = bCRC[0];

            switch (temSend.DataBuffer[1])
            {
                case 1:
                case 2:
                    cMsg.ReLen = (byte)((iNum-1) / 8)+6;
                    break;
                case 3:
                case 4:
                    cMsg.ReLen = (byte)(iNum*2)+5;
                    break;
                case 5:
                case 6:
                    cMsg.ReLen = temSend.Length;
                    break;
                case 7:
                    cMsg.ReLen = 5;
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

        public override CMessage GetSendMsgCyc()
        {
            CMessage cSend = new CMessage();
            cSend = GetNextLoopMsg();

            return cSend;
        }

        public override bool PortDataRecv(CMessage sSend, CRecvBuff cRecv)
        {
            base.PortDataRecv(sSend, cRecv);
            
            for (int i = 0; i < cRecv.iRecvLen - sSend.ReLen+1; i++)
            {
                if (cRecv.bRecvBuff[i] == sSend.sSendMsg.DataBuffer[0])
                {
                    if (cRecv.bRecvBuff[i + 1] == sSend.sSendMsg.DataBuffer[1])
                    {

                        byte[] Datebuff = new byte[sSend.ReLen ];
                        Array.Copy(cRecv.bRecvBuff, i, Datebuff, 0, sSend.ReLen);
                        lock (this)
                        {
                            cRecv.DelBuff(i + sSend.ReLen);
                        }

                        if (CRC.bCheckCRC(Datebuff, sSend.ReLen))//CRC校验
                        {
                            //Debug.WriteLine(Name + " 5:" + sSend.Description);
                            CommStateE = ECommSatate.Normal;
                            present_MsgFailRep = 0;
                            present_DevFailNum = 0;
                            present_DevFailCyc = 0;
                            GetVarValue(sSend, Datebuff);
                            //Debug.WriteLine(Name + " 6:" + sSend.Description);
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
            return false;
        }

        private void GetVarValue(CMessage nMsg, byte[] Datebuff)
        {
            lock (this)
            {
                foreach (CVar nVar in nMsg.ListMsgVar.Values)
                {
                    int iIndex = nVar.ByteAddr - Convert.ToInt32(nMsg.Starting);
                    int iBit = nVar.BitAddr;
                    int iLen = nVar.Length;
                    Int64 Value = 0, Value1 = 0;

                    switch (Convert.ToInt32(nMsg.Function))
                    {
                        case 1:
                        case 2:
                            int nByte = (iIndex >> 3) + 3;
                            Value = (Datebuff[nByte] >> (iIndex % 8)) & 1;
                            nVar.SetValue(Value);
                            break;
                        case 3:
                        case 4:
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
                                for (int i = 0; i < 4; i++)
                                {
                                    Value1 += Datebuff[nByte + nVar.bHighByte[i]] << (8 * i);
                                }
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

                            break;
                        case 7:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
