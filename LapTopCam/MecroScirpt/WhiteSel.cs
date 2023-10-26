using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTopCam.MecroScirpt
{
    internal class WhiteSel : MecroMgr
    {

        //White File Open
        public override bool AutoRun(ref AutoMecro _mecro)
        {
            bool check =  resultfunc(_mecro.SearchImg(GetFullPath() + "whitesvn.png"));
            if (check)
            {
                _mecro.DoubleClick();
            }

            //to.Close();

            return check;

        }
    }
}
