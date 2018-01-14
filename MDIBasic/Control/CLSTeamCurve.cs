using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Drawing2D;
using ZedGraph;
using System.Diagnostics;
using LSSCADA.Database;

namespace LSSCADA.Control
{
    public class CLSTeamCurve
    {
        public string Name = "";
        public List<CLSYAxisGroup> ListYAxisGroup = new List<CLSYAxisGroup>();
        Dictionary<string, string> ListTable = new Dictionary<string, string>();

        public void LoadFromXML(XmlElement Node)
        {
            try
            {
                Name = Node.GetAttribute("Name");
                foreach (XmlElement node in Node.ChildNodes)
                {
                    string str1 = node.GetAttribute("Text");
                    CLSYAxisGroup nYAxis = new CLSYAxisGroup(str1);
                    nYAxis.LoadFromXML(node);
                    ListYAxisGroup.Add(nYAxis);
                }
            }
            catch (Exception e)
            {
            }
        }

        public void SaveToXML(XmlElement Node, XmlDocument MyXmlDoc)
        {
            try
            {
                Node.SetAttribute("Name", Name);
                foreach (CLSYAxisGroup nYAxis in ListYAxisGroup)
                {
                    XmlElement YAxisGroup = MyXmlDoc.CreateElement("YAxisGroup");
                    nYAxis.SaveToXML(YAxisGroup, MyXmlDoc);
                    Node.AppendChild(YAxisGroup);
                }
            }
            catch (Exception e)
            {
            }
        }

        public void UpdateListTable()//更新数据表
        {
            ListTable = new Dictionary<string, string>();
            foreach (CLSYAxisGroup nGroup in ListYAxisGroup)
            {
                foreach (CLSCurve nCurve in nGroup.ListCur)
                {
                    if (ListTable.ContainsKey(nCurve.StaName))//已经存在表
                    {
                        ListTable[nCurve.StaName] += "," + nCurve.VarName;
                    }
                    else
                    {
                        ListTable.Add(nCurve.StaName, "," + nCurve.VarName);
                    }
                }
            }
        }

        public void UpdateReal(DateTime DT_S, DateTime DT_N, int iSec)//按时间长度增加曲线
        {
            if (iSec > 0)
            {
                DT_S = DT_N.AddSeconds(-1 * iSec);
            }

            UpdateListTable();
            foreach (var nTable in ListTable)
            {
                DataTable DTValue = LSDatabase.GetFastHisData(DT_S, DT_N, nTable.Key, nTable.Value);
                DateTime DT_ss;
                if (DTValue.Rows.Count > 0)
                    DT_ss = DateTime.Parse(DTValue.Rows[DTValue.Rows.Count - 1]["Date_Time"].ToString());
                else
                    DT_ss = DT_S;
                double[] x = new double[DTValue.Rows.Count];
                double[] y = new double[DTValue.Rows.Count];
                for (int i = 0; i < DTValue.Rows.Count; i++)
                {
                    x[i] = new XDate(DateTime.Parse(DTValue.Rows[i]["Date_Time"].ToString()));
                    //Debug.WriteLine(DTValue.Rows[i]["Date_Time"].ToString() + ":" + x[i].ToString());
                }
                for (int j = 1; j < DTValue.Columns.Count; j++)
                {
                    foreach (CLSYAxisGroup nGroup in ListYAxisGroup)
                    {
                        foreach (CLSCurve nCurve in nGroup.ListCur)
                        {
                            if (nCurve.StaName == nTable.Key && nCurve.VarName == DTValue.Columns[j].ColumnName)
                            {
                                for (int i = 0; i < DTValue.Rows.Count; i++)
                                {
                                    y[i] =CABC.CABCDGV.GetValFromDT(DTValue, nCurve.VarName, i, 0);
                                }
                                nCurve.ListPT.Clear();
                                nCurve.ListPT.Add(x, y);
                                if (iSec > 0)
                                {
                                    nCurve.iSec = iSec;
                                    nCurve.UpdateReal(DT_ss, DT_N);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UpdateReal(double time)
        {
            foreach (CLSYAxisGroup nGroup in ListYAxisGroup)
            {
                foreach (CLSCurve nCurve in nGroup.ListCur)
                {
                    nCurve.UpdateReal(time);
                }
            }
        }
    }

}
