using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace LSSCADA
{
    public delegate void InvokeInsertText(string sCon, string sPortName, int sFun);
    public partial class Modbus报文查看 : Form
    {
        int[] FunNum = new int[4] { 0, 0, 0, 0 };
        public Modbus报文查看()
        {
            InitializeComponent();
        }

        public void InsertText(string sCon, string sPortName, int sFun)
        {
            if (richTextBox1.InvokeRequired)  //控件是否跨线程？如果是，则执行括号里代码
            {
                InvokeInsertText setListCallback = new InvokeInsertText(InsertText);   //实例化委托对象
                richTextBox1.BeginInvoke(setListCallback, sCon, sPortName, sFun);   //重新调用SetListBox函数
            }
            else
            {
                if (PauseShowMsg.Checked)
                    return;
                switch (sFun)
                {
                    case 1:
                        if (checkFun1.Checked)
                        {
                            FunNum[sFun - 1]++;
                            checkFun1.Text = FunNum[sFun - 1].ToString();
                            return;
                        }
                        break;
                    case 2:
                        if (checkFun2.Checked)
                        {
                            FunNum[sFun - 1]++;
                            checkFun2.Text = FunNum[sFun - 1].ToString();
                            return;
                        }
                        break;
                    case 3:
                        if (checkFun3.Checked)
                        {
                            FunNum[sFun - 1]++;
                            checkFun3.Text = FunNum[sFun - 1].ToString();
                            return;
                        }
                        break;
                    case 4:
                        if (checkFun4.Checked)
                        {
                            FunNum[sFun - 1]++;
                            checkFun4.Text = FunNum[sFun - 1].ToString();
                            return;
                        }
                        break;
                }

                int len1 = richTextBox1.Text.Length;
                string sTime = "";
                if (checkBox3.Checked)
                    sTime = DateTime.Now.ToString("HH:mm:ss");
                richTextBox1.AppendText(sTime + sPortName + ":" + sCon + "\r\n");
                int len2 = richTextBox1.Text.Length;

                if (len2 > len1)
                {
                    richTextBox1.Select(len1, len2 - len1 - 1);
                    if (sPortName == "Recv")
                        richTextBox1.SelectionColor = Color.Red;
                    else if (sPortName == "Send")
                        richTextBox1.SelectionColor = Color.Green;
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    richTextBox1.ScrollToCaret();
                }
                if (richTextBox1.Lines.Length > 1000)
                {
                    richTextBox1.Select(0, richTextBox1.Lines[0].Length + 1);
                    richTextBox1.SelectedText = "";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
    }
}
