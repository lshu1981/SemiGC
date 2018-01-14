using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.ComponentModel;

namespace CABC
{
    public class CABCSTR
    {
        #region 编码定义

        private static readonly Dictionary<int, string> CodeCollections = new Dictionary<int, string> {  
 { -20319, "a" }, { -20317, "ai" }, { -20304, "an" }, { -20295, "ang" }, { -20292, "ao" }, { -20283, "ba" }, { -20265, "bai" },   
{ -20257, "ban" }, { -20242, "bang" }, { -20230, "bao" }, { -20051, "bei" }, { -20036, "ben" }, { -20032, "beng" }, { -20026, "bi" }  
, { -20002, "bian" }, { -19990, "biao" }, { -19986, "bie" }, { -19982, "bin" }, { -19976, "bing" }, { -19805, "bo" },   
{ -19784, "bu" }, { -19775, "ca" }, { -19774, "cai" }, { -19763, "can" }, { -19756, "cang" }, { -19751, "cao" }, { -19746, "ce" },  
 { -19741, "ceng" }, { -19739, "cha" }, { -19728, "chai" }, { -19725, "chan" }, { -19715, "chang" }, { -19540, "chao" },   
{ -19531, "che" }, { -19525, "chen" }, { -19515, "cheng" }, { -19500, "chi" }, { -19484, "chong" }, { -19479, "chou" },   
{ -19467, "chu" }, { -19289, "chuai" }, { -19288, "chuan" }, { -19281, "chuang" }, { -19275, "chui" }, { -19270, "chun" },  
 { -19263, "chuo" }, { -19261, "ci" }, { -19249, "cong" }, { -19243, "cou" }, { -19242, "cu" }, { -19238, "cuan" },   
{ -19235, "cui" }, { -19227, "cun" }, { -19224, "cuo" }, { -19218, "da" }, { -19212, "dai" }, { -19038, "dan" }, { -19023, "dang" },  
 { -19018, "dao" }, { -19006, "de" }, { -19003, "deng" }, { -18996, "di" }, { -18977, "dian" }, { -18961, "diao" }, { -18952, "die" }  
, { -18783, "ding" }, { -18774, "diu" }, { -18773, "dong" }, { -18763, "dou" }, { -18756, "du" }, { -18741, "duan" },   
{ -18735, "dui" }, { -18731, "dun" }, { -18722, "duo" }, { -18710, "e" }, { -18697, "en" }, { -18696, "er" }, { -18526, "fa" },  
 { -18518, "fan" }, { -18501, "fang" }, { -18490, "fei" }, { -18478, "fen" }, { -18463, "feng" }, { -18448, "fo" }, { -18447, "fou" }  
, { -18446, "fu" }, { -18239, "ga" }, { -18237, "gai" }, { -18231, "gan" }, { -18220, "gang" }, { -18211, "gao" }, { -18201, "ge" },  
 { -18184, "gei" }, { -18183, "gen" }, { -18181, "geng" }, { -18012, "gong" }, { -17997, "gou" }, { -17988, "gu" }, { -17970, "gua" }  
, { -17964, "guai" }, { -17961, "guan" }, { -17950, "guang" }, { -17947, "gui" }, { -17931, "gun" }, { -17928, "guo" },  
{ -17922, "ha" }, { -17759, "hai" }, { -17752, "han" }, { -17733, "hang" }, { -17730, "hao" }, { -17721, "he" }, { -17703, "hei" },  
 { -17701, "hen" }, { -17697, "heng" }, { -17692, "hong" }, { -17683, "hou" }, { -17676, "hu" }, { -17496, "hua" },   
{ -17487, "huai" }, { -17482, "huan" }, { -17468, "huang" }, { -17454, "hui" }, { -17433, "hun" }, { -17427, "huo" },   
{ -17417, "ji" }, { -17202, "jia" }, { -17185, "jian" }, { -16983, "jiang" }, { -16970, "jiao" }, { -16942, "jie" },   
{ -16915, "jin" }, { -16733, "jing" }, { -16708, "jiong" }, { -16706, "jiu" }, { -16689, "ju" }, { -16664, "juan" },   
{ -16657, "jue" }, { -16647, "jun" }, { -16474, "ka" }, { -16470, "kai" }, { -16465, "kan" }, { -16459, "kang" }, { -16452, "kao" },  
 { -16448, "ke" }, { -16433, "ken" }, { -16429, "keng" }, { -16427, "kong" }, { -16423, "kou" }, { -16419, "ku" }, { -16412, "kua" }  
, { -16407, "kuai" }, { -16403, "kuan" }, { -16401, "kuang" }, { -16393, "kui" }, { -16220, "kun" }, { -16216, "kuo" },   
{ -16212, "la" }, { -16205, "lai" }, { -16202, "lan" }, { -16187, "lang" }, { -16180, "lao" }, { -16171, "le" }, { -16169, "lei" },   
{ -16158, "leng" }, { -16155, "li" }, { -15959, "lia" }, { -15958, "lian" }, { -15944, "liang" }, { -15933, "liao" },   
{ -15920, "lie" }, { -15915, "lin" }, { -15903, "ling" }, { -15889, "liu" }, { -15878, "long" }, { -15707, "lou" }, { -15701, "lu" },  
 { -15681, "lv" }, { -15667, "luan" }, { -15661, "lue" }, { -15659, "lun" }, { -15652, "luo" }, { -15640, "ma" }, { -15631, "mai" },  
 { -15625, "man" }, { -15454, "mang" }, { -15448, "mao" }, { -15436, "me" }, { -15435, "mei" }, { -15419, "men" },   
{ -15416, "meng" }, { -15408, "mi" }, { -15394, "mian" }, { -15385, "miao" }, { -15377, "mie" }, { -15375, "min" },   
{ -15369, "ming" }, { -15363, "miu" }, { -15362, "mo" }, { -15183, "mou" }, { -15180, "mu" }, { -15165, "na" }, { -15158, "nai" },   
{ -15153, "nan" }, { -15150, "nang" }, { -15149, "nao" }, { -15144, "ne" }, { -15143, "nei" }, { -15141, "nen" }, { -15140, "neng" }  
, { -15139, "ni" }, { -15128, "nian" }, { -15121, "niang" }, { -15119, "niao" }, { -15117, "nie" }, { -15110, "nin" },   
{ -15109, "ning" }, { -14941, "niu" }, { -14937, "nong" }, { -14933, "nu" }, { -14930, "nv" }, { -14929, "nuan" }, { -14928, "nue" }  
, { -14926, "nuo" }, { -14922, "o" }, { -14921, "ou" }, { -14914, "pa" }, { -14908, "pai" }, { -14902, "pan" }, { -14894, "pang" },  
 { -14889, "pao" }, { -14882, "pei" }, { -14873, "pen" }, { -14871, "peng" }, { -14857, "pi" }, { -14678, "pian" },   
{ -14674, "piao" }, { -14670, "pie" }, { -14668, "pin" }, { -14663, "ping" }, { -14654, "po" }, { -14645, "pu" }, { -14630, "qi" },  
 { -14594, "qia" }, { -14429, "qian" }, { -14407, "qiang" }, { -14399, "qiao" }, { -14384, "qie" }, { -14379, "qin" },  
 { -14368, "qing" }, { -14355, "qiong" }, { -14353, "qiu" }, { -14345, "qu" }, { -14170, "quan" }, { -14159, "que" },   
{ -14151, "qun" }, { -14149, "ran" }, { -14145, "rang" }, { -14140, "rao" }, { -14137, "re" }, { -14135, "ren" }, { -14125, "reng" }  
, { -14123, "ri" }, { -14122, "rong" }, { -14112, "rou" }, { -14109, "ru" }, { -14099, "ruan" }, { -14097, "rui" }, { -14094, "run" }  
, { -14092, "ruo" }, { -14090, "sa" }, { -14087, "sai" }, { -14083, "san" }, { -13917, "sang" }, { -13914, "sao" }, { -13910, "se" }  
, { -13907, "sen" }, { -13906, "seng" }, { -13905, "sha" }, { -13896, "shai" }, { -13894, "shan" }, { -13878, "shang" },   
{ -13870, "shao" }, { -13859, "she" }, { -13847, "shen" }, { -13831, "sheng" }, { -13658, "shi" }, { -13611, "shou" },  
 { -13601, "shu" }, { -13406, "shua" }, { -13404, "shuai" }, { -13400, "shuan" }, { -13398, "shuang" }, { -13395, "shui" },  
 { -13391, "shun" }, { -13387, "shuo" }, { -13383, "si" }, { -13367, "song" }, { -13359, "sou" }, { -13356, "su" },   
{ -13343, "suan" }, { -13340, "sui" }, { -13329, "sun" }, { -13326, "suo" }, { -13318, "ta" }, { -13147, "tai" }, { -13138, "tan" },  
 { -13120, "tang" }, { -13107, "tao" }, { -13096, "te" }, { -13095, "teng" }, { -13091, "ti" }, { -13076, "tian" },   
{ -13068, "tiao" }, { -13063, "tie" }, { -13060, "ting" }, { -12888, "tong" }, { -12875, "tou" }, { -12871, "tu" },   
{ -12860, "tuan" }, { -12858, "tui" }, { -12852, "tun" }, { -12849, "tuo" }, { -12838, "wa" }, { -12831, "wai" }, { -12829, "wan" }  
, { -12812, "wang" }, { -12802, "wei" }, { -12607, "wen" }, { -12597, "weng" }, { -12594, "wo" }, { -12585, "wu" }, { -12556, "xi" }  
, { -12359, "xia" }, { -12346, "xian" }, { -12320, "xiang" }, { -12300, "xiao" }, { -12120, "xie" }, { -12099, "xin" },   
{ -12089, "xing" }, { -12074, "xiong" }, { -12067, "xiu" }, { -12058, "xu" }, { -12039, "xuan" }, { -11867, "xue" },   
{ -11861, "xun" }, { -11847, "ya" }, { -11831, "yan" }, { -11798, "yang" }, { -11781, "yao" }, { -11604, "ye" }, { -11589, "yi" },  
 { -11536, "yin" }, { -11358, "ying" }, { -11340, "yo" }, { -11339, "yong" }, { -11324, "you" }, { -11303, "yu" },   
{ -11097, "yuan" }, { -11077, "yue" }, { -11067, "yun" }, { -11055, "za" }, { -11052, "zai" }, { -11045, "zan" },  
 { -11041, "zang" }, { -11038, "zao" }, { -11024, "ze" }, { -11020, "zei" }, { -11019, "zen" }, { -11018, "zeng" },   
{ -11014, "zha" }, { -10838, "zhai" }, { -10832, "zhan" }, { -10815, "zhang" }, { -10800, "zhao" }, { -10790, "zhe" },   
{ -10780, "zhen" }, { -10764, "zheng" }, { -10587, "zhi" }, { -10544, "zhong" }, { -10533, "zhou" }, { -10519, "zhu" },   
{ -10331, "zhua" }, { -10329, "zhuai" }, { -10328, "zhuan" }, { -10322, "zhuang" }, { -10315, "zhui" }, { -10309, "zhun" },   
{ -10307, "zhuo" }, { -10296, "zi" }, { -10281, "zong" }, { -10274, "zou" }, { -10270, "zu" }, { -10262, "zuan" }, { -10260, "zui" }  
, { -10256, "zun" }, { -10254, "zuo" } };

