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
using LSSCADA.Database;

namespace LSSCADA.Control
{
    public partial class frmAlarmList : Form
    {
        public CAlarm staAlarm;
        string[] sCol1 = new string[] { "确认", "时间", "优先级", "类型", "子站名称", "报警内容", "确认时间", "确认人","GUID" };
        string[] sCol2 = new string[] { "时间", "子站名称", "类型", "报警内容", "确认时间", "确认人" };
        bool[] bCol1 = new bool[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        public frmAlarmList()
        {
            InitializeComponent();
        }

        public frmAlarmList(CAlarm _Alarm)
        {
            InitializeComponent();
            staAlarm = _Alarm;
        }

        private void frmAlarmList_Load(object sender, EventArgs e)
        {
            CPublicDGV.InitializeDGV(dGV1, sCol1, bCol1, true);
            CPublicDGV.InitializeDGV(dGV2, sCol1, bCol1, true);
            CPublicDGV.InitializeDGV(dGV3, sCol1, bCol1, true);

            //CPublicDGV.InitializeDGV(HisdGV1, sCol2, bCol1, true);
            //CPublicDGV.InitializeDGV(HisdGV2, sCol2, bCol1, true);
            //CPublicDGV.InitializeDGV(HisdGV3, sCol2, bCol1, true);

            dTP_E.Value = DateTime.Now;
            dTP_S.Value = DateTime.Now.AddDays(-1);

            FillDGVRealTime();
            
        }

        private void FillDGVRealTime()
        {
            List<string[]> obj1 = new List<string[]>();
            List<string[]> obj2 = new List<string[]>();
            List<string[]> obj3 = new List<string[]>();

            for (int i = staAlarm.cAlarmMsg.ListAlarmMsg.Count - 1; i >= 0; i--)
            {
                CAlarmMsgEventArgs pressShowMsg = staAlarm.cAlarmMsg.ListAlarmMsg[i];
                string[] sCell1 = new string[9];
                if (pressShowMsg == null)
                    continue;
                if (pressShowMsg.bConfirm)
                {
                    sCell1[0] = "未确认";
                    //sRow.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                    sCell1[0] = "已确认";
                if (pressShowMsg.Date_Time != null)
                    sCell1[1] = ((DateTime)pressShowMsg.Date_Time).ToString("yyyy-MM-dd HH:mm:ss");
                sCell1[2] = pressShowMsg.PriorityString;
                sCell1[3] = pressShowMsg.AlarmTypeString;
                sCell1[4] = pressShowMsg.StaName;
                sCell1[5] = pressShowMsg.Recorder;
                if(pressShowMsg.ConfirmTime != null)
                    sCell1[6] = ((DateTime)pressShowMsg.ConfirmTime).ToString("yyyy-MM-dd HH:mm:ss"); 
                sCell1[7] = pressShowMsg.ConfirmName;
                sCell1[8] = pressShowMsg.ALGuid.ToString();
                if (pressShowMsg.eAlarmType == EAlarmType.ManualAct)
                    obj3.Add(sCell1);
                else
                {
                    if (pressShowMsg.priority == EAlarmPriority.PRIORITY_3)
                        obj2.Add(sCell1);
                    else
                        obj1.Add(sCell1);
                }
            }

            dGV1.Rows.Clear();
            foreach (string[] rowArray in obj1)
            {
                int i = dGV1.Rows.Add(rowArray);
                if (rowArray[0] == "未确认")
                    dGV1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                else
                    dGV1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
            }

            dGV2.Rows.Clear();
            foreach (string[] rowArray in obj2)
            {
                int i = dGV2.Rows.Add(rowArray);
                if (rowArray[0] == "未确认")
                    dGV2.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                else
                    dGV2.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
            }

            dGV3.Rows.Clear();
            foreach (string[] rowArray in obj3)
            {
                rowArray[0] = "";
                int i = dGV3.Rows.Add(rowArray);
                dGV3.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
            }
            staAlarm.cAlarmMsg.bUpdate = false;
        }
        private void ButFirmOne_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                for (int i = 0; i < dGV1.SelectedCells.Count; i++)
                {
                    CellDoubleClick(dGV1, dGV1.SelectedCells[i].RowIndex, EAlarmPriority.PRIORITY_1);
                }
            }
            else
            {
                for (int i = 0; i < dGV2.SelectedCells.Count; i++)
                {
                    CellDoubleClick(dGV2, dGV2.SelectedCells[i].RowIndex, EAlarmPriority.PRIORITY_3);
                }

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (staAlarm.cAlarmMsg.bUpdate)
            FillDGVRealTime();
        }

        private void ButFirmAll_Click(object sender, EventArgs e)
        {
            foreach (CAlarmMsgEventArgs nMsg in staAlarm.cAlarmMsg.ListAlarmMsg)
            {
                staAlarm.SetConfirm(nMsg);
            }
            FillDGVRealTime();
        }

        private void dGV1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CellDoubleClick((DataGridView)sender, e.RowIndex, EAlarmPriority.PRIORITY_1); 
        }

