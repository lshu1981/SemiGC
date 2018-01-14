using CABC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CWLReport
{
   public class CWLRptDir
    {
        public string key = "";
        public string desc = "";
        public List<CWLRptDir> LsDir = new List<CWLRptDir>();
        public List<CWLRpt> LsRpt = new List<CWLRpt>();

        public List<CWLRpt> GetRpt(bool bchl)
        {
            List<CWLRpt> ls = new List<CWLRpt>();
            ls.AddRange(LsRpt);
            foreach (CWLRptDir dir in LsDir)
            {
                if(bchl)
                    ls.AddRange(dir.GetRpt(bchl));
            }
            return ls;
        }

        public CWLRptDir GetDirByKey(string skey)
        {
            if (this.key == skey)
                return this;
            foreach (CWLRptDir dir in LsDir)
            {
                if (dir.key == skey)
                    return dir;
                CWLRptDir ddir = dir.GetDirByKey(skey);
                if (ddir != null)
                    return ddir;
            }
            return null;
        }

        public void LoadRptFromNode(XmlElement Node)
        {
            try
            {
                key = System.Guid.NewGuid().ToString();
                desc = CABCXML.GetValFromNode(Node, "desc", "报表系统");
                foreach (XmlElement item in Node.ChildNodes)
                {
                    string skey = item.Name;
                    if (skey.ToLower().Substring(0, 3) == "dir")
                    {
                        CWLRptDir ndir = new CWLRptDir();
                        ndir.LoadRptFromNode(item);
                        ndir.key = System.Guid.NewGuid().ToString();
                        LsDir.Add(ndir);
                    }
                    if (skey.ToLower().Substring(0, 3) == "rpt")
                    {
                        CWLRpt nrpt = new CWLRpt();
                        nrpt.LoadFromNode(item);
                        nrpt.key = System.Guid.NewGuid().ToString();
                        nrpt.PDir = this;
                        LsRpt.Add(nrpt);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
