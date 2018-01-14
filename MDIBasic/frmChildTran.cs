using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace LSSCADA
{
    public partial class frmChildTran : Form
    {
        public frmChildTran()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public frmChildTran(Form _Owner)
        {
            InitializeComponent();

            this.MdiParent = _Owner;
            LoadWins();
            this.DoubleBuffered = true;
        }

        public void LoadWins()
        {
            
        }
        public void DrawForms(Graphics dc)
        {
            
        }

       


        protected override void OnPaint(PaintEventArgs e)
        {
            
        }

       

        private void timer1_Tick(object sender, EventArgs e)
        {
            //this.Invalidate();
            //Rectangle rc = new Rectangle(new Point(158, 13), new Size(203, 93));
            //this.Invalidate(rc,true);
        }
    }
}
