/********************************************
 * 
 * http://blog.csdn.net/welliu
 * 
 * Email:lgjwell@gmail.com
 * 
 * QQ:147620454
 * 
 *******************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    /// <summary>
    /// 词法分析器
    /// </summary>
    public class PhraseAnalyzer
    {
        public PhraseAnalyzer(string expression)
        {
            _expression = expression;
        }

        private string _expression = string.Empty;
        private Link_OP _link_OP = null;

        #region 词法分析 - Analyze

        /// <summary>
        /// 词法分析
        /// </summary>
        public Link_OP Analyze()
        {
            _link_OP = new Link_OP();
            char[] chArray = _expression.Trim().ToCharArray();
            int i = 0;
            int startpos = 0;
            int endpos = 0;
            EDFAState prestate = EDFAState.Start;	//DFA的前一个状态

            while (i < chArray.Length)
            {
                if (prestate == EDFAState.CharStr)
                {
                    //字符串结构 - 字符串结构是处理当前态 - 其它类型是处理上一态

                    if (chArray[i] == '"' && chArray[i - 1] != '\\')
                    {
                        endpos = i;
                        AddToLink(startpos, endpos, prestate);
                        prestate = EDFAState.Start;
                        startpos = i + 1;
                    }
                    else if (i + 1 == chArray.Length)
                    {
                        throw new Exception("Error! 词法.缺少字符串引号（\"）");
                    }
                }
                else if (chArray[i] == '"')
                {
                    //字符串开始
                    if (prestate != EDFAState.Start)
                    {
                        endpos = i - 1;
                        AddToLink(startpos, endpos, prestate);
                        startpos = i;
                    }

                    prestate = EDFAState.CharStr;
                }
                else if (Char.IsLetter(chArray[i]))
                {
                    //字母
                    if (prestate == EDFAState.Start)
                    {
                        prestate = EDFAState.ABCStr;
                    }
                    else if (prestate != EDFAState.ABCStr)
                    {
                        //处理前一个词
                        endpos = i - 1;	//保存结束位置
                        AddToLink(startpos, endpos, prestate);
                        prestate = EDFAState.ABCStr;
                        startpos = i; //保存开始位置
                    }
                }
                else if (Char.IsDigit(chArray[i]))
                {
                    //数字
                    if (prestate == EDFAState.Start)
                    {
                        prestate = EDFAState.IntStr;
                    }
                    else if (prestate != EDFAState.IntStr && prestate != EDFAState.DoubleStr)
                    {
                        endpos = i - 1;
                        AddToLink(startpos, endpos, prestate);
                        prestate = EDFAState.IntStr;
                        startpos = i;
                    }
                }
                else if (chArray[i] == '.')
                {
                    //小数点
                    if (prestate == EDFAState.IntStr)
                    {
                        prestate = EDFAState.DoubleStr;
                    }
                    else
                    {
                        throw new Exception(string.Format("Error! 词法.错误的小数点位置,索引：{0}", i.ToString()));
                    }
                }
                else if (chArray[i] == ',')
                {
                    //逗号
                    endpos = i - 1;
                    AddToLink(startpos, endpos, prestate);
                    prestate = EDFAState.Comma;
                    startpos = i;
                }
                else if (Char.IsWhiteSpace(chArray[i]) || chArray[i] == '\r' || chArray[i] == '\n' || chArray[i] == '\t')
                {
                    //空格，回车，制表符 跳过
                }
                else if (chArray[i] == '(' || chArray[i] == ')' || chArray[i] == '+' || chArray[i] == '-' || chArray[i] == '*' ||
                    chArray[i] == '/' || chArray[i] == '%' || chArray[i] == '>' || chArray[i] == '<' || chArray[i] == '=')
                {

                    //操作符只能是单值符号+多值符号在链表添加时处理
                    if (prestate != EDFAState.Start)
                    {
                        endpos = i - 1;
                        AddToLink(startpos, endpos, prestate);
                        startpos = i;
                    }

                    prestate = EDFAState.OperatorStr;
                }
                else
                {
                    throw new Exception(string.Format("Error! 词法.非法字符“{0}”（索引：{1}）", chArray[i].ToString(), i.ToString()));
                }

                i++;

                if (i == chArray.Length)
                {
                    endpos = i - 1;

                    if (startpos == endpos && chArray[endpos] == '"')
                    {
                        throw new Exception("Error! 词法.缺少字符串引号（\"）");
                    }

                    AddToLink(startpos, endpos, prestate);
                }
            }// end while

            return _link_OP;
        }

        #endregion

        #region 元数据添加链表 - AddToLink

        /// <summary>
        /// 加入链表
        /// </summary>
        /// <param name="startpos">开始位</param>
        /// <param name="endpos">结束位</param>
        /// <param name="state">处理状态</param>
        private void AddToLink(int startpos, int endpos, EDFAState state)
        {
            string temp = null;
            IToken token = null;
            if (endpos >= 0 && startpos >= 0 && endpos >= startpos)
            {
                temp = _expression.Substring(startpos, endpos - startpos + 1);

                if (state == EDFAState.CharStr)
                {
                    //字符串格式为"abcd" 转换为 adbc
                    if (temp.Length == 2)
                    {
                        temp = string.Empty;
                    }
                    else
                    {
                        temp = temp.Substring(1, temp.Length - 1).Substring(0, temp.Length - 2).Replace("\\\"","\"");
                    }
                }
                else
                {
                    //处理标记中的空格换行符等 例如：8 8,an  d 转换为88,and
                    temp = temp.Replace(" ", "").Replace("\r\n", "").Replace("\t", "");
                }

                switch (state)
                {
                    case EDFAState.IntStr:
                        token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<int>(EDataType.Dint, Convert.ToInt32(temp)), startpos);

                        break;

                    case EDFAState.DoubleStr:
                        token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<double>(EDataType.Ddouble, Convert.ToDouble(temp)), startpos);

                        break;

                    case EDFAState.CharStr:
                        token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<string>(EDataType.Dstring, temp), startpos);
                        break;

                    case EDFAState.Comma:
                        token = new TOKEN<Separator>(ETokenType.token_separator, new Separator(','), startpos);
                        break;

                    case EDFAState.ABCStr:
                        foreach (string name in Enum.GetNames(typeof(EKeyword)))
                        {
                            if (name.ToLower().Equals(temp.ToLower()))
                            {
                                token = new TOKEN<KeyWord>(ETokenType.token_keyword, Define.KeyWords[(EKeyword)Enum.Parse(typeof(EKeyword), temp, true)] , startpos);
                                break;
                            }
                        }

                        if (token == null)
                        {
                            //非法字
                            throw new Exception(string.Format("Error! 非法关键字“{0}”（索引：{1}）", temp, startpos.ToString()));
                        }

                        break;

                    case EDFAState.OperatorStr:
                        bool flag = true;
                        EOperatorType type = EOperatorType.Plus;
                        switch (temp)
                        {
                            case "(":
                                type = EOperatorType.LeftParen;
                                break;
                            case ")":
                                type = EOperatorType.RightParen;
                                break;
                            case "+":
                            case "-":
                                if (_link_OP.Tail != null && ((_link_OP.Tail.Token.Type == ETokenType.token_operand) ||
                                    (_link_OP.Tail.Token.Type == ETokenType.token_operator && 
                                    ((TOKEN<Operator>)_link_OP.Tail.Token).Tag.Type == EOperatorType.RightParen)))
                                {
                                    if (temp == "-")
                                    {
                                        type = EOperatorType.Minus;
                                    }
                                    else
                                    {
                                        type = EOperatorType.Plus;
                                    }
                                }
                                else
                                {
                                    if (temp == "-")
                                    {
                                        type = EOperatorType.Negative;
                                    }
                                    else
                                    {
                                        type = EOperatorType.Positive;
                                    }
                                }
                                break;
                            case "*":
                                type = EOperatorType.Multiply;
                                break;
                            case "/":
                                type = EOperatorType.Divide;
                                break;
                            case "%":
                                type = EOperatorType.Mod;
                                break;
                            case "<":
                                type = EOperatorType.LessThan;
                                break;
                            case ">":
                                if (_link_OP.Tail != null && (_link_OP.Tail.Token.Type == ETokenType.token_operator && 
                                    ((TOKEN<Operator>)_link_OP.Tail.Token).Tag.Type == EOperatorType.LessThan))
                                {
                                    ((TOKEN<Operator>)_link_OP.Tail.Token).Tag = Define.Operators[EOperatorType.NotEqual];
                                    flag = false;
                                }
                                else
                                {
                                    type = EOperatorType.GreaterThan;
                                }
                                break;
                            case "=":
                                if (_link_OP.Tail != null && (_link_OP.Tail.Token.Type == ETokenType.token_operator && 
                                    ((TOKEN<Operator>)_link_OP.Tail.Token).Tag.Type == EOperatorType.LessThan))
                                {
                                    ((TOKEN<Operator>)_link_OP.Tail.Token).Tag = Define.Operators[EOperatorType.LessEqual];
                                    flag = false;
                                }
                                else if (_link_OP.Tail.Token.Type == ETokenType.token_operator && 
                                    ((TOKEN<Operator>)_link_OP.Tail.Token).Tag.Type == EOperatorType.GreaterThan)
                                {
                                    ((TOKEN<Operator>)_link_OP.Tail.Token).Tag = Define.Operators[EOperatorType.GreaterEqual];
                                    flag = false;
                                }
                                else
                                {
                                    type = EOperatorType.Equal;
                                }
                                break;
                            default:
                                break;
                        }

                        if (flag)
                        {
                            token = new TOKEN<Operator>(ETokenType.token_operator, Define.Operators[type], startpos);
                        }

                        break;

                    default:
                        break;
                }

                if (token != null)
                {
                    _link_OP.Add(new TOKENLink(token));
                }
            }
        }

        #endregion
    }
}
