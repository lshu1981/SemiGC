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
    public class CLSCurve
    {
        public string StaName = "";
        public string VarName = "";
        public string Text = "";                            //标题
        public Color LineColor = Color.Red;                 //线色
        public DashStyle LineStyle = DashStyle.Solid;           //线型

        public CVar cVar = new CVar();                      //关联变量
        public int LineWidth = 1;
        public int iYAxis = 0;                              //Y轴序号
        public PointPairList ListPT = new PointPairList();  //点阵

        public int iSec = 600;
        public string StaVarName
        {
            get
            {
                return StaName + "." + VarName;
            }
        }
        public CLSCurve(string strSta, string strVar)
        {
            StaName = strSta;
            VarName = strVar;
            cVar = frmMain.staComm.GetVarByStaNameVarName(strSta, strVar);
            //UpdateReal();
        }

        public void UpdateReal(double time)
        {
            try
            {
                //Debug.WriteLine(time.ToString("f9"));
                if (cVar != null)
                    ListPT.Add(time, cVar.GetDoubleDataValue());
                else
                    ListPT.Add(time, 0);

                while (ListPT.Count > iSec)
                {
                    ListPT.RemoveAt(0);
                }
            }
            catch (Exception ex)
            { }
        }

        public void UpdateReal(DateTime DT_S, DateTime DT_E)
        {
            try
            {
                if (cVar == null)
                    return;
                int iLen = (int)((TimeSpan)(DT_E - DT_S)).TotalSeconds;
                int k = 1;
                for (int i = 1; i < iLen; i++)
                {
                    DateTime DT_N = DT_S.AddSeconds(i);
                    double time = new XDate(DT_N);
                    //Debug.WriteLine(time.ToString());
                    ListPT.Add(time, cVar.GetDoubleValue(DT_N));
                }
            }
            catch (Exception ex)
            { }
        }

        public CLSCurve(CVar nVar)
        {
            cVar = nVar;
            Text = cVar.StaName + "." + cVar.Name + ":" + cVar.Description;
            StaName = cVar.StaName;
            VarName = cVar.Name;

            //UpdateReal();
        }

        public void LoadFromXML(XmlElement Node)
        {
            try
            {
                LineColor = ColorTranslator.FromHtml(Node.GetAttribute("LineColor"));
                Text = Node.GetAttribute("Text");
                LineStyle = (DashStyle)Enum.Parse(typeof(DashStyle), Node.GetAttribute("LineStyle"));
                iYAxis = Convert.ToInt32(Node.GetAttribute("iYAxis"));
                LineWidth = Convert.ToInt32(Node.GetAttribute("LineWidth"));
            }
            catch (Exception e)
            {

            }
        }

        public void SaveToXML(XmlElement Node, XmlDocument MyXmlDoc)
        {
            try
            {
                Node.SetAttribute("iYAxis", iYAxis.ToString());
                Node.SetAttribute("VarName", VarName);
                Node.SetAttribute("StaName", StaName);
                Node.SetAttribute("Text", Text);
                Node.SetAttribute("LineColor", ColorTranslator.ToHtml(LineColor));
                Node.SetAttribute("LineWidth", LineWidth.ToString());
                Node.SetAttribute("LineStyle", ((int)LineStyle).ToString());
            }
            catch (Exception e)
            {
            }
        }
    }
}