        #endregion

        #region 拼音处理
        public static string ToPinYinHead(string txt, int len, string s0)
        {
            string s1 = ToPinYinHead(txt);
            if (s1.Length <= 0)
                return s0;
            else if (s1.Length > len)
                return s1.Substring(0, len);
            else
                return s1;
        }
        ///   <summary>   
        ///   汉字转拼音   
        ///   </summary>   
        ///   <param   name="txt"> 需要转换的汉字 </param>   
        ///   <returns> 返回汉字对应的拼音 </returns>   
        public static string ToPinYin(string txt)
        {
            txt = txt.Trim();
            byte[] arr = new byte[2];   //每个汉字为2字节   
            StringBuilder result = new StringBuilder();//使用StringBuilder优化字符串连接  
            int charCode = 0;
            int arr1 = 0;
            int arr2 = 0;
            char[] arrChar = txt.ToCharArray();
            for (int j = 0; j < arrChar.Length; j++)   //遍历输入的字符   
            {
                arr = System.Text.Encoding.Default.GetBytes(arrChar[j].ToString());//根据系统默认编码得到字节码   
                if (arr.Length == 1)//如果只有1字节说明该字符不是汉字，结束本次循环   
                {
                    result.Append(arrChar[j].ToString());
                    continue;

                }
                arr1 = (short)(arr[0]);   //取字节1   
                arr2 = (short)(arr[1]);   //取字节2   
                charCode = arr1 * 256 + arr2 - 65536;//计算汉字的编码   

                if (charCode > -10254 || charCode < -20319)  //如果不在汉字编码范围内则不改变   
                {
                    result.Append(arrChar[j]);
                }
                else
                {
                    //根据汉字编码范围查找对应的拼音并保存到结果中   
                    //由于charCode的键不一定存在，所以要找比他更小的键上一个键  
                    if (!CodeCollections.ContainsKey(charCode))
                    {
                        for (int i = charCode; i <= 0; --i)
                        {
                            if (CodeCollections.ContainsKey(i))
                            {
                                result.Append(" " + CodeCollections[i] + " ");
                                break;
                            }
                        }
                    }
                    else
                    {
                        result.Append(" " + CodeCollections[charCode] + " ");
                    }
                }
            }
            return result.ToString();
        }

