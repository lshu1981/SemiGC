/*
 
表达式处理结果会返回一个值。
支持操作数：int、double、bool、datetime、string
支持操作符：(、)、+、-、*、/、%、+正、-负、<、>、=、<>、<=、>=
支持关键字：IF、AND、OR、NOT、TRUE、FALSE、ToString、ToDateTime、ToInt、ToDouble、Len、NowDate
关键字说明：
IF：if(JudgeExpression,FirstExpression,SecondExpression)
    JudgeExpression 为 true 返回 FirstExpression 否则 返回 SecondExpression
AND：and(JudgeExpression,JudgeExpression,......)
     所有 JudgeExpression 表达式为 true 返回 true 否则 返回 false
OR：or(JudgeExpression,JudgeExpression,......)
     一个 JudgeExpression 表达式为 true 返回 true 否则 返回 false
Not：not(JudgeExpression)
     JudgeExpression 表达式为 true 返回 fase 否则 返回 true
TRUE：true() 返回 true
FALSE：false() 返回 false
NowDate：nowdate() 返回 当前时间 datetime类型
Len：Len(Expression) 返回值长度 int类型
ToInt：ToInt(Expression) 值转换 返回 int类型
ToDouble：ToDouble(Expression) 值转换 返回 double类型
ToDateTime：ToDateTime(Expression) 值转换 返回 datetime类型
ToString：ToString(Expression)|ToString(DateTime,Formatstring) 值转换 返回 string类型
 
 
 */



