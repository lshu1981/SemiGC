using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySqlPublic;
using System.Xml;
using System.Diagnostics;
//using MySqlPublic;
using System.Data;

namespace LSSCADA.Database
{
    public class LSDatabase
    {
        public  string sAppPath = "";
        static string HisDBName = "sdm221825259_db";//数据库名称
        static string SOEDBName = "sdm221825259_db";//数据库名称
        static string BaseName = "MySql";//数据库类型
        static string Server = "sdm221825259.my3w.com";//服务器名称
        static string UserID = "sdm221825259";
        static string Password = "AydjkLzbh85";
        public static string constrHis = "";
        public static string constrSOE = "";
        int CycleRapidStore = 1;//快速存储周期 单位s
        int CycleCommonStore = 10;//普通存储周期 单位m
        int CycleSlowStore = 24;//慢速存储周期 单位H

        string HostName = "";
        List<CLSDataTable> ListSta = new List<CLSDataTable>();

        int iOldMonth = -1;
        bool bDebug = false;
        public LSDatabase(string _sApp)
        {
            //frmMain.bDbIsExists = false;
            //return;
            sAppPath = _sApp;
            LoadXML();
            GetVar();
            //bDebug = true;
            HostName =  System.Net.Dns.GetHostName();
            //初始化数据库表
            try
            {
                //MySqlPublic.MySqlHelper.DbIsExists(Server, UserID, Password, HisDBName);
                //MySqlPublic.MySqlHelper.DbIsExists(Server, UserID, Password, SOEDBName);

                //创建事件记录数据表
                constrSOE = "server=" + Server + ";User Id=" + UserID + ";password=" + Password + ";Database='" + SOEDBName + "';Pooling=false;Character Set=utf8;Allow User Variables=True;";
                CreateLogTable(constrSOE);

                //创建历史数据表
                constrHis = "server=" + Server + ";User Id=" + UserID + ";password=" + Password + ";Database='" + HisDBName + "';Pooling=false;Character Set=utf8;Allow User Variables=True;";
                foreach (CLSDataTable nLSDT in ListSta)
                {
                    iOldMonth = DateTime.Now.Month;
                    nLSDT.CreateTable(constrHis,  DateTime.Now);
                }

            }
            catch (Exception ex)
            {
                frmMain.bDbIsExists = false;
                return;
            }
            frmMain.bDbIsExists = true;
        }