        ///   <summary>   
        ///   汉字转拼音   
        ///   </summary>   
        ///   <param   name="txt"> 需要转换的汉字 </param>   
        ///   <returns> 返回汉字对应的拼音 </returns>   
        public static string ToPinYinHead(string txt)
        {
            txt = txt.Trim();
            byte[] arr = new byte[2];   //每个汉字为2字节   
            StringBuilder result = new StringBuilder();//使用StringBuilder优化字符串连接  
            int charCode = 0;
            int arr1 = 0;
            int arr2 = 0;
            char[] arrChar = txt.ToCharArray();
            for (int j = 0; j < arrChar.Length; j++)   //遍历输入的字符   
            {
                arr = System.Text.Encoding.Default.GetBytes(arrChar[j].ToString());//根据系统默认编码得到字节码   
                if (arr.Length == 1)//如果只有1字节说明该字符不是汉字，结束本次循环   
                {
                    result.Append(arrChar[j].ToString());
                    continue;

                }
                arr1 = (short)(arr[0]);   //取字节1   
                arr2 = (short)(arr[1]);   //取字节2   
                charCode = arr1 * 256 + arr2 - 65536;//计算汉字的编码   

                if (charCode > -10254 || charCode < -20319)  //如果不在汉字编码范围内则不改变   
                {
                    result.Append(arrChar[j]);
                }
                else
                {
                    //根据汉字编码范围查找对应的拼音并保存到结果中   
                    //由于charCode的键不一定存在，所以要找比他更小的键上一个键  
                    if (!CodeCollections.ContainsKey(charCode))
                    {
                        for (int i = charCode; i <= 0; --i)
                        {
                            if (CodeCollections.ContainsKey(i))
                            {
                                result.Append(CodeCollections[i].Substring(0, 1));
                                break;
                            }
                        }
                    }
                    else
                    {
                        result.Append(CodeCollections[charCode].Substring(0, 1));
                    }
                }
            }
            return result.ToString();
        }
        #endregion

