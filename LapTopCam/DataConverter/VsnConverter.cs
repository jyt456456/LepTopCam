using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LapTopCam.DataConverter
{
    public class Programs
    {
        public string Source { get; set; }
        public int BePacked { get; set; }
        public double FrameTime { get; set; }
        public string Vsnmd5 { get; set; }
        public Program Program { get; set; }

        public List<List<Rect>> GetRect()
        {
            List<List<Rect>> LLrect = new List<List<Rect>>();


            for(int i=0;  i < Program.Pages.Count; ++i)
            {
                List<Rect> Lrect = new List<Rect>();
                for(int j=0; j < Program.Pages[i].Regions.Count; ++i)
                {
                    Lrect.Add(Program.Pages[i].Regions[j].Rect);
                }
                LLrect.Add(Lrect);
            }


            return LLrect;
        }

        public void SetRect(List<List<Rect>> _rect)
        {
            for (int i = 0; i < Program.Pages.Count; ++i)
            {
                for (int j = 0; j < Program.Pages[i].Regions.Count; ++j)
                {
                    Program.Pages[i].Regions[j].Rect = _rect[i][j];
                }
            }

        }

        public void SetRect(int _xpos, int _ypos, int _width, int _height)
        {
            for (int i = 0; i < Program.Pages.Count; ++i)
            {
                for (int j = 0; j < Program.Pages[i].Regions.Count; ++j)
                {
                    Program.Pages[i].Regions[j].Rect.X = _xpos;
                    Program.Pages[i].Regions[j].Rect.Y = _ypos;
                    Program.Pages[i].Regions[j].Rect.Width = _width;
                    Program.Pages[i].Regions[j].Rect.Height = _height;
                }
            }
        }

    }

    public class Program
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public Information Information { get; set; }
        public List<Page> Pages { get; set; }
    }

    public class Information
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double Duration { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        [XmlIgnore]
        public DateTime CreateTime { get; set; }

        [XmlIgnore]
        public DateTime LastModifyTime { get; set; }

        [XmlElement("CreateTime")]
        public string CreateTimeString
        {
            get { return CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { CreateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }
        }

        [XmlElement("LastModifyTime")]
        public string LastModifyTimeString
        {
            get { return LastModifyTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { LastModifyTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }
        }
    }

    public class Page
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int VisibleOrNot { get; set; }
        public int Globle { get; set; }
        public int AppointDuration { get; set; }
        public int PlayOneTime { get; set; }
        public int LoopType { get; set; }
        public string BgColor { get; set; }
        public BgFile BgFile { get; set; }
        public BgPicture BgPicture { get; set; }
        public List<object> BgAudios { get; set; }
        public List<Region> Regions { get; set; }
    }

    public class BgFile
    {
        public int IsRelative { get; set; }
        public string FilePath { get; set; }
    }

    public class BgPicture
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Version { get; set; }
        public string BackColor { get; set; }
        public double Alhpa { get; set; }
        public int Duration { get; set; }
        public int BeGlaring { get; set; }
        public int IsNeedUpdate { get; set; }
        public int UpdateInterval { get; set; }
        public int MirrorOrHandstand { get; set; }
        public int PlayTimes { get; set; }
        public Effect Effect { get; set; }
        public InEffect InEffect { get; set; }
        public OutEffect OutEffect { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public FileSource FileSource { get; set; }
        public int ReserveAS { get; set; }
    }

    public class Effect
    {
        public int IsStatic { get; set; }
        public int StayType { get; set; }
    }

    public class InEffect : Effect
    {
        public int Type { get; set; }
        public int Time { get; set; }
        public int RepeatX { get; set; }
        public int RepeatY { get; set; }
        public int IsTran { get; set; }
    }

    public class OutEffect : Effect
    {
        public int Type { get; set; }
        public int Time { get; set; }
        public int RepeatX { get; set; }
        public int RepeatY { get; set; }
        public int IsTran { get; set; }
    }

    public class FileSource
    {
        public int IsRelative { get; set; }
        public string FilePath { get; set; }
    }

    public class Region
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int Show { get; set; }
        public int Layer { get; set; }
        public int Locked { get; set; }
        public int IsSameAnim { get; set; }
        public Rect Rect { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Rect
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BorderWidth { get; set; }
        public string BorderColor { get; set; }
    }

    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Version { get; set; }
        public string BackColor { get; set; }
        public double Alhpa { get; set; }
        public int Duration { get; set; }
        public int BeGlaring { get; set; }
        public int IsNeedUpdate { get; set; }
        public int UpdateInterval { get; set; }
        public int MirrorOrHandstand { get; set; }
        public int PlayTimes { get; set; }
        public Effect Effect { get; set; }
        public InEffect InEffect { get; set; }
        public OutEffect OutEffect { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public FileSource FileSource { get; set; }
        public int ReserveAS { get; set; }
    }


}
