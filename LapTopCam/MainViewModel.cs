using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
//using DirectShowLib;
using System.ComponentModel;
using System.Windows;
using System.DirectoryServices.ActiveDirectory;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using System.Runtime.CompilerServices;
using OpenCvSharp.Flann;
using GalaSoft.MvvmLight.Messaging;
using System.Numerics;
using System.Runtime.Intrinsics;
using OpenCvSharp.ML;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Windows.Documents;
using System.Threading;

using AutoHotkey.Interop;
using GalaSoft.MvvmLight.Helpers;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Markup.Localizer;
using System.Security.Cryptography.X509Certificates;
using DirectShowLib;

namespace LapTopCam
{
    public class MainViewModel : INotifyPropertyChanged
    {

        #region 속성
        private string m_imgpath;
        private double m_width = 300;
        private double m_height = 300;
        private int selindex = 0;


        private bool captured = false;

        public ICommand SelectChange { get; set; }

        public ObservableCollection<string> m_cameraNameList { get; set; }
        public ObservableCollection<string> m_SafeBoxList { get; set; }

        public ObservableCollection<RectTile> m_canvasitems { get; set; }

        private List<List<RectTile>> m_Rt;


        //카메라
        private OpenCvSharp.Size img_size;
        private VideoCapture vcam;
        private Mat frame;
        private Mat result;
        private Mat curimg;
        private DispatcherTimer timer;
        private WriteableBitmap btimage;
        private WriteableBitmap btCalimage;
        private List<WriteableBitmap> m_LbtimageSave;
        private List<Mat> m_matFrameList;
        
        private List<DirectShowLib.DsDevice> camDevices;
        
        private double[,] mtx = new double[3, 3], new_mtx = new double[3, 3];
        private double[] dist = new double[5];
        private Mat m_mtx, m_dist = new Mat(1, 5, MatType.CV_64FC1), m_new_mtx = Mat.Eye(3, 3, MatType.CV_64FC1);
        private OpenCvSharp.Rect roi = new OpenCvSharp.Rect();
        private Mat mapx = new Mat(), mapy = new Mat();
        private bool m_bCalibr = false;
        private bool m_brunning = false;

        private double recwidth = 100;
        private double recHeight = 100;


        // Mecro
        private AutoMecro m_AutoMecro;

        //영역 알고리즘
        private System.Windows.Media.Color detectcl;
        private List<detectRect> deterectList;
        int xrealstart = 0;
        int yrealstart = 0;
        int xend = 100;
        int yend = 100;
        int nextxstart = 0;
        int nextystart = 0;
        int searchx = 0;
        int searchy = 0;
        private int realStartPx = 0;
        private int realStartPy = 0;
        private bool bnextx = true;
        int nextx = 0;
        
        public Command.Command btnCommand { get; set; }

        public Command.Command btnConnectCommand { get; set; }
        public Command.Command closecommand { get; set; }
        public Command.Command btnCailCommand { get; set; }
        public Command.Command btnSaveCalimgCommand { get; set; }
        public Command.Command btnGetFrameCommand { get; set; }
        bool is_start = false;

        public string Imgpath { get => m_imgpath; set => m_imgpath = value; }
        public double Width { get => m_width; set => m_width = value; }
        public double Height { get => m_height; set => m_height = value; }
        public WriteableBitmap Btimage { get => btimage; set => btimage = value; }
        public WriteableBitmap BtCalimage { get => btCalimage; set => btCalimage = value; }
 

        public double Recwidth { get => recwidth; set => recwidth = value; }
        public double RecHeight { get => recHeight; set => recHeight = value; }
        public int Selindex { get => selindex; set => selindex = value; }
        #endregion


