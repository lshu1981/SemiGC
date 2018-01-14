using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace SemiGC
{
    public partial class frmSubAdd : Form
    {
        public string SLayList = "";
        public frmSubAdd()
        {
            InitializeComponent();
        }

        public frmSubAdd(string sLay)
        {
            InitializeComponent();
            SLayList = sLay;
        }

        private void frmSubAdd_Load(object sender, EventArgs e)
        {
            textBox3.Text = SLayList;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty || textBox1.Text == "")
            {
                MessageBox.Show("名称不能为空", "错误");
                return;
            }
            foreach (CSubProgram nSub in frmRecipe.ListSubProgram)
            {
                if (nSub.Name == textBox1.Text)
                {
                    MessageBox.Show("名称 " +textBox1.Text + " 已经存在，请重新输入", "错误");
                    return;
                }
            }

            if (MessageBox.Show("是否保存子程序？", "保存",
             MessageBoxButtons.YesNo, MessageBoxIcon.Question,
             MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                return;
            }
            CSubProgram newSub = new CSubProgram();
            newSub.Name = textBox1.Text;
            newSub.Desc = textBox2.Text;
            newSub.sLayerList = textBox3.Text;
            frmRecipe.ListSubProgram.Add(newSub);

            string filePath = frmRecipe.sAppPath + @"\Project\Layer.xml";
            XmlDocument myxmldoc = new XmlDocument();
            myxmldoc.Load(filePath);

            string xpath = "root/SubProgramList";
            XmlElement ListNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            XmlElement nLayNode = myxmldoc.CreateElement("SubProgram"); // 创建根节点album
            nLayNode.SetAttribute("Name", textBox1.Text);
            nLayNode.SetAttribute("Desc", textBox2.Text);
            nLayNode.SetAttribute("sLayerList", textBox3.Text);
            ListNode.AppendChild(nLayNode);
            myxmldoc.Save(filePath);
            MessageBox.Show("保存成功", "成功");
            this.Close();
        }
    }
}
