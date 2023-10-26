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
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;
using LapTopCam.DataConverter;
using System.Security.Policy;

namespace LapTopCam
{
    

    public class MainViewModel : MVbase.MVBase
    {

        #region 속성
        private string m_imgpath;
        private double m_width = 300;
        private double m_height = 300;
        private int selindex = 0;
        private int safeselindex = 0;
        private int selAutoStep = 0;

        private bool captured = false;

        public ICommand SelectChange { get; set; }
        public ICommand StepChangeCommand { get; set; }

        public ObservableCollection<string> m_cameraNameList { get; set; }
        public ObservableCollection<string> m_SafeBoxList { get; set; }

        public ObservableCollection<RectTile> m_canvasitems { get; set; }
        public ObservableCollection<detectRect> m_canvasdetect { get; set; }

        
        public ObservableCollection<string> m_AutoMecroStep { get; set; }


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
        public AutoMecro m_AutoMecro;
        private List<MecroScirpt.MecroMgr> m_MecroList;


        //영역 알고리즘
        private System.Windows.Media.Color detectcl;
        private List<detectRect> deterectList;
        private List<List<RectTile>> m_Rt;
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

        private int maxindex = 0;

        private string strmaxindex;
        

        public Command.Command btnCommand { get; set; }

        public Command.Command btnConnectCommand { get; set; }
        public Command.Command closecommand { get; set; }
        public Command.Command btnCailCommand { get; set; }
        public Command.Command btnSaveCalimgCommand { get; set; }
        public Command.Command btnGetFrameCommand { get; set; }
        
        public Command.Command btnSafeSelCommand { get; set; }
        public Command.Command btnSafeBoxCommand { get; set; }
        public Command.Command btnRunAutoCommand { get; set; }
        public Command.Command btnStepRunCommand { get; set; }




        bool is_start = false;

        public string Imgpath { get => m_imgpath; set => m_imgpath = value; }
        public double Width { get => m_width; set => m_width = value; }
        public double Height { get => m_height; set => m_height = value; }
        public WriteableBitmap Btimage { get => btimage; set => btimage = value; }
        public WriteableBitmap BtCalimage { get => btCalimage; set => btCalimage = value; }
 

        public double Recwidth { get => recwidth; set => recwidth = value; }
        public double RecHeight { get => recHeight; set => recHeight = value; }
        public int Selindex { get => selindex; set => selindex = value; }
        public int Safeselindex { get => safeselindex; set => safeselindex = value; }
        public int SelAutoStep { get => selAutoStep; set => selAutoStep = value; }
        public int Maxindex { get => maxindex; set => maxindex = value; }
        public string Strmaxindex { get => strmaxindex; set => strmaxindex = value; }

        #endregion


        #region 생성자
        public MainViewModel() 
        {

            //카메라 목록 불러오기
            simpleinit();

            //영역 검색용 초기화
            RectInit();
            
            //매크로
            Mecroinit();
            
        }

        
        private void RectInit()
        {

            m_canvasitems = new ObservableCollection<RectTile>();
            m_canvasdetect = new ObservableCollection<detectRect>();

            m_Rt = new List<List<RectTile>>();
            deterectList = new List<detectRect>();
            m_SafeBoxList = new ObservableCollection<string>();
            SelectChange = new GalaSoft.MvvmLight.Command.RelayCommand<object>(SelChange);
            btnSafeBoxCommand = new Command.Command(GetSafeBox);
            btnRunAutoCommand = new Command.Command(RunMecro);
            


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
           
            for(int h=0; h <15; ++h)
            {
                for(int i=0; i<50; ++i)
                {
                    m_Rt[h][i].Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#03f0fc");
                    m_Rt[h][i].Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");

                }
            }
            
           /*
            for (int i = 0; i < 25; ++i)
            {
                m_Rt[i][i].Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#03f0fc");
                m_Rt[i][i].Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");

            }
           */


            for (int h=10; h<11; ++h)
            {
                for(int i=60; i<100; ++i)
                {

                    m_Rt[h][i].Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#03f0fc");
                    m_Rt[h][i].Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");
                }

            }

            for (int h = 68; h < 70; ++h)
            {
                for (int i = 60; i < 100; ++i)
                {

                    m_Rt[h][i].Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#03f0fc");
                    m_Rt[h][i].Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");
                }

            }


            for (int h = 10; h < 70; ++h)
            {
                for (int i = 60; i < 65; ++i)
                {

                    m_Rt[h][i].Rgb = (SolidColorBrush)new BrushConverter().ConvertFrom("#03f0fc");
                    m_Rt[h][i].Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#03f0fc");
                }

            }



            for (int i=0; i<m_Rt.Count; ++i)
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
            




        }

