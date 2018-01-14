using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSSCADA
{
    public partial class frmSet : Form
    {
        public frmSet()
        {
            InitializeComponent();
        }

        private void frmSet_Load(object sender, EventArgs e)
        {
            InitDGV();
        }
        private void InitDGV()
        {
            List<string> sList1 = new List<string> { "只读", "名称", "类型", "容量", "单位", "H2 Idle", "N2 Idle", "Hot Idle", "Run"};
            List<string> sRow1 = new List<string> { "√,Bubbler 1CCMD MFC,MFC,1000,sccm,60,60,60,60", ",Bubbler 1 PC,PC,2000,toor,900,900,900,900", ",Bubbler 2 CMD MFC,MFC,500,sccm,30,30,30,30", ",Bubbler 2 Push MFC,MFC,1000,sccm,60,60,60,60", ",Bubbler 2 DD MFC,MFC,500,sccm,30,30,30,30", ",Bubbler 2 PC,PC,1000,sccm,900,900,900,900", ",Bubbler 3 CMD MFC,MFC,200,sccm,12,12,12,12", ",Bubbler 3 Push MFC,MFC,1000,sccm,60,60,60,60", ",Bubbler 3 DD MFC,MFC,200,sccm,12,12,12,12", ",Bubbler 3 PC,PC,1000,sccm,900,900,900,900" };
            InitDGV(SetDGV1,9,10,sList1,sRow1);

            List<string> sList2 = new List<string> { "源瓶编号", "源瓶名称", "种类", "PC(torr)", "变化率(sccm/s)", "最小Vent时间", "温度(℃)" };
            List<string> sRow2 = new List<string> { "1,TMGa1,单稀释,900,21,60,15", "2,TMGa2,双稀释,900,21,60,20", "3,TMAl1,双稀释,900,21,60,5", "4,TMAl2,单稀释,900,21,60,5", "5,TEGa,单稀释,900,21,60,30", "6,Cp2Mg1,单稀释,900,21,60,30", "7,Cp2Mg2,单稀释,900,21,60,44", "8,TMln1,单稀释,900,21,60,30", "9,TMln2,单稀释,900,21,60,30", "10,Cp2Fe,双稀释,900,21,60,20" };
            InitDGV(SetDGV2, 7, 10, sList2, sRow2);

            List<string> sList3 = new List<string> { "掺杂物编号", "名称",  "最小Vent时间"};
            List<string> sRow3 = new List<string> { "1,SiH4,1", };
            InitDGV(SetDGV3, 3, 1, sList3, sRow3);

            List<string> sList4 = new List<string> { "编号", "规则名称", "规则值","单位" };
            List<string> sRow4 = new List<string> { "1,H2 MFC最大变化率,75,L/min", "2,N2 MFC最大变化率,75,L/min", "3,NH3 MFC最大变化率,75,L/min", "4,Chanmber ISO MFC最大变化时间,180,sec", "5,Chanmber purge MFC最大变化时间,180,sec", "6,最小vent时间,180,sec", "7,最大温度变化率,100,Deg C/min", "8,最大转速变化率,200,RPM", "9,最大生长室压力变化率,400,mbar/min", "10,最大温度跳变范围,100,Deg C" };
            InitDGV(SetDGV4, 4, 10, sList4, sRow4);

            List<string> sList5 = new List<string> { "数字量编号", "名称", "动作" };
            List<string> sRow5 = new List<string> { "11,Facility1 Fault,报警", "12,Facility2 Fault,报警", "13,Facility3 Fault,报警", "14,Facility4 Fault,报警", "15,Facility5 Fault,报警", "22,Gas Inlet 1 Pressure Low,报警", "23,Gas Inlet 2 Pressure Low,报警", "24,Gas Inlet 3 Pressure Low,报警", "27,Gas Inlet 6 Pressure Low,报警", "28,Gas Inlet 7 Pressure Low,报警", "29,Gas Inlet 8 Pressure Low,报警", "30,Gas Inlet 9 Pressure Low,报警", "31,Gas Inlet 10 Pressure Low,报警" };
            InitDGV(SetDGV5, 3, 13, sList5, sRow5);
        }

        private void InitDGV(DataGridView dgv, int iCol, int iRow, List<string> sList, List<string> sRow)
        {
            // Create an unbound DataGridView by declaring a column count.
            dgv.ColumnCount = iCol;
            dgv.RowCount = iRow;
            dgv.ColumnHeadersVisible = true;

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            // Resize the height of the column headers. 
            dgv.AutoResizeColumnHeadersHeight();
            // Resize all the row heights to fit the contents of all non-header cells.
            dgv.AutoResizeRows(
                DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);

            // Set the column header names.
            for (int i = 0; i < iCol; i++)
            {
                dgv.Columns[i].Name = sList[i];
            }

            for (int i = 0; i < iRow; i++)
            {
                string str1 = sRow[i];
                string[] str2 = str1.Split(',');
                for (int j = 0; j < iCol; j++)
                {
                    dgv.Rows[i].Cells[j].Value = str2[j];
                }
            }
            dgv.AutoResizeColumns();
        }
    }
}
