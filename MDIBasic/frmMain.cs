using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using LSSCADA.Control;
using System.IO;
using System.Diagnostics;
using LSSCADA.Database;

namespace LSSCADA
{
    public delegate void InvokeInsertAlarm(object sender, EventArgs e);//报警委托事件
    public partial class frmMain : Form
    {
        public static CProject staPrj = new CProject(); //工程实例
        public static CCommManager staComm = new CCommManager();//通信处理实例
        public CAlarm staAlarm = new CAlarm();//报警实例
        public LSDatabase nDatabase;//数据库实例
        public static bool bDbIsExists = false;//数据库是否存在

        public List<Form> ListOpenForm = new List<Form>();//打开的窗口列表
        public static int iToolHeight = 118 + 150;  //主工具条高度
        public static float iWinFoucs = 1;          //子窗口缩放比例
        public static float iLeftD = 0;             //子窗口居中右移位置
        public static float iTopD = 0;             //子窗口居中右移位置
        public string ProgramPath = "";             //配方路径
        public int RunState = 0;                    //运行状态

        public CUserInfo nUserInfo;//用户类实例

        public static  CPublicVar nMainVar;//

        private System.Windows.Forms.MdiClient m_MdiClient;

        int[] SS1041 = new int[5] { 0, 0, 0, 0, 0 };

        Form[] LsfrmChild = new Form[5];
        public frmMain()
        {
            InitializeComponent();
            Init();
            //statusAlarm.Visible = false;
            //statusStrip1.Visible = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = "testt";


            //显示登陆界面
            //frmLogin fm2 = new frmLogin(nUserInfo,true);
            //fm2.ShowDialog();
        }

        private void Init()
        {
            Cursor.Current = Cursors.WaitCursor;
            LoadXML();

            //LsfrmChild[1] = new frmC数据监视(this);
            LsfrmChild[0] = new frmC报表维护(this);

            ShowForm(LsfrmChild[0], tabPage1);
            //ShowForm(LsfrmChild[1], tabPage2);
        }

        private void ShowForm(Form frmTran, TabPage cc)
        {
            if (frmTran == null)
                return;
            frmTran.StartPosition = FormStartPosition.Manual;
            frmTran.Dock = DockStyle.Fill;
            frmTran.Show();
            cc.Controls.Clear();
            cc.Controls.Add(frmTran);
        }

        private void LoadXML()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LoadXML");
            }
        }

        private int OldRunState = -1;   //上次运行状态
        
    }
}
