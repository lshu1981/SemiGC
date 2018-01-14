using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
namespace SemiGC
{
    public class CExcel
    {
        public static List<System.Drawing.Color> BlackColor = new List<System.Drawing.Color> 
        { 
            Color.AntiqueWhite,//250,235,215,
            Color.Aquamarine,//127,255,212,
            Color.Beige,//245,245,220,
            Color.PowderBlue,//176,224,230,
            Color.BlanchedAlmond,//255,255,205,
            Color.Lavender,//230,230,250,
            Color.PowderBlue,//176,224,230,
            Color.PaleGreen,//152,251,152,
            Color.LightSkyBlue,//135,206,250,
            Color.GreenYellow,//173,255,47,
            Color.LightGreen,//144,238,144,
            Color.WhiteSmoke,//245,245,245,
            Color.LightGray//211,211,211
        };
        /// <summary>
        /// 单元格背景色及填充方式
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="sC">起始列</param>
        /// <param name="eR">结束行</param>
        /// <param name="eC">结束列</param>
        /// <param name="color">颜色索引</param>
        public static void CellsBackColor(Application mEx, int sR, int sC, int eR, int eC, Color color)
        {
            Range range = mEx.Range[mEx.Cells[sR, sC], mEx.Cells[eR, eC]];
            range.Cells.Interior.Color = color;
            range.Interior.Pattern = Pattern.Solid;
        }

