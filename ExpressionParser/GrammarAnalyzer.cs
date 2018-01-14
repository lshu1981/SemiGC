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
    public class GrammarAnalyzer
    {
        public GrammarAnalyzer(SyntaxAnalyzer analyze)
        {
            _analyze = analyze;
        }

        private SyntaxAnalyzer _analyze = null;

        #region 关键字运算

        #region IF

        /// <summary>
        /// IF语句语法 if(JudgeExpression,TrueExpression,FalseExpression)
        /// JudgeExpression 为 true 返回 TrueExpression 否则 返回 FalseExpression
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_IF_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            //分离表达式
            List<TOKENLink> commaList;

            Key_analyze(startLink, out endLink, out commaList);

            if (commaList.Count < 2)
            {
                throw new Exception(string.Format("Error! 关键字“if”（索引:{0}）缺少表达式", startLink.Token.Index.ToString()));
            }
            else if (commaList.Count > 2)
            {
                throw new Exception(string.Format("Error! 关键字“if”（索引:{0}）表达式过多", startLink.Token.Index.ToString()));
            }

            //执行 JudgeExpression 表达式 - 此处必需表达式有值
            IOperand judgeResult = _analyze.Analyze(startLink.Next.Next, commaList[0].Prev);

            if (judgeResult.Type != EDataType.Dbool)
            {
                throw new Exception(string.Format("Error! 关键字“if”（索引:{0}）的逻辑表达式无法转换为“bool”", startLink.Token.Index.ToString()));
            }

            _analyze.Analyze(commaList[0].Next, commaList[1].Prev);
            _analyze.Analyze(commaList[1].Next, endLink.Prev);

            return new Operand<Unknown>(EDataType.Dunknown,new Unknown());
        }

        #endregion

        #region AND,OR

        /// <summary>
        /// AND,OR语句语法 and(JudgeExpression,JudgeExpression,......)
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_ANDOR_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            //分离表达式
            IOperand result = null;
            List<TOKENLink> commaList;
            int count = 1;

            Key_analyze(startLink, out endLink, out commaList);


            TOKENLink link_s = startLink.Next.Next;

            foreach (TOKENLink comma in commaList)
            {
                result = _analyze.Analyze(link_s, comma.Prev);

                if (result.Type != EDataType.Dbool)
                {
                    throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）的第{2}逻辑表达式无法转换为“bool”", 
                        ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString(), count.ToString()));
                }

                link_s = comma.Next;
                count++;
            }

            result = _analyze.Analyze(link_s, endLink.Prev);
            if (result.Type != EDataType.Dbool)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）的第{2}逻辑表达式无法转换为“bool”", 
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString(), count.ToString()));
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
        public IOperand Key_Not_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            //分离表达式
            IOperand result;
            List<TOKENLink> commaList;

            Key_analyze(startLink, out endLink, out commaList);

            if (commaList.Count > 0)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）表达式过多",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            result = _analyze.Analyze(startLink.Next.Next, endLink.Prev);

            if (result.Type != EDataType.Dbool)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）的逻辑表达式无法转换为“bool”",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
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
        public IOperand Key_Len_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_analyze(startLink, out endLink, out commaList);

            if (commaList.Count > 0)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）表达式过多",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            result = _analyze.Analyze(startLink.Next.Next, endLink.Prev);

            return new Operand<int>(EDataType.Dint, 0);
        }

        #endregion

        #region NowDate

        /// <summary>
        /// NowDate语句语法 NowDate()
        /// </summary>
        /// <param name="startLink"></param>
        /// <param name="endLink"></param>
        /// <returns></returns>
        public IOperand Key_NowDate_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            if (!(startLink.Next != null &&
                startLink.Next.Token.Type == ETokenType.token_operator &&
                ((TOKEN<Operator>)startLink.Next.Token).Tag.Type == EOperatorType.LeftParen &&
                startLink.Next.Next != null &&
                startLink.Next.Next.Token.Type == ETokenType.token_operator &&
                ((TOKEN<Operator>)startLink.Next.Next.Token).Tag.Type == EOperatorType.RightParen))
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）附近有语法错误",
                                   ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

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
        public IOperand Key_ToString_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_analyze(startLink, out endLink, out commaList);

            if (commaList.Count == 0)
            {
                _analyze.Analyze(startLink.Next.Next, endLink.Prev);
            }
            else if (commaList.Count == 1)
            {
                result = _analyze.Analyze(startLink.Next.Next, commaList[0].Prev);

                if (result.Type != EDataType.Ddatetime)
                {
                    throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}） 第一表达式无法转换为“datatime”",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
                }

                IOperand format = _analyze.Analyze(commaList[0].Next, endLink.Prev);

                try
                {
                    DateTime.Now.ToString(format.ToString());
                }
                catch
                {
                    throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}） 日期转换格式有误“{2}”",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString(), format.ToString()));
                }

                return new Operand<string>(EDataType.Dstring, ((Operand<DateTime>)result).TValue.ToString(((TOKEN<IOperand>)commaList[0].Next.Token).Tag.ToString()));
            }
            else
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）表达式过多",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            return new Operand<string>(EDataType.Dstring, "");
        }

        #endregion

        #region ToInt

        /// <summary>
        /// ToInt语句语法 ToInt(Expression|Operand) 
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_ToInt_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_analyze(startLink, out endLink, out commaList);

            if (commaList.Count > 0)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）表达式过多",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            result = _analyze.Analyze(startLink.Next.Next, endLink.Prev);

            try
            {
                Convert.ToInt32(result.Value);
            }
            catch
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）无法将表达式转换为“int”",
                                   ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            return new Operand<int>(EDataType.Dint, 0);
        }

        #endregion

        #region ToDouble

        /// <summary>
        /// ToDouble语句语法 ToDouble(Expression|Operand) 
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_ToDouble_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_analyze(startLink, out endLink, out commaList);

            if (commaList.Count > 0)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）表达式过多",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            result = _analyze.Analyze(startLink.Next.Next, endLink.Prev);

            try
            {
                Convert.ToDouble(result.Value);
            }
            catch
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）无法将表达式转换为“double”",
                                   ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            return new Operand<double>(EDataType.Ddouble, 0);
        }

        #endregion

        #region ToDateTime

        /// <summary>
        /// ToDateTime语句语法 ToDateTime(Expression|Operand) 
        /// </summary>
        /// <param name="startLink">语句开始标记</param>
        /// <param name="endLink">语句结束标记</param>
        /// <returns></returns>
        public IOperand Key_ToDateTime_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            IOperand result;
            List<TOKENLink> commaList;

            Key_analyze(startLink, out endLink, out commaList);

            if (commaList.Count > 0)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）表达式过多",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            result = _analyze.Analyze(startLink.Next.Next, endLink.Prev);

            try
            {
                Convert.ToDateTime(result.Value);
            }
            catch
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）无法将表达式转换为“datetime”",
                                   ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

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
        public IOperand Key_True_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            if (!(startLink.Next != null &&
               startLink.Next.Token.Type == ETokenType.token_operator &&
               ((TOKEN<Operator>)startLink.Next.Token).Tag.Type == EOperatorType.LeftParen &&
               startLink.Next.Next != null &&
               startLink.Next.Next.Token.Type == ETokenType.token_operator &&
               ((TOKEN<Operator>)startLink.Next.Next.Token).Tag.Type == EOperatorType.RightParen))
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）附近有语法错误",
                                   ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

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
        public IOperand Key_False_Analyze(TOKENLink startLink, out TOKENLink endLink)
        {
            if (!(startLink.Next != null &&
               startLink.Next.Token.Type == ETokenType.token_operator &&
               ((TOKEN<Operator>)startLink.Next.Token).Tag.Type == EOperatorType.LeftParen &&
               startLink.Next.Next != null &&
               startLink.Next.Next.Token.Type == ETokenType.token_operator &&
               ((TOKEN<Operator>)startLink.Next.Next.Token).Tag.Type == EOperatorType.RightParen))
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）附近有语法错误",
                                   ((TOKEN<KeyWord>)startLink.Token).Tag.Value, startLink.Token.Index.ToString()));
            }

            endLink = startLink.Next.Next;

            return new Operand<bool>(EDataType.Dbool, false);
        }

        #endregion

        #region 关键字分解 - Key_analyze

        /// <summary>
        /// 分解关键字 - 处理拥有逻辑表达式的关键字
        /// </summary>
        /// <param name="startLink">关键字</param>
        /// <param name="endLink">结束括弧</param>
        /// <param name="commaList">表达式分隔符列表</param>
        public void Key_analyze(TOKENLink startLink, out TOKENLink endLink, out List<TOKENLink> commaList)
        {
            if (startLink.Next == null ||
                !(startLink.Next.Token.Type == ETokenType.token_operator &&
                ((TOKEN<Operator>)startLink.Next.Token).Tag.Type == EOperatorType.LeftParen))
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）附近有语法错误",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value,
                    startLink.Token.Index.ToString()));
            }

            int i = 1;
            TOKENLink curLink = startLink.Next;
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
                    if (curLink.Prev.Token.Type == ETokenType.token_operand ||
                        (curLink.Prev.Token.Type == ETokenType.token_operator &&
                        ((TOKEN<Operator>)curLink.Prev.Token).Tag.Type == EOperatorType.RightParen))
                    {
                        commaList.Add(curLink);
                    }
                    else
                    {
                        throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）的分隔符“,”有语法错误",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value,
                    startLink.Token.Index.ToString()));
                    }
                }
            }
            while (i != 0);

            endLink = curLink;

            if (endLink.Prev.Token.Type == ETokenType.token_separator)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）的分隔符“,”有语法错误",
                    ((TOKEN<KeyWord>)startLink.Token).Tag.Value,
                    startLink.Token.Index.ToString()));
            }

            if (startLink.Next.Next == endLink)
            {
                throw new Exception(string.Format("Error! 关键字“{0}”（索引:{1}）缺少表达式",
                                   ((TOKEN<KeyWord>)startLink.Token).Tag.Value,
                                   startLink.Token.Index.ToString()));
            }
        }

        #endregion

        #endregion
    }
}
