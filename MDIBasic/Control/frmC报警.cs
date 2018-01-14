using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSSCADA.Control
{
    public partial class frmC报警 : Form
    {
        List<CVar> ListVar = new List<CVar>();
        string SType = "报警";
        public frmC报警()
        {
            InitializeComponent();
        }

        public frmC报警(Form _Owner,string str1,int iTop)
        {
            InitializeComponent();

            this.MdiParent = _Owner;
            SType = str1;
            this.Top = iTop;
            InitListView();
            Fill();
            this.DoubleBuffered = true;
        }

        private void frmC报警_Resize(object sender, EventArgs e)
        {
            Size aa = this.MdiParent.Size;
            this.Size = new System.Drawing.Size(aa.Width - 20, aa.Height - frmMain.iToolHeight);
            
        }
        private void InitListView()
        {
            ListVar.Clear();
            foreach (CStation nSta in frmMain.staComm.ListStation)
            {
                if (nSta.Name == "NJ301")
                {
                    foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                    {
                        if (nVar.nVarAlarm == null)
                            continue;
                        if (SType == "报警")
                        {
                            if ((EAlarmPriority)nVar.nVarAlarm.VarStatepriority != EAlarmPriority.PRIORITY_3)
                            {
                                ListVar.Add(nVar);
                            }
                        }
                        else
                        {
                            if ((EAlarmPriority)nVar.nVarAlarm.VarStatepriority == EAlarmPriority.PRIORITY_3)
                            {
                                ListVar.Add(nVar);
                            }
                        }
                    }
                }
            }
        }

        private void Fill()
        {
            this.listView1.Columns.Add("状态", 120, HorizontalAlignment.Left);
            //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.TileSize = new Size(400, 25);
            this.listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度

            for (int i = 0; i < ListVar.Count;i++ )
            {
                ListViewItem lvi = new ListViewItem();
                int iVarSet = (int)ListVar[i].nVarAlarm.ListVarState[0].newvalue;
               
                if ((ListVar[i].GetInt64Value() > 0 && iVarSet == 1) || (ListVar[i].GetInt64Value() == 0 && iVarSet == 0))
                {
                    if (SType == "报警")
                        lvi.ImageIndex = 1;     //通过与imageList绑定，显示imageList中第i项图标
                    else
                        lvi.ImageIndex = 2;     //通过与imageList绑定，显示imageList中第i项图标

                }
                else
                    lvi.ImageIndex = 0;

                lvi.Text = ListVar[i].Name + ":" + ListVar[i].Description;
                //  lvi.Text =nVar.Name+ nVar.Description;

                listView1.Items.Add(lvi);
            }
            if (SType == "报警")
            {
                for (int i = 0; i < frmMain.staComm.ListStation.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();

                    if (frmMain.staComm.ListStation[i].CommStateE == ECommSatate.Normal)
                            lvi.ImageIndex = 0;     //通过与imageList绑定，显示imageList中第i项图标
                    else
                        lvi.ImageIndex = 1;

                    lvi.Text = frmMain.staComm.ListStation[i].Description + ":" + frmMain.staComm.ListStation[i].CommStateS;
                    //  lvi.Text =nVar.Name+ nVar.Description;

                    listView1.Items.Add(lvi);
                }
            }
            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateView();
        }


        private void UpdateView()
        {
            for (int i = 0; i < ListVar.Count; i++)
            {
                int iVarSet = (int)ListVar[i].nVarAlarm.ListVarState[0].newvalue;
                if ((ListVar[i].GetInt64Value() > 0 && iVarSet == 1) || (ListVar[i].GetInt64Value() == 0 && iVarSet == 0))
                {
                    if (SType == "报警")
                        listView1.Items[i].ImageIndex = 1;     //通过与imageList绑定，显示imageList中第i项图标
                    else
                        listView1.Items[i].ImageIndex = 2;     //通过与imageList绑定，显示imageList中第i项图标

                }
                else
                    listView1.Items[i].ImageIndex = 0;
            }
            if (SType == "报警")
            {
                for (int i = 0; i < frmMain.staComm.ListStation.Count; i++)
                {
                    if (frmMain.staComm.ListStation[i].CommStateE == ECommSatate.Normal)
                        listView1.Items[ListVar.Count+i].ImageIndex = 0;     //通过与imageList绑定，显示imageList中第i项图标
                    else
                        listView1.Items[ListVar.Count+i].ImageIndex = 1;
                    listView1.Items[ListVar.Count + i].Text = frmMain.staComm.ListStation[i].Description + ":" + frmMain.staComm.ListStation[i].CommStateS;

                }
            }
        }

    }
}
