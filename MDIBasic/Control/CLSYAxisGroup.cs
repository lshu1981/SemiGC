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
    public class CLSYAxisGroup//Y轴组
    {
        public List<CLSCurve> ListCur = new List<CLSCurve>();
        public Color FontColor = Color.Red;
        public string Text = "";                            //
        public bool ScaleMinAuto = true;
        public bool ScaleMaxAuto = true;
        public double ScaleMin = 0;
        public double ScaleMax = 100;

        public CLSYAxisGroup(string str1)
        {
            Text = str1;
        }

        public void LoadFromXML(XmlElement Node)
        {
            try
            {
                FontColor = ColorTranslator.FromHtml(Node.GetAttribute("FontColor"));
                ScaleMinAuto = Convert.ToBoolean(Node.GetAttribute("ScaleMinAuto"));
                ScaleMaxAuto = Convert.ToBoolean(Node.GetAttribute("ScaleMaxAuto"));
                ScaleMin = Convert.ToDouble(Node.GetAttribute("ScaleMin"));
                ScaleMax = Convert.ToDouble(Node.GetAttribute("ScaleMax"));
                foreach (XmlElement node in Node.ChildNodes)
                {
                    string StaName = node.GetAttribute("StaName");
                    string VarName = node.GetAttribute("VarName");
                    CLSCurve nCur = new CLSCurve(StaName, VarName);
                    nCur.LoadFromXML(node);
                    ListCur.Add(nCur);
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
                Node.SetAttribute("Text", Text);
                Node.SetAttribute("FontColor", ColorTranslator.ToHtml(FontColor));
                Node.SetAttribute("ScaleMinAuto", ScaleMinAuto.ToString());
                Node.SetAttribute("ScaleMaxAuto", ScaleMaxAuto.ToString());
                Node.SetAttribute("ScaleMin", ScaleMin.ToString());
                Node.SetAttribute("ScaleMax", ScaleMax.ToString());

                foreach (CLSCurve nCur in ListCur)
                {
                    XmlElement CurveCircs = MyXmlDoc.CreateElement("CurveCircs");
                    nCur.SaveToXML(CurveCircs, MyXmlDoc);
                    Node.AppendChild(CurveCircs);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
