using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LapTopCam
{
    public class RectTile
    {
        private int rx;
        private int ry;
        private SolidColorBrush rgb;
        private int rwidth;
        private int rheight;
        private int endxpos;
        private int endypos;
        private System.Windows.Media.Color color;
        private bool checkrect = false;
        
        //private Rectangle

        public int Rx { get => rx; set => rx = value; }
        public int Ry { get => ry; set => ry = value; }
        public SolidColorBrush Rgb { get => rgb; set => rgb = value; }
        public int Rwidth { get => rwidth; set => rwidth = value; }
        public int Rheight { get => rheight; set => rheight = value; }
        public System.Windows.Media.Color Color { get => color; set => color = value; }
        public bool Checkrect { get => checkrect; set => checkrect = value; }
        
        public int Endxpos { get => endxpos; set => endxpos = value; }
        public int Endypos { get => endypos; set => endypos = value; }
    }


    public class detectRect
    {

        private int xpos;
        private int ypos;
        private int endxpos;
        private int endypos;
        private int rwidth;
        private int rheight;
        private Visibility visib;

        public int Xpos { get => xpos; set => xpos = value; }
        public int Ypos { get => ypos; set => ypos = value; }
        public int Endxpos { get => endxpos; set => endxpos = value; }
        public int Endypos { get => endypos; set => endypos = value; }
        public Visibility Visib { get => visib; set => visib = value; }
        public int Rwidth { get => rwidth; set => rwidth = value; }
        public int Rheight { get => rheight; set => rheight = value; }
    }
}
