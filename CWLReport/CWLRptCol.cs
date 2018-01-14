using CABC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;

namespace CWLReport
{
   public class CWLRptCol
    {
        public string desc = "";//描述
        public string ColType = "";//列数据类型  D一天的数据 M月数据 Y年数据
        public int iRow = 0;   //起始行号
        public int iCol = 0;   //起始列号
        public int iRows = 1;  //总行数
        public int iCols = 1;  //总列数
        public int iRowInt = 1;//间隔行数
        public int iColInt = 1;//间隔列数
        public string iDire = "N";//是否转换 纵横转换

        public List<Point> LsLoaction = new List<Point>();
        public string sLocation //固定的坐标点1,2;2,3;
        {
            get
            {
                string str1 = "";
                for (int i = 0; i < LsLoaction.Count; i++)
                {
                    str1 += string.Format("{0},{1};",LsLoaction[i].X, LsLoaction[i].Y);
                }
                return str1;
            }
            set
            {
                LsLoaction = new List<Point>();
                string[] str1 = value.Split(';');
                for (int i = 0; i < str1.Length; i++)
                {
                    string[] str2 = str1[i].Split(',');
                    if (str2.Length >= 2)
                    {
                        if (CABCSTR.IsNum(str2[0]) && CABCSTR.IsNum(str2[1]))
                        {
                            LsLoaction.Add(new Point(Convert.ToInt32(str2[0]), Convert.ToInt32(str2[1])));
                        }
                    }
                }
            }
        }

        public string sFormat = "";
        public string sSheetName = "";
        public string DataSource = "";
        public string sSQL = "";

        public void LoadFromNode(XmlElement Node)
        {
            try
            {
                desc = CABCXML.GetValFromNode(Node, "desc", "");
                ColType = CABCXML.GetValFromNode(Node, "ColType", "");
                iRow = CABCXML.GetValFromNode(Node, "iRow", 0);
                iCol = CABCXML.GetValFromNode(Node, "iCol", 0);
                iRows = CABCXML.GetValFromNode(Node, "iRows", 1);
                iCols = CABCXML.GetValFromNode(Node, "iCols", 1);
                iRowInt = CABCXML.GetValFromNode(Node, "iRowInt", 1);
                iColInt = CABCXML.GetValFromNode(Node, "iColInt",1);
                iDire = CABCXML.GetValFromNode(Node, "iDire", "N");
                sLocation = CABCXML.GetValFromNode(Node, "sLocation", "");
                sFormat = CABCXML.GetValFromNode(Node, "sFormat", "");
                sSheetName = CABCXML.GetValFromNode(Node, "sSheetName", "");
                DataSource = CABCXML.GetValFromNode(Node, "DataSource", "");
                sSQL = CABCXML.GetValFromNode(Node, "sSQL", "");
            }
            catch (Exception e)
            {

            }
        }
    }
}