        private void Mecroinit()
        {
            //AutoHotKey 연동
            btnStepRunCommand = new Command.Command(StepRun);
            m_AutoMecroStep = new ObservableCollection<string>();
            m_AutoMecro = new AutoMecro();
            string script = "MouseSearchScript";
            m_AutoMecro.LoadScript(script);
            m_MecroList = new List<MecroScirpt.MecroMgr>
            {
                
                new MecroScirpt.OepnClcikScr(), // 0
                new MecroScirpt.ChessboardSel(),  // 1
                new MecroScirpt.ImageClick(),  // 2
                new MecroScirpt.OepnClcikScr(),  // 3
                new MecroScirpt.WhiteSel(), // 4
                new MecroScirpt.ImageClick(),  // 5
                new MecroScirpt.OepnClcikScr(),  // 6
                new MecroScirpt.ResultSel(),   //7
                new MecroScirpt.ImageClick()   //8
                 

            };


            

            for(int i=0; i<9; ++i)
            {
                m_AutoMecroStep.Add(i.ToString());
            }
            

            //     SelectChange = new GalaSoft.MvvmLight.Command.RelayCommand<object>(SelChange);


        }

        #endregion




        #region 메서드
        private bool init_camera()
        {
            try
            {
                camDevices = new List<DsDevice>();
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

                MessageBox.Show("연결완료");

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

                    
                }
            }

            for(int i=0; i < images.Count; ++i)
            {
                frame = Cv2.ImRead(images[i]);
                m_matFrameList.Add(frame);                    
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

            deterectList.Clear();
            m_canvasdetect.Clear();
            m_SafeBoxList.Clear();

            bool xfind = false;

            DateTime dt = DateTime.Now;
            for (searchy = 0; searchy < 100;)
            {
                xfind = DetectiveSuccRect(xfind);
                xend = 100;
                yend = 100;
                searchx = 0;
            }


            DateTime enddt = DateTime.Now;

            Console.WriteLine(dt.Millisecond.ToString());
            Console.WriteLine(enddt.Millisecond.ToString());




            if (deterectList.Count < 1)
            {
                //Error? Safe X
            }
            else
            {

                double resultrh = 0;
                for(int i=0; i<deterectList.Count; ++i)
                {

        
                    
                    deterectList[i].Rwidth= (deterectList[i].Endxpos - deterectList[i].Xpos) * 10;
                    deterectList[i].Rheight = (deterectList[i].Endypos - deterectList[i].Ypos) * 10;
                    deterectList[i].Xpos *= 10;
                    deterectList[i].Ypos *= 10;
                    deterectList[i].Visib = Visibility.Visible;

                    double maxrh = deterectList[i].Rwidth * deterectList[i].Rheight;

                    if (maxrh > resultrh)
                    {
                        resultrh = maxrh;
                        maxindex = i;
                    }

                    m_canvasdetect.Add(deterectList[i]);
                    m_SafeBoxList.Add(i.ToString());
                  
                    

                }
               
                strmaxindex = maxindex.ToString();

                OnPropertyChanged("m_SafeBoxList");
                //OnPropertyChanged("Safeselindex");
                OnPropertyChanged("Strmaxindex");
              

            }

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
                                if(j== 99)
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
                            for (int k = 0; k < deterectList.Count; ++k)
                            {
                                if (deterectList[k].Xpos == searchx && deterectList[k].Endxpos == xend && deterectList[k].Endypos == i)
                                {
                                    if (succxend == false)
                                    {
                                        ++searchy;
                                    }

                                    return succxend;
                                }
                            }

                            resultrect.Xpos = searchx;
                            resultrect.Ypos = setysearch(searchx, xend, searchy);
                            resultrect.Endxpos = xend;
                            resultrect.Endypos = i;
                            deterectList.Add(resultrect);
                           // ++searchy;
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



        private void SelChange(object obj)
        {
            
            if (deterectList.Count < 1)
            {
                return;
            }

            SelectionChangedEventArgs args = obj as SelectionChangedEventArgs;
         
            if (args == null)
                return;

            var selecteditem = args.AddedItems[0];

            int curframe = Convert.ToInt32(selecteditem);

            m_canvasdetect.Clear();

            for (int i=0; i< deterectList.Count; ++i)
            {
                if (i == curframe)
                {
                    deterectList[i].Visib = Visibility.Visible;
                }
                    
                else
                {
                    deterectList[i].Visib = Visibility.Hidden;
                }
                    


                m_canvasdetect.Add(deterectList[i]);
            }
            
            OnPropertyChanged("m_canvasdetect");
            OnPropertyChanged("SelectChange");

        }

        private void StepRun()
        {
            Thread.Sleep(200);
            bool succ = m_MecroList[selAutoStep].AutoRun(ref m_AutoMecro);

            if (succ)
            {
                if (selAutoStep == 2)
                {
                    //캘리브레이션
                    //cal = true;
                }

                if (selAutoStep == 5)
                {

                    //White 연산

                    //사각형 찾기
                    GetSafeBox();

                    //vsn 파일 수정
                    Xmldeserialize();


                    //white = true;
                }



            }
        }


            /*
            private void FileMoDefiy()
            {
                string fullpath;
                string line;
                fullpath = System.Environment.CurrentDirectory + @"\vsn\컬러라이트 프로그램 테스트.vsn";
                XDocument xmlDoc = XDocument.Load(fullpath);
                var nodes = xmlDoc.Root.XPathSelectElements("//Programs//Program//Pages//Page//Regions//Region").ToList();

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Element("Name").Value.ToString().Equals("문서 창 1"))
                    {
                      string x =  nodes[i].Element("Rect").Element("X").Value;
                      string y = nodes[i].Element("Rect").Element("Y").Value;
                      string w = nodes[i].Element("Rect").Element("Width").Value;
                      string h = nodes[i].Element("Rect").Element("Height").Value;
                    }

                }

              //  xmlDoc.Save(fullpath);

            }
            */

            //Xml파싱
            private void Xmldeserialize()
        {
            /*
            string fullpath = Directory.GetCurrentDirectory() + "\\";
            DataConverter.Programs programs = new DataConverter.Programs();
            List<List<DataConverter.Rect>> VsnRect = new List<List<DataConverter.Rect>>();
            string path = fullpath + "vsn\\white.vsn";
            //읽기
            
            using (var sr = new StreamReader(path))
            {
                var xs = new XmlSerializer(typeof(DataConverter.Programs));
                programs = (DataConverter.Programs)xs.Deserialize(sr);
                
            }
            
            programs.SetRect(deterectList[maxindex].Xpos, deterectList[maxindex].Ypos, deterectList[maxindex].Rwidth, deterectList[maxindex].Rheight) ;


            //쓰기
            using (StreamWriter wr = new StreamWriter(path))
            {
                XmlSerializer xs = new XmlSerializer(typeof(DataConverter.Programs));
                xs.Serialize(wr, programs);
            }
            */


            
            string fullpath = Directory.GetCurrentDirectory() + "\\";
            string path = fullpath + "vsn\\white.vsn";
            XDocument xmlDoc = XDocument.Load(path);
            var nodes = xmlDoc.Root.XPathSelectElements("//Programs//Program//Pages//Page//Regions//Region").ToList();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Element("Name").Value.ToString().Equals("문서 창 1"))
                {
                    nodes[i].Element("Rect").Element("X").Value = deterectList[maxindex].Xpos.ToString();
                    string y = nodes[i].Element("Rect").Element("Y").Value = deterectList[maxindex].Ypos.ToString();
                    string w = nodes[i].Element("Rect").Element("Width").Value = deterectList[maxindex].Rwidth.ToString();
                    string h = nodes[i].Element("Rect").Element("Height").Value = deterectList[maxindex].Rheight.ToString();
                }

            }

