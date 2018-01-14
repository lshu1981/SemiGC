using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace LSSCADA
{
    class CPortTCPClient : CPort
    {
        //打开以太网口，建立连接
        public override bool Open()
        {
            base.Open();

            //if (PortName != "Com08")
            //    return false;
            foreach (CStation nSta in ListStation)
            {
                nSta.Open();
            }
            bOpen = true;

            return bOpen;
        }

        public override bool Close()
        {
            base.Close();

            //if (PortName != "Com08")
            //    return false;
            foreach (CStation nSta in ListStation)
            {
                nSta.Close();
            }
            bOpen = false;

            return bOpen;
        }

        protected bool Check(String szIPInfo)
        {
            String[] split = szIPInfo.Split(';');
            if (split.Length > 2)
                MessageBox.Show("请输入正确格式的IP地址与端口号字符串，型如：192.168.1.1:502或！192.168.1.2:5002;192.168.1.3:5003", "提示", MessageBoxButtons.OK);
            if (split.Length == 1)
            {
                if (!CheckPortID(szIPInfo))
                    return false;
                return true;
            }
            else
            {
                String TCPServerAddress = split[0];
                if (!CheckPortID(TCPServerAddress))
                    return false;
                TCPServerAddress = split[1];
                if (!CheckPortID(TCPServerAddress))
                    return false;
            }
            return true;
        }

        protected bool CheckPortID(String TCPServerAddress)
        {
            int ColonPos = TCPServerAddress.IndexOf(":");
            if (ColonPos > 0)
            {
                String[] split = TCPServerAddress.Split(':');
                int PortSource = System.Convert.ToInt32(split[1]);
                if (PortSource < 100 || PortSource > 9999)
                {
                    MessageBox.Show("使用TCP模式时端口号范围是100～9999！", "提示", MessageBoxButtons.OK);
                    return false;
                }
                if (!CheckIP(split[0]))
                {
                    MessageBox.Show("请输入正确格式的IP地址与端口号字符串，型如：192.168.1.1:502！", "提示", MessageBoxButtons.OK);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("请输入正确格式的IP地址与端口号字符串，型如：192.168.1.1:502！", "提示", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }
        protected bool CheckIP(String szRemoteIP)
        {
            String[] split = szRemoteIP.Split('.');
            int i = 0;
            IEnumerator myEnum = split.GetEnumerator();
            while (myEnum.MoveNext())
            {
                String s = (String)(myEnum.Current);
                try
                {
                    int IPSec = System.Convert.ToInt32(s);
                    if (i == 0)
                    {
                        if (IPSec < 1 || IPSec > 223)
                            return false;
                    }
                    if (IPSec > 255)
                        return false;

                }
                catch (Exception e)
                {
                    return false;
                }
                i++;
            }
            if (i < 4)
                return false;
            return true;
        }


        public CPortTCPClient()
            : base()
        {
        }
        public override bool LoadFromNode(XmlElement Node)
        {
            //加载设备属性
            base.LoadFromNode(Node);
            return true;
        }

    }
}
