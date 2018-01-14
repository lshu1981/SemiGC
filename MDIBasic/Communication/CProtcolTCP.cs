using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace LSSCADA
{
    public class CProtcolTCP : CStation
    {
        public TcpClient client;
        public NetworkStream netstream;

        protected String _ServerIP;
        protected int _ServerPort;

        protected int DelayTime = 0;           //发送后等待延时
        protected List<CMessage> ListImmSendMsg = new List<CMessage>();//优先发送的报文队列

        protected System.Timers.Timer CommTimer;// = new Timer(10000);
        protected Thread TPortThread = null;             //通信线程
        protected CRecvBuff cRecvBuff = new CRecvBuff();//接收到的数据

        protected CMessage LastSendMsg = new CMessage();   //上次发送的报文

        public bool bOpen = false;//打开标示

        protected List<string> ListStrMsg = new List<string>();
        protected int ListStrMsgMax = 2000;

        public CProtcolTCP()
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
            string[] sPortSet = Setting.Split(':');

            _ServerIP = sPortSet[0];
            _ServerPort = Convert.ToInt32(sPortSet[1]);

            ListImmSendMsg.Clear();
            return true;
        }

        public void Show(bool bb)
        {
            bDebug = bb;
            if (bDebug)
            {
                frmShow = new Modbus报文查看();
                frmShow.ShowDialog();
            }
            else
            {
                frmShow = null;
            }

        }


        public void SaveDebugMsg(string sState)//打开一个文件（如果文件不存在则创建该文件）并将信息追加到文件末尾
        {
            return;
            string sDir = CProject.sPrjPath + "\\Debug\\";
            string sFile = sDir + Name + "_"+ sState + DateTime.Now.ToString("_yyMMddHHmmss") + ".txt";

            FileStream aFile = new FileStream(sFile,FileMode.Append, FileAccess.Write, FileShare.Write);
            aFile.Close();
            StreamWriter sw = new StreamWriter(sFile, true, Encoding.Unicode);
            foreach (string str1 in ListStrMsg)
            {
                sw.WriteLine(str1);
            }
            sw.Close();
        }

        public virtual bool ConnectServer() //连接TCP Server
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
                    Debug.WriteLine("TCP.ConnectServer1:" + ee.ToString());
                    return false;
                }

                netstream = client.GetStream();
                Socket s = client.Client;
                return true;
            }
            catch (Exception e)
            {
                CommStateE = ECommSatate.Unknown;
                Debug.WriteLine("TCP.ConnectServer2:" + e.ToString());
                return false;
            }

        }
    }
}
