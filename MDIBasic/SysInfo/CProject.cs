using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;

namespace LSSCADA
{
    public struct WinAtt
    {
        public string Name;
        public int WinType;//0：通用子窗口，1：非模式特定子窗口 2：模式特定子窗口
    }

    public class CProject
    {
        public string Name = "";                //
        public string LogicName = "";        //
        public Int32 ID = 0;        //
        public string Version = "4.0";        //
        public Int32 Edition = 2896;        //
        public string ConfigStatus = "正在组态";//
        public string RunStatus = "闲置";      //

        public bool bShowTopTool = true;

        public static string sPrjPath = "";        //工程路径
        public static string sPrjName = "";        //工程名称

        public List<WinAtt> AFormList = new List<WinAtt>();				//所有窗口集合
        public List<frmChild> AOpenForm = new List<frmChild>();               //所有打开的窗口集合

        public void LoadFromXML()
        {
            sPrjPath = System.Environment.CurrentDirectory;//System.IO.Path.GetDirectoryName(sFilePath);
            string sFilePath = sPrjPath + "\\Project\\Project.xml";
            sPrjName = System.IO.Path.GetFileNameWithoutExtension(sFilePath);
            /// 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            myxmldoc.Load(sFilePath);

            string xpath = "Root/Project";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            Name = childNode.GetAttribute("Name");
            LogicName = childNode.GetAttribute("COName");
            ID = Convert.ToInt32(childNode.GetAttribute("ID"));
            Version = childNode.GetAttribute("Version");
            Edition = Convert.ToInt32(childNode.GetAttribute("Edition"));
            ConfigStatus = childNode.GetAttribute("ConfigStatus");
            RunStatus = childNode.GetAttribute("RunStatus");
            bShowTopTool = Convert.ToBoolean(childNode.GetAttribute("bShowTopTool"));

            xpath = "Root/Project/SysDefaultWin/SysOpenWindows/OpenWin";
            XmlNodeList mynodes = myxmldoc.SelectNodes(xpath);
            AFormList.Clear();
            foreach (XmlElement item in mynodes)
            {
                WinAtt NewOb = new WinAtt();
                NewOb.Name = item.GetAttribute("Name");
                NewOb.WinType = Convert.ToInt32(item.GetAttribute("WinType"));
                AFormList.Add(NewOb);
            }
        }
        public void OpenForm(string sFormName, object _Owner,int iTop)//打开子窗口
        {
            foreach (frmChild item in AOpenForm)
            {
                item.Close();
            }

            frmChild NewForm = new frmChild(sFormName, (Form)_Owner, iTop);
            NewForm.Show();
            AOpenForm.Add(NewForm);
        }

        public void OpenForm(string sFormName, object _Owner,PointF LocationPF)//打开子窗口
        {
            foreach (frmChild item in AOpenForm)
            {
                item.Close();
            }

            frmChild NewForm = new frmChild(sFormName, (Form)_Owner, 0);
            NewForm.cForm.m_Location = LocationPF;
            NewForm.Show();
            AOpenForm.Add(NewForm);
        }

        public void CloseAllForm()//关闭所有子窗口
        {
            foreach (frmChild item in AOpenForm)
            {
                item.Close();
            }
        }

        public void CloseForm(string sFormName, object _Owner)
        {
            foreach (frmChild item in AOpenForm)
            {
                if (item.cForm.Name == sFormName)
                {
                    item.Close();
                    AOpenForm.Remove(item);
                    return;
                }
            }
        }
    }
}
