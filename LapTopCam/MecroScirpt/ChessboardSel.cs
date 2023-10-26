using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapTopCam.MecroScirpt
{
    internal class ChessboardSel : MecroMgr
    {

        //FileSelect (Chess_board)
        public override bool AutoRun(ref AutoMecro _mecro)
        {

            bool check = resultfunc(_mecro.SearchImg(GetFullPath() + "chessboardsvn.png"));
            if(check)
            {
                _mecro.DoubleClick();
            }

            return check;


        }
    }
}
