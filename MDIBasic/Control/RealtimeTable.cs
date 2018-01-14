using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using PublicDll;

namespace LSSCADA
{
    public partial class RealtimeTable : Form
    {
        int iIndex = 0;
        private System.Timers.Timer CommTimer;// = new Timer(1000);
        
        public RealtimeTable()
        {
            InitializeComponent();
            string[] sCol2 = new string[] { "序号", "子站名称", "变量名称", "变量值", "变量描述", "单位", "数据类型", "读写属性" };
            bool[] bCol2 = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};

            CPublicDGV.InitializeDGV(dataGridView1, sCol2, bCol2, true);
            FillTreeList();
            CommTimer = new System.Timers.Timer(1000);//实例化Timer类，设置间隔时间为1000毫秒； 
            CommTimer.Elapsed += new System.Timers.ElapsedEventHandler(CommTimerCall);//到达时间的时候执行事件； 
            CommTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
            CommTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        private void FillTreeList()
        {
            treeView1.Nodes.Add("Prj.", CProject.sPrjName);
            foreach (CPort nPort in frmMain.staComm.ListPort)
            {
                //treeView1.Nodes[0].Nodes.Add(nPort.PortName,nPort.PortDescript);
                TreeNode nNode = new TreeNode(nPort.PortName + "(" + nPort.PortConfig1 + ")");
                nNode.Name = "Prt." + frmMain.staComm.ListPort.IndexOf(nPort);
                if (nPort.bOpen)
                    nNode.BackColor = Color.Lime;
                else
                    nNode.BackColor = Color.Red;

                foreach (CStation nSta in nPort.ListStation)
                {
                    nNode.Nodes.Add("Sta." + frmMain.staComm.ListStation.IndexOf(nSta).ToString(), nSta.Name + "(" + nSta.Address64 + ")");
                }
                treeView1.Nodes[0].Nodes.Add(nNode);
            }

            treeView1.ExpandAll();
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            string str1 = node.Name;
            string str2 = node.Text;
            this.Text = str1;
            
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
                dataGridView1.Rows.Clear();
                CStation sta1 = frmMain.staComm.ListPort[2].ListStation[0];
                CStation sta2 = frmMain.staComm.ListStation[0];
                switch (sType)
                {
                    case "Prj":
                        break;
                    case "Prt":
                        AddPortToTable(Int32.Parse(sName));
                        break;
                    case "Sta":
                        AddStaToTable(Int32.Parse(sName));
                        break;
                    default:
                        break;
                }
                dataGridView1.AutoResizeColumns();
                CommTimer.Enabled = true;
            }
            catch (Exception ee)
            {
                MessageBox.Show("treeView1_AfterSelect:"+ee.Message);
            }
        }

        private void AddPortToTable(int iIndex)
        {
            if (iIndex < 0)
                return;

            CPort nPort = (CPort)frmMain.staComm.ListPort[iIndex];
            foreach (CStation nSta in nPort.ListStation)
            {
                AddStaToTable(nSta);
            }
        }

        private void AddStaToTable(int iIndex)
        {
            if (iIndex < 0)
                return;

            CStation nSta = (CStation)frmMain.staComm.ListStation[iIndex];
            ArrayList obj = new ArrayList();
            string[] str1;

            foreach (CVar nVar in nSta.StaDevice.ListDevVar)
            {
                str1 = AddRow(nVar, nSta.Name);
                dataGridView1.Rows.Add(str1);
            }
        }

        private void AddStaToTable(CStation nSta)
        {
            if (nSta ==null)
                return;

            ArrayList obj = new ArrayList();
            string[] str1;

            foreach (CVar nVar in nSta.StaDevice.ListDevVar)
            {
                str1 = AddRow(nVar, nSta.Name);
                dataGridView1.Rows.Add(str1);
            }

            dataGridView1.AutoResizeColumns();
        }
        private string[] AddRow(CVar nVar,string StaName)
        {
            
            string[] str1 = new string[9];
            string sWR = "";
            if (nVar == null)
                return str1;
            if(nVar.Readable>0)
                sWR += "读";
            if (nVar.Writeable>0)
                sWR += "写";

            iIndex++;
            str1[0] = iIndex.ToString();
            str1[1] = StaName;
            str1[2] = nVar.Name;
            str1[3] = nVar.GetStringValue(1);
            str1[4] = nVar.Description;
            str1[5] = nVar.Unit;
            str1[6] = nVar.GetTypeName();
            str1[7] = sWR;
            return str1;
        }

        //定时器
        public void CommTimerCall(object source, System.Timers.ElapsedEventArgs e)
        {
            UpdateTreelist(treeView1.Nodes);
            UpdateValue();
        }

        private void UpdateValue()
        {
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    string sSta = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string sVar = (string)dataGridView1.Rows[i].Cells[2].Value;
                    string sValue = frmMain.staComm.GetVarValueByStaNameVarName(sSta, sVar);
                    dataGridView1.Rows[i].Cells[3].Value = sValue;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "UpdateValue");
            }
        }

        private void UpdateTreelist(TreeNodeCollection aNodes)
        {
            int i = 0;
            foreach (TreeNode item in aNodes)//遍历Treeview的所有节点
            {
                string sType = item.Name.Substring(0, 3);
                switch (sType)
                {
                    case "Prj":
                        break;
                    case "Prt":
                         i =int.Parse( item.Name.Substring(4));
                         CPort nPort = (CPort)frmMain.staComm.ListPort[i];
                         if (nPort != null)
                         {
                             if (nPort.bOpen)
                                 item.BackColor = Color.Lime;
                             else
                                 item.BackColor = Color.Red;
                         }
                        break;
                    case "Sta":
                         i = int.Parse(item.Name.Substring(4));
                         CStation nSta = (CStation)frmMain.staComm.ListStation[i];
                         if (nSta != null)
                         {
                             if (nSta.CommStateE == ECommSatate.Unknown)
                                 item.BackColor = Color.White;
                             else if (nSta.CommStateE == ECommSatate.Failure)
                                 item.BackColor = Color.Red;
                             else if (nSta.CommStateE == ECommSatate.Normal)
                                 item.BackColor = Color.Lime;
                         }
                        break;
                    default:
                        break;
                }
                 UpdateTreelist(item.Nodes);
            }
        }
    }
}