        /// <summary>
        /// 获取字符串中的数值字符串
        /// </summary>
        /// <param name="InStr">输入的字符串</param>
        /// <param name="index">字符串序号:输出第几组数值字符串,从0开始</param>
        /// <param name="iMode">顺序：0从左开始 非0从右开始</param>
        /// <param name="iStart">起始位置</param>
        /// <returns></returns>
        public static string GetNumberFromStr(string InStr, int index, int iMode, ref int iStart)
        {
            char[] arr = InStr.ToCharArray();
            List<string> LsNum = new List<string>();
            List<int> LsInd = new List<int>();
            int i = 0;
            int startpos = 0;
            bool bStart = false;
            while (i < arr.Length)
            {
                if (bStart)
                {
                    if (arr[i] != '.' && !Char.IsDigit(arr[i]))
                    {
                        string temp = InStr.Substring(startpos, i - startpos);
                        LsNum.Add(temp);
                        LsInd.Add(startpos);
                        bStart = false;
                    }
                }
                else
                {
                    if (Char.IsDigit(arr[i]))
                    {
                        bStart = true;
                        startpos = i;
                        if (i > 0 && arr[i - 1] == '-')
                        {
                            startpos = i - 1;
                        }
                    }
                }

                i++;
                if (i == arr.Length && bStart)
                {
                    string temp = InStr.Substring(startpos, i - startpos);
                    LsNum.Add(temp);
                    LsInd.Add(startpos);
                    bStart = false;
                }
            }// end while
            if (index < LsNum.Count)
            {
                if (iMode == 0)
                {
                    iStart = LsInd[index];
                    return LsNum[index];
                }
                else
                {
                    iStart = LsInd[LsNum.Count - index - 1];
                    return LsNum[LsNum.Count - index - 1];
                }
            }
            else
                return null;
        }
        /// <summary>
        /// 获取字符串中的数值字符串
        /// </summary>
        /// <param name="InStr">输入的字符串</param>
        /// <param name="index">字符串序号:输出第几组数值字符串,从0开始</param>
        /// <param name="iMode">顺序：0从左开始 非0从右开始</param>
        /// <returns></returns>
        public static string GetNumberFromStr(string InStr, int index, int iMode)
        {
            char[] arr = InStr.ToCharArray();
            List<string> LsNum = new List<string>();
            List<int> LsInd = new List<int>();
            int i = 0;
            int startpos = 0;
            bool bStart = false;
            while (i < arr.Length)
            {
                if (bStart)
                {
                    if (arr[i] != '.' && !Char.IsDigit(arr[i]))
                    {
                        string temp = InStr.Substring(startpos, i - startpos);
                        LsNum.Add(temp);
                        LsInd.Add(startpos);
                        bStart = false;
                    }
                }
                else
                {
                    if (Char.IsDigit(arr[i]))
                    {
                        bStart = true;
                        startpos = i;
                        if (i > 0 && arr[i - 1] == '-')
                        {
                            startpos = i - 1;
                        }
                    }
                }

                i++;
                if (i == arr.Length && bStart)
                {
                    string temp = InStr.Substring(startpos, i - startpos);
                    LsNum.Add(temp);
                    LsInd.Add(startpos);
                    bStart = false;
                }
            }// end while
            if (index < LsNum.Count)
            {
                if (iMode == 0)
                {
                    return LsNum[index];
                }
                else
                {
                    return LsNum[LsNum.Count - index - 1];
                }
            }
            else
                return null;
        }

