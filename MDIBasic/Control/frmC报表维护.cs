using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CABC;
using CWLReport;

namespace LSSCADA.Control
{
    public partial class frmC报表维护 : Form
    {
        string sCol = "key;;RptPath;RptType;AutoType;StartTime;iRows;iCols;CreateTime;ModifyTime;FilePath";
        string bCol = "110000";
        private System.Windows.Forms.RichTextBox richText1;
        public string sFilePath = "";
        public CWLRptManager nWLRptM = new CWLRptManager();
        CWLRptDir nWLRpt = new CWLRptDir();
        List<CWLRpt> LSelRpt = new List<CWLRpt>();
        CWLRptDir nSelDir;
        public frmC报表维护()
        {
            InitializeComponent();
        }

        public frmC报表维护(Form _Owner)
        {
            InitializeComponent();

            this.MdiParent = _Owner;
            richText1 = ((frmMain)_Owner).richText1;

            nWLRptM = new CWLRptManager();
            nWLRptM.sPath = System.Environment.CurrentDirectory;
            nWLRptM.sFIle = System.Environment.CurrentDirectory + "\\TableInf.xml";
            string sMsg = "";

            nWLRptM.LoadFromXML(ref sMsg);
            if (sMsg.Length > 0)
                CABCDGV.InsertRichMsg(richText1, sMsg, true);
            nWLRpt = nWLRptM.nRpt;
            CABCDGV.InitializeDGV(dGV1, sCol, bCol, true);
            Init();
            this.DoubleBuffered = true;
        }

        private void Init()
        {
            UpdateTV();
        }
        //更新树形列表
        private void UpdateTV()
        {
            try
            {
                tV1.Nodes.Clear();
                tV1.Nodes.Add(nWLRpt.key,nWLRpt.desc                                                                                                                                );
                AddNodeToTV(tV1.Nodes[0], nWLRpt);
                tV1.ExpandAll();
            }
            catch (Exception ex)
            {

            }
        }

        private void AddNodeToTV(TreeNode nNode, CWLRptDir ndir)
        {
            foreach (CWLRptDir nd in ndir.LsDir)
            {
                TreeNode nNode1 = new TreeNode(nd.desc);
                nNode1.Name = nd.key;
                nNode1.ImageIndex = 0;
                AddNodeToTV(nNode1, nd);
                nNode.Nodes.Add(nNode1);
            }
            /*foreach (CWLRpt nd in ndir.LsRpt)
            {
                TreeNode nNode1 = new TreeNode(nd.desc);
                nNode1.Name = nd.key;
                nNode1.ImageIndex = 1;
                nNode1.SelectedImageIndex = 1;
                nNode.Nodes.Add(nNode1);
            }
            */
        }

        private void tV1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            nSelDir = nWLRpt.GetDirByKey(node.Name);
            LSelRpt = nSelDir.GetRpt(chBChild.Checked);
            UpdatedGV1();
        }

        private void UpdatedGV1()
        {//string sCol = "desc;RptPath;RptType;AutoType;StartTime;iRows;iCols;CreateTime;ModifyTime;FilePath";
            dGV1.RowCount = LSelRpt.Count;
            for (int i = 0; i < LSelRpt.Count; i++)
            {
                DataGridViewRow nRow = dGV1.Rows[i];
                nRow.Cells["desc"].Value = LSelRpt[i].desc;
                nRow.Cells["RptPath"].Value = LSelRpt[i].RptPath;
                nRow.Cells["RptType"].Value = LSelRpt[i].RptType;
                nRow.Cells["AutoType"].Value = LSelRpt[i].AutoType;
                nRow.Cells["StartTime"].Value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", LSelRpt[i].StartTime);
                nRow.Cells["iRows"].Value = LSelRpt[i].iRows;
                nRow.Cells["iCols"].Value = LSelRpt[i].iCols;
                nRow.Cells["FilePath"].Value = LSelRpt[i].FilePath;
                nRow.Cells["CreateTime"].Value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", LSelRpt[i].CreateTime);
                nRow.Cells["ModifyTime"].Value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", LSelRpt[i].ModifyTime);
            }
        }
    }
}
