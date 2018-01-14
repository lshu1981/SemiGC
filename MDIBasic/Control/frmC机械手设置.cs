using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSSCADA.Control
{
    public partial class frmC机械手设置 : Form
    {
        CProtcolTran CTran = new CProtcolTran();
        TextBox[] ListText = new TextBox[16];
//        List<TextBox> ListText = new List<TextBox>();
        string[] sSTN = new string[] { "R", "T", "Z", "LOWER", "SLOT", "PITCH" };
        public frmC机械手设置()
        {
            InitializeComponent();
        }

        public frmC机械手设置(CProtcolTran CT)
        {
            InitializeComponent();
            CTran = CT;
            ListText[0] = textSTN01;
            ListText[01] = textSTN02;
            ListText[02] = textSTN03;
            ListText[03] = textSTN04;
            ListText[04] = textSTN05;
            ListText[05] = textSTN06;
            ListText[06] = textSTN07;
            ListText[07] = textSTN08;
            ListText[08] = textSTN09;
            ListText[09] = textSTN10;
            ListText[10] = textSTN11;
            ListText[11] = textSTN12;
            ListText[12] = textSTN13;
            ListText[13] = textSTN14;
            ListText[14] = textSTN15;
            ListText[15] = textSTN16;
            //ClickRefresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4;j++ )
                {
                    if (ListText[i * 4 + j].ForeColor == Color.Red)
                    {
                        string str1 = "SET STN " +(i+1).ToString() .Trim();
                        str1 += " ALL " + ListText[i * 4 + 0].Text.Trim();
                        str1 += " " + ListText[i * 4 + 1].Text.Trim();
                        str1 += " " + ListText[i * 4 + 2].Text.Trim();
                        str1 += " " + ListText[i * 4 + 3].Text.Trim();
                        str1 += " 1 0\r\n";
                        CTran.SendAODO(str1);
                        CTran.SendHandAODO("RQ STN " + (i + 1).ToString().Trim() + " ARM A ALL\r\n");
                        break;
                    }
                }
            }
        }


        private void ExtramanGoto(object sender, EventArgs e)//机械手工位移动
        {
            Button nCon = (Button)sender;
            string str1 = "";
            switch (nCon.Name)
            {
                case "ExtraMove13": str1 = "MOVE Z REL 1"; break;
                case "ExtraMove14": str1 = "MOVE Z REL -1"; break;
                case "ExtraMove16": str1 = "MOVE R REL 1"; break;
                case "ExtraMove15": str1 = "MOVE R REL -1"; break;
                case "ExtraMove17": str1 = "MOVE T REL 0.1"; break;
                case "ExtraMove18": str1 = "MOVE T REL -0.1"; break;
            }
            //Mach.RTZ2Angle();
            if (str1.Length > 0)
            {
                CTran.SendAODO(str1 + "\r\n");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBoxPRT.Text = CTran.RTZ[0].ToString("0.000");
            textBoxPLT.Text = CTran.RTZ[1].ToString("0.000");
            textBoxPTT.Text = CTran.RTZ[2].ToString("0.000");
            if (CTran.bSTN)
            {
                RefreshSTN();
                CTran.bSTN = false;
            }
        }

        private void RefreshSTN()
        {
            foreach (TextBox nCon in ListText)
            {
                int i = Convert.ToInt32(nCon.Name.Substring(7));
                int i1 = (i - 1) / 4 + 1;
                int i2 = (i - 1) % 4;
                nCon.Text = CTran.STN[i1][i2].ToString("0.000");
                nCon.ForeColor = Color.Black;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClickRefresh();
        }
        private void ClickRefresh()
        {
            CTran.SendHandAODO("RQ STN 1 ARM A ALL\r\n");
            CTran.SendHandAODO("RQ STN 2 ARM A ALL\r\n");
            CTran.SendHandAODO("RQ STN 3 ARM A ALL\r\n");
            CTran.SendHandAODO("RQ STN 4 ARM A ALL\r\n");
        }

        private void textSTNChanged(object sender, EventArgs e)
        {
            TextBox nCon = (TextBox)sender;
            int i = Convert.ToInt32(nCon.Name.Substring(7));
            int i1 = (i - 1) / 4+1;
            int i2 = (i - 1) % 4;
            double dV = Convert.ToDouble(nCon.Text);
            if (dV != CTran.STN[i1][i2])
            {
                nCon.ForeColor = Color.Red;
            }
            else
            {
                nCon.ForeColor = Color.Black;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RefreshSTN();
        }
    }
}
