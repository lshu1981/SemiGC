using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Diagnostics;
using System.Collections;

namespace LSSCADA
{
    public enum LCExpressType
    {
        StaticText =0,//静态文本 
        RTVar =1, //电力
        Var =2, //通信
        Expression = 3//表达式
    }

    public class CParameterOperate
    {
        public static ArrayList GetParamList(string Exipression)
        {
            char[] chArray = Exipression.Trim().ToCharArray();
            ArrayList ListParam = new ArrayList();
            int i = 0;
            int startpos = 0;
            int endpos = 0;
            ExpressionParser.EDFAState prestate = ExpressionParser.EDFAState.Start;	//DFA的前一个状态
            ListParam.Clear();
            while (i < chArray.Length)
            {
                if (prestate == ExpressionParser.EDFAState.CharStr)
                {
                    //字符串结构 - 字符串结构是处理当前态 - 其它类型是处理上一态
                    if (chArray[i] == '"')
                    {
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                }
                else if (prestate == ExpressionParser.EDFAState.ABCStr)
                {//参数或者字母已经开始
                    //不是字母、数字、下划线、点号，表示参数结束
                    if (chArray[i] != '_' && chArray[i] != '.' && !Char.IsLetter(chArray[i]) && !Char.IsDigit(chArray[i]))
                    {
                        endpos = i - 1;
                        CParameter obj =  GetParam(startpos, endpos, Exipression);
                        if(obj != null)
                            ListParam.Add(obj);
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                }
                else if (chArray[i] == '"')
                {
                    if (prestate == ExpressionParser.EDFAState.CharStr)
                    {
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                    else
                    {
                        //字符串开始
                        prestate = ExpressionParser.EDFAState.CharStr;
                    }
                }
                else if (Char.IsLetter(chArray[i]))
                {//参数 或者字母开始
                    //字母
                    if (prestate != ExpressionParser.EDFAState.ABCStr)
                    {
                        prestate = ExpressionParser.EDFAState.ABCStr;
                        startpos = i; //保存开始位置
                    }
                }
                i++;
                if (i == chArray.Length)
                {
                    endpos = i - 1;
                    if (prestate == ExpressionParser.EDFAState.ABCStr)
                    {
                        CParameter obj = GetParam(startpos, endpos, Exipression);
                        if (obj != null)
                            ListParam.Add(obj);
                    }
                }
            }// end while
            return ListParam;
        }

        public static ArrayList GetParamList(string Exipression , string StaName)
        {
            char[] chArray = Exipression.Trim().ToCharArray();
            ArrayList ListParam = new ArrayList();
            int i = 0;
            int startpos = 0;
            int endpos = 0;
            ExpressionParser.EDFAState prestate = ExpressionParser.EDFAState.Start;	//DFA的前一个状态
            ListParam.Clear();
            while (i < chArray.Length)
            {
                if (prestate == ExpressionParser.EDFAState.CharStr)
                {
                    //字符串结构 - 字符串结构是处理当前态 - 其它类型是处理上一态
                    if (chArray[i] == '"')
                    {
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                }
                else if (prestate == ExpressionParser.EDFAState.ABCStr)
                {//参数或者字母已经开始
                    //不是字母、数字、下划线、点号，表示参数结束
                    if (chArray[i] != '_' && chArray[i] != '.' && !Char.IsLetter(chArray[i]) && !Char.IsDigit(chArray[i]))
                    {
                        endpos = i - 1;
                        CParameter obj = GetParam(startpos, endpos, Exipression, StaName);
                        if (obj != null)
                            ListParam.Add(obj);
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                }
                else if (chArray[i] == '"')
                {
                    if (prestate == ExpressionParser.EDFAState.CharStr)
                    {
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                    else
                    {
                        //字符串开始
                        prestate = ExpressionParser.EDFAState.CharStr;
                    }
                }
                else if (Char.IsLetter(chArray[i]))
                {//参数 或者字母开始
                    //字母
                    if (prestate != ExpressionParser.EDFAState.ABCStr)
                    {
                        prestate = ExpressionParser.EDFAState.ABCStr;
                        startpos = i; //保存开始位置
                    }
                }
                i++;
                if (i == chArray.Length)
                {
                    endpos = i - 1;
                    if (prestate == ExpressionParser.EDFAState.ABCStr)
                    {
                        CParameter obj = GetParam(startpos, endpos, Exipression, StaName);
                        if (obj != null)
                            ListParam.Add(obj);
                    }
                }
            }// end while
            return ListParam;
        }

        private static CParameter GetParam(int startpos, int endpos, string Exipression)
        {
            string temp = null;
            if (endpos >= 0 && startpos >= 0)
            {
                temp = Exipression.Substring(startpos, endpos - startpos + 1);
                temp = temp.Replace(" ", "").Replace("\r\n", "").Replace("\t", "");
                foreach (string name in Enum.GetNames(typeof(ExpressionParser.EKeyword)))//检查是否函数关键字
                {
                    if (name.ToLower().Equals(temp.ToLower()))
                    {
                        return null;
                    }
                }
                if (temp != null)
                {
                    CParameter nObj = new CParameter(temp, "0");
                    string[] str1 = temp.Split('.');
                    if (str1.Length >= 2)
                    {
                        nObj.cVar = frmMain.staComm.GetVarByStaNameVarName(str1[0], str1[1]);
                    }
                    else
                    {
                        nObj.cVar = null;
                    }
                    return nObj;
                }
            }
            return null;
        }

        private static CParameter GetParam(int startpos, int endpos, string Exipression,string StaName)
        {
            string temp = null;
            if (endpos >= 0 && startpos >= 0)
            {
                temp = Exipression.Substring(startpos, endpos - startpos + 1);
                temp = temp.Replace(" ", "").Replace("\r\n", "").Replace("\t", "");
                foreach (string name in Enum.GetNames(typeof(ExpressionParser.EKeyword)))//检查是否函数关键字
                {
                    if (name.ToLower().Equals(temp.ToLower()))
                    {
                        return null;
                    }
                }
                if (temp != null)
                {
                    CParameter nObj = new CParameter(temp, "0");
                    string[] str1 = temp.Split('.');
                    if (str1.Length >= 2)
                    {
                        nObj.cVar = frmMain.staComm.GetVarByStaNameVarName(str1[0], str1[1]);
                    }
                    else if (str1.Length == 1)
                    {
                        nObj.cVar = frmMain.staComm.GetVarByStaNameVarName(StaName, str1[0]);
                    }
                    else
                    {
                        nObj.cVar = null;
                    }
                    return nObj;
                }
            }
            return null;
        }
    }

    //参数类
    public class CParameter
    {
        private string _Name = "";
        private string _Value = "";
        public CVar cVar = null;
        [Browsable(true), Description("变量路径"), Category("Design"), DisplayName("变量路径")]
        public String Name
        {
            get
            { return _Name; }
            set
            {
                _Name = value;
            }
        }
        [Browsable(true), Description("变量值"), Category("Design"), DisplayName("变量值")]
        public String Value
        {
            get
            {
                if (cVar != null)
                    return cVar.GetDoubleValue().ToString();
                else
                    return _Value; 
            }
            set
            {
                _Value = value;
            }
        }
        public CParameter(string _name, string _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    public class CExpression
    {
        //参数
        public ArrayList ListParam = new ArrayList();

        private ExpressionParser.ExpressionParse ex = new ExpressionParser.ExpressionParse();

        public LCExpressType ExpressType = LCExpressType.StaticText;
        public string sText = "";
        public CVar cVar = null;//Var类型时关联的变量引用
        public CStation cSta = null;//Var类型时关联的子站引用
        public String Exipression = "";
        public bool IsShowUnit = false;

        public CExpression Clone()
        {
            CExpression obj = (CExpression)this.MemberwiseClone();
            if (cVar != null)
                obj.cVar = cVar.Clone();
            obj.ListParam.Clear();
            return obj;

        }


        /// <summary>
        /// 获取参数
        /// </summary>
        public void GetParam()
        {
            char[] chArray = Exipression.Trim().ToCharArray();
            int i = 0;
            int startpos = 0;
            int endpos = 0;
            ExpressionParser.EDFAState prestate = ExpressionParser.EDFAState.Start;	//DFA的前一个状态
            ListParam.Clear();
            while (i < chArray.Length)
            {
                if (prestate == ExpressionParser.EDFAState.CharStr)
                {
                    //字符串结构 - 字符串结构是处理当前态 - 其它类型是处理上一态
                    if (chArray[i] == '"')
                    {
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                }
                else if (prestate == ExpressionParser.EDFAState.ABCStr)
                {//参数或者字母已经开始
                    //不是字母、数字、下划线、点号，表示参数结束
                    if (chArray[i] != '_' && chArray[i] != '.' && !Char.IsLetter(chArray[i]) && !Char.IsDigit(chArray[i]))
                    {
                        endpos = i - 1;
                        AddToLink(startpos, endpos);
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                }
                else if (chArray[i] == '"')
                {
                    if (prestate == ExpressionParser.EDFAState.CharStr)
                    {
                        prestate = ExpressionParser.EDFAState.Start;
                    }
                    else
                    {
                        //字符串开始
                        prestate = ExpressionParser.EDFAState.CharStr;
                    }
                }
                else if (Char.IsLetter(chArray[i]))
                {//参数 或者字母开始
                    //字母
                    if (prestate != ExpressionParser.EDFAState.ABCStr)
                    {
                        prestate = ExpressionParser.EDFAState.ABCStr;
                        startpos = i; //保存开始位置
                    }
                }
                i++;
                if (i == chArray.Length)
                {
                    endpos = i - 1;
                    if (prestate == ExpressionParser.EDFAState.ABCStr)
                        AddToLink(startpos, endpos);
                }
            }// end while
        }

        private void AddToLink(int startpos, int endpos)
        {
            string temp = null;
            if (endpos >= 0 && startpos >= 0)
            {
                temp = Exipression.Substring(startpos, endpos - startpos + 1);
                temp = temp.Replace(" ", "").Replace("\r\n", "").Replace("\t", "");
                foreach (string name in Enum.GetNames(typeof(ExpressionParser.EKeyword)))
                {
                    if (name.ToLower().Equals(temp.ToLower()))
                    {
                        return;
                    }
                }
                if (temp != null)
                {
                    CParameter nObj = new CParameter(temp, "0");
                    string[] str1 = temp.Split('.');
                    if (str1.Length >= 2)
                    {
                        nObj.cVar = frmMain.staComm.GetVarByStaNameVarName(str1[0], str1[1]);
                    }
                    else
                    {
                        nObj.cVar = null;
                    }
                    ListParam.Add(nObj);
                }
            }
        }

        public bool Edit() { return false; }
        public override String ToString()
        {
            return sText;
        }
        public void GetDeviceVar()
        {
            if (ExpressType == LCExpressType.Var)
            {
                string str1 = Exipression.Replace(".", ",");
                string[] sPath = str1.Split(',');
                if (sPath.Length >= 3)
                {
                    cSta = frmMain.staComm.GetStaByStaName(sPath[1]);
                    cVar = frmMain.staComm.GetVarByStaNameVarName(sPath[1], sPath[2]);
                }
            }
            else if(ExpressType == LCExpressType.Expression)//表达式
            {
                ListParam.Clear();
                ListParam = CParameterOperate.GetParamList(Exipression);
            }
        }

        public void GetDeviceVar(string StaName)
        {
            if (ExpressType == LCExpressType.Var)
            {
                string str1 = Exipression.Replace(".", ",");
                string[] sPath = str1.Split(',');
                if (sPath.Length >= 3)
                {
                    cSta = frmMain.staComm.GetStaByStaName(sPath[1]);
                    cVar = frmMain.staComm.GetVarByStaNameVarName(sPath[1], sPath[2]);
                }
            }
            else if (ExpressType == LCExpressType.Expression)//表达式
            {
                ListParam.Clear();
                ListParam = CParameterOperate.GetParamList(Exipression, StaName);
            }
        }

        public String execStr()
        {
            if (ExpressType == LCExpressType.Expression)
            {
                string tempExipression = Exipression;
                //Debug.WriteLine(tempExipression);
                foreach (CParameter node in ListParam)
                {
                    tempExipression = tempExipression.Replace(node.Name, node.Value);
                }
                tempExipression = tempExipression.Replace("==", "=");
                ExpressionParser.ExpressionParse res = new ExpressionParser.ExpressionParse();
                //Debug.WriteLine(tempExipression);
                res.Expression = tempExipression;
                string mes = res.ExecuteToString();
                
                if (IsShowUnit)
                    return mes + cVar.Unit;
                else
                    return mes;
            }
            else if (ExpressType == LCExpressType.Var)
            {
                if (cVar != null)
                {
                    if (IsShowUnit)
                        return cVar.GetStringValue(sText) + cVar.Unit;

                    else
                        return cVar.GetStringValue(sText);
                }
                else
                    return "VarIsNull";
            }
            else if (ExpressType == LCExpressType.StaticText)
                return sText;
            return "0";
        }

        public Double execInt64()
        {
            if (ExpressType == LCExpressType.Expression)
            {
                string tempExipression = Exipression;
                //Debug.WriteLine(tempExipression);
                foreach (CParameter node in ListParam)
                {
                    tempExipression = tempExipression.Replace(node.Name, node.Value);
                }
                tempExipression = tempExipression.Replace("==", "=");
                ExpressionParser.ExpressionParse res = new ExpressionParser.ExpressionParse();
                //Debug.WriteLine(tempExipression);
                res.Expression = tempExipression;
                string mes = res.ExecuteToString();

                try
                {
                    return Convert.ToDouble(mes);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            else if (ExpressType == LCExpressType.Var)
            {
                if (cVar != null)
                {
                    try
                    {
                        return Convert.ToDouble(cVar.GetInt64Value());
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
                else
                    return 0;
            }
            else if (ExpressType == LCExpressType.StaticText)
                try
                {
                    return Convert.ToDouble(sText);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            return 0;
        }
    }
}
