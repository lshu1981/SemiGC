using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace PublicDll
{
    public class CPublicDGV
    {
        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="DGV">表格控件名</param>
        /// <param name="strHeader">标题字符串数组</param>
        /// <param name="bReadOnly">只读字符串数组</param>
        /// <param name="bAuto">是否自动调整列宽</param>
        /// <returns></returns>
        public static bool InitializeDGV(DataGridView DGV, string[] strHeader ,bool[] bReadOnly,bool bAuto)
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

        public static void DGVAutoCol(DataGridView DGV)
        {
            int width = 0;//定义一个局部变量，用于存储自动调整列宽以后整个DtaGridView的宽度

            for (int i = 0; i < DGV.Columns.Count; i++)//对于DataGridView的每一个列都调整
            {
                DGV.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
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
                {
                    return iRe;
                }

                iRe.Add(dGV1.SelectedCells[0].ColumnIndex);
                foreach (DataGridViewCell nCell in dGV1.SelectedCells)
                {
                    int k = 0;
                    for (int i = 0; i < iRe.Count; i++)
                    {
                        if (nCell.ColumnIndex == iRe[i])
                        {
                            k = 1;
                            break;
                        }
                    }
                    if (k == 1)
                        continue;

                    for (int i = 0; i < iRe.Count; i++)
                    {
                        if (nCell.ColumnIndex < iRe[i])
                        {
                            iRe.Insert(i, nCell.ColumnIndex);
                            k = 2;
                            break;
                        }
                    }
                    if (k == 2)
                        continue;
                    iRe.Add(nCell.ColumnIndex);
                }
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
                {
                    return iRe;
                }

                iRe.Add(dGV1.SelectedCells[0].RowIndex);
                foreach (DataGridViewCell nCell in dGV1.SelectedCells)
                {
                    int k = 0;
                    for (int i = 0; i < iRe.Count; i++)
                    {
                        if (nCell.RowIndex == iRe[i])
                        {
                            k = 1;
                            break;
                        }
                    }
                    if (k == 1)
                        continue;

                    for (int i = 0; i < iRe.Count; i++)
                    {
                        if (nCell.RowIndex < iRe[i])
                        {
                            iRe.Insert(i, nCell.RowIndex);
                            k = 2;
                            break;
                        }
                    }
                    if (k == 2)
                        continue;
                    iRe.Add(nCell.RowIndex);
                }
                return iRe;
            }
            catch (Exception ex)
            {
                return iRe;
            }
        }
    }

    public class CStrPublicFun
    {
        /// <summary>
        /// 获取字符串中的数值字符串
        /// </summary>
        /// <param name="InStr">输入的字符串</param>
        /// <param name="index">字符串序号:输出第几组数值字符串,从0开始</param>
        /// <param name="iMode">顺序：0从左开始 非0从右开始</param>
        /// <returns></returns>
        public static string GetNumberFromStr(string InStr,int index, int iMode)
        {
            string number = null;
            int iIn = -1;
            int iIn2 = -2;
            int iIns = -2;
            int iIne = -2;
            char[] arr = InStr.ToCharArray();

            if (iMode != 0)
                Array.Reverse(arr);//反转
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] >= 48 && arr[i] <= 58)
                {
                    if (i > iIn2 + 1)
                    {
                        iIn++;
                        iIn2 = i;
                        iIns = i;
                    }
                    else if (i == iIn2 + 1)
                    {
                        iIn2 = i;
                    }
                }
                else
                {
                    if (i == iIn2 + 1)
                    {
                        iIne = i;
                        if (index == iIn)
                        {
                            number = "";
                            if (iMode == 0)
                                number = InStr.Substring(iIns, iIne - iIns);
                            else
                                number = InStr.Substring(InStr.Length- iIne, iIne - iIns );
                            return number;
                        }
                    }
                }
            }
            if (index == iIn)
            {
                iIne = InStr.Length ;
                number = "";
                if (iMode == 0)
                    number = InStr.Substring(iIns, iIne - iIns);
                else
                    number = InStr.Substring(InStr.Length - iIne, iIne - iIns);
                return number;
            }
            return number;
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

        public static string Get2StrTo1(string str1,string  str2,int Length,char nchar)
        {
            int i1 = GetLength(str1);

            if (i1 > Length)
                return str1 + str2;
            else
                return str1 + str2.PadLeft(Length - i1, nchar);
        } 
    }
}