             xmlDoc.Save(path);
            


        }


        private void RunMecro()
        {
            bool succ = false;
            bool cal = false;
            bool white = false;
            int failcnt = 0;
            int lastlevel = 0;
            int lastcnt = 0;            
            
            for(int i=0; i< m_MecroList.Count;)
            {
                Thread.Sleep(200);
                succ =  m_MecroList[i].AutoRun(ref m_AutoMecro);
              
                if(succ)
                {
                    if(i == 2 && cal == false)
                    {
                        //캘리브레이션
                        cal = true;

                    }

                    if(i == 5 && white == false)
                    {

                        //White 연산

                        //사각형 찾기
                        GetSafeBox();

                        //vsn 파일 수정
                        Xmldeserialize();


                        white = true;
                    }



                    
                    ++i;
                    lastlevel = i;
                    lastcnt = 0;
                }
                else
                { // 매크로 꼬임 방지
                    ++failcnt;
                    ++lastcnt;
                    if (lastlevel == 0)
                    {

                        if (failcnt >= 3)
                            break;


                       
                        continue;
                    }

                    if(failcnt >= 3 && lastlevel <= i)
                    {
                        failcnt = 0;
                        --i;
                    }
                    else if(failcnt >=3 && lastlevel > i)
                    {
                        ++i;
                    }


                    if (lastcnt >= 25)
                        break;
                }
            }
            




        }



        #endregion

    }

}
