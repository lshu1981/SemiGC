using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PublicDll;
using System.Collections;

namespace LSSCADA.Control
{
    public partial class frmCComm : Form
    {
        string[] sCol1 = new string[] { "设备名称", "设备描述", "通信端口", "子站地址", "子站设置", "通信状态", "网络状态" };
        bool[] bCol1 = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        private System.Timers.Timer CommTimer;// = new Timer(1000);
        int iIndex = 0;

        public frmCComm()
        {
            InitializeComponent();
            FillTreeList();
            CommTimer = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为1000毫秒； 
            CommTimer.Elapsed += new System.Timers.ElapsedEventHandler(CommTimerCall);//到达时间的时候执行事件； 
            CommTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
            CommTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件；

            CPublicDGV.InitializeDGV(dGV1, sCol1, bCol1, true);
        }

        private void FillTreeList()
        {
            treeView1.Nodes.Add("Prj.", CProject.sPrjName);
            foreach (CPort nPort in frmMain.staComm.ListPort)
            {
                //treeView1.Nodes[0].Nodes.Add(nPort.PortName,nPort.PortDescript);
                TreeNode nNode = new TreeNode(nPort.PortName);
                nNode.Name = "Prt." + frmMain.staComm.ListPort.IndexOf(nPort);
                treeView1.Nodes[0].Nodes.Add(nNode);
            }

            treeView1.ExpandAll();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode node = treeView1.SelectedNode;
                string sType = node.Name.Substring(0, 3);
                string sName = node.Name.Substring(4);
                CommTimer.Enabled = false;
                iIndex = 0;
                dGV1.Rows.Clear();
                switch (sType)
                {
                    case "Prj":
                        break;
                    case "Prt":
                        AddPortToTable(Int32.Parse(sName));
                        UpdateValue();
                        break;
                    default:
                        break;
                }
                CommTimer.Enabled = true;
            }
            catch (Exception ee)
            {
                MessageBox.Show("treeView1_AfterSelect:" + ee.Message);
            }
        }

        private void AddPortToTable(int iIndex)
        {
            if (iIndex < 0)
                return;

            CPort nPort = (CPort)frmMain.staComm.ListPort[iIndex];

            ArrayList obj = new ArrayList();
            string[] str1;
            foreach (CStation nSta in nPort.ListStation)
            {
                //{ "设备名称", "设备描述", "通信端口", "子站地址", "子站设置", "通信状态", "网络状态" };
                str1 = new string[sCol1.Length];
                int k = 0;
                str1[k++] = nSta.Name;
                str1[k++] = nSta.Description;
                str1[k++] = nSta.PortName;
                str1[k++] = nSta.Setting;
                str1[k++] = nSta.CommStateS;
                str1[k++] = nSta.RunStateS;
                obj.Add(str1);
            }
            dGV1.Rows.Clear();
            foreach (string[] rowArray in obj)
            {
                dGV1.Rows.Add(rowArray);
            }
        }

        //定时器
        public void CommTimerCall(object source, System.Timers.ElapsedEventArgs e)
        {
            UpdateValue();
        }

        private void UpdateValue()
        {
            try
            {
                for (int i = 0; i < dGV1.Rows.Count; i++)
                {
                    string sSta = (string)dGV1.Rows[i].Cells[0].Value;
                    CStation nSta = frmMain.staComm.GetStaByStaName(sSta);

                    dGV1.Rows[i].Cells[5].Value = nSta.CommStateS;
                    dGV1.Rows[i].Cells[6].Value = nSta.RunStateS;
                    dGV1.Rows[i].DefaultCellStyle.BackColor = nSta.CommStateC;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "警告");
            }
        }

        private void dGV1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string sSta = (string)dGV1.Rows[e.RowIndex].Cells[0].Value;
            CStation nSta = frmMain.staComm.GetStaByStaName(sSta);
            if (nSta == null)
                return;
            if (nSta.StaDevice.PortProtocol == "Modbus_TCP"  )
            {
                CProtcolModbusTCP nnn = (CProtcolModbusTCP)nSta;
                nnn.Show(true);
                nnn.bDebug = false;
            }
            if (nSta.StaDevice.PortProtocol == "FINS_TCP")
            {
                CProtcolFINS nnn = (CProtcolFINS)nSta;
                nnn.Show(true);
                nnn.bDebug = false;
 
            }
        }
    }
}
