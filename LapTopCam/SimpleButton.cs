using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace LapTopCam
{
    public class SimpleButton : Button
    {
        public Brush MouseOverColor
        {
            get { return (Brush)GetValue(MouseOverColorProperty); }
            set { SetValue(MouseOverColorProperty, value); }
        }

        public static readonly DependencyProperty MouseOverColorProperty =
            DependencyProperty.Register("MouseOverColor", typeof(Brush), typeof(SimpleButton),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush MouseOverBorderColor
        {
            get { return (Brush)GetValue(MouseOverBorderColorProperty); }
            set { SetValue(MouseOverBorderColorProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBorderColorProperty =
            DependencyProperty.Register("MouseOverBorderColor", typeof(Brush), typeof(SimpleButton),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));
    }
}