        /// <summary>
        /// 获取两个字符串递增的规律
        /// </summary>
        /// <param name="str0">输入：字符串1</param>
        /// <param name="str1">输入：字符串2</param>
        /// <param name="dStart">输出：递增的起始值</param>
        /// <param name="dAdd">输出：递增值</param>
        /// <param name="sChar">去掉递增数值后剩下的字符</param>
        /// <param name="iStart">递增数值串在字符串的起始位置</param>
        /// <param name="iLen">递增数值串的长度</param>
        /// <returns>0:不存在递增关系  1:纯数值  2：字符和数字混合字符串 3:字符串相等</returns>
        public static int Get2StrChange(string str0, string str1, ref double dStart, ref double dAdd, ref string sChar, ref int iStart, ref int iLen)
        {
            if (str0 == str1)
                return 3;
            int istr1 = 0, istr0 = 0;
            string num0 = GetNumberFromStr(str0, 0, 1, ref istr0);
            string num1 = GetNumberFromStr(str1, 0, 1, ref istr1);
            string str00 = str0.Substring(0, istr0);
            string str01 = str0.Substring(istr0 + num0.Length);
            string str10 = str1.Substring(0, istr1);
            string str11 = str1.Substring(istr1 + num1.Length);
            if (num0.Length == str0.Length && num1.Length == str1.Length)
            {
                dStart = Convert.ToDouble(num0);
                dAdd = Convert.ToDouble(num1) - dStart;
                sChar = "";
                iStart = 0;
                iLen = num0.Length;
                return 1;
            }

            if (num0.Length != num1.Length || str00 != str10 || str01 != str11 || istr0 != istr1)
                return 0;
            sChar = str00 + str01;
            dStart = Convert.ToDouble(num0);
            dAdd = Convert.ToDouble(num1) - dStart;
            iStart = istr0;
            iLen = num0.Length;
            return 2;
        }
        /// <summary>
        /// 根据两个字符串的递增规律返回下一个字符串
        /// </summary>
        /// <param name="str0">第一个字符串</param>
        /// <param name="str1">第二个字符串</param>
        /// <param name="dZ">默认递增量</param>
        /// <param name="bChun">如果是纯数字是否不考虑数据长度</param>
        /// <returns>下一个字符串</returns>
        public static string GetNextFrom2Str(string str0, string str1, int dZ, bool bChun)
        {
            //如果两个字符相等，返回任意一个
            if (str0 == str1)
                return str1;
            //如果一个为空，按一个递增dZ
            string sOne = "";
            if (str0 == "") sOne = str1;
            if (str1 == "") sOne = str0;
            if (sOne.Length > 0)//有一个为空
            {
                return GetNextFrom1Str(sOne, dZ, bChun);
            }

            if (IsNum(str0) && IsNum(str1) && bChun)
            {
                double d0 = Convert.ToDouble(str0);
                double d1 = Convert.ToDouble(str1);
                return (d1 + d1 - d0).ToString();
            }
            int istr1 = 0, istr0 = 0;
            string num0 = GetNumberFromStr(str0, 0, 1, ref istr0);
            string num1 = GetNumberFromStr(str1, 0, 1, ref istr1);
            string str00 = str0.Substring(0, istr0);
            string str01 = str0.Substring(istr0 + num0.Length);
            string str10 = str1.Substring(0, istr1);
            string str11 = str1.Substring(istr1 + num1.Length);
            if (num0.Length == str0.Length && num1.Length == str1.Length)
            {
                double dStart = Convert.ToDouble(num0);
                double dAdd = Convert.ToDouble(num1) - dStart;
                return GetNextFrom1Str(str1, dAdd, bChun);
            }
            else if (num0.Length != num1.Length || str00 != str10 || str01 != str11 || istr0 != istr1)
            {
                return GetNextFrom1Str(str1, dZ, bChun);
            }
            else
            {
                double dStart = Convert.ToDouble(num0);
                double dAdd = Convert.ToDouble(num1) - dStart;
                return GetNextFrom1Str(str1, dAdd, bChun);
            }
        }
        /// <summary>
        /// 根据初始字符串和递增量返回下一个字符串
        /// </summary>
        /// <param name="str0">初始字符串</param>
        /// <param name="dZ">递增量</param>
        /// <param name="bChun">如果是纯数字是否不考虑数据长度</param>
        /// <returns>下一个字符串</returns>
        public static string GetNextFrom1Str(string str0, double dZ, bool bChun)
        {
            if (IsNum(str0) && bChun)
            {
                double d0 = Convert.ToDouble(str0);
                return (d0 + dZ).ToString();
            }

            int iOne = 0;
            string One = GetNumberFromStr(str0, 0, 1, ref iOne);
            if (One.Length > 0)
            {
                int i0 = Convert.ToInt32(One);
                string s1 = (i0 + dZ + Math.Pow(10, One.Length + 5)).ToString();
                return str0.Substring(0, iOne) + s1.Substring(s1.Length - One.Length) + str0.Substring(iOne + One.Length);
            }
            else
            {
                int len = (dZ * 10).ToString().Length;
                string s1 = (dZ + Math.Pow(10, len + 5)).ToString();
                return str0.Substring(0, str0.Length - One.Length) + s1.Substring(s1.Length - One.Length);
            }
        }
        /// <summary>
        /// 获取字符串长度，汉字当两个字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetLength(string str)
        {
            if (str.Length == 0) return 0;

            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }

            return tempLen;
        }

        public static string Get2StrTo1(string str1, string str2, int Length, char nchar)
        {
            int i1 = GetLength(str1);

            if (i1 > Length)
                return str1 + str2;
            else
                return str1 + str2.PadLeft(Length - i1, nchar);
        }