        private void CellDoubleClick(DataGridView dGV, int iRow, EAlarmPriority priority)
        {
            try
            {
                string sbConfirm = dGV.Rows[iRow].Cells[0].Value.ToString();
                if (sbConfirm == "已确认")
                    return;
                string sGUID = dGV.Rows[iRow].Cells[8].Value.ToString();

                for (int i = 0; i < staAlarm.cAlarmMsg.ListAlarmMsg.Count; i++)
                {
                    if (staAlarm.cAlarmMsg.ListAlarmMsg[i].ALGuid.ToString() != sGUID)
                        continue;
                    CAlarmMsgEventArgs MsgE = staAlarm.cAlarmMsg.ListAlarmMsg[i];
                    staAlarm.SetConfirm(MsgE);
                   
                    dGV.Rows[iRow].Cells[0].Value = "已确认";
                    dGV.Rows[iRow].DefaultCellStyle.ForeColor = Color.Black;
                    if (staAlarm.cAlarmMsg.ListAlarmMsg[i].ConfirmTime != null)
                        dGV.Rows[iRow].Cells[6].Value = ((DateTime)MsgE.ConfirmTime).ToString("yyyy-MM-dd HH:mm:ss");
                    dGV.Rows[iRow].Cells[7].Value = staAlarm.cAlarmMsg.ListAlarmMsg[i].ConfirmName;
                }
            }
            catch (Exception ex)
            { }
        }

        private void dGV2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CellDoubleClick((DataGridView)sender, e.RowIndex, EAlarmPriority.PRIORITY_3);
        }

        private void butQuery_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                butQuery.Enabled = false;
                DateTime DT_S, DT_E;
                if (dTP_S.Value > dTP_E.Value)
                {
                    DT_E = dTP_S.Value;
                    DT_S = dTP_E.Value;
                }
                else
                {
                    DT_E = dTP_E.Value;
                    DT_S = dTP_S.Value;
                }
                string sSQL = "SELECT (@i:=@i+1) as 序号, AL_SOELog.Date_Time as 时间,";
                sSQL += "AL_SOELog.StationName as 子站名称,AL_SOELog.AlarmType as 类型 ,";
                sSQL += "AL_SOELog.Recorder as 报警内容,AL_SOELog.ConfirmTime as 确认时间,";
                sSQL += "AL_SOELog.ConfirmName as 确认人 FROM (select @i:=0)it,AL_SOELog ";
                sSQL += "where AL_SOELog.Date_Time between '" + DT_S.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + DT_E.ToString("yyyy-MM-dd HH:mm:ss") + "' ";

                string sSQL1 = sSQL + " and (AL_SOELog.Priority=1 or AL_SOELog.Priority=2) and AL_SOELog.AlarmType!= '人工操作' order by AL_SOELog.Date_Time;";
                DataTable DTValue = LSDatabase.GetSOEData(sSQL1);
                HisdGV1.DataSource = DTValue;
                HisdGV1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                string sSQL2 = sSQL + " and AL_SOELog.Priority=3 and AL_SOELog.AlarmType!= '人工操作' order by AL_SOELog.Date_Time;";
                DataTable DTValue2 = LSDatabase.GetSOEData(sSQL2);
                HisdGV2.DataSource = DTValue2;
                HisdGV2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                string sSQL3 = sSQL + " and AL_SOELog.AlarmType= '人工操作' order by AL_SOELog.Date_Time;";
                DataTable DTValue3 = LSDatabase.GetSOEData(sSQL3);
                HisdGV3.DataSource = DTValue3;
                HisdGV3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "butQuery_Click");
            }
            finally
            {
                this.Cursor = Cursors.Default;
                butQuery.Enabled = true;
            }
        }
    }
}
