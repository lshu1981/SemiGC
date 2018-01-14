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
    public class Define
    {
        /// <summary>
        /// 操作符集
        /// </summary>
        public static Dictionary<EOperatorType, Operator> Operators = new Dictionary<EOperatorType, Operator>() { 
        {EOperatorType.LeftParen,new Operator(0, 99, EOperatorType.LeftParen, "(") },       // 左括号   
        {EOperatorType.RightParen,new Operator(0, 99, EOperatorType.RightParen, ")") },    // 右括号   
        {EOperatorType.Plus,new Operator(2, 44, EOperatorType.Plus, "+") },                // 加   
        {EOperatorType.Minus,new Operator(2, 44, EOperatorType.Minus, "-") },              // 减   
        {EOperatorType.Multiply,new Operator(2, 55, EOperatorType.Multiply, "*") },        // 乘   
        {EOperatorType.Divide,new Operator(2, 55, EOperatorType.Divide, "/") },            // 除   
        {EOperatorType.Mod,new Operator(2, 55, EOperatorType.Mod, "%") },                  // 模    
        {EOperatorType.Positive,new Operator(1, 77, EOperatorType.Positive, "+") },        // 正号   
        {EOperatorType.Negative,new Operator(1, 77, EOperatorType.Negative, "-") },        // 负号    
        // 关系运算
        {EOperatorType.LessThan,new Operator(2, 33, EOperatorType.LessThan, "<") },            // 小于   
        {EOperatorType.GreaterThan,new Operator(2, 33, EOperatorType.GreaterThan, ">") },      // 大于   
        {EOperatorType.Equal,new Operator(2, 22, EOperatorType.Equal, "=") },                    // 等于   
        {EOperatorType.NotEqual,new Operator(2, 22, EOperatorType.NotEqual, "<>") },             // 不等于   
        {EOperatorType.LessEqual,new Operator(2, 33, EOperatorType.LessEqual, "<=") },         // 不大于   
        {EOperatorType.GreaterEqual,new Operator(2, 33, EOperatorType.GreaterEqual, ">=") }    // 不小于     
        };

        /// <summary>
        /// 关键字集
        /// </summary>
        public static Dictionary<EKeyword, KeyWord> KeyWords = new Dictionary<EKeyword, KeyWord>() { 
        {EKeyword.IF,new KeyWord(EKeyword.IF,EDataType.Dunknown,"if")},
        {EKeyword.AND,new KeyWord(EKeyword.AND,EDataType.Dbool,"and")},
        {EKeyword.OR,new KeyWord(EKeyword.OR,EDataType.Dbool,"or")},
        {EKeyword.TRUE,new KeyWord(EKeyword.TRUE,EDataType.Dbool,"true")},
        {EKeyword.FALSE,new KeyWord(EKeyword.FALSE,EDataType.Dbool,"false")},
        {EKeyword.NOT,new KeyWord(EKeyword.NOT,EDataType.Dbool,"not")},

        {EKeyword.Len,new KeyWord(EKeyword.Len,EDataType.Dint,"len")},
        {EKeyword.NowDate,new KeyWord(EKeyword.NowDate,EDataType.Ddatetime,"nowdate")},
        {EKeyword.ToInt,new KeyWord(EKeyword.ToInt,EDataType.Dint,"toint")},
        {EKeyword.ToDouble,new KeyWord(EKeyword.ToDouble,EDataType.Ddouble,"todouble")},
        {EKeyword.ToDateTime,new KeyWord(EKeyword.ToDateTime,EDataType.Ddatetime,"todatetime")},
        {EKeyword.ToString,new KeyWord(EKeyword.ToString,EDataType.Dstring,"tostring")}
        };
    }

    /// <summary>
    /// 未知类型
    /// </summary>
    public struct Unknown
    { }

    /// <summary>
    /// 关键字
    /// </summary>
    public struct KeyWord
    {
        public KeyWord(EKeyword type, EDataType returnType, string value)
        {
            Type = type;
            ReturnType = returnType;
            Value = value;
        }

        public EKeyword Type;         //关键字类型
        public EDataType ReturnType;  //返回数据类型
        public string Value;          //关键字
    }

    /// <summary>
    /// 分隔符
    /// </summary>
    public struct Separator
    {
        public Separator(char value)
        {
            Value = value;
        }

        public char Value;
    }

    /// <summary>
    /// 操作数接口
    /// </summary>
    public interface IOperand
    {
        EDataType Type { get; }
        Object Value { get; }
    }

    /// <summary>
    /// 操作数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Operand<T> : IOperand
    {
        public Operand(EDataType type, T value)
        {
            _type = type;
            _value = value;
        }

        private EDataType _type;
        private T _value;

        /// <summary>
        /// 获取操作数值
        /// </summary>
        public T TValue
        {
            get { return _value; }
        }

        #region IOperand 成员

        /// <summary>
        /// 获取操作数类型
        /// </summary>
        public EDataType Type
        {
            get
            {
                return _type;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        #endregion

        public override string ToString()
        {
            return _value.ToString();
        }
    }

    /// <summary>
    /// 操作符
    /// </summary>
    public struct Operator
    {
        public Operator(int dimension, int pri, EOperatorType type, string value)
        {
            Dimension = dimension;
            PRI = pri;
            Type = type;
            Value = value;
        }

        public int Dimension;		  // 几元操作符
        public int PRI;		          // 优先级
        public EOperatorType Type;    // 操作符类型
        public string Value;          // 操作符
    }

    /// <summary>
    /// 标记接口
    /// </summary>
    public interface IToken
    {
        ETokenType Type { get; }
        int Index { get; }
    }

    /// <summary>
    /// 标记
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TOKEN<T> : IToken
    {
        public TOKEN(ETokenType type, T tag, int index)
        {
            _type = type;
            _tag = tag;
            _index = index;
        }

        private ETokenType _type;
        private T _tag;
        private int _index = 0;

        /// <summary>
        /// 获取或设置标记值
        /// </summary>
        public T Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        #region IToken 成员

        public ETokenType Type
        {
            get
            {
                return _type;
            }
        }

        public int Index
        {
            get { return _index; }
        }

        #endregion
    }

    /// <summary>
    /// 链表操作
    /// </summary>
    public class Link_OP
    {
        //用于记录头尾指针
        public TOKENLink Head;
        public TOKENLink Tail;

        public void Add(TOKENLink token)
        {
            if (token != null)
            {
                if (Tail != null)
                {
                    Tail.Next = token;
                    token.Prev = Tail;
                    Tail = token;
                }
                else
                {
                    Head = token;
                    Tail = token;
                }
            }
        }
    }

    /// <summary>
    /// 标记链表
    /// </summary>
    public class TOKENLink
    {
        public TOKENLink(IToken token)
        {
            _token = token;
        }

        private IToken _token;
        private TOKENLink _prev;
        private TOKENLink _next;

        public IToken Token
        {
            get { return _token; }
        }

        public TOKENLink Prev
        {
            get { return _prev; }
            set
            {
                if (value != this)
                {
                    _prev = value;
                }
            }
        }

        public TOKENLink Next
        {
            get { return _next; }
            set
            {
                if (value != this)
                {
                    _next = value;
                }
            }
        }
    }

    public class KeyValueList<K, T>
    {
        private Dictionary<K, T> list = new Dictionary<K, T>();

        public void Add(K key, T value)
        {
            list.Add(key, value);
        }

        public T this[K key]
        {
            get
            {
                T value;
                list.TryGetValue(key, out value);
                return value;
            }
        }
    }
}
