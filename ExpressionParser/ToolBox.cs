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
    public class ToolBox
    {
        /// <summary>
        /// 中缀表达式转后缀表达式
        /// -1*6+5*(2+3) 转换成 1 - 6 * 5 2 3 + * +
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public TOKENLink InfixToPostfix(TOKENLink startLink, TOKENLink endLink)
        {
            //进入此函数的链表 - 只含操作符和操作数
            TOKENLink postfixLinkHead = null;
            TOKENLink postfixLinkTail = null;
            TOKENLink tempLink = null;
            TOKENLink curLink = startLink;
            KeyValueList<IToken, int> tokenList = new KeyValueList<IToken, int>();
            int Deep_PRI = 0;  //括弧深度优先级

            try
            {
                while (true)
                {
                    if (curLink.Token.Type == ETokenType.token_operand)
                    {
                        //操作数 直接放入后缀链表
                        TOKENLink link = new TOKENLink(curLink.Token);

                        if (postfixLinkHead == null)
                        {
                            postfixLinkHead = link;
                            postfixLinkTail = link;
                        }
                        else
                        {
                            postfixLinkTail.Next = link;
                            link.Prev = postfixLinkTail;
                            postfixLinkTail = link;
                        }
                    }
                    else if (curLink.Token.Type == ETokenType.token_operator)
                    {
                        if (((TOKEN<Operator>)curLink.Token).Tag.Type == EOperatorType.LeftParen)
                        {
                            Deep_PRI++;
                        }
                        else if (((TOKEN<Operator>)curLink.Token).Tag.Type == EOperatorType.RightParen)
                        {
                            Deep_PRI--;
                        }
                        else
                        {
                            //将操作符放入临时链表
                            TOKENLink link_new = new TOKENLink(curLink.Token);
                            tokenList.Add(link_new.Token, Deep_PRI);

                            if (tempLink == null)
                            {
                                tempLink = link_new;
                            }
                            else
                            {
                                tempLink.Next = link_new;
                                link_new.Prev = tempLink;
                                tempLink = link_new;
                            }

                            //判断需要放入后缀链表的项
                            while (tempLink.Prev != null)
                            {
                                if ((tokenList[tempLink.Prev.Token] > tokenList[tempLink.Token]) ||
                                  ((tokenList[tempLink.Prev.Token] == tokenList[tempLink.Token]) &&
                                  (((TOKEN<Operator>)tempLink.Prev.Token).Tag.PRI >= ((TOKEN<Operator>)tempLink.Token).Tag.PRI)))
                                {
                                    TOKENLink link_Operator = tempLink.Prev;

                                    if (tempLink.Prev.Prev != null)
                                    {
                                        tempLink.Prev.Prev.Next = tempLink;
                                        tempLink.Prev = tempLink.Prev.Prev;
                                    }
                                    else
                                    {
                                        tempLink.Prev = null;
                                    }

                                    postfixLinkTail.Next = link_Operator;
                                    link_Operator.Prev = postfixLinkTail;
                                    postfixLinkTail = link_Operator;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }// end if
                    }
                    else
                    {
                        throw new Exception(string.Format("Error! 后缀表达式出现未解析类型“{0}”（索引：{1}）", curLink.Token.Type.ToString(), curLink.Token.Index.ToString()));
                    }

                    if (curLink == endLink)
                    {
                        break;
                    }

                    curLink = curLink.Next;

                }// end while

                TOKENLink link_p = tempLink;
                while (link_p != null)
                {
                    tempLink = tempLink.Prev;

                    postfixLinkTail.Next = link_p;
                    link_p.Prev = postfixLinkTail;
                    postfixLinkTail = link_p;

                    link_p = tempLink;
                }

                postfixLinkHead.Prev = null;
                postfixLinkTail.Next = null;

                return postfixLinkHead;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
