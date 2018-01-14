using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Windows.Forms;
using LSSCADA.Database;

namespace LSSCADA
{
    public enum EAlarmPriority
    {
        PRIORITY_1 = 0,
        PRIORITY_2 = 1,
        PRIORITY_3 = 2
    }
    public class SLimit
    {
        public Double value = 0; //
        public string text = ""; //
        public int priority = 0;//
        public Double Deadband = 0;
        public int Delay = 0;
        public DateTime AlarmDT;
        public int bAlarm = 0;
        public Double MinValue
        {
            get
            {
                return value - Math.Abs(Deadband);
            }
        }
        public Double MaxValue
        {
            get
            {
                return value + Math.Abs(Deadband);
            }
        }
    }

    //变位报警内容
    public class CVarState
    {
        public Double oldvalue = 0;//旧值
        public Double newvalue = 0;//新值
        public string text = "";//报警文本
        public string addition = "";//报警条件
        public string Program="";//运行程序
        public int Delay = 0;
        public DateTime AlarmDT;
    }

    public class Ccondition//条件报警
    {
        public string Expression = "";//描述
        public string Description = "";//条件
        public string Response = "";//
        public int Priority = 1;//

        public List<Ccondition> ListSubCond = new List<Ccondition>();

        public bool LoadFromNode(XmlElement Node)
        {
            ListSubCond.Clear();
            foreach (XmlElement StaNode in Node.ChildNodes)
            {
                switch (StaNode.Name)
                {
                    case "Expression": Expression = StaNode.InnerText; break;
                    case "Description": Description = StaNode.InnerText; break;
                    case "Response": Response = StaNode.InnerText; break;
                    case "Priority": Priority = Convert.ToInt32(StaNode.InnerText); break;
                    case "SubConditions":
                        if (StaNode.ChildNodes.Count > 0)
                        {
                            foreach (XmlElement SubNode in StaNode.ChildNodes)
                            {
                                Ccondition nCond = new Ccondition();
                                nCond.LoadFromNode(SubNode);
                                ListSubCond.Add(nCond);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        public string[] GetRow()
        {
            string[] str1 = new string[8];
            int k = 2;
            str1[k++] = Expression;
            str1[k++] = Description;
            str1[k++] = Priority.ToString();
            str1[k++] = Response;
            str1[k++] = ListSubCond.Count.ToString();
            string strSub = "";
            foreach (Ccondition nSub in ListSubCond)
            {
                strSub += "[" + nSub.GetString() + "]";
            }
            str1[k++] = strSub;
            return str1;
        }

        private string GetString()
        {
            return Expression + "," + Description + "," + Priority + "," + Response + "," + ListSubCond.ToString();
        }
    }

    //变量报警
    public class CVarAlarm
    {
        public string Name = "";
        public string staName = "";
        //越限报警OverLimit
        public List<SLimit> nLimtLO = new List<SLimit>();//越低限
        public List<SLimit> nLimtHI = new List<SLimit>();//越高限
        public int nLimtType = 0;//0 无报警  <0越低限报警 绝对值=nLimtLO的序号+1  >0越高限报警 绝对值=nLimtHI的序号+1
        //偏差值报警
        //<Warp enabled="True" default="5" warp="6" priority="1"/>
        public bool Warpenabled = false;//偏差报警使能
        public Double Warpdefault = 0;//目标值
        public Double Warp = 0;//偏差
        public Double WarpDeadband = 0;//偏差
        public int Warppriority = 1;//报警优先级

        //变化率报警
        //<ChangePercent enabled="True" percent="50" period="6" priority="2"/>
        public bool ChgPercentenabled = false;//变化率报警使能
        public Double ChgPercent = 0;//变化率 %
        public Double Chgperiod = 0;//周期 秒
        public int ChgPerpriority = 1;//报警优先级

        //变位报警
        public List<CVarState> ListVarState = new List<CVarState>();//
        public int VarStatepriority = 1;//变位报警优先级
        CVarState nVarState = null;
        //变量异常报警
        public bool VarErrorenabled = false;
        public Double minvalue = 0;
        public string mintext = "";
        public Double maxvalue = 0;
        public string maxtext = "";
        public int VarErrorpriority = 1;//报警优先级
        public CAlarm staAlarm;

        public CVarAlarm()
        {
        }
        //复制实例
        public CVarAlarm Clone()
        {
            CVarAlarm obj = (CVarAlarm)this.MemberwiseClone();
            return obj;
        }

        /// <summary>
        /// 报警配置内容
        /// </summary>
        /// <returns>二进制排列 bit0越限 bit1变位 bit2偏差 bit3变化率 bit4变量异常</returns>
        public int GetNull()
        {
            int iRe = 0;
            if (nLimtHI.Count > 0 || nLimtLO .Count> 0)
                iRe = (iRe | 1);
            if(ListVarState.Count>0)
                iRe = (iRe | 2);
            if (Warpenabled) iRe = (iRe | 4);
            if (ChgPercentenabled) iRe = (iRe | 8);
            if (VarErrorenabled) iRe = (iRe | 16);
            return iRe;
        }
        public string[] GetOverLimit()
        {
            /*
            //                                       00      01      02      03     04     05      06      07       08       09     10      11      12       13     14   15    16
            string[] sColOverLimit = new string[] { "序号", "子站", "变量", "描述", "LL", "LL文本", "LO", "LO文本", "HI", "HI文本", "HH", "HH文本", "死区", "偏差", "±", "%", "周期" };
            if (nLimtLO[0].enabled || nLimtLO[1].enabled || nLimtLO[2].enabled || nLimtLO[3].enabled || Deadbandenabled || Warpenabled || ChgPercentenabled)
            {
                string[] str1 = new string[10];
                str1[2] = Name;
                for (int i = 0; i < 4; i++)
                {
                    if (nLimtLO[i].enabled)
                    {
                        str1[3 + i] = nLimtLO[i].value.ToString() + ";" + nLimtLO[i].text + ";" + nLimtLO[i].priority;
                    }
                }
                if (Deadbandenabled)
                    str1[7] = Deadbandvalue.ToString();
                if (Warpenabled)
                    str1[8] = Warpdefault.ToString() + "±" + Warp.ToString() + ";" + Warppriority;
                if (ChgPercentenabled)
                    str1[9] = ChgPercent.ToString() + "%;" + Chgperiod.ToString() + "s;" + ChgPerpriority;
                return str1;
            }
            else
             * */
            return null;
        }

        public string[] GetVarState()
        {
            //string[] sColVarState = new string[] { 
            //"0序号", "1子站名称", "2变量名称", "3优先级", 
            //"41组变位", "51组文本", "61组条件", "71组程序", 
            //"82组变位", "92组文本", "102组条件", "112组程序", 
            //"123组变位", "133组文本", "143组条件", "153组程序", "164组以上" };
            if (ListVarState.Count == 0)
                return null;
            string[] str1 = new string[17];
            str1[2] = Name;
            str1[3] = VarStatepriority.ToString();
            string str4 = "";
            for (int i = 0; i < ListVarState.Count; i++)
            {
                if (i < 3)
                {
                    str1[i * 4 + 4] = ListVarState[i].oldvalue.ToString() + "->" + ListVarState[i].newvalue.ToString();
                    str1[i * 4 + 5] = ListVarState[i].text;
                    str1[i * 4 + 6] = ListVarState[i].addition;
                    str1[i * 4 + 7] = ListVarState[i].Program;
                }
                else
                {
                    str4 += ListVarState[i].oldvalue.ToString() + ",";
                    str4 += ListVarState[i].oldvalue.ToString() + "->" + ListVarState[i].newvalue.ToString() + ",";
                    str4 += ListVarState[i].text + ",";
                    str4 += ListVarState[i].addition + ",";
                    str4 += ListVarState[i].Program + ";";
                }
            }
            str1[16] = str4;
            return str1;
        }

        public string[] GetVarError()
        {
            //string[] sColVarError = new string[] { "0序号", "1子站名称", "2变量名称", "3最小值", "4文本", "5最大值", "6文本", "7优先级" };
            if (VarErrorenabled)
            {
                string[] str1 = new string[8];
                str1[2] = Name;
                str1[3] = minvalue.ToString();
                str1[4] = mintext;
                str1[5] = maxvalue.ToString();
                str1[6] = maxtext;
                str1[7] = VarErrorpriority.ToString();
                return str1;
            }
            else
            {
                return null;
            }
        }

        public bool LoadFromNode(XmlElement VarNode)//读取报警值
        {
            try
            {
                nLimtLO.Clear();
                nLimtHI.Clear();
                ListVarState.Clear();
                //读取越限报警
                string xpath = "OverLimit";
                XmlElement temNode = (XmlElement)VarNode.SelectSingleNode(xpath);

                foreach (XmlElement nNode in temNode.ChildNodes)
                {
                    bool k = true;
                    switch (nNode.Name)
                    {
                        case "LimitLO":     //越低限
                            SLimit nLimit = new SLimit();
                            nLimit.value = Convert.ToDouble(nNode.GetAttribute("value"));
                            nLimit.Delay = Convert.ToInt32(nNode.GetAttribute("Delay"));
                            nLimit.Deadband = Convert.ToDouble(nNode.GetAttribute("Deadband"));
                            nLimit.priority = Convert.ToInt32(nNode.GetAttribute("priority"));
                            nLimit.text = nNode.GetAttribute("text");
                            for (int i = 0; i < nLimtLO.Count; i++)
                            {
                                if (nLimit.value < nLimtLO[i].value)
                                {
                                    nLimtLO.Insert(i, nLimit);
                                    k = false;
                                    break;
                                }
                            }
                            if (k)
                                nLimtLO.Add(nLimit);
                            break;
                        case "LimitHI":     //越高限
                            nLimit = new SLimit();
                            nLimit.value = Convert.ToDouble(nNode.GetAttribute("value"));
                            nLimit.Delay = Convert.ToInt32(nNode.GetAttribute("Delay"));
                            nLimit.Deadband = Convert.ToDouble(nNode.GetAttribute("Deadband"));
                            nLimit.priority = Convert.ToInt32(nNode.GetAttribute("priority"));
                            nLimit.text = nNode.GetAttribute("text");
                            for (int i = 0; i < nLimtLO.Count; i++)
                            {
                                if (nLimit.value < nLimtLO[i].value)
                                {
                                    nLimtHI.Insert(i, nLimit);
                                    k = false;
                                    break;
                                }
                            }
                            if (k)
                                nLimtHI.Add(nLimit);
                            break;
                        case "Warp":        //偏差
                            Warpenabled = Convert.ToBoolean(nNode.GetAttribute("enabled"));
                            if (Warpenabled)
                            {
                                Warpdefault = Convert.ToDouble(nNode.GetAttribute("default"));
                                Warp = Convert.ToDouble(nNode.GetAttribute("warp"));
                                WarpDeadband = Convert.ToDouble(nNode.GetAttribute("Deadband"));
                                Warppriority = Convert.ToInt32(nNode.GetAttribute("priority"));
                            }
                            break;
                        case "ChangeRate":  //变化率
                            ChgPercentenabled = bool.Parse(nNode.GetAttribute("enabled"));
                            if (ChgPercentenabled)
                            {
                                ChgPercent = Double.Parse(nNode.GetAttribute("percent"));
                                Chgperiod = Double.Parse(nNode.GetAttribute("period"));
                                ChgPerpriority = Convert.ToInt32(nNode.GetAttribute("priority"));
                            }
                            break;
                        default:
                            break;
                    }
                }
                //读取变位报警
                xpath = "VarStates";
                temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                ListVarState.Clear();
                foreach (XmlElement varstate in temNode.ChildNodes)
                {
                    CVarState objVarState = new CVarState();
                    objVarState.oldvalue = Convert.ToDouble(varstate.GetAttribute("oldvalue"));
                    objVarState.newvalue = Convert.ToDouble(varstate.GetAttribute("newvalue"));
                    objVarState.text = varstate.GetAttribute("text");
                    objVarState.addition = varstate.GetAttribute("addition");
                    objVarState.Program = varstate.GetAttribute("Program");
                    objVarState.Delay = Convert.ToInt32(varstate.GetAttribute("Delay"));
                    ListVarState.Add(objVarState);
                }
                VarStatepriority = Convert.ToInt32(temNode.GetAttribute("priority"));
                //读取变量异常报警
                xpath = "VarError";
                temNode = (XmlElement)VarNode.SelectSingleNode(xpath);
                VarErrorenabled = bool.Parse(temNode.GetAttribute("enabled"));
                if (VarErrorenabled)
                {
                    minvalue = Double.Parse(temNode.GetAttribute("minvalue"));
                    mintext = temNode.GetAttribute("mintext");
                    maxvalue = Double.Parse(temNode.GetAttribute("maxvalue"));
                    maxtext = temNode.GetAttribute("maxtext");
                    VarErrorpriority = Convert.ToInt32(temNode.GetAttribute("priority"));
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void SaveAlarmToNode(XmlElement nNode, XmlDocument MyXmlDoc)
        {
            XmlElement OverLimit = MyXmlDoc.CreateElement("OverLimit");
            foreach(SLimit nLimit in nLimtLO)
            {
                XmlElement LL = MyXmlDoc.CreateElement("LimitLO");
                LL.SetAttribute("value", nLimit.value.ToString());
                LL.SetAttribute("text", nLimit.text);
                LL.SetAttribute("Delay", nLimit.Delay.ToString());
                LL.SetAttribute("Deadband", nLimit.Deadband.ToString());
                LL.SetAttribute("priority", nLimit.priority.ToString());
                OverLimit.AppendChild(LL);
            }
            foreach (SLimit nLimit in nLimtHI)
            {
                XmlElement LL = MyXmlDoc.CreateElement("LimitHI");
                LL.SetAttribute("value", nLimit.value.ToString());
                LL.SetAttribute("text", nLimit.text);
                LL.SetAttribute("Delay", nLimit.Delay.ToString());
                LL.SetAttribute("Deadband", nLimit.Deadband.ToString());
                LL.SetAttribute("priority", nLimit.priority.ToString());
                OverLimit.AppendChild(LL);
            }

            XmlElement xWarp = MyXmlDoc.CreateElement("Warp");
            xWarp.SetAttribute("enabled", Warpenabled.ToString());
            xWarp.SetAttribute("default", Warpdefault.ToString());
            xWarp.SetAttribute("warp", Warp.ToString());
            xWarp.SetAttribute("priority", Warppriority.ToString());

            XmlElement ChangeRate = MyXmlDoc.CreateElement("ChangeRate");
            ChangeRate.SetAttribute("enabled", ChgPercentenabled.ToString());
            ChangeRate.SetAttribute("percent", ChgPercent.ToString());
            ChangeRate.SetAttribute("period", Chgperiod.ToString());
            ChangeRate.SetAttribute("priority", ChgPerpriority.ToString());
            OverLimit.AppendChild(xWarp);
            OverLimit.AppendChild(ChangeRate);

            XmlElement VarState = MyXmlDoc.CreateElement("VarStates");
            VarState.SetAttribute("priority", VarStatepriority.ToString());
            foreach (CVarState node in ListVarState)
            {
                XmlElement varstate1 = MyXmlDoc.CreateElement("VarState");

                varstate1.SetAttribute("oldvalue", node.oldvalue.ToString());
                varstate1.SetAttribute("newvalue", node.newvalue.ToString());
                varstate1.SetAttribute("text", node.text);
                varstate1.SetAttribute("Delay", node.Delay.ToString());
                varstate1.SetAttribute("addition", node.addition);
                varstate1.SetAttribute("Program", node.Program);
                VarState.AppendChild(varstate1);
            }

            XmlElement VarError = MyXmlDoc.CreateElement("VarError");
            VarError.SetAttribute("enabled", VarErrorenabled.ToString());
            VarError.SetAttribute("minvalue", minvalue.ToString());
            VarError.SetAttribute("mintext", mintext);
            VarError.SetAttribute("maxvalue", maxvalue.ToString());
            VarError.SetAttribute("maxtext", maxtext);
            VarError.SetAttribute("priority", VarErrorpriority.ToString());

            nNode.AppendChild(OverLimit);
            nNode.AppendChild(VarState);
            nNode.AppendChild(VarError);
        }

        /// <summary>
        /// 获取越限报警结果
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="StaName"></param>
        public void GetELimitAlarm(Double Value)
        {
            for (int i = 0; i < nLimtLO.Count; i++)//检查越低限报警
            {
                if (nLimtLO[i].bAlarm == 2)//原来处于报警状态时
                {
                    if (Value > nLimtLO[i].MaxValue)//如果超过最大值取消报警
                    {
                        nLimtLO[i].bAlarm = 0;
                    }
                }
                else if (nLimtLO[i].bAlarm == 1)//原来处于报警预备状态，只是延时时间不到
                {
                    if (Value <= nLimtLO[i].MinValue)//继续满足报警条件，检查延时时间，满足就置报警
                    {
                        if ((DateTime.Now - nLimtLO[i].AlarmDT).TotalSeconds >= nLimtLO[i].Delay)//延时到
                        {
                            nLimtLO[i].bAlarm = 2;
                        }
                    }
                    else//不满足报警条件，取消报警
                    {
                        nLimtLO[i].bAlarm = 0;
                    }
                }
                else//原来处于非报警状态
                {
                    if (Value <= nLimtLO[i].MinValue)//满足报警条件,启动报警延时
                    {
                        nLimtLO[i].AlarmDT = DateTime.Now;
                        if ((DateTime.Now - nLimtLO[i].AlarmDT).TotalSeconds >= nLimtLO[i].Delay)//延时到
                            nLimtLO[i].bAlarm = 2;
                        else
                            nLimtLO[i].bAlarm = 1;
                    }
                }
            }
            for (int i = 0; i < nLimtHI.Count; i++)//检查越高限报警
            {
                if (nLimtHI[i].bAlarm == 2)//原来处于报警状态时
                {
                    if (Value < nLimtHI[i].MinValue)//如果低于最小值取消报警
                    {
                        nLimtHI[i].bAlarm = 0;
                    }
                }
                else if (nLimtHI[i].bAlarm == 1)//原来处于报警预备状态，只是延时时间不到
                {
                    if (Value >= nLimtHI[i].MaxValue)//继续满足报警条件，检查延时时间，满足就置报警
                    {
                        if ((DateTime.Now - nLimtHI[i].AlarmDT).TotalSeconds >= nLimtHI[i].Delay)//延时到
                        {
                            nLimtHI[i].bAlarm = 2;
                        }
                    }
                    else//不满足报警条件，取消报警
                    {
                        nLimtHI[i].bAlarm = 0;
                    }
                }
                else//原来处于非报警状态
                {
                    if (Value >= nLimtHI[i].MinValue)//满足报警条件,启动报警延时
                    {
                        nLimtHI[i].AlarmDT = DateTime.Now;
                        if ((DateTime.Now - nLimtHI[i].AlarmDT).TotalSeconds >= nLimtHI[i].Delay)//延时到
                            nLimtHI[i].bAlarm = 2;
                        else
                            nLimtHI[i].bAlarm = 1;
                    }
                }
            }

            int nLimtType1 = 0;
            for (int i = 0; i < nLimtLO.Count; i++)//检查越低限报警
            {
                if (nLimtLO[i].bAlarm == 2)
                {
                    nLimtType1 = -1 - i;
                    break;
                }
            }
            if (nLimtType1 == 0)
            {
                for (int i = nLimtHI.Count - 1; i >= 0; i--)//检查越低限报警
                {
                    if (nLimtHI[i].bAlarm == 2)
                    {
                        nLimtType1 = i + 1;
                        break;
                    }
                }
            }

            if (nLimtType1 != nLimtType)
            {
                GetELimitAlarm(nLimtType, nLimtType1);
                nLimtType = nLimtType1;
            }
        }
        //产生越限报警消息
        public void GetELimitAlarm(int oldType, int newType)
        {
            int iN = Math.Abs(newType) - 1;
            int iO = Math.Abs(oldType) - 1;
            if (newType == 0)
            {
                GetELimitAlarmMsg(oldType, iO, " 恢复");
            }
            else if (oldType == 0)
            {
                GetELimitAlarmMsg(newType, iN, "");
            }
            else if (iN < iO && oldType < 0 && newType < 0)
            {
                GetELimitAlarmMsg(newType, iN, "");
            }
            else if (iN > iO && oldType < 0 && newType < 0)
            {
                GetELimitAlarmMsg(oldType, iO, " 恢复");
                GetELimitAlarmMsg(newType, iN, "");
            }
            else if (iN < iO && oldType > 0 && newType > 0)
            {
                GetELimitAlarmMsg(oldType, iO, " 恢复");
                GetELimitAlarmMsg(newType, iN, "");
            }
            else if (iN > iO && oldType > 0 && newType > 0)
            {
                GetELimitAlarmMsg(newType, iN, "");
            }
            else if (oldType * newType < 0)
            {
                GetELimitAlarmMsg(oldType, iO, " 恢复");
                GetELimitAlarmMsg(newType, iN, "");
            }
            else
            {
                GetELimitAlarmMsg(newType, iN, "");
            }
            oldType = newType;
        }

        public void GetELimitAlarmMsg(int iLOHI, int index, string sText)
        {
            string Recorder;
            EAlarmPriority priority;
            if (iLOHI < 0)
            {
                priority = (EAlarmPriority)nLimtLO[index].priority;
                Recorder = nLimtLO[index].text + sText;
            }
            else
            {
                priority = (EAlarmPriority)nLimtHI[index].priority;
                Recorder = nLimtHI[index].text + sText;
            }
            cAlarmMsgAddMsg(Recorder, EAlarmType.OverLimit, priority);
        }
        //获取开关变位报警
        public void GetVarStateAlarmMsg(Double OldValue, Double NewValue)
        {
            try
            {
                if (nVarState != null)
                {
                    if (OldValue == NewValue && nVarState.newvalue == NewValue)
                    {
                        if ((DateTime.Now - nVarState.AlarmDT).TotalSeconds >= nVarState.Delay)//延时到
                            {
                                cAlarmMsgAddMsg(nVarState.text, EAlarmType.StateChange, (EAlarmPriority)VarStatepriority);
                                nVarState = null;
                                return;
                            }
                    }
                    else
                        nVarState = null;
                }
                foreach (CVarState node in ListVarState)
                {
                    if (node.oldvalue == OldValue && node.newvalue == NewValue && OldValue != NewValue)
                    {
                        if (node.Delay == 0)
                        {
                            //立马报警
                            cAlarmMsgAddMsg(node.text, EAlarmType.StateChange, (EAlarmPriority)VarStatepriority);
                            nVarState = null;
                        }
                        else
                        {
                            nVarState = node;
                            nVarState.AlarmDT = DateTime.Now;
                            if ((DateTime.Now - nVarState.AlarmDT).TotalSeconds >= nVarState.Delay)//延时到
                            {
                                cAlarmMsgAddMsg(nVarState.text, EAlarmType.StateChange, (EAlarmPriority)VarStatepriority);
                                nVarState = null;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void cAlarmMsgAddMsg(string text, EAlarmType AlarmType, EAlarmPriority priority)
        {
            //string sShow = "[" + staAlarm.GetAlarmTypeString(AlarmType) + "]" + text + "；等级:" + staAlarm.GetPriorityString(priority);
            //MessageBox.Show(sShow);
            CAlarmMsgEventArgs e = new CAlarmMsgEventArgs();
            e.Date_Time = DateTime.Now;
            e.Recorder = text;
            e.priority = priority;
            e.eAlarmType = AlarmType;
            e.StaName = staName;
            staAlarm.OnAlarmEvent(e);
        }
    }

    public class CAlarm
    {
        public event EventHandler AlarmEvent; //声明事件
        public CUserInfo nUserInfo;
        public CAlarmMsg cAlarmMsg = new CAlarmMsg();

        public string PrjName = "";
        public string strPath = "";

        public LSDatabase nDatabase;

        public void OnAlarmEvent(CAlarmMsgEventArgs e)
        {
            EventHandler alarmevent = AlarmEvent;
            if (alarmevent != null)
            {
                e.ALGuid = System.Guid.NewGuid();
                nDatabase.SaveMsg(e);
                cAlarmMsg.AddMsg(e);
                alarmevent(this, e);
            }
        }

        public void SetConfirm(CAlarmMsgEventArgs e)
        {
            e.bConfirm = false;
            e.ConfirmTime = DateTime.Now;
            e.ConfirmName = nUserInfo.UserName;
            nDatabase.UpdateMsg(e);
        }

        public static string InsertToText(string str1, CStation nSta, CVar nVar)
        {
            if (str1.Length > 0)
            {
                str1 = str1.Replace("[SN]", nSta.Name);
                str1 = str1.Replace("[SD]", nSta.Description);
                str1 = str1.Replace("[VN]", nVar.Name);
                str1 = str1.Replace("[VD]", nVar.Description);
            }
            return str1;
        }

        public static bool LikeString(string strSource, string strKey)
        {
            string str01 = strKey.Replace(';', ',');
            str01 = str01.Replace('；', ',');
            str01 = str01.Replace('，', ',');
            str01 = str01.Replace(' ', ',');

            string[] str2 = str01.Split(',');
            for (int i = 0; i < str2.Length; i++)
            {
                int ik = strSource.IndexOf(str2[i]);
                if (ik < 0)
                    return false;
            }
            return true;
        }

        public string GetAlarmTypeString(EAlarmType AlarmType)
        {
            switch (AlarmType)
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

        public string GetPriorityString(EAlarmPriority priority)
        {
            switch (priority)
            {
                case EAlarmPriority.PRIORITY_3:
                    return "三级";
                case EAlarmPriority.PRIORITY_2:
                    return "二级";
                case EAlarmPriority.PRIORITY_1:
                    return "一级";
                default:
                    return "未定义";
            }
        }
    }

    //报警类型：系统，SOE，开关变位，越限，子站通信，变量错误，人工操作
    public enum EAlarmType
    {
        NONE = -1,
        /// <summary>
        /// 系统状态
        /// </summary>
        Server = 0,         //系统
        SOE = 1,            //SOE
        /// <summary>
        /// 开关变位
        /// </summary>
        StateChange = 2,    //开关变位
        /// <summary>
        /// 越限报警
        /// </summary>
        OverLimit = 3,      //越限
        /// <summary>
        /// 子站通道状态
        /// </summary>
        StationState = 4,   //子站通信状态
        /// <summary>
        /// 变量错误
        /// </summary>
        VarError = 5,     //变量错误\
        /// <summary>
        /// 人工操作
        /// </summary>
        ManualAct = 6,       //人工操作
        /// <summary>
        /// PLC报警
        /// </summary>
        PLCALarm = 7
    }
    //报警内容结构体，
    public class CAlarmMsgEventArgs : EventArgs
    {
        public DateTime? Date_Time;     //报警时间
        public string StaName;          //子站名
        public EAlarmType eAlarmType;   //报警类型
        public string Recorder;         //报警内容
        public string Remark;           //备注
        public EAlarmPriority priority; //报警优先级
        public DateTime? ConfirmTime;   //确认时间
        public string ConfirmName;      //确认用户名
        public Guid ALGuid;
        public bool bConfirm;           //是否需要确认      ，
        public CAlarmMsgEventArgs()
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
        public string PriorityString
        {
            get
            {
                switch (priority)
                {
                    case EAlarmPriority.PRIORITY_3:
                        return "三级";
                    case EAlarmPriority.PRIORITY_2:
                        return "二级";
                    case EAlarmPriority.PRIORITY_1:
                        return "一级";
                    default:
                        return "未定义";
                }
            }
        }
        public string PriorityStringNum
        {
            get
            {
                switch (priority)
                {
                    case EAlarmPriority.PRIORITY_3:
                        return "3";
                    case EAlarmPriority.PRIORITY_2:
                        return "2";
                    case EAlarmPriority.PRIORITY_1:
                        return "1";
                    default:
                        return "0";
                }
            }
        }
        public string AlarmTypeString
        {
            get
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
                    case EAlarmType.PLCALarm:
                        return "PLC报警";
                    default:
                        return "未定义";
                }
            }
        }
    }

    public class CAlarmMsg
    {
        public bool bUpdate = false;
        public List<CAlarmMsgEventArgs> ListAlarmMsg = new List<CAlarmMsgEventArgs>();//所有报警

        public void AddMsg(CAlarmMsgEventArgs e)
        {
            ListAlarmMsg.Add(e);
            bUpdate = true; 
            while (ListAlarmMsg.Count > CCONST.ListMax)
            {
                ListAlarmMsg.RemoveAt(0);
            }
        }
    }
}