        #region 생성자
        public MainViewModel() 
        {

            //카메라 목록 불러오기
            simpleinit();

            //영역 검색용 초기화
            RectInit();

        }

        
        private void RectInit()
        {

            m_canvasitems = new ObservableCollection<RectTile>();
            
            m_Rt = new List<List<RectTile>>();
            deterectList = new List<detectRect>();
            //OpenCvSharp.Rect rect  = new OpenCvSharp.Rect(,);
            //System.Drawing.Rectangle rt = new System.Drawing.Rectangle();

            //System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#fc1703");
            SelectChange = new GalaSoft.MvvmLight.Command.RelayCommand<object>(SelChange);
            SolidColorBrush col = (SolidColorBrush)new BrushConverter().ConvertFrom("#fc1703");
            detectcl = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");
            for (int i=0; i< 100; ++i)
            {

               List<RectTile> tile = new List<RectTile>();
                for (int j=0; j<100; ++j)
                {
                    RectTile rt = new RectTile();
                    rt.Rx = j * 10;
                    rt.Ry = i * 10;
                    rt.Rwidth =  10;
                    rt.Rheight = 10;
                    rt.Rgb = col;
                    rt.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#fc1703");


                    tile.Add(rt);
                }
                m_Rt.Add(tile);

            }

            for(int h=50; h <70; ++h)
            {
                for(int i=0; i<50; ++i)
                {
                    m_Rt[h][i].Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#03f0fc");
                    m_Rt[h][i].Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");

                }
            }


            for(int h=10; h<85; ++h)
            {
                for(int i=60; i<100; ++i)
                {

                    m_Rt[h][i].Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#03f0fc");
                    m_Rt[h][i].Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");
                }

            }
            for(int i=0; i<m_Rt.Count; ++i)
            {
                for(int j=0; j < m_Rt[i].Count; ++j)
                {
                    m_canvasitems.Add(m_Rt[i][j]);
                }
            }



            //m_canvasitems
            OnPropertyChanged("m_canvasitems");


            GetSafeBox();

        }

        private void simpleinit()
        {
            timer = new DispatcherTimer();
            m_LbtimageSave = new List<WriteableBitmap>();
            m_matFrameList = new List<Mat>();
            timer.Interval = TimeSpan.FromMilliseconds(0.01);
            timer.Tick += new EventHandler(timer_simpletick);
            m_cameraNameList = new ObservableCollection<string>();

            btnConnectCommand = new Command.Command(ConnCam);
            btnCommand = new Command.Command(oldRun);
            closecommand = new Command.Command(Closing);
            btnCailCommand = new Command.Command(CalVer4);
            btnSaveCalimgCommand = new Command.Command(SaveCalimage);
            //btnCailCommand = new Command.Command(ImgSearchMecro);

            //  btnGetFrameCommand = new Command.Command(Emtpy);

            init_camera();
            img_size = new OpenCvSharp.Size(m_width, m_height);


            m_cameraNameList.Clear();
            for (int i = 0; i < camDevices.Count; ++i)
            {
                m_cameraNameList.Add(camDevices[i].Name);
            }

            if (m_cameraNameList.Count >= 1)
            {
                OnPropertyChanged("m_cameraNameList");
            }
            
            //AutoHotKey 연동
            m_AutoMecro = new AutoMecro();
            m_AutoMecro.LoadScript("MouseSearchScript");
            

        }
        #endregion




        #region 메서드
        private bool init_camera()
        {
            try
            {
                camDevices = new List<DsDevice>();
                //GetCameraList();
                GetListCam();
                return true;
            }
            catch
            {
                return false;
            }



        }

        /*
        private void GetCameraList() => camDevices.AddRange(from DsDevice dsDevice in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)
                                                            where !dsDevice.DevicePath.Contains("device:sw")
                                                            select dsDevice);
        */


        // 노트북WebCam이 1번으로 저장되어 Insert문으로 변경
        private void GetListCam()
        {
            /*camDevices.InsertRange(0, from DsDevice dsDevice in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)
                                      where !dsDevice.DevicePath.Contains("device:sw")
                                      select dsDevice);*/
            DsDevice[] ds = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (int i=0; i < DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice).Count(); ++i)
            {
                if(!ds[i].DevicePath.Contains("device:sw"))
                {
                    camDevices.Insert(0,ds[i]);
                }
                
                
            }
        }

        private void ConnCam()
        {
            try
            {
                if(vcam != null)
                {
                    if (m_brunning)
                    {
                        m_brunning = !m_brunning;
                        timer.IsEnabled = false;
                        timer.Stop();
                    }


                    vcam.Release();
                    vcam.Dispose();


                }

                vcam = VideoCapture.FromCamera(Selindex, VideoCaptureAPIs.ANY);
                vcam.FrameHeight = (int)Width;
                vcam.FrameWidth = (int)Height;

                curimg = new Mat();
                result = new Mat();
            }
            catch
            {
            }
        }

        public void oldRun()
        {
            if (m_brunning)
            {
                m_brunning = !m_brunning;
                timer.IsEnabled = false;
                timer.Stop();
            }
            else
            {
                if (vcam != null && !timer.IsEnabled)
                {

                    timer.IsEnabled = true;

                    timer.Start();
                    m_brunning = !m_brunning;
                }
                else
                    return;

            }
            

        }


