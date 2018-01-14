using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace LSSCADA
{
    class CProtcolFINS :CProtcolTCP
    {
        public CMessage SendMsgHands = new CMessage();   //握手报文
        public int SendMsgHandsNum = 0;
        public byte srv_node_no = 0;   //PLC IP末尾数字
        public byte cli_node_no = 0;   //	Source node address       计算机 IP末尾数字
        public byte[] FINSHeader = new byte[10];
        public byte[] FINSTCPHeader = new byte[32];
        public byte SID = 0;
        public int intStart = 1054;//开始地址
        public int intEnd = 1183;//结束地址
        
        //系统交互信息
        public int[] System = new int[50];
        //System[00]	1001	R	程序状态	1:H2 Idle,2:N2 Idle,3:Hot Idle,4:生长室门阀开启,5:进样室门阀开启,6:shutdown,7:Run,8:源瓶维护,9:生长室维护,10:动作中
        //System[01]	1002	R	下发运行状态	1:H2 Idle,2:N2 Idle,3:Hot Idle,4:生长室门阀开启,5:进样室门阀开启,6:shutdown,7:Run,8:源瓶维护,9:生长室维护,10:动作中
        //System[19]	1020	R	Idle返回码
        public CProtcolFINS()
            : base()
        {
            FINSTCPHeader[0] = (byte)'F';/* Header */
            FINSTCPHeader[1] = (byte)'I';
            FINSTCPHeader[2] = (byte)'N';
            FINSTCPHeader[3] = (byte)'S';
            FINSTCPHeader[4] = 0x00; /* Length */
            FINSTCPHeader[5] = 0x00;
            FINSTCPHeader[6] = 0x00;
            FINSTCPHeader[7] = 0x0C;
            FINSTCPHeader[8] = 0x00; /* Command */
            FINSTCPHeader[9] = 0x00;
            FINSTCPHeader[10] = 0x00;
            FINSTCPHeader[11] = 0x00;
            FINSTCPHeader[12] = 0x00; /* Error Code */
            FINSTCPHeader[13] = 0x00;
            FINSTCPHeader[14] = 0x00;
            FINSTCPHeader[15] = 0x00;
            FINSTCPHeader[16] = 0x00; /* Client Node Add */
            FINSTCPHeader[17] = 0x00;
            FINSTCPHeader[18] = 0x00;
            FINSTCPHeader[19] = 0x00; /*AUTOMATICALLY GET FINS CLIENT FINS NODE NUMBER*/

            SendMsgHands.sSendMsg.Length = 20;
            SendMsgHands.sSendMsg.DataBuffer = new byte[20];
            for (int i = 0; i < SendMsgHands.sSendMsg.Length; i++)
            {
                SendMsgHands.sSendMsg.DataBuffer[i] = FINSTCPHeader[i];
            }
            SendMsgHands.Delay_Time = 1000;

            FINSHeader[0] = 0x80;//	Displays frame information
            FINSHeader[1] = 0x00;//	Reserved by system.
            FINSHeader[2] = 0x02;//	Permissible number of gateways
            FINSHeader[3] = 0x00;//	Destination network address
            FINSHeader[4] = srv_node_no;//	Destination node address  PLC IP末尾数字
            FINSHeader[5] = 0x00;//	Destination unit address  
            FINSHeader[6] = 0x00;//	Source network address    
            FINSHeader[7] = cli_node_no;//	Source node address       计算机 IP末尾数字
            FINSHeader[8] = 0x00;//	Source unit address
            FINSHeader[9] = SID;//	Service ID  
        }

        public override bool LoadFromNode(XmlElement Node)
        {
            base.LoadFromNode(Node);
            return true;
        }

        public override bool Open() //组装所有读报文
        {
            base.Open();

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
                CommStateE = ECommSatate.Failure;
                string[] sIP = _ServerIP.Split('.');
                FINSHeader[4] = Convert.ToByte(sIP[3]);
                Socket s = client.Client;
                FINSHeader[7] = (byte)(((IPEndPoint)s.LocalEndPoint).Address.Address >> 24);
                SendMsgHands.sSendMsg.DataBuffer[19] = FINSHeader[7];
                InitLoopMsg();
                GetSendMsg();
                return true;
            }
            catch (Exception e)
            {
                CommStateE = ECommSatate.Unknown;
                Debug.WriteLine("PLC.ConnectServer2:" + e.ToString());
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

        //通信线程
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
                            iNum = netstream.Read(buf, iLength, buf.Length - iLength);
                            iLength += iNum;
                        }
                        while (netstream.DataAvailable || iLength > 1024);
                        if (iLength > 0)
                        {
                            if (bDebug)
                            {
                                string sShow1 = "";
                                for (int i = 0; i < iLength; i++)
                                {
                                    sShow1 += buf[i].ToString("X2") + " ";
                                }
                                frmShow.InsertText(sShow1, "PLC;Recv", iLength);
                                Thread.Sleep(50);
                            }
                            string sShow = "";
                             for (int i = 0; i <Math.Min( iLength,34); i++)
                             {
                                 sShow += buf[i].ToString("X2") + " ";
                             }
                             sShow = DateTime.Now.ToLongTimeString() + "PLCRecv:" + sShow + " (" + iLength.ToString() + ")";
                            // Debug.WriteLine(sShow);
                             ListStrMsg.Add(sShow);
                            if (PortDataRecv(LastSendMsg, buf, iLength))
                                GetSendMsg();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("CProtcolFINS.CTPortThread:" + e.Message);
                }
                Thread.Sleep(100);
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
                if (SendMsgHandsNum > 10)
                {
                    client.Close();
                    SendMsgHandsNum = 0;
                }
                DelayTime += 100;
                if (DelayTime >= Math.Max(LastSendMsg.Delay_Time, 400))//超时
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
                        Write(LastSendMsg.sSendMsg);
                    }
                }
            }
            catch (Exception e1)
            {
                Debug.WriteLine("CProtcolFINS.CmmTimerCall:" + e1.Message);

            }
            //Debug.WriteLine("Send");
        }


        //读取下一条报文
        private bool GetSendMsg()
        {
            try
            {
                CMessage sSend = new CMessage();

                SID++;
                if (CommStateE == ECommSatate.Failure)
                {
                    LastSendMsg = (CMessage)SendMsgHands;
                    SendMsgHandsNum++;
                    //Debug.WriteLine(SendMsgHandsNum.ToString());
                    if (LastSendMsg.sSendMsg.Length > 25)
                    {
                        LastSendMsg.sSendMsg.DataBuffer[25] = SID;
                    }
                    Write(LastSendMsg.sSendMsg);
                    return true;
                }
                lock (this)
                {
                    if (ListImmSendMsg.Count > 0)
                    {
                        LastSendMsg = (CMessage)ListImmSendMsg[0];
                        if (LastSendMsg.sSendMsg.Length > 25)
                        {
                            LastSendMsg.sSendMsg.DataBuffer[25] = SID;
                        }
                        Write(LastSendMsg.sSendMsg);
                        ListImmSendMsg.RemoveAt(0);
                        //Debug.WriteLine(DateTime.Now.ToLongTimeString() + "CommTimerCall:GetSendMsg:ListImmSendMsg");
                        return true;
                    }
                }

                sSend = GetNextLoopMsg();
                if (sSend.sSendMsg.Length > 0)
                {
                    LastSendMsg = sSend;
                    if (LastSendMsg.sSendMsg.Length > 25)
                    {
                        LastSendMsg.sSendMsg.DataBuffer[25] = SID;
                    }
                    Write(LastSendMsg.sSendMsg);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("CProtcolFINS.GetSendMsg" + e.Message);
            }
            return true;
        }

        //写报文到以太网口
        public virtual bool Write(SSend_Message sSend)
        {
            if (CommStateE != ECommSatate.Unknown)
            {//写串口数据
                try
                {
                    //n_SerialPort.WriteLine(sSendData);
                    netstream.Write(sSend.DataBuffer, 0, sSend.Length);

                    if (bDebug)
                    {
                        string sShow1 = "";
                        for (int i = 0; i < LastSendMsg.sSendMsg.Length; i++)
                        {
                            sShow1 += LastSendMsg.sSendMsg.DataBuffer[i].ToString("X2") + " ";
                        }
                        frmShow.InsertText(sShow1, "PLC;Send", LastSendMsg.sSendMsg.Length);
                    }
                     string sShow = "";
                     for (int i = 0; i < sSend.Length; i++)
                     {
                         sShow += sSend.DataBuffer[i].ToString("X2") + " ";
                     }
                     //if(sSend.DataBuffer[27]!=1)
                     string strAdd =" ";
                    if(sSend.Length>30)
                        strAdd += ((sSend.DataBuffer[29] << 8) + sSend.DataBuffer[30]).ToString("0000");
                    sShow = DateTime.Now.ToLongTimeString() + "PLCSend:" + sShow + " (" + sSend.Length.ToString() + ")" + strAdd;
                    //Debug.WriteLine(sShow);
                    ListStrMsg.Add(sShow);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }


        public void InitLoopMsg()//初始化循环发送报文ListMsgSend
        {
            ListMsgCyc.Clear();
            foreach (CMessage nMsg in StaDevice.ListMsgLoop)
            {
                nMsg.iPortIndex = this.iPortIndex;
                nMsg.iStaIndex = this.iStaIndex;
                PacketLoopMsg(nMsg);
                ListMsgCyc.Add(nMsg);
                ListImmSendMsg.Add(nMsg); Debug.WriteLine("InitLoopMsg.ListImmSendMsg");
            }
            SendReadIdle();
        }

        public override void MsgSplit() { }

        public string GetMsg(string sVar, int iValue)
        {
            try
            {
                if (frmMain.staComm.nReminder.ManualActList.ContainsKey(Convert.ToInt32(sVar) * 1000 + iValue))
                {
                    return frmMain.staComm.nReminder.ManualActList[Convert.ToInt32(sVar) * 1000 + iValue];
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                return sVar + ":" + iValue.ToString() + "," + ex.Message;
            }
        }

        //界面操作
        public void SendAODO(string sVar, double fValue, string sType)
        {
            foreach (CVar nVar in StaDevice.ListDevVar)
            {
                if (nVar.Name == sVar)
                {
                    SSend_Message temSet = new SSend_Message();
                    int iLen;
                    CMessage nMsg = new CMessage();
                    switch (sType)
                    {
                        case "AO"://AO 需要处理变比
                         int   iValue = (int)Math.Round(fValue / nVar.RatioValue);
                            temSet.Length = 36;
                            temSet.DataBuffer = new byte[temSet.Length];
                            for (int i = 0; i < 4; i++)
                            {
                                temSet.DataBuffer[i] = FINSTCPHeader[i];
                            }
                            iLen = temSet.Length - 8;
                            temSet.DataBuffer[4] = (byte)(iLen >> 24);
                            temSet.DataBuffer[5] = (byte)(iLen >> 16);
                            temSet.DataBuffer[6] = (byte)(iLen >> 8);
                            temSet.DataBuffer[7] = (byte)iLen;
                            temSet.DataBuffer[11] = 2;
                            for (int i = 0; i < 10; i++)
                            {
                                temSet.DataBuffer[i + 16] = FINSHeader[i];
                            }
                            temSet.DataBuffer[26] = 1;
                            temSet.DataBuffer[27] = 2;
                            temSet.DataBuffer[28] = 130;

                            iLen = nVar.ByteAddr + 1000;
                            temSet.DataBuffer[29] = (byte)(iLen >> 8);
                            temSet.DataBuffer[30] = (byte)iLen;
                            temSet.DataBuffer[33] = 1;
                            temSet.DataBuffer[34] = (byte)(iValue >> 8);
                            temSet.DataBuffer[35] = (byte)iValue;

                            nMsg.sSendMsg = temSet;
                            ListImmSendMsg.Add(nMsg); Debug.WriteLine("SendAODO:AO.ListImmSendMsg");
                            break;
                    }
                }
            }
        }

        //界面操作
        public void SendAODO(string sVar, int iValue, string sType)
        {
            if (sType == "SY")
            {
                SSend_Message temSet = new SSend_Message();
                int iLen;
                CMessage nMsg = new CMessage();

                temSet.Length = 36;
                temSet.DataBuffer = new byte[temSet.Length];
                for (int i = 0; i < 4; i++)
                {
                    temSet.DataBuffer[i] = FINSTCPHeader[i];
                }
                iLen = temSet.Length - 8;
                temSet.DataBuffer[4] = (byte)(iLen >> 24);
                temSet.DataBuffer[5] = (byte)(iLen >> 16);
                temSet.DataBuffer[6] = (byte)(iLen >> 8);
                temSet.DataBuffer[7] = (byte)iLen;
                temSet.DataBuffer[11] = 2;
                for (int i = 0; i < 10; i++)
                {
                    temSet.DataBuffer[i + 16] = FINSHeader[i];
                }
                temSet.DataBuffer[26] = 1;
                temSet.DataBuffer[27] = 2;
                temSet.DataBuffer[28] = 130;

                iLen = Convert.ToInt32(sVar);
                temSet.DataBuffer[29] = (byte)(iLen >> 8);
                temSet.DataBuffer[30] = (byte)iLen;
                temSet.DataBuffer[33] = 1;
                temSet.DataBuffer[34] = (byte)(iValue >> 8);
                temSet.DataBuffer[35] = (byte)iValue;

                nMsg.sSendMsg = temSet;
                ListImmSendMsg.Add(nMsg);

                CAlarmMsgEventArgs e = new CAlarmMsgEventArgs();
                e.Date_Time = DateTime.Now;
                e.Recorder = GetMsg(sVar, iValue);
                e.priority = EAlarmPriority.PRIORITY_1;
                e.eAlarmType = EAlarmType.ManualAct;
                e.StaName = Name;
                if (e.Recorder != "")
                    staAlarm.OnAlarmEvent(e);
                return;
            }
            foreach (CVar nVar in StaDevice.ListDevVar)
            {
                if (nVar.Name == sVar)
                {
                    SSend_Message temSet = new SSend_Message();
                    int iLen;
                    CMessage nMsg = new CMessage();
                    switch (sType)
                    {
                        case "AO"://AO 需要处理变比
                            iValue = (int)Math.Round(iValue / nVar.RatioValue);
                            temSet.Length = 36;
                            temSet.DataBuffer = new byte[temSet.Length];
                            for (int i = 0; i < 4; i++)
                            {
                                temSet.DataBuffer[i] = FINSTCPHeader[i];
                            }
                            iLen = temSet.Length - 8;
                            temSet.DataBuffer[4] = (byte)(iLen >> 24);
                            temSet.DataBuffer[5] = (byte)(iLen >> 16);
                            temSet.DataBuffer[6] = (byte)(iLen >> 8);
                            temSet.DataBuffer[7] = (byte)iLen;
                            temSet.DataBuffer[11] = 2;
                            for (int i = 0; i < 10; i++)
                            {
                                temSet.DataBuffer[i + 16] = FINSHeader[i];
                            }
                            temSet.DataBuffer[26] = 1;
                            temSet.DataBuffer[27] = 2;
                            temSet.DataBuffer[28] = 130;

                            iLen = nVar.ByteAddr + 1000;
                            temSet.DataBuffer[29] = (byte)(iLen >> 8);
                            temSet.DataBuffer[30] = (byte)iLen;
                            temSet.DataBuffer[33] = 1;
                            temSet.DataBuffer[34] = (byte)(iValue >> 8);
                            temSet.DataBuffer[35] = (byte)iValue;

                            nMsg.sSendMsg = temSet;
                            ListImmSendMsg.Add(nMsg); Debug.WriteLine("SendAODO:AO.ListImmSendMsg");
                            break;
                        case "AO2"://AO2 不需要处理变比
                            temSet.Length = 36;
                            temSet.DataBuffer = new byte[temSet.Length];
                            for (int i = 0; i < 4; i++)
                            {
                                temSet.DataBuffer[i] = FINSTCPHeader[i];
                            }
                            iLen = temSet.Length - 8;
                            temSet.DataBuffer[4] = (byte)(iLen >> 24);
                            temSet.DataBuffer[5] = (byte)(iLen >> 16);
                            temSet.DataBuffer[6] = (byte)(iLen >> 8);
                            temSet.DataBuffer[7] = (byte)iLen;
                            temSet.DataBuffer[11] = 2;
                            for (int i = 0; i < 10; i++)
                            {
                                temSet.DataBuffer[i + 16] = FINSHeader[i];
                            }
                            temSet.DataBuffer[26] = 1;
                            temSet.DataBuffer[27] = 2;
                            temSet.DataBuffer[28] = 130;

                            iLen = nVar.ByteAddr + 1000;
                            temSet.DataBuffer[29] = (byte)(iLen >> 8);
                            temSet.DataBuffer[30] = (byte)iLen;
                            temSet.DataBuffer[33] = 1;
                            temSet.DataBuffer[34] = (byte)(iValue >> 8);
                            temSet.DataBuffer[35] = (byte)iValue;

                            nMsg.sSendMsg = temSet;
                            ListImmSendMsg.Add(nMsg); Debug.WriteLine("SendAODO:AO2.ListImmSendMsg");
                            break;
                        case "DO"://DO
                            int iByte = nVar.ByteAddr - 1000;
                            int iBit = nVar.BitAddr;
                            if (nVar.GetBoolValue())
                                iValue = 0;
                            else
                                iValue = 1;
                            temSet.Length = 35;
                            temSet.DataBuffer = new byte[temSet.Length];
                            for (int i = 0; i < 4; i++)
                            {
                                temSet.DataBuffer[i] = FINSTCPHeader[i];
                            }
                            iLen = temSet.Length - 8;
                            temSet.DataBuffer[4] = (byte)(iLen >> 24);
                            temSet.DataBuffer[5] = (byte)(iLen >> 16);
                            temSet.DataBuffer[6] = (byte)(iLen >> 8);
                            temSet.DataBuffer[7] = (byte)iLen;
                            temSet.DataBuffer[11] = 2;
                            for (int i = 0; i < 10; i++)
                            {
                                temSet.DataBuffer[i + 16] = FINSHeader[i];
                            }
                            temSet.DataBuffer[26] = 1;
                            temSet.DataBuffer[27] = 2;
                            temSet.DataBuffer[28] = 2;

                            temSet.DataBuffer[29] = (byte)(iByte >> 8);
                            temSet.DataBuffer[30] = (byte)iByte;
                            temSet.DataBuffer[31] = (byte)(iBit);
                            temSet.DataBuffer[33] = 1;
                            //iValue = 0;
                            temSet.DataBuffer[34] = (byte)iValue;

                            nMsg.sSendMsg = temSet;
                            ListImmSendMsg.Add(nMsg); Debug.WriteLine("SendAODO:DO.ListImmSendMsg");
                            // ListImmSendMsg.Add(SendMag8000);
                            break;
                        case "DO2"://DO
                            iByte = nVar.ByteAddr - 1000;
                            iBit = nVar.BitAddr;
                            temSet.Length = 35;
                            temSet.DataBuffer = new byte[temSet.Length];
                            for (int i = 0; i < 4; i++)
                            {
                                temSet.DataBuffer[i] = FINSTCPHeader[i];
                            }
                            iLen = temSet.Length - 8;
                            temSet.DataBuffer[4] = (byte)(iLen >> 24);
                            temSet.DataBuffer[5] = (byte)(iLen >> 16);
                            temSet.DataBuffer[6] = (byte)(iLen >> 8);
                            temSet.DataBuffer[7] = (byte)iLen;
                            temSet.DataBuffer[11] = 2;
                            for (int i = 0; i < 10; i++)
                            {
                                temSet.DataBuffer[i + 16] = FINSHeader[i];
                            }
                            temSet.DataBuffer[26] = 1;
                            temSet.DataBuffer[27] = 2;
                            temSet.DataBuffer[28] = 2;

                            temSet.DataBuffer[29] = (byte)(iByte >> 8);
                            temSet.DataBuffer[30] = (byte)iByte;
                            temSet.DataBuffer[31] = (byte)(iBit);
                            temSet.DataBuffer[33] = 1;
                            //iValue = 0;
                            temSet.DataBuffer[34] = (byte)iValue;

                            nMsg.sSendMsg = temSet;
                            ListImmSendMsg.Add(nMsg); Debug.WriteLine("SendAODO:DO2.ListImmSendMsg");
                            // ListImmSendMsg.Add(SendMag8000);
                            break;
                    }
                }
            }
        }

        public void SendWriteIdle()//下发写Idle值
        {
            for (int kk = 5; kk < 7; kk++)
            {
                SSend_Message temSet = new SSend_Message();
                int iLen = intEnd - intStart;
                CMessage nMsg = new CMessage();
                temSet.Length = 34 + iLen * 2;
                temSet.DataBuffer = new byte[temSet.Length];
                for (int i = 0; i < 4; i++)
                {
                    temSet.DataBuffer[i] = FINSTCPHeader[i];
                }
                int iNum = temSet.Length - 8;
                temSet.DataBuffer[4] = (byte)(iNum >> 24);
                temSet.DataBuffer[5] = (byte)(iNum >> 16);
                temSet.DataBuffer[6] = (byte)(iNum >> 8);
                temSet.DataBuffer[7] = (byte)iNum;
                temSet.DataBuffer[11] = 2;
                for (int i = 0; i < 10; i++)
                {
                    temSet.DataBuffer[i + 16] = FINSHeader[i];
                }
                temSet.DataBuffer[26] = 1;
                temSet.DataBuffer[27] = 2;
                temSet.DataBuffer[28] = 130;

                temSet.DataBuffer[29] = (byte)((intStart + (kk - 3) * 1000) >> 8);
                temSet.DataBuffer[30] = (byte)(intStart + (kk - 3) * 1000);
                temSet.DataBuffer[33] = (byte)iLen;
                foreach (CVar nVar in StaDevice.ListDevVar)
                {
                    if (nVar.ByteAddr >= intStart && nVar.ByteAddr < intEnd)
                    {
                        int ibyte = nVar.ByteAddr;
                        if (ibyte < intStart || ibyte > intEnd)
                            continue;
                        long iValue = (long)nVar.IntTag[kk];
                        temSet.DataBuffer[34 + (ibyte - intStart) * 2] = (byte)(iValue >> 8);
                        temSet.DataBuffer[35 + (ibyte - intStart) * 2] = (byte)iValue;
                    }
                }
                nMsg.sSendMsg = temSet;
                ListImmSendMsg.Add(nMsg); Debug.WriteLine("SendWriteIdle.ListImmSendMsg");
            }
        }

        public void SendReadIdle()//下发读Idle报文
        {
            for (int kk = 2; kk < 4; kk++)
            {
                SSend_Message temSend = new SSend_Message();
                CMessage nMsg = new CMessage();
                temSend.Length = 34;
                temSend.DataBuffer = new byte[temSend.Length];
                for (int i = 0; i < 4; i++)
                {
                    temSend.DataBuffer[i] = FINSTCPHeader[i];
                }
                int iLen = temSend.Length - 8;
                temSend.DataBuffer[4] = (byte)(iLen >> 24);
                temSend.DataBuffer[5] = (byte)(iLen >> 16);
                temSend.DataBuffer[6] = (byte)(iLen >> 8);
                temSend.DataBuffer[7] = (byte)iLen;
                temSend.DataBuffer[11] = 2;
                for (int i = 0; i < 10; i++)
                {
                    temSend.DataBuffer[i + 16] = FINSHeader[i];
                }
                string Function = "010182";
                for (int i = 0; i < 3; i++)
                {
                    temSend.DataBuffer[i + 26] = Convert.ToByte(Function.Substring(i * 2, 2), 16);
                }
                int iStart = intStart + kk * 1000;
                temSend.DataBuffer[29] = (byte)(iStart >> 8);
                temSend.DataBuffer[30] = (byte)iStart;
                iLen = intEnd - intStart;
                temSend.DataBuffer[32] = (byte)(iLen >> 8);
                temSend.DataBuffer[33] = (byte)iLen;
                nMsg.sSendMsg = temSend;
                nMsg.Starting = iStart.ToString();
                ListImmSendMsg.Add(nMsg); Debug.WriteLine("SendReadIdle.ListImmSendMsg");
            }
        }

        /// <summary>
        /// 下发写多值
        /// </summary>
        /// <param name="SendBuff">发送的字节buf</param>
        /// <param name="intStart">起始地址</param>
        /// <param name="iLen">数据长度</param>
        public void SendRun(byte[] SendBuff, int intStart, int iLen)//下发配方
        {
            try
            {
                SSend_Message temSet = new SSend_Message();

                CMessage nMsg = new CMessage();
                temSet.Length = 34 + iLen * 2;
                temSet.DataBuffer = new byte[temSet.Length];
                for (int i = 0; i < 4; i++)
                {
                    temSet.DataBuffer[i] = FINSTCPHeader[i];
                }
                int iNum = temSet.Length - 8;
                temSet.DataBuffer[4] = (byte)(iNum >> 24);
                temSet.DataBuffer[5] = (byte)(iNum >> 16);
                temSet.DataBuffer[6] = (byte)(iNum >> 8);
                temSet.DataBuffer[7] = (byte)iNum;
                temSet.DataBuffer[11] = 2;
                for (int i = 0; i < 10; i++)
                {
                    temSet.DataBuffer[i + 16] = FINSHeader[i];
                }
                temSet.DataBuffer[26] = 1;
                temSet.DataBuffer[27] = 2;
                temSet.DataBuffer[28] = 130;

                temSet.DataBuffer[29] = (byte)(intStart >> 8);
                temSet.DataBuffer[30] = (byte)intStart;
                temSet.DataBuffer[33] = (byte)iLen;
                for (int i = 0; i < Math.Min(SendBuff.Length, iLen * 2); i++)
                {
                    temSet.DataBuffer[34 + i] = SendBuff[i];
                }
                nMsg.sSendMsg = temSet;
                ListImmSendMsg.Add(nMsg);
                //Debug.WriteLine("SendRun.ListImmSendMsg");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CProtcolFINS.SendRun" + ex.Message);
            }
        }

        //组装报文
        public void PacketLoopMsg(CMessage cMsg)
        {
            SSend_Message temSend = new SSend_Message();
            switch (cMsg.Function.Substring(0, 4))
            {
                case "0101":
                    temSend.Length = 34;
                    temSend.DataBuffer = new byte[temSend.Length];
                    for (int i = 0; i < 4; i++)
                    {
                        temSend.DataBuffer[i] = FINSTCPHeader[i];
                    }
                    int iLen = temSend.Length - 8;
                    temSend.DataBuffer[4] = (byte)(iLen >> 24);
                    temSend.DataBuffer[5] = (byte)(iLen >> 16);
                    temSend.DataBuffer[6] = (byte)(iLen >> 8);
                    temSend.DataBuffer[7] = (byte)iLen;
                    temSend.DataBuffer[11] = 2;
                    for (int i = 0; i < 10; i++)
                    {
                        temSend.DataBuffer[i + 16] = FINSHeader[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        temSend.DataBuffer[i + 26] = Convert.ToByte(cMsg.Function.Substring(i * 2, 2), 16);
                    }
                    iLen = Convert.ToInt32(cMsg.Starting);
                    temSend.DataBuffer[29] = (byte)(iLen >> 8);
                    temSend.DataBuffer[30] = (byte)iLen;
                    iLen = Convert.ToInt32(cMsg.Number);
                    temSend.DataBuffer[32] = (byte)(iLen >> 8);
                    temSend.DataBuffer[33] = (byte)iLen;
                    break;
                case "0102":
                    break;
                default:
                    break;
            }

            cMsg.sSendMsg = temSend;
            cMsg.ListMsgVar.Clear();

            // foreach (CVar nVar in StaDevice.ListDevVar)
            //  {
            //if (Convert.ToInt32(nVar.IntTag[0]) == cMsg.Message_No)
            //    cMsg.ListMsgVar.Add(nVar.Name, nVar);
            //  }
        }

        public bool PortDataRecv(CMessage sSend, byte[] cRecv, int iRecvLen)
        {
            try
            {
                if (iRecvLen < 12)
                {
                    ListStrMsg.Add("PortDataRecv:False:iRecvLen < 12");
                    return false;
                }
                for (int i = 0; i < iRecvLen - 11; i++)
                {
                    if (cRecv[i] == FINSTCPHeader[0] && cRecv[i + 1] == FINSTCPHeader[1] && cRecv[i + 2] == FINSTCPHeader[2] && cRecv[i + 3] == FINSTCPHeader[3])
                    {
                        int iLen = cRecv[i + 4] * 256 * 256 * 256 + cRecv[i + 5] * 256 * 256 + cRecv[i + 6] * 256 + cRecv[i + 7];
                        if (iRecvLen - i < iLen + 8)
                        {
                            ListStrMsg.Add("PortDataRecv:False:iRecvLen - i < iLen + 8");
                            return false;
                        }
                        byte[] Datebuff = new byte[iLen];
                        Array.Copy(cRecv, i + 8, Datebuff, 0, iLen);
                        if (iLen + 8 > 25)
                        {
                            if (Datebuff[17] != SID)
                            {
                                ListStrMsg.Add("PortDataRecv:False:Datebuff[17] != SID");
                                return false;
                            }
                        }

                        GetVarValue(sSend, Datebuff);
                        return true;

                    }
                }
                ListStrMsg.Add("PortDataRecv:False:for (int i = 0; i < iRecvLen - 11; i++)");
                return false;
            }
            catch (Exception ex)
            {
                ListStrMsg.Add("CProtcolFINS.PortDataRecv:" + ex.Message);
                Debug.WriteLine("CProtcolFINS.PortDataRecv:" + ex.Message);
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
                int iQuCommand = nMsg.sSendMsg.DataBuffer[8] * 256 * 256 * 256 + nMsg.sSendMsg.DataBuffer[9] * 256 * 256 + nMsg.sSendMsg.DataBuffer[10] * 256 + nMsg.sSendMsg.DataBuffer[11];
                int iReCommand = Datebuff[0] * 256 * 256 * 256 + Datebuff[1] * 256 * 256 + Datebuff[2] * 256 + Datebuff[3];
                int Errorcode = Datebuff[4] * 256 * 256 * 256 + Datebuff[5] * 256 * 256 + Datebuff[6] * 256 + Datebuff[7];
                if (Errorcode > 0)
                {
                    string sErr = "";
                    switch (Errorcode)
                    {
                        case 1: sErr = "The header is not ‘FINS’ (ASCII code)."; break;
                        case 2: sErr = "The data length is too long."; break;
                        case 3: sErr = "The command is not supported."; break;
                        case 32: sErr = "All connections are in use."; break;
                        case 33: sErr = "The specified node is already connected."; break;
                        case 34: sErr = Errorcode.ToString() + "(" + Errorcode.ToString("X2") + "H):Attempt to access a protected node from an unspecified IP address."; break;
                        case 35: sErr = Errorcode.ToString() + "(" + Errorcode.ToString("X2") + "H):The client FINS node address is out of range."; break;
                        case 36: sErr = Errorcode.ToString() + "(" + Errorcode.ToString("X2") + "H):The same FINS node address is being used by the client and server."; break;
                        case 37: sErr = Errorcode.ToString() + "(" + Errorcode.ToString("X2") + "H):All the node addresses available for allocation have been used."; break;
                        default: sErr = "No Details."; break;
                    }
                    CAlarmMsgEventArgs ee = new CAlarmMsgEventArgs();
                    ee.Date_Time = DateTime.Now;
                    ee.Recorder = "PLC报警:" + Errorcode.ToString() + "," + sErr;
                    ee.priority = EAlarmPriority.PRIORITY_2;
                    ee.eAlarmType = EAlarmType.StationState;
                    ee.StaName = Name;
                    staAlarm.OnAlarmEvent(ee);
                    Debug.WriteLine("CProtcolFINS.Errorcode" + sErr);

                    if (CommStateE == ECommSatate.Failure)
                    {
                        CommStateE = ECommSatate.Normal;//子站置通信恢复
                        CAlarmMsgEventArgs eea = new CAlarmMsgEventArgs();
                        eea.Date_Time = DateTime.Now;
                        eea.Recorder = Description + "通信恢复";
                        eea.priority = EAlarmPriority.PRIORITY_1;
                        eea.eAlarmType = EAlarmType.StationState;
                        eea.StaName = Name;
                        staAlarm.OnAlarmEvent(eea);
                    }
                    else if (CommStateE != ECommSatate.Normal)
                    {
                        CommStateE = ECommSatate.Normal;//子站置通信恢复
                    }
                    DelayTime = 0;
                    present_MsgFailRep = 0;
                    present_DevFailNum = 0;
                    return;
                }
                switch (iReCommand)
                {
                    case 1:
                        break;
                    case 2:
                        if (iReCommand == 2)
                        {
                            if (Datebuff[18] != nMsg.sSendMsg.DataBuffer[26] || Datebuff[19] != nMsg.sSendMsg.DataBuffer[27])
                            {
                                return;
                            }
                            if (Datebuff[18] == 1 && Datebuff[19] == 1)//读
                            {
                                int iStart = Convert.ToInt32(nMsg.Starting);
                                int iReadType = (int)((iStart + 1) / 1000);

                                if (iReadType < 8)
                                {
                                    iStart = iStart - (iReadType - 1) * 1000;
                                    if (iReadType == 1)
                                    {
                                        for (int i = 0; i < 50; i++)
                                        {
                                            int n = i * 2 + 22;
                                            System[i] = Datebuff[n] * 256 + Datebuff[n + 1];
                                        }
                                    }
                                    //foreach (KeyValuePair<string, CVar> kvp in nMsg.ListMsgVar)
                                    foreach (CVar nVar in StaDevice.ListDevVar)
                                    {
                                        if (nVar.DAType != EDAType.DA_YC)
                                            continue;
                                        int iIndex = nVar.ByteAddr - iStart;
                                        int iBit =nVar.BitAddr;
                                        int iLen = nVar.Length;
                                        Int64 Value = 0;
                                        int nByte = iIndex * 2 + 22;
                                        if (nByte + iLen / 8 > Datebuff.Length || nByte < 0 || iIndex < 0)
                                            continue;
                                        switch (iLen)
                                        {
                                            case 1:
                                                int aa = (int)(Math.Pow(2, iBit));
                                                if (nVar.bHighLow == 1)
                                                    Value = (Datebuff[nByte] * 256 + Datebuff[nByte + 1]) & aa;
                                                else
                                                    Value = (Datebuff[nByte] + Datebuff[nByte + 1] * 256) & aa;
                                                if (Value > 0) Value = 1;
                                                break;
                                            case 8:
                                                aa = (int)((iBit + 1) / 8);
                                                if (nVar.bHighLow == 1)
                                                    Value = Datebuff[nByte + 1 - aa];
                                                else
                                                    Value = Datebuff[nByte + aa];
                                                break;
                                            case 16:
                                                if (nVar.bHighLow == 1)
                                                    Value = Datebuff[nByte] * 256 + Datebuff[nByte + 1];
                                                else
                                                    Value = Datebuff[nByte] + Datebuff[nByte + 1] * 256;
                                                break;
                                            case 32:
                                                if (nVar.bHighLow == 1)
                                                    Value = Datebuff[nByte] * 256 * 256 * 256 + Datebuff[nByte + 1] * 256 * 256 + Datebuff[nByte + 2] * 256 + Datebuff[nByte + 3];
                                                else
                                                    Value = Datebuff[nByte] + Datebuff[nByte + 1] * 256 + Datebuff[nByte + 2] * 256 * 256 + Datebuff[nByte + 3] * 256 * 256 * 256;
                                                break;
                                            default:
                                                break;
                                        }
                                        if (iReadType == 1)
                                        {
                                            nVar.SetValue(Value);
                                        }
                                        else if (iReadType == 2)
                                        {
                                            nVar.PLCValue[iReadType] = (Int64)Math.Round(Value * nVar.RatioValue + nVar.BaseValue);
                                        }
                                        else
                                        {
                                            nVar.PLCValue[iReadType] = (Int64)Value;
                                        }
                                    }
                                    CheckRunState();
                                }
                                else if (iReadType == 8)//DO
                                {
                                    foreach (CVar nVar in StaDevice.ListDevVar)
                                    {
                                        if (nVar.DAType != EDAType.DA_YX)
                                            continue;

                                        int iIndex =nVar.ByteAddr - iStart;
                                        int iBit = nVar.BitAddr;
                                        int iLen = nVar.Length;
                                        Int64 Value = 0;
                                        int nByte = iIndex * 2 + 22;
                                        if (nByte + iLen / 8 > Datebuff.Length || nByte < 0)
                                            continue;

                                        int aa = (int)(Math.Pow(2, iBit));
                                        if (nVar.bHighLow == 1)
                                            Value = (Datebuff[nByte] * 256 + Datebuff[nByte + 1]) & aa;
                                        else
                                            Value = (Datebuff[nByte] + Datebuff[nByte + 1] * 256) & aa;
                                        if (Value > 0) Value = 1;

                                        nVar.SetValue(Value);
                                    }
                                }
                                else if (iReadType == 9)//DI
                                {
                                    foreach (CVar nVar in StaDevice.ListDevVar)
                                    {
                                        if (nVar.DAType != EDAType.DA_YX)
                                            continue;
                                        int iIndex = nVar.ByteAddr - iStart;
                                        int iBit = nVar.BitAddr;
                                        int iLen = nVar.Length;
                                        Int64 Value = 0;
                                        int nByte = iIndex * 2 + 22;
                                        if (nByte + iLen / 8 > Datebuff.Length || nByte < 0)
                                            continue;

                                        int aa = 1 << iBit;

                                        if (nVar.bHighLow == 1)
                                        {
                                            Value = (Datebuff[nByte] * 256 + Datebuff[nByte + 1]) & aa;
                                        }
                                        else
                                        {
                                            Value = (Datebuff[nByte] + Datebuff[nByte + 1] * 256) & aa;
                                        }
                                        if (Value > 0) Value = 1;

                                        nVar.SetValue(Value);

                                    }
                                }
                            }
                            else if (Datebuff[18] == 1 && Datebuff[19] == 2)//写
                            {
                                //Debug.WriteLine("CProtcolFINS.PLC0102");
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (CommStateE == ECommSatate.Failure)
                {
                    CommStateE = ECommSatate.Normal;//子站置通信恢复
                    CAlarmMsgEventArgs ee = new CAlarmMsgEventArgs();
                    ee.Date_Time = DateTime.Now;
                    ee.Recorder = Description + "通信恢复";
                    ee.priority = EAlarmPriority.PRIORITY_1;
                    ee.eAlarmType = EAlarmType.StationState;
                    ee.StaName = Name;
                    //staAlarm.OnAlarmEvent(ee);
                }
                else if (CommStateE != ECommSatate.Normal)
                {
                    CommStateE = ECommSatate.Normal;//子站置通信恢复
                }
                DelayTime = 0;
                present_MsgFailRep = 0;
                present_DevFailNum = 0;
            }
        }
    }  
}
