using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace LSSCADA.Control
{
    /// <summary>
    /// 报警优先级
    /// </summary>
    public enum EAlarmPriority
    {
        PRIORITY_1 = 0,
        PRIORITY_2 = 1,
        PRIORITY_3 = 2
    }
    /// <summary>
    /// 越限报警状态
    /// </summary>
    public enum ELimitState
    {
        NN = -1,
        LL = 0,
        LO = 1,
        HI = 2,
        HH = 3,
        LLRe = 4,
        LORe = 5,
        HIRe = 6,
        HHRe = 7
    }
    /// <summary>
    /// 越限报警内容
    /// </summary>
    public struct SLimit
    {
        public bool enabled; //
        public int value; //
        public string text; //
        public EAlarmPriority priority;//
    }

    //变位报警内容
    public struct CVarState
    {
        public Single oldvalue;//旧值
        public Single newvalue;//新值
        public string text;//报警文本
        public string addition;//报警条件
        public string Program;//运行程序
    }

    //变量报警
    public class CVarAlarm
    {
        //越限报警OverLimit
        public int LimitDelay = 0;//越限延时 秒
        public SLimit[] sLimt = new SLimit[4];//0:LL  1:LO 2:HI 3:HH
        public string[] LimitType = { "LL", "LO", "HI", "HH" };
        public bool Deadbandenabled = false;
        public int Deadbandvalue = 0;

        //偏差值报警
        //<Warp enabled="True" default="5" warp="6" priority="1"/>
        public bool Warpenabled = false;//偏差报警使能
        public int Warpdefault = 0;//目标值
        public int Warp = 0;//偏差
        public EAlarmPriority Warppriority = EAlarmPriority.PRIORITY_2;//报警优先级

        //变化率报警
        //<ChangePercent enabled="True" percent="50" period="6" priority="2"/>
        public bool ChgPercentenabled = false;//变化率报警使能
        public int ChgPercent = 0;//变化率 %
        public int Chgperiod = 0;//周期 秒
        public EAlarmPriority ChgPerpriority = EAlarmPriority.PRIORITY_2;//报警优先级

        //变位报警
        public ArrayList ListVarState = new ArrayList();//
        public EAlarmPriority VarStatepriority = EAlarmPriority.PRIORITY_2;//变位报警优先级

        //变量异常报警
        public bool VarErrorenabled = false;
        public Single minvalue = 0;
        public string mintext = "";
        public Single maxvalue = 0;
        public string maxtext = "";
        public EAlarmPriority VarErrorpriority = EAlarmPriority.PRIORITY_2;//报警优先级

        public ELimitState eLimitState = ELimitState.NN;
        //复制实例
        public CVarAlarm Clone()
        {
            CVarAlarm obj = (CVarAlarm)this.MemberwiseClone();
            return obj;
        }

        public bool LoadFromNode(XmlElement VarNode)
        {
            //读取越限报警
            try
            {
                string xpath = "OverLimit";
                XmlElement temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                LimitDelay = int.Parse(temNode.GetAttribute("Delay"));

                for (int i = 0; i < 4; i++)
                {
                    xpath = "OverLimit/Limit/" + LimitType[i];
                    temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                    sLimt[i].enabled = bool.Parse(temNode.GetAttribute("enabled"));
                    if (sLimt[i].enabled)
                    {
                        sLimt[i].value = int.Parse(temNode.GetAttribute("value"));
                        sLimt[i].text = temNode.GetAttribute("text");
                        sLimt[i].priority = (EAlarmPriority)int.Parse(temNode.GetAttribute("priority"));
                    }
                }
                xpath = "OverLimit/Limit/Deadband";
                temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                Deadbandenabled = bool.Parse(temNode.GetAttribute("enabled"));
                if (Deadbandenabled)
                    Deadbandvalue = int.Parse(temNode.GetAttribute("value"));

                xpath = "OverLimit/Warp";
                temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                Warpenabled = bool.Parse(temNode.GetAttribute("enabled"));
                if (Warpenabled)
                {
                    Warpdefault = int.Parse(temNode.GetAttribute("default"));
                    Warp = int.Parse(temNode.GetAttribute("warp"));
                    Warppriority = (EAlarmPriority)int.Parse(temNode.GetAttribute("priority"));
                }
                xpath = "OverLimit/ChangePercent";
                temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                ChgPercentenabled = bool.Parse(temNode.GetAttribute("enabled"));
                if (ChgPercentenabled)
                {
                    ChgPercent = int.Parse(temNode.GetAttribute("percent"));
                    Chgperiod = int.Parse(temNode.GetAttribute("period"));
                    ChgPerpriority = (EAlarmPriority)int.Parse(temNode.GetAttribute("priority"));
                }
                //读取变位报警
                xpath = "VarState";
                temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                ListVarState.Clear();
                foreach (XmlElement varstate in temNode.ChildNodes)
                {
                    CVarState objVarState = new CVarState();
                    objVarState.oldvalue = Single.Parse(varstate.GetAttribute("oldvalue"));
                    objVarState.newvalue = Single.Parse(varstate.GetAttribute("newvalue"));
                    objVarState.text = varstate.GetAttribute("text");
                    objVarState.addition = varstate.GetAttribute("addition");
                    objVarState.Program = varstate.GetAttribute("Program");
                    ListVarState.Add(objVarState);
                }
                VarStatepriority = (EAlarmPriority)int.Parse(temNode.GetAttribute("priority"));
                //读取变量异常报警
                xpath = "VarError";
                temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                VarErrorenabled = bool.Parse(temNode.GetAttribute("enabled"));
                if (VarErrorenabled)
                {
                    minvalue = Single.Parse(temNode.GetAttribute("minvalue"));
                    mintext = temNode.GetAttribute("mintext");
                    maxvalue = Single.Parse(temNode.GetAttribute("maxvalue"));
                    maxtext = temNode.GetAttribute("maxtext");
                    VarErrorpriority = (EAlarmPriority)int.Parse(temNode.GetAttribute("priority"));
                } 
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void GetELimitAlarm(Int64 ValueInt64,string StaName)
        {
            int DeadValue = 0;
            if (Deadbandenabled) 
                DeadValue = Deadbandvalue;

            switch (eLimitState)
            {
                case ELimitState.NN:
                    if (sLimt[0].enabled)
                    {
                        if (ValueInt64 < sLimt[0].value - DeadValue)
                        {
                            eLimitState = ELimitState.LL;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[1].enabled)
                    {
                        if (ValueInt64 < sLimt[1].value - DeadValue)
                        {
                            eLimitState = ELimitState.LO;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[3].enabled)
                    {
                        if (ValueInt64 > sLimt[3].value + DeadValue)
                        {
                            eLimitState = ELimitState.HH;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[2].enabled)
                    {
                        if (ValueInt64 > sLimt[2].value + DeadValue)
                        {
                            eLimitState = ELimitState.HI;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    break;
                case ELimitState.LO:
                    if (sLimt[0].enabled)
                    {
                        if (ValueInt64 < sLimt[0].value - DeadValue)
                        {
                            eLimitState = ELimitState.LL;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[1].enabled)
                    {
                        if (ValueInt64 > sLimt[1].value + DeadValue)
                        {
                            eLimitState = ELimitState.NN;
                            GetELimitAlarmMsg(StaName,ELimitState.LORe);
                        }
                    }
                    if (sLimt[3].enabled)
                    {
                        if (ValueInt64 > sLimt[3].value + DeadValue)
                        {
                            eLimitState = ELimitState.HH;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[2].enabled)
                    {
                        if (ValueInt64 > sLimt[2].value + DeadValue)
                        {
                            eLimitState = ELimitState.HI;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    break;
                case ELimitState.LL:
                    if (sLimt[0].enabled)
                    {
                        if (ValueInt64 > sLimt[0].value + DeadValue)
                        {
                            eLimitState = ELimitState.LO;
                            GetELimitAlarmMsg(StaName,ELimitState.LLRe);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (sLimt[1].enabled)
                    {
                        if (ValueInt64 > sLimt[1].value + DeadValue)
                        {
                            eLimitState = ELimitState.NN;
                            GetELimitAlarmMsg(StaName,ELimitState.LORe);
                        }
                        else if (ValueInt64 < sLimt[1].value - DeadValue)
                        {
                            eLimitState = ELimitState.LO;
                            GetELimitAlarmMsg(StaName,ELimitState.LO);
                            break;
                        }
                    }
                    if (sLimt[3].enabled)
                    {
                        if (ValueInt64 > sLimt[3].value + DeadValue)
                        {
                            eLimitState = ELimitState.HH;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[2].enabled)
                    {
                        if (ValueInt64 > sLimt[2].value + DeadValue)
                        {
                            eLimitState = ELimitState.HI;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    break;
                case ELimitState.HI:
                    if (sLimt[3].enabled)
                    {
                        if (ValueInt64 > sLimt[3].value + DeadValue)
                        {
                            eLimitState = ELimitState.HH;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[2].enabled)
                    {
                        if (ValueInt64 < sLimt[2].value - DeadValue)
                        {
                            eLimitState = ELimitState.NN;
                            GetELimitAlarmMsg(StaName,ELimitState.HIRe);
                        }
                    }
                    if (sLimt[0].enabled)
                    {
                        if (ValueInt64 < sLimt[0].value - DeadValue)
                        {
                            eLimitState = ELimitState.LL;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    if (sLimt[1].enabled)
                    {
                        if (ValueInt64 < sLimt[1].value - DeadValue)
                        {
                            eLimitState = ELimitState.LO;
                            GetELimitAlarmMsg(StaName,eLimitState);
                            break;
                        }
                    }
                    break;
                case ELimitState.HH:
                    if (sLimt[3].enabled)
                    {
                        if (ValueInt64 < sLimt[3].value - DeadValue)
                        {
                            eLimitState = ELimitState.HI;
                            GetELimitAlarmMsg(StaName,ELimitState.HHRe);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (sLimt[2].enabled)
                    {
                        if (ValueInt64 < sLimt[2].value - DeadValue)
                        {
                            eLimitState = ELimitState.NN;
                            GetELimitAlarmMsg(StaName,ELimitState.HIRe);
                            //HI Re
                        }
                        else if (ValueInt64 > sLimt[2].value + DeadValue)
                        {
                            eLimitState = ELimitState.HI;
                            GetELimitAlarmMsg(StaName,ELimitState.HI);
                            break;
                        }
                    }
                    if (sLimt[0].enabled)
                    {
                        if (ValueInt64 < sLimt[0].value - DeadValue)
                        {
                            eLimitState = ELimitState.LL;
                            GetELimitAlarmMsg(StaName,ELimitState.LL);
                            break;
                        }
                    }
                    if (sLimt[1].enabled)
                    {
                        if (ValueInt64 < sLimt[1].value - DeadValue)
                        {
                            eLimitState = ELimitState.LO;
                            GetELimitAlarmMsg(StaName,ELimitState.LO);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        //产生越限报警消息
        public void GetELimitAlarmMsg(string StaName,ELimitState node)
        {
            int i = (int)node;
            string Recorder;
            EAlarmPriority priority;

            if (i >= 0 && i <= 3)
            {
                priority = sLimt[i].priority;
                Recorder = sLimt[i].text;
            }
            else if (i >= 4 && i <= 7)
            {
                priority = sLimt[i - 4].priority;
                Recorder = sLimt[i - 4].text + " 恢复";
            }
            else
            {
                return;
            }
            frmMain.staAlarm.cAlarmMsg.AddMsg(StaName, Recorder, EAlarmType.OverLimit, priority);
        }
        //获取开关变位报警
        public void GetVarStateAlarmMsg(Int64 OldValue,Int64 NewValue,string StaName)
        {
            foreach (CVarState node in ListVarState)
            {
                if (node.oldvalue == OldValue && node.newvalue == NewValue)
                {
                    frmMain.staAlarm.cAlarmMsg.AddMsg(StaName, node.text, EAlarmType.StateChange, VarStatepriority);
                    break;
                }

            }
        }
    }

    public class CAlarm
    {
        public ArrayList ListVarAlarm = new ArrayList();
        public CAlarmMsg cAlarmMsg = new CAlarmMsg();

        public void LoadFromXML()
        {
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sPath = CProject.sPrjPath + "\\Alarm.xml";
            myxmldoc.Load(sPath);

            string xpath = "System/" + CProject.sPrjName;
            XmlElement PrjNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);

            ListVarAlarm.Clear();
            foreach (XmlElement StaNode in PrjNode.ChildNodes)
            {
                if (StaNode.Name == "Alarms")
                    continue;
                CStation nSta = frmMain.staComm.GetStaByStaName(StaNode.Name);
                if (nSta == null)
                    continue;
                xpath = "Alarms/Vars";
                XmlElement VarsNode = (XmlElement)StaNode.SelectSingleNode(xpath);
                if (VarsNode == null)
                    continue;
                foreach (XmlElement VarNode in VarsNode.ChildNodes)
                {
                    CVar nVar = frmMain.staComm.GetVarByStaNameVarName(nSta.Name, VarNode.Name);
                    if (nVar == null)
                        continue;
                    CVarAlarm newAlarm = new CVarAlarm();
                    newAlarm.LoadFromNode(VarNode);

                    ListVarAlarm.Add(newAlarm);
                    nVar.VarAlarm = newAlarm;
                }//end foreach (XmlElement VarNode in VarsNode.ChildNodes)
            }
        }
    }

    //报警类型：系统，SOE，开关变位，越限，子站通信，变量错误，人工操作
    public enum EAlarmType
    {
        NONE = -1,
        Server = 0,         //系统
        SOE = 1,            //SOE
        StateChange = 2,    //开关变位
        OverLimit = 3,      //越限
        StationState = 4,   //子站通信状态
        VarError = 5,     //变量错误
        ManualAct = 6       //人工操作
    }
    //报警内容结构体，
    public class SAlarmMsg : EventArgs
    {
        public DateTime? Date_Time;      //报警时间
        public string StaName;      //子站名
        public string Recorder;         //报警内容
        public string Remark;           //备注
        public DateTime? ConfirmTime;    //确认时间
        public string ConfirmName;      //确认用户名
        public EAlarmPriority priority; //报警优先级

        public EAlarmType eAlarmType;   //报警类型
        public bool bConfirm;           //是否需要确认      ，
        public SAlarmMsg()
        {
            Date_Time = DateTime.Now;
            StaName = "";
            Recorder = "";
            Remark = "";
            ConfirmTime = null;
            ConfirmName = "";
            priority = EAlarmPriority.PRIORITY_2;
            eAlarmType = EAlarmType.NONE;
            bConfirm = true;
        }
        public string GetPriorityString()
        {
            switch (priority)
            {
                case EAlarmPriority.PRIORITY_3:
                   return  "三级";
                case EAlarmPriority.PRIORITY_2:
                    return "二级";
                case EAlarmPriority.PRIORITY_1:
                    return "一级";
                default:
                    return "未定义";
            }
        }
        public string GetAlarmTypeString()
        {
            switch (eAlarmType)
            {
                case EAlarmType.Server:
                    return "系统事件";
                case EAlarmType.SOE:
                    return "SOE事件";
                case EAlarmType.StateChange:
                    return "开关变位";
                case EAlarmType.OverLimit:
                    return "数值越限";
                case EAlarmType.StationState:
                    return "通信状态";
                case EAlarmType.VarError:
                    return "变量错误";
                case EAlarmType.ManualAct:
                    return "人工操作";
                default:
                    return "未定义";
            }
        }
    }

    public class CAlarmMsg
    {
        //声明委托
        public delegate void BoiledEventHandler(Object sender, SAlarmMsg e);
        public event BoiledEventHandler Boiled; //声明事件
        public bool bUpdate = false;
        public List<SAlarmMsg> ListAlarmMsg  = new List<SAlarmMsg>();//所有报警
        public List<SAlarmMsg> ListAlarmMsg1 = new List<SAlarmMsg>();//一级报警，声音、画面、保存、
        public List<SAlarmMsg> ListAlarmMsg2 = new List<SAlarmMsg>();//二级报警，画面、保存
        public List<SAlarmMsg> ListAlarmMsg3 = new List<SAlarmMsg>();//三级报警，保存

        // 可以供继承自 Heater 的类重写，以便继承类拒绝其他对象对它的监视
        protected virtual void OnBoiled(SAlarmMsg e)
        {
            if (Boiled != null)
            { // 如果有对象注册
                Boiled(this, e); // 调用所有注册对象的方法
            }
        }

        public void AddMsg(string StaName, string Recorder,EAlarmType eAlarmType, EAlarmPriority priority)
        {
            SAlarmMsg newMsg = new SAlarmMsg();
            newMsg.StaName = StaName;
            newMsg.Recorder = Recorder;
            newMsg.priority = priority;
            newMsg.eAlarmType = eAlarmType;
            ListAlarmMsg.Add(newMsg);
            bUpdate = true;
            switch (priority)
            {
                case EAlarmPriority.PRIORITY_1:
                    ListAlarmMsg1.Add(newMsg);
                    while (ListAlarmMsg1.Count >CCONST. ListMax)
                    {
                        ListAlarmMsg1.RemoveAt(0);
                    }
                    OnBoiled(newMsg);
                    break;
                case EAlarmPriority.PRIORITY_2:
                    ListAlarmMsg2.Add(newMsg);
                    while (ListAlarmMsg2.Count > CCONST.ListMax)
                    {
                        ListAlarmMsg2.RemoveAt(0);
                    }
                    OnBoiled(newMsg);
                    break;
                case EAlarmPriority.PRIORITY_3:
                    ListAlarmMsg3.Add(newMsg);
                    while (ListAlarmMsg3.Count > CCONST.ListMax)
                    {
                        ListAlarmMsg3.RemoveAt(0);
                    }
                    break;
            }
        }
    }
}
