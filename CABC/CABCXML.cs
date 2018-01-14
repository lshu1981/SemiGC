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
    public class CABCXML
    {
        /// <summary>
        /// 获取工程名称
        /// 先获取Project目录下的文件夹ListPrj，然后从RunConfig.xml读取当前运行的工程名称
        /// 如果RunConfig.xml里的工程名称不在ListPrj里，将ListPrj的第一个项目设为工程名称
        /// 如果RunConfig.xml文件不存在，新建RunConfig.xml
        /// </summary>
        /// <returns></returns>
        public static string GetPrjName(string sCurDir)
        {
            string ssPrjName = "";
            List<string> ListPrj = new List<string>();
            try
            {
                //string sCurDir = System.Environment.CurrentDirectory;//System.IO.Path.GetDirectoryName(sFilePath);
                ListPrj = GetPrjList(sCurDir + "\\Project");
                if (ListPrj == null || ListPrj.Count < 1)
                {
                    return "";
                }
                string sFilePath = sCurDir + "\\Project\\RunConfig.xml";
                /// 创建XmlDocument类的实例
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sFilePath);

                string xpath = "Servers/ProjectName";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                ssPrjName = childNode.GetAttribute("ProjectName");
                if (!ListPrj.Contains(ssPrjName))
                    ssPrjName = ListPrj[0];
                return ssPrjName;
            }
            catch (Exception ex)
            {
                if (ListPrj.Count > 0)
                {
                    ssPrjName = ListPrj[0];
                    return ssPrjName;
                }
                else
                    MessageBox.Show(ex.Message, "CProject.GetPrjName");
            }
            return "";
        }

        /// <summary>
        /// 返回工程目录PrjList
        /// </summary>
        /// <param name="sPath">工程根目录</param>
        /// <returns>返回工程目录PrjList</returns>
        public static List<string> GetPrjList(string sPath)
        {
            List<string> ListPrj = new List<string>();
            try
            {
                string[] fn = Directory.GetDirectories(sPath);
                foreach (string s in fn)
                {
                    ListPrj.Add(Path.GetFileName(s));
                }

                return ListPrj;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LoadTableFromXml");
            }
            return null;
        }
       
        public static string GetVarTypeName(int VarType)
        {
            string sName = ((int)VarType).ToString("D2");
            switch (VarType)
            {
                case 1:
                    sName += "整型";
                    break;
                case 2:
                    sName += "无符号整型";
                    break;
                case 3:
                    sName += "长整型";
                    break;
                case 4:
                    sName += "无符号长整型";
                    break;
                case 5:
                    sName += "浮点型";
                    break;
                case 6:
                    sName += "双精度型";
                    break;
                case 7:
                    sName += "字符型";
                    break;
                case 8:
                    sName += "无符号字符型";
                    break;
                case 9:
                    sName += "布尔型";
                    break;
                case 10:
                    sName += "字符串型";
                    break;
                case 11:
                    sName += "二进制型";
                    break;
                default:
                    sName += "类型不确定";
                    break;
            }
            return sName;
        }
      

        public static string GetDATypeName(string sVal)
        {
            try
            {
                int iVal = Convert.ToInt32(sVal.Substring(0, 1));
                switch (iVal)
                {
                    case 1: return "YC遥测";
                    case 2: return "YX遥信";
                    case 3: return "YT遥调";
                    case 4: return "YK遥控";
                    case 5: return "YM遥脉";
                    case 6: return "YS其他";
                    default:
                        return "YS其他";
                }
            }
            catch
            {
                switch (sVal.Substring(0, 2).ToUpper())
                {
                    case "YC": return "YC遥测";
                    case "YX": return "YX遥信";
                    case "YT": return "YT遥调";
                    case "YK": return "YK遥控";
                    case "YM": return "YM遥脉";
                    case "YS": return "YS其他";
                    default:
                        return "YS其他";
                }
            }
        }

        public static string GetRWEnableName(int iVal)
        {
            switch (iVal)
            {
                case 0: return "0内存量";
                case 1: return "1可读";
                case 2: return "2可写";
                case 3: return "3可读写";
                default: return "3可读写";
            }
        }

        public static string GetRWEnableByName(int iVal)
        {
            switch (iVal)
            {
                case 0: return "0内存量";
                case 1: return "1可读";
                case 2: return "2可写";
                case 3: return "3可读写";
                default: return "3可读写";
            }
        }

        public static string GetSaveToDBName(int iVal)
        {
            try
            {
                string[] Ls = new string[] { "快", "中", "慢" };
                string sRes = iVal.ToString();
                for (int i = 0; i < 3; i++)
                {
                    int kkk = iVal & (int)Math.Pow(2, i);
                    if (kkk > 0)
                    {
                        sRes += "," + Ls[i];
                    }
                }
                return sRes;
            }
            catch
            {
                return iVal.ToString() + "不识别";
            }
        }

        /// <summary>
        /// 创建xml文档
        /// </summary>
        /// <param name="sPath"></param>
        public static void CreateNewXML(string sPath)
        {
            try
            {
                string sDir = System.IO.Path.GetDirectoryName(sPath);
                if (!System.IO.Directory.Exists(sDir))
                    System.IO.Directory.CreateDirectory(sDir);
                if (System.IO.File.Exists(sPath))
                {
                    return;
                }
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    //创建类型声明节点
                    XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    xmlDoc.AppendChild(node);
                    xmlDoc.Save(sPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "CreateNewXML");
            }
        }

        /// <summary>  
        /// 创建节点  
        /// </summary>  
        /// <param name="xmldoc"></param>  xml文档
        /// <param name="parentnode"></param>父节点  
        /// <param name="name"></param>  节点名
        /// <param name="value"></param>  节点值
        /// 
        public static void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static int GetValFromNode(XmlElement childNode, string Name, int value)
        {
            try
            {
                if (childNode == null) return value;
                if (childNode.HasAttribute(Name))
                    return Convert.ToInt32(childNode.GetAttribute(Name));
                else
                    return value;
            }
            catch
            {
                return value;
            }
        }
        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static float GetValFromNode(XmlElement childNode, string Name, float value)
        {
            try
            {
                if (childNode == null) return value;

                if (childNode.HasAttribute(Name))
                    return Convert.ToSingle(childNode.GetAttribute(Name));
                else
                    return value;
            }
            catch
            {
                return value;
            }
        }
        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static string GetValFromNode(XmlElement childNode, string Name)
        {
            return GetValFromNode(childNode, Name, "");
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static string GetValFromNode(XmlElement childNode, string Name, string value)
        {
            try
            {
                if (childNode == null) return value;

                if (childNode.HasAttribute(Name))
                    return childNode.GetAttribute(Name);
                else
                    return value;
            }
            catch
            {
                return value;
            }
        }
        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static bool GetValFromNode(XmlElement childNode, string Name, bool value)
        {
            try
            {
                if (childNode == null) return value;

                if (childNode.HasAttribute(Name))
                    return Convert.ToBoolean(childNode.GetAttribute(Name));
                else
                    return value;
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static DateTime GetValFromNode(XmlElement childNode, string Name, DateTime value)
        {
            try
            {
                if (childNode == null) return value;

                if (childNode.HasAttribute(Name))
                    return Convert.ToDateTime(childNode.GetAttribute(Name));
                else
                    return value;
            }
            catch
            {
                return value;
            }
        }
        /// <summary>
        /// 返回节点的属性值
        /// </summary>
        /// <param name="childNode">节点</param>
        /// <param name="Name">属性名称</param>
        /// <param name="value">默认值</param>
        /// <returns>如果属性值存在，返回属性值，如果不存在，返回默认值</returns>
        public static bool SetValToNode(XmlElement childNode, string Name, string value)
        {
            try
            {
                if (childNode == null)
                    return false;
                else
                    childNode.SetAttribute(Name, value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
