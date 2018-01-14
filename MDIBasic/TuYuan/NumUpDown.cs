using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;

namespace LSSCADA
{
    class CNumUpDown
    {
        public NumericUpDown nNumUD;

        public Guid tyguid;
        public float x;
        public float y;
        public float w;
        public float h;
        public float Size;

        public CNumUpDown(Point PT, float _w, float _h, float _Size)
        {
            x = PT.X;
            y = PT.Y;
            w = _w;
            h = _h;
            Size = _Size;
        }
    }
}
