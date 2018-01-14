using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSSCADA
{
    public partial class frmMainTop : Form
    {
        public frmMainTop()
        {
            InitializeComponent();
        }

        public frmMainTop( Form _Owner)
        {
            InitializeComponent();
            this.MdiParent = _Owner;
            this.DoubleBuffered = true;

            this.StartPosition = FormStartPosition.Manual;
            this.Left = (int)0;
            this.Top = (int)0;
            this.Size = _Owner.ClientSize;
            this.Height = 200;
        }
    }
}
