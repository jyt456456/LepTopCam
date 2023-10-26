using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTopCam.MecroScirpt
{
    //결과파일 클릭
    internal class ResultSel : MecroMgr
    {
        public override bool AutoRun(ref AutoMecro _mecro)
        {

            bool check = resultfunc(_mecro.SearchImg(GetFullPath() + "filename.png"));
            if (check)
            {
                _mecro.DoubleClick();
            }

          //  _mecro.Close();

            return check;
        }
    }
}