using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class Grammar
    {
        public Grammar(Evaluator eval)
        {
            _eval = eval;
        }

        private Evaluator _eval = null;

        #region 关键字运算

        #region IF

        /// <summary>
        /// IF语句语法 if(JudgeExpression,TrueExpression,FalseExpression)
        /// JudgeExpression 为 true 返回 TrueExpression 否则 返回 FalseExpression
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_IF(TOKENLink startLink, out TOKENLink endLink)
        {
            //分离表达式
            IOperand result;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            //执行 JudgeExpression 表达式 - 此处必需表达式有值
            IOperand judgeResult = _eval.ExpressionEvaluate(startLink.Next.Next, commaList[0].Prev);

            if (judgeResult != null && judgeResult.Type == EDataType.Dbool && ((Operand<bool>)judgeResult).TValue == true)
            {
                //执行 TrueExpression 表达式
                result = _eval.ExpressionEvaluate(commaList[0].Next, commaList[1].Prev);
            }
            else
            {
                //执行 FalseExpression 表达式
                result = _eval.ExpressionEvaluate(commaList[1].Next, endLink.Prev);
            }

            return result;
        }

        #endregion

        #region AND

        /// <summary>
        /// AND语句语法 and(JudgeExpression,JudgeExpression,......)
        /// 所有 JudgeExpression 表达式为 true 返回 true 否则 返回 false
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_AND(TOKENLink startLink, out TOKENLink endLink)
        {
            //分离表达式
            IOperand result = null;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            TOKENLink link_s = startLink.Next.Next;

            foreach (TOKENLink comma in commaList)
            {
                result = _eval.ExpressionEvaluate(link_s, comma.Prev);

                if (result != null && result.Type == EDataType.Dbool && ((Operand<bool>)result).TValue == false)
                {
                    break;
                }

                link_s = comma.Next;
            }

            if (result == null || (result != null && result.Type == EDataType.Dbool && ((Operand<bool>)result).TValue == true))
            {
                result = _eval.ExpressionEvaluate(link_s, endLink.Prev);
            }

            return result;
        }

        #endregion

        #region OR

        /// <summary>
        /// OR语句语法 or(JudgeExpression,JudgeExpression,......)
        /// 所有 JudgeExpression 表达式为 fase 返回 fase 否则 返回 true
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_OR(TOKENLink startLink, out TOKENLink endLink)
        {
            //分离表达式
            IOperand result = null;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            TOKENLink link_s = startLink.Next.Next;

            foreach (TOKENLink comma in commaList)
            {
                result = _eval.ExpressionEvaluate(link_s, comma.Prev);

                if (result != null && result.Type == EDataType.Dbool && ((Operand<bool>)result).TValue == true)
                {
                    break;
                }

                link_s = comma.Next;
            }

            if (result != null && result.Type == EDataType.Dbool && ((Operand<bool>)result).TValue == false)
            {
                result = _eval.ExpressionEvaluate(link_s, endLink.Prev);
            }

            return result;
        }

        #endregion

        #region Not

        /// <summary>
        /// Not语句语法 not(JudgeExpression)
        /// JudgeExpression 表达式为 true 返回 fase 否则 返回 true
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_Not(TOKENLink startLink, out TOKENLink endLink)
        {
            //分离表达式
            IOperand result;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            result = _eval.ExpressionEvaluate(startLink.Next.Next, endLink.Prev);

            if (result != null && result.Type == EDataType.Dbool && ((Operand<bool>)result).TValue == true)
            {
                result = new Operand<bool>(EDataType.Dbool, false);
            }
            else
            {
                result = new Operand<bool>(EDataType.Dbool, true);
            }

            return result;
        }

        #endregion

        #region LEN

        /// <summary>
        /// LEN语句语法 Len(Expression)
        /// Expression 返回值 长度
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_Len(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            result = _eval.ExpressionEvaluate(startLink.Next.Next, endLink.Prev);

            result = new Operand<int>(EDataType.Dint, result.ToString().Length);

            return result;
        }

        #endregion

        #region NowDate

        /// <summary>
        /// NowDate语句语法 NowDate()
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_NowDate(TOKENLink startLink, out TOKENLink endLink)
        {
            endLink = startLink.Next.Next;

            return new Operand<DateTime>(EDataType.Ddatetime, DateTime.Now);
        }

        #endregion

        #region ToString

        /// <summary>
        /// ToString语句语法 ToString(Expression|Operand,FormatString) 
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_ToString(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            if (commaList.Count == 0)
            {
                result = _eval.ExpressionEvaluate(startLink.Next.Next, endLink.Prev);
                return new Operand<string>(EDataType.Dstring, result.ToString());
            }
            else
            {
                result = _eval.ExpressionEvaluate(startLink.Next.Next, commaList[0].Prev);
                IOperand format = _eval.ExpressionEvaluate(commaList[0].Next, endLink.Prev);
                return new Operand<string>(EDataType.Dstring, ((Operand<DateTime>)result).TValue.ToString(format.ToString()));
            }
        }

        #endregion

        #region ToInt

        /// <summary>
        /// ToInt语句语法 ToInt(Expression|Operand) 
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_ToInt(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            result = _eval.ExpressionEvaluate(startLink.Next.Next, endLink.Prev);

            return new Operand<int>(EDataType.Dint, Convert.ToInt32(result.Value));
        }

        #endregion

        #region ToDouble

        /// <summary>
        /// ToDouble语句语法 ToDouble(Expression|Operand) 
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_ToDouble(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            result = _eval.ExpressionEvaluate(startLink.Next.Next, endLink.Prev);

            return new Operand<double>(EDataType.Ddouble, Convert.ToDouble(result.Value));
        }

        #endregion

        #region ToDateTime

        /// <summary>
        /// ToDateTime语句语法 ToDateTime(Expression|Operand) 
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_ToDateTime(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_Analyze(startLink.Next, out endLink, out commaList);

            result = _eval.ExpressionEvaluate(startLink.Next.Next, endLink.Prev);

            return new Operand<DateTime>(EDataType.Ddatetime, Convert.ToDateTime(result.Value));
        }

        #endregion

        #region True

        /// <summary>
        /// True语句语法 true()
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_True(TOKENLink startLink, out TOKENLink endLink)
        {
            endLink = startLink.Next.Next;

            return new Operand<bool>(EDataType.Dbool, true);
        }

        #endregion

        #region False

        /// <summary>
        /// False语句语法 false()
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_False(TOKENLink startLink, out TOKENLink endLink)
        {
            endLink = startLink.Next.Next;

            return new Operand<bool>(EDataType.Dbool, false);
        }

        #endregion

        #region 关键字分解 - Key_Analyze

        /// <summary>
        /// 分解关键字 - 包含括弧表达式
        /// </summary>
        /// <param name="startLink">开始括弧</param>
        /// <param name="endLink">结束括弧</param>
        /// <param name="commaList">表达式分隔符列表</param>
        public void Key_Analyze(TOKENLink startLink, out TOKENLink endLink, out List<TOKENLink> commaList)
        {
            int i = 1;
            TOKENLink curLink = startLink;
            commaList = new List<TOKENLink>();

            do
            {
                curLink = curLink.Next;

                if (curLink.Token.Type == ETokenType.token_operator)
                {
                    if (((TOKEN<Operator>)curLink.Token).Tag.Type == EOperatorType.LeftParen)
                    {
                        i++;
                    }
                    else if (((TOKEN<Operator>)curLink.Token).Tag.Type == EOperatorType.RightParen)
                    {
                        i--;
                    }
                }
                else if (i == 1 && curLink.Token.Type == ETokenType.token_separator)
                {
                    commaList.Add(curLink);
                }
            }
            while (i != 0);

            endLink = curLink;
        }

        #endregion

        #endregion
    }
}
