using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SemiGC
{
    public partial class frmCycleAdd : Form
    {
        public frmCycleAdd()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int i = int.Parse(textBox1.Text);
                frmRecipe.iCycle = i;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmRecipe.iCycle = -1;
            this.Close();
        }
    }
}
