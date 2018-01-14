using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.IO;

namespace LSSCADA
{
    class KJIcon
    {
        public string IconName = "";    //图标名称
        public ArrayList ListTuYuan = new ArrayList();
        public KJIcon(string sName)
        {
            IconName = sName;
            LoadFromXML();
        }
        public KJIcon Clone()
        {
            KJIcon obj = (KJIcon)this.MemberwiseClone();
            obj.ListTuYuan = new ArrayList();
            foreach (CBase nIcon in ListTuYuan)
            {
                obj.ListTuYuan.Add(nIcon);
            }
            return obj;
        }

        public ArrayList ListClone()
        {
            ArrayList NewListTuYuan = new ArrayList();
            foreach (CBase nIcon in ListTuYuan)
            {
                NewListTuYuan.Add(nIcon);
            }
            return NewListTuYuan;
        }

        public bool LoadFromXML()
        {
            XmlDocument myxmldoc = new XmlDocument();
            string sPath = CProject.sPrjPath + "\\IconLib\\" + IconName + ".yic";
            if (File.Exists(sPath))
            {
                myxmldoc.Load(sPath);
                //取图元
                string xpath = "Root/Misc";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                int i = 0;
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    string sNodeName = item.Name;
                    string str1 = sNodeName.Substring(0, 2);
                    if ((sNodeName.Substring(0, 2) == "TY" || sNodeName.Substring(0, 2) == "KJ") && item.ChildNodes.Count > 0)
                    {
                        foreach (XmlElement TYNode in item.ChildNodes)
                        {
                            switch (TYNode.Name)
                            {
                                case "TYLine":
                                    CElementFactory.SetClassIndex(LCElementType.LINE);
                                    break;
                                case "TYText":
                                    CElementFactory.SetClassIndex(LCElementType.TEXT);
                                    break;
                                case "TYRect":
                                    CElementFactory.SetClassIndex(LCElementType.RECTANGLE);
                                    break;
                                case "TYEllipse":
                                    CElementFactory.SetClassIndex(LCElementType.ELLIPS);
                                    break;
                                case "TYRndRect":
                                    CElementFactory.SetClassIndex(LCElementType.ROUNDRECTANGLE);
                                    break;
                                case "TYArc":
                                    CElementFactory.SetClassIndex(LCElementType.ARC);
                                    break;
                                case "KJHotImage":
                                    CElementFactory.SetClassIndex(LCElementType.IMAGECONTROL);
                                    break;
                                case "KJIcon":
                                    CElementFactory.SetClassIndex(LCElementType.GROUP);
                                    break;
                                default:
                                    continue;
                                //break;
                            }

                            CBase NewOb = CElementFactory.CreateElement(null, this);
                            if (NewOb == null)
                                continue;
                            NewOb.LoadFromXML(TYNode);
                            for (i = ListTuYuan.Count - 1; i > -1; i--)
                            {
                                Object Ob = ListTuYuan[i];
                                Int32 iEO = ((CBase)Ob).iElementOrder;
                                if (NewOb.iElementOrder >= iEO)
                                {
                                    ListTuYuan.Insert(i+1, NewOb);
                                    break;
                                }
                            }
                            if (i == -1)
                                ListTuYuan.Insert(0, NewOb);
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class KJIconList
    {
        public static ArrayList  ListKJIcon = new ArrayList();
        public static void AddKJIcon(string sName)
        {
            foreach (KJIcon nIcon in ListKJIcon)
            {
                if (sName == nIcon.IconName)
                {
                    return;
                }
            }
            KJIcon newIcon = CreateKJIcon(sName);
            if (newIcon != null)
            {
                ListKJIcon.Add(newIcon);
            }
        }

        public static KJIcon CreateKJIcon(string sName)
        {
            KJIcon newObj = new KJIcon(sName);
            if (newObj.LoadFromXML())
            {
                return newObj;
            }
            else
            {
                return null;
            }
        }
    }
}
