using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoHotkey.Interop;

namespace LapTopCam
{
    public class AutoMecro
    {

        public AutoHotkeyEngine ahk;



        public AutoMecro()
        {
            ahk = AutoHotkeyEngine.Instance;
        }

        
        public void LoadScript(string _script)
        {
            ahk.LoadFile(_script + ".ahk");
        }

        public void SetPixel(double x,double y, double width, double height,int rang,string _bgrtoHex)
        {
            //Search(xstart, ystart, xend, yend, colorhex, rang)
            string temp = ahk.ExecFunction("SetData", x.ToString(), y.ToString(), (x + width).ToString(), (y + height).ToString(), rang.ToString(),_bgrtoHex);

            //SetData(_bgrtoHex, "colorhex");


        }


        public void SetData(string _ix, string _name)
        {
            ahk.SetVar(_name, _ix);
            
        }

        public int RunScript(string _script, string _funcion)
        {

            var result = ahk.ExecFunction(_funcion);
            
            if (result != null)  
            {
                if (result.Length != 0)
                {
                    try
                    {
                        return Convert.ToInt32(result);
                    }
                    catch
                    {

                    }
                }
                
                
            }

            return 9;


        }


        public string RunPixelSearh(string _outx, string _outy)
        {
           return ahk.ExecFunction("RunSearch", _outx, _outy);
        }


        public int GetValint(string _target)
        {

            return Convert.ToInt32(ahk.GetVar(_target));

        }

        public string GetVal(string _target)
        {
            return ahk.GetVar(_target);
        }

    }
}
