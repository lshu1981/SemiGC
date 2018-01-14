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
    /// 语法分析
    /// </summary>
    public class SyntaxAnalyzer
    {
        //分三步
        //1、关键字语法分析，去关键字返回数据类型
        //2、操作符语法分析
        //3、分析操作符与操作数匹配类型
        public SyntaxAnalyzer()
        {
            _gaz = new GrammarAnalyzer(this);
            _toolBox = new ToolBox();
            _operandSource = new KeyValueList<IToken, string>();
        }

        private GrammarAnalyzer _gaz;
        private ToolBox _toolBox;
        private KeyValueList<IToken, string> _operandSource;

        /// <summary>
        /// 执行语法检查
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public EDataType Execute(TOKENLink startLink, TOKENLink endLink)
        {
            CheckParen(startLink, endLink);
            return Analyze(startLink, endLink).Type;
        }

        #region 分析入口

        /// <summary>
        /// 分析函数
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        public IOperand Analyze(TOKENLink startLink, TOKENLink endLink)
        {
            TOKENLink curLink = startLink;

            while (true)
            {
                if (curLink.Token.Type == ETokenType.token_keyword)
                {
                    TOKENLink endLink_key = null;
                    IOperand result = null;

                    switch (((TOKEN<KeyWord>)curLink.Token).Tag.Type)
                    {
                        case EKeyword.IF:
                            result = _gaz.Key_IF_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.AND:
                        case EKeyword.OR:
                            result = _gaz.Key_ANDOR_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.NOT:
                            result = _gaz.Key_Not_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.FALSE:
                            result = _gaz.Key_False_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.TRUE:
                            result = _gaz.Key_True_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.Len:
                            result = _gaz.Key_Len_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.NowDate:
                            result = _gaz.Key_NowDate_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.ToDateTime:
                            result = _gaz.Key_ToDateTime_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.ToDouble:
                            result = _gaz.Key_ToDouble_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.ToInt:
                            result = _gaz.Key_ToInt_Analyze(curLink, out endLink_key);
                            break;
                        case EKeyword.ToString:
                            result = _gaz.Key_ToString_Analyze(curLink, out endLink_key);
                            break;
                        default:
                            break;
                    }

                    TOKENLink tokenLink = new TOKENLink(new TOKEN<IOperand>(ETokenType.token_operand, result, curLink.Token.Index));
                    _operandSource.Add(tokenLink.Token, ((TOKEN<KeyWord>)curLink.Token).Tag.Value);

                    if (endLink_key != null)
                    {
                        //链表重构
                        if (curLink.Prev != null)
                        {
                            tokenLink.Prev = curLink.Prev;
                            curLink.Prev.Next = tokenLink;
                        }

                        if (endLink_key.Next != null)
                        {
                            tokenLink.Next = endLink_key.Next;
                            endLink_key.Next.Prev = tokenLink;
                        }

                        if (curLink == startLink)
                        {
                            startLink = tokenLink;
                        }

                        if (endLink_key == endLink)
                        {
                            endLink = tokenLink;
                        }

                        curLink = tokenLink;

                    }// end if
                }//end if

                if (curLink == endLink || curLink.Next == null)
                {
                    break;
                }
                else
                {
                    curLink = curLink.Next;
                }
            }

            if (startLink == endLink)
            {
                if (startLink.Token.Type != ETokenType.token_operand)
                {
                    string err = string.Empty;
                    if (startLink.Token.Type == ETokenType.token_operator)
                    {
                        err = string.Format("Error! 操作符“{0}”附近有语法错误（索引：{1}）",
                           ((TOKEN<Operator>)curLink.Token).Tag.Value, curLink.Token.Index.ToString());
                    }
                    else if (startLink.Token.Type == ETokenType.token_separator)
                    {
                        err = string.Format("Error! 分隔符“{0}”附近有语法错误（索引：{1}）",
                           ((TOKEN<Separator>)curLink.Token).Tag.Value.ToString(), curLink.Token.Index.ToString());
                    }
                    else
                    {
                        err = string.Format("Error! 索引{0}附近有语法错误",curLink.Token.Index.ToString());
                    }

                    throw new Exception(err);
                }

                return ((TOKEN<IOperand>)curLink.Token).Tag;
            }
            else
            {
                // 

                return OperatorEvalAnalyze(startLink, endLink);
            }
        }

        #endregion

        #region 检查标记应用环境

        /// <summary>
        /// 去关键字后 检查标记应用环境（不检查操作符与数据类型匹配关系）
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        private void TokenEnvirAnalyze(TOKENLink startLink, TOKENLink endLink)
        {
            TOKENLink curLink = startLink;

            while (true)
            {
                switch (curLink.Token.Type)
                {
                    case ETokenType.token_keyword:
                        throw new Exception(string.Format("Error! 关键字“{0}”未解析（索引：{1}）", 
                            ((TOKEN<KeyWord>)curLink.Token).Tag.Value, curLink.Token.Index.ToString()));

                    case ETokenType.token_operand:
                        if (curLink.Prev != null && (curLink.Prev.Token.Type == ETokenType.token_operand ||
                            (curLink.Prev.Token.Type == ETokenType.token_operator &&
                            ((TOKEN<Operator>)curLink.Prev.Token).Tag.Type == EOperatorType.RightParen)))
                        {
                            string err = "";
                            if (_operandSource[curLink.Token] == null)
                            {
                                err = string.Format("Error! 操作数“{0}”附近有语法错误（索引：{1}）", 
                                    ((TOKEN<IOperand>)curLink.Token).Tag.ToString(), curLink.Token.Index.ToString());
                            }
                            else
                            {
                                err = string.Format("Error! 关键字“{0}”附近有语法错误（索引：{1}）",
                                    _operandSource[curLink.Token], curLink.Token.Index.ToString());
                            }

                            throw new Exception(err);
                        }

                        break;

                    case ETokenType.token_operator:
                        EOperatorType type = ((TOKEN<Operator>)curLink.Token).Tag.Type;
                        switch (type)
                        {
                            case EOperatorType.Positive:  //正
                            case EOperatorType.Negative:  //负
                                if (!(curLink.Next != null && (curLink.Next.Token.Type == ETokenType.token_operand ||
                                    (curLink.Next.Token.Type == ETokenType.token_operator &&
                                    ((TOKEN<Operator>)curLink.Next.Token).Tag.Type == EOperatorType.LeftParen))))
                                {
                                    throw new Exception(string.Format("Error! 一元操作符“{0}”附近有语法错误（索引：{1}）", 
                                        ((TOKEN<Operator>)curLink.Token).Tag.Value, curLink.Token.Index.ToString()));
                                }

                                break;

                            case EOperatorType.LeftParen:
                                if (curLink.Prev != null && (curLink.Prev.Token.Type == ETokenType.token_operand ||
                                   (curLink.Prev.Token.Type == ETokenType.token_operator &&
                                   ((TOKEN<Operator>)curLink.Prev.Token).Tag.Type == EOperatorType.RightParen)))
                                {
                                    throw new Exception(string.Format("Error! 左括弧“{0}”附近有语法错误（索引：{1}）", 
                                        ((TOKEN<Operator>)curLink.Token).Tag.Value, curLink.Token.Index.ToString()));
                                }

                                break;

                            case EOperatorType.RightParen:
                                if (curLink.Prev == null || (curLink.Prev.Token.Type == ETokenType.token_operator && 
                                    ((TOKEN<Operator>)curLink.Prev.Token).Tag.Type == EOperatorType.LeftParen))
                                {
                                    throw new Exception(string.Format("Error! 右括弧“{0}”附近有语法错误（索引：{1}）", 
                                        ((TOKEN<Operator>)curLink.Token).Tag.Value, curLink.Token.Index.ToString()));
                                }

                                break;

                            default:
                                if (!((curLink.Prev != null && (curLink.Prev.Token.Type == ETokenType.token_operand || 
                                    (curLink.Prev.Token.Type == ETokenType.token_operator && 
                                    ((TOKEN<Operator>)curLink.Prev.Token).Tag.Type == EOperatorType.RightParen))) &&
                                        (curLink.Next != null && (curLink.Next.Token.Type == ETokenType.token_operand ||
                                    (curLink.Next.Token.Type == ETokenType.token_operator &&
                                    (((TOKEN<Operator>)curLink.Next.Token).Tag.Type == EOperatorType.LeftParen || 
                                    ((TOKEN<Operator>)curLink.Next.Token).Tag.Type == EOperatorType.Negative || 
                                    ((TOKEN<Operator>)curLink.Next.Token).Tag.Type == EOperatorType.Positive))))))
                                {
                                    throw new Exception(string.Format("Error! 二元操作符“{0}”附近有语法错误（索引：{1}）", 
                                        ((TOKEN<Operator>)curLink.Token).Tag.Value, curLink.Token.Index.ToString()));
                                }

                                break;
                        }

                        break;

                    case ETokenType.token_separator:
                        throw new Exception(string.Format("Error! 分隔符“{0}”只能用于关键字（索引：{1}）", 
                            ((TOKEN<Separator>)curLink.Token).Tag.Value, curLink.Token.Index.ToString()));

                    default:
                        break;
                }

                if (curLink == endLink)
                {
                    break;
                }
                else
                {
                    curLink = curLink.Next;
                }
            }
        }

        #endregion

        #region 检查操作符与操作数的类型匹配

        /// <summary>
        /// 检查操作符与操作数的类型匹配
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        private IOperand OperatorEvalAnalyze(TOKENLink startLink, TOKENLink endLink)
        {
            //先检查再进行后缀表达式转换
            TokenEnvirAnalyze(startLink, endLink);

            TOKENLink postfixLink = _toolBox.InfixToPostfix(startLink, endLink);
            TOKENLink link_new = null;
            IToken token = null;

            while (postfixLink.Next != null)
            {
                postfixLink = postfixLink.Next;

                if (postfixLink.Token.Type == ETokenType.token_operator)
                {
                    link_new = null;
                    token = null;
                    EOperatorType type = ((TOKEN<Operator>)postfixLink.Token).Tag.Type;
                    switch (type)
                    {
                        case EOperatorType.Positive:  //正
                        case EOperatorType.Negative:  //负
                            IOperand operand = ((TOKEN<IOperand>)postfixLink.Prev.Token).Tag;
                            if (operand.Type == EDataType.Ddouble || operand.Type == EDataType.Dint)
                            {
                                token = postfixLink.Prev.Token;
                            }
                            else
                            {
                                throw new Exception(string.Format("Error! 运算符“{0}”无法应用于“{2}”类型的操作数（索引:{1}）", 
                                    ((TOKEN<Operator>)postfixLink.Token).Tag.Value, postfixLink.Token.Index.ToString(), operand.Type.ToString()));
                            }

                            break;

                        case EOperatorType.Plus:
                        case EOperatorType.Minus:
                        case EOperatorType.Multiply:
                        case EOperatorType.Divide:
                        case EOperatorType.Mod:
                            if ((((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dstring ||
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dstring) && 
                                (type == EOperatorType.Plus || type == EOperatorType.Minus))
                            {
                                token = new TOKEN<IOperand>(ETokenType.token_operand,
                                        new Operand<string>(EDataType.Dstring, ""), postfixLink.Token.Index);

                            }
                            else if ((((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddouble || 
                                ((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dint) &&
                                (((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Ddouble || 
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dint))
                            {
                                if ((((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Ddouble ||
                                    ((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddouble) || 
                                    (type == EOperatorType.Divide || type == EOperatorType.Mod))
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<double>(EDataType.Ddouble, 0), postfixLink.Token.Index);
                                }
                                else
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<int>(EDataType.Dint, 0), postfixLink.Token.Index);
                                }
                            }
                            else
                            {
                                throw new Exception(string.Format("Error! 运算符“{0}”无法应用于“{2}”和“{3}”类型的操作数（索引:{1}）", 
                                    ((TOKEN<Operator>)postfixLink.Token).Tag.Value, postfixLink.Token.Index.ToString(),
                                    ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type.ToString(),
                                    ((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type.ToString()));
                            }

                            break;
                     
                        case EOperatorType.LessThan:
                        case EOperatorType.GreaterThan:
                        case EOperatorType.Equal:
                        case EOperatorType.NotEqual:
                        case EOperatorType.GreaterEqual:
                        case EOperatorType.LessEqual:

                            if (((((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddouble ||
                                ((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dint) &&
                                (((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Ddouble ||
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dint)) ||
                                (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddatetime &&
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Ddatetime) ||
                                (((((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dstring &&
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dstring) ||
                                (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dbool &&
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dbool)) &&
                                (type == EOperatorType.Equal || type == EOperatorType.NotEqual)))
                            {
                                token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<bool>(EDataType.Dbool, true), postfixLink.Token.Index);
                            }
                            else
                            {
                                throw new Exception(string.Format("Error! 运算符“{0}”无法应用于“{2}”和“{3}”类型的操作数（索引:{1}）",
                                    ((TOKEN<Operator>)postfixLink.Token).Tag.Value, postfixLink.Token.Index.ToString(),
                                    ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type.ToString(),
                                    ((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type.ToString()));
                            }

                            break;

                        default:
                            break;
                    }

                    if (token != null)
                    {
                        link_new = new TOKENLink(token);

                        link_new.Next = postfixLink.Next;
                        if (postfixLink.Next != null)
                        {
                            postfixLink.Next.Prev = link_new;
                        }

                        if (((TOKEN<Operator>)postfixLink.Token).Tag.Dimension == 1)
                        {
                            //一元操作符
                            if (postfixLink.Prev.Prev != null)
                            {
                                link_new.Prev = postfixLink.Prev.Prev;
                                postfixLink.Prev.Prev.Next = link_new;
                            }
                        }
                        else if (((TOKEN<Operator>)postfixLink.Token).Tag.Dimension == 2)
                        {
                            //二元操作符
                            if (postfixLink.Prev.Prev.Prev != null)
                            {
                                link_new.Prev = postfixLink.Prev.Prev.Prev;
                                postfixLink.Prev.Prev.Prev.Next = link_new;
                            }
                        }

                        postfixLink = link_new;
                    }

                }//end if


            }//end while

            return ((TOKEN<IOperand>)postfixLink.Token).Tag;
        }

        #endregion

        #region 括弧对称性验证

        private void CheckParen(TOKENLink startLink, TOKENLink endLink)
        {
            TOKENLink curLink = startLink;
            int cout = 0;

            while (true)
            {
                if (curLink.Token.Type == ETokenType.token_operator)
                {
                    if (((TOKEN<Operator>)curLink.Token).Tag.Type == EOperatorType.LeftParen)
                    {
                        cout++;
                    }
                    else if (((TOKEN<Operator>)curLink.Token).Tag.Type == EOperatorType.RightParen)
                    {
                        cout--;
                    }

                    if (cout < 0)
                    {
                        throw new Exception(string.Format("Error! 缺少左括弧（索引：{0}）", curLink.Token.Index.ToString()));
                    }
                }

                if (curLink == endLink)
                {
                    break;
                }

                curLink = curLink.Next;
            }

            if (cout > 0)
            {
                throw new Exception(string.Format("Error! 缺少“{0}”个右括弧", cout.ToString()));
            }
        }

        #endregion
    }
}
