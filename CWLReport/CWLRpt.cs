using CABC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CWLReport
{
  public  class CWLRpt
    {
        public CWLRptDir PDir;
        public string key = "Rpt0101";      //关键字，用于点击选取用
        public string desc = "日报";//报表名称，可以带日期格式参数
        public string RptPath = "日报YYYYMM\\";//保存存储的目录，可以带日期格式参数
        public string RptType = "D";            //报表类型  D日报表，M月报表，Y年报表
        public string AutoType = "Y";           //是否是自动运行报表 Y是自动运行 非Y不自动运行
        public DateTime StartTime = new DateTime(2001,1,1,3,0,0);//报表起始时间
        public string iRows = "50";             //报表总行数
        public string iCols = "27";             //报表总列数
        public DateTime CreateTime = new DateTime(2001, 1, 1, 3, 0, 0);//创建时间
        public DateTime ModifyTime = new DateTime(2001, 1, 1, 3, 0, 0);//修改时间
        public string FilePath = "2站运行日报.xlsx";//模板名称
        public List<CWLRptCol> LsCol = new List<CWLRptCol>();
        public void LoadFromNode(XmlElement Node)
        {
            try
            {
                desc = CABCXML.GetValFromNode(Node, "desc", "");
                RptPath = CABCXML.GetValFromNode(Node, "RptPath", "");
                RptType = CABCXML.GetValFromNode(Node, "RptType", "");
                AutoType = CABCXML.GetValFromNode(Node, "AutoType", "");
                StartTime = CABCXML.GetValFromNode(Node, "StartTime", new DateTime(2001, 1, 1, 3, 0, 0));
                iRows = CABCXML.GetValFromNode(Node, "iRows", "");
                iCols = CABCXML.GetValFromNode(Node, "iCols", "");
                CreateTime = CABCXML.GetValFromNode(Node, "CreateTime", DateTime.Now);
                ModifyTime = CABCXML.GetValFromNode(Node, "ModifyTime", DateTime.Now);
                FilePath = CABCXML.GetValFromNode(Node, "FilePath", "");
                foreach (XmlElement item in Node.ChildNodes)
                {
                    CWLRptCol ncol = new CWLRptCol();
                    ncol.LoadFromNode(item);
                    LsCol.Add(ncol);
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
