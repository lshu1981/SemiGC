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
using System.Collections;

namespace ExpressionParser
{
    public class ExpressionParse
    {
        public ExpressionParse()
            : this("")
        { }

        public ExpressionParse(string expression)
        {
            _expression = expression;
        }

        private string _expression = string.Empty;
        private Link_OP _link_OP = null;
        private Evaluator _eval = new Evaluator();

        #region 获取分词

        /// <summary>
        /// 获取分词
        /// </summary>
        public List<string> Words
        {
            get
            {
                Analyze();

                TOKENLink link = _link_OP.Head;
                List<string> wordList = new List<string>();
                while (link != null)
                {
                    switch (link.Token.Type)
                    {
                        case ETokenType.token_keyword:
                            wordList.Add(((TOKEN<KeyWord>)link.Token).Tag.Value);
                            break;
                        case ETokenType.token_operand:
                            wordList.Add(((TOKEN<IOperand>)link.Token).Tag.ToString());
                            break;
                        case ETokenType.token_operator:
                            Operator op = ((TOKEN<Operator>)link.Token).Tag;

                            if (op.Value == "+" || op.Value == "-")
                            {
                                wordList.Add(op.Value + "  " + op.Type.ToString());
                            }
                            else
                            {
                                wordList.Add(op.Value);
                            }
                            break;
                        case ETokenType.token_separator:
                            wordList.Add(((TOKEN<Separator>)link.Token).Tag.Value.ToString());
                            break;
                        default:
                            break;
                    }

                    link = link.Next;

                }

                return wordList;
            }
        }

        #endregion

        #region 表达式

        /// <summary>
        /// 获取或赋值表达式
        /// </summary>
        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value.Trim();
                _link_OP = null;
            }
        }

        #endregion

        /// <summary>
        /// 检查语法
        /// </summary>
        /// <returns></returns>
        public bool Check(ref string mes)
        {
            bool result = false;
            try
            {
                Analyze();
                SyntaxAnalyzer a = new SyntaxAnalyzer();
                EDataType type = a.Execute(_link_OP.Head, _link_OP.Tail);
                mes = "Success!";
                result = true;
            }
            catch (Exception ex)
            {
                mes = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 表达式式执行
        /// </summary>
        public IOperand Execute()
        {
            Analyze();
            return _eval.ExpressionEvaluate(_link_OP.Head, _link_OP.Tail);
        }

        /// <summary>
        /// 表达式式执行-返回字符串结果
        /// </summary>
        public string ExecuteToString()
        {
            try
            {
                return Execute().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        private void Analyze()
        {
            if (_link_OP == null)
            {
                if (_expression.Trim().Length > 0)
                {
                    PhraseAnalyzer analyze = new PhraseAnalyzer(_expression);
                    _link_OP = analyze.Analyze();
                }
                else
                {
                    throw new Exception("Error! 表达式为空");
                }
            }
        }

    }
}
