using CABC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CWLReport
{
    public class CDBConfig
    {
        public List<CDBParmeter> LsDB = new List<CDBParmeter>();

        public void LoadFromNode(XmlElement Node)
        {
            try
            {
                LsDB = new List<CDBParmeter>();
                foreach (XmlElement item in Node.ChildNodes)
                {
                    CDBParmeter ndb = new CDBParmeter();
                    ndb.Db_server = CABCXML.GetValFromNode(item, "Db_server", "");
                    ndb.Db_uid = CABCXML.GetValFromNode(item, "Db_uid", "");
                    ndb.Db_pwd = CABCXML.GetValFromNode(item, "Db_pwd", "");
                    ndb.Db_name = CABCXML.GetValFromNode(item, "Db_name", "");
                    LsDB.Add(ndb);
                }
            }
            catch (Exception e)
            {

            }
        }
    }

    //数据库连接参数
    public class CDBParmeter
    {
        public string Db_server = "";
        public string Db_uid = "";
        public string Db_pwd = "";
        public string Db_name = "";
    }
}