        public static string Reverse(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>  
        /// 判断输入的字符串是否只包含数字和英文字母  
        /// </summary>  
        /// <param name="input"></param>  
        /// <returns></returns>  
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>  
        /// 是否匹配正则表达式 
        /// </summary>  
        /// <param name="input"></param>  
        /// <returns></returns>  
        public static bool IsMatchKeys(string input, string sKey)
        {
            if (sKey == null || sKey == "")
                return true;
            sKey = sKey.Trim().Replace("\r\n", "\n");
            sKey = sKey.Replace('\n', ';');
            sKey = sKey.Replace('；', ';');
            sKey = sKey.Replace('\t', ';');
            string[] str1 = sKey.Split(';');
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i].Length > 0)
                {
                    if (IsMatchKey(input, str1[i]))
                        return true;
                }
            }
            return false;
        }
        /// <summary>  
        /// 是否匹配正则表达式 
        /// </summary>  
        /// <param name="input"></param>  
        /// <returns></returns>  
        public static bool IsMatchKey(string input, string sKey)
        {
            try
            {
                Regex regex = new Regex(sKey);
                return regex.IsMatch(input);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取SQL查询条件
        /// </summary>
        /// <param name="scon"></param>
        /// <param name="sCol"></param>
        public static string GetWhereFromStrs(string scon, string sCol)
        {
            if (scon == null || scon == "")
                return "";
            string sRe = "(";
            scon = scon.Trim().Replace("\r\n", "\n");
            scon = scon.Replace('\n', ';');
            scon = scon.Replace('；', ';');
            scon = scon.Replace('\t', ';');
            string[] str1 = scon.Split(';');
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i].Length > 0)
                {
                    if (sRe.Length > 1)
                        sRe += " or ";
                    sRe += string.Format("{0} like '%{1}%' ", sCol, str1[i]);
                }
            }
            if (sRe.Length > 1)
                sRe += ")";
            else
                sRe = "";
            return sRe;
        }

        /// <summary>  
        /// 只含有汉字、数字、字母、下划线，下划线位置不限 
        /// </summary>  
        /// <param name="input"></param>  
        /// <returns></returns>  
        public static bool Is_0A汉(string input)
        {
            string pattern = @"^[a-zA-Z0-9_\u4e00-\u9fa5]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        public static bool IsNum(string input)
        {
            string pattern = @"^[0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        public static bool IsFloat(string input)
        {
            string pattern = @"^(\+?|\-?)(\d+)\.?\d*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 判断字符串是否是IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsCorrectIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string Repalc(string sOld, Dictionary<string, string> LsTH)
        {
            if (sOld == "")
                return sOld;
            if (IsFloat(sOld))
                return sOld;
            foreach (KeyValuePair<string, string> nkv in LsTH)
            {
                if (sOld.IndexOf(nkv.Key) >= 0)
                    sOld = sOld.Replace(nkv.Key, nkv.Value);
            }
            return sOld;
        }

        public static List<int> IndexOf(string input, string value, int startindex)
        {
            List<int> iRe = new List<int>();
            try
            {
                int iIndex = input.IndexOf(value, startindex);
                while (iIndex >= 0)
                {
                    iRe.Add(iIndex);
                    iIndex = input.IndexOf(value, iIndex + 1);
                }
                return iRe;
            }
            catch (Exception ex)
            {
                return iRe;
            }
        }

        public static bool Check(String szIPInfo)
        {/*
            String[] split = szIPInfo.Split(';');
            if (split.Length > 2)
                MessageBox.Show("请输入正确格式的IP地址与端口号字符串，型如：192.168.1.1:502或！192.168.1.2:5002;192.168.1.3:5003", "提示", MessageBoxButtons.OK);
            if (split.Length == 1)
            {
                if (!CheckPortID(szIPInfo))
                    return false;
                return true;
            }
            else
            {
                String TCPServerAddress = split[0];
                if (!CheckPortID(TCPServerAddress))
                    return false;
                TCPServerAddress = split[1];
                if (!CheckPortID(TCPServerAddress))
                    return false;
            }*/
            return true;
        }

        public static bool CheckPortID(String TCPServerAddress, ref string sMsg)
        {
            sMsg = "请输入正确格式的IP地址与端口号字符串，型如：192.168.1.1:502！";
            bool bRes = true;
            String[] split = TCPServerAddress.Split(':');
            if (split.Length > 1)
            {
                int PortSource = System.Convert.ToInt32(split[1]);
                if (PortSource < 100 || PortSource > 9999)
                {
                    sMsg += "\r\n使用TCP模式时端口号范围是100～9999！";
                    bRes = false;
                }
                if (CheckIP(split[0]) ==false)
                {
                    bRes =false;
                }
            }
            else
            {
                bRes = false;
            }
            return bRes;
        }

        public static bool CheckIP(String szRemoteIP)
        {
            String[] split = szRemoteIP.Split('.');
            int i = 0;
            IEnumerator myEnum = split.GetEnumerator();
            while (myEnum.MoveNext())
            {
                String s = (String)(myEnum.Current);
                try
                {
                    int IPSec = System.Convert.ToInt32(s);
                    if (i == 0)
                    {
                        if (IPSec < 1 || IPSec > 223)
                            return false;
                    }
                    if (IPSec > 255)
                        return false;

                }
                catch (Exception e)
                {
                    return false;
                }
                i++;
            }
            if (i < 4)
                return false;
            return true;
        }

        public static bool CheckCommConfig(String CommConfig, ref string sMsg)
        {
            sMsg = "请输入正确格式的串口参数设置字符串，型如：Com8,9600,N,8,1";
            bool bRes = true;
            CommConfig = CommConfig.Replace('，', ',');
            String[] split = CommConfig.Split(',');
            if (split.Length >= 5)
            {
                try
                {
                    int BaudRate = Convert.ToInt32(split[1]);
                    if (Convert.ToInt16(split[3]) < 5 || Convert.ToInt16(split[3]) > 8)
                    {
                        sMsg += "\r\n数据位的值不能小于 5 或大于 8。";
                        bRes = false;
                    }
                    if (split[2] != "N" && split[2] != "O" && split[2] != "E")
                    {
                        sMsg += "\r\n校验位字母分别为：N无校验 O奇校验 E偶校验";
                        bRes =false;
                    }
                    if (split[4] != "0" && split[4] != "1" && split[4] != "2" && split[4] != "1.5")
                    {
                        sMsg += "\r\n停止位的取值只能是：0, 1, 2, 1.5";
                        bRes = false;
                    }
                }
                catch (Exception ex)
                {
                    sMsg += "\r\n" + ex.Message;
                    bRes = false;
                }
                return bRes;
            }
            else
            {
                bRes = false;
            }
            return bRes;
        }

        /************************************************************************/
        /* Shorten a path with ellipses.                                        */
        /************************************************************************/
        /// <summary>
        /// 路径缩写
        /// </summary>
        /// <param name="Inpath">输入的完整路径名</param>
        /// <param name="maxPath">压缩后的长度</param>
        /// <returns>中间经过压缩的路径名</returns>
        public static string shortenPath(string Inpath, int maxPath)
        {
            string path = Inpath;
            /**********************************************************************/
            /* If the path fits, just return it                                   */
            /**********************************************************************/
            if (path.Length <= maxPath)
            {
                return path;
            }
            /**********************************************************************/
            /* If there's no backslash, just truncate the path                    */
            /**********************************************************************/
            int lastBackslash = path.LastIndexOf('\\');
            if (lastBackslash < 0)
            {
                return path.Substring(0, maxPath - 3) + "......";
            }

            /**********************************************************************/
            /* Shorten the front of the path                                      */
            /**********************************************************************/
            int fromLeft = (lastBackslash - 3) - (path.Length - maxPath);
            // (12 - 3) - (19 - 10) = 9 - 9 = 0 
            if ((lastBackslash <= 3) || (fromLeft < 1))
            {
                path = "......" + path.Substring(lastBackslash);
            }
            else
            {
                path = path.Substring(0, fromLeft) + "......" + path.Substring(lastBackslash);
            }

            /**********************************************************************/
            /* Truncate the path                                                  */
            /**********************************************************************/
            if (path.Length > maxPath)
            {
                path = path.Substring(0, maxPath - 3) + "......";
            }

            return path;
        }
        /// <summary>
        /// 缩写路径
        /// </summary>
        /// <param name="Inpath">输入的完整路径名</param>
        /// <returns>中间经过压缩的路径名</returns>
        public static string shortenPath(string Inpath)
        {
            return shortenPath(Inpath, 40);
        }

        /**/
        /// <summary>
        /// 替换单个文本文件中的文本
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <param name="sMsg"></param>
        /// <returns></returns>
        public static bool ReplaceFile(string filename, string search, string replace, ref string sMsg)
        {
            try
            {
                //防止文本字符中有特殊字符。必须用Encoding.Default
                StreamReader reader = new StreamReader(filename, Encoding.Default);

                String a = reader.ReadToEnd();
                a = a.Replace(search, replace);

                StreamWriter readTxt = new StreamWriter(filename + ".bak", false, Encoding.Default);
                readTxt.Write(a);
                readTxt.Flush();
                readTxt.Close();
                reader.Close();
                File.Copy(filename + ".bak", filename, true);
                File.Delete(filename + ".bak");
                return true;
            }
            catch (Exception ex)
            {
                sMsg = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 列表转字符串
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="jiange"></param>
        /// <returns></returns>
        public static string ListToString(List<string> ls, string jiange)
        {
            string str = "";
            foreach (string s in ls)
            {
                str += s + jiange;
            }
            return str;
        }
        /// <summary>
        /// 字符串转列表
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="jiange"></param>
        /// <returns></returns>
        public static List<string> StringToList(string ls, char jiange)
        {
            string[] str = ls.Split(jiange);
            List<string> str2 = new List<string>();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Length > 0)
                    str2.Add(str[i]);
            }
            return str2;
        }
        /// <summary>
        /// 字符串转列表
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="jiange"></param>
        /// <returns></returns>
        public static List<string> StringToList(string ls, char[] jiange)
        {
            string[] str = ls.Split(jiange);
            List<string> str2 = new List<string>();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Length > 0)
                    str2.Add(str[i]);
            }
            return str2;
        }

        public static string IntListToStr(List<int> ls)
        {
            string str1 = "";
            ls.Sort();
            ls = ls.Distinct().ToList();

            if (ls.Count < 3)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    str1 += "," + ls[i].ToString();
                }
                if (str1.Length > 0)
                    return str1.Substring(1);
                else
                    return "";

            }
            int i0 = ls[0];
            int i1 = ls[1];
            List<int> lsd = new List<int>();
            for (int i = 2; i < ls.Count; i++)
            {
                if ((ls[i] - i1) != (i1 - i0))//i开始没有规律
                {
                    if (lsd.Count == 0)
                        str1 += "," + i0.ToString();
                    else
                    {
                        str1 += "," + ListToStr(lsd);
                        lsd = new List<int>();
                        if (i == ls.Count - 1)
                        {
                            str1 += "," + ls[ls.Count - 1].ToString();
                            if (str1.Length > 0)
                                return str1.Substring(1);
                            else
                                return "";
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    if (lsd.Count == 0)//重新开始规律
                    {
                        lsd.Add(i0);
                        lsd.Add(i1);
                        lsd.Add(ls[i]);
                    }
                    else
                    {
                        lsd.Add(ls[i]);
                    }
                }
                if (i >= ls.Count - 1)
                {
                    if (lsd.Count == 0)
                    {
                        str1 += "," + ls[ls.Count - 2].ToString();
                        str1 += "," + ls[ls.Count - 1].ToString();
                    }
                    else
                    {
                        str1 += "," + ListToStr(lsd);
                    }
                }
                else
                {
                    i0 = ls[i - 1];
                    i1 = ls[i];
                }
            }
            if (str1.Length > 0)
                return str1.Substring(1);
            else
                return "";
        }

        public static string ListToStr(List<int> ls)
        {
            string str1 = "";
            if (ls.Count <= 2)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    if (str1.Length <= 0)
                        str1 = ls[i].ToString();
                    else
                        str1 = "," + ls[i].ToString();
                }
                return str1;
            }
            else
            {
                if (ls[1] - ls[0] == 1)
                    return string.Format("{0}-{1}", ls[0], ls[ls.Count - 1]);
                else
                    return string.Format("{0}-{1}-{2}", ls[0], ls[1], ls[ls.Count - 1]);

            }
        }

        public static bool IsIntList(string str1, ref string sMsg)
        {
            sMsg = "";
            string str2 = str1.Replace("，", ",");

            string str3 = str2.Replace("_", "");
            str3 = str3.Replace("-", "");
            str3 = str3.Replace(",", "");
            if (!IsNum(str3))
            {
                sMsg = string.Format("字符串{0}不符合下面的格式：1,4,6或者1_3,5或者1-3-9,11", str1);
                return false;
            }
            return true;
        }
        public static List<int> StrToIntList(string str1)
        {
            List<int> ls = new List<int>();
            string str2 = str1.Replace("，",",");
            str2 = str2.Replace("_", "-");
            string str3 = str2.Replace("-","");
            str3 = str3.Replace(",","");
            if (!IsNum(str3))
            {
                //sMsg = string.Format("字符串{0}不符合下面的格式：1,4,6或者1-3,5或者1-3-9,11",str1);
                return ls;
            }
            string[] str4 = str2.Split(',');
            for (int i = 0; i < str4.Length; i++)
            {
                string s = str4[i];
                if (s.Length <= 0) continue;
                if (IsNum(s))
                {
                    int iS = Convert.ToInt32(s);
                        ls.Add(iS);
                }
                else
                {
                    string[] str5 = s.Split('-');
                    if (str5.Length >= 3 && IsNum(str5[0]) && IsNum(str5[1]) && IsNum(str5[2]))
                    {
                        int i0 = Convert.ToInt32(str5[0]);
                        int i1 = Convert.ToInt32(str5[1]);
                        int i2 = Convert.ToInt32(str5[2]);
                        int k0 = i0;
                        int k1 = i1;
                        if (i0 == i1) continue;
                        if ((i2 > i0 && i2 > i1) || (i2 < i0 && i2 < i1))
                        {
                            if ((i2 > i0 && i0 > i1) || (i2 < i0 && i0 < i1))
                            {
                                k0 = i1;
                                k1 = i0;
                            }
                            int d = k1 - k0;
                            for (int m = k0; m <= i2; m += d)
                            {
                                ls.Add(m);
                            }
                        }
                    }
                    else if (str5.Length >= 2 && IsNum(str5[0]) && IsNum(str5[1]))
                    {
                        int i0 = Convert.ToInt32(str5[0]);
                        int i1 = Convert.ToInt32(str5[1]);
                        int k0 = i0;
                        int k1 = i1;
                        if(i0>i1)
                        {
                            k0 = i1;
                            k1 = i0;
                        }
                        for (int m = k0; m <= k1; m++)
                        {
                            ls.Add(m);
                        }
                    }
                }
            }
            return ls;
        }

        public static int GetIValFromTextbox(TextBox txt, int v0)
        {
            try
            {
                if (IsNum(txt.Text))
                    return Convert.ToInt32(txt.Text);
                else
                    return v0;
            }
            catch 
            {
                return v0;
            }
        }

        public static float GetFValFromTextbox(TextBox txt, float v0)
        {
            try
            {
                if (IsNum(txt.Text))
                    return Convert.ToSingle(txt.Text);
                else
                    return v0;
            }
            catch
            {
                return v0;
            }
        }

        public static void InsertMsg(RichTextBox richText1, string sText)
        {
            InsertMsg(richText1, sText, true, 1000);
        }

        public static void InsertMsg(RichTextBox richText1,string sText, bool bError,int MaxLen)
        {
            //iMsg++;
            //richText1.AppendText( iMsg.ToString() + ":" + sText );
            int len1 = richText1.Text.Length;
            string ss = DateTime.Now.ToString("HH:mm:ss ") + sText + "\n";
            richText1.AppendText(ss);
            int len2 = richText1.Text.Length;

            if (len2 > len1)
            {
                richText1.Select(len1, len2 - len1 - 1);
                if (!bError)
                    richText1.SelectionColor = Color.Red;
                else
                    richText1.SelectionColor = Color.Black;
                richText1.Select(richText1.TextLength, 0);
                richText1.ScrollToCaret();
            }
            if (richText1.Lines.Length > MaxLen)
            {
                richText1.Select(0, richText1.Lines[0].Length + 1);
                richText1.SelectedText = "";
            }
        }
    }
}
