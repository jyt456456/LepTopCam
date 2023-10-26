using OpenCvSharp.Internal.Vectors;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTopCam.MecroScirpt
{
    internal class OepnClcikScr : MecroMgr
    {
        //FileOpen_1
        public override bool AutoRun(ref AutoMecro _mecro)
        {
            int result = Open1(ref _mecro, "Load.png");
            if(result == 1)
            {
                //이미지못찾음
                result =  Open1(ref _mecro, "Load2.png");
            }


            return resultfunc(result);





        }


        private int Open1(ref AutoMecro _mecro,string filename)
        {
            return _mecro.SearchImg(GetFullPath() + filename);
        }


    }
}
