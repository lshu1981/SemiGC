using CABC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CWLReport
{
    public class CSysConfig
    {
        public string AutoType = "N";//Y自动运行 非Y不自动运行
        public string SPassWord = "";//修改默认密码 密码匹配是 DDabcdHH    DD是当前的日期，两位，HH是当前的小时数，两位

        public CRptPath nModPath = new CRptPath();//模板数据存放的路径
        public CRptPath nRptPath = new CRptPath();//报表存放的路径

        public void LoadFromNode(XmlElement Node)
        {
            try
            {
                AutoType = CABCXML.GetValFromNode(Node, "AutoType", "N");
                SPassWord = CABCXML.GetValFromNode(Node, "SPassWord", "abcd");
                XmlElement Mod = (XmlElement)(Node.SelectSingleNode("Mod"));
                if (Mod != null)
                    nModPath.LoadFromNode(Mod);

                Mod = (XmlElement)(Node.SelectSingleNode("Rpt"));
                if (Mod != null)
                    nRptPath.LoadFromNode(Mod);
            }
            catch (Exception e)
            {

            }
        }

    }
    //路径管理
    public class CRptPath
    {
        public string PathType = "0";
        public string Path = "";
        public string LogPath = "";

        public void LoadFromNode(XmlElement Node)
        {
            try
            {
                PathType = CABCXML.GetValFromNode(Node, "PathType", "0");
                Path = CABCXML.GetValFromNode(Node, "Path", "");
                LogPath = CABCXML.GetValFromNode(Node, "LogPath", "");
            }
            catch (Exception e)
            {

            }
        }
    }
}
