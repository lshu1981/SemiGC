using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace LSSCADA
{
    public class CProtcolTran :CProtcolTCP
    {
        public LSSCADA.Control.frmC机械手 fm;
        
        public int mDelayTime = 400;           //发送后等待延时

        //public CRecvBuff cRecvBuff = new CRecvBuff();//接收到的数据

        public List<string> ListImmSendMsg = new List<string>();//发送报文队列
        public string LastSendMsg = "";
        public string smRQPOS = "RQ POS ABS ALL\r\n";
        public string smRESET = "RESET\r\n";
        public string smSetLoadOFF = "Set load off\r\n";
        public string smSetLoadON = "Set load on\r\n";

        public double[] RTZ = new double[3] { 0, 0, 0 };
        public double[][] STN = new double[5][];
        public bool bSTN = true;
        public double R
        {
            get { return RTZ[0]; }
        }
        public double T
        {
            get { return RTZ[1]; }
        }
        public double Z
        {
            get { return RTZ[2]; }
        }

        public string ActionType = "";
        public string[] ActionCon = new string[3];
        public int ActionStep = -1;

        public int iSVON = 0;//上电次数
        public CProtcolTran()
        {

        }

        public bool Open() //组装所有读报文
        {
            base.Open();

            client = new TcpClient();

            for (int i = 0; i < 5; i++)
            {
                double[] stn1 = new double[6] { 0, 0, 0, 0, 0, 0 };
                STN[i] = stn1;
            }
            //bOpen = ConnectServer();
            //if(bOpen)
            //    GetSendMsg();
            //Write("test12345");
            return true;
        }

        public bool ConnectServer() //连接TCP Server
        {
            try
            {
                client = new TcpClient();
                try
                {
                    client.Connect(_ServerIP, _ServerPort);
                }
                catch (Exception ee)
                {
                    CommStateE = ECommSatate.Unknown;
                    fm.InsertText(ee.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                CommStateE = ECommSatate.Unknown;
                fm.InsertText(e.Message);
                return false;
            }

            CommStateE = ECommSatate.Failure;
            netstream = client.GetStream();
            GetSendMsg();
            return true;
        }

        public bool Close() //组装所有读报文
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

        //定时器
        public void CommTimerCall()
        {
            if (!bOpen)
                return;
            if (CommStateE == ECommSatate.Unknown)
            {
                return;
            }
            try
            {
                DelayTime += 100;
                if (DelayTime >= mDelayTime && DelayTime >= 2000)//超时
                {
                    DelayTime = 0;
                    present_MsgFailRep++;//子站重发次数+1

                    if (present_MsgFailRep > 2)//超过最大重发次数
                    {
                        present_MsgFailRep = 0;//子站重发次数置0
                        present_DevFailNum++;//子站失败次数置+1
                        if (present_DevFailNum > 3)//超过最大失败次数
                        {
                            CommStateE = ECommSatate.Failure;//子站置通信故障
                            GetSendMsg();//失败次数没达到，继续发送下个子站报文
                        }
                        else
                        {
                            GetSendMsg();//失败次数没达到，继续发送下个子站报文
                        }
                    }
                    else
                    {
                        Write(LastSendMsg);
                    }
                }
            }
            catch (Exception e1)
            {
                fm.InsertText("CommTimerCall" + e1.Message);

            }
            //Debug.WriteLine("Send");
        }

        public string GetSendMsg()
        {
            try
            {
                if (CommStateE != ECommSatate.Failure)
                {
                    lock (this)
                    {
                        if (ListImmSendMsg.Count > 0)
                        {
                            LastSendMsg = ListImmSendMsg[0];
                            Write(LastSendMsg);
                            ListImmSendMsg.RemoveAt(0);
                            return LastSendMsg;
                        }
                    }
                }
                LastSendMsg = smRQPOS;
                Write(LastSendMsg);
                return LastSendMsg;
            }
            catch (Exception e)
            {
                fm.InsertText("GetSendMsg" + e.Message);
            }
            return "NULL";
        }

        public bool Write(string sSend)
        {
            if (CommStateE != ECommSatate.Unknown)
            {//
                try
                {
                    //n_SerialPort.WriteLine(sSendData);
                    byte[] data = Encoding.Default.GetBytes(sSend);
                    netstream.Write(data, 0, sSend.Length);
                    fm.InsertText(sSend);
                    /*                    if (sSend.IndexOf("RQ POS") >= 0)
                                        {
                                            //fm.InsertText("PQ");
                                        }
                                        else
                                        {
                                            fm.InsertText(sSend);
                                        }*/
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public bool PortDataRecv(string sSend, string sRecv)
        {
            DelayTime = 0;
            present_MsgFailRep = 0;
            present_DevFailNum = 0;
            CommStateE = ECommSatate.Normal;
            try
            {
                if (sRecv.IndexOf("POS ABS R") >= 0)
                {
                    string sStr1 = sRecv.Substring(sRecv.IndexOf("POS ABS R"));
                    //fm.InsertText(sRecv);
                    string[] sValue = sStr1.Split(' ');
                    if (sValue.Length > 8)
                    {
                        for (int j = 0; j < sValue.Length; j++)
                        {

                            if (sValue[j] == "R")
                            {
                                RTZ[0] = Convert.ToDouble(sValue[j + 2]);
                            }
                            else if (sValue[j] == "T")
                            {
                                RTZ[1] = Convert.ToDouble(sValue[j + 1]);
                            }
                            else if (sValue[j] == "Z")
                            {
                                RTZ[2] = Convert.ToDouble(sValue[j + 1]);
                            }

                        }
                    }
                }
                else if (sRecv.IndexOf("STN ") >= 0)
                {
                    string sStr1 = sRecv.Substring(sRecv.IndexOf("STN "));
                    bSTN = true;
                    //fm.InsertText(sRecv);
                    string[] sValue = sStr1.Split(' ');
                    if (sValue.Length > 13)
                    {
                        int i = Convert.ToInt32(sValue[1]);
                        if (i < 1 || i > 4) return false;
                        double[] stn1 = new double[6];
                        for (int j = 0; j < 6; j++)
                        {
                            stn1[j] = Convert.ToDouble(sValue[5 + 2 * j]);
                        }
                        STN[i] = stn1;
                    }
                }
                else if (sRecv.IndexOf("_ERR") >= 0)
                {
                    int iErr = Convert.ToInt32(sRecv.Substring(sRecv.IndexOf("_ERR") + 5));
                    switch (iErr)
                    {
                        case 5200://没上电
                            //  fm.InsertText("错误" + iErr.ToString("0000") + " 没上电\r\n");
                            //ListImmSendMsg.Add("RESET\r\n");
                            if (iSVON < 3)
                            {
                                ListImmSendMsg.Add("SVON\r\n");
                                ListImmSendMsg.Add("RESET\r\n");
                                if (ActionStep >= 1)
                                {
                                    ListImmSendMsg.Add(ActionCon[ActionStep]);
                                }
                                iSVON++;
                            }
                            break;
                        case 194://急停未打开
                            ActionType = "";
                            ActionStep = 0;
                            //  fm.InsertText("错误" + iErr.ToString("0000") + " 试教盒急停未解除\r\n");
                            break;
                        default:
                            //   fm.InsertText(sRecv + "(" + LastSendMsg);
                            break;
                    }
                    Thread.Sleep(300);
                }
                else if (sRecv.IndexOf("BG_RDY") >= 0)
                {
                    // fm.InsertText(sRecv + "\r\n");
                    if (ActionType == "XFER" && ActionStep == 1)
                    {
                        ActionStep = 2;
                        ListImmSendMsg.Add(smSetLoadON);
                        ListImmSendMsg.Add(ActionCon[ActionStep]);
                    }
                    else
                    {
                        ActionType = "";
                        ActionStep = 0;
                    }
                }
                else if (sRecv.IndexOf("HALT") >= 0)
                {
                    ActionType = "";
                    ActionStep = 0;
                }
                else
                {
                    //fm.InsertText(sRecv + "\r\n");
                }
            }
            catch (Exception ee)
            {
                fm.InsertText(ee.Message);
            }
            return false;
        }

        //界面操作
        public void SendAODO(string sCommand)
        {
            ActionCon = new string[3];
            ActionType = sCommand.Substring(0, 4).ToUpper();
            ActionStep = 1;
            ActionCon[ActionStep] = sCommand;
            switch (ActionType)
            {
                case "PICK":
                    ListImmSendMsg.Add(smSetLoadOFF);
                    break;
                case "PLAC":
                    ListImmSendMsg.Add(smSetLoadON);
                    break;
                case "XFER":
                    string[] sStation = sCommand.Split(' ');
                    if (sStation.Length < 3)
                        return;
                    ActionCon[1] = "PICK " + sStation[1] + "\r\n";
                    ActionCon[2] = "PLACE " + sStation[2] + "\r\n";
                    ListImmSendMsg.Add(smSetLoadOFF);
                    break;
                case "HALT":
                    ActionType = "";
                    ActionStep = 0;
                    ListImmSendMsg.Add(sCommand);
                    return;
                default:
                    break;
            }
            iSVON = 0;
            ListImmSendMsg.Add(ActionCon[ActionStep]);
        }

        //界面操作
        public void SendHandAODO(string sCommand)
        {
            if (sCommand.Length < 6)
                return;
            iSVON = 0;
            ListImmSendMsg.Add(sCommand);
        }

    }
}