        private void timer_simpletick(object sender, EventArgs e)
        {
            // 0번 장비로 생성된 VideoCapture 객체에서 frame을 읽어옴
            Mat img;
            vcam.Read(curimg);
            img = curimg;
            if (m_bCalibr)
            {

                Mat temp;
                imgToCal(img, out temp);
                // 읽어온 Mat 데이터를 Bitmap 데이터로 변경
                BtCalimage = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(temp);                
                Btimage = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(img);
            }
            else
            {
                Btimage = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(img);
            }
            OnPropertyChanged("BtCalimage");
            OnPropertyChanged("Btimage");
        }
       
        private void SaveImage(Mat img,int icnt)
        {

           m_imgpath = Directory.GetCurrentDirectory() + "\\";

            Cv2.ImWrite(System.IO.Path.Combine(m_imgpath, string.Format("img_{0:D4}.png", icnt)), img);
        }


        private void Closing()
        {
            
            if(vcam != null)
            {
                vcam.Release();
                vcam.Dispose();

            }
        }



        private void GetImageList()
        {

            List<string> images = new List<string>();
            string path = Directory.GetCurrentDirectory() + "\\";
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".png") == 0)
                {
                    //String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                    images.Add(File.FullName);

                    //MessageBox.Show(FullFileName + " " + FileNameOnly);
                }
            }

            //btimage = new WriteableBitmap();
            for(int i=0; i < images.Count; ++i)
            {
                frame = Cv2.ImRead(images[i]);
                //btimage = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
                m_matFrameList.Add(frame);
                    //  btimage = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
            }
        }


        private void SaveCalimage()
        {
            //저장 경로 : 실행파일위치
            if(m_brunning == false)
            {
                Mat img = new Mat();
                Mat gray = new Mat();
                ChessboardFlags flags = ChessboardFlags.AdaptiveThresh | ChessboardFlags.FastCheck | ChessboardFlags.NormalizeImage;
                Point2f[] corners;
                //success = Cv2.FindChessboardCorners(gray, chess_board_size, OutputArray.Create(corner_pts), flags);
                int chess_board_countX = 9, chess_board_countY = 6;
                OpenCvSharp.Size chess_board_size = new OpenCvSharp.Size(chess_board_countX, chess_board_countY);

                for (int i = 0; i < 50; ++i)
                {
                    vcam.Read(curimg);
                    img = curimg;
                    Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);

                    if (Cv2.FindChessboardCorners(gray, chess_board_size, out corners))
                    {
                        SaveImage(img, i);
                    }
                    Btimage = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(img);
                    OnPropertyChanged("Btimage");
                    Thread.Sleep(50);
                }
                MessageBox.Show("이미지저장완료");
            }
            else
            {
                MessageBox.Show("Running중XXX");
            }

          
        }



        //현재Ver
        private void CalVer4()
        {
           
            List<string> images = new List<string>();
            string path = Directory.GetCurrentDirectory() + "\\";
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

            List<Point3f[]> object_points = new List<Point3f[]>();
            List<Point2f[]> image_points = new List<Point2f[]>();
            TermCriteria criteria = new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 30, 0.001);
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".png") == 0)
                {
                    //String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                    images.Add(File.FullName);

                    //MessageBox.Show(FullFileName + " " + FileNameOnly);
                }
            }

            int chess_board_countX = 9, chess_board_countY = 6;

            Point3f[] objarrp = new Point3f[chess_board_countX * chess_board_countY];

            for (int i = 0; i < chess_board_countX * chess_board_countY; ++i)
            {
                objarrp[i] = new Point3f((float)(i % chess_board_countX), (float)(i / chess_board_countX), 0.0f);
            }

            Mat frame = new Mat(), gray = new Mat();

            /* vector to store the pixel coordinates of detected checker board corners */
            List<Point2f> corner_pts = new List<Point2f>();     // 검출된 check board corner 포인트(2D 좌표)를 담을 벡터 선언
            bool success;                                 // findChessboardCorners 되었는지 안 되었는지를 확인하기 위한 용도
            char[] chars = new char[256];
            int index = 0;


            OpenCvSharp.Size chess_board_size = new OpenCvSharp.Size(chess_board_countX, chess_board_countY);

            for (int i = 0; i < images.Count; ++i)             // 받아온 이미지들은 images에 경로가? 저장되어있음. 한장씩 불러온다		// 매번 이미지에 대해 CHESSBOARD를 그리고 창 띄우기
            {
                frame = Cv2.ImRead(images[i]);  // images로부터 한 장씩 받아서 frame으로 읽어옴
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);      // COLOR 니까 GRAY로 바꾸기 위해 cvtColor 함수를 사용해 변경

                /* Finding checker board corners */
                /* If desired number of corners are found in the image then success = true */
                /*success = cv::findChessboardCorners(gray, cv::Size(CHECKERBOARD[0],
                    CHECKERBOARD[1]), corner_pts, CV_CALIB_CB_ADAPTIVE_THRESH | CV_CALIB_CB_FAST_CHECK | CV_CALIB_CB_NORMALIZE_IMAGE);*/
                ChessboardFlags flags = ChessboardFlags.AdaptiveThresh | ChessboardFlags.FastCheck | ChessboardFlags.NormalizeImage;
                success = Cv2.FindChessboardCorners(gray, chess_board_size, OutputArray.Create(corner_pts), flags);


                /* If desired number of vcorner are detected, we refine the pixel coordinates and display them on the images of checker board*/
                if (success)
                {
                    object_points.Add(objarrp);

                    Point2f[] corners2 =  Cv2.CornerSubPix(gray, corner_pts, new OpenCvSharp.Size(11, 11), new OpenCvSharp.Size(-1, -1), criteria);
                    Cv2.DrawChessboardCorners(frame, chess_board_size, corner_pts, success);

                    image_points.Add(corners2);
                    ++index;
                }


                if(index >= 30)
                {
                    break;
                }

                Cv2.ImShow("Image", frame);			// 해당 번째의 이미지에 대해 CHESSBOARD CORNER 그린걸 창에 띄움
                btimage = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
                

                //Cv2.WaitKey(0);						// 특별한 입력 있을 때까지 대기
            }




          //  Mat cameraMatrix_right = new Mat(), distCoeffs_right = new Mat();
          ///  Mat[] rotation_vecs, translate_vecs;

            OpenCvSharp.Size graysize = new OpenCvSharp.Size(gray.Rows, gray.Cols);
            
            try
            {
                //double retval = Cv2.CalibrateCamera(object_points, image_points, frame.Size(), cameraMatrix_right, distCoeffs_right, out _, out _);
                double retval = Cv2.CalibrateCamera(object_points, image_points, img_size, mtx, dist, out _, out _);
                //Mat new_camera_matrix = Cv2.GetOptimalNewCameraMatrix(mtx, dist, frame.Size(), 1.0, frame.Size(), out roi);
                  new_mtx = Cv2.GetOptimalNewCameraMatrix(mtx, dist, img_size, 1.0, img_size, out roi);
            }
            catch
            {
                int asd = 0;
            }

            //double v = Cv2.CalibrateCamera(new[] { obj_pt }, imgpoints_right[0] as IEnumerable<Mat>, frame.Size(), cameraMatrix_right, distCoeffs_right, out rotation_vecs, out translate_vecs);
            //alibrateCamera(, , cv::Size(gray.rows, gray.cols), , , R_right, T_right);
            
            m_mtx = new Mat(3, 3, MatType.CV_64FC1, mtx);
            m_dist = new Mat(1, 5, MatType.CV_64FC1, dist);
            m_new_mtx = new Mat(3, 3, MatType.CV_64FC1, new_mtx);

            //new_mtx = Cv2.GetOptimalNewCameraMatrix(mtx, dist, img_size, 1.0, img_size, out roi);
            Cv2.InitUndistortRectifyMap(m_mtx, m_dist, new Mat(), m_new_mtx, img_size, MatType.CV_32FC1, mapx, mapy);


            m_bCalibr = true;


        }

        //현재이미지 Cal
        private void imgToCal(Mat src, out Mat dst)
        {
            Mat temp = new Mat(img_size, MatType.CV_8UC3);
            Cv2.Remap(src, temp, mapx, mapy, InterpolationFlags.Linear, BorderTypes.Constant, new Scalar(0, 0, 255)); //  insted of undistort
            temp.Rectangle(roi, Scalar.Blue, 3);
            dst = temp;
            
        }








        


        //Safe박스 생성
        private void GetSafeBox()
        {
            searchx = 0;
            searchy = 0;
            realStartPx = 0;
            realStartPy = 0;

            int recnt = 0;
            bool xfind = false;
            for (searchy = 0; searchy < 99;)
            {
                xfind = DetectiveSuccRect(xfind);
                xend = 100;
                yend = 100;
                searchx = 0;
            }
            
            
            


            if (deterectList.Count < 1)
            {
                //Error 없음
            }
            else
            {


                for(int i=0; i<deterectList.Count; ++i)
                {
                    RectTile rtle = new RectTile();
                    rtle.Rx = deterectList[i].Xpos * 10;
                    rtle.Ry = deterectList[i].Ypos * 10;
                    rtle.Rwidth = (deterectList[i].Endxpos - deterectList[i].Xpos) * 10;
                    rtle.Rheight = (deterectList[i].Endypos - deterectList[i].Ypos) * 10;
                    rtle.Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#f511a1");
                    rtle.Checkrect = true;
                    m_canvasitems.Add(rtle);
                }
                

            }

            //


        }


        //검색 알고리즘
        private bool DetectiveSuccRect(bool _xfind) // xstart
        {
            detectRect resultrect = new detectRect();
            bool xstartcheck = false;
            bool succxend = false;
            //int nextxindex = 0;
            if (_xfind)
            {
                searchx = nextx;
            }
            else
            {
                searchx = 0;
            }
            //xend = 100;

            for (int i = searchy; i < yend; ++i)
            {
                for (int j = searchx; j < xend; ++j)
                {
                    //if(bp1 == false && m_Rt[i][j].Rgb == )


                    if (m_Rt[i][j].Color == detectcl)
                    {

             
                            if (searchx == j && i == searchy)
                            {
                                if(j== 100)
                                {
                                //찾기실패
                                ++searchy;
                                return succxend;
                                }

                                ++searchx;
                                xstartcheck = true;
                                
                                continue;
                            }

                        



                        if (i == searchy)
                        {
                            //xend = 파랑색
                            // next find return searchx
                            succxend = true;
                            xend = j;
                            nextx = xend;
                        }
                        else // 다음줄 파랑색일경우
                        {
                            resultrect.Xpos = searchx;
                            resultrect.Ypos = realStartPy;
                            resultrect.Endxpos = xend;
                            resultrect.Endypos = i;
                            deterectList.Add(resultrect);
                             
                            //resultrect = new detectRect();
                            searchy = i;
                           
                            return succxend;
                        }


                    }
                    else
                    {
                        if(xstartcheck)
                        {
                            searchx = j;
                            xstartcheck = false;
                        }
                    }
                }

            }


            for(int i=0; i< deterectList.Count; ++i)
            {
                if(deterectList[i].Xpos == searchx && deterectList[i].Endxpos == xend && deterectList[i].Endypos == yend)
                {
                    if(succxend == false)
                    {
                        ++searchy;
                    }
                    
                    return succxend;
                }
            }

            resultrect.Xpos = searchx;
            resultrect.Ypos = setysearch(searchx, xend, searchy);
            //resultrect.Ypos = searchy;
            resultrect.Endxpos = xend;
            resultrect.Endypos = yend;
            deterectList.Add(resultrect);
            if (succxend == false)
            {
                ++searchy;
            }
            return succxend;


        }


        private int setysearch(int xst, int endx, int endy)
        {
            bool succ = false;
            int resulty = 0;
            for(int i=0; i< endy; ++i)
            {
                for(int j=xst; j<endx; ++j)
                {
                    if (m_Rt[i][j].Color == detectcl)
                    {
                        resulty = i;
                        ++resulty;
                        break;
                    }

                }

            }
            return resulty;
        }

        private void ImgSearchMecro()
        {
            m_AutoMecro.RunScript("MouseSearchScript", "ImgSearch");
        }


        private void SelChange(object obj)
        {
            /*
            if (m_cameraNameList.Count < 1)
            {
                return;
            }

            SelectionChangedEventArgs args = obj as SelectionChangedEventArgs;
         
            if (args == null)
                return;

            string selecteditem = args.AddedItems[0].ToString();

            int curindex = 0;
            for (int i=0; i<m_cameraNameList.Count; ++i)
            {
                if(m_cameraNameList[i].Equals(selecteditem))
                {
                    curindex = i;
                    break;
                }
            }
            
            //args.
            //graph 변경
            //int curframe = Convert.ToInt32(selecteditem);
            OnPropertyChanged("SelectChange");
            */

        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
        }


        #endregion

    }

}
