using CABC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CWLReport
{
    public class CWLRptManager
    {
        public string sPath = "";
        public string sFIle = "";
        public CSysConfig nSysConfig = new CSysConfig();
        public CDBConfig nDBConfig = new CDBConfig();
        public CWLRptDir nRpt = new CWLRptDir();
        public void LoadFromXML(ref string sMsg)//读取窗口文件
        {
            sMsg = "";
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sFIle);

                string xpath = "root/SysConfig";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                if (childNode != null)
                    nSysConfig.LoadFromNode(childNode);

                xpath = "root/DBConfig";
                childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                if (childNode != null)
                    nDBConfig.LoadFromNode(childNode);

                xpath = "root/ListTree";
                childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                if (childNode != null)
                    nRpt. LoadRptFromNode(childNode);
            }
            catch (Exception ex)
            {
                sMsg = ex.Message;
            }
        }

       
    }
}