        /// <summary>
        /// 单元格网格
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="sC">起始列</param>
        /// <param name="eR">结束行</param>
        /// <param name="eC">结束列</param>
        public static void CellsLineStyle(Application mEx, int sR, int sC, int eR, int eC)
        {
            Range range = mEx.Range[mEx.Cells[sR, sC], mEx.Cells[eR, eC]];
            range.Borders[XlBordersIndex.xlDiagonalDown].LineStyle = XlLineStyle.xlLineStyleNone;
            range.Borders[XlBordersIndex.xlDiagonalUp].LineStyle = XlLineStyle.xlLineStyleNone;

            range.Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            range.Borders[XlBordersIndex.xlEdgeLeft].Color = Color.Black;
            range.Borders[XlBordersIndex.xlEdgeLeft].TintAndShade = 0;
            range.Borders[XlBordersIndex.xlEdgeLeft].Weight = XlBorderWeight.xlThin;

            range.Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            range.Borders[XlBordersIndex.xlEdgeTop].Color = Color.Black;
            range.Borders[XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            range.Borders[XlBordersIndex.xlEdgeTop].Weight = XlBorderWeight.xlThin;

            range.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            range.Borders[XlBordersIndex.xlEdgeBottom].Color = Color.Black;
            range.Borders[XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            range.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThin;

            range.Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            range.Borders[XlBordersIndex.xlEdgeRight].Color = Color.Black;
            range.Borders[XlBordersIndex.xlEdgeRight].TintAndShade = 0;
            range.Borders[XlBordersIndex.xlEdgeRight].Weight = XlBorderWeight.xlThin;

            range.Borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            range.Borders[XlBordersIndex.xlInsideVertical].Color = Color.Black;
            range.Borders[XlBordersIndex.xlInsideVertical].TintAndShade = 0;
            range.Borders[XlBordersIndex.xlInsideVertical].Weight = XlBorderWeight.xlThin;

            range.Borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;
            range.Borders[XlBordersIndex.xlInsideHorizontal].Color = Color.Black;
            range.Borders[XlBordersIndex.xlInsideHorizontal].TintAndShade = 0;
            range.Borders[XlBordersIndex.xlInsideHorizontal].Weight = XlBorderWeight.xlThin;
        }

        /// <summary>
        /// 单元格网格
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="sC">起始列</param>
        /// <param name="eR">结束行</param>
        /// <param name="eC">结束列</param>
        public static void CellsAlignment(Application mEx, int sR, int sC, int eR, int eC, Constants HA, Constants VA)
        {
            Range range = mEx.Range[mEx.Cells[sR, sC], mEx.Cells[eR, eC]];
            range.HorizontalAlignment = HA;
            range.VerticalAlignment = VA;
        }
        /// <summary>
        /// 单元格背景色及填充方式
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="sC">起始列</param>
        /// <param name="eR">结束行</param>
        /// <param name="eC">结束列</param>
        /// <param name="color">颜色索引</param>
        /// <param name="pattern">填充方式</param>
        public static void CellsBackColor(Application mEx, int sR, int sC, int eR, int eC, Color color, Pattern pattern)
        {
            Range range = mEx.get_Range(mEx.Cells[sR, sC], mEx.Cells[eR, eC]);
            range.Interior.Color = color;
            range.Interior.Pattern = pattern;
        }
        /// <summary>
        /// 设置行高
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="eR">结束行</param>
        /// <param name="height">行高</param>
        public static void SetRowHeight(Application mEx, int sR, int eR, int height)
        {
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Rows[sR.ToString() + ":" + eR.ToString(), System.Type.Missing];
            range.RowHeight = height;
        }
        /// <summary>
        /// 自动调整行高
        /// </summary>
        /// <param name="columnNum">列号</param>
        public static void RowAutoFit(Application mEx, int rowNum)
        {
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Rows[rowNum.ToString() + ":" + rowNum.ToString(), System.Type.Missing];
            range.EntireColumn.AutoFit();
        }
        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="sC">起始列(列对应的字母)</param>
        /// <param name="eC">结束列(列对应的字母)</param>
        /// <param name="width"></param>
        public static void SetColumnWidth(Application mEx, string sC, string eC, int width)
        {
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Columns[sC + ":" + eC, System.Type.Missing];
            range.ColumnWidth = width;
        }
        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="sC">起始列</param>
        /// <param name="eC">结束列</param>
        /// <param name="width"></param>
        public static void SetColumnWidth(Application mEx, int sC, int eC, int width)
        {
            string strsC = GetColumnName(sC);
            string streC = GetColumnName(eC);
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Columns[strsC + ":" + streC, System.Type.Missing];
            range.ColumnWidth = width;
        }
        /// <summary>
        /// 自动调整列宽
        /// </summary>
        /// <param name="columnNum">列号</param>
        public static void ColumnAutoFit(Application mEx, string column)
        {
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Columns[column + ":" + column, System.Type.Missing];
            range.EntireColumn.AutoFit();
        }

        /// <summary>
        /// 自动调整列宽
        /// </summary>
        /// <param name="columnNum">列号</param>
        public static void ColumnAutoFit(Application mEx, int columnNum)
        {
            string strcolumnNum = GetColumnName(columnNum);
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Columns[strcolumnNum + ":" + strcolumnNum, System.Type.Missing];
            range.EntireColumn.AutoFit();

        }
        /// <summary>
        /// 自动调整列宽
        /// </summary>
        /// <param name="sC">起始列(列对应的字母)</param>
        /// <param name="eC">结束列(列对应的字母)</param>
        public static void ColumnAutoFit(Application mEx, string sC, string eC)
        {
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Columns[sC + ":" + eC, System.Type.Missing];
            range.EntireColumn.AutoFit();
        }
        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="sC">起始列</param>
        /// <param name="eC">结束列</param>
        public static void ColumnAutoFit(Application mEx, int sC, int eC)
        {
            string strsC = GetColumnName(sC);
            string streC = GetColumnName(eC);
            //获取当前正在使用的工作表
            Worksheet worksheet = (Worksheet)mEx.ActiveSheet;
            Range range = (Range)worksheet.Columns[strsC + ":" + streC, System.Type.Missing];
            range.EntireColumn.AutoFit();
        }
        /// <summary>
        /// 字体颜色
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="sC">起始列</param>
        /// <param name="eR">结束行</param>
        /// <param name="eC">结束列</param>
        /// <param name="color">颜色索引</param>
        public static void FontColor(Application mEx, int sR, int sC, int eR, int eC, Color color)
        {
            Range range = mEx.get_Range(mEx.Cells[sR, sC], mEx.Cells[eR, eC]);
            range.Font.Color = color;
        }
        /// <summary>
        /// 字体样式(加粗,斜体,下划线)
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="sC">起始列</param>
        /// <param name="eR">结束行</param>
        /// <param name="eC">结束列</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="isItalic">是否斜体</param>
        /// <param name="underline">下划线类型</param>
        public static void FontStyle(Application mEx, int sR, int sC, int eR, int eC, bool isBold, bool isItalic, UnderlineStyle underline)
        {
            Range range = mEx.get_Range(mEx.Cells[sR, sC], mEx.Cells[eR, eC]);
            range.Font.Bold = isBold;
            range.Font.Underline = underline;
            range.Font.Italic = isItalic;
        }
        /// <summary>
        /// 单元格字体及大小
        /// </summary>
        /// <param name="sR">起始行</param>
        /// <param name="sC">起始列</param>
        /// <param name="eR">结束行</param>
        /// <param name="eC">结束列</param>
        /// <param name="fontName">字体名称</param>
        /// <param name="fontSize">字体大小</param>
        public static void FontNameSize(Application mEx, int sR, int sC, int eR, int eC, string fontName, int fontSize)
        {
            Range range = mEx.get_Range(mEx.Cells[sR, sC], mEx.Cells[eR, eC]);
            range.Font.Name = fontName;
            range.Font.Size = fontSize;
        }
        /// <summary>
        /// 打开一个存在的Excel文件
        /// </summary>
        /// <param name="fileName">Excel完整路径加文件名</param>
        public static void Open(Application mEx, string fileName)
        {
            mEx = new Application();
            //  myWorkBook = mEx.Workbooks.Add(fileName);
            // myFileName = fileName;
        }
        /// <summary>
        /// 保存Excel
        /// </summary>
        /// <returns>保存成功返回True</returns>
        public bool Save()
        {
            if (myFileName == "")
            {
                return false;
            }
            else
            {
                try
                {
                    myWorkBook.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Excel文档另存为
        /// </summary>
        /// <param name="fileName">保存完整路径加文件名</param>
        /// <returns>保存成功返回True</returns>
        public bool SaveAs(Application mEx, string fileName)
        {
            try
            {
                myWorkBook.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 关闭Excel
        /// </summary>
        public static void Close(Application mEx)
        {
            //myWorkBook.Close(Type.Missing, Type.Missing, Type.Missing);
            mEx.Quit();
            //myWorkBook = null;
            mEx = null;
            GC.Collect();
        }
        /// <summary>
        /// 关闭Excel
        /// </summary>
        /// <param name="isSave">是否保存</param>
        public static void Close(Application mEx, bool isSave)
        {
            //myWorkBook.Close(isSave, Type.Missing, Type.Missing);
            mEx.Quit();
            //myWorkBook = null;
            mEx = null;
            GC.Collect();
        }
        /// <summary>
        /// 关闭Excel
        /// </summary>
        /// <param name="isSave">是否保存</param>
        /// <param name="fileName">存储文件名</param>
        public static void Close(Application mEx, bool isSave, string fileName)
        {
            //myWorkBook.Close(isSave, fileName, Type.Missing);
            mEx.Quit();
            //myWorkBook = null;
            mEx = null;
            GC.Collect();
        }
        #region 私有成员
        private static string GetColumnName(int number)
        {
            int h, l;
            h = number / 26;
            l = number % 26;
            if (l == 0)
            {
                h -= 1;
                l = 26;
            }
            string s = GetLetter(h) + GetLetter(l);
            return s;
        }
        private static string GetLetter(int number)
        {
            switch (number)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                case 5:
                    return "E";
                case 6:
                    return "F";
                case 7:
                    return "G";
                case 8:
                    return "H";
                case 9:
                    return "I";
                case 10:
                    return "J";
                case 11:
                    return "K";
                case 12:
                    return "L";
                case 13:
                    return "M";
                case 14:
                    return "N";
                case 15:
                    return "O";
                case 16:
                    return "P";
                case 17:
                    return "Q";
                case 18:
                    return "R";
                case 19:
                    return "S";
                case 20:
                    return "T";
                case 21:
                    return "U";
                case 22:
                    return "V";
                case 23:
                    return "W";
                case 24:
                    return "X";
                case 25:
                    return "Y";
                case 26:
                    return "Z";
                default:
                    return "";
            }
        }
        #endregion

        public string myFileName { get; set; }
        public Workbook myWorkBook { get; set; }
    }
    /// <summary>
    /// 水平对齐方式
    /// </summary>
    public enum ExcelHAlign
    {
        常规 = 1,
        靠左,
        居中,
        靠右,
        填充,
        两端对齐,
        跨列居中,
        分散对齐
    }
    /// <summary>
    /// 垂直对齐方式
    /// </summary>
    public enum ExcelVAlign
    {
        靠上 = 1,
        居中,
        靠下,
        两端对齐,
        分散对齐
    }
    /// <summary>
    /// 线粗
    /// </summary>
    public enum BorderWeight
    {
        极细 = 1,
        细 = 2,
        粗 = -4138,
        极粗 = 4
    }
    /// <summary>
    /// 线样式
    /// </summary>
    public enum LineStyle
    {
        连续直线 = 1,
        短线 = -4115,
        线点相间 = 4,
        短线间两点 = 5,
        点 = -4118,
        双线 = -4119,
        无 = -4142,
        少量倾斜点 = 13
    }
    /// <summary>
    /// 下划线方式
    /// </summary>
    public enum UnderlineStyle
    {
        无下划线 = -4142,
        双线 = -4119,
        双线充满全格 = 5,
        单线 = 2,
        单线充满全格 = 4
    }
    /// <summary>
    /// 单元格填充方式
    /// </summary>
    public enum Pattern
    {
        Automatic = -4105,
        Checker = 9,
        CrissCross = 16,
        Down = -4121,
        Gray16 = 17,
        Gray25 = -4124,
        Gray50 = -4125,
        Gray75 = -4126,
        Gray8 = 18,
        Grid = 15,
        Horizontal = -4128,
        LightDown = 13,
        LightHorizontal = 11,
        LightUp = 14,
        LightVertical = 12,
        None = -4142,
        SemiGray75 = 10,
        Solid = 1,
        Up = -4162,
        Vertical = -4166
    }
}


