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
        private double rx;
        private double ry;
        private SolidColorBrush rgb;
        private bool maker;
        private double rwidth;
        private double rheight;
        private System.Windows.Media.Color color;
        private bool checkrect = false;
        private Visibility visib = Visibility.Visible;
        //private Rectangle

        public double Rx { get => rx; set => rx = value; }
        public double Ry { get => ry; set => ry = value; }
        public SolidColorBrush Rgb { get => rgb; set => rgb = value; }
        public bool Maker { get => maker; set => maker = value; }
        public double Rwidth { get => rwidth; set => rwidth = value; }
        public double Rheight { get => rheight; set => rheight = value; }
        public System.Windows.Media.Color Color { get => color; set => color = value; }
        public bool Checkrect { get => checkrect; set => checkrect = value; }
        public Visibility Visib { get => visib; set => visib = value; }
    }


    public struct detectRect
    {

        private int xpos;
        private int ypos;
        private int endxpos;
        private int endypos;

        public int Xpos { get => xpos; set => xpos = value; }
        public int Ypos { get => ypos; set => ypos = value; }
        public int Endxpos { get => endxpos; set => endxpos = value; }
        public int Endypos { get => endypos; set => endypos = value; }
    }
}
