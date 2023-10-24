using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;


using System.Windows.Media.Imaging;

namespace LapTopCam
{
    internal class Calibration
    {
        VideoCapture vcam;
        Mat frame;
        DispatcherTimer timer;
        bool is_init;
        bool is_intTimer;

        Calibration()
        {
        //    is_in
        }

        private bool init_camera()
        {
            try
            {
                vcam = new VideoCapture();
                vcam.FrameHeight = (int)frame.Height;
                vcam.FrameWidth = (int)frame.Width;

                frame = new Mat();
                return true;
            }
            catch {
                return false;
            }

        }

        private void initTimer(double interval_ms)
        {
            try
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(interval_ms);
                timer.Tick += new EventHandler(timer_tick);
            }
            catch { }
        }

        public void Run()
        {

        }

        private void timer_tick(object sender, EventArgs e) 
        {
            vcam.Read(frame);
            WriteableBitmap bt = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);


        }

    }
}