        private static void SetConstr()
        {
            //创建事件记录数据表
            constrSOE = "server=" + Server + ";User Id=" + UserID + ";password=" + Password + ";Database='" + SOEDBName + "';Pooling=false;Character Set=utf8;Allow User Variables=True;";

            //创建历史数据表
            constrHis = "server=" + Server + ";User Id=" + UserID + ";password=" + Password + ";Database='" + HisDBName + "';Pooling=false;Character Set=utf8;Allow User Variables=True;";

        }
        /// <summary>
        /// 读取配置信息
        /// </summary>
        private void LoadXML()
        {
            try
            {
                /// 创建XmlDocument类的实例
                XmlDocument myxmldoc = new XmlDocument();
                string sXMLPath = sAppPath + "\\Project\\Server.xml";
                myxmldoc.Load(sXMLPath);
                
                string xpath = "Servers/Server/HSDBService";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                BaseName = childNode.GetAttribute("BaseName");
                Server = childNode.GetAttribute("Server");
                UserID = childNode.GetAttribute("UserID");
                Password = childNode.GetAttribute("Password");
                HisDBName = childNode.GetAttribute("DatabaseName");

                xpath = "Servers/Server/Cycle";
                childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                CycleRapidStore =Convert.ToInt32( childNode.GetAttribute("RapidStore"));
                CycleCommonStore = Convert.ToInt32(childNode.GetAttribute("CommonStore"));
                CycleSlowStore =Convert.ToInt32( childNode.GetAttribute("SlowStore"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LSDatabase.LoadXML" + ex.Message);
            }
        }

        /// <summary>
        /// 获取存储数据库变量信息
        /// </summary>
        public void GetVar()
        {
            foreach (CStation nSta in frmMain.staComm.ListStation)
            {
                CLSDataTable nLSDT = new CLSDataTable(nSta.Name);
                bool bSave = false;
                foreach (CVar nVar in nSta.StaDevice.ListDevVar)
                {
                    switch (nVar.SaveToDB)
                    {
                        case 1:
                            bSave = true;
                            nLSDT.strVar[nVar.SaveToDB-1] += "," + nVar.Name;
                            nLSDT.ListVar1.Add( nVar);
                            break;
                        case 2:
                            bSave = true;
                            nLSDT.strVar[nVar.SaveToDB-1] += "," + nVar.Name;
                            nLSDT.ListVar2.Add( nVar);
                            break;
                        case 3:
                            bSave = true;
                            nLSDT.strVar[nVar.SaveToDB-1] += "," + nVar.Name;
                            nLSDT.ListVar3.Add( nVar);
                            break;
                        default:
                            break;
                    }
                }
                if(bSave)
                    ListSta.Add(nLSDT);
            }
        }

        private void CreateLogTable(string constr)//创建事件数据表
        {
            string sLogTBName = "AL_SOELog";//事件顺序记录数据库
            Dictionary<string, string> ListLogFields = new Dictionary<string, string>();
            ListLogFields.Add("Date_Time", "TIMESTAMP not null");
            ListLogFields.Add("ServerName", "VARCHAR(50)");
            ListLogFields.Add("ProjectName", "VARCHAR(50)");
            ListLogFields.Add("StationName", "VARCHAR(50)");
            ListLogFields.Add("AlarmType", "VARCHAR(20)");
            ListLogFields.Add("Priority", "int not null default 2");
            ListLogFields.Add("Recorder", "TEXT");
            ListLogFields.Add("Remark", "TEXT");
            ListLogFields.Add("ConfirmTime", "DATETIME");
            ListLogFields.Add("ConfirmName", "VARCHAR(20)");
            ListLogFields.Add("rowguid", "CHAR(36) CHARACTER SET utf8  NOT NULL DEFAULT ''");
            string sExra = "PRIMARY KEY(rowguid),key Date_Time(Date_Time))ENGINE=MyISAM default charset=utf8;";
            CreateTable(constr, sLogTBName, ListLogFields, sExra);

            sLogTBName = "UM_UserInfo";//用户表
            ListLogFields = new Dictionary<string, string>();
            ListLogFields.Add("UserID", "int not null auto_increment");            //工号   10001开始增1
            ListLogFields.Add("UserName", "VARCHAR(20) not null default ''");  //用户名 不少于两个字节
            ListLogFields.Add("UserPassword", "VARCHAR(50) not null default ''");//密码
            ListLogFields.Add("PasswordExpired", "TINYINT not null default 0");//密码是否过期 0不过期 1过期
            ListLogFields.Add("Expired_DT", "DATETIME");             //过期时间
            ListLogFields.Add("UserRoles", "TEXT");                 //角色:操作授权A;操作授权B
            ListLogFields.Add("ValidTime", "int not null default 60");//有效时间 分钟 0表示一直有效
            ListLogFields.Add("UserAddress", "VARCHAR(100)");//地址
            ListLogFields.Add("UserTelephone", "VARCHAR(100)");//电话
            ListLogFields.Add("UserEmail", "VARCHAR(100)");//邮箱
            ListLogFields.Add("UserDescriber", "VARCHAR(100)");//描述
            ListLogFields.Add("rowguid", "CHAR(36) CHARACTER SET utf8  NOT NULL DEFAULT ''");
            sExra = "PRIMARY KEY(rowguid),key UserID(UserID),unique UserName(UserName))ENGINE=MyISAM default charset=utf8 auto_increment=10001;";
            sExra += "insert into " + sLogTBName + "(UserName,UserPassword,UserRoles,rowguid,Expired_DT) values('Administrator','10001','Administrators',uuid(),'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');";
            CreateTable(constr, sLogTBName, ListLogFields, sExra);

            sLogTBName = "UL_UserLog";//用户登录注销记录表
            ListLogFields = new Dictionary<string, string>();
            ListLogFields.Add("Date_Time", "TIMESTAMP not null");   //时间
            ListLogFields.Add("UserID", "int not null");            //工号
            ListLogFields.Add("UserName", "VARCHAR(50) not null");  //用户名
            ListLogFields.Add("ServerName", "VARCHAR(50)");         //操作节点
            ListLogFields.Add("ActionType", "VARCHAR(20) not null");//操作类型
            ListLogFields.Add("Remark", "VARCHAR(20)");             //备注
            ListLogFields.Add("rowguid", "CHAR(36) CHARACTER SET utf8  NOT NULL DEFAULT ''");
            sExra = "PRIMARY KEY(rowguid),key Date_Time(Date_Time))ENGINE=MyISAM default charset=utf8;";
            CreateTable(constr, sLogTBName, ListLogFields, sExra);

            sLogTBName = "P_ProcessInfo";//配方运行信息
            ListLogFields = new Dictionary<string, string>();
            ListLogFields.Add("Name", "VARCHAR(50) not null");              //配方名字
            ListLogFields.Add("DT_Start", "DATETIME");            //开始运行时间
            ListLogFields.Add("DT_End", "DATETIME");                        //结束时间
            ListLogFields.Add("UserName", "VARCHAR(50)");          //操作用户名
            ListLogFields.Add("Status", "int not null default 11");     //配方状态 11:运行中 22：keep 44:停止 66:完成 
            ListLogFields.Add("StepNumber", "int not null default 0");  //配方总层数
            ListLogFields.Add("StepTotal", "int not null default 0");   //运行总层数
            ListLogFields.Add("TotalTime", "int not null default 0");       //总时间
            ListLogFields.Add("Recipe", "text");                            //配方内容 ,,,],,,]
            ListLogFields.Add("rowguid", "CHAR(36) CHARACTER SET utf8  NOT NULL DEFAULT ''");
            sExra = "PRIMARY KEY(rowguid),key DT_Start(DT_Start))ENGINE=MyISAM default charset=utf8;";
            CreateTable(constr, sLogTBName, ListLogFields, sExra);

            sLogTBName = "P_ProcessLog";//配方运行记录
            ListLogFields = new Dictionary<string, string>();
            ListLogFields.Add("Date_Time", "TIMESTAMP not null");           //记录时间
            ListLogFields.Add("LogType", "VARCHAR(20) not null");           //记录类型 PLC自动运行 人工操作
            ListLogFields.Add("Recorder", "text");         //记录内容 ,,,],,,]
            ListLogFields.Add("Processguid", "CHAR(36) CHARACTER SET utf8  NOT NULL DEFAULT ''");
            ListLogFields.Add("rowguid", "CHAR(36) CHARACTER SET utf8  NOT NULL DEFAULT ''");
            sExra = "PRIMARY KEY(rowguid),key Date_Time(Date_Time))ENGINE=MyISAM default charset=utf8;";
            CreateTable(constr, sLogTBName, ListLogFields, sExra);
        }
        private void CreateTable(string constr, string TBName, Dictionary<string, string> ListD,string sExra)
        {
            try
            {
                if (!MySqlPublic.MySqlHelper.TableIsExists(constr, TBName))
                {
                    string sSQL = "CREATE TABLE " + TBName + " (";
                    string stmp = "";
                    foreach (KeyValuePair<string, string> node in ListD)
                    {
                        stmp += node.Key + " " + node.Value + ",";
                    }

                    sSQL += stmp + sExra;
                    Debug.WriteLine(sSQL);
                    int iVal = MySqlHelper.ExecuteNonQuery(constr, sSQL);
                }
                else
                {
                    EditOldTable(constr, TBName, ListD, sExra);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LSDatabase.CreateTable:" + ex.Message);
            }
        }
        public void EditOldTable(string constr, string TBName, Dictionary<string, string> ListD, string sExra)
        {
            string sSQL = "desc " + TBName + ";";
            DataSet Fields = MySqlHelper.GetDataSet(constr, sSQL);
            if (Fields.Tables.Count <= 0)
            {
                sSQL = "drop table " + TBName + ";";
                int iVal = MySqlHelper.ExecuteNonQuery(constr, sSQL);
                CreateTable(constr, TBName, ListD, sExra);
                return;
            }

            string sEditSQL = "";
            DataTable tb = Fields.Tables[0];
            foreach (KeyValuePair<string, string> node in ListD)
            {
                bool balter = true;
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    //Debug.WriteLine(tb.Rows[i]["Field"]);
                    if (node.Key == tb.Rows[i]["Field"].ToString().Trim())
                    {
                        balter = false;
                        break;
                    }
                }
                if (balter)
                {
                    sEditSQL += "alter table " + TBName + " add column " + node.Key + " " + node.Value + ";";
                }
            }
            if (sEditSQL.Length > 0)
            {
                Debug.WriteLine(sEditSQL);
                int iVal = MySqlHelper.ExecuteNonQuery(constr, sEditSQL);
            }
        }
        //定时器
        public void CommTimerCall()
        {
            if (!frmMain.bDbIsExists)
                return;
            if(bDebug)
                return;
            DateTime DTNow = DateTime.Now;
            long iTodaySecond = (long)(DTNow - DateTime.Today).TotalSeconds;
            try
            {
                if (iOldMonth != DTNow.Month)
                {
                    foreach (CLSDataTable nLSDT in ListSta)
                    {
                        nLSDT.CreateNextTable(constrHis, DateTime.Now);
                    }
                    iOldMonth = DateTime.Now.Month;
                }
                if (iTodaySecond % CycleRapidStore == 0)//快速存储时间到
                {
                    string sSave = "";
                    foreach (CLSDataTable nLSDT in ListSta)
                    {
                        nLSDT.GetFastSaveSQL(DTNow);
                        if (CycleRapidStore >= 30)
                        {
                            sSave += nLSDT.FastSaveSQL();
                        }
                    }
                    if (sSave.Length > 0)
                    {
                        int iVal = MySqlHelper.ExecuteNonQuery(constrHis, sSave);
                    }
                }
                
                if (iTodaySecond % 30 == 0 && CycleRapidStore < 30)
                {
                    DateTime startTime = DateTime.Now;
                    string sSave = "";
                    foreach (CLSDataTable nLSDT in ListSta)
                    {
                        sSave += nLSDT.FastSaveSQL();
                    }
                    if (sSave.Length > 0)
                    {
                        int iVal = MySqlHelper.ExecuteNonQuery(constrHis, sSave);
                    }
                    DateTime endTime = DateTime.Now;

                    Debug.WriteLine(endTime.ToLongTimeString()+ "FastSaveSQL:" + ((TimeSpan)(endTime - startTime)).TotalMilliseconds);
                }
            }
            catch (Exception e1)
            {
                Debug.WriteLine("LSDatabase.CmmTimerCall:" + e1.Message);

            }
            //Debug.WriteLine("Send");
        }

        public void SaveMsg(CAlarmMsgEventArgs e)
        {
            if (bDebug)
                return;
            string stmp = "insert into AL_SOELog(Date_Time,ServerName,ProjectName,StationName,AlarmType,Recorder,Remark,ConfirmTime,ConfirmName,rowguid,Priority)values('";
            if(e.Date_Time ==null)
                stmp += "',";
            else
                stmp += ((DateTime)e.Date_Time).ToString("yyyy-MM-dd HH:mm:ss") + "',";
            stmp += "'" + HostName + "','" + frmMain.staPrj.Name + "','" + e.StaName +"','";
            stmp += e.AlarmTypeString + "','" + e.Recorder + "','" + e.Remark + "',";
            if (e.ConfirmTime == null)
                stmp += "null,'";
            else
                stmp +="'" + ((DateTime)e.Date_Time).ToString("yyyy-MM-dd HH:mm:ss") + "','";
            stmp += e.ConfirmName + "','"+ e.ALGuid.ToString() +"',"+e.PriorityStringNum+");";

           // Debug.WriteLine(stmp);
            int iVal = MySqlHelper.ExecuteNonQuery(constrSOE, stmp);
            //sMsgSaveSQL += stmp;
        }

        public void UpdateMsg(CAlarmMsgEventArgs e)
        {
            if (bDebug)
                return;
            //stmp = "update P_ProcessInfo set DT_End = '" + sDT + "',Status = " + Status.ToString() + " where rowguid='" + nP.rowguid.ToString() + "';";

            string stmp = "update AL_SOELog set Recorder = '" + e.Recorder + "',";
            stmp += "Remark = '" + e.Remark + "',";
            if (e.ConfirmTime != null)
                stmp += "ConfirmTime='" + ((DateTime)e.Date_Time).ToString("yyyy-MM-dd HH:mm:ss") + "',";
            stmp += "ConfirmName = '" + e.ConfirmName + "' where rowguid='" + e.ALGuid.ToString() + "';";
            // Debug.WriteLine(stmp);
            int iVal = MySqlHelper.ExecuteNonQuery(constrSOE, stmp);
            //sMsgSaveSQL += stmp;
        }

        /// <summary>
        /// 获取历史遥测数据
        /// </summary>
        /// <param name="DT_Start">开始时间</param>
        /// <param name="DT_End">结束时间</param>
        /// <param name="StaName">子站名</param>
        /// <param name="VarName">变量名</param>
        /// <returns></returns>
        public static DataTable GetFastHisData(DateTime DT_Start,DateTime DT_End,string StaName,string VarName)
        {
            string sSQL = "";
            if(DT_End.Month != DT_Start.Month)
            {
                string sTable1 = StaName  + DT_Start.ToString("yyMM");
                string sTable2 = StaName  + DT_End.ToString("yyMM");
                sSQL = "(SELECT Date_Time "+VarName+" FROM "+sTable1+" where date_Time>='"+DT_Start.ToString("yyyy-MM-dd HH:mm:ss") +"' order by date_Time)union";
                sSQL += "(SELECT Date_Time "+VarName+" FROM "+sTable2+" where date_Time<='"+DT_End.ToString("yyyy-MM-dd HH:mm:ss") +"' order by date_Time);";
            }
            else
            {
                string sTable = StaName + DT_Start.ToString("yyMM");
                sSQL =string.Format("SELECT Date_Time {0} FROM {1} where date_Time between '{2:yyyy-MM-dd HH:mm:ss}' and '{3:yyyy-MM-dd HH:mm:ss}' order by date_Time;", VarName, sTable, DT_Start, DT_End);
                sSQL = string.Format("call GetFastVal('{0:yyyy-MM-dd HH:mm:ss}','{1:yyyy-MM-dd HH:mm:ss}')", DT_Start, DT_End);
            }

            Debug.WriteLine(sSQL);
            SetConstr();
            DataSet nData = MySqlHelper.GetDataSet(constrHis, sSQL);
            if (nData.Tables.Count <= 0)
                return null;

            DataTable tb = nData.Tables[0];
            return tb;
        }

     /// <summary>
     /// 根据输入的SQL语句查询SOE数据库
     /// </summary>
     /// <param name="sSQL">需要查询的SQL语句</param>
     /// <returns>结果数据表</returns>
        public static DataTable GetSOEData(string sSQL)
        {
            DataSet nData = MySqlHelper.GetDataSet(constrSOE, sSQL);
            if (nData.Tables.Count <= 0)
                return null;

            DataTable tb = nData.Tables[0];
            return tb;
        }

    }

    class CLSDataTable
    {
        public string StaName;
        /// <summary>
        /// 快速
        /// </summary>
        public List< CVar> ListVar1 = new List<CVar>();
        /// <summary>
        /// 普通
        /// </summary>
        public List< CVar> ListVar2 = new List<CVar>();
        /// <summary>
        /// 慢速
        /// </summary>
        public List< CVar> ListVar3 = new List<CVar>();

        public string[] strVar = new string[3];

        string[] sTableName =  new string[3];
        string[] ListSQL = new string[3];
        public CLSDataTable(string _Name)
        {
            StaName = _Name;
            strVar[0] = "(Date_Time";
            strVar[1] = "(Date_Time";
            strVar[2] = "(Date_Time";
            ListSQL[0] = "";
            ListSQL[1] = "";
            ListSQL[2] = "";
        }

        public void CreateTable(string constr,  DateTime DT)
        {
            if (ListVar1.Count > 0)
            {
                sTableName[0] = StaName + "_Fast_" + DT.ToString("yyyyMM");
                if (!MySqlPublic.MySqlHelper.TableIsExists(constr, sTableName[0]))
                {
                    CreateNewTable(constr, sTableName[0], 1);
                }
                else
                {
                    EditOldTable(constr, sTableName[0], 1);
                }
            }
        }

        public void CreateNextTable(string constr, DateTime DT)
        {
            if (ListVar1.Count > 0)
            {
                sTableName[0] = StaName + "_Fast_" + DT.ToString("yyyyMM");
                if (!MySqlPublic.MySqlHelper.TableIsExists(constr, sTableName[0]))
                {
                    CreateNewTable(constr, sTableName[0], 1);
                }
            }
        }

        public void CreateNewTable(string constr, string sTableName, int iType)
        {
            string sSQL = "CREATE TABLE " + sTableName + " (Date_Time TIMESTAMP not null";
            foreach(CVar nVar in ListVar1)
            {
                sSQL += "," + nVar.Name + " " + nVar.MySqlType + " not null default 0";
            }
            sSQL += ",rowguid CHAR(36) CHARACTER SET utf8  NOT NULL DEFAULT '',PRIMARY KEY(rowguid),key Date_Time(Date_Time))ENGINE=MyISAM default charset=utf8;";
            Debug.WriteLine(sSQL);
            int iVal = MySqlHelper.ExecuteNonQuery(constr, sSQL);
        }

        public void GetFastSaveSQL(DateTime DTNow)
        {
            string stmp =  "insert into " + sTableName[0] + strVar[0] + ",rowguid) values ('" + DTNow.ToString("yyyy-MM-dd HH:mm:ss") + "'";
           
            for (int i = 0; i < ListVar1.Count; i++)
            {
                if (!ListVar1[i].GetValueQuality)
                    return;
                stmp += "," + ListVar1[i].GetStrDataValue();
            }
            stmp += ",uuid());";
            ListSQL[0] += stmp;
        }

        public string FastSaveSQL()//存储快速
        {
            string str1 = "";
            if (ListSQL[0].Length > 0)
            {
                str1 = ListSQL[0];
                ListSQL[0] = "";
            }
            return str1;
        }

        public void EditOldTable(string constr, string sTableName, int iType)
        {
            string sSQL = "desc " + sTableName + ";";
            DataSet Fields = MySqlHelper.GetDataSet(constr, sSQL);
            if (Fields.Tables.Count <= 0)
            {
                sSQL = "drop table " + sTableName + ";";
                CreateNewTable(constr, sTableName, iType);
                return;
            }

            string sEditSQL = "";
            DataTable tb = Fields.Tables[0];
            foreach (CVar nVar in ListVar1)
            {
                bool balter = true;
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    //Debug.WriteLine(tb.Rows[i]["Field"]);
                    if (nVar.Name == tb.Rows[i]["Field"].ToString().Trim())
                    {
                        balter = false;
                        //Debug.WriteLine(tb.Rows[i]["Type"].ToString().Trim());
                        if (nVar.MySqlType != tb.Rows[i]["Type"].ToString().Trim())
                        {
                            sEditSQL += "alter table " + sTableName + " modify " + nVar.Name + " " + nVar.MySqlType + " not null default 0;";
                        }
                        break;
                    }

                }
                if (balter)
                {
                    sEditSQL += "alter table " + sTableName + " add column " + nVar.Name + " " + nVar.MySqlType + " not null default 0;";
                }
            }
            if (sEditSQL.Length > 0)
            {
                //Debug.WriteLine(sEditSQL);
                int iVal = MySqlHelper.ExecuteNonQuery(constr, sEditSQL);
            }
        }

    }
}
