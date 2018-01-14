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
    public static class CABCDGV
    {
        /// <summary>
        /// 按规律设定表格单元格
        /// </summary>
        /// <param name="dGV">表格名称</param>
        /// <param name="iCRC">循环行数</param>
        /// <param name="bAdd">是否递增</param>
        public static bool dGVSetByCrc(DataGridView dGV, int iCRC, bool bAdd)
        {
            try
            {
                List<int> LsRow = CABCDGV.GetSelRow(dGV);
                List<int> LsCol = CABCDGV.GetSelCol(dGV);
                if (LsRow.Count < iCRC + 1)
                    return false;
                for (int n = 0; n < iCRC; n++)
                {
                    for (int i = 0; i < LsCol.Count; i++)
                    {
                        string sHead = dGV.Columns[LsCol[i]].HeaderText;
                        string str0 = "";
                        if (dGV.Rows[LsRow[n]].Cells[LsCol[i]].Value != null)
                            str0 = dGV.Rows[LsRow[n]].Cells[LsCol[i]].Value.ToString();
                        string str1 = "";
                        if (bAdd && dGV.Rows[LsRow[n + iCRC]].Cells[LsCol[i]].Value != null)
                            str1 = dGV.Rows[LsRow[n + iCRC]].Cells[LsCol[i]].Value.ToString();
                        else
                            str1 = str0;
                        double dStart = 0, dAdd = 0;
                        int iStart = 0, iLen = 0;
                        string sChar = "";
                        int iRes = CABCSTR.Get2StrChange(str0, str1, ref dStart, ref dAdd, ref  sChar, ref  iStart, ref  iLen);
                        if (iRes == 0)
                            return false;
                        int kStart = 1;
                        if (bAdd)
                            kStart = 2;
                        for (int k = kStart; k < Math.Ceiling(((LsRow.Count / (double)iCRC))); k++)
                        {
                            if (n + k * iCRC >= LsRow.Count)
                                continue;
                            double dVal = dStart + dAdd * k;
                            string sValue = "";
                            if (iRes == 1)
                                sValue = dVal.ToString();
                            else if (iRes == 2)
                            {
                                string sVal = new String('0', iLen) + dVal.ToString();
                                sValue = sChar.Insert(iStart, sVal.Substring(sVal.Length - iLen, iLen));
                            }
                            else if (iRes == 3)
                                sValue = str0;

                            dGV.Rows[LsRow[n + k * iCRC]].Cells[LsCol[i]].Value = sValue;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool dGVRowMove(DataGridView dGV, int dRow0, string sCol)
        {
            try
            {
                bool bRes = dGVRowMove(dGV, dRow0);
                if (bRes)
                {
                    for (int i = 0; i < dGV.RowCount; i++)
                    {
                        dGV.Rows[i].Cells[sCol].Value = i + 1;
                    }
                }
                return bRes;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        /// <summary>
        /// 移动选择的行
        /// </summary>
        /// <param name="dGV">操作的表格</param>
        /// <param name="dRow0">移动值，-1上移1行，-2下移一行，>=0移动到指定行</param>
        /// <returns></returns>
        public static bool dGVRowMove(DataGridView dGV, int dRow0)
        {
            DataGridViewSelectedRowCollection dgvsrc = dGV.SelectedRows;
            if (dgvsrc.Count < 1)
                return false;
            List<int> iRowSel = GetSelRowByFullRow(dGV);

            List<DataGridViewRow> LsRowSel = new List<DataGridViewRow>();

            if (dRow0 == -1)
                dRow0 = iRowSel[0] - 1;
            if (dRow0 == -2)
                dRow0 = iRowSel[iRowSel.Count - 1] + 2;

            if (dRow0 < 0 || dRow0 > dGV.RowCount)
                return false;
            int insRow = -1;
            if (dRow0 < iRowSel[0])
            {
                insRow = dRow0;
            }
            if (dRow0 > iRowSel[iRowSel.Count - 1] + 1)
            {
                insRow = dRow0 - iRowSel.Count;
            }

            if (insRow >= 0)
            {
                dGV.ClearSelection();
                for (int i = iRowSel.Count - 1; i >= 0; i--)
                {
                    LsRowSel.Insert(0, dGV.Rows[iRowSel[i]]);
                    dGV.Rows.RemoveAt(iRowSel[i]);
                }

                for (int i = iRowSel.Count - 1; i >= 0; i--)
                {
                    dGV.Rows.Insert(insRow, LsRowSel[i]);
                    LsRowSel[i].Selected = true;
                }
                return true;
            }
            return false;
        }

        public static string dGVGetWidth(DataGridView dGV)
        {
            string str1 = "";
            for (int i = 0; i < dGV.ColumnCount; i++)
            {
                str1 += dGV.Columns[i].Width.ToString() + ",";
            }
            return str1;
        }

        public static bool DGVDelSelRow(DataGridView DGV)
        {
            try
            {
                for (int i = DGV.SelectedRows.Count; i > 0; i--)
                {
                    //int ID = Convert.ToInt32(DGV.SelectedRows[i - 1].Cells[0].Value);
                    DGV.Rows.RemoveAt(DGV.SelectedRows[i - 1].Index);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="DGV">表格控件名</param>
        /// <param name="strHeader">标题字符串数组</param>
        /// <param name="bReadOnly">只读字符串数组</param>
        /// <param name="bAuto">是否自动调整列宽</param>
        /// <returns></returns>
        public static bool InitializeDGV(DataGridView DGV, string[] strHeader, bool[] bReadOnly, bool bAuto)
        {
            try
            {
                // Create an unbound DataGridView by declaring a column count.
                DGV.ColumnHeadersVisible = true;

                // Create an unbound DataGridView by declaring a column count.
                DGV.ColumnCount = strHeader.Length;
                // DGV.ColumnHeadersVisible = true;

                DGV.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

                // Set the column header style.
                DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

                columnHeaderStyle.BackColor = Color.Beige;
                columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
                DGV.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

                // Set the column header names.
                for (int i = 0; i < strHeader.Length; i++)
                {
                    DGV.Columns[i].Name = strHeader[i];
                    if (bAuto)
                        DGV.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                for (int i = 0; i < Math.Min(strHeader.Length, bReadOnly.Length); i++)
                {
                    DGV.Columns[i].ReadOnly = bReadOnly[i];
                    if (DGV.Columns[i].ReadOnly)
                        DGV.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    else
                        DGV.Columns[i].DefaultCellStyle.BackColor = Color.White;
                }

                // Resize the height of the column headers. 
                //        DGV.AutoResizeColumnHeadersHeight();
                // Resize all the row heights to fit the contents of all non-header cells.

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="DGV">表格控件名</param>
        /// <param name="strHeader">标题字符串数组</param>
        /// <param name="bReadOnly">只读字符串数组</param>
        /// <param name="bAuto">是否自动调整列宽</param>
        /// <returns></returns>
        public static bool InitializeDGV(DataGridView DGV, List<string> strHeader, bool[] bReadOnly, bool bAuto)
        {
            string[] sHead = strHeader.ToArray();
            return InitializeDGV(DGV, sHead, bReadOnly, bAuto);
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="DGV">表格控件名</param>
        /// <param name="strHeader">标题字符串数组，以';'区分</param>
        /// <param name="bReadOnly">只读字符串数组，01组成的字符串</param>
        /// <param name="bAuto">是否自动调整列宽</param>
        /// <returns></returns>
        public static bool InitializeDGV(DataGridView DGV, List<string> strHeader, string sReadOnly, bool bAuto)
        {
            string[] sHead = strHeader.ToArray();
            bool[] bReadOnly = new bool[sReadOnly.Length];
            for (int i = 0; i < bReadOnly.Length; i++)
            {
                if (Convert.ToInt32(sReadOnly.Substring(i, 1)) > 0)
                    bReadOnly[i] = true;
            }
            return InitializeDGV(DGV, sHead, bReadOnly, bAuto);
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="DGV">表格控件名</param>
        /// <param name="strHeader">标题字符串数组，以';'区分</param>
        /// <param name="bReadOnly">只读字符串数组，01组成的字符串</param>
        /// <param name="bAuto">是否自动调整列宽</param>
        /// <returns></returns>
        public static bool InitializeDGV(DataGridView DGV, string strHeader, string sReadOnly, bool bAuto)
        {
            try
            {
                string str1 = strHeader.Replace('；', ';');
                string[] sHeader = str1.Split(';');
                bool[] bReadOnly = new bool[sReadOnly.Length];
                for (int i = 0; i < bReadOnly.Length; i++)
                {
                    if (Convert.ToInt32(sReadOnly.Substring(i, 1)) > 0)
                        bReadOnly[i] = true;
                }
                return InitializeDGV(DGV, sHeader, bReadOnly, bAuto);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="DGV">表格控件名</param>
        /// <param name="strHeader">标题字符串数组，以';'区分</param>
        /// <param name="bReadOnly">只读字符串数组，01组成的字符串</param>
        /// <param name="bAuto">是否自动调整列宽</param>
        /// <returns></returns>
        public static bool InitializeDGV(DataGridView DGV, string[] strHeader, string sReadOnly, bool bAuto)
        {
            try
            {
                bool[] bReadOnly = new bool[sReadOnly.Length];
                for (int i = 0; i < bReadOnly.Length; i++)
                {
                    if (Convert.ToInt32(sReadOnly.Substring(i, 1)) > 0)
                        bReadOnly[i] = true;
                }
                return InitializeDGV(DGV, strHeader, bReadOnly, bAuto);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="DGV">表格控件名</param>
        /// <param name="strHeader">标题字符串数组，以';'区分</param>
        /// <param name="bReadOnly">只读字符串数组，01组成的字符串 1表示只读</param>
        /// <param name="iWidth">列宽</param>
        /// <returns></returns>
        /// 
        public static bool InitializeDGV(DataGridView DGV, List<string> strHeader, string sReadOnly, int[] iWidth)
        {
            string[] sHead = strHeader.ToArray();
            bool[] bReadOnly = new bool[sReadOnly.Length];
            for (int i = 0; i < bReadOnly.Length; i++)
            {
                if (Convert.ToInt32(sReadOnly.Substring(i, 1)) > 0)
                    bReadOnly[i] = true;
            }
            return InitializeDGV(DGV, sHead, bReadOnly, iWidth);
        }
        public static bool InitializeDGV(DataGridView DGV, List<string> strHeader, bool[] bReadOnly, int[] iWidth)
        {
            string[] sHead = strHeader.ToArray();
            return InitializeDGV(DGV, sHead, bReadOnly, iWidth);
        }
        public static bool InitializeDGV(DataGridView DGV, string strHeader, string sReadOnly, int[] iWidth)
        {
            try
            {
                string str1 = strHeader.Replace('；', ';');
                string[] sHeader = str1.Split(';');
                bool[] bReadOnly = new bool[sReadOnly.Length];
                for (int i = 0; i < bReadOnly.Length; i++)
                {
                    if (Convert.ToInt32(sReadOnly.Substring(i, 1)) > 0)
                        bReadOnly[i] = true;
                }
                return InitializeDGV(DGV, sHeader, bReadOnly, iWidth);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool InitializeDGV(DataGridView DGV, string[] strHeader, bool[] bReadOnly, int[] iWidth)
        {
            try
            {
                // Create an unbound DataGridView by declaring a column count.
                DGV.ColumnHeadersVisible = true;

                // Create an unbound DataGridView by declaring a column count.
                DGV.ColumnCount = strHeader.Length;
                // DGV.ColumnHeadersVisible = true;

                DGV.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

                // Set the column header style.
                DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();

                columnHeaderStyle.BackColor = Color.Beige;
                columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
                DGV.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

                // Set the column header names.
                for (int i = 0; i < strHeader.Length; i++)
                {
                    DGV.Columns[i].Name = strHeader[i];
                }
                for (int i = 0; i < Math.Min(strHeader.Length, iWidth.Length); i++)
                {
                    if (iWidth[i] >= 0)
                        DGV.Columns[i].Width = iWidth[i];
                }

                for (int i = 0; i < Math.Min(strHeader.Length, bReadOnly.Length); i++)
                {
                    DGV.Columns[i].ReadOnly = bReadOnly[i];
                    if (DGV.Columns[i].ReadOnly)
                        DGV.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    else
                        DGV.Columns[i].DefaultCellStyle.BackColor = Color.White;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 列宽自适应模式
        /// </summary>
        /// <param name="DGV">表格名</param>
        /// <param name="bHeader">是否包含标题</param>
        public static void DGVAutoCol(DataGridView DGV, bool bHeader)
        {
            int width = 0;//定义一个局部变量，用于存储自动调整列宽以后整个DtaGridView的宽度

            for (int i = 0; i < DGV.Columns.Count; i++)//对于DataGridView的每一个列都调整
            {
                if (bHeader)
                    DGV.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
                else
                    DGV.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader);//将每一列都调整为自动适应模式

                width += DGV.Columns[i].Width;//记录整个DataGridView的宽度
            }
            if (width > DGV.Size.Width)//判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，则将DataGridView的列自动调整模式设置为显示的列即可，如果是小于原来设定的宽度，将模式改为填充。
            {
                DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        /// <summary>
        /// 返回单元格值，如果没有返回默认值
        /// </summary>
        /// <param name="ncell">单元格</param>
        /// <param name="s0">默认值</param>
        /// <returns>返回值</returns>
        public static float GetValueFromCell(DataGridViewCell ncell, float s0)
        {
            string s1 = GetValueFromCell(ncell, "0");
            if (CABCSTR.IsFloat(s1))
                return Convert.ToSingle(s1);
            else
                return 0;
        }

        /// <summary>
        /// 返回单元格值，如果没有返回默认值
        /// </summary>
        /// <param name="ncell">单元格</param>
        /// <param name="s0">默认值</param>
        /// <returns>返回值</returns>
        public static int GetValueFromCell(DataGridViewCell ncell, int s0)
        {
            string s1 = GetValueFromCell(ncell, "0");
            if (CABCSTR.IsNum(s1))
                return Convert.ToInt32(s1);
            else
                return 0;
        }

        /// <summary>
        /// 隐藏字段
        /// </summary>
        /// <param name="dGV"></param>
        /// <param name="LsHide"></param>
        public static void SetHideCol(DataGridView dGV, List<string>LsHide)
        {
            for (int i = 0; i < dGV.ColumnCount; i++)
            {
                if (LsHide.Contains(dGV.Columns[i].HeaderText))
                    dGV.Columns[i].Visible = false;
                else
                    dGV.Columns[i].Visible = true;
            }
        }

        /// <summary>
        /// 返回单元格值，如果没有返回默认值
        /// </summary>
        /// <param name="ncell">单元格</param>
        /// <param name="s0">默认值</param>
        /// <returns>返回值</returns>
        public static string GetValueFromCell(DataGridViewCell ncell, string s0)
        {
            if (ncell.Value == null) return s0;
            else return ncell.Value.ToString();
        }

        /// <summary>
        /// 返回单元格值，如果没有返回默认值
        /// </summary>
        /// <param name="ncell">单元格</param>
        /// <param name="s0">默认值</param>
        /// <returns>返回值</returns>
        public static string GetValueFromCell(DataGridViewRow nRow,string sHead, string s0)
        {
            return GetValueFromCell(nRow.Cells[sHead], s0);
        }

        /// <summary>
        /// 返回单元格值，如果没有返回默认值
        /// </summary>
        /// <param name="ncell">单元格</param>
        /// <param name="s0">默认值</param>
        /// <returns>返回值</returns>
        public static string GetValueFromCell(DataGridViewRow nRow, string sHead)
        {
            return GetValueFromCell(nRow.Cells[sHead], "");
        }

        /// <summary>
        /// 获取表格内容
        /// </summary>
        /// <param name="dGV">表格</param>
        /// <param name="iRow">行号</param>
        /// <param name="sHead">标题</param>
        /// <param name="s0">默认值</param>
        /// <param name="sMsg">返回信息</param>
        /// <returns></returns>
        public static string[] GetValueFromCell(DataGridView dGV, int iRow, string sHead, string s0, ref string sMsg)
        {
            string str1 = sHead.Replace('；', ';');
            string[] sHeader = str1.Split(';');

            string str2 = s0.Replace('；', ';');
            string[] s0er = str2.Split(';');

            List<string> LsHead = new List<string>();
            List<string> LsS0 = new List<string>();
            for (int i = 0; i < sHeader.Length; i++)
            {
                if (sHeader[i].Length > 0)
                {
                    LsHead.Add(sHeader[i]);
                    if (s0er.Length > i)
                        LsS0.Add(s0er[i]);
                    else
                        LsS0.Add("");
                }
            }
            string[] sRe = GetValueFromCell(dGV, iRow, LsHead, LsS0, ref sMsg);
            return sRe;
        }
        /// <summary>
        /// 获取表格内容
        /// </summary>
        /// <param name="dGV">表格</param>
        /// <param name="iRow">行号</param>
        /// <param name="sHead">标题</param>
        /// <param name="s0">默认值</param>
        /// <param name="sMsg">返回信息</param>
        /// <returns></returns>
        public static string[] GetValueFromCell(DataGridView dGV, int iRow, List<string> sHead, List<string> s0, ref string sMsg)
        {
            sMsg = "";
            string[] str1 = new string[sHead.Count];
            if(iRow<0 || iRow >= dGV.RowCount)  
            {
                sMsg = string.Format("行序号{0}溢出",iRow);
                return str1;
            }
            DataGridViewRow nRow = dGV.Rows[iRow];
            for (int i = 0; i < sHead.Count; i++)
            {

                if (dGV.Columns.Contains(sHead[i]))
                {
                    if (s0 != null && s0.Count > i)
                        str1[i] = GetValueFromCell(nRow.Cells[sHead[i]], s0[i]);
                    else
                        str1[i] = GetValueFromCell(nRow.Cells[sHead[i]], "");
                }
                else
                {
                    sMsg += string.Format("表格中不存在列标题{0}！", sHead[i]);
                }
            }
            return str1;
        }

        public static int[] GetSelState(DataGridView dGV1)
        {
            int[] iRe = new int[4] { -1, -1, -1, -1 };
            try
            {
                if (dGV1.SelectedCells.Count <= 0)
                {
                    return iRe;
                }

                iRe[0] = dGV1.SelectedCells[0].ColumnIndex;
                iRe[1] = dGV1.SelectedCells[0].ColumnIndex;
                iRe[2] = dGV1.SelectedCells[0].RowIndex;
                iRe[3] = dGV1.SelectedCells[0].RowIndex;
                foreach (DataGridViewCell nCell in dGV1.SelectedCells)
                {
                    iRe[0] = Math.Min(iRe[0], nCell.ColumnIndex);
                    iRe[1] = Math.Max(iRe[1], nCell.ColumnIndex);
                    iRe[2] = Math.Min(iRe[2], nCell.RowIndex);
                    iRe[3] = Math.Max(iRe[3], nCell.RowIndex);
                }
                return iRe;
            }
            catch (Exception ex)
            {
                return iRe;
            }
        }

        public static List<int> GetSelCol(DataGridView dGV1)
        {
            List<int> iRe = new List<int>();
            try
            {
                if (dGV1.SelectedCells.Count <= 0)
                    return iRe;

                iRe.Add(dGV1.SelectedCells[0].ColumnIndex);
                foreach (DataGridViewCell nCell in dGV1.SelectedCells)
                {
                    if (!iRe.Contains(nCell.ColumnIndex))
                        iRe.Add(nCell.ColumnIndex);
                }
                iRe.Sort();
                return iRe;
            }
            catch (Exception ex)
            {
                return iRe;
            }
        }

        public static List<int> GetSelRow(DataGridView dGV1)
        {
            List<int> iRe = new List<int>();
            try
            {
                if (dGV1.SelectedCells.Count <= 0)
                    return iRe;

                iRe.Add(dGV1.SelectedCells[0].RowIndex);
                foreach (DataGridViewCell nCell in dGV1.SelectedCells)
                {
                    if (!iRe.Contains(nCell.RowIndex))
                        iRe.Add(nCell.RowIndex);
                }
                iRe.Sort();
                return iRe;
            }
            catch (Exception ex)
            {
                return iRe;
            }
        }

        public static List<int> GetSelRowByFullRow(DataGridView dGV1)
        {
            List<int> iRe = new List<int>();
            try
            {
                if (dGV1.SelectedRows.Count <= 0)
                {
                    return iRe;
                }
                iRe.Sort();
                iRe.Add(dGV1.SelectedRows[0].Index);
                foreach (DataGridViewRow nCell in dGV1.SelectedRows)
                {
                    if (!iRe.Contains(nCell.Index))
                        iRe.Add(nCell.Index);
                }
                iRe.Sort();
                return iRe;
            }
            catch (Exception ex)
            {
                return iRe;
            }
        }

        /// <summary>
        /// 显示信息提示
        /// </summary>
        /// <param name="richText1">信息显示控件</param>
        /// <param name="sText">要显示的信息文本</param>
        /// <param name="bError">是否红色显示</param>
        public static void InsertRichMsg(RichTextBox richText1, string sText)
        {
            InsertRichMsg(richText1, sText, false, 1000);
        }

        public static void ColAuto(DataGridView dGV)
        {
            for (int i = 0; i < dGV.Columns.Count; i++)//对于DataGridView的每一个列都调整
            {
                dGV.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
            }
        }

        /// <summary>
        /// 显示信息提示
        /// </summary>
        /// <param name="richText1">信息显示控件</param>
        /// <param name="sText">要显示的信息文本</param>
        /// <param name="bError">是否红色显示</param>
        public static void InsertRichMsg(RichTextBox richText1, string sText, bool bError)
        {
            InsertRichMsg(richText1, sText, bError, 1000);
        }
        /// <summary>
        /// 显示信息提示
        /// </summary>
        /// <param name="richText1">信息显示控件</param>
        /// <param name="sText">要显示的信息文本</param>
        /// <param name="bError">是否红色显示</param>
        /// <param name="MaxLines">最大显示条数</param>
        public static void InsertRichMsg(RichTextBox richText1, string sText, bool bError, int MaxLines)
        {
            //iMsg++;
            //richText1.AppendText( iMsg.ToString() + ":" + sText );
            int len1 = richText1.Text.Length;
            string ss = richText1.Lines.Count().ToString() + ". " + DateTime.Now.ToString("HH:mm:ss ") + sText + "\n";
            richText1.AppendText(ss);
            int len2 = richText1.Text.Length;

            if (len2 > len1)
            {
                richText1.Select(len1, len2 - len1 - 1);
                if (bError)
                    richText1.SelectionColor = Color.Red;
                else
                    richText1.SelectionColor = Color.Black;
                richText1.Select(richText1.TextLength, 0);
                richText1.ScrollToCaret();
            }
            if (richText1.Lines.Length > MaxLines)
            {
                richText1.Select(0, richText1.Lines[0].Length + 1);
                richText1.SelectedText = "";
            }
        }

        /// <summary>
        /// 获取被选区域
        /// </summary>
        /// <param name="dGV1"></param>
        /// <returns>int[0] minR  int[1]minC int[2]maxR int[3]maxC</returns>
        public static int[] GetSelMinRCMaxRC(DataGridView dGV1)
        {
            int[] iRe = new int[] { -1, -1, -1, -1 };
            try
            {
                if (dGV1.SelectedCells.Count <= 0)
                    return iRe;

                iRe[0] = dGV1.SelectedCells[0].RowIndex;
                iRe[1] = dGV1.SelectedCells[0].ColumnIndex;
                iRe[2] = dGV1.SelectedCells[0].RowIndex;
                iRe[3] = dGV1.SelectedCells[0].ColumnIndex;
                foreach (DataGridViewCell nCell in dGV1.SelectedCells)
                {
                    iRe[0] = Math.Min(iRe[0], nCell.RowIndex);
                    iRe[1] = Math.Min(iRe[1], nCell.ColumnIndex);
                    iRe[2] = Math.Max(iRe[2], nCell.RowIndex);
                    iRe[3] = Math.Max(iRe[3], nCell.ColumnIndex);
                }
                return iRe;
            }
            catch (Exception ex)
            {
                return iRe;
            }
        }


        /// <summary>
        /// 粘贴表格
        /// </summary>
        /// <param name="dgv">表格控件</param>
        /// <param name="LsGDCol">固定表格列</param>
        public static void PasteToDGV(DataGridView dgv, List<int> LsGDCol, ref string sMsg)
        {
            try
            {
                int[] iRC = CABCDGV.GetSelMinRCMaxRC(dgv);
                if (iRC[0] < 0 || iRC[1] < 0)
                {
                    return;
                }
                //获取剪贴板内容
                string pasteText = Clipboard.GetText();
                //判断是否有字符存在
                if (string.IsNullOrEmpty(pasteText))
                    return;
                //以换行符分割的数组
                pasteText = pasteText.Trim().Replace("\r\n", "\n");
                string[] lines = pasteText.Trim().Split('\n');
                List<List<string>> lsPaste = new List<List<string>>();

                for (int j = 0; j < lines.Length; j++)
                {
                    string[] vals = lines[j].Split('\t');
                    lsPaste.Add(vals.ToList());
                }
                foreach (DataGridViewCell nCell in dgv.SelectedCells)
                {
                    if (LsGDCol != null && LsGDCol.Contains(nCell.ColumnIndex))
                        continue;

                    int iR = nCell.RowIndex - iRC[0];
                    int iC = nCell.ColumnIndex - iRC[1];
                    if (iR < lsPaste.Count)
                    {
                        if (iC < lsPaste[iR].Count)
                        {
                            nCell.Value = lsPaste[iR][iC];
                        }
                    }
                }
            }
            catch (Exception MyEx)
            {
                sMsg = "粘贴出错：" + MyEx.Message;
            }
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static string GetValFromDT(DataSet ds, string Name, int irow, string sValue)
        {
            return GetValFromDT(ds, 0, Name, irow, sValue);
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static string GetValFromDT(DataSet ds,int index, string Name, int irow, string sValue)
        {
            try
            {
                DataTable dt;
                if(index<ds.Tables.Count)
                    dt = ds.Tables[index];
                else
                    dt = ds.Tables[0];
                if (dt == null)
                    return sValue;

                string ss = "";
                if (ds.Tables[0].Columns.Contains(Name))
                    ss = ds.Tables[0].Rows[irow][Name].ToString();
                if (sValue.Length > 0 && ss.Length <= 0)
                    return sValue;
                else
                    return ss;
            }
            catch
            {
                return sValue;
            }
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static float GetValFromDT(DataSet ds, string Name, int irow, float sValue)
        {
            return GetValFromDT(ds, 0, Name, irow, sValue);
        }
        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static float GetValFromDT(DataSet ds, int index, string Name, int irow, float sValue)
        {
            try
            {
                DataTable dt;
                if (index < ds.Tables.Count)
                    dt = ds.Tables[index];
                else
                    dt = ds.Tables[0];
                if (dt == null)
                    return sValue;

                string ss = "";
                if (ds.Tables[0].Columns.Contains(Name))
                    ss = ds.Tables[0].Rows[irow][Name].ToString();
                if (CABCSTR.IsFloat(ss) || CABCSTR.IsNum(ss))
                    return Convert.ToSingle(ss);
                else
                    return sValue;
            }
            catch
            {
                return sValue;
            }
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static float GetValFromDT(DataTable dt, string Name, int irow, float sValue)
        {
            try
            {
                if (dt == null)
                    return sValue;

                string ss = "";
                if (dt.Columns.Contains(Name))
                    ss = dt.Rows[irow][Name].ToString();
                if (CABCSTR.IsFloat(ss) || CABCSTR.IsNum(ss))
                    return Convert.ToSingle(ss);
                else
                    return sValue;
            }
            catch
            {
                return sValue;
            }
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static int GetValFromDT(DataSet ds, string Name, int irow, int sValue)
        {
            return GetValFromDT(ds, 0, Name, irow, sValue);
        }
        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static int GetValFromDT(DataSet ds, int index, string Name, int irow, int sValue)
        {
            try
            {
                DataTable dt;
                if (index < ds.Tables.Count)
                    dt = ds.Tables[index];
                else
                    dt = ds.Tables[0];
                if (dt == null)
                    return sValue;

                string ss = "";
                if (ds.Tables[0].Columns.Contains(Name))
                    ss = ds.Tables[0].Rows[irow][Name].ToString();
                if (CABCSTR.IsFloat(ss) || CABCSTR.IsNum(ss))
                    return (int) Convert.ToSingle(ss);
                else
                    return sValue;
            }
            catch
            {
                return sValue;
            }
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static DateTime? GetDTValFromDT(DataSet ds, string Name, int irow, DateTime? sValue)
        {
            return GetDTValFromDT(ds, 0, Name, irow, sValue);
        }
        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static DateTime? GetDTValFromDT(DataSet ds, int index, string Name, int irow, DateTime? sValue)
        {
            try
            {
                DataTable dt;
                if (index < ds.Tables.Count)
                    dt = ds.Tables[index];
                else
                    dt = ds.Tables[0];
                if (dt == null)
                    return sValue;

                string ss = "";
                if (ds.Tables[0].Columns.Contains(Name))
                    ss = ds.Tables[0].Rows[irow][Name].ToString();

                DateTime dateTime = new DateTime();
                bool convertResult = DateTime.TryParse(ss, out dateTime);

                if (!convertResult)
                    return sValue;
                else
                    return dateTime;
            }
            catch
            {
                return sValue;
            }
        }

        /// <summary>
        /// 获取表格没有隐藏的列名，所有可以看到的列
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        public static List<int> GetDGVShowCol(DataGridView dgv)
        {
            return GetDGVShowCol(dgv,new List<string>(),true);
        }

        /// <summary>
        /// 获取表格没有隐藏的列名，根据输入的隐藏列名队列排除
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="LsHideCol"></param>
        /// <returns></returns>
        public static List<int> GetDGVShowCol(DataGridView dgv, List<string> LsHideCol)
        {
            return GetDGVShowCol(dgv, LsHideCol, false);
        }
        /// <summary>
        /// 获取列表没有隐藏的列名，根据输入的隐藏列名队列排除
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="LsHideCol">固定隐藏的列名</param>
        /// <param name="bHide">是否判断列是否隐藏</param>
        /// <returns></returns>
        public static List<int> GetDGVShowCol(DataGridView dgv, List<string> LsHideCol,bool bHide)
        {
            List<int> LsShow = new List<int>();
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (LsHideCol.Contains(dgv.Columns[i].HeaderText) || (bHide && dgv.Columns[i].Visible == false))
                {
                }
                else
                    LsShow.Add(i);
            }
            return LsShow;
        }
        /// <summary>
        /// 根据列名队列获取列序号
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="LsCol"></param>
        /// <returns></returns>
        public static List<int> GetDGVColsIndex(DataGridView dgv, List<string> LsCol)
        {
            List<int> LsShow = new List<int>();
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (LsCol.Contains(dgv.Columns[i].HeaderText))
                    LsShow.Add(i);
            }
            return LsShow;
        }

        /// <summary>
        /// 根据列名字符串获取列序号，可以通过';'隔开
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="sCol"></param>
        /// <returns></returns>
        public static List<int> GetDGVColsIndex(DataGridView dgv,string sCol)
        {
            List<string> ls = new List<string>();
            string[] str2 = sCol.Split(';');
            for (int i = 0; i < str2.Length; i++)
            {
                if (str2[i].Length > 0)
                    ls.Add(str2[i]);
            }
            return GetDGVColsIndex(dgv, ls);
        }
        /// <summary>
        /// 根据列名字符串获取列序号，可以通过';'隔开
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="sCol"></param>
        /// <returns></returns>
        public static int GetDGVColIndex(DataGridView dgv, string sCol)
        {
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (sCol == dgv.Columns[i].HeaderText)
                   return i;
            }
            return -1;
        }

        public static void dGV1CloneTOdGV2(DataGridView dGV1, ref DataGridView dGV2)
        {
            if (dGV2.ColumnCount != dGV1.ColumnCount)
            {
                dGV2.ColumnCount = dGV1.ColumnCount;
                for (int i = 0; i < dGV1.ColumnCount; i++)
                {
                    dGV2.Columns[i].HeaderText = dGV1.Columns[i].HeaderText;
                    dGV2.Columns[i].Name = dGV1.Columns[i].Name;
                    dGV2.Columns[i].Visible = dGV1.Columns[i].Visible;
                }
            }
            dGV2.Rows.Clear();
            foreach (DataGridViewRow nRow in dGV1.Rows)
            {
                dGV2.RowCount++;
                DataGridViewRow nRow2 = dGV2.Rows[dGV2.RowCount - 1];
                for (int i = 0; i < dGV1.ColumnCount; i++)
                {
                    nRow2.Cells[i].Value = nRow.Cells[i].Value;
                }
            }
        }

        public static void Sort(this DataGridView dgv, Comparison<DataGridViewRow> comparison)
        {
            dgv.Sort(new RowCompare(comparison));
        }
        public class RowCompare : IComparer
        {
            Comparison<DataGridViewRow> comparison;
            public RowCompare(Comparison<DataGridViewRow> comparison)
            {
                this.comparison = comparison;
            }

            #region IComparer 成员  

            public int Compare(object x, object y)
            {
                return comparison((DataGridViewRow)x, (DataGridViewRow)y);
            }

            #endregion
        }
    }
}
