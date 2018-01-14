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
    /// 表达式求值
    /// </summary>
    public class Evaluator
    {
        public Evaluator()
        {
            _gmm = new Grammar(this);
            _toolBox = new ToolBox();
        }

        private Grammar _gmm = null;
        private ToolBox _toolBox = null;

        #region 逻辑表达式求值 - ExpressionEvaluate

        /// <summary>
        /// 逻辑表达式求值
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand ExpressionEvaluate(TOKENLink startLink, TOKENLink endLink)
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
                            result = _gmm.Key_IF(curLink, out endLink_key);
                            break;
                        case EKeyword.AND:
                            result = _gmm.Key_AND(curLink, out endLink_key);
                            break;
                        case EKeyword.OR:
                            result = _gmm.Key_OR(curLink, out endLink_key);
                            break;
                        case EKeyword.NOT:
                            result = _gmm.Key_Not(curLink, out endLink_key);
                            break;
                        case EKeyword.FALSE:
                            result = _gmm.Key_False(curLink, out endLink_key);
                            break;
                        case EKeyword.TRUE:
                            result = _gmm.Key_True(curLink, out endLink_key);
                            break;
                        case EKeyword.Len:
                            result = _gmm.Key_Len(curLink, out endLink_key);
                            break;
                        case EKeyword.NowDate:
                            result = _gmm.Key_NowDate(curLink, out endLink_key);
                            break;
                        case EKeyword.ToDateTime:
                            result = _gmm.Key_ToDateTime(curLink, out endLink_key);
                            break;
                        case EKeyword.ToDouble:
                            result = _gmm.Key_ToDouble(curLink, out endLink_key);
                            break;
                        case EKeyword.ToInt:
                            result = _gmm.Key_ToInt(curLink, out endLink_key);
                            break;
                        case EKeyword.ToString:
                            result = _gmm.Key_ToString(curLink, out endLink_key);
                            break;
                        default:
                            break;
                    }

                    TOKENLink tokenLink = new TOKENLink(new TOKEN<IOperand>(ETokenType.token_operand, result, curLink.Token.Index));

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

                if (curLink == endLink)
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
                return ((TOKEN<IOperand>)curLink.Token).Tag;
            }
            else
            {
                return MathEvaluate(startLink, endLink);
            }
        }

        #endregion

        #region 运算表达式求值 - MathEvaluate

        /// <summary>
        /// 数表达式求值
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        private IOperand MathEvaluate(TOKENLink startLink, TOKENLink endLink)
        {

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
                            if (type == EOperatorType.Negative)
                            {
                                if (operand.Type == EDataType.Dint)
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand,
                                        new Operand<int>(EDataType.Dint, -((Operand<int>)operand).TValue), postfixLink.Token.Index);
                                }
                                else if (operand.Type == EDataType.Ddouble)
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand,
                                        new Operand<double>(EDataType.Ddouble, -((Operand<double>)operand).TValue), postfixLink.Token.Index);
                                }
                            }
                            else
                            {
                                token = postfixLink.Prev.Token;
                            }

                            break;

                        case EOperatorType.Plus:
                        case EOperatorType.Minus:
                            if (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dstring ||
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dstring)
                            {
                                if (type == EOperatorType.Plus)
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand,
                                        new Operand<string>(EDataType.Dstring, ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.ToString() +
                                        ((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.ToString()), postfixLink.Token.Index);
                                }
                                else
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand,
                                        new Operand<string>(EDataType.Dstring,
                                            ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.ToString().Replace(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.ToString(), "")), 
                                            postfixLink.Token.Index);

                                }
                            }
                            else if (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddouble ||
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Ddouble)
                            {
                                if (type == EOperatorType.Plus)
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<double>(EDataType.Ddouble,
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Value) +
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Value)), postfixLink.Token.Index);
                                }
                                else
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<double>(EDataType.Ddouble,
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Value) -
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Value)), postfixLink.Token.Index);
                                }
                            }
                            else
                            {
                                if (type == EOperatorType.Plus)
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<int>(EDataType.Dint,
                                       ((Operand<int>)((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag).TValue +
                                       ((Operand<int>)((TOKEN<IOperand>)postfixLink.Prev.Token).Tag).TValue), postfixLink.Token.Index);
                                }
                                else
                                {
                                    token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<int>(EDataType.Dint,
                                       ((Operand<int>)((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag).TValue -
                                       ((Operand<int>)((TOKEN<IOperand>)postfixLink.Prev.Token).Tag).TValue), postfixLink.Token.Index);
                                }
                            }

                            break;

                        case EOperatorType.Multiply:
                            if (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddouble ||
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Ddouble)
                            {
                                token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<double>(EDataType.Ddouble,
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Value) *
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Value)), postfixLink.Token.Index);
                            }
                            else
                            {
                                token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<int>(EDataType.Dint,
                                       ((Operand<int>)((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag).TValue *
                                       ((Operand<int>)((TOKEN<IOperand>)postfixLink.Prev.Token).Tag).TValue), postfixLink.Token.Index);
                            }

                            break;

                        case EOperatorType.Divide:
                            token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<double>(EDataType.Ddouble,
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Value) /
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Value)), postfixLink.Token.Index);

                            break;

                        case EOperatorType.Mod:
                            token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<double>(EDataType.Ddouble,
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Value) %
                                        Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Value)), postfixLink.Token.Index);

                            break;

                        case EOperatorType.LessThan:
                        case EOperatorType.GreaterThan:
                        case EOperatorType.GreaterEqual:
                        case EOperatorType.LessEqual:
                            bool result = false;

                            if (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddatetime)
                            {
                                DateTime f = ((Operand<DateTime>)((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag).TValue;
                                DateTime s = ((Operand<DateTime>)((TOKEN<IOperand>)postfixLink.Prev.Token).Tag).TValue;

                                switch (type)
                                {
                                    case EOperatorType.LessThan:
                                        result = f < s;
                                        break;
                                    case EOperatorType.GreaterThan:
                                        result = f > s;
                                        break;;
                                    case EOperatorType.GreaterEqual:
                                        result = f >= s;
                                        break;
                                    case EOperatorType.LessEqual:
                                        result = f <= s;
                                        break;
                                }

                            }
                            else
                            {
                                double f = Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Value);
                                double s = Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Value);

                                switch (type)
                                {
                                    case EOperatorType.LessThan:
                                        result = f < s;
                                        break;
                                    case EOperatorType.GreaterThan:
                                        result = f > s;
                                        break; ;
                                    case EOperatorType.GreaterEqual:
                                        result = f >= s;
                                        break;
                                    case EOperatorType.LessEqual:
                                        result = f <= s;
                                        break;
                                }
                            }

                            token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<bool>(EDataType.Dbool, result), postfixLink.Token.Index);

                            break;

                        case EOperatorType.Equal:
                        case EOperatorType.NotEqual:
                            bool r = false;

                            if (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dstring &&
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dstring)
                            {
                                if (type == EOperatorType.Equal)
                                {
                                    r = ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.ToString().Equals(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.ToString());
                                }
                                else
                                {
                                    r = !((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.ToString().Equals(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.ToString());
                                }
                            }
                            else if (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Ddatetime &&
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Ddatetime)
                            {
                                DateTime f = ((Operand<DateTime>)((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag).TValue;
                                DateTime s = ((Operand<DateTime>)((TOKEN<IOperand>)postfixLink.Prev.Token).Tag).TValue;

                                if (type == EOperatorType.Equal)
                                {
                                    r = f == s;
                                }
                                else
                                {
                                    r = f != s;
                                }
                            }
                            else if (((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Type == EDataType.Dbool &&
                                ((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Type == EDataType.Dbool)
                            {
                                bool f = ((Operand<bool>)((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag).TValue;
                                bool s = ((Operand<bool>)((TOKEN<IOperand>)postfixLink.Prev.Token).Tag).TValue;

                                if (type == EOperatorType.Equal)
                                {
                                    r = f == s;
                                }
                                else
                                {
                                    r = f != s;
                                }
                            }
                            else
                            {
                                double f = Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Prev.Token).Tag.Value);
                                double s = Convert.ToDouble(((TOKEN<IOperand>)postfixLink.Prev.Token).Tag.Value);

                                if (type == EOperatorType.Equal)
                                {
                                    r = f == s;
                                }
                                else
                                {
                                    r = f != s;
                                }
                            }

                            token = new TOKEN<IOperand>(ETokenType.token_operand, new Operand<bool>(EDataType.Dbool, r), postfixLink.Token.Index);

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
    }
}
