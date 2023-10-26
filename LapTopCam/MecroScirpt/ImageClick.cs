using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTopCam.MecroScirpt
{
    internal class ImageClick : MecroMgr
    {

        //ImageClick
        public override bool AutoRun(ref AutoMecro _mecro)
        {

            bool check = resultfunc(_mecro.SearchImg(GetFullPath() + "image.png"));
            if(check == false)
            {
                check = resultfunc(_mecro.SearchImg(GetFullPath() + "imagge2.png"));
            }

            return check;

        }
    }
}
